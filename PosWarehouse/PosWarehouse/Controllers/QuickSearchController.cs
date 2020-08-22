using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class QuickSearchController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly QuickSearchDal _objQuickSearchDal = new QuickSearchDal();
        private readonly DataExchangeDal _objDataExchangeDal = new DataExchangeDal();

        #region Product Search
        [RoleFilter]
        public async Task<ActionResult> ProductSearch()
        {
            ViewBag.ShopList = GetSelectListByDataTableForAddStaticValue(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
            //ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");
            ViewBag.MerchandiserList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMerchandiserListDropdown(), "EMPLOYEE_ID", "EMPLOYEE_NAME");
            ViewBag.DesignerList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetDesignerListDropdown(), "EMPLOYEE_ID", "EMPLOYEE_NAME");
            ViewBag.StyleList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_STYLE", "PRODUCT_STYLE");
            return View();
        }
        public async Task<ActionResult> GetSubCategory(int categoryId)
        {
            var list = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(categoryId), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> GetAllProductList(QuickSearchModel objQuickSearchModel)
        //{
        //    var searchResult = await _objQuickSearchDal.GetAllProductList(objQuickSearchModel); 
        //    return Json(searchResult, JsonRequestBehavior.AllowGet);
        //}

        public async Task<ActionResult> GetAllProductList(DataTableQuickSearch model, QuickSearchModel objQuickSearchModel)
        {
            model.QuickSearchModel = objQuickSearchModel;
            var searchResult = await _objQuickSearchDal.GetAllProductList(model);
            List<QuickSearchModelDataTable> data = (List<QuickSearchModelDataTable>)model.ListOfData;

            return Json(new { model.draw, model.recordsTotal, model.recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }

        //This dropdownlist Use for Get Daynamic Value shathe Static Value Add Korar jonno .
        private static SelectList GetSelectListByDataTableForAddStaticValue(DataTable objDataTable, string pValueField, string pTextField)
        {
            List<SelectListItem> objSelectListItems = new List<SelectListItem>
            {
                new SelectListItem() {Value = "", Text = "--Select Shop--"},
                new SelectListItem() {Value = "331", Text = "Warehouse"}
            };
            objSelectListItems.AddRange(from DataRow dataRow in objDataTable.Rows
                select new SelectListItem()
                {
                    Value = dataRow[pValueField].ToString(),
                    Text = dataRow[pTextField].ToString()
                });

            return new SelectList(objSelectListItems, "Value", "Text");
        }
        //end
        #endregion

        #region Sale Customer Search
        [RoleFilter]
        public async Task<ActionResult> SaleCustomerSearch()
        {
            return View();
        }

        public async Task<ActionResult> ProcessData()
        {
            var shopUrl = await _objDataExchangeDal.GetAllShopUrlWithId();
            var message = "";
            using (var client = new HttpClient())
            {
                foreach (var url in shopUrl)
                {
                    if (url.ShopUrl != "")
                    {
                        client.BaseAddress = new Uri(url.ShopUrl);
                        var responseTask = client.GetAsync("SaleCustomerInfo");
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<IList<CustomerSaleModel>>();
                            readTask.Wait();

                            IEnumerable<CustomerSaleModel> customer = readTask.Result;
                            foreach (var value in customer)
                            {
                                value.ShopId = Convert.ToString(url.ShopId);
                                 message = await _objDataExchangeDal.SaveAndUpdateCustomerSale(value);
                            }

                        }
                    }
                }

            }
            var messageAndReload = new
            {
                m = message,
                isRedirect = true,
                redirectUrl = Url.Action("SaleCustomerSearch")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllSaleCustomerList(DataTablesSaleCustomerSearch model, SaleCustomerSearchModel objSaleCustomerSearchModel)
        {
            model.SaleCustomerSearchModel = objSaleCustomerSearchModel;
            var searchResult = await _objQuickSearchDal.GetAllSaleCustomerList(model);
            List<SaleCustomerSearchModelDataTable> data = (List<SaleCustomerSearchModelDataTable>)model.ListOfData;

            return Json(new { model.draw, model.recordsTotal, model.recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}