using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class MaterialDetailsModel
    {
        public int MaterialId { get; set; }

        [Required]
        [DisplayName("Material Name")]
        public string MaterialName { get; set; }

        public string CreateBy { get; set; }

        public string UpdateBy { get; set; }

        public string UpdateDate { get; set; }

        public string CreateDate { get; set; }
    }
}