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
    public class ShopToShopDeliveryController : ApiController
    {
        private readonly ShopToShopDeliveryDal _deliveryDal;

        public ShopToShopDeliveryController()
        {
            _deliveryDal = new ShopToShopDeliveryDal();
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveAllShopToShopDeliveryData(ShopToShopDeliveryModel objShopToShopDeliveryModel)
        {

            string returnMessageForItem = "";
            var returnMessage = await _deliveryDal.SaveShopToShopDeliveryData(objShopToShopDeliveryModel);
            if (Convert.ToInt32(returnMessage) > 0 || returnMessage != null)
            {

                foreach (var tableData in objShopToShopDeliveryModel.ShopToShopRequDeliveryItemModelList)
                {
                    tableData.DeliveryId = Convert.ToInt32(returnMessage);
                    returnMessageForItem = await _deliveryDal.SaveShopToShopDeliveryItem(tableData);

                }
                return Ok(returnMessageForItem);

            }
            return BadRequest();
        }

       
        public async Task<IHttpActionResult> GetAllDeliveryItem(string toShopId, string requisitionNumber)
        {
            ShopToShopDeliveryModel model = new ShopToShopDeliveryModel();
            model = await _deliveryDal.GetAllShopToShopDeliveryData(toShopId, requisitionNumber);
            if (model != null)
            {
                int deliveryId = model.DeliveryId;
                model.ShopToShopRequDeliveryItemModelList = await _deliveryDal.GetAllShopToShopDeliveryItemData(deliveryId);

                return Ok(model);
            }

            return Ok(model);
        }
    }
}
