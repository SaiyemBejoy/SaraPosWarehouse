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
    public class ShopOrderController : Controller
    {
        // GET: ShopOrder
        private readonly ShopOrderDal _objShopOrderDal = new ShopOrderDal();

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

        public async Task<ActionResult> Index()
        {
            var data = await _objShopOrderDal.ShopOrderList();
            ViewBag.ShopOrderList = data;
            return View();
        }

        public async Task<JsonResult> GetShopOrderItemList(int requisitionId)
        {
            var objShopOrderItemModel = await _objShopOrderDal.GetShopOrderItemList(requisitionId);
            return Json(objShopOrderItemModel, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopOrderInfoByRequisitionNum(string requisitionNumber)
        {
           var objShopOrderInfo = await _objShopOrderDal.ShopOrderListByRequisitionNum(requisitionNumber);
           if (objShopOrderInfo != null)
           {
               var requisitionId = objShopOrderInfo.RequisitionId;
               objShopOrderInfo.RequisitionDeliveryItemList = await _objShopOrderDal.GetShopOrderItemInfoDelivery(requisitionId);
           }
            return Json(objShopOrderInfo, JsonRequestBehavior.AllowGet);
        }
    }
}