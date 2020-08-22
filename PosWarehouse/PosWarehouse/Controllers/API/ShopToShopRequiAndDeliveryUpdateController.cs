using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.Controllers.Api
{
    public class ShopToShopRequiAndDeliveryUpdateController : ApiController
    {
        private readonly ShopToShopDeliveryDal _deliveryDal;

        public ShopToShopRequiAndDeliveryUpdateController()
        {
            _deliveryDal = new ShopToShopDeliveryDal();
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateReQuiAndDeliveryTable(ShopToShopDeliveryModel objShopToShopDeliveryModel)
        {

            if (!string.IsNullOrWhiteSpace(objShopToShopDeliveryModel.RequisitionNumber))
            {
                var returnMessage = await _deliveryDal.UpdateReQuiAndDeliveryTable(objShopToShopDeliveryModel);
                return Ok(returnMessage);
            }
            return BadRequest();
        }
    }
}
