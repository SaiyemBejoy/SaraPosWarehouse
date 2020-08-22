using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class SalesManWiseSaleReportModel
    {
        public string SalesManId { get; set; }
        public string InvoiceDate { get; set; }
        public int SaleQuantity { get; set; }
        public double TotalAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double NetAmount { get; set; }
        public double VatAmount { get; set; }
        public int ShopId { get; set; }
    }
}