namespace Blue.Cosacs.Merchandising.Models
{
    public class InventoryModel
    {
        public decimal RegularValue { get; set; }
        public decimal RepossessedValue { get; set; }
        public decimal SparePartsValue { get; set; }
        public int RegularUnits { get; set; }
        public int RepossessedUnits { get; set; }
        public int SparePartsUnits { get; set; }
    }
}