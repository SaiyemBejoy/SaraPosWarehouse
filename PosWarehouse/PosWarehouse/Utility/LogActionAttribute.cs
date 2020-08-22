using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Utility
{
    public class LogActionAttribute: ActionFilterAttribute
    {
        private static readonly AuthenticationDAL _objAuthenticationDAL = new AuthenticationDAL();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.RequestContext.RouteData.Values.ContainsKey("Controller") ? filterContext.RequestContext.RouteData.Values["Controller"].ToString() : null;
            var action = filterContext.RequestContext.RouteData.Values.ContainsKey("Action") ? filterContext.RequestContext.RouteData.Values["Action"].ToString() : null;  
            var IP = filterContext.HttpContext.Request.UserHostAddress;
            var sessionUser = (AuthModel)HttpContext.Current.Session["authentication"];

            LogActionModel userLog = new LogActionModel()
            {
                ControllerName=controller,
                ActionName = action,
                IpAddress= IP,
                MenuUrl = ""
            };
            if(sessionUser == null)
            {
                AuthModel objAuthModel = new AuthModel();
                Task.Run(() => _objAuthenticationDAL.LogAction(userLog, objAuthModel)).Wait();
            }
            else
            {
                Task.Run(() => _objAuthenticationDAL.LogAction(userLog, sessionUser)).Wait();
            }
            base.OnActionExecuting(filterContext);
          
        }
    }
}