using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;
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
    public class MaterialPurchaseReportController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();

        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly MaterialPurchaseReportDal _objReportDal = new MaterialPurchaseReportDal();
        private readonly DataExchangeDal _objDataExchangeDal = new DataExchangeDal();


        public FileStreamResult ShowReport(string pReportType, string pFileDownloadName)
        {

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Clear();
            Response.Buffer = true;



            if (pReportType == "PDF")
            {
                _objExportFormatType = ExportFormatType.PortableDocFormat;

                Stream oStream = _objReportDocument.ExportToStream(_objExportFormatType);
                byte[] byteArray = new byte[oStream.Length];
                oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));

                Response.ContentType = "application/pdf";

                pFileDownloadName += ".pdf";

                Response.BinaryWrite(byteArray);
                Response.Flush();
                Response.Close();
                _objReportDocument.Close();
                _objReportDocument.Dispose();

                return File(oStream, Response.ContentType, pFileDownloadName);
            }
            else if (pReportType == "Excel")
            {
                _objExportFormatType = ExportFormatType.ExcelRecord;

                Stream oStream = _objReportDocument.ExportToStream(_objExportFormatType);
                byte[] byteArray = new byte[oStream.Length];
                oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));

                Response.ContentType = "application/vnd.ms-excel";

                pFileDownloadName += ".xls";

                Response.BinaryWrite(byteArray);
                Response.Flush();
                Response.Close();
                _objReportDocument.Close();
                _objReportDocument.Dispose();

                return File(oStream, Response.ContentType, pFileDownloadName);
            }
            else if (pReportType == "CSV")
            {
                _objExportFormatType = ExportFormatType.CharacterSeparatedValues;

                Stream oStream = _objReportDocument.ExportToStream(_objExportFormatType);
                byte[] byteArray = new byte[oStream.Length];
                oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));

                Response.ContentType = "text/csv";

                pFileDownloadName += ".csv";

                Response.BinaryWrite(byteArray);
                Response.Flush();
                Response.Close();
                _objReportDocument.Close();
                _objReportDocument.Dispose();

                return File(oStream, Response.ContentType, pFileDownloadName);
            }
            else if (pReportType == "TXT")
            {
                _objExportFormatType = ExportFormatType.RichText;

                Stream oStream = _objReportDocument.ExportToStream(_objExportFormatType);
                byte[] byteArray = new byte[oStream.Length];
                oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));

                Response.ContentType = "text/plain";

                pFileDownloadName += ".txt";

                Response.BinaryWrite(byteArray);
                Response.Flush();
                Response.Close();
                _objReportDocument.Close();
                _objReportDocument.Dispose();

                return File(oStream, Response.ContentType, pFileDownloadName);
            }

            return null;
        }

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

        // GET: MaterialPurchaseReport
        [RoleFilter]
        public async Task<ActionResult> Index()
        {
            LoadSession();
            var model = new PurchaseReportModelApi();

            ViewBag.MaterialTypeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMaterialTypeListDropdown(), "MATERIAL_TYPE_ID", "MATERIAL_TYPE_NAME");
            ViewBag.MaterialSubTypeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMaterialSubTypeListDropdown(), "MATERIAL_SUB_TYPE_ID", "MATERIAL_SUB_TYPE_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(PurchaseReportModelApi purchaseReportModelApi)
        {
            LoadSession();
            if (purchaseReportModelApi.ReportType == "PDF")
            {
                purchaseReportModelApi.ReportType = purchaseReportModelApi.ReportType;
                if (purchaseReportModelApi.RadioFor == "MP")
                {
                    await ErpApi(purchaseReportModelApi); 
                    await GenerateMeterialPurchaseReport(purchaseReportModelApi);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            ViewBag.MaterialTypeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMaterialTypeListDropdown(), "MATERIAL_TYPE_ID", "MATERIAL_TYPE_NAME");
            ViewBag.MaterialSubTypeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMaterialSubTypeListDropdown(), "MATERIAL_SUB_TYPE_ID", "MATERIAL_SUB_TYPE_NAME");

            return View(purchaseReportModelApi);
        }

        private async Task<int> GenerateMeterialPurchaseReport(PurchaseReportModelApi objPurchaseReportModelApi)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/MaterialPurchase/MaterialPurchase.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.MaterialPurchaseReport(objPurchaseReportModelApi));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objPurchaseReportModelApi.ReportType, "Material Purchase Report");
            return 0;
        }


        public async Task<ActionResult> GetMaterialSubType(int materialTypeId)
        {
            var list = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMaterialSubTypeListDropdown(materialTypeId), "MATERIAL_SUB_TYPE_ID", "MATERIAL_SUB_TYPE_NAME");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<string> ErpApi(PurchaseReportModelApi purchaseReportModelApi)
        {
            var result = await _objDataExchangeDal.ErpPurchaseMaterialReportApi(purchaseReportModelApi);
            return "0";
        }
    }
}