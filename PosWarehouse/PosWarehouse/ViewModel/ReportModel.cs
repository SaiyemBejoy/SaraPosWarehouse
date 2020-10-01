using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class ReportModel
    {
    }
    public class SaleDetailsSummary
    {
        public int SaleSummaryId { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        [DisplayName("Shop Name")]
        public int ShopId { get; set; }
        [DisplayName("Category")]
        public string CategoryId { get; set; }

        [DisplayName("SubCategory")]
        public string SubCategoryId { get; set; }


        public int ProductId { get; set; }
        public string ProductStyle { get; set; }

        [DisplayName("InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        public string DesignerId { get; set; }

        public string MerchandiserId { get; set; }

        public string RadioFor { get; set; }

        public string ReportType { get; set; }
    }

    public class PurchaseOrderReport
    {
        public string PurchaseOrderNumber { get; set; }

        public string OrderDatefrom { get; set; }

        public string OrderDateTo { get; set; }

        public string DeliveryDateFrom { get; set; }

        public string DeliveryDateTo { get; set; }

        public int ShopId { get; set; }

        public string RadioFor { get; set; }

        public string ReportType { get; set; }
    }

    public class StoreDeliveryReport
    {
        public string DeliveryNumber { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        [DisplayName("Product Style")]
        public string ProductStyle { get; set; }

        public int CategoryId { get; set; }

        public int SubCategoryId { get; set; }

        public string DesignerId { get; set; }

        public string DesignerName { get; set; }

        public string MerchandiserId { get; set; }

        public string MerchandiserName { get; set; }

        public string Barcode { get; set; }

        public int ShopId { get; set; }

        public string RadioFor { get; set; }

        public string ReportType { get; set; }
    }

    public class StockSummaryReport
    {
        public int StockSummaryId { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }

        [DisplayName("Shop Name")]
        public int ShopId { get; set; }

        [DisplayName("Category")]
        public string CategoryId { get; set; }

        [DisplayName("SubCategory")]
        public string SubCategoryId { get; set; }

        [DisplayName("Season")]
        public int SeasonId { get; set; }

        [DisplayName("Color")]
        public string Color { get; set; }

        public string Barcode { get; set; }

        public List<int> ProductIdList { get; set; }
        public string ProductIds { get; set; }

       // public string StyleName { get; set; }

        public string RadioFor { get; set; }

        public string RadioForZero { get; set; }

        public string ReportType { get; set; }
    }

    public class CustomerReport
    {
        public int CustomerId { get; set; }

        [DisplayName("Shop Name")]
        public int ShopId { get; set; }

        public string RadioFor { get; set; }

        public string ReportType { get; set; }
    }

    public class PeriodicalStockReportModel
    {
        [DisplayName("Start Date")]
        public string FromDate { get; set; }

        [DisplayName("End Date")]
        public string ToDate { get; set; }

        [DisplayName("Shop Name")]
        public int ShopId { get; set; }

        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [DisplayName("SubCategory")]
        public int SubCategoryId { get; set; }

        public string Barcode { get; set; }

        [DisplayName("Style Name")]
        public string StyleName { get; set; }

        public string Fabrics { get; set; }

        [DisplayName("Designer Name")]
        public string DesignerName { get; set; }

        [DisplayName("Merchandiser Name")]
        public string MerchandiserName { get; set; }


        [DisplayName("Warehouse")]
        [Required]
        public string WarehouseId { get; set; }

        public string UpdatedBy { get; set; }

        public string ReportType { get; set; }

        public bool CheckBox { get; set; }
    }

    public class PeriodicalStockReportDataModel
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public string StyleName { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string DesignerId { get; set; }
        public string DesignerName { get; set; }
        public string MerchandiserId { get; set; }
        public string MerchandiserName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }

        public DateTime Date { get; set; }

        public double Discount { get; set; }
        public double DiscountSuggestion { get; set; }
        public double ProfitPercent { get; set; }
        public double Profit { get; set; }
        public double Vat { get; set; }

        public int OpeningStock { get; set; }
        public double OpeningStockCpu { get; set; }
        public double OpeningStockMrp { get; set; }
        public int ReceivedQty { get; set; }
        public int ReturnQty { get; set; }
        public int ShopReceivedQty { get; set; }
        public int ShopTransferQty { get; set; }
        public int DamagedQty { get; set; }
        public int SaleQty { get; set; }
        public int AdjustQty { get; set; }
        public int ClosingStock { get; set; }
        public double ClosingStockCpu { get; set; }
        public double ClosingStockMrp { get; set; }
    }

    public class DeliveredProductList
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public string StyleName { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string DesignerId { get; set; }
        public string DesignerName { get; set; }
        public string MerchandiserId { get; set; }
        public string MerchandiserName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }

        public DateTime DeliveredDate { get; set; }
    }

    public class SaleProductList
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public string Barcode { get; set; }
        public double SalePrice { get; set; }
        public int Quantity { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
    }

    public class ReceivedFromShop
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public string Barcode { get; set; }
        public double SalePrice { get; set; }
        public int Quantity { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
    }

    public class ReturnProduct
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public string Barcode { get; set; }
        public double SalePrice { get; set; }
        public int Quantity { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
    }

    public class TransferProduct
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public string Barcode { get; set; }
        public double SalePrice { get; set; }
        public int Quantity { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
    }

    public class DamagedProduct
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public string Barcode { get; set; }
        public double SalePrice { get; set; }
        public int Quantity { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
    }


    public class StockTransferDetails
    {
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        [DisplayName("Shop Name")]
        [Required]
        public int ShopId { get; set; }

        public string TransferChallanNum { get; set; }

        public string RadioFor { get; set; }

        public string ReportType { get; set; }
    }

    public class SolPurchaseReceiveReport
    {
        public int ItemId { get; set; }

        public List<int> ProductId { get; set; }

        [DisplayName("Style")]
        public string ProductStyle { get; set; }

        public string ItemName { get; set; }

        public string Barcode { get; set; }

        public int HoldReceiveQuantity { get; set; }

        public int RescanDelivery { get; set; }

        public int Quantity { get; set; }

        public double SalePrice { get; set; }

        public string RadioFor { get; set; }

        public string RadioForZero { get; set; }

        public string ReportType { get; set; }

        public string ProductIDs { get; set; }
    }


}