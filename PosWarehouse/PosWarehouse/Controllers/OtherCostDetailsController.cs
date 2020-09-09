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
    public class OtherCostDetailsController : Controller
    {
        private readonly OtherCostDetailsDal _objOtherCostDetailsDal = new OtherCostDetailsDal();

        // GET: OtherCostDetails
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


        [RoleFilter]
        public async Task<ActionResult> Index(int? otherCostId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            OtherCostDetailsModel model = new OtherCostDetailsModel();

            var objOtherCostDetailsModel = await _objOtherCostDetailsDal.GetOtherCostList();
            ViewBag.OtherCostList = objOtherCostDetailsModel;

            if (otherCostId != null && otherCostId != 0)
            {
                model = await _objOtherCostDetailsDal.GetAOtherCost((int)otherCostId);
            }

            //ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateOterCost(OtherCostDetailsModel objOtherCostDetailsModels)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objOtherCostDetailsModels.UpdateBy = _strEmployeeId;
                objOtherCostDetailsModels.CreateBy = _strEmployeeId;

                string strMessage = await _objOtherCostDetailsDal.SaveAndUpdateOterCost(objOtherCostDetailsModels);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Index");
        }


    }
}