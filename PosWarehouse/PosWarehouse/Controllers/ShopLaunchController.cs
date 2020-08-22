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
    public class ShopLaunchController : Controller
    {
        // GET: ShopLaunch

        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly ShopLaunchDal _objShopLaunchDal = new ShopLaunchDal();

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
        public ActionResult Index()
        {
            return View();
        }
        public  async Task<ActionResult> SaveData()
        {
            LoadSession();
            ShopLaunchModel model = new ShopLaunchModel();
            ViewBag.styleName = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetAllProductStyleList(), "PRODUCT_ID", "PRODUCT_STYLE");
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            return View(model);
        }
        public async Task<ActionResult> SaveAllData(ShopLaunchModel objShopLaunchModel)
        {
            LoadSession();
            objShopLaunchModel.UpdatedBy = _strEmployeeId;
            objShopLaunchModel.WarehouseId = _strWareHouseId;
            var result = await _objShopLaunchDal.SaveData(objShopLaunchModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}