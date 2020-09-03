using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using WebGrease.Css.Ast.Selectors;

namespace PosWarehouse.Controllers
{
   
    public class AuthController : Controller
    {
      readonly AuthenticationDAL _authentication = new AuthenticationDAL();

        // GET: Auth
        public ActionResult Index(bool? userExist)
        {
            var employee = Session["authentication"] as AuthModel;
            if (employee != null)
            { 
              return RedirectToAction("About", "Home");
            }
            var b = !userExist;
            if (b != null && (bool)b)
            {
                ViewBag.Message = TempData["message"];
            }
            return View("Index");
        }
        [LogAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AuthModel authModel)
        {
            if (authModel.EmployeeId !=null && authModel.EmployeePassword != null)
            {
                authModel = await _authentication.Login(authModel.EmployeeId, authModel.EmployeePassword);
                if (authModel.Message)
                {
                    if (authModel.EmployeeRole != "null")//erp er user er role asa na manual e role assagin korte hoi///a jonno ai check kora hoise//role na thakle login oo hoobe na
                    {
                        Session["authentication"] = authModel;
                        return RedirectToAction("About", "Home");
                    }
                    else
                    {
                        TempData["message"] = "You do not have enough permissions to access this software!. Please contact with your concern person to get access. ";
                        return RedirectToAction("Index", new { userExist = false });
                    }
                }
                TempData["message"] = "Unauthorize access denied for this system !! ";
                return RedirectToAction("Index", new { userExist = false });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotePassword(AuthModel authModel)
        {
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> LoadTopMenue()
        {
            var employee = Session["authentication"] as AuthModel;
            if (employee != null)
            {
                employee.RequisitionMainModels = await _authentication.GetAllRequisition();
                employee.StockTransferModels = await _authentication.GetAllStockTransfer();
                return PartialView("_TopMenuPartial", employee);
            }
            return null;
        }
        public ActionResult RedirectActionResult()
        {
            Response.Clear();
            Response.Redirect("~/Auth/Index");
            //return RedirectToAction("Index");
            return null;
        }
        [LogAction]
        public ActionResult LogOut()
        {
            var employee = Session["authentication"] as AuthModel;
            if (employee != null)
            {
                Session.Abandon();
            }
            return RedirectToAction("Index");
        }

        
    }
}