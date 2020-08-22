using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class HomeModel
    {
    }

    public class MenuModel
    {
        public List<MenuMain> MenuMains { get; set; }
    }

    public class MenuMain
    {
        public int MenuMainId { get; set; }

        public string MenuMainName { get; set; }

        public string MenuUrl { get; set; }

        public string EmployeeRole { get; set; }

        public string UserId { get; set; }//This is for Role Update in permision screen
        public string ActiveYn { get; set; }//This is for Role Update in permision screen
        public string UserName { get; set; }//This is for Role Update in permision screen

        public string MenuIcon { get; set; }

        public string UpdateBy { get; set; }

        public string HeadOfficeId { get; set; }

        public string BranchOfficeId { get; set; }

        public List<MenuSub> MenuSubs { get; set; }
    }

    public class MenuSub
    {
        public int MenuMainId { get; set; }

        public int MenuSubId { get; set; }

        public string MenuSubName { get; set; }

        public string MenuUrl { get; set; }

        public string MenuIcon { get; set; }

        public string EmployeeRole { get; set; }

        public string UpdateBy { get; set; }

        public string HeadOfficeId { get; set; }

        public string BranchOfficeId { get; set; }
    }
    public class MenuSubPermesionDelete
    {
        public int MenuMainId { get; set; }

        public int MenuSubId { get; set; }

        public string EmployeeRole { get; set; }
    }
}
