using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyRenewal
    {
        public string WarrantyNumber { get; set; }
        public int Location { get; set; }
        public List<WarrantyRenewalPrice> Warranties { get; set; }
    }

}
