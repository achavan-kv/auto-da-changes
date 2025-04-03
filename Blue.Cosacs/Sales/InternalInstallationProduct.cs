using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Sales
{
    public class InternalInstallationProduct
    {
        public int ItemId { get; set; }
        public string ItemNo { get; set; }
        public string ItemDescription1 { get; set; }
        public string ItemDescription2 { get; set; }
        public decimal? TaxExclusivePrice { get; set; }
        public decimal? TaxInclusivePrice { get; set; }
        public double TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal? DutyFreePrice { get; set; } //As it is in db :(
    }
}
