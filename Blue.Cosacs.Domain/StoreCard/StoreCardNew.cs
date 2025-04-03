using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class StoreCardNew
    {
        public string CustId { get; set; }
        public string AcctNo { get; set; }
        public string Source { get; set; }
        public int User { get; set; }
        public decimal StoreCardLimit { get; set; }
        public decimal StoreCardAvailable { get; set; }
    }
}
