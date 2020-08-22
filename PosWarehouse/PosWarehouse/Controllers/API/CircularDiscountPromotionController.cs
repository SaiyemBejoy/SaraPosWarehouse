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
    public class CircularDiscountPromotionController : ApiController
    {
        private readonly PromotionDAL _objPromotionDal;

        public CircularDiscountPromotionController()
        {
            _objPromotionDal = new PromotionDAL();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(int shopId)
        {
            var getData = await _objPromotionDal.GetAllPromotionProduct(shopId);

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateWarehousePromotion(CircularDiscountPromotionModelApi model)
        {
            if (model.DiscountCircularId > 0 && model.ShopId > 0 )
            {
                var returnMessage = await _objPromotionDal.UpdateWarehousePromotion(model.DiscountCircularId,model.ShopId);
                return Ok(returnMessage);
            }
            return BadRequest();
        }
    }
}
