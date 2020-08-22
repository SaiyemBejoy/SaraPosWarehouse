using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class ShopToShopDeliveryModel
    {
        public int DeliveryId { get; set; }

        public string DeliveryNumber { get; set; }

        public string RequisitionNumber { get; set; }

        public string DeliveryShopIdTo { get; set; }

        public string DeliveryShopNameTo { get; set; }

        public string DeliveryShopIdFrom { get; set; }

        public string DeliveryShopNameFrom { get; set; }

        public string DeliveryDate { get; set; }

        public string DeliveredBy { get; set; }

        public string ReceivedYN { get; set; }

        public string ReceivedBy { get; set; }

        public string ReceivedDate { get; set; }

        public string ReturnYN { get; set; }

        public string ReturnBy { get; set; }

        public string ReturnDate { get; set; }

        public IEnumerable<ShopToShopRequDeliveryItemModel> ShopToShopRequDeliveryItemModelList { get; set; }
    }

    public class ShopToShopRequDeliveryItemModel
    {
        public int DeliveryItemId { get; set; }
        public int DeliveryId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

    public class ShopToShopRequReturnItemModel
    {
        public int ReturnItemId { get; set; }
        public int DeliveryId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}