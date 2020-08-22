using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class ItemInfoShopReceiveModel
    {
        public string SeasonName { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public string Brand { get; set; }
        public string Umo { get; set; }

        public double SalePrice { get; set; }



    }
}