using System;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class StoreDeliveryController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly StoreDeliveryDal _objStoreDeliveryDal = new StoreDeliveryDal();

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
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"] as string;
            }
            return View();
        }
        public ActionResult GetStoreDeliveryList(DataTableAjaxPostModel model)
        {
            LoadSession();
            StoreDeliveryGrid objStoreDeliveryGrid = new StoreDeliveryGrid
            {
                SearchBy = model.search?.value,
                //UpdateBy = _strEmployeeId,
                WareHouseId = _strWareHouseId,
                ShopId = _strShopId
            };

            if (model.order != null)
            {
                objStoreDeliveryGrid.OrderByDeliveryNumber = model.columns[model.order[0].column].data;
                // objStoreDeliveryGrid.OrderByDirection = model.order[0].dir.ToUpper();
            }

            List<StoreDeliveryGrid> data = _objStoreDeliveryDal.GetStoreDeliveryGrids(objStoreDeliveryGrid).ToList();

            int recordsFiltered = data.Count;
            int recordsTotal = data.Count;

            if (recordsTotal < model.length)
            {
                recordsTotal = model.length;
            }

            if (model.length == -1)
            {
                data = data.ToList();
            }
            else
            {
                data = data.Skip(model.start).Take(model.length).ToList();
            }

            return Json(new { model.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }
        [RoleFilter]
        public async Task<ActionResult> CreateOrEdit()
        {
            StoreDeliveryModel model = new StoreDeliveryModel();

            ViewBag.PurchaseReceiveNumberList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetPurchaseReceiveListDropdown(), "PURCHASE_RECEIVE_NUMBER", "PURCHASE_RECEIVE_NUMBER");
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.SeasonList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSeasonListDropdown(), "SEASON_ID", "SEASON_NAME");
            ViewBag.RegisterInfoList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetRegisterInfoListDropdown(), "REGISTER_ID", "REGISTER_PERSON_NAME");
            ViewBag.ShopRequisitionList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllShopRequisitionListDropDown(), "REQUISITION_NO", "REQUISITION_NO");
            ViewBag.MaxRequisitionNumber = await _objStoreDeliveryDal.MaxRequisitionNumber();
            return View(model);
        }      
        public async Task<ActionResult> GetDateWisePurchaseReceiveNumber(string purchaseDate)
        {
            var list = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetPurchaseReceiveListDropdown(purchaseDate), "PURCHASE_RECEIVE_NUMBER", "PURCHASE_RECEIVE_NUMBER");
            return Json(list, JsonRequestBehavior.AllowGet);
        }
       
        public async Task<ActionResult> GetProductItemDetailsByBarcode(string barcode)
        {

            var list = await _objStoreDeliveryDal.GetProductItemDetailsByBarcode(barcode);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ReceiveChallanWiseProductItem(string purchaseReceiveNumber)
        {
            List<ReceiveItem> model = new List<ReceiveItem>();
            LoadSession();
            if (!string.IsNullOrEmpty(purchaseReceiveNumber))
            {
                model = await _objStoreDeliveryDal.ReceiveChallanWiseProductItem(purchaseReceiveNumber, _strWareHouseId, _strShopId);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveAllStoreDeliveryItem(StoreDeliveryModel objStoreDeliveryModel)
        {
            string returnMessage = null;

            if (!ModelState.IsValid)
            {
                returnMessage = "Invalid Attempt, Please Try Again";
            }
            LoadSession();
            objStoreDeliveryModel.ReceiveChallanDelivery = objStoreDeliveryModel.ReceiveChallanDeliveryStatus ? "Y" : "N";
            objStoreDeliveryModel.UpdateBy = _strEmployeeId;
            objStoreDeliveryModel.WareHouseId = _strWareHouseId;
            objStoreDeliveryModel.ShopId = _strShopId;


            var saveMessage = await _objStoreDeliveryDal.StoreDeliverySave(objStoreDeliveryModel);

            if (saveMessage.Item2 !=null)
            {
                int deliveryShopId = objStoreDeliveryModel.DeliveryShopId;
                returnMessage = await _objStoreDeliveryDal.StoreDeliveryItemSave(objStoreDeliveryModel.ProductInventoryList, saveMessage.Item2, deliveryShopId);
            }

            var messageAndReload = new
            {
                deliveryNumber = saveMessage.Item2,
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("Index")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetStoreDeliveryItemList(string storeDeliveryNumber)
        {
            var objPurChaseReceiveItemModel = await _objStoreDeliveryDal.GetStoreDeliveryItemList(storeDeliveryNumber);
            return Json(objPurChaseReceiveItemModel, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetMaxChallanNumber()
        {
            var maxChallanNumber = await _objStoreDeliveryDal.GetMaxChallanNumber();
            return Json(maxChallanNumber, JsonRequestBehavior.AllowGet);
        }


        #region Report
        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly ReportDal _objReportDal = new ReportDal();

        public async Task<ActionResult> ShowReport(string storeDeliveryNo)
        {
            _objExportFormatType = ExportFormatType.PortableDocFormat;
            ExportOptions option = new ExportOptions();
            option.ExportFormatType = ExportFormatType.PortableDocFormat;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/StoreDeliveryDetails.rpt"));
            _objReportDocument.Load(strPath);
           
            DataSet objDataSet = (await _objReportDal.DeliveryNumberWiseDetails(storeDeliveryNo));

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

            string pFileDownloadName = "StoreDeliveryReport.pdf";

            Response.BinaryWrite(byteArray);
            Response.Flush();
            Response.Close();
            _objReportDocument.Close();
            _objReportDocument.Dispose();

            return File(oStream, Response.ContentType, pFileDownloadName);
        }

        public async Task<ActionResult> ShowReportGovtFormate(string storeDeliveryNo)
        {
            StoreDeliveryReport objStoreDeliveryReport = new StoreDeliveryReport();
            objStoreDeliveryReport.DeliveryNumber = storeDeliveryNo;

            _objExportFormatType = ExportFormatType.PortableDocFormat;
            ExportOptions option = new ExportOptions();
            option.ExportFormatType = ExportFormatType.PortableDocFormat;
            string strPath = Path.Combine(Server.MapPath("~/Reports/ShopDeliveryForGvt/ShopDeliveryGvt.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.DeliveryNumberWiseDetails(objStoreDeliveryReport.DeliveryNumber));

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

            string pFileDownloadName = "StoreDeliveryReport.pdf";

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