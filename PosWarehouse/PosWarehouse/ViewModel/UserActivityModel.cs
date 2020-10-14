using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class UserActivityModel
    {
        [Required]
        [DisplayName("Employee ID")]
        public string EmployeeId { get; set; }

        public string EmployeeRole { get; set; }

        public string EmployeeName { get; set; }

        public string UsedController { get; set; }

        public string UsedAction { get; set; }

        public string IpAddress { get; set; }

        public string ActivityDate { get; set; }

        [Required]
        [DisplayName("From Date")]
        public string FromDate { get; set; }

        [Required]
        [DisplayName("To Date")]
        public string ToDate { get; set; }
    }
}