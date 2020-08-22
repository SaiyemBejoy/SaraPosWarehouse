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
    public class ShopToShopRequisitionController : ApiController
    {
        private readonly ShopToShopRequisitionDAL _dal;

        public ShopToShopRequisitionController()
        {
            _dal = new ShopToShopRequisitionDAL();
        }

        public async Task<IHttpActionResult> GetActiveShop()
        {
            var getData = await _dal.GetShopList();

            if (getData == null || getData.Count <= 0)
            {
                return NotFound();
            }

            return Ok(getData);
        }

        public async Task<IHttpActionResult> GetActiveShopStock(int shopId)
        {
            var model = new List<ShopToShopRequisitionModel>();
            var getData = await _dal.GetShopStockList(shopId);

            if (getData == null || getData.Count <= 0)
            {
                return Ok(model);
            }

            return Ok(getData);
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveShopToShopRequisition(ShopToShopRequisitionMainModel model)
        {

            string returnMessageForItem = "";
            var returnMessage = await _dal.SaveShopRequisitionMainModel(model);
            if (Convert.ToInt32(returnMessage) > 0 || returnMessage != null)
            {

                foreach (var tableData in model.ShopToShopRequisitionMainItemList)
                {
                    tableData.RequisitionId = Convert.ToInt32(returnMessage);
                    returnMessageForItem = await _dal.SaveShopRequisitionMainItemModel(tableData);

                }
                return Ok(returnMessageForItem);

            }
            return BadRequest();
        }

        public async Task<IHttpActionResult> GetShopRequisitionList(int fromShopId)
        {
            List<ShopToShopRequisitionMainModel> model = new List<ShopToShopRequisitionMainModel>();
            model = await _dal.GetShopRequisitionMainData(fromShopId);
            if (model != null)
            {
                foreach (var item in model)
                {
                    int requitionId = item.RequisitionId;
                    item.ShopToShopRequisitionMainItemList = await _dal.GetShopRequisitionMainItemData(requitionId);
                }
                return Ok(model);
            }

            return Ok(model);
        }
        public async Task<IHttpActionResult> GetShopRequisitionFromList(int toShopId)
        {
            List<ShopToShopRequisitionMainModel> model2 = new List<ShopToShopRequisitionMainModel>();
            model2 = await _dal.GetFromShopRequisitionMainData(toShopId);
            if (model2 != null)
            {
                foreach (var item in model2)
                {
                    int requitionId = item.RequisitionId;
                    item.ShopToShopRequisitionMainItemList = await _dal.GetShopRequisitionMainItemData(requitionId);
                }
                return Ok(model2);
            }

            return Ok(model2);
        }

    }
}
