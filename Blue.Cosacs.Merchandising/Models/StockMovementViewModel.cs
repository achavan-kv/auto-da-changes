using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    public class StockMovementViewModel
    {
        public string SKU { get; set; }

        public string LongDescription { get; set; }

        public DateTime MovementSince { get; set; }

        public List<StockMovementTypeViewModel> Movements { get; set; }

        public int InitalCount { get; set; }

        public DateTime? MovementUntil { get; set; }
    }
}
