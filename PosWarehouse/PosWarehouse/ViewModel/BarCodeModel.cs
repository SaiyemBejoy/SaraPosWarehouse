using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class BarCodeModel
    {
        public int ItemId { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string BrandName { get; set; }

        public string SalePrice { get; set; }

        public string PurchasePrice { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductStyle { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }

        public int VendorId { get; set; }

        public string VendorName { get; set; }

    }

    public class BarCodeTableModel
    {
        public int PrintedBarCodeId { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string SalePrice { get; set; }

        public string BarCodeImageString { get; set; }
        public byte[] BarCodeImageArray { get; set; }

        public int VendorId { get; set; }
        public string BrandName { get; set; }

        public string Quantity { get; set; }

        public string UpdateBy { get; set; }

        public string WareHouseId { get; set; }

        public string ShopId { get; set; }

    }
}