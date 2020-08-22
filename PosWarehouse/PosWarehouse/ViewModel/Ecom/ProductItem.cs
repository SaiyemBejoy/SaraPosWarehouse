using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.Ecom
{
    public class ProductItem
    {
        public int WhProductId { get; set; }
        public int ItemId { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public string ItemName { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string FabricName { get; set; }
    }
}