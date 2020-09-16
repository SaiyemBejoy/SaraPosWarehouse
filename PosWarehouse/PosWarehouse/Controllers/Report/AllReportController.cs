using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ExcelReport;

namespace PosWarehouse.Controllers.Report
{
    [LogAction]
    public class AllReportController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();

        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly ReportDal _objReportDal = new ReportDal();
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
        
        // GET: AllReport
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

        #region Sale Summary
        [RoleFilter]
        public async Task<ActionResult> SaleSummary()
        {
            LoadSession();
            var saleDetailsSummary = new SaleDetailsSummary();
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
            ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");
            ViewBag.ProductStyleist = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_ID", "PRODUCT_STYLE");
            return View(saleDetailsSummary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaleSummary(SaleDetailsSummary saleDetailsSummary)
        {
            LoadSession();
            if (saleDetailsSummary.ReportType == "PDF")
            {
                ModelState.Remove("ShopId");
                saleDetailsSummary.ReportType = saleDetailsSummary.ReportType;
                if (saleDetailsSummary.RadioFor == "D")
                {
                   await GenerateSaleDetailsSummary(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "S")
                {
                    await GenerateSaleSummary(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "DS")
                {
                   await GenerateSaleSummaryDateWise(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "CSWS")
                {
                   await GenerateSaleSummaryCategorySubCategoryWise(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "CSWSWV")
                {
                    await GenerateSaleSummaryCategorySubCategoryWiseWithOutVat(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "ASCSS")
                {
                    await AllShopSaleSummaryCategorySubCategoryWisePro(saleDetailsSummary);
                    await GenerateAllShopSaleSummaryCategorySubCategoryWise(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "CWD")
                {
                    await GenerateSaleSummaryCategoryWiseDetails(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "IVD")
                {
                    await GenerateInvoiceWisDetails(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "DWSS")
                {
                    await GenerateDesignerWiseSaleSummary(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "SMSS")
                {
                    await GenerateSaleManWiseSaleSummary(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "SCC")
                {
                    await GenerateShopCPUCalculation(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "SWSSR")
                {
                    await GenerateStyleWiseSaleSummary(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "SWSS")
                {
                    await GenerateSizeWiseSaleSummary(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "GVDH")
                {
                    await GenerateGiftVoucherDepositSummary(saleDetailsSummary);
                }
            }
            else if (saleDetailsSummary.ReportType == "Excel")
            {
                ModelState.Remove("ShopId");
                if (saleDetailsSummary.RadioFor == "S")
                {
                    await ForExcleFormateSaleSummary(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "DS")
                {
                    await GenerateSaleSummaryDateWise(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "CSWS")
                {
                    await GenerateSaleSummaryCategorySubCategoryWise(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "SMSS")
                {
                    await GenerateSaleManWiseSaleSummary(saleDetailsSummary);
                }
                else if (saleDetailsSummary.RadioFor == "SWSS")
                {
                    await GenerateSizeWiseSaleSummary(saleDetailsSummary);
                }
            }

            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
            ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");
            ViewBag.ProductStyleist = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_ID", "PRODUCT_STYLE");
            return View(saleDetailsSummary);
        }

        private async Task<int> GenerateSaleDetailsSummary(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/SaleDetailsSummary.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.SaleDetailsSummary(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Sale Details Report");
            return 0;
        }

        private async Task<int> GenerateSaleSummary(SaleDetailsSummary objSaleDetailsSummary)
        {
            SaleSummaryPro(objSaleDetailsSummary).Wait();
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/SaleSummary.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = ( await _objReportDal.SaleSummary(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Sale Summary Report");
            return 0;
        }

        private async Task<int> GenerateSaleSummaryDateWise(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/SaleSummaryByDate.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = ( await _objReportDal.SaleSummaryDateWise(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Date Sale Summary Report");
            return 0;
        }

        private async Task<int> GenerateSaleSummaryCategorySubCategoryWise(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/SaleCategorySubCategoryWiseSummary.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.SaleSummaryCategorySubCategoryWise(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Date Sale Summary Report");
            return 0;
        }

        private async Task<int> GenerateSaleSummaryCategorySubCategoryWiseWithOutVat(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/SaleCategorySubCategoryWiseSummaryWVat.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.SaleSummaryCategorySubCategoryWiseWVat(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Sale Summary Report WithOut Vat");
            return 0;
        }
        private async Task<int> GenerateAllShopSaleSummaryCategorySubCategoryWise(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/AllShopCategorySubCateSaleSummary.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.AllShopSaleSummaryCategorySubCategoryWise(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "All Shop Sale Summary Report");
            return 0;
        }

        private async Task<int> GenerateSaleSummaryCategoryWiseDetails(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/CategoryWiseDetails.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.SaleCategoryWiseDetails(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Category Wise Details Report");
            return 0;
        }

        private async Task<int> GenerateInvoiceWisDetails(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/InvoiceWiseDetails.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.InvoiceWiseDetails(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Invoice Wise Details Report");
            return 0;
        }

        private async Task<int> GenerateDesignerWiseSaleSummary(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/DesignerWiseSale.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.DesignerWiseSaleSummary(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Designer Sale Report");
            return 0;
        }

        private async Task<int> GenerateSaleManWiseSaleSummary(SaleDetailsSummary objSaleDetailsSummary)
        {
            await SaveAndDeleteSaleManWiseSaleSummary(objSaleDetailsSummary);
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/SaleManWiseSaleSummary.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.SaleManWiseSaleSummary(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Sales Man Wise Sale Summary");
            return 0;
        }

        private async Task<int> GenerateShopCPUCalculation(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/CPUCalculation.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.ShopCPUCalculation(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Shop CPU Calculation");
            return 0;
        }

        private async Task<int> GenerateStyleWiseSaleSummary(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/StyleWiseSaleSummary.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.StyleWiseSaleSummaryRpt(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Style Wise Sale Summary");
            return 0;
        }

        private async Task<int> GenerateSizeWiseSaleSummary(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/SizeWiseSaleSummary.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.SizeWiseSaleSummaryRpt(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Size Wise Sale Summary");
            return 0;
        }
        private async Task<int> GenerateGiftVoucherDepositSummary(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/GiftVoucherDeposit.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.GiftVoucherDepositRpt(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Gift Voucher Deposit History");
            return 0;
        }
        #endregion

        #region PurchaseOrderForWareHouse 
        [RoleFilter]
        public async Task<ActionResult> PurchaseOrder()
        {
            LoadSession();
            var purchaseOrderReport = new PurchaseOrderReport();
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");

            return View(purchaseOrderReport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PurchaseOrder(PurchaseOrderReport purchaseOrderReport)
        {
            LoadSession();
            if (purchaseOrderReport.ReportType == "1")
            {
                ModelState.Remove("ShopId");
                purchaseOrderReport.ReportType = "PDF";
                if (purchaseOrderReport.RadioFor == "P")
                {
                    GeneratePoNumberWiseDetails(purchaseOrderReport);
                }
                if (purchaseOrderReport.RadioFor == "D")
                {
                    GenerateDateAndShopWiseDetails(purchaseOrderReport);
                }

            }
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");

            return View(purchaseOrderReport);
        }

        private void GeneratePoNumberWiseDetails(PurchaseOrderReport objPurchaseOrderReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/PurchaseOrderDetails.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (_objReportDal.PoNumberWiseDetails(objPurchaseOrderReport));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objPurchaseOrderReport.ReportType, "PO Number Wise Details Report");
        }
        private void GenerateDateAndShopWiseDetails(PurchaseOrderReport objPurchaseOrderReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/PurchaseOrderDetailsSummary.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (_objReportDal.DateAndShopWiseDetails(objPurchaseOrderReport));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objPurchaseOrderReport.ReportType, "PO Number Wise Details Report");
        }


        #endregion

        #region ShopDelivery   For GOVT FOrmate
        [RoleFilter]
        public async Task<ActionResult> StoreDelivery()
        {
            LoadSession();
            var shopDeliveryReport = new StoreDeliveryReport();
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");   
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            return View(shopDeliveryReport);
        } 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StoreDelivery(StoreDeliveryReport objDeliveryReport)
        {
            LoadSession();
            objDeliveryReport.ReportType = "PDF";
            if (objDeliveryReport.RadioFor == "CD")
            {
                await GenerateDeliveryNumberWiseDetails(objDeliveryReport);

            }else if (objDeliveryReport.RadioFor == "DSD")
            {
               await GenerateDateWiseStoreDeliveryChallan(objDeliveryReport);
            }
            else if (objDeliveryReport.RadioFor == "SWD")
            {
                await GenerateStyleWiseStoreDelivery(objDeliveryReport);
            }
            else if (objDeliveryReport.RadioFor == "SWLD")
            {
                await GenerateStyleWiselaunchDate(objDeliveryReport);
            }
            else if (objDeliveryReport.RadioFor == "SWPRH")
            {
                await GenerateStyleWisePurchaseReceive(objDeliveryReport);
            }
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            return View();
        }

        private async Task<int> GenerateDeliveryNumberWiseDetails(StoreDeliveryReport objDeliveryReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/ShopDeliveryForGvt/ShopDeliveryGvt.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet =await _objReportDal.DeliveryNumberWiseDetails(objDeliveryReport.DeliveryNumber);
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objDeliveryReport.ReportType, "PO Number Wise Details Report");
            return 0;
        }

        private async  Task<int> GenerateDateWiseStoreDeliveryChallan(StoreDeliveryReport objDeliveryReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/StoreDelivery/StoreDeliveryDatewise.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = await _objReportDal.DatewiseStoreDeliveryChallanDetails(objDeliveryReport);
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objDeliveryReport.ReportType, "Date Wise Challan Details");
            return 0;
        }

        private async Task<int> GenerateStyleWiseStoreDelivery(StoreDeliveryReport objDeliveryReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/StoreDelivery/StoreDeliveryStyleWise.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = await _objReportDal.StylewiseStoreDeliveryChallanDetails(objDeliveryReport);
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objDeliveryReport.ReportType, "Style Wise Challan Details");
            return 0;
        }

        private async Task<int> GenerateStyleWiselaunchDate(StoreDeliveryReport objDeliveryReport)
        {
            StyleWiseFristLaunchDatePro(objDeliveryReport).Wait();
            string strPath = Path.Combine(Server.MapPath("~/Reports/StoreDelivery/StyleWiseFristLaunchDate.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = await _objReportDal.StyleWiseFristLaunchDate(objDeliveryReport);
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objDeliveryReport.ReportType, "Style Wise Frist Launch Date");
            return 0;
        }

        public async Task<string> StyleWiseFristLaunchDatePro(StoreDeliveryReport objDeliveryReport)
        {
            var msg = await _objReportDal.StyleWiseFristLaunchDateSaveRpt(objDeliveryReport);
            return "o";
        }

        private async Task<int> GenerateStyleWisePurchaseReceive(StoreDeliveryReport objDeliveryReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/POReceive/StyleWisePurchaseReceiveHistory.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = await _objReportDal.StylewisePurchaseReceiveHistory(objDeliveryReport);
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objDeliveryReport.ReportType, "Style Wise Purchase Receive History");
            return 0;
        }

        #endregion

        #region Stock Report
        [RoleFilter]
        public async Task<ActionResult> StockSummary()
        {
            //await GetUpdateData();
            LoadSession();
            var stockSummary = new StockSummaryReport();
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_NAME", "CATEGORY_NAME");
            ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_NAME", "SUB_CATEGORY_NAME");
            ViewBag.ProductStyleist = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_ID", "PRODUCT_STYLE");
            ViewBag.ProductColor = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetColorForDropdown(), "ATTRIBUTE_VALUE_NAME", "ATTRIBUTE_VALUE_NAME");
            //var shopData = await _objDataExchangeDal.GetAllShopActiveShopInfo();
            //foreach (var shop in shopData)
            //{
            //    if (shop.ShopId >0)
            //    {
            //        await ShopStockDataSaveForRpt(shop.ShopId);
            //    }

            //}
            return View(stockSummary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StockSummary(StockSummaryReport stockSummaryReport)
        {
            LoadSession();
            if (stockSummaryReport.ReportType == "1")
            {
                stockSummaryReport.ReportType = "PDF";
                if (stockSummaryReport.ProductIdList != null)
                {
                    if (stockSummaryReport.ProductIdList.Count > 0)
                    {
                        for (int i = 0; i < stockSummaryReport.ProductIdList.Count; i++)
                        {
                            stockSummaryReport.ProductIds += stockSummaryReport.ProductIdList[i] + ",";
                        }
                        stockSummaryReport.ProductIds = stockSummaryReport.ProductIds.Remove(stockSummaryReport.ProductIds.Length - 1);
                    }
                }
                //if (stockSummaryReport.ShopId > 0)
                //{
                //    await ShopStockDataSaveForRpt(stockSummaryReport.ShopId);
                //}

                if (stockSummaryReport.RadioFor == "SR")
                {
                   await GenerateStockDetailsSummary(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "CR")
                {
                   await GenerateStockDetailsSummaryCategoryWise(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "SCR")
                {
                    await GenerateStockDetailsSummarySubCategoryWise(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "SWR")
                {
                    await GenerateStockDetailsSummaryByStyleName(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "BR")
                {
                    await GenerateStockDetailsSummaryByBarcode(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "CSSS")
                {
                   await GenerateCategorySubCategoryStyleSymmary(stockSummaryReport);
                }
                else if(stockSummaryReport.RadioFor == "SCWS")
                {
                    await GenerateSubCategorywiseSymmary(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "WHS")
                {
                    await GenerateStyleWiseWarehouseStockSymmary(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "WHD")
                {
                    await GenerateStyleWiseWarehouseStockDetails(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "SWH")
                {
                    await GenerateStyleWiseWarehouseHistory(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "SLS")
                {
                    await GenerateShopWiseLowStock(stockSummaryReport);
                }
                else if (stockSummaryReport.RadioFor == "SWLS")
                {
                    await GenerateStyleWiseShopLowStock(stockSummaryReport);
                }
            }
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_NAME", "CATEGORY_NAME");
            ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_NAME", "SUB_CATEGORY_NAME");
            ViewBag.ProductStyleist = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_ID", "PRODUCT_STYLE");
            ViewBag.ProductColor = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetColorForDropdown(), "ATTRIBUTE_VALUE_NAME", "ATTRIBUTE_VALUE_NAME");
            return View(stockSummaryReport);
        }
        private async Task<int> GenerateStockDetailsSummary(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/CurrentStock.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport.RadioForZero == "WOZ")
            {
                 objDataSet = (await _objReportDal.StockDetailsSummaryWithOutZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.StockDetailsSummary(objStockSummaryReport));
            }
           
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Stock Details Report");
            return 0;
        }
        private async Task<int> GenerateStockDetailsSummaryByBarcode(StockSummaryReport objStockSummaryReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/CurrentStockBarcodeWise.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = (await _objReportDal.StockDetailsSummaryByBarcode(objStockSummaryReport));
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Stock Details Report");
            return 0;
        }

        private async Task<int> GenerateStockDetailsSummaryCategoryWise(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/CurrentStockCategoryWise.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport.RadioForZero == "WOZ")
            {
                objDataSet = (await _objReportDal.StockDetailsSummaryByCategoryWithOutZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.StockDetailsSummaryByCategory(objStockSummaryReport));
            }
  
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Stock Report Category Wise");
            return 0;
        }

        private async Task<int> GenerateStockDetailsSummarySubCategoryWise(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/CurrentStockSubCategoryWise.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport .RadioForZero == "WOZ")
            {
                objDataSet = (await _objReportDal.StockDetailsSummaryBySubCategoryWithOutZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.StockDetailsSummaryBySubCategory(objStockSummaryReport));
            }
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Stock Report SubCategory Wise");
            return 0;
        }

        private async Task<int> GenerateStockDetailsSummaryByStyleName(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/CurrentStockStyleWise.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport.RadioForZero == "WOZ")
            {
                objDataSet = (await _objReportDal.StockDetailsSummaryByStyleNameWithoutZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.StockDetailsSummaryByStyleName(objStockSummaryReport));
            }
           
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Stock Report Style Wise");
            return 0;
        }

        private async Task<int> GenerateSubCategorywiseSymmary(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/SubCategoryWiseSummary.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport.RadioForZero == "WOZ")
            {
                objDataSet = (await _objReportDal.SubCategorywiseSymmaryWithoutZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.SubCategorywiseSymmary(objStockSummaryReport));
            }
            
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Subcategory symmary");
            return 0;
        }

        private async Task<int> GenerateCategorySubCategoryStyleSymmary(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/CurrentStockStyleWiseSummary.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport.RadioForZero  == "WOZ")
            {
                objDataSet = (await _objReportDal.StockCategorySubcategoryStyleWiseSummaryWithoutZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.StockCategorySubcategoryStyleWiseSummary(objStockSummaryReport));
            }
           
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Category SubCategory Details");
            return 0;
        }

        private async Task<int> GenerateShopWiseLowStock(StockSummaryReport objStockSummaryReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/ShopLowStock.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = (await _objReportDal.ShopWiseLowStock(objStockSummaryReport));
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Low Stock Summary");
            return 0;
        }
        private async Task<int> GenerateStyleWiseShopLowStock(StockSummaryReport objStockSummaryReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/StyleWiseShopLowStock.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = (await _objReportDal.ShopStyleWiseLowStock(objStockSummaryReport));
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Shop Wise Low Stock Summary");
            return 0;
        }

        private async Task<int> GenerateStyleWiseWarehouseStockSymmary(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/CurrentStyleWiseWarehouseStockSummary.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport.RadioForZero == "WOZ")
            {
                objDataSet = (await _objReportDal.StyleWiseWarehouseStockSummaryWithoutZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.StyleWiseWarehouseStockSummary(objStockSummaryReport));
            }
           
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Warehouse Stock Summary");
            return 0;
        }
        private async Task<int> GenerateStyleWiseWarehouseStockDetails(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/DCStyleWiseStockDetails.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport.RadioForZero == "WOZ")
            {
                objDataSet = (await _objReportDal.StyleWiseWarehouseStockDetailsWithoutZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.StyleWiseWarehouseStockDetails(objStockSummaryReport));
            }
           
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Warehouse Stock Details");
            return 0;
        }
        private async Task<int> GenerateStyleWiseWarehouseHistory(StockSummaryReport objStockSummaryReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/DCStyleWiseHistory.rpt"));
            _objReportDocument.Load(strPath);
            if (objStockSummaryReport.RadioForZero == "WOZ")
            {
                objDataSet = (await _objReportDal.StyleWiseWarehouseHistoryWithZero(objStockSummaryReport));
            }
            else
            {
                objDataSet = (await _objReportDal.StyleWiseWarehouseHistory(objStockSummaryReport));
            }
             
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objStockSummaryReport.ReportType, "Warehouse Stock Details");
            return 0;
        }

        public async Task<ActionResult> GetSubCategoryByCategoryName(string categoryName)
        {
               var list = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdownByName(categoryName), "SUB_CATEGORY_NAME", "SUB_CATEGORY_NAME");
              return Json(list, JsonRequestBehavior.AllowGet);
        }
      
        public async Task<ActionResult> GetUpdateData()
        {
            var result = "";
            var shopData = await _objDataExchangeDal.GetAllShopActiveShopInfo();
           // var deleteData = await _objReportDal.ShopStockDataDeleteForRpt(0);
            foreach (var shop in shopData)
            {
                if (shop.ShopId > 0)
                {
                  result =  await ShopStockDataSaveForRpt(shop.ShopId);
                  //var resultTr = await ShopStockDataSaveTrForRpt(shop.ShopId);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("StockSummary");
        }


        #endregion

        #region Periodical Stock Report

        public async Task<ActionResult> GetSubCategory(int categoryId)
        {
            var list = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(categoryId), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetStyleList(int categoryId, int subCategoryId)
        {
            var list = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetStyleListByCategorySubCategoryDropdown(categoryId, subCategoryId), "PRODUCT_ID", "PRODUCT_STYLE");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetBarcodeList(string styleName)
        {
            var list = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetBarcodeListByStyleNameDropdown(styleName), "PRODUCT_CODE", "PRODUCT_CODE");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [RoleFilter]
        public async Task<ActionResult> PeriodicalStock()
        {
            PeriodicalStockReportModel model = new PeriodicalStockReportModel();

            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.WarehouseName = UtilityClass.GetSelectListForWarehouseName(await _objDropdownDal.GetWarehouseListDropdown(), "WARE_HOUSE_ID", "WARE_HOUSE_NAME");
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
            ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_NAME", "SUB_CATEGORY_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PeriodicalStock(PeriodicalStockReportModel model)
        {
            if (model.ReportType == "PDF")
            {
               
                model.ReportType = "PDF";
                if (model.WarehouseId != null)
                {
                    await GenerateWarehousePeriodicalStockRpt(model);
                }
                else
                {
                    await GeneratePeriodicalStockRpt(model);
                }

            }
            else if (model.ReportType == "Excel")
            {
                model.ReportType = "Excel";
                if (model.WarehouseId != null)
                {
                    await GenerateWarehousePeriodicalStockRpt(model);
                }
                else
                {
                    await GeneratePeriodicalStockRpt(model);
                }
            }
                ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.WarehouseName = UtilityClass.GetSelectListForWarehouseName(await _objDropdownDal.GetWarehouseListDropdown(), "WARE_HOUSE_ID", "WARE_HOUSE_NAME");
            return RedirectToAction("PeriodicalStock");
        }

        private async Task<List<PeriodicalStockReportDataModel>> GetPeriodicalProductHistory()
        {
            var periodicalStockReport = new List<PeriodicalStockReportDataModel>();

            var getAllDeliveredItem = await _objReportDal.GetAllDeliveredProduct();

            //var getAllSaleItem = 

            return periodicalStockReport;
        }

        private async Task<int> GeneratePeriodicalStockReport(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/SaleSummaryByDate.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.SaleSummaryDateWise(objSaleDetailsSummary));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSaleDetailsSummary.ReportType, "Date Sale Summary Report");
            return 0;
        }

        //Sazzad vai er formate
        private async Task<int> GeneratePeriodicalStockRpt(PeriodicalStockReportModel objPeriodicalStockReportModel)
        {
            await PeriodicalStockRptPro(objPeriodicalStockReportModel);
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/PeriodicalStockDetails.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.PeriodicalStockRpt(objPeriodicalStockReportModel));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objPeriodicalStockReportModel.ReportType, "Periodical Stock Report");
            return 0;
        }

        private async Task<int> GenerateWarehousePeriodicalStockRpt(PeriodicalStockReportModel objPeriodicalStockReportModel)
        {
            await WarehousePeriodicalStockRptPro(objPeriodicalStockReportModel);
            string strPath = Path.Combine(Server.MapPath("~/Reports/Stock/DCPeriodicalStockDetails.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.WarehousePeriodicalStockRpt(objPeriodicalStockReportModel));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objPeriodicalStockReportModel.ReportType, "Periodical Stock Report");
            return 0;
        }


        #endregion

        #region Customer info Report
        [RoleFilter]
        public async Task<ActionResult> CustomerReport()
        {
            LoadSession();
            var customerReport = new CustomerReport();
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            return View(customerReport);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CustomerReport(CustomerReport customerReport)
        {
            LoadSession();
            if (customerReport.ReportType == "PDF")
            {
                
                if (customerReport.RadioFor == "PC")
                {
                   await GeneratePrivilegeCustomerDetails(customerReport);
                }
                else if (customerReport.RadioFor == "SC")
                {
                    await GenerateSaleCustomerDetails(customerReport);
                }
            }
            else if (customerReport.ReportType == "Excel")
            {
                if (customerReport.RadioFor == "PC")
                {
                    await GeneratePrivilegeCustomerDetails(customerReport);
                }
                else if (customerReport.RadioFor == "SC")
                {
                    await GenerateSaleCustomerDetails(customerReport);
                }
            }
            
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
             return View(customerReport);
        }

        private async Task<int> GeneratePrivilegeCustomerDetails(CustomerReport customerReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Customer/PrivilegeCustomer.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = (await _objReportDal.PrivilegeCustomerDetails(customerReport));
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(customerReport.ReportType, "Privilege Customer Details");
            return 0;
        }

        private async Task<int> GenerateSaleCustomerDetails(CustomerReport customerReport)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/Customer/SaleCustomerDetails.rpt"));
            _objReportDocument.Load(strPath);
            DataSet objDataSet = (await _objReportDal.SaleCustomerDetails(customerReport));
            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(customerReport.ReportType, "Sale Customer Details");
            return 0;
        }

        #endregion

        #region StockTransfer
        [RoleFilter]
        public async Task<ActionResult> StockTransfer()
        {
            LoadSession();
            var stockTransferDetails = new StockTransferDetails();
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            return View(stockTransferDetails);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StockTransfer(StockTransferDetails stockTransferDetails)
        {
            LoadSession();
            if (stockTransferDetails.ReportType == "1")
            {
                stockTransferDetails.ReportType = "PDF";
                if (stockTransferDetails.RadioFor == "DST")
                {
                   await GenerateDateWiseStockTransferDetails(stockTransferDetails);
                }

            }
            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            return View(stockTransferDetails);
        }

        private async Task<int> GenerateDateWiseStockTransferDetails(StockTransferDetails stockTransferDetails)
        {
            string strPath = Path.Combine(Server.MapPath("~/Reports/ReturnReceive/StockTransferDateWiseFromShop.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = await _objReportDal.StockTransferDetails(stockTransferDetails);

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(stockTransferDetails.ReportType, "Stock Transfer Details");

            return 0;
        }
        #endregion

        #region ReportProcedure

        public async Task<string> SaveAndDeleteSaleManWiseSaleSummary(SaleDetailsSummary objSaleDetailsSummary)
        {
            var saveReportData = "";
            var shopUrl = await _objDataExchangeDal.GetAShopUrlWithShopId(objSaleDetailsSummary.ShopId);
            if (shopUrl.ShopUrl != null)
            {
                var data = await _objReportDal.SalesManWiseSaleSummaryForRpt(objSaleDetailsSummary, shopUrl.ShopUrl);
                if (data != null)
                {
                    var deleteData = await _objReportDal.SalesManWiseSaleSummaryDelete(objSaleDetailsSummary.ShopId);
                    if (deleteData != null)
                    {
                        foreach (var salesManWiseSaleReportModel in data)
                        {
                            //string frmdt = Convert.ToDateTime(salesManWiseSaleReportModel.InvoiceDate).ToString("MM/dd/yyyy");
                            salesManWiseSaleReportModel.ShopId = objSaleDetailsSummary.ShopId;
                            saveReportData = await _objReportDal.SalesManSaleSummarySaveForRpt(salesManWiseSaleReportModel);
                        }
                    }

                }

            }

            return saveReportData;
        }

        public async Task<string> SaleSummaryPro(SaleDetailsSummary objSaleDetailsSummary)
        {
            var msg = await _objReportDal.SaleSummarySaveForRpt(objSaleDetailsSummary);
            return "o";
        }
        public async Task<string> AllShopSaleSummaryCategorySubCategoryWisePro(SaleDetailsSummary objSaleDetailsSummary)
        {
            var msg = await _objReportDal.AllShopSaleSummaryCategorySubCategorySaveForRpt(objSaleDetailsSummary);
            return "o";
        }

        public async Task<string> ShopStockDataSaveForRpt(int shopId)
        {
            var msg = await _objReportDal.ShopStockDataSaveForRpt(shopId);
            return "SUCCESS";
        }

        public async Task<string> ShopStockDataSaveTrForRpt(int shopId)
        {
            var msg = await _objReportDal.ShopStockDataSaveTrForRpt(shopId);
            return "SUCCESS";
        }
        public async Task<string> PeriodicalStockRptPro(PeriodicalStockReportModel objPeriodicalStockReportModel)
        {
            var msg = await _objReportDal.PeriodicalStockSaveForRpt(objPeriodicalStockReportModel);
            return "o";
        }

        public async Task<string> WarehousePeriodicalStockRptPro(PeriodicalStockReportModel objPeriodicalStockReportModel)
        {
            var msg = await _objReportDal.WarehousePeriodicalStockSaveForRpt(objPeriodicalStockReportModel);
            return "o";
        }

        #endregion

        #region XLSX Report 

        private async Task<int> ForExcleFormateSaleSummary(SaleDetailsSummary saleDetailsSummary)
        {
            PaymentTypeSaleDetailsSummaryForExcelReport payment = new PaymentTypeSaleDetailsSummaryForExcelReport();
           var  objSaleSummaryForExcel =  await _objReportDal.GetSaleSummaryFroExcel(saleDetailsSummary);
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
            ws.Cells["A1"].Value = "INVOICE_NUM";
            ws.Cells["B1"].Value = "DATE";
            ws.Cells["C1"].Value = "TOTAL";
            ws.Cells["D1"].Value = "EX/RTN AMOUNT";
            ws.Cells["E1"].Value = "AC. VALUE";
            ws.Cells["F1"].Value = "PRE DISCOUNT";
            ws.Cells["G1"].Value = "DISCOUNT AMOUNT";
            ws.Cells["H1"].Value = "AC. DISCOUNT";
            ws.Cells["I1"].Value = "DISCOUNT PERCENT(%)";
            ws.Cells["J1"].Value = "NET AMOUNT";
            ws.Cells["K1"].Value = "VAT";
            ws.Cells["L1"].Value = "PAYMENT TYPE";

            double sumTotalAmount = 0;
            double sumExReturnAmount = 0;
            double sumActualAmount = 0;
            double sumPreDiscount = 0;
            double sumDiscountAmount = 0;
            double sumActualDiscount = 0;
            double sumNetAmount = 0;
            double sumVatAmount = 0;

            int rowStart = 2;
            foreach (var item in objSaleSummaryForExcel)
            {
                payment.CashAmount = item.CashAmount;
                payment.BkashAmount = item.BkashAmount;
                payment.BracBankAmount = item.BracBankAmount;
                payment.RocketAmount = item.RocketAmount;
                payment.DBBLAmount = item.DBBLAmount;
                payment.SCBAmount = item.SCBAmount;
                payment.SIBLAmount = item.SIBLAmount;
                payment.CITYAmount = item.CITYAmount;
                payment.EBLAmount = item.EBLAmount;
                payment.GIFTVoucherAmount = item.GIFTVoucherAmount;
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.InvoiceNumber;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.InvoiceDate;
                ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDouble(item.TotalAmount);
                ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDouble(item.ExReturnAmount);
                ws.Cells[string.Format("E{0}", rowStart)].Value = Convert.ToDouble(item.ActualAmount);
                ws.Cells[string.Format("F{0}", rowStart)].Value = Convert.ToDouble(item.PreDiscount);
                ws.Cells[string.Format("G{0}", rowStart)].Value = Convert.ToDouble(item.DiscountAmount);
                ws.Cells[string.Format("H{0}", rowStart)].Value = Convert.ToDouble(item.ActualDiscount);
                ws.Cells[string.Format("I{0}", rowStart)].Value = Convert.ToDouble(item.DiscountPercent);
                ws.Cells[string.Format("J{0}", rowStart)].Value = Convert.ToDouble(item.NetAmount);
                ws.Cells[string.Format("K{0}", rowStart)].Value = Convert.ToDouble(item.Vat);
                ws.Cells[string.Format("L{0}", rowStart)].Value = item.PaymentType;

                sumTotalAmount += Convert.ToDouble(item.TotalAmount);
                sumExReturnAmount += Convert.ToDouble(item.ExReturnAmount);
                sumActualAmount += Convert.ToDouble(item.ActualAmount);
                sumPreDiscount += Convert.ToDouble(item.PreDiscount);
                sumDiscountAmount += Convert.ToDouble(item.DiscountAmount);
                sumActualDiscount += Convert.ToDouble(item.ActualDiscount);
                sumNetAmount += Convert.ToDouble(item.NetAmount);
                sumVatAmount += Convert.ToDouble(item.Vat);

                rowStart++;
            }
            int endFor = rowStart + 2;
            //Sum Section
            ws.Cells[string.Format("A{0}", rowStart)].Value = "Total";
            ws.Cells[string.Format("C{0}", rowStart)].Value = sumTotalAmount;
            ws.Cells[string.Format("D{0}", rowStart)].Value = sumExReturnAmount;
            ws.Cells[string.Format("E{0}", rowStart)].Value = sumActualAmount;
            ws.Cells[string.Format("F{0}", rowStart)].Value = sumPreDiscount;
            ws.Cells[string.Format("G{0}", rowStart)].Value = sumDiscountAmount;
            ws.Cells[string.Format("H{0}", rowStart)].Value = sumActualDiscount;
            ws.Cells[string.Format("J{0}", rowStart)].Value = sumNetAmount;
            ws.Cells[string.Format("K{0}", rowStart)].Value = sumVatAmount;

            //Payment Type Section
            ws.Cells[string.Format("A{0}", endFor)].Value = "CashAmount";        
            ws.Cells[string.Format("B{0}", endFor)].Value = "BkashAmount";
            ws.Cells[string.Format("C{0}", endFor)].Value = "BracBankAmount";
            ws.Cells[string.Format("D{0}", endFor)].Value = "RocketAmount";
            ws.Cells[string.Format("E{0}", endFor)].Value = "DBBLAmount";
            ws.Cells[string.Format("F{0}", endFor)].Value = "SCBAmount";
            ws.Cells[string.Format("G{0}", endFor)].Value = "SIBLAmount";
            ws.Cells[string.Format("H{0}", endFor)].Value = "CITYAmount";
            ws.Cells[string.Format("I{0}", endFor)].Value = "EBLAmount";
            ws.Cells[string.Format("J{0}", endFor)].Value = "GIFTVoucherAmount";

            // value Asagin

            ws.Cells[string.Format("A{0}", endFor+1)].Value = Convert.ToDouble(payment.CashAmount);
            ws.Cells[string.Format("B{0}", endFor+1)].Value = Convert.ToDouble(payment.BkashAmount);
            ws.Cells[string.Format("C{0}", endFor+1)].Value = Convert.ToDouble(payment.BracBankAmount);
            ws.Cells[string.Format("D{0}", endFor + 1)].Value = Convert.ToDouble(payment.RocketAmount);
            ws.Cells[string.Format("E{0}", endFor+1)].Value = Convert.ToDouble(payment.DBBLAmount);
            ws.Cells[string.Format("F{0}", endFor+1)].Value = Convert.ToDouble(payment.SCBAmount);
            ws.Cells[string.Format("G{0}", endFor+1)].Value = Convert.ToDouble(payment.SIBLAmount);
            ws.Cells[string.Format("H{0}", endFor+1)].Value = Convert.ToDouble(payment.CITYAmount);
            ws.Cells[string.Format("I{0}", endFor+1)].Value = Convert.ToDouble(payment.EBLAmount);
            ws.Cells[string.Format("J{0}", endFor+1)].Value = Convert.ToDouble(payment.GIFTVoucherAmount);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "SaleSummaryExcelFile.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

            return 0;
        }

        #endregion

        #region "SOL Purchase Receive Report"

        [RoleFilter]
        public async Task<ActionResult> SolPurchaseReceive()
        {
            //await GetUpdateData();
            LoadSession();
            var solPurchaseReceive = new SolPurchaseReceiveReport();
            ViewBag.ProductStyleist = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_ID", "PRODUCT_STYLE");

            return View(solPurchaseReceive);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SolPurchaseReceive(SolPurchaseReceiveReport solPurchaseReceiveReport)
        {
            LoadSession();
            if (solPurchaseReceiveReport.ReportType == "1")
            {
                solPurchaseReceiveReport.ReportType = "PDF";

                if (solPurchaseReceiveReport.ProductId != null)
                {
                    if (solPurchaseReceiveReport.ProductId.Count > 0)
                    {
                        for (int i = 0; i < solPurchaseReceiveReport.ProductId.Count; i++)
                        {
                            solPurchaseReceiveReport.ProductIDs += solPurchaseReceiveReport.ProductId[i] + ",";
                        }
                        solPurchaseReceiveReport.ProductIDs = solPurchaseReceiveReport.ProductIDs.Remove(solPurchaseReceiveReport.ProductIDs.Length - 1);
                    }
                }
                
                if (solPurchaseReceiveReport.RadioFor == "CSR")
                {
                    await GenerateSolPurchaseReceiveReport(solPurchaseReceiveReport);
                }

            }

            ViewBag.ProductStyleist = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_ID", "PRODUCT_STYLE");
            return View(solPurchaseReceiveReport);
        }

        private async Task<int> GenerateSolPurchaseReceiveReport(SolPurchaseReceiveReport objSolPurchaseReceiveReport)
        {
            DataSet objDataSet = null;
            string strPath = Path.Combine(Server.MapPath("~/Reports/SolPurchaseReceive/SolPurchaseReceive.rpt"));
            _objReportDocument.Load(strPath);

            objDataSet = (await _objReportDal.SolPurchaseReceive(objSolPurchaseReceiveReport));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            ShowReport(objSolPurchaseReceiveReport.ReportType, "SOL Purchase Receive Report");
            return 0;
        }

        #endregion


        }
}