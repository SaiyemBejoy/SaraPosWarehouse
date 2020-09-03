using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Controllers
{

    public class HomeController : Controller
    {
        private readonly HomeModel _objHomeModel = new HomeModel();
        private readonly HomeDal _objHomeDal = new HomeDal();
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();

        #region "Common"

        private string _strEmployeeId = "";
        private string _strWareHouseId = "";
        private string _strShopId = "";
        private string _strEmployeeRole = "";

        public void LoadSession()
        {
            if (Session["authentication"] is AuthModel auth)
            {
                _strEmployeeId = auth.EmployeeId;
                _strWareHouseId = auth.WareHouseId;
                _strShopId = auth.ShopId;
                _strEmployeeRole = auth.EmployeeRole;
            }
            else
            {
                Response.Headers.Clear();
                string url = Url.Action("Index", "Auth");
                if (url != null) Response.Redirect(url);
            }
        }
        #endregion
        [LogAction]
        public async Task<ActionResult> Index()
        {
            if (Session["authentication"] is AuthModel employee)
            {
                DashboardModel model = new DashboardModel();
                model = await _objHomeDal.GetAllSaleInfoData();
                ViewBag.ShopList = GetSelectListByDataTableForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");
                return View(model);
            }

            return RedirectToAction("Index", "Auth");
        }
        //For Shop Dropdown
        public static SelectList GetSelectListByDataTableForShop(DataTable objDataTable, string pValueField, string pTextField)
        {
            List<SelectListItem> objSelectListItems = new List<SelectListItem>
            {
                new SelectListItem() {Value = "", Text = "--ALL Shop--"},
            };
            objSelectListItems.AddRange(from DataRow dataRow in objDataTable.Rows
                select new SelectListItem()
                {
                    Value = dataRow[pValueField].ToString(),
                    Text = dataRow[pTextField].ToString()
                });

            return new SelectList(objSelectListItems, "Value", "Text");
        }
        //End
        [LogAction]
        public async Task<ActionResult> AllDashboardInfoByShopId(string shopId)
        {
            var data = await _objHomeDal.AllDashboardInfoByShopId(shopId);
            data.ShopUrl = await _objHomeDal.GetShopUrlByShopId(shopId);
            return Json(data,JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AllLowStockByShopId(string shopId)
        {
            var data = await _objHomeDal.AllLowStockByShopId(shopId);
            var result = data.ToList().Take(50);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetAllSaleInfo()
        {
            var data = await _objHomeDal.GetAllSaleInfoData();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AllHotSaleByShopId(string shopId)
        {
            var data = await _objHomeDal.AllHotSaleByShopId(shopId);
            var result = data.ToList().Take(50);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [LogAction]
        public ActionResult About()
        {
            var employee = Session["authentication"] as AuthModel;

            if (employee != null)
            {
                    return View();
            }
           
            return RedirectToAction("Index", "Auth");
        }
        [LogAction]
        [RoleFilter]
        public ActionResult Contact()
        {
            var employee = Session["authentication"] as AuthModel;

            if (employee != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Auth");
        }

        [ChildActionOnly]
        public async Task<ActionResult> Menu()
        {
            LoadSession();

            MenuModel menuModel = new MenuModel();
            //role wise menu
            if (_strEmployeeRole != "null")
            {
                menuModel.MenuMains = await _objHomeDal.GetMenuMainRoleWise(_strEmployeeRole);
                foreach (var menuMain in menuModel.MenuMains)
                {
                    menuMain.MenuSubs = await _objHomeDal.GetMenuSubRoleWise(menuMain.MenuMainId, _strEmployeeRole);
                }
            }
            else
            {
                ViewBag.MenuPermisionMessage = "You Have No Permision.Please contact with concern Person!.";
            }
            //End 

            //if (_strEmployeeId != "null")
            //{
            //    menuModel.MenuMains = await _objHomeDal.GetMenuMain(_strEmployeeId, _strWareHouseId, _strShopId);
            //    foreach (var menuMain in menuModel.MenuMains)
            //    {
            //        menuMain.MenuSubs = await _objHomeDal.GetMenuSub(menuMain.MenuMainId, _strEmployeeId, _strWareHouseId, _strShopId);
            //    }
            //}
            return PartialView("_MenuPartial", menuModel);
        }

       
    }
}