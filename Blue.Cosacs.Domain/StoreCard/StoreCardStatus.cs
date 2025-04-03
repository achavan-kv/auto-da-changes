using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class StoreCardStatus
    {

        public string StatusName
        {
            get
            {
                return StoreCardAccountStatus_Lookup.FromString(StatusCode).Description;
            }
        }

        public string AccountStatus { get; set; }
        public string AccountStatusName
        {
            get
            {
                return StoreCardAccountStatus_Lookup.FromString(AccountStatus).Description;
            }
        }

    }
}
