using FileHelpers;

namespace Blue.Cosacs.Merchandising.Models
{
    [DelimitedRecord(",")]
    public class SetExportModel
    {
        public string Parent { get; set; }
        public string Child { get; set; }
        public string Quantity { get; set; }
    }
}
