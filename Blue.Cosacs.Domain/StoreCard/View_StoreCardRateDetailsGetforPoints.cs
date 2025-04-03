using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class View_StoreCardRateDetailsGetforPoints
    {
        public string NameWithRate
        {
            get { return String.Format("{0} - {1:0.00}%",Name,PurchaseInterestRate);}
        }
    }
}
