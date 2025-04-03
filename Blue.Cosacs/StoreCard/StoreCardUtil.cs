using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;


namespace Blue.Cosacs.StoreCardUtil
{
    public static class StoreCardCalcs
    {
        public static void AvailableStoreCardBalance(string custID, out decimal? storeCardLimit, out decimal? storeCardAvailable)
        {
            //IP - 22/12/10 - Update and retrieve the Store Card Available
            //storeCardLimit = 0;
            //storeCardAvailable = 0;

            new CustomerRepository().CustomerUpdateAndGetStoreCardAvailable(null, null, custID, out storeCardLimit, out storeCardAvailable);
        }

        public class StoreCardCustDetails
        {
            public string Custid { get; set; }
            public string Title { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }

            public string NameConCat()
            {
                const int max = 26;
                var total = Title.Length + Name.Length + LastName.Length + 2;

                if (total <= max)
                    return string.Format("{0} {1} {2}", Title, Name, LastName);
                else if (total - Name.Length + 1 <= max)
                    return string.Format("{0} {1} {2}", Title, Name.Substring(0, 1), LastName);
                else if (LastName.Length + 2 <= max)
                    return string.Format("{0} {1}", Name.Substring(0, 1), LastName);
                else if (LastName.Length <= max)
                    return string.Format("{0}", LastName);
                else
                    return string.Format("{0}", LastName.Substring(0, 26));
            }
        }

        //IP - 26/11/10 - Moved to Blue.Cosacs.Shared StoreCardValidation.cs
        //public static ValidatationResult VaildateCard(Blue.Cosacs.Shared.StoreCard storeCard)
        //{
        //    var message = new StringBuilder();

        //    if (!storeCard.CardNumber.HasValue)
        //    {
        //        message.AppendLine("Card not found in database.");
        //        return new ValidatationResult() { isValid = false, Message = message.ToString() };
        //    }

        //    if (StoreCardExpDate(storeCard.ExpirationMonth.Value, storeCard.ExpirationYear.Value) < DateTime.Now)
        //    {
        //        message.AppendLine("Card has expired.");
        //    }

        //    if (storeCard.IsDeleted.HasValue)
        //    {
        //        message.AppendLine("Card has been deleted.");
        //    }

        //    if (!storeCard.LostorStolen.HasValue)
        //    {
        //        message.AppendLine("Card has been reported lost or stolen.");
        //    }

        //    if (!storeCard.ActivatedOn.HasValue)
        //    {
        //        message.AppendLine("Card has not been activated.");
        //    }
        //    return new ValidatationResult() { isValid = message.Length > 0, Message = message.ToString() };
        //}

        //IP - 26/11/10 - Moved to Blue.Cosacs.Shared StoreCardValidation.cs
        //private static DateTime StoreCardExpDate(byte month, short year)
        //{
        //    return new DateTime(year, month, 1).AddMonths(1);
        //}


        //[Serializable]
        //public class ValidatationResult
        //{
        //    public bool isValid { get; set; }

        //    public string Message { get; set; }
        //}

     
    }
}
