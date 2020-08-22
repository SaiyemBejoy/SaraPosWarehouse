using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.Ecom
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProductSubCategory> ProductSubCategories { get; set; }
    }

    public class ProductSubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}