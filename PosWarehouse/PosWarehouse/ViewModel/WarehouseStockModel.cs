using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class WarehouseStockModel
    {
        public int ItemId { get; set; }

        public int ProductId { get; set; }

        public string ItemName { get; set; }

        public string BarCode { get; set; }

        public int Quantity { get; set; }

        public double PurchasePrice { get; set; }

        public double SalePrice { get; set; }

        public double Vat { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

    }
}