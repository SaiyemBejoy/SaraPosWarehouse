using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Utility
{
    public class RoleFilter : ActionFilterAttribute
    {
        private static readonly AuthenticationDAL _objAuthenticationDAL = new AuthenticationDAL();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionUser = (AuthModel)HttpContext.Current.Session["authentication"];
            if (sessionUser.EmployeeRole != "")// Check the Role Against the database Value
            {
                string requiredPermission = String.Format("{0}/{1}",
                    filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    filterContext.ActionDescriptor.ActionName);

                var access = _objAuthenticationDAL.RoleWiseActionPermision(requiredPermission, sessionUser.EmployeeRole);
                if (access.Result == "null")
                {
                    var urlHelper = new UrlHelper(filterContext.RequestContext);
                    HttpContext.Current.Session.Abandon();
                    var url = urlHelper.Action("Index", "Auth");
                    filterContext.Result = new RedirectResult(url);
                }

            }
            //ActionNames(controller);
        }
        public List<string> ActionNames(string controllerName)
        {
            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                where typeof(IController).IsAssignableFrom(t) &&
                      string.Equals(controllerName + "Controller", t.Name, StringComparison.OrdinalIgnoreCase)
                select t;

            var controllerType = types.FirstOrDefault();

            if (controllerType == null)
            {
                return Enumerable.Empty<string>().ToList();
            }
            var actionList = new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().Select(x => x.ActionName).ToList();
            return actionList;

        }

    }
}