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
    public class GiftVoucherDepositController : Controller
    {
        private readonly GiftVoucherDepositDAL _objGiftVoucherDepositDal = new GiftVoucherDepositDAL();

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

        // GET: GiftVoucherDeposit
        public async Task<ActionResult> Index(int? giftVoucherId, int? giftVoucherDepositId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            GiftVoucherDepositModel model = new GiftVoucherDepositModel();

            var objGiftVoucherDepositModel = await _objGiftVoucherDepositDal.GetGiftVoucherDepositList(2);
            ViewBag.GiftVoucherDepositList = objGiftVoucherDepositModel;

            //if (giftVoucherId != null && giftVoucherId != 0 && giftVoucherDepositId != null && giftVoucherDepositId != 0)
            //{
            //    model = await _objGiftVoucherDepositDal.GetAGiftVoucherDeposit((int)giftVoucherId, (int)giftVoucherDepositId);
            //}

            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetWareHouseListDropdown(), "WARE_HOUSE_ID", "WARE_HOUSE_NAME");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateGiftVoucherDeposit(GiftVoucherDepositModel objGiftVoucherDeposit)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objGiftVoucherDeposit.CreateBy = _strEmployeeId;

                string strMessage = await _objGiftVoucherDepositDal.SaveAndUpdateGiftVoucherDeposit(objGiftVoucherDeposit);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Index");
        }
    }
}