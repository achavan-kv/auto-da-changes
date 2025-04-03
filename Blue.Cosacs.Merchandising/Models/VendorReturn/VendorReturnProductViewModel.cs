namespace Blue.Cosacs.Merchandising.Models
{
    public class VendorReturnProductViewModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductCode { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }

        public int QuantityReceived { get; set; }

        public int QuantityReturned { get; set; }

        public decimal? LastLandedCost { get; set; }  
    }
}