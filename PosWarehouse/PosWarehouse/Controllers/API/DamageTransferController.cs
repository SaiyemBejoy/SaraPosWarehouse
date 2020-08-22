using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.Controllers.Api
{
    public class DamageTransferController : ApiController
    {
        private readonly DamageTransferDal _damageTransferDal;

        public DamageTransferController()
        {
            _damageTransferDal = new DamageTransferDal();
        }

        [HttpPost]
        public async Task<IHttpActionResult> SaveAllDamageTransferData(DamageTransferMain objDamageTransferMainModel)
        {

            string returnMessageForItem = "";
            var returnMessage = await _damageTransferDal.SaveAllDamageTransferMainData(objDamageTransferMainModel);
            if (Convert.ToInt32(returnMessage) > 0 || returnMessage != null)
            {
                foreach (var tableData in objDamageTransferMainModel.DamageTransferItemList)
                {
                    tableData.DamageTransferId = returnMessage;
                    returnMessageForItem = await _damageTransferDal.SaveAllDamageTransferItemData(tableData);
                }
                return Ok(returnMessageForItem);
            }
            return BadRequest();
        }

    }
}