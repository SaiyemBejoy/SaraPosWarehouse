using System.Collections.Generic;

namespace PosWarehouse.ViewModel.ApiModel
{
    public class DamageTransferMain
    {
        public int DamageTransferId { get; set; }

        public string DamageTrschallanNo { get; set; }

        public string RequisitionNo { get; set; }

        public string TransferShopIdfrom { get; set; }

        public string TransferShopNamefrom { get; set; }

        public string TransferedBy { get; set; }

        public string TransferDate { get; set; }

        public List<DamageTransferItem> DamageTransferItemList { get; set; }
    }
}