using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class StoreDeliveryModel
    {   //primary Key
        public int StoreDeliveryId { get; set; }

        public string DeliveryNumber { get; set; }

        public string FromDate { get; set; }

        public int PurchaseReceiveId { get; set; }

        public string PurchaseReceiveNumber { get; set; }

        public string ShopRequisitionNumber { get; set; }

        public int DeliveryShopId { get; set; }

        public string DeliveryShopName { get; set; }

        public int SeasonId { get; set; }

        public string SeasonName { get; set; }

        public int RegisterId { get; set; }
    
        public string RequistionNo { get; set; }

        public bool ReceiveChallanDeliveryStatus { get; set; }
        public string ReceiveChallanDelivery { get; set; }

        public bool RequisitionDelivery { get; set; }

        public string DeliveryDate { get; set; }

        public string Barcode { get; set; }

        public string ProductName { get; set; }

        public double SalePrice { get; set; }

        public int? CurrentStock { get; set; }

        public int? DeliveryQuantity { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }

        public List<ProductInventoryModel> ProductInventoryList { get; set; }
    }

    public class ProductInventoryModel
    {
        public int ProductId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public string Barcode { get; set; }

        public int OrderQuantity { get; set; }

        public int DeliveryQuantity { get; set; }

        public int CurrentStock { get; set; }

        public double PurchasePrice { get; set; }

        public double SalePrice { get; set; }

        public double Vat { get; set; }
    }

    public class StoreDeliveryGrid
    {
        public string StoreDeliveryId { get; set; }
        public string DeliveryNumber { get; set; }

        public string DeliveryShopId { get; set; }

        public string DeliveryShopName { get; set; }

        public string PurchaseReceiveId { get; set; }

        public string PurchaseReceiveNumber { get; set; }

        public string DeliveryDate { get; set; }

        public string RequistionNo { get; set; }

        public bool ReceiveChallanDeliveryStatus { get; set; }
        public string Receive_YN { get; set; }
        public string ReceivedBy { get; set; }

        public string SearchBy { get; set; }
        //public string UpdateBy { get; set; }
        public string OrderByDeliveryNumber { get; set; }
        public string OrderByDirection { get; set; }

        public string WareHouseId { get; set; }
        public string ShopId { get; set; }
    }

    public class StoreDeliveryItemModel
    {
        public int StoreDeliveryItemId { get; set; }
        public string StoreDeliveryNumber { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public string DeliveryQuantity { get; set; }
        public string PurchasePrice { get; set; }
        public string SalePrice { get; set; }
        public string Vat { get; set; }
    }
}