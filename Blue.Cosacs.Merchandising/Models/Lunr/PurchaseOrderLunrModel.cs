namespace Blue.Cosacs.Merchandising.Models
{
    public class PurchaseOrderLunrModel
    {
        public int PurchaseOrderId { get; set; }
        public int VendorId { get; set; }
        public string Vendor { get; set; }
        public string Status { get; set; }
        public string Summary { get; set; }
    }
}