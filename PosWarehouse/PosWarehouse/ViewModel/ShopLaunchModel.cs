using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class ShopLaunchModel
    {
        public int  ShopLaunchId { get; set; }

        public int ProductId { get; set; }

        public string StyleName { get; set; }

        public string LaunchDate { get; set; }

        public int ShopId { get; set; }

        public string CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string WarehouseId { get; set; }

    }
}