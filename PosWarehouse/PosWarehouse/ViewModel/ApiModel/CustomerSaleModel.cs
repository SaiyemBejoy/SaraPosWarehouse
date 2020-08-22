using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class CustomerSaleModel
    {
        public int CustomerAutoId { get; set; }
        public int CustomerId { get; set; }
        public int CustomerInfoId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CustomerMedium { get; set; }
        public string Discount { get; set; }
        public string EnrollmentDate { get; set; }
        public string ShopId { get; set; }
    }
}