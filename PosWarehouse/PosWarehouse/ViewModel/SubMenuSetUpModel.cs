using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class SubMenuSetUpModel
    {
        [Required]
        [DisplayName("Menu")]
        public int MenuId { get; set; }
        [DisplayName("Menu Name")]
        public string MenuName { get; set; }

        public int SubMenuId { get; set; }
        [Required]
        [DisplayName("Sub Menu Name")]
        public string SubMenuName { get; set; }

        [Required]
        [DisplayName("Sub Menu URL")]
        public string SubMenuURL { get; set; }

        [Required]
        [DisplayName("Sub Menu Icon")]
        public string SubMenuIcon { get; set; }

        [Required]
        [DisplayName("Sub Menu Order")]
        public int SubMenuOrder { get; set; }

        //[DisplayName("Active Status")]
        //public bool ActiveStatus { get; set; }
        //public string Active_YN { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }
    }
}