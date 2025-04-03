namespace Blue.Cosacs.Warehouse
{
    public class BookingSearch
    {
        public string DeliveryZone { get; set; }
        public string ProductCategory { get; set; }
        public int? DeliveryBranch { get; set; }
        public string Fascia { get; set; }
        public bool Internal { get; set; }
        public int? ReceivingLocation { get; set; }
    }
}
