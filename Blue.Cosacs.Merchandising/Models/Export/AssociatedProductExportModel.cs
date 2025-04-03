using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    using FileHelpers;

    [DelimitedRecord(",")]
    public class AssociatedProductExportModel
    {
        public string Level1 { get; set; }

        public string Level2 { get; set; }

        public string Level3 { get; set; }

        public string Level4 { get; set; }

        public string Sku { get; set; }
    }
}