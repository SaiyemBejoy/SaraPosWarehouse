using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class RequisitionMainItemModel
    {
        public int RequisitionMainItemAutoId { get; set; }
        public int RequisitionMainItemId { get; set; }
        public int RequisitionAutoId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string Price { get; set; }
        public string RqsnQuantity { get; set; }
    }
    public class RequisitionDeliveryModel
    {
        public int RequisitionMainItemId { get; set; }
        public int RequisitionId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string Price { get; set; }
        public int ProductStock { get; set; }
        public double Vat { get; set; }

    }
}