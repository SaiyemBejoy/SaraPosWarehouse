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
    public class ChangePasswordController : ApiController
    {
        private readonly AuthenticationDAL _authentication;

        public ChangePasswordController()
        {
            _authentication = new AuthenticationDAL();
        }

        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel oChangePasswordModel)
        {
            if (oChangePasswordModel.EmployeeId != null && oChangePasswordModel.OldPassword != null && oChangePasswordModel.NewPassword != null)
            {

                var returnMessage = await _authentication.ChangePassword(oChangePasswordModel);
                return Ok(returnMessage);


            }

            return BadRequest();
        }

    }
}
