using Blue.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.NonStocks.Promotions
{
    public class Filter : PagedSearch
    {
        public int? Id { get; set; }
        public int? NonStockId { get; set; }
        public string SKU { get; set; }
        public string NonStock { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public string Fascia { get; set; }
        public string BranchNumber { get; set; }
    }
}
