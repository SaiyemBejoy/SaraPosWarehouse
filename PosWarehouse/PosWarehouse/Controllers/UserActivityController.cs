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
    public class UserActivityController : Controller
    {
        private readonly UserActivityDal _objUserActivityDal = new UserActivityDal();

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

        // GET: UserActivity
        [RoleFilter]
        public async Task<ActionResult> Index()
        {
            UserActivityModel model = new UserActivityModel();

            ViewBag.EmployeeList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetLogedInEmployeeDropdown(), "VALUE", "TEXT");

            return View(model);
        }

        public async Task<JsonResult> GetEmployeeInfoByEmployeeId(UserActivityModel objUserActivityModel)
        {
            var employeeList = await _objUserActivityDal.GetUserActivityListById(objUserActivityModel);
           
            return Json(employeeList, JsonRequestBehavior.AllowGet);
        }
    }
}