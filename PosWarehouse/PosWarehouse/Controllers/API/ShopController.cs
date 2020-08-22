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
    public class ShopController : ApiController
    {
        private readonly SetupDAL _setupDal;

        public ShopController()
        {
            _setupDal = new SetupDAL();
        }

        public async Task<IHttpActionResult> Get()
        {
            var getData = await _setupDal.GetShopList(null);

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }

        public async Task<IHttpActionResult> Get(int shopId)
        {
            var getData = await _setupDal.GetShopList(shopId);

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateShop(ShopModel model)
        {
            if (model.ShopId > 0 && !string.IsNullOrWhiteSpace(model.ShopUrl))
            {
                var returnMessage = await _setupDal.UpdateShop(model);
                return Ok(returnMessage);
            }

            return BadRequest();
        }
    }
}
