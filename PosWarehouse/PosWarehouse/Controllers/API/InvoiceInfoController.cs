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
    public class InvoiceInfoController : ApiController
    {
        private readonly InvoiceInfoDAL _invoiceInfo;

        public InvoiceInfoController()
        {
            _invoiceInfo = new InvoiceInfoDAL();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetInvoiceInfoTask(string invoiceNumber, int shopId)
        {
            if (!string.IsNullOrWhiteSpace(invoiceNumber) && shopId > 0)
            {
                var invoiceInfo = await _invoiceInfo.GetInvoiceInfo(invoiceNumber, shopId);

                if (invoiceInfo == null)
                    return BadRequest();

                invoiceInfo.SaleItemList = await _invoiceInfo.GetInvoiceItemInfo(invoiceInfo.SaleInfoAutoId, invoiceInfo.SaleInfoId);

                if(!invoiceInfo.SaleItemList.Any())
                    return BadRequest();

                return Ok(invoiceInfo);
            }

            return BadRequest();
        }
    }
}
