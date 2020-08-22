using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PosWarehouse.DAL;

namespace PosWarehouse.Controllers.Api
{
    public class MaxRequisitionNumberController : ApiController
    {
        private readonly ShopToShopRequisitionDAL _dal;

        public MaxRequisitionNumberController()
        {
            _dal = new ShopToShopRequisitionDAL();
        }

        public async Task<IHttpActionResult> GetMaxRequisition()
        {

            var maxNumber = await _dal.GetMaxRequisitionNumber();
            if (maxNumber != null)
            {
                return Ok(maxNumber);

            }
            return BadRequest();
        }

    }
}
