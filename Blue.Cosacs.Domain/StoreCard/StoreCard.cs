//using System.ComponentModel.DataAnnotations

using System;

namespace Blue.Cosacs.Shared
{
    public partial class StoreCard : Entity
    {
        
    }

    public class StoreCardChecks
    {
        public bool allowNewStoreCardAccount(StoreCard storeCard, Customer customer)
        {
            bool buttonEnabled = false;
            if (/*customer.StoreCardApproved == true &&*/ (storeCard.AcctNo == String.Empty || storeCard.AcctNo == null))
                buttonEnabled = true;


            return buttonEnabled;
        }

    }
}
