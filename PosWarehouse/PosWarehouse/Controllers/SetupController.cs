
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class SetupController : Controller
    {

        private readonly SetupDAL _objSetupDal = new SetupDAL();
        
        //For All Dropdown Load for this Object.
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();

        //End

        // GET: Setup
        #region "Common"       
        private string _strEmployeeId = "";
        private string _strWareHouseId = "";
        private string _strShopId = "";
        private string _strEmployeeRole = "";

        public void LoadSession()
        {
            var auth = Session["authentication"] as AuthModel;
            if (auth != null)
            {
                _strEmployeeId = auth.EmployeeId;
                _strWareHouseId = auth.WareHouseId;
                _strShopId = auth.ShopId;
                _strEmployeeRole = auth.EmployeeRole;
            }
            else
            {
                string url = Url.Action("Index", "Auth");
                if (url != null) Response.Redirect(url);
            }
        }

        #endregion
        #region "Category"
        [RoleFilter]
        public async Task<ActionResult> Category(int? categoryId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            CategoryModel model = new CategoryModel();

            var objCategoryModel = await _objSetupDal.GetCategoryList(_strWareHouseId, _strShopId);
            ViewBag.CategoryList = objCategoryModel;

            if (categoryId != null && categoryId != 0)
            {
                model = await _objSetupDal.GetACategory((int)categoryId, _strWareHouseId, _strShopId);
            }

            //ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateCategory(CategoryModel objCategoryModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objCategoryModel.UpdateBy = _strEmployeeId;
                objCategoryModel.Active_YN = objCategoryModel.ActiveStatus ? "Y" : "N";
                objCategoryModel.WareHouseId = _strWareHouseId;
                objCategoryModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateCategory(objCategoryModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Category");
        }

        public async Task<ActionResult> DeleteCategory(int categoryId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteCategory(categoryId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Category");
        }
        #endregion

        #region SubCategory
        [RoleFilter]
        public async Task<ActionResult> SubCategory(int? categoryId, int? subCategoryId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            SubCategoryModel model = new SubCategoryModel();

            var objSubCategoryModels = await _objSetupDal.GetSubCategoryList(_strWareHouseId, _strShopId);
            ViewBag.SubCategoryList = objSubCategoryModels;

            if (categoryId != null && categoryId != 0 && subCategoryId != null && subCategoryId != 0)
            {
                model = await _objSetupDal.GetASubCategory((int)categoryId, (int)subCategoryId, _strWareHouseId, _strShopId);
            }

            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
           
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateSubCategory(SubCategoryModel objSubCategoryModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objSubCategoryModel.UpdateBy = _strEmployeeId;
                objSubCategoryModel.Active_YN = objSubCategoryModel.ActiveStatus ? "Y" : "N";
                objSubCategoryModel.WareHouseId = _strWareHouseId;
                objSubCategoryModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateSubCategory(objSubCategoryModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("SubCategory");
        }

        public async Task<ActionResult> DeleteSubCategory(int categoryId, int subCategoryId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteSubCategory(categoryId, subCategoryId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("SubCategory");
        }

        #endregion

        #region "Brand Section"
        [RoleFilter]
        public async Task<ActionResult> Brand(int? brandId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            BrandModel model = new BrandModel();

            var objBrandModel = await _objSetupDal.GetBrandList(_strWareHouseId, _strShopId);
            ViewBag.BrandList = objBrandModel;

            if (brandId != null && brandId != 0)
            {
                model = await _objSetupDal.GetABrand((int)brandId, _strWareHouseId, _strShopId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateBrand(BrandModel objBrandModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objBrandModel.UpdateBy = _strEmployeeId;
                objBrandModel.WareHouseId = _strWareHouseId;
                objBrandModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateBrand(objBrandModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Brand");
        }

        public async Task<ActionResult> DeleteBrand(int brandId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteBrand(brandId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Brand");
        }
        #endregion

        #region "Attribute Section"
        [RoleFilter]
        public async Task<ActionResult> Attribute(int? attributeId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            AttributeModel model = new AttributeModel();

            var objAttributeModel = await _objSetupDal.GetAttributeList(_strWareHouseId, _strShopId);
            ViewBag.AttributeList = objAttributeModel;

            if (attributeId != null && attributeId != 0)
            {
                model = await _objSetupDal.GetAAttribute((int)attributeId, _strWareHouseId, _strShopId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateAttribute(AttributeModel objAttributeModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objAttributeModel.UpdateBy = _strEmployeeId;
                objAttributeModel.WareHouseId = _strWareHouseId;
                objAttributeModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateAttribute(objAttributeModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Attribute");
        }

        public async Task<ActionResult> DeleteAttribute(int attributeId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteAttribute(attributeId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Attribute");
        }
        #endregion


        #region Attribute Value Section 
        [RoleFilter]
        public async Task<ActionResult> AttributeValue(int? attributeId, int? attributeValueId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            AttributeValue model = new AttributeValue();

            var objAttributeValue = await _objSetupDal.GetAttributeValueList(_strWareHouseId, _strShopId);
            ViewBag.AttributeValueList = objAttributeValue;

            if (attributeId != null && attributeId != 0 && attributeValueId != null && attributeValueId != 0)
            {
                model = await _objSetupDal.GetAAttributeValue((int)attributeId, (int)attributeValueId, _strWareHouseId, _strShopId);
            }

            ViewBag.AttributeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAttributeListDropdown(), "ATTRIBUTE_ID", "ATTRIBUTE_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateAttributeValue(AttributeValue objAttributeValue)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objAttributeValue.UpdateBy = _strEmployeeId;
                objAttributeValue.WareHouseId = _strWareHouseId;
                objAttributeValue.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateAtributeValue(objAttributeValue);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("AttributeValue");
        }

        public async Task<ActionResult> DeleteAttributeValue(int attributeId, int attributeValueId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteAttributeValue(attributeId, attributeValueId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("AttributeValue");
        }

        #endregion


        #region "Vendor Section"
        [RoleFilter]
        public async Task<ActionResult> Vendor(int? vendorId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            VendorModel model = new VendorModel();

            var objVendorModel = await _objSetupDal.GetVendorList(_strWareHouseId, _strShopId);
            ViewBag.VendorList = objVendorModel;

            if (vendorId != null && vendorId != 0)
            {
                model = await _objSetupDal.GetAVendor((int)vendorId, _strWareHouseId, _strShopId);

            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateVendor(VendorModel objVendorModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objVendorModel.UpdateBy = _strEmployeeId;
                objVendorModel.Active_YN = objVendorModel.ActiveStatus ? "Y" : "N";
                objVendorModel.WareHouseId = _strWareHouseId;
                objVendorModel.ShopId = _strShopId;



                string strMessage = await _objSetupDal.SaveAndUpdateVendor(objVendorModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Vendor");
        }

        public async Task<ActionResult> DeleteVendor(int vendorId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteVendor(vendorId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Vendor");
        }
        #endregion

        #region "Unit Section"
        [RoleFilter]
        public async Task<ActionResult> Unit(int? unitId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            UnitModel model = new UnitModel();

            var objUnitModel = await _objSetupDal.GetUnitList(_strWareHouseId, _strShopId);
            ViewBag.UnitList = objUnitModel;

            if (unitId != null && unitId != 0)
            {
                model = await _objSetupDal.GetAUnit((int)unitId, _strWareHouseId, _strShopId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateUnit(UnitModel objUnitModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objUnitModel.UpdateBy = _strEmployeeId;
                objUnitModel.WareHouseId = _strWareHouseId;
                objUnitModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateUnit(objUnitModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Unit");
        }

        public async Task<ActionResult> DeleteUnit(int unitId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteUnit(unitId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Unit");
        }
        #endregion

        #region "Card Type Section"
        [RoleFilter]
        public async Task<ActionResult> CardType(int? cardtypeId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            CardTypeModel model = new CardTypeModel();

            var objCardTypeModel = await _objSetupDal.GetCardTypeList(_strWareHouseId, _strShopId);
            ViewBag.CardTypeList = objCardTypeModel;

            if (cardtypeId != null && cardtypeId != 0)
            {
                model = await _objSetupDal.GetACardType((int)cardtypeId, _strWareHouseId, _strShopId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateCardType(CardTypeModel objCardTypeModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objCardTypeModel.UpdateBy = _strEmployeeId;
                objCardTypeModel.WareHouseId = _strWareHouseId;
                objCardTypeModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateCardType(objCardTypeModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("CardType");
        }

        public async Task<ActionResult> DeleteCardType(int cardtypeId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteCardType(cardtypeId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("CardType");
        }
        #endregion

        #region "Customer Type Section"
        [RoleFilter]
        public async Task<ActionResult> CustomerType(int? customerTypeId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            CustomerTypeModel model = new CustomerTypeModel();

            var objCustomerTypeModel = await _objSetupDal.GetCustomerTypeList(_strWareHouseId, _strShopId);
            ViewBag.CustomerTypeList = objCustomerTypeModel;

            if (customerTypeId != null && customerTypeId != 0)
            {
                model = await _objSetupDal.GetACustomerType((int)customerTypeId, _strWareHouseId, _strShopId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateCustomerType(CustomerTypeModel objCustomerTypeModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objCustomerTypeModel.UpdateBy = _strEmployeeId;
                objCustomerTypeModel.WareHouseId = _strWareHouseId;
                objCustomerTypeModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateCustomerType(objCustomerTypeModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("CustomerType");
        }

        public async Task<ActionResult> DeleteCustomerType(int customerTypeId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteCustomerType(customerTypeId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("CustomerType");
        }
        #endregion


        #region "Payment Type Section"
        [RoleFilter]
        public async Task<ActionResult> PaymentType(int? paymentTypeId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            PaymentTypeModel model = new PaymentTypeModel();

            var objPaymentTypeModel = await _objSetupDal.GetPaymentTypeList(_strWareHouseId, _strShopId);
            ViewBag.PaymentTypeTypeList = objPaymentTypeModel;

            if (paymentTypeId != null && paymentTypeId != 0)
            {
                model = await _objSetupDal.GetAPaymentType((int)paymentTypeId, _strWareHouseId, _strShopId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdatePaymentType(PaymentTypeModel objPaymentTypeModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objPaymentTypeModel.UpdateBy = _strEmployeeId;
                objPaymentTypeModel.WareHouseId = _strWareHouseId;
                objPaymentTypeModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdatePaymentType(objPaymentTypeModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("PaymentType");
        }

        public async Task<ActionResult> DeletePaymentType(int paymentTypeId)
        {
            LoadSession();
            string message = await _objSetupDal.DeletePaymentType(paymentTypeId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("PaymentType");
        }
        #endregion

        #region "Country Section"
        [RoleFilter]
        public async Task<ActionResult> Country(int? countryid)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            CountryModel model = new CountryModel();

            var objCountryModel = await _objSetupDal.GetCountryList(_strWareHouseId, _strShopId);
            ViewBag.CountryList = objCountryModel;

            if (countryid != null && countryid != 0)
            {
                model = await _objSetupDal.GetACountry((int)countryid, _strWareHouseId, _strShopId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateCountry(CountryModel objCountryModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objCountryModel.UpdateBy = _strEmployeeId;
                objCountryModel.WareHouseId = _strWareHouseId;
                objCountryModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateCountry(objCountryModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Country");
        }

        public async Task<ActionResult> DeleteCountry(int countryid)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteCountry(countryid, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Country");
        }
        #endregion

        #region WAREHOUSE section
        [RoleFilter]
        public async Task<ActionResult> WareHouse(int? countryId, int? wareHouseId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            WareHouseModel model = new WareHouseModel();

            var objWareHouseModels = await _objSetupDal.GetWareHouseList();
            ViewBag.WareHouseList = objWareHouseModels;

            if (countryId != null && countryId != 0 && wareHouseId != null && wareHouseId != 0)
            {
                model = await _objSetupDal.GetAWareHouse((int)countryId, (int)wareHouseId);
            }

            ViewBag.CountryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCountryListDropdown(), "COUNTRY_ID", "COUNTRY_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateWareHouseModel(WareHouseModel objWareHouseModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objWareHouseModel.UpdateBy = _strEmployeeId;

                string strMessage = await _objSetupDal.SaveAndUpdateWareHouse(objWareHouseModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("WareHouse");
        }

        public async Task<ActionResult> DeleteWareHouse(int countryId, int wareHouseId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteWareHouse(countryId, wareHouseId);
            TempData["message"] = message;

            return RedirectToAction("WareHouse");
        }

        #endregion

        #region Shop Section
        [RoleFilter]
        public async Task<ActionResult> Shop(int? countryId, int? wareHouseId, int? shopId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            ShopModel model = new ShopModel();

            var objShopModels = await _objSetupDal.GetShopList(null);
            ViewBag.ShopList = objShopModels;

            if (countryId != null && countryId != 0 && wareHouseId != null && wareHouseId != 0 && shopId != null && shopId != 0)
            {
                model = await _objSetupDal.GetAShop((int)countryId, (int)wareHouseId, (int)shopId);
            }

            ViewBag.CountryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCountryListDropdown(), "COUNTRY_ID", "COUNTRY_NAME");
            ViewBag.WareHouseList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetWareHouseListDropdown(), "WARE_HOUSE_ID", "WARE_HOUSE_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateShop(ShopModel objShopModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objShopModel.UpdateBy = _strEmployeeId;

                string strMessage = await _objSetupDal.SaveAndUpdateShop(objShopModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Shop");
        }

        public async Task<ActionResult> DeleteShop(int countryId, int warehouseId, int shopId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteShop(countryId, warehouseId, shopId);
            TempData["message"] = message;

            return RedirectToAction("Shop");
        }

        #endregion

        #region GiftVoucher Section
        [RoleFilter]
        public async Task<ActionResult> GiftVoucherGenerate(int? cardTypeId, int? giftVoucherId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            GiftVoucherGenerateModel model = new GiftVoucherGenerateModel();

            var objGiftVoucherGenerateModels = await _objSetupDal.GetGiftVoucherGenerateList(_strWareHouseId, _strShopId);
            ViewBag.GiftVoucherGenerateModelList = objGiftVoucherGenerateModels;

            if (cardTypeId != null && cardTypeId != 0 && giftVoucherId != null && giftVoucherId != 0)
            {
                model = await _objSetupDal.GetAGiftVoucherGenerate((int)cardTypeId, (int)giftVoucherId, _strWareHouseId, _strShopId);
            }

            ViewBag.CardTypeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCardtypeListDropdown(), "CARD_TYPE_ID", "CARD_TYPE_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateGiftVoucherGenerate(GiftVoucherGenerateModel objGiftVoucherGenerateModel)
        {
            LoadSession();
            string giftItem = "";
            if (ModelState.IsValid)
            {
                objGiftVoucherGenerateModel.UpdateBy = _strEmployeeId;
                objGiftVoucherGenerateModel.WareHouseId = _strWareHouseId;
                objGiftVoucherGenerateModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateGiftVoucherGenerate(objGiftVoucherGenerateModel);
                if (strMessage != null)
                {
                    objGiftVoucherGenerateModel.GiftVoucherId = Convert.ToInt32(strMessage);
                     giftItem = await _objSetupDal.SaveGiftVoucherItem(objGiftVoucherGenerateModel);
                }
                TempData["message"] = giftItem;
            }
            return RedirectToAction("GiftVoucherGenerate");
        }
        public async Task<ActionResult> DeleteGiftvoucherGenerate(int cardTypeId, int giftVoucherId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteGiftVoucherGenerate(cardTypeId, giftVoucherId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("GiftVoucherGenerate");
        }

        public async Task<JsonResult> GiftVoucherItemListById(string giftVoucherId)
        {
            var data = await _objSetupDal.GetGiftVoucherGenerateItemList(giftVoucherId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> PrintAllGiftVoucherCode(List<GiftVoucherGeneratePrintItemModel> objGiftVoucherGeneratePrintItemModel)
        {
            List<string> giftVoucherCode = new List<string>();
            foreach (var data in objGiftVoucherGeneratePrintItemModel)
            {
                giftVoucherCode.Add(data.GiftVoucherCode);
                data.GiftVoucherCodeImageString = "data:image/png;base64," + Convert.ToBase64String(UtilityClass.RenderBarcode(data.GiftVoucherCode));
                data.GiftVoucherCodeImageArray = UtilityClass.RenderBarcode(data.GiftVoucherCode);
            }

            if (objGiftVoucherGeneratePrintItemModel.Any())
            {

                LoadSession();
                //var deletemessage = await _objBarCodeDAL.DeletePreviousBarCodeDatafromTable();
                var message = await _objSetupDal.SavePrintedGiftVoucherCode(objGiftVoucherGeneratePrintItemModel, _strEmployeeId);
                //Session["ItemCode"] = itemCode;

            }
            return Json(objGiftVoucherGeneratePrintItemModel, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> GiftVoucherLastStartFrom()
        {
            var data = await _objSetupDal.GiftVoucherLastStartFrom();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Gift Voucher Delivery Section
        [RoleFilter]
        public async Task<ActionResult> GiftVoucherDelivery(int? cardTypeId, int? shopId, int? giftVoucherDeliveryId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            GiftVoucherDeliveryModel model = new GiftVoucherDeliveryModel();

            var objGiftVoucherDeliveryModels = await _objSetupDal.GetGiftVoucherDeliveryList(_strWareHouseId);
            ViewBag.GiftVoucherDeliveryList = objGiftVoucherDeliveryModels;

            //if (cardTypeId != null && cardTypeId != 0 && giftVoucherDeliveryId != null && giftVoucherDeliveryId != 0 && shopId != null && shopId != 0)
            //{
            //    model = await _objSetupDal.GetAGiftVoucherDelivery((int)cardTypeId, (int)shopId, (int)giftVoucherDeliveryId, _strWareHouseId);
            //}

            //ViewBag.CardTypeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCardTypeListDropdown(), "CARD_TYPE_ID", "CARD_TYPE_NAME");
            //ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.GiftValueList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetGiftVouchervalueListDropdown(), "GIFT_VOUCHER_ID", "GIFT_VOUCHER_VALUE");
            return View(model);
        }

        public async Task<ActionResult> SaveAndUpdateGiftVoucherDelivery(List<GiftVoucherDeliveryModel> objGiftVoucherDeliveryModel)
        {
            LoadSession();
            string strMessage = "";
            foreach (var giftVoucherDeliveryModel in objGiftVoucherDeliveryModel)
            {
                giftVoucherDeliveryModel.UpdateBy = _strEmployeeId;
                giftVoucherDeliveryModel.WareHouseId = _strWareHouseId;
                 strMessage = await _objSetupDal.SaveAndUpdateGiftVoucherDelivery(giftVoucherDeliveryModel);
                TempData["message"] = strMessage;
            }
            var messageAndReload = new
            {
                m = strMessage,
                isRedirect = true,
                redirectUrl = Url.Action("GiftVoucherDelivery")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
            
        }

        public async Task<ActionResult> DeleteGiftVoucherDelivery(int cardTypeId, int shopId, int giftVoucherDeliveryId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteGiftVoucherDelivery(cardTypeId,shopId, giftVoucherDeliveryId,_strWareHouseId);
            TempData["message"] = message;

            return RedirectToAction("GiftVoucherDelivery");
        }

        #endregion

        #region "Discount Policy"
        [RoleFilter]
        public async Task<ActionResult> DiscountPolicy(int? discountPolicyId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            DiscountPolicyModel model = new DiscountPolicyModel();
            model.SelectCategoryList = new List<int>();
            model.SelectSubCategoryList = new List<int>();
            //All list of data display
            var objDiscountPolicyModel = await _objSetupDal.GetDiscountPolicyList(_strWareHouseId);
            ViewBag.DiscountPolicyList = objDiscountPolicyModel;
            //end

            if (discountPolicyId != null && discountPolicyId > 0)
            {
                model = await _objSetupDal.GetADiscountPolicy((int)discountPolicyId, _strWareHouseId);
                model.SelectCategoryList = await _objSetupDal.GetDiscountPolicyCategoryList((int)discountPolicyId, _strWareHouseId);
                model.SelectSubCategoryList = await _objSetupDal.GetDiscountPolicySubCategoryList((int)discountPolicyId, _strWareHouseId);
            }

            var objSelectedCategoryViewModel = await _objSetupDal.GetselectedCategoryList();
            ViewBag.SelectedCategory = objSelectedCategoryViewModel;

            var objSelectedSubCategoryViewModel = await _objSetupDal.GetselectedSubCategoryList();
            ViewBag.SelectedSubCategory = objSelectedSubCategoryViewModel;

            // category and subcategory er data checkbox a display koranor jonno
            //model.CategoryList = await _objSetupDal.GetCategoryList(_strWareHouseId,_strShopId);
           // model.SubCategoryList = await _objSetupDal.GetSubCategoryList(_strWareHouseId,_strShopId);
            //end
            //dropdown a data show koranor jonno
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.CustomerTypeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCustomerTypeListDropdown(), "CUSTOMER_TYPE_ID", "CUSTOMER_TYPE_NAME");
             //end

            return View(model);
        }

    
        public async Task<ActionResult> SaveAndUpdateDiscountPolicy(DiscountPolicyModel objDiscountPolicyModel)
        {
            LoadSession();
            string returnMessage = "";
            if (ModelState.IsValid)
            {
                objDiscountPolicyModel.UpdateBy = _strEmployeeId;
                objDiscountPolicyModel.WareHouseId = _strWareHouseId;


                try
                {
                    string strMessagedata = await _objSetupDal.SaveAndUpdateDiscountPolicy(objDiscountPolicyModel);

                    if (Convert.ToInt32(strMessagedata) > 0 || strMessagedata != null)
                    {
                        // this for category Save
                        DiscountPolicyCategory model = new DiscountPolicyCategory();
                        model.UpdateBy = _strEmployeeId;
                        model.WareHouseId = _strWareHouseId;
                        model.DiscountPolicyId = Convert.ToInt32(strMessagedata);

                        foreach (var category in objDiscountPolicyModel.SelectCategoryList)
                        {
                            model.CategoryId = category;
                            returnMessage = await _objSetupDal.SaveAndUpdateSubDiscountPolicyCategory(model);
                        }
                        //end

                        // this for subcategory Save
                        DiscountPolicySubCategory model2 = new DiscountPolicySubCategory();
                        model2.UpdateBy = _strEmployeeId;
                        model2.WareHouseId = _strWareHouseId;
                        model2.DiscountPolicyId = Convert.ToInt32(strMessagedata);

                        foreach (var subcategory in objDiscountPolicyModel.SelectSubCategoryList)
                        {
                            model2.SubCategoryId = subcategory;
                            returnMessage = await _objSetupDal.SaveAndUpdateSubDiscountPolicySubCategory(model2);
                        }
                        //end
                    }
                    
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }

            }
            var data = new
            {

                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("DiscountPolicy")
            };

            return Json(data);
        }

        public async Task<ActionResult> DeleteCircularPriceChange(int discountPolicyId)
         {
            LoadSession();
            string message = await _objSetupDal.DeleteDiscountPolicy(discountPolicyId, _strWareHouseId);
            TempData["message"] = message;

            return RedirectToAction("DiscountPolicy");
        }

        #endregion

        #region Vat Section
        [RoleFilter]
        public async Task<ActionResult> Vat(int? vatId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            VatModel model = new VatModel();

            var objVatModel = await _objSetupDal.GetVatList(_strWareHouseId, _strShopId);
            ViewBag.VatList = objVatModel;

            if (vatId != null && vatId != 0)
            {
                model = await _objSetupDal.GetAVat((int)vatId, _strWareHouseId, _strShopId);
            }
            //ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateVat(VatModel objVatModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objVatModel.UpdateBy = _strEmployeeId;
                objVatModel.IncludeVat = objVatModel.VatStatus ? "Y" : "N";
                objVatModel.WareHouseId = _strWareHouseId;
                objVatModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateVat(objVatModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Vat");
        }

        #endregion

        #region Customer
        [RoleFilter]
        public async Task<ActionResult> Customer(int? customerId, int? customerTypeId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            CustomerModel model = new CustomerModel();

            var objSubCustomerModels = await _objSetupDal.GetcustomerList(_strWareHouseId, _strShopId);
            ViewBag.customerList = objSubCustomerModels;

            if (customerTypeId != null && customerTypeId != 0 && customerId != null && customerId != 0)
            {
                model = await _objSetupDal.GetAcustomer((int)customerId, (int)customerTypeId, _strWareHouseId, _strShopId);
            }

            ViewBag.customerTypeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCustomerTypeListDropdown(), "CUSTOMER_TYPE_ID", "CUSTOMER_TYPE_NAME");
            ViewBag.countryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCountryListDropdown(), "COUNTRY_ID", "COUNTRY_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateCustomer(CustomerModel objCustomerModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objCustomerModel.UpdateBy = _strEmployeeId;
                objCustomerModel.StoreCustomer = objCustomerModel.StoreCustomerStatus ? "Y" : "N";
                objCustomerModel.WholeSaleCustomer = objCustomerModel.WholeSaleCustomerStatus ? "Y" : "N";
                objCustomerModel.Active_YN = objCustomerModel.ActiveStatus ? "Y" : "N";
                objCustomerModel.WareHouseId = _strWareHouseId;
                objCustomerModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateCustomer(objCustomerModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Customer");
        }

        public async Task<ActionResult> DeleteCustomer(int customerId, int customerTypeId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteCustomer(customerId, customerTypeId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Customer");
        }

        #endregion

        #region "Register Info Model"
        [RoleFilter]
        public async Task<ActionResult> RegisterInfo(int? registerId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            RegisterInfoModel model = new RegisterInfoModel();

            var objRegisterInfoModel = await _objSetupDal.GetRegisterInfoList();
            ViewBag.RegisterInfoList = objRegisterInfoModel;

            if (registerId != null && registerId != 0)
            {
                model = await _objSetupDal.GetARegisterInfo((int)registerId);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateRegisterInfo(RegisterInfoModel objRegisterInfoModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objRegisterInfoModel.UpdateBy = _strEmployeeId;
                string strMessage = await _objSetupDal.SaveAndUpdateRegisterInfo(objRegisterInfoModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("RegisterInfo");
        }

        public async Task<ActionResult> DeleteRegisterInfo(int registerId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteRegisterInfo(registerId);
            TempData["message"] = message;

            return RedirectToAction("RegisterInfo");
        }
        #endregion

        #region "Season"
        [RoleFilter]
        public async Task<ActionResult> Season(int? seasonId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            SeasonModel model = new SeasonModel();

            var objSeasonModel = await _objSetupDal.GetSeasonList(_strWareHouseId, _strShopId);
            ViewBag.SeasonList = objSeasonModel;

            if (seasonId != null && seasonId != 0)
            {
                model = await _objSetupDal.GetASeason((int)seasonId, _strWareHouseId, _strShopId);
            }
            //ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateSeason(SeasonModel objSeasonModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objSeasonModel.UpdateBy = _strEmployeeId;
                objSeasonModel.WareHouseId = _strWareHouseId;
                objSeasonModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateSeason(objSeasonModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Season");
        }

        public async Task<ActionResult> DeleteSeason(int seasonId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteSeason(seasonId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Season");
        }
        #endregion
    }
}