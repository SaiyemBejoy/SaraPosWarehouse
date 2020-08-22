using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class InvoiceInfoModel
    {
        public int SaleInfoAutoId { get; set; }
        public int SaleInfoId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public int TotalItem { get; set; }
        public double TotalAmount { get; set; }
        public double Vat { get; set; }
        public int DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
        public double SubTotal { get; set; }
        public double NetAmount { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContactNO { get; set; }
        public string CreatedBy { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int WareHouseId { get; set; }

        public IEnumerable<SaleItemForInvoiceModel> SaleItemList { get; set; }
    }

    public class SaleItemForInvoiceModel
    {
        public int SaleItemId { get; set; }
        public int SaleInfoId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string StyleName { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public double Vat { get; set; }
        public double Total { get; set; }
        public double PaidAmount { get; set; }
    }
}