using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class PromotionController : Controller
    {


        private readonly PromotionDAL _objPromotionDAL = new PromotionDAL();

        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();

        // GET: Setup
        #region "Common"
        //private string _strEmployeeId = "01-25-01-009";
        //private string _strWareHouseId = "331";
        //private string _strShopId = "1";

        private string _strEmployeeId = "";
        private string _strWareHouseId = "";
        private string _strShopId = "";

        public void LoadSession()
        {
            var auth = Session["authentication"] as AuthModel;

            if (auth != null)
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


        #region "Circular Price Change"
        [RoleFilter]
        public async Task<ActionResult> CircularPriceChange(int? circularId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            CircularPriceChangeMain model = new CircularPriceChangeMain();
            model.ShopList = new List<int>();
            model.CircularPriceChangeSubList = new List<CircularPriceChangeSub>();
            CircularPriceChangeShop model2 = new CircularPriceChangeShop();


            var objCircularPriceChangeModel = await _objPromotionDAL.GetCircularPriceChangeList(_strWareHouseId, _strShopId);
            ViewBag.CircularPriceChangeList = objCircularPriceChangeModel;



            if (circularId != null && circularId > 0)
            {
                model = await _objPromotionDAL.GetACircularPriceChangeMain((int)circularId, _strWareHouseId, _strShopId);
                model.CircularPriceChangeSubList = await _objPromotionDAL.GetCircularPriceChangeSubList((int)circularId, _strWareHouseId, _strShopId);
                model.ShopList = await _objPromotionDAL.GetCircularPriceChangeShopList((int)circularId, _strWareHouseId);
            }

            var objShopListModel = await _objPromotionDAL.GetShopList();
            ViewBag.ShopList = objShopListModel;

            return View(model);

        }

        public async Task<ActionResult> GetChangeAllCircularList(int circularId)
        {
            LoadSession();
            var circularData = await _objPromotionDAL.GetACircularPriceChangeMain(circularId, _strWareHouseId, _strShopId);
            var circularSubData = await _objPromotionDAL.GetCircularPriceChangeSubList(circularId, _strWareHouseId, _strShopId);
            var circularShopDate = await _objPromotionDAL.GetALLCircularPriceChangeShopList(circularId);
            var data = new
            {
                mainData = circularData,
                subData = circularSubData,
                shopData = circularShopDate
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveAndUpdateCircularPriceChange(CircularPriceChangeMain objCircularPriceChangeMainModel)
        {
            if (objCircularPriceChangeMainModel != null &&
                objCircularPriceChangeMainModel.CircularPriceChangeSubList.Any() && 
                objCircularPriceChangeMainModel.ShopList.Any())
            {
                List<string> shopPriceChangeMessage = new List<string>();
                string circularId = "0";

                List<ShopPriceChange> objShopPriceChanges = new List<ShopPriceChange>();
                foreach (var id in objCircularPriceChangeMainModel.ShopList)
                {
                    ShopPriceChange model = new ShopPriceChange();
                    model.ShopId = id;
                    model.ShopUrl = await _objPromotionDAL.GetAllShopUrl(id);
                    model.ShopName = await _objPromotionDAL.GetAllShopName(id);
                    model.IsShopUpdate = false;
                    objShopPriceChanges.Add(model);
                }

                foreach (var shop in objShopPriceChanges)
                {
                    try
                    {
                        if (shop.ShopUrl != "")
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(shop.ShopUrl);

                                HttpResponseMessage response = await client.PostAsJsonAsync(
                                    "CircularPriceChange", objCircularPriceChangeMainModel);
                                var returnMessage = response.ReasonPhrase.ToString();
                                shop.ResponceStatus = returnMessage;
                                if (response.IsSuccessStatusCode)
                                {
                                    shop.IsShopUpdate = response.IsSuccessStatusCode;
                                }
                                if(returnMessage == "Not Found")
                                    shop.IsShopUpdate = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        shop.IsShopUpdate = false;
                    }
                }

                var checker = false;

                foreach (var shop in objShopPriceChanges)
                {
                    string priceChangeMessage;
                    if (shop.ShopUrl != "")
                    {
                        if (shop.IsShopUpdate)
                        {
                            priceChangeMessage = shop.ShopName + ": Data Save Success";
                            checker = true;
                        }
                        else
                        {
                            priceChangeMessage = shop.ShopName + ": Failed to save, connection problem";
                        }
                    }
                    else
                    {
                        priceChangeMessage = "Warehouse Data Successfully Changed.";
                    }
                    shopPriceChangeMessage.Add(priceChangeMessage);
                }

                if (checker && objCircularPriceChangeMainModel.SelectALlShop == "Y")
                {
                    circularId = await SavePriceChange(objCircularPriceChangeMainModel, objShopPriceChanges);
                }
                else if (objCircularPriceChangeMainModel.CheckWarehouseYN == "Y")
                {
                    circularId = await OnlyWarehouseSavePriceChange(objCircularPriceChangeMainModel, objShopPriceChanges);
                }
                else if (objCircularPriceChangeMainModel.CheckWarehouseYN == "N" && objCircularPriceChangeMainModel.SelectALlShop == "N")
                {
                    circularId = await OnlyShopSavePriceChange(objCircularPriceChangeMainModel, objShopPriceChanges);
                }
                else
                {
                    if ( checker || objCircularPriceChangeMainModel.SelectALlShop != "Y")
                    {
                        objCircularPriceChangeMainModel.SelectALlShop = "N";
                        circularId = await SavePriceChange(objCircularPriceChangeMainModel, objShopPriceChanges); 
                    }  
                }
                var data = new
                {
                    m = circularId,
                    isRedirect = Convert.ToInt32(circularId) > 0,
                    redirectUrl = Url.Action("CircularPriceChange"),
                    notChangeShop = shopPriceChangeMessage
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        private async Task<string> SavePriceChange(CircularPriceChangeMain objCircularPriceChangeMainModel, List<ShopPriceChange> objShopPriceChanges)
        {
            LoadSession();
            objCircularPriceChangeMainModel.UpdateBy = _strEmployeeId;
            objCircularPriceChangeMainModel.WareHouseId = _strWareHouseId;
            objCircularPriceChangeMainModel.ShopId = _strShopId;

            string circularId = await _objPromotionDAL.SaveAndUpdateCircularPriceChangeMain(objCircularPriceChangeMainModel);

            if (!string.IsNullOrWhiteSpace(circularId) && Convert.ToInt32(circularId) > 0)
            {
                foreach (var priceChangeSub in objCircularPriceChangeMainModel.CircularPriceChangeSubList)
                {
                    priceChangeSub.UpdateBy = _strEmployeeId;
                    priceChangeSub.WareHouseId = _strWareHouseId;
                    priceChangeSub.ShopId = _strShopId;
                    priceChangeSub.CircularId = Convert.ToInt32(circularId);
                    string priceChangeSubMessage = await _objPromotionDAL.SaveAndUpdateSubCircularPriceChangeSub(priceChangeSub);

                    if (!string.IsNullOrWhiteSpace(priceChangeSubMessage))
                    {
                        priceChangeSub.EffectiveDate = objCircularPriceChangeMainModel.EffectiveDate;
                        var updateWarehousePrice = await _objPromotionDAL.ChangeAllWarehousePrice(priceChangeSub);
                    }
                }

                CircularPriceChangeShop model = new CircularPriceChangeShop();
                model.UpdateBy = _strEmployeeId;
                model.WareHouseId = _strWareHouseId;
                model.CircularId = Convert.ToInt32(circularId);

                foreach (var changeShopId in objShopPriceChanges)
                {
                    if (changeShopId.ShopName == "" && changeShopId.ShopUrl == "" && changeShopId.ShopId == 0)
                    {
                        changeShopId.ShopId = 331;
                        changeShopId.IsShopUpdate = true;
                    }
                    if (changeShopId.IsShopUpdate)
                    {
                        model.ShopId = changeShopId.ShopId;
                        string shopSaveMessage = await _objPromotionDAL.SaveAndUpdateSubCircularPriceChangeForShop(model);
                    }
                }
            }

            return circularId;
        }

        private async Task<string> OnlyShopSavePriceChange(CircularPriceChangeMain objCircularPriceChangeMainModel, List<ShopPriceChange> objShopPriceChanges)
        {
            LoadSession();
            objCircularPriceChangeMainModel.UpdateBy = _strEmployeeId;
            objCircularPriceChangeMainModel.WareHouseId = _strWareHouseId;
            objCircularPriceChangeMainModel.ShopId = _strShopId;

            string circularId = await _objPromotionDAL.SaveAndUpdateCircularPriceChangeMain(objCircularPriceChangeMainModel);

            if (!string.IsNullOrWhiteSpace(circularId) && Convert.ToInt32(circularId) > 0)
            {
                foreach (var priceChangeSub in objCircularPriceChangeMainModel.CircularPriceChangeSubList)
                {
                    priceChangeSub.UpdateBy = _strEmployeeId;
                    priceChangeSub.WareHouseId = _strWareHouseId;
                    priceChangeSub.ShopId = _strShopId;
                    priceChangeSub.CircularId = Convert.ToInt32(circularId);
                    string priceChangeSubMessage = await _objPromotionDAL.SaveAndUpdateSubCircularPriceChangeSub(priceChangeSub);

                }

                CircularPriceChangeShop model = new CircularPriceChangeShop();
                model.UpdateBy = _strEmployeeId;
                model.WareHouseId = _strWareHouseId;
                model.CircularId = Convert.ToInt32(circularId);

                foreach (var changeShopId in objShopPriceChanges)
                {
                    if (changeShopId.IsShopUpdate)
                    {
                        model.ShopId = changeShopId.ShopId;

                        string shopSaveMessage = await _objPromotionDAL.SaveAndUpdateSubCircularPriceChangeForShop(model);
                    }
                }
            }

            return circularId;
        }
        //only warehouse Change
        private async Task<string> OnlyWarehouseSavePriceChange(CircularPriceChangeMain objCircularPriceChangeMainModel, List<ShopPriceChange> objShopPriceChanges)
        {
            LoadSession();
            objCircularPriceChangeMainModel.UpdateBy = _strEmployeeId;
            objCircularPriceChangeMainModel.WareHouseId = _strWareHouseId;
            objCircularPriceChangeMainModel.ShopId = _strShopId;

            string circularId = await _objPromotionDAL.SaveAndUpdateCircularPriceChangeMain(objCircularPriceChangeMainModel);

            if (!string.IsNullOrWhiteSpace(circularId) && Convert.ToInt32(circularId) > 0)
            {
                foreach (var priceChangeSub in objCircularPriceChangeMainModel.CircularPriceChangeSubList)
                {
                    priceChangeSub.UpdateBy = _strEmployeeId;
                    priceChangeSub.WareHouseId = _strWareHouseId;
                    priceChangeSub.ShopId = _strShopId;
                    priceChangeSub.CircularId = Convert.ToInt32(circularId);
                    string priceChangeSubMessage = await _objPromotionDAL.SaveAndUpdateSubCircularPriceChangeSub(priceChangeSub);
                    if (!string.IsNullOrWhiteSpace(priceChangeSubMessage))
                    {
                        priceChangeSub.EffectiveDate = objCircularPriceChangeMainModel.EffectiveDate;
                        var updateWarehousePrice = await _objPromotionDAL.ChangeAllWarehousePrice(priceChangeSub);
                    }
                }

                CircularPriceChangeShop model = new CircularPriceChangeShop();
                model.UpdateBy = _strEmployeeId;
                model.WareHouseId = _strWareHouseId;
                model.CircularId = Convert.ToInt32(circularId);

                foreach (var changeShopId in objShopPriceChanges)
                {
                    if (changeShopId.ShopName == "" && changeShopId.ShopUrl == "" && changeShopId.ShopId == 0)
                    {
                        changeShopId.ShopId = 331;
                        changeShopId.IsShopUpdate = true;
                    }
                    if (changeShopId.IsShopUpdate)
                    {
                        model.ShopId = changeShopId.ShopId;
                        string shopSaveMessage = await _objPromotionDAL.SaveAndUpdateSubCircularPriceChangeForShop(model);
                    }
                }
            }
            return circularId;
        }
        public async Task<ActionResult> DeleteCircularPriceChange(int circularId)
        {
            LoadSession();
            string message = await _objPromotionDAL.DeleteCircularPriceChange(circularId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("CircularPriceChange");
        }

        public async Task<ActionResult> SearchProductItemByBarcode(string barcode)
        {
            LoadSession();
            var item = await _objPromotionDAL.SearchProductItemByBarcode(barcode, _strWareHouseId, _strShopId);

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region CirCular Discount Promotion
        [RoleFilter]
        public async Task<ActionResult> CircularDiscountPromotion(int? categoryId, int? discountCircularId)
        {

            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }
            LoadSession();
            CircularDiscountPromotionMain model = new CircularDiscountPromotionMain();
            model.CircularDiscountPromotionSubList = new List<CircularDiscountPromotionSub>();
            model.CircularDiscountPromotionShopList = new List<int>();
            CircularDiscountPromotionShop model2 = new CircularDiscountPromotionShop();

            var objCircularDiscountPromotionModel = await _objPromotionDAL.CircularDiscountPromotionList(_strWareHouseId, _strShopId);
            ViewBag.DiscountPromotionList = objCircularDiscountPromotionModel;

            if (discountCircularId != null && discountCircularId > 0)
            {
                model = await _objPromotionDAL.GetACircularDiscountPromotonMain((int)discountCircularId, _strWareHouseId, _strShopId);
                model.CircularDiscountPromotionSubList = await _objPromotionDAL.GetACircularDiscountPromotionSubList((int)discountCircularId, _strWareHouseId, _strShopId);
                model.CircularDiscountPromotionShopList = await _objPromotionDAL.GetACircularDiscountPromotionShopList((int)discountCircularId, _strWareHouseId);
            }

            ViewBag.CategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetCategoryListDropdown(), "CATEGORY_ID", "CATEGORY_NAME");
            ViewBag.SubCategoryList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSubCategoryListDropdown(), "SUB_CATEGORY_ID", "SUB_CATEGORY_NAME");


            var objShopListModel = await _objPromotionDAL.GetShopList();
            ViewBag.ShopList = objShopListModel;
            return View(model);
        }


        public async Task<ActionResult> SaveAndUpdateCircularDiscountPromotion(CircularDiscountPromotionMain objCircularDiscountPromotionMainModel)
        {
            LoadSession();
            string returnMessage = "";
            if (ModelState.IsValid && objCircularDiscountPromotionMainModel.CircularDiscountPromotionShopList.Any() && objCircularDiscountPromotionMainModel.CircularDiscountPromotionSubList.Any())
            {
                objCircularDiscountPromotionMainModel.UpdateBy = _strEmployeeId;
                objCircularDiscountPromotionMainModel.WareHouseId = _strWareHouseId;
                objCircularDiscountPromotionMainModel.ShopId = _strShopId;

                try
                {
                    string strMessagedata = await _objPromotionDAL.SaveAndUpdateCircularDiscountPromotionMain(objCircularDiscountPromotionMainModel);

                    if (Convert.ToInt32(strMessagedata) > 0 || strMessagedata != null)
                    {
                        foreach (var tabledata in objCircularDiscountPromotionMainModel.CircularDiscountPromotionItemList)
                        {

                            tabledata.UpdateBy = _strEmployeeId;
                            tabledata.WareHouseId = _strWareHouseId;
                            tabledata.ShopId = _strShopId;
                            tabledata.DiscountCircularId = Convert.ToInt32(strMessagedata);
                            returnMessage = await _objPromotionDAL.SaveAndUpdateSubCircularDiscountPromotionSub(tabledata);

                        }

                        CircularDiscountPromotionShop model = new CircularDiscountPromotionShop();
                        model.UpdateBy = _strEmployeeId;
                        model.WareHouseId = _strWareHouseId;
                        model.DiscountCircularId = Convert.ToInt32(strMessagedata);

                        foreach (var shop in objCircularDiscountPromotionMainModel.CircularDiscountPromotionShopList)
                        {
                            model.ShopId = shop;
                            returnMessage = await _objPromotionDAL.SaveAndUpdateCircularDiscountPromotionForShop(model);
                        }
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
                redirectUrl = Url.Action("CircularDiscountPromotion")
            };


            return Json(data);
        }

        public async Task<ActionResult> SaveCircularDiscountPromotion(CircularDiscountPromotionMain objCircularDiscountPromotionMainModel)
        {
            LoadSession();
            string returnMessage = "";
            if (ModelState.IsValid && objCircularDiscountPromotionMainModel.CircularDiscountPromotionShopList.Any() && objCircularDiscountPromotionMainModel.CircularDiscountPromotionItemList.Any())
            {
                objCircularDiscountPromotionMainModel.UpdateBy = _strEmployeeId;
                objCircularDiscountPromotionMainModel.WareHouseId = _strWareHouseId;
                objCircularDiscountPromotionMainModel.ShopId = _strShopId;

                try
                {
                    string strMessagedata = await _objPromotionDAL.SaveAndUpdateCircularDiscountPromotionMain(objCircularDiscountPromotionMainModel);

                    if (Convert.ToInt32(strMessagedata) > 0 || strMessagedata != null)
                    {
                        foreach (var tabledata in objCircularDiscountPromotionMainModel.CircularDiscountPromotionItemList)
                        {

                            tabledata.UpdateBy = _strEmployeeId;
                            tabledata.WareHouseId = _strWareHouseId;
                            tabledata.ShopId = _strShopId;
                            tabledata.DiscountCircularId = Convert.ToInt32(strMessagedata);
                            returnMessage = await _objPromotionDAL.SaveAndUpdateSubCircularDiscountPromotionSub(tabledata);

                        }
                        if (returnMessage != "EXISTS")
                        {
                            CircularDiscountPromotionShop model = new CircularDiscountPromotionShop();
                            model.UpdateBy = _strEmployeeId;
                            model.WareHouseId = _strWareHouseId;
                            model.DiscountCircularId = Convert.ToInt32(strMessagedata);

                            foreach (var shop in objCircularDiscountPromotionMainModel.CircularDiscountPromotionShopList)
                            {
                                model.ShopId = shop;
                                returnMessage = await _objPromotionDAL.SaveAndUpdateCircularDiscountPromotionForShop(model);
                            }
                        }
                        else
                        {
                            int circularId = Convert.ToInt32(strMessagedata);
                            string warehouseId = _strWareHouseId;
                            string shopId = _strShopId;
                            returnMessage = await _objPromotionDAL.DeleteCircularDiscountPromotion(circularId, warehouseId, shopId);
                        }
                       
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
                redirectUrl = Url.Action("CircularDiscountPromotion")
            };


            return Json(data);
        }

        public async Task<ActionResult> DeleteCircularDiscountPromotion(int discountCircularId)
        {
            LoadSession();
            string message = await _objPromotionDAL.DeleteCircularDiscountPromotion(discountCircularId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("CircularDiscountPromotion");
        }

        #endregion
    }
}