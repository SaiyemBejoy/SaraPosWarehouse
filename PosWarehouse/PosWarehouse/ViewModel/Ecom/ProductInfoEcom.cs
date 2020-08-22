using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.Ecom
{
    public class ProductInfoEcom
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductStyle { get; set; }
        public string ProductDescription { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string ActiveYN { get; set; }
        public string ProductImage { get; set; }
        public string VatYN { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public string WarehouseId { get; set; }
        public string ShopId { get; set; }
        public int BrandId { get; set; }
        public string DesignerId { get; set; }
        public string MerchandiserId { get; set; }
        public double CM { get; set; }
        public double MaterialCost { get; set; }
        public int ItemId { get; set; }
        public string ProductCode { get; set; }
        public string ItemName { get; set; }
        public int ItemAttributeId { get; set; }
        public int ProductItemId { get; set; }
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public int AttributeValueId { get; set; }
        public string AttributeValueName { get; set; }
    }
}