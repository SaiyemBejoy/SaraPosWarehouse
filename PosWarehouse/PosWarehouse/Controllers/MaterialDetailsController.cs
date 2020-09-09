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
    public class MaterialDetailsController : Controller
    {
        private readonly MaterialDetailsDal _objMaterialDetailsDal = new MaterialDetailsDal();

        // GET: MaterialDetails
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
        public async Task<ActionResult> Index(int? materialId)
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            MaterialDetailsModel model = new MaterialDetailsModel();

            var objMaterialDetailsModel = await _objMaterialDetailsDal.GetMaterialList();
            ViewBag.MaterialList = objMaterialDetailsModel;

            if (materialId != null && materialId != 0)
            {
                model = await _objMaterialDetailsDal.GetAMaterial((int)materialId);
            }

            //ModelState.Clear();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAndUpdateMaterial(MaterialDetailsModel objMaterialDetailsModel)
        {
            LoadSession();
            if (ModelState.IsValid)
            {
                objMaterialDetailsModel.UpdateBy = _strEmployeeId;
                objMaterialDetailsModel.CreateBy = _strEmployeeId;

                string strMessage = await _objMaterialDetailsDal.SaveAndUpdateMaterial(objMaterialDetailsModel);
                TempData["message"] = strMessage;
            }
            return RedirectToAction("Index");
        }


    }
}