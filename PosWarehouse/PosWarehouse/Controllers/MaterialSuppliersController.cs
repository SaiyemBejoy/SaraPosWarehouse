using PosWarehouse.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PosWarehouse.Controllers
{
    public class MaterialSuppliersController : Controller
    {
        // GET: MaterialSuppliers
        [RoleFilter]
        public async Task<ActionResult> Index()
        {
            return View();
        }
    }
}