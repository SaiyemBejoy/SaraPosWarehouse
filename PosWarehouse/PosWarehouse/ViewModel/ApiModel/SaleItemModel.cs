using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class SaleItemModel
    {
        public int SaleInfoAutoId { get; set; }
        public int SaleItemAutoId { get; set; }
        public int SaleItemId { get; set; }
        public int SaleInfoId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string StyleName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Vat { get; set; }
        public double Total { get; set; }   
    }
}