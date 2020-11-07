using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

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
                var requisitionAutoId = objShopOrderInfo.RequisitionAutoId;
               objShopOrderInfo.RequisitionDeliveryItemList = await _objShopOrderDal.GetShopOrderItemInfoDelivery(requisitionAutoId);
           }
            return Json(objShopOrderInfo, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateStoreDeliveryStatus(int requisitionId)
        {
            string message = await _objShopOrderDal.UpdateStoreDeliveryStatus(requisitionId);
            TempData["message"] = message;

            return RedirectToAction("Index");
        }

        #region Report
        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly ReportDal _objReportDal = new ReportDal();

        public async Task<ActionResult> ShowReport(int requAutoId)
        {
            _objExportFormatType = ExportFormatType.PortableDocFormat;
            ExportOptions option = new ExportOptions();
            option.ExportFormatType = ExportFormatType.PortableDocFormat;
            string strPath = Path.Combine(Server.MapPath("~/Reports/ShopOrder/ShopRequisition.rpt"));
            _objReportDocument.Load(strPath);

            RequisitionMainModel model = new RequisitionMainModel();
            model.RequisitionAutoId = requAutoId;

            DataSet objDataSet = (await _objReportDal.AutoIdWiseShopOrderDetails(model));

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

            string pFileDownloadName = "poReport.pdf";

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