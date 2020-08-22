using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class OtherCompanyOfferDisModel
    {
        public int OtherCompanyOfferId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string Offer { get; set; }
        [Required]
        public string OfferValidity { get; set; }
        [Required]
        public string EligibleForOffer { get; set; }
        public string CreatedBy { get; set; }
        public string WareHouseId { get; set; }
        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
        public string Active_YN { get; set; }
    }
}