using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class PoCuttingModel
    {
        public int PoCuttingId { get; set; }

        public string PurchaseOrderNumber { get; set; }

        [Required]
        public string OrderDate { get; set; }

        [Required]
        public string DeliveryDate { get; set; }

        [Required]
        public string DisplayDate { get; set; }

        [Required]
        public int VendorId { get; set; }
        public string VendorName { get; set; }

        [Required]
        public int DeliveryShopId { get; set; }
        public string DeliveryShopName { get; set; }

        [Required]
        public int SeasonId { get; set; }
        public string SeasonName { get; set; }

        public string UpdateBy { get; set; }

        public string UpdateDate { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }

        public List<PoCuttingItem> PoCuttingItems { get; set; }
    }

    public class PoCuttingItem
    {
        public int PoCuttingItemId { get; set; }
        public int PoCuttingId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public int PoQuantity { get; set; }
        [Required]
        public int CuttingQuantity { get; set; }
    }
}