using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class SalePaymentInfoModel
    {
        public int SaleInfoAutoId { get; set; }
        public int PaymentTypeAutoId { get; set; }
        public int PaymentTypeId { get; set; }
        public int SaleInfoId { get; set; }
        public double DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
        public string PaymentType { get; set; }
        public double Amount { get; set; }
        public double SubTotal { get; set; }
    }
}