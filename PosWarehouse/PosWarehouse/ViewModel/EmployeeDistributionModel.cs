using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class EmployeeDistributionModel
    {
        public int EmployeeDisId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        [DisplayName("Role")]
        public string EmployeeRole { get; set; }
        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
        public string ActiveYn { get; set; }
        public string Password { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string CreateBy { get; set; }
        public string CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateDate { get; set; }

    }
    public class EmployeeListModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Designation { get; set; }
    }

}