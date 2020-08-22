using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel
{
    public class DeliveredProduct
    {
        public string StoreDeliveryNumber { get; set; }

        public string RegisterId { get; set; }

        public int DeliveryShopId { get; set; }

        public string DeliveryShopName { get; set; }

        public int SeasonId { get; set; }

        public string SeasonName { get; set; }

        public string PurchaseReceiveNumber { get; set; }

        public string DeliveryDate { get; set; }

        public string RequisitionNo { get; set; }

        public string ReceiveChallanDelivery { get; set; }

        public int DeliveryItemId { get; set; }

        public int ItemId { get; set; }

        public int ProductId { get; set; }

        public string ItemName { get; set; }

        public string BarCode { get; set; }

        public int DeliveryQuantity { get; set; }

        public double PurchasePrice { get; set; }

        public double SalePrice { get; set; }

        public double Vat { get; set; }

        public string Umo { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public string Brand { get; set; }

        public int ReceivedShopId { get; set; }

        public string UpdateBy { get; set; }
    }


    public class TransferReturnProduct
    {
        public string StockTranferChallanNo { get; set; }

        public string StockTransferId { get; set; }

        public int TransferShopIdTo { get; set; }

        public string TransferShopToName { get; set; }

        public int TransferShopIdFrom { get; set; }

        public string TransferShopIdFromName { get; set; }

        public string TransferDate { get; set; }

        public string RequisitionNo { get; set; }

        public int StocktransferItemId { get; set; }

        public int ItemId { get; set; }

        public int ProductId { get; set; }

        public string ItemName { get; set; }

        public string BarCode { get; set; }

        public int TransferQuantity { get; set; }

        public double PurchasePrice { get; set; }

        public double SalePrice { get; set; }

        public double Vat { get; set; }

        public string Umo { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public string Brand { get; set; }

        public int WarehouseId { get; set; }

        public string TransferBy { get; set; }

        public string ReceiveBy { get; set; }
    }

}