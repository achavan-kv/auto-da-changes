using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model.Types
{
    public class ProductDetailType
    {
        public string UPC { get; set; }
        public string SKU { get; set; }
        public string UPCVendor { get; set; }
        public int Quantity { get; set; }
    }
}
