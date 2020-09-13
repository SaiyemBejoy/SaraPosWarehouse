using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        [Required]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [Required]
        [DisplayName("Product Style")]
        public string ProductStyle { get; set; }
        [Required]
        [DisplayName(" Description")]
        public string ProductDescription { get; set; }
        [Required]
        [DisplayName("Image")]
        public HttpPostedFileBase ProductImage { get; set; }
        public string ProductImageString { get; set; }

        [DisplayName("Designer")]
        [Required]
        public string DesignerId { get; set; }

        [DisplayName("Merchandiser")]
        [Required]
        public string MerchandiserId { get; set; }

        [DisplayName("Brand Name")]
        public int BrandId { get; set; }

        [DisplayName("Purchase Measuring Unit")]
        public int PurchaseMeasuringUnitId { get; set; }

        [DisplayName("Sales Measuring Unit")]
        public int SalesMeasuringUnitId { get; set; }

        [DisplayName("1 Purchase Unit = ? Sales Unit")]
        public string PurchaseUnitSaleUnit { get; set; }
        // Pore material and kisu other cost add hoise oiter jonno 
        public int MaterialId { get; set; }
        public int OtherCostId { get; set; }
        //End
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [Required]
        [DisplayName("Sub Category")]
        public int SubCategoryId { get; set; }

        [DisplayName("Active Status")]
        public bool IsActive { get; set; } = false;

        [Required]
        [DisplayName("Purchase Price")]
        public double? PurchasePrice { get; set; }

        [Required]
        [DisplayName("Material  Cost")]
        public double? MaterialCost { get; set; }

        [Required]
        [DisplayName("CM")]
        public double? CM { get; set; }

        [Required]
        [DisplayName("COGS")]
        public double? COGS { get; set; }

        [DisplayName("Karchupi")]
        public bool Karchupi { get; set; }

        [DisplayName("Wash")]
        public string Wash { get; set; }

        [DisplayName("Print")]
        public string Print { get; set; }


        [DisplayName("Embroidery")]
        public string Embroidery { get; set; }

        [Required]
        [DisplayName("Sale Price")]
        public double? SalePrice { get; set; }

        [DisplayName("Include Vat")]
        public bool IncludeVat { get; set; } = true;

        [DisplayName("Attribute")]
        public int AttributeId { get; set; }
        [Required]
        [DisplayName("Season")]
        public int? SeasonId { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }

        
        public string MaterialCostDetailse { get; set; }
        public string OthersCostDetailse { get; set; }
    }

    public class ProductGrid
    {
        public bool IsActive { get; set; }

        public string ProductId { get; set; }

        public string ProductImage { get; set; }

        public string ProductName { get; set; }

        public string ProductStyle { get; set; }

        public string ProductDescription { get; set; }

        public bool IncludeVat { get; set; }

        public string MaterialCost { get; set; }
        public string CM { get; set; }

        public string PurchasePrice { get; set; }
        public string SellPrice { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }

        public string SearchBy { get; set; }
        public string UpdateBy { get; set; }
        public string OrderByName { get; set; }
        public string OrderByDirection { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class ProductAttribute
    {
        public int ProductId { get; set; }

        public int AttributeId { get; set; }
        public string Attribute { get; set; }

        public int AttributeValueId { get; set; }

        public string AttributeValue { get; set; }
    }

    public class AttributesModel
    {
        public List<Attributes> Attributes { get; set; }
        //public IEnumerable<ProductItem> ProductItems { get; set; }
        public List<ProductItem> ProductItemList { get; set; }
    }

    public class Attributes
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public int AttributeOrder { get; set; }

        public List<AttributesValue> AttributesValues { get; set; }

        public IEnumerable<ProductItem> ProductItems { get; set; }
    }

    public class AttributesValue
    {
        public int AttributeValueId { get; set; }
        public string AttributeValueName { get; set; }
    }

    public class ProductItem
    {
        public int ItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        public string ItemName { get; set; }

        public double PurchasePrice { get; set; }
        public double MaterialCost { get; set; }
        public double CM { get; set; }

        public double SalePrice { get; set; }
        public string Style { get; set; }
        public string ItemColor { get; set; }
        public string ItemSize { get; set; }
        public double VatPercent { get; set; }

        public bool IsActive { get; set; }

        public List<ItemAttribute> ItemAttributes { get; set; }

        public int TotalItem { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class ItemAttribute
    {
        public int ProductId { get; set; }

        public int ProductItemId { get; set; }

        public string ProductCode { get; set; }

        public int AttributeId { get; set; }
        public string AttributeName { get; set; }

        public int AttributeValueId { get; set; }
        public string AttributeValueName { get; set; }
    }

    public class ProductMaterialCostDetails
    {
        public int MaterialCostAutoId { get; set; }

        public int ProductId { get; set; }

        public int MaterialId{ get; set; }

        public string MaterialName { get; set; }

        public double UnitPrice { get; set; }

        public double UsedMaterial { get; set; }

        public double SubTotal { get; set; }

        public string UpdatedBy { get; set; }
    }

    public class ProductOthersCostDetails
    {
        public int OthersCostAutoId { get; set; }

        public int ProductId { get; set; }

        public int OtherCostId { get; set; }
        public string PurposeOfCost { get; set; }

        public double CostValue { get; set; }

        public string UpdatedBy { get; set; }


    }
}