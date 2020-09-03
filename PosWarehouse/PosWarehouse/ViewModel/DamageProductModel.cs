using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class DamageProductModel
    {
        public int DamageId { get; set; }
        public string DamageChallanNo { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        //For Display Data
        public string EmployeeName { get; set; }
        public string ApproverStatus { get; set; }
        public string RejectMessage { get; set; }
        public string RejectedDate { get; set; }
        public string RejectedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        //End

        public IEnumerable<DamageProductItemModel> DamageProductItemList { get; set; }
    }
    public class DamageProductItemModel
    {
        public int DamageItemId { get; set; }
        public int DamageId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public string Remarks { get; set; }

    }

}