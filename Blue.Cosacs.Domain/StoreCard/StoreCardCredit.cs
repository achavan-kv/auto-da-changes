using System.Data.SqlClient;
using Blue.Cosacs.Shared;
using STL.Common;
using System.Collections.Generic;
using System;

namespace Blue.Cosacs.Shared
{
    public class StoreCardCredit : CommonObject
    {
        public Customer Customer{ get; set; }
        public decimal? RFLimit { get; set; }
        public decimal? RFAvailable { get; set; }
        public List<View_StoreCardTransactionsByCustid> Fintrans { get; set; }
        public bool Active { get; set; }
        private readonly decimal storecardpercent;

        public StoreCardCredit(decimal storecardpercent)
        {
            this.storecardpercent = storecardpercent;
        }

        public decimal? GetStoreCardLimit()
        {
            if (RFLimit.HasValue)
                return RFLimit.Value * storecardpercent / 100.0M;
            else
                return 0;
        }

        public decimal AvailableSpend()
        {
            var spent = 0.0M;

                Fintrans.FindAll(f => 
            {
                 if (f.transtypecode != null)
                    spent += f.transvalue.HasValue ? f.transvalue.Value:0;
                 return f.transtypecode != null;
            });

            var avail = 0m;

            if (Customer.StoreCardLimit.HasValue)
                avail = Customer.StoreCardLimit.Value - spent;
            else
                avail = 0m;

            if (RFAvailable.HasValue && avail > RFAvailable)
                avail = RFAvailable.Value;

            if (avail < 0)
                return 0;
            else
                return Math.Round(avail, 2);
        }

        public void UpdateStoreCardAmount()
        {
            Customer.StoreCardLimit = Customer.StoreCardLimit.HasValue?  Customer.StoreCardLimit.Value : GetStoreCardLimit();
            Customer.AvailableSpend = AvailableSpend();
            if (!Active)
            {
                Customer.StoreCardApproved = Customer.AvailableSpend > 0 ? true : false;
            }
        }
        
    }
}
