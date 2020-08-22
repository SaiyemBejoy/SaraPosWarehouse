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
    public class OtherCompanyDiscountController : Controller
    {
        private readonly OtherCompanyOfferDal _objOtherCompanyOfferDal = new OtherCompanyOfferDal();

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
        // GET: OtherCompanyDiscount
        [RoleFilter]
        public async Task<ActionResult> OtherCompanyDiscount(int? companyOfferId)
        {
            ModelState.Clear();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            OtherCompanyOfferDisModel model = new OtherCompanyOfferDisModel();

            var objOtherCompanyOfferDisModel = await _objOtherCompanyOfferDal.GetOtherCompanyOfferListFroDisplay();
            ViewBag.objOtherCompanyOfferDisList = objOtherCompanyOfferDisModel;
            if (companyOfferId != null && companyOfferId != 0)
            {
                model = await _objOtherCompanyOfferDal.GetAOtherCompanyOffer((int)companyOfferId);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveData(OtherCompanyOfferDisModel objOtherCompanyOfferDisModel)
        {
            LoadSession();
            objOtherCompanyOfferDisModel.CreatedBy = _strEmployeeId;
            objOtherCompanyOfferDisModel.Active_YN = objOtherCompanyOfferDisModel.ActiveStatus ? "Y" : "N";
            objOtherCompanyOfferDisModel.WareHouseId = _strWareHouseId;

            string strMessage = await _objOtherCompanyOfferDal.SaveData(objOtherCompanyOfferDisModel);
            TempData["message"] = strMessage;
            return RedirectToAction("OtherCompanyDiscount");
        }
    }
}