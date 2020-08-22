using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class PurchaseOrderModel
    {
        [Required]
        public string SearchKey { get; set; }

        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }

        [Required]
        public string OrderDate { get; set; }

        [Required]
        public string DeliveryDate { get; set; }

        [Required]
        public string DisplayDate { get; set; }

        [Required]
        public int VendorId { get; set; }

        [Required]
        public int DeliveryShopId { get; set; }
        public string DeliveryShopName { get; set; }

        [Required]
        public int SeasonId { get; set; }
        public string SeasonName { get; set; }

        public double SubTotal { get; set; }

        public double Vat { get; set; }

        public double GrandTotal { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
        [Required]
        public string[] GarmentsType { get; set; }
        [Required]
        public string[] FabricType { get; set; }
        [Required]
        public string[] FabricCode { get; set; }
        [Required]
        public string[] Consumption { get; set; }
        [Required]
        public string[] FabricQuantity { get; set; }
        public int[] RowCount { get; set; }


        public List<PurchaseItem> PurchaseItems { get; set; }
        public List<FabricModel> FabricModels { get; set; }
        public FabricModel FabricModel { get; set; }
    }

    public class PurchaseItem
    {
        public int PurchaseOrderItemId { get; set; }
        public string PurchaseOrderNumber { get; set; }

        public int ItemId { get; set; }

        public int ProductId { get; set; }

        public int VendorId { get; set; }
        public string VendorName { get; set; }

        public int DeliveryShopId { get; set; }
        public string DeliveryShopName { get; set; }

        public string SeasonName { get; set; }

        public string OrderDate { get; set; }

        public string DeliveryDate { get; set; }

        public string DisplayDate { get; set; }

        public string ItemName { get; set; }

        public string Barcode { get; set; }

        public string Uom { get; set; }

        public double LastPurchasePrice { get; set; }

        public double PurchasePrice { get; set; }

        public double SalePrice { get; set; }
        [Required]
        public int? Quantity { get; set; }

        public double? SubTotalPrice { get; set; }

        public double VatPercent { get; set; }
        public double? VatAmount { get; set; }

        public double? GrandTotalPrice { get; set; }

        public bool IsActiveItem { get; set; }
    }

    public class FabricModel
    {
        public int FabricId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        [Required]
        public string GarmentsType { get; set; }
        [Required]
        public string FabricType { get; set; }
        [Required]
        public string FabricCode { get; set; }
        [Required]
        public string Consumption { get; set; }
        [Required]
        public string FabricQuantity { get; set; }

    }

    public class PurchaserOrderModelForReport
    {
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string DeliveryDate { get; set; }
        public string DisplayDate { get; set; }
        public string DeliveryShopName { get; set; }
        public string SeasonName { get; set; }  
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public List<PurchaseItemRpt> PurchaseItemList { get; set; }
        public List<FabricModel> FabricModelList { get; set; }
        public List<PurchaseItemSizeRpt> SizeModelListHeading { get; set; }

    }

    public class PurchaseItemRpt
    {
        public string PurchaseOrderNumber { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public int? Quantity { get; set; }
        public string ItemName { get; set; }
        public string ProductStyle { get; set; }
        public string ProductType { get; set; }
        public string Barcode { get; set; }
        public double SalePrice { get; set; }
        public double PurchasePrice { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Designer { get; set; }
        public string Merchandiser { get; set; }
        public string Wash { get; set; }
        public string Karcupi { get; set; }
        public string Print { get; set; }
        public string Embroidery { get; set; }

        public List<PurchaseItemColorRpt> PurchaseItemColorRpt { get; set; }

        public List<PurchaseItemFitRpt> FFitModelList { get; set; }
    }

    public class PurchaseItemColorRpt
    {
        public string PurchaseOrderNumber { get; set; }
        public int ItemId { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public List<PurchaseItemSizeRpt> SizeModelList { get; set; }
    }
    public class PurchaseItemSizeRpt
    {
        public string PurchaseOrderNumber { get; set; }
        public int ColorId { get; set; }
        public int ItemId { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int Quantity { get; set; }
    }
    public class PurchaseItemFitRpt
    {
        public string PurchaseOrderNumber { get; set; }
        public string Fit { get; set; }
    }
}