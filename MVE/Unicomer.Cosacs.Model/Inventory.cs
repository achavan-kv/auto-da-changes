using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class PriceUpdateList
    {
        public string SupplierId { get; set; }
        public int Category { get; set; }
        public string ItemNo { get; set; }
        public int BranchNo { get; set; }
        public string Price { get; set; }
        public string CostPrice { get; set; }
        public string TaxrateId { get; set; }
        public string DateActivated { get; set; }
    }

    public class PriceUpdate
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public List<PriceUpdateList> PriceUpdateList { get; set; }
    }

}
