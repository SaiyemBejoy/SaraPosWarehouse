using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class SetupModel
    {
    }

    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Required]
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }

        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
        public string Active_YN { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class SubCategoryModel
    {
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }

        public int SubCategoryId { get; set; }
        [Required]
        [DisplayName("Sub Category Name")]
        public string SubCategoryName { get; set; }

        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
        public string Active_YN { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }


    public class BrandModel
    {
        public int BrandId { get; set; }

        [Required]
        [DisplayName("Brand Name")]
        public string BrandName { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }


    }

    public class AttributeModel
    {
        public int AttributeId { get; set; }

        [Required]
        [DisplayName("Attribute Name")]
        public string AttributeName { get; set; }

        [Required]
        [DisplayName("Display Order")]
        public string DisplayOrder { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }


    }

    public class AttributeValue
    {
        [Required]
        [DisplayName("Attribute")]
        public int AttributeId { get; set; }

        [DisplayName("Attribute Name")]
        public string AttributeName { get; set; }

        public int AttributeValueId { get; set; }
        [Required]
        [DisplayName("Attribute Value Name")]
        public string AttributeValueName { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class VendorModel
    {
        public int VendorId { get; set; }

        [Required]
        [DisplayName("Vendor Name")]
        public string VendorName { get; set; }
        [Required]
        [DisplayName("Address")]
        public string Address { get; set; }

        [Required]
        [DisplayName("Contact No")]
        [StringLength(14, MinimumLength = 11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact_No  must be Number")]
        public string ContactNo { get; set; }

        [Required]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }

        public string Active_YN { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }


    }

    public class UnitModel
    {
        public int UnitId { get; set; }

        [Required]
        [DisplayName("Unit Name")]
        public string UnitName { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }


    }

    public class CardTypeModel
    {
        public int CardTypeId { get; set; }

        [Required]
        [DisplayName("Card Type Name")]
        public string CardTypeName { get; set; }

        public string UpdateBy { get; set; }
        public string WareHouseId { get; set; }

        public string ShopId { get; set; }


    }

    public class CustomerTypeModel
    {
        public int CustomerTypeId { get; set; }

        [Required]
        [DisplayName("Customer Type Name")]
        public string CustomerTypeName { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }


    }

    public class PaymentTypeModel
    {
        public int PaymentTypeId { get; set; }

        [Required]
        [DisplayName("Payment Type Name")]
        public string PaymentTypeName { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }

    }

    public class CountryModel
    {
        public int CountryId { get; set; }

        [Required]
        [DisplayName("Country Name")]
        public string CountryName { get; set; } 

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }


    }

    public class WareHouseModel
    {
        [Required]
        [DisplayName("Country")]
        public int CountryId { get; set; } = 1;

        [DisplayName("Country Name")]
        public string CountryName { get; set; }

        public int WareHouseId { get; set; }
        [Required]
        [DisplayName("Ware House Name")]
        public string WareHouseName { get; set; }

        [Required]
        [DisplayName("Contact No")]
        [MaxLength(11)]
        [MinLength(11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact_No  must be Number.")]
        public string ContactNo { get; set; }

        [Required]
        [DisplayName("Postal Code")]
        [MaxLength(4)]
        [MinLength(4)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Postal Code must be Number(4-Digit).")]
        public string PostalCode { get; set; }

        [Required]
        [DisplayName("Date Of Enrollment")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfEnrollment { get; set; }

        [Required]
        [DisplayName("VAT No")]
        public string VatNo { get; set; }

        [Required]
        [DisplayName("TIN No")]
        public string TINNo { get; set; }

        [Required]
        [DisplayName("BIN No")]
        public string BINNo { get; set; }

       
        [DisplayName("FAX No")]
        public string FAXNo { get; set; }

        [Required]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [DisplayName(" Address")]
        public string WareHouseAddress { get; set; }

        public string UpdateBy { get; set; }

    }

    public class ShopModel
    {
        [Required]
        [DisplayName("Country")]
        public int CountryId { get; set; }

        [DisplayName("Country Name")]
        public string CountryName { get; set; }

        [Required]
        [DisplayName("WareHouse")]
        public int WareHouseId { get; set; }

        
        [DisplayName(" WareHouse Name")]
        public string WareHouseName { get; set; }

        public int ShopId { get; set; }
        [Required]
        [DisplayName("Shop Name")]
        public string ShopName { get; set; }

        [Required]
        [DisplayName("Contact No")]
        [MaxLength(11)]
        [MinLength(11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact_No  must be Number.")]
        public string ContactNo { get; set; }

        [Required]
        [DisplayName("Postal Code")]
        [MaxLength(4)]
        [MinLength(4)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Postal Code must be Number(4-Digit).")]
        public string PostalCode { get; set; }

        [Required]
        [DisplayName("Date Of Enrollment")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfEnrollment { get; set; }

        [Required]
        [DisplayName("VAT No")]
        public string VatNo { get; set; }

        [Required]
        [DisplayName("TIN No")]
        public string TINNo { get; set; }

        [Required]
        [DisplayName("BIN No")]
        public string BINNo { get; set; }

       
        [DisplayName("FAX No")]
        public string FAXNo { get; set; }

        [Required]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [DisplayName(" Address")]
        public string ShopAddress { get; set; }

        public string ShopUrl { get; set; }

        public bool IsActive { get; set; }

        public string UpdateBy { get; set; }

    }

    public class GiftVoucherGenerateModel
    {

        [DisplayName("Card Type")]
        public int CardTypeId { get; set; }
        [DisplayName("Card Type Name")]
        public string CardTypeName { get; set; }

        public int GiftVoucherId { get; set; }

        public int GiftVoucherValue { get; set; }
        [Required]
        [DisplayName("No Of Card")]
        public string NoOfCard { get; set; }

        [Required]
        [DisplayName("Start From")]
        public string StartFrom { get; set; }

        [Required]
        [DisplayName("End From")]
        public string EndFrom { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }
    public class GiftVoucherGenerateItemModel
    {

        public int GiftVoucherId { get; set; }

        public int GiftVoucherValue { get; set; }

        public string GiftVoucherCode { get; set; }

    }
    public class GiftVoucherGeneratePrintItemModel
    {

        public int GiftVoucherId { get; set; }

        public string GiftVoucherValue { get; set; }

        public string GiftVoucherCode { get; set; }

        public int Quantity { get; set; }

        public string GiftVoucherCodeImageString { get; set; }

        public byte[] GiftVoucherCodeImageArray { get; set; }

    }
    public class GiftVoucherDeliveryModel
    {
        [DisplayName("Shop Name")]
        public int ShopId { get; set; }

        public string ShopName { get; set; }

        public int GiftVoucherDeliveryId { get; set; }

        public int GiftVoucherId { get; set; }

        public string GiftVoucherCode { get; set; }

        public string GiftVoucherValue { get; set; }

        public string GiftVoucherRemainingValue { get; set; }

        public string DeliveryDate { get; set; }

        public string WareHouseId { get; set; }

        public string UpdateBy { get; set; }

        public string CreatedBy { get; set; }

    }

    public class GiftVoucherDeliveryApiModel
    {
        public int GiftVoucherDeliveryId { get; set; }
        public int GiftVoucherId { get; set; }
        public int DeliveryItemNum { get; set; }
        public string GiftVoucherCode { get; set; }
        public string GiftVoucherValue { get; set; }

        public string GiftVoucherCustomerName { get; set; }//giftvoucher Deposit korar jonno save kora hoise
        public string GiftVoucherCustomerPhone { get; set; }//giftvoucher Deposit korar jonno save kora hoise
        public string DepositShopId { get; set; }

        public double RemainingValue { get; set; }
        public string DeliveryDate { get; set; }
        public string WareHouseId { get; set; }
        public string DepositYN{ get; set; }
        public string UpdateBy { get; set; }

    }

    public class GiftVoucherDepositModel
    {
        [Required]
        public int GiftVoucherDepositId { get; set; }
        [Required]
        public int GiftVoucherId { get; set; }
        [Required]
        [DisplayName("Gift Voucher Code")]
        public string GiftVoucherCode { get; set; }
        [Required]
        [DisplayName("Gift Voucher Value")]
        public string GiftVoucherValue { get; set; }
        [Required]
        [DisplayName("Customer Name")]
        public string GiftVoucherCustomerName { get; set; }
        [Required]
        [DisplayName("Customer's Phone No")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "You must enter 11 digits")]
        public string GiftVoucherCustomerPhone { get; set; }
        [Required]
        [DisplayName("Deposit From")]
        public int DepositShopId { get; set; }
        public string DepositShopName { get; set; }
        public string CreateBy { get; set; }
        public string CreateDate { get; set; }
    }

    public class DiscountPolicyModel
    {      
        public int DiscountPolicyId { get; set; }

        [Required]
        [DisplayName("Discount(%)")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Discount  must be Number.")]
        public string DiscountPercent { get; set; }

       
        [Required]
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string StartDate { get; set; }
   
        [Required]
        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string EndDate { get; set; }

        [DisplayName("Shop Name")]
        [Required]
        public int ShopId { get; set; }
        public string ShopName { get; set; }


        [Required]
        [DisplayName("Customer Type")]
        public int CustomerTypeId { get; set; }
        public string CustomerTypeName { get; set; }

        public string WareHouseId { get; set; }

        public string UpdateBy { get; set; }

        //view a List of Data aner jonno.
       //public List<CategoryModel> CategoryList { get; set; }
       //public List<SubCategoryModel> SubCategoryList { get; set; }

       // 1 ta discountID er jonno list er data pathanor jonno.
       public IEnumerable<int> SelectCategoryList { get; set; }
       public IEnumerable<int> SelectSubCategoryList { get; set; }

    }

    public class DiscountPolicyCategory
    {


        public int DiscountPolicyId { get; set; }


        public int DiscountPolicyCategoryId { get; set; }


        public int CategoryId { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

    }

    public class DiscountPolicySubCategory
    {


        public int DiscountPolicyId { get; set; }


        public int DiscountPolicySubCategoryId { get; set; }


        public int SubCategoryId { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

    }

    // Discount policy edit korar somoy selected id gulo anar jonno ai viewmodel use kora hoise 
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }

    public class SubCategoryViewModel
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public bool IsActive { get; set; }
    }
    //end

    public class VatModel
    {
        public int VatId { get; set; }

        [Required]
        [DisplayName("Vat Percent(%)")]
        public string VatPercent { get; set; }

        [DisplayName("Include Vat")]
        public bool VatStatus { get; set; }
        public string IncludeVat { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class CustomerModel
    {
        public int CustomerId { get; set; }
        [Required]
        [DisplayName("Code")]
        public string CustomerCode { get; set; }
        [Required]
        [DisplayName("Customer Type")]
        public int CustomerTypeId { get; set; }

        [DisplayName("Customer Type Name")]
        public string CustomerTypeName { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string CustomerFirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string CustomerLastName { get; set; }
        [DisplayName("Date Of Birth")]
        public string DateOfBirth { get; set; }
        [Required]
        [DisplayName("Date Of Enrollment")]
        public string DateOfEnrollment { get; set; }
        [DisplayName("Date Of Expire")]
        public string DateOfExpire { get; set; }
        [Required]
        [DisplayName("Contact No")]
        public string ContactNo { get; set; }
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        public string City { get; set; }
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [DisplayName("Country")]
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        [Required]
        [DisplayName("Discount(%)")]
        public string DiscountPercent { get; set; }

        [DisplayName("WholeSale Customer")]
        public bool WholeSaleCustomerStatus { get; set; }
        public string WholeSaleCustomer { get; set; }

        [DisplayName("Store Customer")]
        public bool StoreCustomerStatus { get; set; }
        public string StoreCustomer { get; set; }

        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
        public string Active_YN { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }

    public class RegisterInfoModel
    {
        public int RegisterId { get; set; }

        [Required]
        [DisplayName("Person Name")]
        public string RegisterPersonName { get; set; }
        [Required]
        [DisplayName("BIN NO")]
        public string BinNO { get; set; }
        [Required]
        [DisplayName("Address")]
        public string Address { get; set; }

        public string UpdateBy { get; set; }

    }

    public class SeasonModel
    {
        public int SeasonId { get; set; }

        [Required]
        [DisplayName("Season Name")]
        public string SeasonName { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }


}