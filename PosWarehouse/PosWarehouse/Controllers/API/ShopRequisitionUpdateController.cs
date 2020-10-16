using PosWarehouse.DAL;
using PosWarehouse.ViewModel.ApiModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PosWarehouse.Controllers.Api
{
    public class ShopRequisitionUpdateController : ApiController
    {
        private readonly ShopToShopRequisitionDAL _dal;

        public ShopRequisitionUpdateController()
        {
            _dal = new ShopToShopRequisitionDAL();
        }


        [HttpPost]
        public async Task<IHttpActionResult> DeleteShopToShopRequisition(ShopToShopRequisitionMainModel objShopToShopRequisitionMainModel)
        {

            var returnMessage = await _dal.DeleteShopRequisitionMainModel(objShopToShopRequisitionMainModel);
            if (returnMessage != null)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
