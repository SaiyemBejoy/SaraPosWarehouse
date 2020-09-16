using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class LowStockModel
    {
        public int ProductId { get; set; }

        public int ProductStyleCount { get; set; }

        public int ShopId { get; set; }

        public string ShopName { get; set; }

    }
}