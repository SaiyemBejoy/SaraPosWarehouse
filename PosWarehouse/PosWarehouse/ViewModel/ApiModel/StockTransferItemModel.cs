using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class StockTransferItemModel
    {
        public int StockTransferItemId { get; set; }
        public int StockTransferId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public int TransferQuantity { get; set; }
        public double SalePrice { get; set; }
        public double Vat { get; set; }
    }
}