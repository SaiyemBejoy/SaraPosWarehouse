using PosWarehouse.DAL;
using PosWarehouse.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PosWarehouse.Controllers.Api
{
    public class GiftVoucherDataUpdateController : ApiController
    {
        private readonly DataExchangeDal _dataExchangeDal;

        public GiftVoucherDataUpdateController()
        {
            _dataExchangeDal = new DataExchangeDal();
        }

        [HttpPost]
        public async Task<IHttpActionResult> GiftVoucherSaleDataUpdate(GiftVoucherDeliveryApiModel objGiftVoucherModel)
        {
            string returnMessage = "";
            returnMessage = await _dataExchangeDal.GiftVoucherDataUpdate(objGiftVoucherModel);
            if (returnMessage != "SUCCESS")
            {
                return BadRequest();
            }
            return Ok(returnMessage);
        }

    }
}
