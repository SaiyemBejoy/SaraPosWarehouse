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
    public class UserSubActionPermissionController : Controller
    {
        private readonly UserSubActionPermissionDal _objSubActionPermissionDal = new UserSubActionPermissionDal();

        //For All Dropdown Load for this Object.
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();

        // GET: UserSubActionPermission
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
        public async Task<ActionResult> Index(int? permissionId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            UserSubActionPermissionModel model = new UserSubActionPermissionModel();

            var objSubActionPermissionModel = await _objSubActionPermissionDal.GetSubActionPermissionList();
            ViewBag.SubActionPermissionList = objSubActionPermissionModel;

            ViewBag.RoleList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetUserRoleListDropdown(), "EMPLOYEE_ROLE", "EMPLOYEE_ROLE");

            if (permissionId != null && permissionId != 0)
            {
                model = await _objSubActionPermissionDal.GetASubActionPermission((int)permissionId);
            }

            //ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateSubActionPermission(UserSubActionPermissionModel objSubPermissionModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objSubPermissionModel.CreateBy = _strEmployeeId;
                objSubPermissionModel.Active_YN = objSubPermissionModel.ActiveStatus ? "Y" : "N";

                string strMessage = await _objSubActionPermissionDal.SaveAndUpdateSubActionPermission(objSubPermissionModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Index");
        }

    }
}