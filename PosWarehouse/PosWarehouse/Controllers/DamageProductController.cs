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

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class DamageProductController : Controller
    {
        private readonly DamageProductDal _objDamageProductDal = new DamageProductDal();
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
        public async Task<ActionResult> Index()
        {
            LoadSession();
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.MaxChallanNo = await _objDamageProductDal.GetMaxChallanNumber();
            return View();
        }
        [RoleFilter]
        public async Task<ActionResult> DamageList()
        {
            LoadSession();
            var damageList = await _objDamageProductDal.GetAllDamagelist();
            ViewBag.damageList = damageList;
            return View();
        }

        [RoleFilter]
        public async Task<ActionResult> DamageProductApprove()
        {
            LoadSession();
            var damageList = await _objDamageProductDal.GetAllDamagelistForApproval();
            ViewBag.damageList = damageList;
            return View();
        }

        [RoleFilter]
        public async Task<ActionResult> DamageProductReject()
        {
            LoadSession();
            var damageList = await _objDamageProductDal.GetAllRejectedList();
            ViewBag.damageList = damageList;
            return View();
        }

        public async Task<ActionResult> GetProductInfoByBarcode(string barcode)
        {
            var data = await _objStoreDeliveryDal.GetProductItemDetailsByBarcode(barcode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDamageProductItemList(string challanNo)
        {

            var objDamageProductItemModel = await _objDamageProductDal.GetDamageProductItem(challanNo);

            return Json(objDamageProductItemModel, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DamageChallanNoForApproval(string challanNo)
        {
            LoadSession();
            var returnMessage = "";
            returnMessage = await _objDamageProductDal.UpdateDamageProductByChallanNo(challanNo, _strEmployeeId);
            var messageAndReload = new
            {
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("DamageProductApprove")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> DamageChallanNoForReject(string challanNo, string rejectMessage)
        {
            LoadSession();
            var returnMessage = "";
            returnMessage = await _objDamageProductDal.RejectDamageProductByChallanNo(challanNo, rejectMessage, _strEmployeeId);
            var messageAndReload = new
            {
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("DamageProductApprove")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveAllDamageProduct(DamageProductModel objDamageProductModel)
        {
            string returnMessage = "";
            LoadSession();
            objDamageProductModel.CreatedBy = _strEmployeeId;
            var data = await _objDamageProductDal.SaveAllDamageProductMain(objDamageProductModel);
            if (data != null)
            {
                try
                {
                    foreach (var tableData in objDamageProductModel.DamageProductItemList)
                    {
                        tableData.DamageId = Convert.ToInt32(data);
                        returnMessage = await _objDamageProductDal.SaveAllDamageProductMainItem(tableData);

                    }
                }
                catch (Exception e)
                {
                    var deleteDamageProductMain = await _objDamageProductDal.DeleteDamageProductMain(data);
                }
              
            }
            var messageAndReload = new
            {
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("DamageList")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }


        #region Report
        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly ReportDal _objReportDal = new ReportDal();

        public async Task<ActionResult> ShowReport(string challanNum)
        {
            _objExportFormatType = ExportFormatType.PortableDocFormat;
            ExportOptions option = new ExportOptions();
            option.ExportFormatType = ExportFormatType.PortableDocFormat;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Damage/DamageProductDetails.rpt"));
            _objReportDocument.Load(strPath);

            DamageProductModel model = new DamageProductModel();
            model.DamageChallanNo = challanNum;

            DataSet objDataSet = (await _objReportDal.ChallanNumberWiseDamageDetails(model));

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