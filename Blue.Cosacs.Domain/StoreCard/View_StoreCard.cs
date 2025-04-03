using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class View_StoreCard
    {
        public string CardDisplay
        {
            get 
            {
                return String.Format("{0:####-####-####-####}", CardNumber); 
            }
        }

        public string CreditBlockedYN
        {
            get
            {
                return Convert.ToBoolean(CreditBlocked) ? "Yes" : "No";
            }
        }

        public string StatusName
        {
            get
            {
                return StoreCardAccountStatus_Lookup.FromString(Status).Description;
            }
        }
    }
}
