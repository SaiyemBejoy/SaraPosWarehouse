using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ExcelReport
{
    public class ExcelReportModel
    {
    }

    public class SaleDetailsSummaryForExcelReport
    {
        public int SaleSummaryId { get; set; }
        public int ShopId { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string TotalAmount { get; set; }
        public double ExReturnAmount { get; set; }
        public double ActualAmount { get; set; }
        public double PreDiscount { get; set; }
        public string DiscountAmount { get; set; }
        public double ActualDiscount { get; set; }
        public string DiscountPercent { get; set; }
        public string NetAmount { get; set; }
        public string Vat { get; set; }
        public string PaymentType { get; set; }
        public string CashAmount { get; set; }
        public string BkashAmount { get; set; }
        public string BracBankAmount { get; set; }
        public string RocketAmount { get; set; }
        public string DBBLAmount { get; set; }
        public string SCBAmount { get; set; }
        public string SIBLAmount { get; set; }
        public string CITYAmount { get; set; }
        public string EBLAmount { get; set; }
        public string GIFTVoucherAmount { get; set; }
    }
    public class PaymentTypeSaleDetailsSummaryForExcelReport
    {
        public string CashAmount { get; set; }
        public string BkashAmount { get; set; }
        public string BracBankAmount { get; set; }
        public string RocketAmount { get; set; }
        public string DBBLAmount { get; set; }
        public string SCBAmount { get; set; }
        public string SIBLAmount { get; set; }
        public string CITYAmount { get; set; }
        public string EBLAmount { get; set; }
        public string GIFTVoucherAmount { get; set; }

    }
}