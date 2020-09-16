using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class OtherPurchaseReceiveItemModel
    {
        public int OtherPurchaseReceiveItemId { get; set; }

        public string OtherPurchaseReceiveNumber { get; set; }

        public int ItemId { get; set; }

        public int ProductId { get; set; }

        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string Style { get; set; }

        public int ReceiveQuantity { get; set; }

        public int StockQuantity { get; set; }

        public double SalePrice { get; set; }

        public string WareHouseId { get; set; }

    }
}