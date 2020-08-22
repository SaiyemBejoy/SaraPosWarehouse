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
    public class EmployeeDistributionController : ApiController
    {
        private readonly EmployeeDistributionDal _employeeDistributionDal;

        public EmployeeDistributionController()
        {
            _employeeDistributionDal = new EmployeeDistributionDal();
        }

        public async Task<IHttpActionResult> Get()
        {
            var getData = await _employeeDistributionDal.GetAllEmployeeInfoList();

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }

        public async Task<IHttpActionResult> Get(string shopId)
        {
            var getData = await _employeeDistributionDal.GetAllEmployeeInfoList(shopId);

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }
    }
}
