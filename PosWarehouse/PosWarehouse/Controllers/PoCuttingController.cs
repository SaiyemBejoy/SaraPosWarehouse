using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class PoCuttingController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly PurchaseReceiveDal _objPurchaseReceiveDal = new PurchaseReceiveDal();
        private readonly PoCuttingDal _objPoCuttingDal = new PoCuttingDal();
        // GET: POCutting
        #region "Common"

        private string _strEmployeeId = "";
        private string _strWareHouseId = "";
        private string _strShopId = "";

        public void LoadSession()
        {
            var auth = Session["authentication"] as AuthModel;

            if (auth != null)
            {
                _strEmployeeId = auth.EmployeeId;
                _strWareHouseId = auth.WareHouseId;
                _strShopId = auth.ShopId;
            }
            else
            {
                string url = Url.Action("Index", "Auth");
                if (url != null) Response.Redirect(url);
            }
        }

        #endregion

        [RoleFilter]
        public async Task<ActionResult> PoCuttingList()
        {
            LoadSession();
            PoCuttingModel model = new PoCuttingModel();
            var objPoCuttingModel = await _objPoCuttingDal.PoCuttingList(_strWareHouseId, _strShopId);
            ViewBag.PoCuttingList = objPoCuttingModel;
            return View(model);
        }
        [RoleFilter]
        public async Task<ActionResult> PoCutting()
        {
            PoCuttingModel model = new PoCuttingModel();
            ViewBag.PurchaseOrderList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetPurchaseOrderNumberDropdownDub(), "PURCHASE_ORDER_NUMBER", "PURCHASE_ORDER_NUMBER");
            return View(model);
        }
        public async Task<ActionResult> SearchProductByOrderNumber(string purchaseOrderNumber)
        {
            var list = await _objPurchaseReceiveDal.SearchProductByOrderNumber(purchaseOrderNumber);
            var  fabricList = await _objPoCuttingDal.SearchPoFabricByOrderNumber(purchaseOrderNumber);
            var data = new
            {
                listData = list,
                fabricData = fabricList
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SavePoCutting(PoCuttingModel objPoCuttingModel)
        {
            LoadSession();
            string returnMessage = "";
            //if (ModelState.IsValid)
            //{
            objPoCuttingModel.UpdateBy = _strEmployeeId;
            objPoCuttingModel.WareHouseId = _strWareHouseId;
            objPoCuttingModel.ShopId = _strShopId;

            try
            {
                var strMessageData = await _objPoCuttingDal.PoCuttingSave(objPoCuttingModel);
                if (strMessageData != null)
                {
                    try
                    {
                        int poCuttingId = Convert.ToInt32(strMessageData);
                        returnMessage = await _objPoCuttingDal.PoCuttingItemSave(objPoCuttingModel.PoCuttingItems, poCuttingId);
                    }
                    catch (Exception e)
                    {
                        int poCuttingId = Convert.ToInt32(strMessageData);
                        int deleteMessage = await _objPoCuttingDal.DeletePoCutting(poCuttingId, _strWareHouseId, _strShopId);
                        TempData["message"] = e + returnMessage;
                    }  
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error : " + ex.Message);
            }
            // }
            var messageAndReload = new
            {
                purchaseOrderNumber = objPoCuttingModel.PurchaseOrderNumber,
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("PoCuttingList")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }
    }
}