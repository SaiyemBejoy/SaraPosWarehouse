using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class PurchaseReceiveModel
    {
        public string SearchKey { get; set; }

        public int PurchaseReceiveId { get; set; }
        public string PurchaseReceiveNumber { get; set; }

        [DisplayName("Purchase Date")]
        public string PurchaseDate { get; set; }
        [Required]
        [DisplayName("Vendor")]
        public int VendorId { get; set; }
        [DisplayName("Delivery To")]
        public string DeliveryShopId { get; set; }
        public string DeliveryShopName { get; set; }
        [Required]
        [DisplayName("Purchase Order")]
        public string PurchaseOrderId { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string OtherPurchaseReceiveNumber { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }

        public bool SOLReceiveYN { get; set; }

        public List<ReceiveItem> ReceiveItemsList { get; set; }
        public List<PoNumber> PoNumberList { get; set; }

    }

    public class ReceiveItem
    {
        public int PurchaseReceiveItemId { get; set; }

        public string PurchaseReceiveNumber { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public int PurchaseReceiveId { get; set; }

        public int ItemId { get; set; }

        public int ProductId { get; set; }

        public string ItemName { get; set; }       

        public int OrderQuantity { get; set; }

        public double PurchasePrice { get; set; }

        public string Barcode { get; set; }

        public int ReceiveQuantity { get; set; }

        public double Vat { get; set; }

        public double? SubTotalAmount { get; set; }

        public double SalePrice { get; set; }

        public string Profit { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class PoNumber
    {
        public string PurchaseOrderId { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string DeliveryShopId { get; set; }

        public List<ReceiveItem> ReceiveItemsList { get; set; }
    }
}