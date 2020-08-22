using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Controllers.Api
{
    public class GiftVoucherController : ApiController
    {
        private readonly DataExchangeDal _dataExchangeDal;

        public GiftVoucherController()
        {
            _dataExchangeDal = new DataExchangeDal();
        }

        public async Task<IHttpActionResult> Get()
        {
            var getData = await _dataExchangeDal.GetAllGiftVoucher();

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveGiftVoucherDepositData(GiftVoucherDeliveryApiModel objGiftVoucherDeliveryApiModel)
        {
            string returnMessage = "";
            returnMessage = await _dataExchangeDal.SaveGiftVoucherDepositData(objGiftVoucherDeliveryApiModel);
            if (returnMessage != "SUCCESS")
            {
                return BadRequest();
            }
            return Ok(returnMessage);
        }
    }
}
