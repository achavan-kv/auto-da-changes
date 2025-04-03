using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class View_StoreCardWithPayments
    {

        public string CardNameCombined
        {
            get { return String.Format("{0} - {1}", CardName, CardNumber.ToString().Substring(12)); }
        }

        public bool NewCard { get; set; }

        public string NewCardCustid { get; set; }

        public string CardStatusName
        {
            get
            {
                return StoreCardAccountStatus_Lookup.FromString(CardStatus).Description;
            }
        }
        public decimal PendingInterest { get; set; }
        public decimal AverageBalance { get; set; }
    }

    public partial class View_StoreCardHistory
    {
        public short ContactMonths { get; set; }
    } 
	

}
