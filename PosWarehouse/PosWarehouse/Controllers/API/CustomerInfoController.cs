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
    public class CustomerInfoController : ApiController
    {
        private readonly SetupDAL _setupDal;

        public CustomerInfoController()
        {
            _setupDal = new SetupDAL();
        }

        public async Task<IHttpActionResult> Get()
        {
            var getData = await _setupDal.GetCustomerList();

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }
    }
}
