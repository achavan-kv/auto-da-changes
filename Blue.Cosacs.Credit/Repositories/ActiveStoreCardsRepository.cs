using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Credit.Repositories
{
    public class ActiveStoreCardsRepository
    {
        public List<ActiveStoreCardsResult> getActiveStoreCards()
        {
            using (var scope = Context.Write())
            {
                var activeStoreCards = (from st in scope.Context.ActiveStoreCardsView
                                        select new ActiveStoreCardsResult
                                        {
                                            CardNumber = st.cardNumber,
                                            NameOnCard = st.nameOnCard,
                                            AvailableSpend = st.availableSpend,
                                            AccountNumber = st.accountNumber
                                        }).ToList();

                return activeStoreCards;
            }
        }
    }
}
