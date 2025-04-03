namespace Unicomer.Cosacs.Model.Models.Orders
{
    public class DeliveryAddress
    {
        public string Region { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string DeliveryArea { get; set; }
        public string AddressLine { get; set; }
        public string ZipCode { get; set; }
        public DeliveryContact DeliveryContact { get; set; }
    }
}
