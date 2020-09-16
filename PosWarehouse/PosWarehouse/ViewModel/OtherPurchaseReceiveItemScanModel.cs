using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class OtherPurchaseReceiveItemScanModel
    {
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ItemName { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public string Style { get; set; }
        public int Quantity { get; set; }
        public double VatPercent { get; set; }
    }
}