using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class EmployeeDistributionController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly EmployeeDistributionDal _objEmployeeDistributionDal = new EmployeeDistributionDal();

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
            EmployeeDistributionModel model = new EmployeeDistributionModel();
            var objEmployeeDistributionModel = await _objEmployeeDistributionDal.GetAllEmployeeInfoList();
            ViewBag.ShopEmployeeList = objEmployeeDistributionModel;

            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            return View(model);
        }

        public async Task<JsonResult> GetEmployeeInfoByEmployeeId(string employeeId)
        {
            var employeeList = await _objEmployeeDistributionDal.GetstaticListValueforSearchById(employeeId);
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            var shopList = ViewBag.ShopList;
            var allList = new
            {
                shopList = shopList,
                employeeList = employeeList,
            };
            return Json(allList, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateEmployeeInfo(EmployeeDistributionModel objEmployeeDistributionModel)
        {
            LoadSession();
            string returnMessage = "";
            var getShopUrl = await _objEmployeeDistributionDal.GetShopUrlByEmployeeId(objEmployeeDistributionModel.EmployeeId);
            if (getShopUrl != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(getShopUrl);

                    HttpResponseMessage response = await client.PostAsJsonAsync(
                        "Employee", objEmployeeDistributionModel);
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        returnMessage = await _objEmployeeDistributionDal.UpdateEmployeeInfo(objEmployeeDistributionModel);
                    }
                }
            }
            else
            {
                returnMessage = await _objEmployeeDistributionDal.UpdateEmployeeInfo(objEmployeeDistributionModel);
            }
           
            var messageAndReload = new
            {
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("Index")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveAllEmployeeInfo(List<EmployeeDistributionModel> objEmployeeDistributionModel)
        {
            LoadSession();
            string returnMessage = "";
            if (ModelState.IsValid || objEmployeeDistributionModel.Count > 0)
            {
                foreach (var value in objEmployeeDistributionModel)
                {
                    value.UpdateBy = _strEmployeeId;
                    value.ActiveYn = value.ActiveStatus ? "Y" : "N";
                    returnMessage = await _objEmployeeDistributionDal.SaveAllEmployeeInfo(value);
                }
            }
            var messageAndReload = new
            {
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("Index")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public static List<EmployeeListModel> GetstaticListValueforSearch(string employeeId)
        {
            List<EmployeeListModel> emlist = new List<EmployeeListModel>
            {
                new EmployeeListModel { EmployeeId= "01-25-01-258", EmployeeName= "Ankur", Designation= "Manager",ContactNo = "015444",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-259", EmployeeName= "Rasel", Designation= "Sales Executive",ContactNo = "2212",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-257", EmployeeName= "Sakib", Designation= "Store man",ContactNo = "4221",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-360", EmployeeName= "nirab", Designation= "Sales Executive",ContactNo = "4221",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-254", EmployeeName= "Tamim", Designation= "Store man",ContactNo = "4221",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-345", EmployeeName= "sabbir", Designation= "Sales Executive",ContactNo = "4221",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-645", EmployeeName= "Hassan", Designation= "Sales Executive",ContactNo = "4221",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-845", EmployeeName= "hasib", Designation= "Sales Executive",ContactNo = "4221",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-945", EmployeeName= "Sakib", Designation= "Store man",ContactNo = "4221",Email = "gdfgdfgdf@fdd.gfdg"},
                new EmployeeListModel { EmployeeId= "01-25-01-256", EmployeeName= "Piyash", Designation= "Manager",ContactNo = "69363",Email = "gdfgdfgdf@fdd.gfdg"}

            }.Where(p => p.EmployeeId == employeeId).ToList();

            return emlist;
        }
    }
}