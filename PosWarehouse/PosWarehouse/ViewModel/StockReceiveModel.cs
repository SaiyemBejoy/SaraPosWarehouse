using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class StockReceiveModel
    {
        public int StockReceiveId { get; set; }

        public string StockReceiveNumber { get; set; }

        public string StockTransferChallanNo { get; set; }

        public string DamageTransferChallanNo { get; set; }

        public string StockTransferFromShopId { get; set; }

        public string StockTransferFromShopName{ get; set; }

        public string RequisitionNo { get; set; }

        public string ReceivedBy { get; set; }

        public string ReceivedDate { get; set; }

        public string ReceivedStatus { get; set; }

        public List<StockReceiveItemModel> StockReceiveItemList { get; set; }

    }
}