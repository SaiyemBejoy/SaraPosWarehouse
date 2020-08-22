namespace PosWarehouse.ViewModel.ApiModel
{
    public class DamageTransferItem
    {
        public int DamageTransferItemId { get; set; }
        public string DamageTransferId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public string Remarks { get; set; }
        public double Vat { get; set; }
    }
}