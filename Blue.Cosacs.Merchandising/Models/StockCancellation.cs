using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models
{
    public class StockChange
    {
        public int ProductId { get; set; }
        public int Location { get; set; }
        public int QuantityReceived { get; set; }
        public int QuantityCancelled { get; set; }
    }
}
