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
    public class WarehouseStockController : ApiController
    {
        private readonly DataExchangeDal _dataExchangeDal;

        public WarehouseStockController()
        {
            _dataExchangeDal = new DataExchangeDal();
        }

        public async Task<IHttpActionResult> Get()
        {
            var getData = await _dataExchangeDal.GetAllWareHouseProductStockList();

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }
        public async Task<IHttpActionResult> GetProductStockByStyle(string styleName)
        {
            var getData = await _dataExchangeDal.GetAllWareHouseProductStockListByStyleName(styleName);

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }
    }
}
