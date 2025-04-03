using System.Collections.Generic;

namespace Blue.Cosacs.Shared
{
    public class StoreCardCreateCheck
    {
        private readonly StoreCardAccountStatus_Lookup acctStatus;
        private  List<View_StoreCardWithPayments> payment;
        private readonly string custid;
        private readonly bool acceptedAgreement;
        private readonly int numberOfCards;

        public StoreCardCreateCheck(StoreCardAccountStatus_Lookup acctStatus, 
                                    List<View_StoreCardWithPayments> payment, 
                                    string custid, 
                                    bool acceptedAgreement, 
                                    int numberOfCards)
        {
            this.acctStatus = acctStatus;
            this.payment = payment;
            this.custid = custid;
            this.numberOfCards = numberOfCards;
            this.acceptedAgreement = acceptedAgreement;

        }

        public bool ReplaceCheck()
        {

            return  IsAccountActive() &&
                    GetActiveCards() == 0 &&
                    GetCancelledCards() > 0;
        }

        public bool AddNewCardCheck()
        {
            return IsAccountActive() &&
                   acceptedAgreement &&
                   GetActiveCards() < numberOfCards;
        }

        private int GetActiveCards()
        {
            var activecards = payment.FindAll(p =>
              p.custid == custid && (StoreCardCardStatus_Lookup.Active.Equals(p.CardStatus) ||
                                     StoreCardCardStatus_Lookup.AwaitingActivation.Equals(p.CardStatus) ||
                                     StoreCardCardStatus_Lookup.CardToBeIssued.Equals(p.CardStatus)));
            return activecards == null ? 0 : activecards.Count;
        }

        private int GetCancelledCards() // per customer id.
        {
            var cancelledcards = payment.FindAll(p => p.custid == custid && (StoreCardCardStatus_Lookup.Cancelled.Equals(p.CardStatus)));
            return cancelledcards == null ? 0 : cancelledcards.Count;
        }

        private bool IsAccountActive()
        {
            return !StoreCardAccountStatus_Lookup.Cancelled.Equals(acctStatus) &&
                   !StoreCardAccountStatus_Lookup.Suspended.Equals(acctStatus) &&
                   !StoreCardAccountStatus_Lookup.Blocked.Equals(acctStatus);
        }
    }
}
