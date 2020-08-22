using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class CloudPosDataEntryModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
    }

    public class WareHouseBarcodeUpdate
    {
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Style { get; set; }
        public string[] Attribute { get; set; }
        public string BarCode { get; set; }
    }
}