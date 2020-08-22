using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class ShopToShopExchangeMain
    {
        public int StoreReceiveAutoId { get; set; }
        public int StoreReceiveId { get; set; }
        public string StoreReceiveChallanNo { get; set; }
        public string SeasonName { get; set; }
        public string ReceiveFrom { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceivedDate { get; set; }
        public string StoreReceive_YN { get; set; }
        public string WareHouseId { get; set; }
        public string ShopId { get; set; }
        public string ShopExChallan_YN { get; set; }

        public List<ShopToShopExItem> ShopToShopExItemList { get; set; }

    }

    public class ShopToShopExItem
    {
        public int StoreReceiveItemId { get; set; }
        public int StoreReceiveAutoId { get; set; }
        public int StoreReceiveId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public int ReceiveQuantity { get; set; }
        public double SalePrice { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string BrandName { get; set; }
        public string Umo { get; set; }
        public double Vat { get; set; }


    }
}