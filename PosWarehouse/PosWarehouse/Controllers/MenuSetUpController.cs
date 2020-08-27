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
    public class MenuSetUpController : Controller
    {
        private readonly MenuAndSubMenuDal _objSetupDal = new MenuAndSubMenuDal();

        //For All Dropdown Load for this Object.
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();


        // GET: MenuSetUp
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

        //[RoleFilter]
        public async Task<ActionResult> Index(int? menuId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            MenuSetUpModel model = new MenuSetUpModel();

            var objMenuModel = await _objSetupDal.GetMenuList(_strWareHouseId, _strShopId);
            ViewBag.MenuList = objMenuModel;

            
            ViewBag.MaxOrderNumber = await _objSetupDal.GetMaxOrderNumberForMenu();

            if (menuId != null && menuId != 0)
            {
                model = await _objSetupDal.GetAMenu((int)menuId, _strWareHouseId, _strShopId);
            }

            //ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateMenu(MenuSetUpModel objMenuModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objMenuModel.UpdateBy = _strEmployeeId;
                objMenuModel.Active_YN = objMenuModel.ActiveStatus ? "Y" : "N";
                objMenuModel.WareHouseId = _strWareHouseId;
                objMenuModel.ShopId = _strShopId;

                string strMessage = await _objSetupDal.SaveAndUpdateMenu(objMenuModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> DeleteMenu(int menuId)
        {
            LoadSession();
            string message = await _objSetupDal.DeleteMenu(menuId, _strWareHouseId, _strShopId);
            TempData["message"] = message;

            return RedirectToAction("Index");
        }
    }
}