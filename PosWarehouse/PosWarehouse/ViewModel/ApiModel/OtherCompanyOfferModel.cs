using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class OtherCompanyOfferModel
    {
        public int OtherCompanyOfferId { get; set; }
        public string CompanyName { get; set; }
        public string Offer { get; set; }
        public string OfferValidity { get; set; }
        public string EligibleForOffer { get; set; }
        public string CreatedBy { get; set; }
        public string WareHouseId { get; set; }
    }
}