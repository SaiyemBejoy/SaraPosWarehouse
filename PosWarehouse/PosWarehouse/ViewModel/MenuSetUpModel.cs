using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class MenuSetUpModel
    {

        public int MenuId { get; set; }

        [Required]
        [DisplayName("Menu Name")]
        public string MenuName { get; set; }

        [Required]
        [DisplayName("Menu URL")]
        public string MenuURL { get; set; }

        [Required]
        [DisplayName("Menu Icon")]
        public string MenuIcon { get; set; }

        [Required]
        [DisplayName("Menu Order")]
        public int MenuOrder { get; set; }

        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
        public string Active_YN { get; set; }

        public string UpdateBy { get; set; }

        public string UpdateDate { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }
}