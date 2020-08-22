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
    public class ShopToShopReturnController : ApiController
    {
        private readonly DeliveredProductDAL _deliveredProductDal;
        private readonly HomeDal _homeDal;

        public ShopToShopReturnController()
        {
            _deliveredProductDal = new DeliveredProductDAL();
            _homeDal = new HomeDal();
        }

        public async Task<IHttpActionResult> Get(int shopId)
        {
            var getData = await _deliveredProductDal.GetAllReturnProduct(shopId);

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateWarehouse(TransferReturnProduct model)
        {
            int transferShopIdTo = model.TransferShopIdTo;
            string stockTranferChallanNo = model.StockTranferChallanNo;
            string updateBy = model.ReceiveBy;
            if (transferShopIdTo > 0 && !string.IsNullOrWhiteSpace(stockTranferChallanNo) && !string.IsNullOrWhiteSpace(updateBy))
            {
                //var returnMessage = await _deliveredProductDal.ReturnProductUpdateByShop(stockTranferChallanNo, transferShopIdTo, updateBy);

                    var getShopUrl = await _homeDal.GetShopUrlByChallanNo(stockTranferChallanNo);

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(getShopUrl);

                        HttpResponseMessage response = await client.PostAsJsonAsync(
                            "StockReturnReceive", model);
                        response.EnsureSuccessStatusCode();

                        if (response.IsSuccessStatusCode)
                        {
                            var returnMessage = await _deliveredProductDal.ReturnProductUpdateByShop(stockTranferChallanNo, transferShopIdTo, updateBy);

                            if (returnMessage == "SUCCESS")
                            {
                                return Ok(returnMessage);
                            }

                            return BadRequest();
                        }
                    }
                    
            }

            return BadRequest();
        }


    }
}
