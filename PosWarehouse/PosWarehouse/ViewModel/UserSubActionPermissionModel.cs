using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class UserSubActionPermissionModel
    {
        public int PermissionId { get; set; }

        [Required]
        [DisplayName("Role Name")]
        public string RoleName { get; set; }

        [Required]
        [DisplayName("Action Name")]
        public string ActioinName { get; set; }

        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
        public string Active_YN { get; set; }

        public string CreateBy { get; set; }

        public string CreateDate { get; set; }
    }
}