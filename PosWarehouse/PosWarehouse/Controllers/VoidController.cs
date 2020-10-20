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
    public class VoidController : Controller
    {
        private readonly InvoiceInfoDAL _objVoidInvoiceDal = new InvoiceInfoDAL();

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

        // GET: Void
        [RoleFilter]
        public async Task<ActionResult> Index()
        {
            var model = new InvoiceInfoModel();

            ViewBag.ShopList = UtilityClass.GetSelectListForShop(await _objDropdownDal.GetActiveShopListDropdown(), "SHOP_ID", "SHOP_NAME");

            return View(model);
        }

        public async Task<JsonResult> GetVoidInfoByShopId(InvoiceInfoModel objInvoiceInfoModel)
        {
            var voidList = await _objVoidInvoiceDal.GetVoidInfoByShopId(objInvoiceInfoModel);

            return Json(voidList, JsonRequestBehavior.AllowGet);
        }

    }
}