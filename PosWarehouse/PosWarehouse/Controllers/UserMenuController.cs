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
    public class UserMenuController : Controller
    {

        //For All Dropdown Load for this Object.
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly HomeDal _objHomeDal = new HomeDal();

        //End
        #region "Common"

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

        [RoleFilter]
        public async Task<ActionResult> Index()
        {
            LoadSession();
            MenuMain menuMainmodel = new MenuMain();
            ViewBag.RoleList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetUserRoleListDropdown(), "EMPLOYEE_ROLE", "EMPLOYEE_ROLE");
            ViewBag.UserMenuList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetUserMenuListDropdown(), "MENU_ID", "MENU_NAME");
            ViewBag.UserIdList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetUserIdDropdown(), "EMPLOYEE_ID", "EMPLOYEE_ID");
            return View(menuMainmodel);
        }

        public async Task<JsonResult> GetSubmenuByMenuId(int  menuId)
        {
            var data = await _objHomeDal.GetMenuSubByMenuId(menuId);
           
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetInfoByUserId(string userId)
        {
            var data = await _objHomeDal.GetInfoByUserId(userId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SaveUserMenuPermision(MenuMain objMenuMain)
        {
            var data = "";
            var subdata = "";
            LoadSession();
            var menuMainInfo = await _objHomeDal.GetMenuInfoByMenuId(objMenuMain.MenuMainId);
            if (menuMainInfo != null)
            {
                objMenuMain.MenuMainName = menuMainInfo.MenuMainName;
                objMenuMain.MenuUrl = menuMainInfo.MenuUrl;
                objMenuMain.MenuIcon = menuMainInfo.MenuIcon;
                objMenuMain.UpdateBy = _strEmployeeId;
                data = await _objHomeDal.SaveUserMenuPermision(objMenuMain);
            }
            if (objMenuMain.MenuSubs != null)
            {
                foreach (var menuSub in objMenuMain.MenuSubs)
                {
                    menuSub.MenuMainId = objMenuMain.MenuMainId;
                    menuSub.EmployeeRole = objMenuMain.EmployeeRole;
                    subdata = await _objHomeDal.SaveUserSubMenuPermision(menuSub);
                }
            }
            var messageAndReload = new
            {
                m = data,
                subMenuMessage = subdata,
                isRedirect = true,
                redirectUrl = Url.Action("Index")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteUserMenuPermision(MenuSubPermesionDelete objMenuMain)
        {
            var data = "";
            LoadSession();
            if (objMenuMain.EmployeeRole  != null)
            {
                data = await _objHomeDal.DeleteUserSubMenuPermision(objMenuMain);
            }
            var messageAndReload = new
            {
                m = data,
                isRedirect = true,
                redirectUrl = Url.Action("Index")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteSelectedMenuAndSubMenuPermision(MenuSubPermesionDelete objMenuMain)
        {
            var data = "";
            LoadSession();
            if (objMenuMain.EmployeeRole != null)
            {
                data = await _objHomeDal.DeleteUserMenuAndSubMenuPermision(objMenuMain);
            }
            var messageAndReload = new
            {
                m = data,
                isRedirect = true,
                redirectUrl = Url.Action("Index")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateUserRoleByUserId(MenuMain objMenuMain)
        {
            var data = "";
            LoadSession();
            data = await _objHomeDal.UpdateUserRoleByUserId(objMenuMain);
            var messageAndReload = new
            {
                m = data,
                isRedirect = true,
                redirectUrl = Url.Action("Index")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }
    }
}