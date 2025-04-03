using System.Collections.Generic;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyRenewalPrice
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public short Length { get; set; }
        public decimal? TaxRate { get; set; }
        public bool Free { get; set; }
        public bool Deleted { get; set; }
        public decimal Price { get; set; }
        public int Location { get; set; }
        public decimal CostPrice { get; set; }
        public string TypeCode { get; set; }        // #17313
    }
}