using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.Controllers.Api
{
    public class VoidInvoiceController : ApiController
    {
        private readonly DataExchangeDal _dataExchangeDal;

        public VoidInvoiceController()
        {
            _dataExchangeDal = new DataExchangeDal();
        }

        [HttpPost]
        public async Task<IHttpActionResult> VoidInvoice(SaleInfoModel model)
        {
            string invoiceNumber = model.InvoiceNumber;
            string shopId = model.ShopId;
            if (!string.IsNullOrWhiteSpace(invoiceNumber) && !string.IsNullOrWhiteSpace(shopId))
            {
                var returnMessage = await _dataExchangeDal.VoidInvoiceNumber(invoiceNumber, shopId);

                if (returnMessage == "SUCCESS")
                    return Ok(returnMessage);
            }

            return BadRequest();
        }
    }
}
