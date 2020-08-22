using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class DashboardModel
    {
        public double TotalSale { get; set; }
        public double ToDaysSale { get; set; }
        public double CurrentMonthSale { get; set; }
        public double LastSevenDaysSale { get; set; }
        public string ShopId { get; set; }
        public string WareHouseId { get; set; }
        public string ShopUrl { get; set; }
    }
}