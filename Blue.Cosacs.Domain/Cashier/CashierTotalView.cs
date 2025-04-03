using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class CashierTotalsView
    {
        public decimal UserAmounts { get; set; }
        public decimal Difference
        {
            get
            {
                if (foreigntender != 0)
                    return Convert.ToDecimal(UserAmounts) - Convert.ToDecimal(foreigntender);
                else
                    return Convert.ToDecimal(UserAmounts) - Convert.ToDecimal(NetReceipts);
            }
        }
       
        public string Comments { get; set; }
        public decimal? NetRecieptsAdj { get { return foreigntender != 0 ? foreigntender : NetReceipts; } } 
    }
}
