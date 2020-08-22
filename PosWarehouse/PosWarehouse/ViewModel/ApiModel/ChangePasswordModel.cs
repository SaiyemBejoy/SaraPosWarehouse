using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class ChangePasswordModel
    {
        public string EmployeeId { get; set; }

        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

        public string ShopId { get; set; }

        public string ShopName { get; set; }
    }
}