using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class QuickSearchModel
    {
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int ShopId { get; set; }
        public string ShopName { get; set; }
        //public string WarehouseId { get; set; }
        //public string WarehouseName { get; set; }

        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }

        public string MerchandiserId { get; set; }

        public string DesignerId { get; set; }

        public string ProductStyle { get; set; }

        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public double? PurchasePrice { get; set; }
        public double? SalePrice { get; set; }
        public double? Vat { get; set; }
        public int? Stock { get; set; }
        //public int? WarehouseStock { get; set; }
        //public int? ShopStock { get; set; }
    }
    public class QuickSearchModelDataTable
    {
  
        public string CategoryName { get; set; }
        public string ShopName { get; set; }
        public string SubCategoryName { get; set; }
        public string ProductStyle { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public double Vat { get; set; }
        public int Stock { get; set; }
    }

    public class SaleCustomerSearchModel
    {
        public string CustomerCode { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string CustomerName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string EnrollDate { get; set; }
        public string Discount { get; set; }
    }

    public class SaleCustomerSearchModelDataTable
    {

        public string CUSTOMER_CODE { get; set; }
        public int SHOP_ID { get; set; }
        public string SHOP_NAME { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CONTACT_NO { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string ENROLMENT_DATE { get; set; }
        public string DISCOUNT { get; set; }
    }
}