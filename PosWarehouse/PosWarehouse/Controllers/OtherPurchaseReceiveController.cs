using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class OtherPurchaseReceiveController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly PurchaseReceiveDal _objPurchaseReceiveDal = new PurchaseReceiveDal();
        private readonly OtherPurchaseReceiveDal _objOtherPurchaseReceiveDal = new OtherPurchaseReceiveDal();
        private readonly ProductDal _objProductDalDal = new ProductDal();
        // GET: OtherPurchaseReceive
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
        public async Task<ActionResult> OtherPurchaseReceive()
        {
            OtherPurchaseReceiveModel model = new OtherPurchaseReceiveModel();
            ViewBag.VendorList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetVendorListDropdown(), "VENDOR_ID", "VENDOR_NAME");
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            return View(model);
        }
        [RoleFilter]
        public async Task<ActionResult> OtherPurchaseReceiveList()
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            OtherPurchaseReceiveModel model = new OtherPurchaseReceiveModel();
            var objOtherPurchaseReceiveModel = await _objOtherPurchaseReceiveDal.OtherPurchaseReceiveList();
            ViewBag.OtherPurchaseReceiveList = objOtherPurchaseReceiveModel;
            return View(model);
        }
        public async Task<ActionResult> GetProductInfoForScanReceive(string barcode)
        {
            var list = await _objPurchaseReceiveDal.GetProductInfoForScanReceive(barcode);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetMaxChallanNumber()
        {
            var maxChallanNumber = await _objOtherPurchaseReceiveDal.GetMaxChallanNumber();
            return Json(maxChallanNumber, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveAll(OtherPurchaseReceiveModel objOtherPurchaseReceiveModel)
        {
            LoadSession();
            string returnMessage = "";
            string OtherPurchaseReceiveNumber = "";
            //if (ModelState.IsValid)
            //{
            objOtherPurchaseReceiveModel.UpdateBy = _strEmployeeId;
            objOtherPurchaseReceiveModel.WareHouseId = _strWareHouseId;

            try
            {
                var strMessageData = await _objOtherPurchaseReceiveDal.SaveOtherPurchaseReceive(objOtherPurchaseReceiveModel);
                OtherPurchaseReceiveNumber = strMessageData.Item1;
                if (strMessageData.Item1 != null)
                {
                    foreach (var tableData in objOtherPurchaseReceiveModel.ReceiveItemsList)
                    {
                        tableData.OtherPurchaseReceiveNumber = strMessageData.Item1;
                        tableData.WareHouseId = _strWareHouseId;
                        returnMessage = await _objOtherPurchaseReceiveDal.SaveOtherPurchaseReceiveItemList(tableData);
                    }

                   
                        objOtherPurchaseReceiveModel.OtherPurchaseReceiveNumber = strMessageData.Item1;
                }
            }
            catch (Exception ex)
            {
                objOtherPurchaseReceiveModel.OtherPurchaseReceiveNumber = OtherPurchaseReceiveNumber;
                //throw new Exception("Error : " + ex.Message);
                await _objOtherPurchaseReceiveDal.DeleteMainAndItem(objOtherPurchaseReceiveModel);
            }

            // }
            var messageAndReload = new
            {
                OtherPurchaseReceiveNumber = OtherPurchaseReceiveNumber,
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("OtherPurchaseReceiveList")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        #region Report
        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly ReportDal _objReportDal = new ReportDal();

        public async Task<ActionResult> ShowReport(string OtherPurchaseReceiveNumber)
        {
            _objExportFormatType = ExportFormatType.PortableDocFormat;
            ExportOptions option = new ExportOptions();
            option.ExportFormatType = ExportFormatType.PortableDocFormat;
            string strPath = Path.Combine(Server.MapPath("~/Reports/POReceive/OtherPurchaseReceiveReport.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.OtherPurchaseReceiveNumberWiseDetails(OtherPurchaseReceiveNumber));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Clear();
            Response.Buffer = true;

            _objExportFormatType = ExportFormatType.PortableDocFormat;

            Stream oStream = _objReportDocument.ExportToStream(_objExportFormatType);
            byte[] byteArray = new byte[oStream.Length];
            oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));

            Response.ContentType = "application/pdf";

            string pFileDownloadName = "PurReceiveReport.pdf";

            Response.BinaryWrite(byteArray);
            Response.Flush();
            Response.Close();
            _objReportDocument.Close();
            _objReportDocument.Dispose();

            return File(oStream, Response.ContentType, pFileDownloadName);
        }

        #endregion
    }
}