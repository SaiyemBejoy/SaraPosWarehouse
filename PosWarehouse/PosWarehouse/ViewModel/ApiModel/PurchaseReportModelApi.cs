using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class PurchaseReportModelApi
    {
        [DisplayName("Start Date")]
        public string StartDate { get; set; }
        [DisplayName("End Date")]
        public string EndDate { get; set; }
        [Required(ErrorMessage = "Material Type is required")]
        [DisplayName("Material Type")]
        public string MaterialTypeId { get; set; }
        public string MaterialTypeName { get; set; }
        [DisplayName("Material Sub Type")]
        public string MaterialSubTypeId { get; set; }
        public string MaterialSubTypeName { get; set; }

        public string PurchaseId { get; set; }
        public string RequisitionId { get; set; }
        public string RequisitionCode { get; set; }
        public string CreateDate { get; set; }
        public string StoreId { get; set; }
        public string PurchaseCode { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string Bin { get; set; }

        public string UnitPrice { get; set; }
        public string TotalAmount { get; set; }
        public string Quantity { get; set; }
        public string BulkUsed { get; set; }
        public string SampleUsed { get; set; }
        public string UtilizedQuantity { get; set; }
        public string InStock { get; set; }
        public string BulkUtilizedDate { get; set; }
        public string SampleUtilizedDate { get; set; }
        public string OpeningStock { get; set; }
        public string PeriodDate { get; set; }
        public string ClosingStock { get; set; }

        public string ReportType { get; set; }
        public string RadioFor { get; set; }
    }
}