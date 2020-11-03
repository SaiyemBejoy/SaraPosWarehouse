using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class OtherPurchaseReceiveModel
    {
        public int OtherPurchaseReceiveId { get; set; }
        public string OtherPurchaseReceiveNumber { get; set; }
        public int VendorId { get; set; }
        public string DeliveryShopId { get; set; }
        public string DeliveryShopName { get; set; }

        public string UpdateBy { get; set; }
        public string UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }

        public string WareHouseId { get; set; }

        public string ReceiveYN { get; set; }
        
        public bool HoldStatus { get; set; }
        public string Hold_YN { get; set; }

        public bool ReScanStatus { get; set; }
        public string Scan_Type { get; set; }

        public int TotalChallanQty { get; set; }

        public List<OtherPurchaseReceiveItemModel> ReceiveItemsList { get; set; }
       
    }
}