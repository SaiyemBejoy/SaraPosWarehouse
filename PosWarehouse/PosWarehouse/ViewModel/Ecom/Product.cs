using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.Ecom
{
    public class Product
    {
        public int WhProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductStyle { get; set; }
        public string ShortDescription { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }

        public ICollection<ProductItem> ProductItem { get; set; }
    }
}