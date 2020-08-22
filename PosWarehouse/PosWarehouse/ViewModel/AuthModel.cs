using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.ViewModel
{
    public class AuthModel
    {
        [Required]
        public string EmployeeId { get; set; }

       
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string EmployeePassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Re-type password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string RetypePassword { get; set; }

        [Required]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email")]
        public string EmployeeEmail { get; set; }

       
        [Display(Name = "Phone")]
        public string EmployeePhone { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Role")]
        public string EmployeeRole { get; set; }

        [Display(Name = "Designation")]
        public string EmployeeDesignation { get; set; }

        public string EmployeeImage { get; set; }
        public HttpPostedFileBase Image { get; set; }
        [Display(Name = "Area")]
        public string EmployeeArea { get; set; }

        public IEnumerable<RequisitionMainModel> RequisitionMainModels { get; set; }
        public IEnumerable<StockTransferModel> StockTransferModels { get; set; }

        public bool Message { get; set; }

        public string UpdateBy { get; set; }
        public string WareHouseId { get; set; }
        public string ShopId { get; set; }
    }
}