using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class StoreCardAccountResult
    {
        //private StoreCardStatus storeCardStatus = new StoreCardStatus();
        //public StoreCardStatus StoreCardStatus
        //{
        //    get { return storeCardStatus; }
        //    set { storeCardStatus = value; }
        //}

        public string AccountStatus
        {
            get;
            //{
            //    if (storecard.Count > 0)
            //        return storecard[0].StatusCode;
            //    else
            //        return StoreCardAccountStatus_Lookup.Unknown.Code;
            //}
            set;
        }

        public string MainCustid { get; set; }

        public bool AcceptedAgreement;

        public string GetCardStatusCodeByCard(long cardno)
        {
            var status = StoreCardWithPayments.Find(s => s.CardNumber == cardno).CardStatus;
            if (status == string.Empty)
                return StoreCardCardStatus_Lookup.Unknown.Code;
            else
                return status;
        }

        public DateTime? GetCardStatusDateCodeByCard(long cardno)
        {
            return StoreCardWithPayments.Find(s => s.CardNumber == cardno).CardStatusDateChanged;
        }

        public View_StoreCardWithPayments GetPaymentByCard(long cardno)
        {
            return StoreCardWithPayments.Find(s => s.CardNumber == cardno);
        }

        //public Customer Customer { get; set; }

        private List<View_StoreCardWithPayments> storecard = new List<View_StoreCardWithPayments>();
        public List<View_StoreCardWithPayments> StoreCardWithPayments 
        {
            get { return storecard; }
            set { storecard = value; }
        }

        public Acct Acct { get; set; }
        public List<StoreCardStatement> StoreCardStatements { get; set; }
        public List<CustAddress> Addresses { get; set; }
        public List<Customer> Customers { get; set; }
        public List<View_StoreCardRateDetailsGetforPoints> Rates { get; set; }
        public List<view_FintranswithTransfers> Fintransfers { get; set; }
        public List<View_StoreCardHistory> History { get; set; }
    }
}
