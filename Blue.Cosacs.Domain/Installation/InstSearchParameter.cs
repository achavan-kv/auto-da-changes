using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class InstSearchParameter
    {
        public string AcctNo { get; set; }
        public string CustID { get; set; }
        public string ItemNo { get; set; }
        public int? ItemId { get; set; }
        public int? InstNo { get; set; }
        public DateTime? InstDateFrom { get; set; }
        public DateTime? InstDateTo { get; set; }
        public InstStatus? Status { get; set; }
    }
}
