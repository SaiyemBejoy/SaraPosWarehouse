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
    public class DeliveredProductController : ApiController
    {
        private readonly DeliveredProductDAL _deliveredProductDal;

        public DeliveredProductController()
        {
            _deliveredProductDal = new DeliveredProductDAL();
        }

        public async Task<IHttpActionResult> Get()
        {
            var getData = await _deliveredProductDal.GetAllDeliveredProduct();

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }

        public async Task<IHttpActionResult> Get(int shopId)
        {
            var getData = await _deliveredProductDal.GetAllDeliveredProduct(shopId);

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateShop(DeliveredProduct model)
        {
            int deliveryShop = model.DeliveryShopId;
            string deliveryNumber = model.StoreDeliveryNumber;
            string updateBy = model.UpdateBy;
            if (deliveryShop > 0 && !string.IsNullOrWhiteSpace(deliveryNumber) && !string.IsNullOrWhiteSpace(updateBy))
            {
                var returnMessage = await _deliveredProductDal.DeliveryProductUpdateByShop(deliveryNumber, deliveryShop, updateBy);

                if(returnMessage == "SUCCESS")
                    return Ok(returnMessage);
            }

            return BadRequest();
        }
    }
}
