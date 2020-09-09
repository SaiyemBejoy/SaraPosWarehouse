using System;
using System.Collections.Generic;
using System.IO;
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
    public class ProductController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly ProductDal _objProductDal = new ProductDal();

        #region "Common"

        private string _strEmployeeId = "";
        private string _strWareHouseId = "";
        private string _strShopId = "";

        public void LoadSession()
        {
            if (Session["authentication"] is AuthModel auth)
            {
                _strEmployeeId = auth.EmployeeId;
                _strWareHouseId = auth.WareHouseId;
                _strShopId = auth.ShopId;
            }
            else
            {
                string url = Url.Action("Index", "Auth");
                if (url != null) Response.Redirect(url);
            }
        }
        #endregion
        [RoleFilter]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"] as string;
            }
            return View();
        }

        public ActionResult GetProductList(DataTableAjaxPostModel model)
        {
            LoadSession();

            ProductGrid objProductGrid = new ProductGrid
            {
                SearchBy = model.search?.value,
                UpdateBy = _strEmployeeId,
                WareHouseId = _strWareHouseId,
                ShopId = _strShopId
            };

            if (model.order != null)
            {
                objProductGrid.OrderByName = model.columns[model.order[0].column].data;
                objProductGrid.OrderByDirection = model.order[0].dir.ToUpper();
            }

            List<ProductGrid> data = _objProductDal.GetProductGrids(objProductGrid).ToList();
            foreach (var grid in data)
            {
                grid.ProductImage = Url.Content(grid.ProductImage);
            }

            int recordsFiltered = data.Count;
            int recordsTotal = data.Count;

            if (recordsTotal < model.length)
            {
                recordsTotal = model.length;
            }

            if (model.length == -1)
            {
                data = data.ToList();
            }
            else
            {
                data = data.Skip(model.start).Take(model.length).ToList();
            }

            return Json(new { model.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }


        [RoleFilter]
        public async Task<ActionResult> CreateOrEdit(int? productId)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"] as string;
            }

            LoadSession();
            ProductModel model = new ProductModel();
            if (productId != null && productId != 0)
            {

                model = await _objProductDal.GetAProduct((int)productId, _strWareHouseId, _strShopId);

            }
            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
            ViewBag.BrandList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetBrandListDropdown(), "BRAND_ID", "BRAND_NAME");
            //ViewBag.MerchandiserList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMerchandiserListDropdown(), "EMPLOYEE_ID", "EMPLOYEE_NAME");
            //ViewBag.DesignerList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetDesignerListDropdown(), "EMPLOYEE_ID", "EMPLOYEE_NAME");
            ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");
            ViewBag.AttributeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAttributeListDropdown(), "ATTRIBUTE_ID", "ATTRIBUTE_NAME");
            ViewBag.MeasuringUnitList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMeasuringUnitListDropdown(), "UNIT_ID", "UNIT_NAME");
            ViewBag.MaterialList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetMaterialListDropdown(), "MATERIAL_ID", "MATERIAL_NAME");
            ViewBag.OtherCostList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetOtherCostListDropdown(), "OTHER_COST_ID", "COST_PURPOSE");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveOrUpdate(ProductModel objProductModel)
        {
            int productId = 0;

            if (objProductModel.ProductId != 0)
            {
                ModelState.Remove("ProductImage");
            }
            if (!ModelState.IsValid)
            {
                if (objProductModel.ProductId != 0)
                    return RedirectToAction("CreateOrEdit", new { productId = objProductModel.ProductId });
                return RedirectToAction("CreateOrEdit");
            }

            ModelState.Clear();
            LoadSession();
            objProductModel.UpdateBy = _strEmployeeId;
            objProductModel.WareHouseId = _strWareHouseId;
            objProductModel.ShopId = _strShopId;

            string filePath = Server.MapPath("~/Files/ProductImage/");
            string vFileName = Guid.NewGuid().ToString();

            //if (objProductModel.ProductImage != null)
            if (objProductModel.ProductImage != null)
            {
                // var imageExtension = Path.GetExtension(objProductModel.ProductImage.FileName);
                var imageExtension = Path.GetExtension(objProductModel.ProductImage.FileName);
                if (imageExtension != null)
                {
                    imageExtension = imageExtension.ToUpper();

                    if (imageExtension == ".JPG" || imageExtension == ".JPEG" || imageExtension == ".PNG")
                    {
                        if (objProductModel.ProductId != 0)
                        {
                            string imagePath = filePath + objProductModel.ProductImageString;
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                        filePath += vFileName;
                        filePath += imageExtension.ToLower();

                        objProductModel.ProductImage.SaveAs(filePath);

                        objProductModel.ProductImageString = vFileName + imageExtension.ToLower();
                    }
                }

                if (objProductModel.ProductImageString != null)
                {

                    var strMessage = await _objProductDal.SaveProductInfo(objProductModel);
                    productId = Convert.ToInt32(strMessage[1]);
                    TempData["message"] = strMessage[0];
                }
                else
                {
                    LoadSession();

                    //load DropDown List
                    ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
                    //ViewBag.BrandList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetBrandListDropdown(), "BRAND_ID", "BRAND_NAME");
                    ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");

                    ViewBag.message = "Image must be jpg or png";

                    return View("CreateOrEdit", objProductModel);
                }

            }
            else if (objProductModel.ProductImageString != null)
            {
                var strMessage = await _objProductDal.SaveProductInfo(objProductModel);
                productId = Convert.ToInt32(strMessage[1]);
                TempData["message"] = strMessage[0];
            }

            if (productId != 0)
            {
                return RedirectToAction("CreateOrEdit", new { productId });
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public async Task<ActionResult> GetSubCategory(int categoryId)
        {
            var list = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(categoryId), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAttributeValue(int attributeId)
        {
            LoadSession();
            var attribute = await _objProductDal.GetAttributeValue(attributeId, _strWareHouseId, _strShopId);
            attribute = attribute.OrderBy(c => c.AttributeValue).ToList();
            return Json(attribute, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveProductAttribute(List<ProductAttribute> objProductAttributes)
        {
            LoadSession();

            var message = await _objProductDal.SaveProductAttributes(objProductAttributes, _strEmployeeId, _strWareHouseId, _strShopId);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetProductAttribute(int productId, double purchasePrice, double salePrice, double materialCost, double CM)
        {
            LoadSession();
            AttributesModel model = new AttributesModel();
            List<ProductItem> items = new List<ProductItem>();

            model.Attributes = await _objProductDal.GetProductAttribute(productId, _strWareHouseId, _strShopId);
            model.Attributes = model.Attributes.OrderBy(c => c.AttributeOrder).ToList();
            var allName = new List<List<string>>();

            var itemCode = _objProductDal.GetMaxItemCode();

            foreach (var attribute in model.Attributes)
            {
                attribute.AttributesValues = _objProductDal.GetProductAttributeValue(productId, attribute.AttributeId, _strWareHouseId, _strShopId, out var value);
                allName.Add(value);
            }

            var productItem = await _objProductDal.GetProductItem(productId, _strWareHouseId, _strShopId);

            if (model.Attributes.Any() && allName.Any())
            {
                var names = UtilityClass.GenerateAllCombinationsOfName(allName);

                foreach (var name in names)
                {
                    if (productItem.Any())
                    {
                        foreach (var data in productItem)
                        {
                            ProductItem item = new ProductItem();
                            if (data.ProductName.Trim() == name.Trim())
                            {
                                item.IsActive = true;
                                item.ProductId = data.ProductId;
                                item.ProductName = data.ProductName;
                                item.PurchasePrice = data.PurchasePrice;
                                item.SalePrice = data.SalePrice;
                                item.MaterialCost = data.MaterialCost;
                                item.CM = data.CM;
                                item.ItemId = data.ItemId;
                                item.ProductCode = data.ProductCode;

                                var itemToRemove = items.SingleOrDefault(r => r.ProductName == data.ProductName);
                                if (itemToRemove != null)
                                {
                                    items.Remove(itemToRemove);
                                }
                                items.Add(item);
                                break;
                            }
                            else
                            {
                                item.IsActive = false;
                                item.ProductId = productId;
                                item.ProductName = name;
                                item.PurchasePrice = purchasePrice;
                                item.SalePrice = salePrice;
                                item.MaterialCost = materialCost;
                                item.CM = CM;
                                item.ProductCode = itemCode.ToString("000000");

                                var itemToRemove = items.SingleOrDefault(r => r.ProductName == name);
                                if (itemToRemove != null)
                                {
                                    items.Remove(itemToRemove);
                                }
                                items.Add(item);
                                itemCode = itemCode + 1;
                            }
                        }
                    }
                    else
                    {
                        ProductItem item = new ProductItem();
                        item.IsActive = true;
                        item.ProductId = productId;
                        item.ProductName = name;
                        item.PurchasePrice = purchasePrice;
                        item.SalePrice = salePrice;
                        item.MaterialCost = materialCost;
                        item.CM = CM;
                        item.ProductCode = itemCode.ToString("000000");
                        items.Add(item);
                        itemCode = itemCode + 1;
                    }



                }

                //model.ProductItems = productItem;

            }

            model.ProductItemList = items;

            return PartialView("_AttributeTablePartial", model);
        }

        private List<ItemAttribute> ItemAttribute(int productId, int itemId, string productCode, string productName, List<Attributes> attributes, List<AttributesValue> attributesValues)
        {
            List<ItemAttribute> attribute = new List<ItemAttribute>();
            string[] attrValueName = productName.Split('|');

            for (int i = 0; i < attrValueName.Length; i++)
            {
                ItemAttribute locAttribute = new ItemAttribute();
                var attributeId = attributes[i].AttributeId;
                var attributeName = attributes[i].AttributeName;
                var attrValue = attributesValues.FirstOrDefault(c => c.AttributeValueName.Trim() == attrValueName[i].Trim());
                if (attrValue != null)
                {
                    locAttribute.ProductId = productId;
                    locAttribute.ProductItemId = itemId;
                    locAttribute.ProductCode = productCode.Trim();
                    locAttribute.AttributeId = attributeId;
                    locAttribute.AttributeName = attributeName.Trim();
                    locAttribute.AttributeValueId = attrValue.AttributeValueId;
                    locAttribute.AttributeValueName = attrValue.AttributeValueName.Trim();

                    attribute.Add(locAttribute);
                }
            }

            return attribute;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveProductItem(AttributesModel item)
        {
            LoadSession();
            //var deleteMessage = await _objProductDal.DeleteProductItem(item.ProductItemList[1].ProductId, _strWareHouseId, _strShopId);

            if (true)
            {
                List<Attributes> attributes = await _objProductDal.GetProductAttribute(item.ProductItemList[0].ProductId, _strWareHouseId, _strShopId);
                attributes = attributes.OrderBy(c => c.AttributeOrder).ToList();
                List<AttributesValue> attributeValue = _objProductDal.GetProductAttributeValue(item.ProductItemList[0].ProductId, _strWareHouseId, _strShopId);

                foreach (var productItem in item.ProductItemList)
                {
                    var message = await _objProductDal.SaveProductItem(productItem, _strEmployeeId, _strWareHouseId, _strShopId);

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        if (message == "BARCODE ALREADY EXIST")
                        {
                            TempData["message"] = message;
                            return RedirectToAction("Index", "Product");
                        }
                        if (message == "ITEM ALREADY USED")
                        {
                            TempData["message"] = message;
                            continue;
                            //return RedirectToAction("Index", "Product");

                        }
                        var modAttributes = ItemAttribute(productItem.ProductId, Convert.ToInt32(message), productItem.ProductCode, productItem.ProductName, attributes, attributeValue);
                        if (modAttributes.Any())
                        {
                            var strMessage = await _objProductDal.SaveProductItemAttributes(modAttributes, _strEmployeeId, _strWareHouseId, _strShopId);
                            TempData["message"] = strMessage;
                        }
                        else
                        {
                            await _objProductDal.DeleteProductItem(productItem.ProductId, _strWareHouseId, _strShopId);
                            TempData["message"] = "Item Can Not Save !! Something Wrong !!.";
                            return RedirectToAction("Index", "Product");
                        }


                    }
                    else
                    {
                        TempData["message"] = await _objProductDal.DeleteProductItemAndAttribute(productItem.ProductId, productItem.ProductCode);
                    }
                }
            }
            return RedirectToAction("Index", "Product");
        }

        public async Task<JsonResult> DeleteProductAttribute(string productId, string attributeId, string attributeValueId)
        {
            LoadSession();
            string message = await _objProductDal.DeleteProductAttribute(productId, attributeId, attributeValueId, _strWareHouseId, _strShopId);
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> GetAllGenerateItemListByProductId(int? productId)
        {
            List<ProductItem> item = new List<ProductItem>();
            if (productId != null)
            {
                LoadSession();
                item = await _objProductDal.GetProductItem((int)productId, _strWareHouseId, _strShopId);
            }


            return Json(item, JsonRequestBehavior.AllowGet);

        }
    }
}