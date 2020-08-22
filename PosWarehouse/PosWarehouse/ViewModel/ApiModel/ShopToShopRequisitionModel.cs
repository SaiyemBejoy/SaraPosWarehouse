using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class ShopToShopRequisitionModel
    {

        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double SalePrice { get; set; }
        public double PurchasePrice { get; set; }
        public int Quantity { get; set; }
        public int ShoId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }


    }
    //save Requisition Data In wareHouse Database
    public class ShopToShopRequisitionMainModel
    {

        public int RequisitionId { get; set; }

        public string RequisitionNumber { get; set; }

        public string RequisitionShopIdTo { get; set; }

        public string RequisitionShopNameTo { get; set; }

        public string RequisitionShopIdFrom { get; set; }

        public string RequisitionShopNameFrom { get; set; }

        public string RequisitionDate { get; set; }

        public string CreatedBy { get; set; }

        public string DeliveryStatus { get; set; }

        public IEnumerable<ShopToShopRequisitionMainItemModel> ShopToShopRequisitionMainItemList { get; set; }
    }
    public class ShopToShopRequisitionMainItemModel
    {
        public int ShopRequisitionMainItemId { get; set; }
        public int RequisitionId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}