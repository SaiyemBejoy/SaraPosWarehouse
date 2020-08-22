using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class RequisitionMainModel
    {
        public int RequisitionAutoId { get; set; }
        public int RequisitionId { get; set; }
        public string RequisitionNo { get; set; }
        public string RequisitionDate { get; set; }
        public string CreatedBy { get; set; }
        public string Delivery_YN { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }

        public IEnumerable<RequisitionMainItemModel> RequisitionMainItemList { get; set; }
        public IEnumerable<RequisitionDeliveryModel> RequisitionDeliveryItemList { get; set; }
    }
}