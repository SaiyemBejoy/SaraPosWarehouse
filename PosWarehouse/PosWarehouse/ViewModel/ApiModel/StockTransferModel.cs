using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class StockTransferModel
    {
        public int StockTransferId { get; set; }

        public string StockTransferChallanNumber { get; set; }

        public string RequisitionNumber { get; set; }

        public string TransferDate { get; set; }

        public string TransferToShopId { get; set; }

        public string TransferFromShopId { get; set; }

        public string TransferFromShopName { get; set; }

        public string ReceivedYN { get; set; }

        public string TransferedBy { get; set; }

        public List<StockTransferItemModel> StockTransferItemList { get; set; }
    }
}