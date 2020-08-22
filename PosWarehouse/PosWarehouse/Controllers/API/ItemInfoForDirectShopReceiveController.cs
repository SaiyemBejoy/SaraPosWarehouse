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
    public class ItemInfoForDirectShopReceiveController : ApiController
    {
        private readonly InvoiceInfoDAL _invoiceInfo;

        public ItemInfoForDirectShopReceiveController()
        {
            _invoiceInfo = new InvoiceInfoDAL();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetItemInfoForShopReceiveTask(int productId, int itemId)
        {
            if (productId > 0 && itemId > 0)
            {
                var iteminfoForDirectShopReceive = await _invoiceInfo.iteminfoForDirectShopReceiveInfo(productId, itemId);
                return Ok(iteminfoForDirectShopReceive);
            }

            return BadRequest();
        }
    }
}
