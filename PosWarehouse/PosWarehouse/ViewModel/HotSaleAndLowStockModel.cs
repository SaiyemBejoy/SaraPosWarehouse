using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class HotSaleAndLowStockModel
    {
        public string BarCode { get; set; }
        public string StyleName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
    public class HotSaleModel
    {
        public string BarCode { get; set; }
        public string StyleName { get; set; }
        public int Quantity { get; set; }
    }
}