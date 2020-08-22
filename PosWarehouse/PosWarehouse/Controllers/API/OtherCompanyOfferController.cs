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
    public class OtherCompanyOfferController : ApiController
    {
        private readonly OtherCompanyOfferDal _otherCompanyOfferDal;

        public OtherCompanyOfferController()
        {
            _otherCompanyOfferDal = new OtherCompanyOfferDal();
        }

        public async Task<IHttpActionResult> Get()
        {
            var getData = await _otherCompanyOfferDal.GetOtherCompanyOfferList();

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }
    }
}
