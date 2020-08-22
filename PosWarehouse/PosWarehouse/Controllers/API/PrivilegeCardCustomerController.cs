using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.Controllers.Api
{
    public class PrivilegeCardCustomerController : ApiController
    {
        private readonly SetupDAL _setupDAL;

        public PrivilegeCardCustomerController()
        {
            _setupDAL = new SetupDAL();
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveAllPrivilegeCardCustomerData(CustomerModel objCustomerModel)
        {
            string returnMessage = "";
             returnMessage = await _setupDAL.SavePrivilegeCustomer(objCustomerModel);
            if (returnMessage != "SUCCESS")
            {
                return BadRequest();
            }
            return Ok(returnMessage);
        }
    }
}
