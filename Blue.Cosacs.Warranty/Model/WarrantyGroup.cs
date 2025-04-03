using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyGroup
    {
        public int ParentId { get; set; }
        public string WarrantyType { get; set; }
        public int Count { get; set; }
    }
}
