using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class SaleInfoModel
    {

        public int SaleInfoAutoId { get; set; }
        public int SaleInfoId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string SalesManId { get; set; }
        public string SalesManName { get; set; }
        public int TotalItem { get; set; }
        public double TotalAmount { get; set; }
        public double Vat { get; set; }
        public double DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
        public double BagPrice { get; set; }
        public string PaymentType { get; set; }
        public double CashAmount { get; set; }
        public double CardAmount { get; set; }
        public double SubTotal { get; set; }
        public double NetAmount { get; set; }
        public int CustomerId { get; set; }
        public string HoldInvoiceYN { get; set; }
        public string ExchangeYN { get; set; }
        public int ExchangeShopId { get; set; }
        public string CreatedBy { get; set; }
        public string ShopId { get; set; }
        public string WareHouseId { get; set; }

        public IEnumerable<SaleItemModel> SaleItemList { get; set; }
        public IEnumerable<SalePaymentInfoModel> SalePaymentInfoList { get; set; }
    }
}