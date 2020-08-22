using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class LogActionModel
    {
        public int LogActionAutoId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string IpAddress { get; set; }
        public string MenuUrl { get; set; }

    }
}