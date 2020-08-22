using System.Web.Mvc;

namespace PosWarehouse
{
    public class CustomRazorViewEngine : RazorViewEngine
    {
        public CustomRazorViewEngine()
        {
            AreaViewLocationFormats = new[] {

                "~/Areas/{2}/Views/{1}/{0}.cshtml",

                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };
            AreaMasterLocationFormats = new[] {

                "~/Areas/{2}/Views/{1}/{0}.cshtml",

                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };
            AreaPartialViewLocationFormats = new[] {

                "~/Areas/{2}/Views/{1}/{0}.cshtml",

                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };
            ViewLocationFormats = new[] {

                "~/Views/{1}/{0}.cshtml",

                "~/Views/Shared/{0}.cshtml"
            };
            PartialViewLocationFormats = new[] {

                "~/Views/{1}/{0}.cshtml",

                "~/Views/Shared/{0}.cshtml"
            };
            MasterLocationFormats = new[] {

                "~/Views/{1}/{0}.cshtml",

                "~/Views/Shared/{0}.cshtml"
            };
        }
    }
}