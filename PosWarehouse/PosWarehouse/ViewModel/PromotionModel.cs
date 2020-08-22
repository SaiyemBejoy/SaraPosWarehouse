using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class PromotionModel
    {
    }


    public class CircularPriceChangeMain
    {
        public int CircularId { get; set; }
   
        [Required]
        public string CircularName { get; set; }
        [Required]
        public string EffectiveDate { get; set; }
        //[DisplayName("Shop Name")]
        //public int ShopId { get; set; }

        //public string ShopName { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        [DisplayName("Shop")]
        public string ShopId { get; set; }

        public string SelectALlShop { get; set; }

        public string CheckWarehouseYN { get; set; }

        public bool IsShopPriceChange { get; set; }

        // 1 ta circularId er jonno list er data pathanor jonno.
        public IEnumerable<CircularPriceChangeSub> CircularPriceChangeSubList { get; set; }
        public CircularPriceChangeSub CircularPriceChangeSub { get; set; }
        // 1 ta circularId er jonno list er data pathanor jonno.
        public IEnumerable<int> ShopList { get; set; }


    }

    public class ShopViewModel
    {
        public int ShopId { get; set; }  
        public string ShopName { get; set; }  
        public bool IsActive { get; set; }  
    }

    public class CircularPriceChangeSub
    {
      
   
        public int CircularId { get; set; }

        public int CircularPriceSubId { get; set; }
        [Required]
        [DisplayName("Barcode")]
        public string BarcodeNo { get; set; }

        [DisplayName("Purchase Price")]
        public string PurchasePrice { get; set; }

        public string PreSalePrice { get; set; }
        public string NewSalePrice { get; set; }
        //warehouse er price change korar jonno
        public string EffectiveDate { get; set; }
        //End
        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class CircularPriceChangeShop
    {


        public int CircularId { get; set; }


        public int CircularPriceChangeShopId { get; set; }


        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string UpdateBy { get; set; }


        public string ShopUrl { get; set; }

        public string WareHouseId { get; set; }

    }

    public class CircularDiscountPromotionMain
    {
        public int DiscountCircularId { get; set; }
        [Required]
        [DisplayName("Circular Name")]
        public string DiscountCircularName { get; set; }
        [Required]
        [DisplayName("Valid From")]
        public string ValidFrom { get; set; }
        [Required]
        [DisplayName("Valid To")]
        public string ValidTo { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        [DisplayName("Shop")]
        public string ShopId { get; set; }

        // 1 ta circularId er jonno list er data pathanor jonno.
        public IEnumerable<CircularDiscountPromotionSub> CircularDiscountPromotionSubList { get; set; }//use hosse na.Requirement Change Hoise
        public IEnumerable<CircularDiscountPromotionItem> CircularDiscountPromotionItemList { get; set; }
        // 1 ta circularId er jonno Shop_list er data pathanor jonno.
        public IEnumerable<int> CircularDiscountPromotionShopList { get; set; }

        public CircularDiscountPromotionSub CircularDiscountPromotionSub { get; set; }

    }

    public class CircularDiscountPromotionSub   // Aga Kora hoisilo....pore requirment change korsa
    {
        public int DiscountCircularId { get; set; }

        public int DiscountCircularSubId { get; set; }

        [Required]
        [DisplayName("Discount(%)")]
        public string Discount { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        [Required]
        public int SubCategoryId { get; set; }
        [DisplayName("Sub Category")]
        public string SubCategoryName { get; set; }
        [Required]
        public int StyleId { get; set; }
        [DisplayName("Style Name")]
        public string StyleName { get; set; }
        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }


    public class CircularDiscountPromotionItem
    {
        public int DiscountCircularId { get; set; }

        public int DiscountCircularItemId { get; set; }
        public string Barcode { get; set; }
        public string PurchasePrice { get; set; }
        public string SalePrice { get; set; }
        public double DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class CircularDiscountPromotionShop
    {

        public int DiscountCircularId { get; set; }

        public int ShopId { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

    }

    public class ShopPriceChange
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopUrl { get; set; }
        public bool IsShopUpdate { get; set; }
        public string ResponceStatus { get; set; }
    }

    public class CircularDiscountPromotionModelApi
    {
        public int DiscountCircularId { get; set; }
        public int DiscountCircularItemId { get; set; }
        public string DiscountCircularName { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string Barcode { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public double DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
        public string UpdateBy { get; set; }
        public string WareHouseId { get; set; }
        public int ShopId { get; set; }
    }

}