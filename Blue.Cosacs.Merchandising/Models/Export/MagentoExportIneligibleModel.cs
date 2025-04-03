namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    using Blue.Cosacs.Merchandising.Helpers;

    using FileHelpers;

    [DelimitedRecord(",")]
    public class MagentoExportIneligibleModel
    {
        [FieldTitle("Product Code")]
        public string ProductCode { get; set; }

        [FieldTitle("Desc 01")]
        public string Desc01 { get; set; }
        [FieldTitle("Desc 02")]
        public string Desc02 { get; set; }

        [FieldTitle("Online Date Added")]
        public DateTime? OnlineDateAdded { get; set; }
    }
}