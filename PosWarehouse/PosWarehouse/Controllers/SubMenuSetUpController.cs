using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class SubMenuSetUpController : Controller
    {
        private readonly MenuAndSubMenuDal _objSubMenuSetupDal = new MenuAndSubMenuDal();

        //For All Dropdown Load for this Object.
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();

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

        // GET: SubMenuSetUp
        [RoleFilter]
        public async Task<ActionResult> Index(int? menuId, int? subMenuId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            SubMenuSetUpModel model = new SubMenuSetUpModel();

            var objSubMenuModels = await _objSubMenuSetupDal.GetSubMenuList(_strWareHouseId, _strShopId);
            ViewBag.SubMenuList = objSubMenuModels;

           // ViewBag.MaxOrderNumber = await _objSetupDal.GetMaxOrderNumberForSubMenu(menuId);

            if (menuId != null && menuId != 0 && subMenuId != null && subMenuId != 0)
            {
                model = await _objSubMenuSetupDal.GetASubMenu((int)menuId, (int)subMenuId, _strWareHouseId, _strShopId);
            }

            ViewBag.MenuList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetUserMenuListDropdown(), "MENU_ID", "MENU_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateSubMenu(SubMenuSetUpModel objSubMenuModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objSubMenuModel.UpdateBy = _strEmployeeId;
                objSubMenuModel.WareHouseId = _strWareHouseId;
                objSubMenuModel.ShopId = _strShopId;

                string strMessage = await _objSubMenuSetupDal.SaveAndUpdateSubMenu(objSubMenuModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DeleteSubMenu(int menuId, int subMenuId)
        {
            LoadSession();
            string message = await _objSubMenuSetupDal.DeleteSubMenu(menuId, subMenuId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> GetMenuIdFromView(int menuId)
        {
            var data = await _objSubMenuSetupDal.GetMaxOrderNumberForSubMenu(menuId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}