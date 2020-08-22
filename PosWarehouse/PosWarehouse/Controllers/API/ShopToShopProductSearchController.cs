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
    public class ShopToShopProductSearchController : ApiController
    {
        private readonly ShopToShopRequisitionDAL _dal;

        public ShopToShopProductSearchController()
        {
            _dal = new ShopToShopRequisitionDAL();
        }

        public async Task<IHttpActionResult> GetActiveShopStockProduct(int shopId, string styleName)
        {
            var model = new List<ShopToShopRequisitionModel>();
            var getData = await _dal.GetShopSearchProductList(shopId, styleName);

            if (getData == null || getData.Count <= 0)
            {
                return Ok(model);
            }

            return Ok(getData);
        }
    }
}
