using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    public class BarCodeController : Controller
    {
        private readonly BarCodeDAL _objBarCodeDAL = new BarCodeDAL();

        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();

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

        #region "Barcode Print Section"
        [RoleFilter]
        public async Task<ActionResult> Barcode()
        {
            BarCodeModel model = new BarCodeModel();

            ViewBag.ProductStyleist = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_ID", "PRODUCT_STYLE");
            ViewBag.VendorList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetVendorListDropdown(), "VENDOR_ID", "VENDOR_NAME");

            return View(model);
        }

        public async Task<ActionResult> SendDataAndGetData(string productstyle)
        {
            ModelState.Clear();
            List<BarCodeModel> model = new List<BarCodeModel>();
            
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            //BarCodeModel model = new BarCodeModel();
            //List<BarCodeModel> model = new List<BarCodeModel>();
            //var   model = await _objBarCodeDAL.GetDataListByProductStyle(productstyle, _strWareHouseId, _strShopId);

            if (!string.IsNullOrEmpty(productstyle))
            {
                model = await _objBarCodeDAL.GetDataListByProductStyle(productstyle, _strWareHouseId, _strShopId);
            }
            //if (model.ProductStyle == null && model.ItemName == null)
            //{
            //    TempData["message"] = "ItemCode Not Found !! Enter Valid  ItemCode !!.";
            //}

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> PrintAllTableValue(List<BarCodeTableModel> objBarCodeModel)
        {
            List<string> itemCode = new List<string>();
            foreach (var data in objBarCodeModel)
            {
                if(data.VendorId > 0)
                {
                    itemCode.Add(data.ItemCode);
                    data.BarCodeImageString = "data:image/png;base64," + Convert.ToBase64String(UtilityClass.RenderBarcodeFor7Digit(data.ItemCode));
                    data.BarCodeImageArray = UtilityClass.RenderBarcodeFor7Digit(data.ItemCode);
                }
                else
                {
                    itemCode.Add(data.ItemCode);
                    data.BarCodeImageString = "data:image/png;base64," + Convert.ToBase64String(UtilityClass.RenderBarcode(data.ItemCode));
                    data.BarCodeImageArray = UtilityClass.RenderBarcode(data.ItemCode);
                }
            }

            if (objBarCodeModel.Any())
            {

                LoadSession();
                var deletemessage = await _objBarCodeDAL.DeletePreviousBarCodeDatafromTable();
                var message = await _objBarCodeDAL.SavePrintedBarCode(objBarCodeModel, _strEmployeeId, _strWareHouseId, _strShopId);
                //Session["ItemCode"] = itemCode;

            }
            
            return Json(objBarCodeModel, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}