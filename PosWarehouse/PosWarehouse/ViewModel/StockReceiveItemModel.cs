using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class StockReceiveItemModel
    {
        public int StockReceiveItemId { get; set; }
        public int StockReceiveId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public int ReceiveQuantity { get; set; }
        public double SalePrice { get; set; }
        public double Vat { get; set; }
    }
}