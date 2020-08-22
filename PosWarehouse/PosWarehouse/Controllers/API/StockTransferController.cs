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
    public class StockTransferController : ApiController
    {
        private readonly StockTransferDal _stockTransferDal;

        public StockTransferController()
        {
            _stockTransferDal = new StockTransferDal();
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveAllStockTransferData(StockTransferModel objStockTransferModel)
        {
            var returnMessage = await _stockTransferDal.SaveAllStockTransferMainData(objStockTransferModel);
            if (Convert.ToInt32(returnMessage.Item1) > 0 || returnMessage.Item1 != null)
            {
                foreach (var tableData in objStockTransferModel.StockTransferItemList)
                {
                    tableData.StockTransferId = Convert.ToInt32(returnMessage.Item1);
                    await _stockTransferDal.SaveAllStockTransferItemData(tableData);
                }
                return Ok(returnMessage.Item2);

            }
            return BadRequest();
        }

    }
}
