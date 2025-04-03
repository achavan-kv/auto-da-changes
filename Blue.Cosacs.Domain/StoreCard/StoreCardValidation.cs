using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public static class StoreCardValidation
    {


        //IP - 25/11/10
        public static bool IsStoreCardValid(string storeCardNo)
        {
            long x;
            if (!long.TryParse(storeCardNo, out x))
                return false;
            try
            {
                int multiplier, digit, sum, total = 0;

                for (var i = 1; i <= 16; i++)
                {
                    multiplier = 1 + (i % 2);
                    digit = int.Parse(storeCardNo.Substring(i - 1, 1));
                    sum = digit * multiplier;
                    if (sum > 9)
                        sum -= 9;
                    total += sum;
                }
                return (total % 10 == 0);
            }
            catch
            {
                return false;
            }
        }

        ////IP - 26/11/10 - Moved method from Blue.Cosacs StoreCardUtil.cs
        //public static ValidatationResult VaildateCard(View_StoreCardAll storeCard)
        //{
        //    var message = new StringBuilder();

        //    if (storeCard == null)
        //    {
        //        message.AppendLine("Card not found in database.");
        //        return new ValidatationResult() { isValid = false, Message = message.ToString() };
        //    }
        //    else
        //    {
        //        if (StoreCardExpDate(storeCard.ExpirationMonth, storeCard.ExpirationYear) < DateTime.Now)
        //        {
        //            message.AppendLine("Card has expired.");
        //        }

        //        if (storeCard.StatusCode != "A")
        //        {
        //            message.AppendLine("Card is not activated.");
        //        }

        //        if (storeCard.StatusCode == "C")
        //        {
        //            message.AppendLine("Card is cancelled.");
        //        }

        //        //if (storeCard.IsDeleted == true)
        //        //{
        //        //    message.AppendLine("Card has been deleted.");
        //        //}

        //        ////if (storeCard.LostorStolenOn.HasValue == true)
        //        ////{
        //        ////    message.AppendLine("Card has been reported lost or stolen.");
        //        ////}

        //        //if (!storeCard.ActivatedOn.HasValue)
        //        //{
        //        //    message.AppendLine("Card has not been activated.");
        //        //}
        //    }


        //    return new ValidatationResult() { isValid = message.Length == 0, Message = message.ToString() };
        //}

        //IP - 26/11/10 - Moved from Blue.Cosacs StoreCardUtil.cs

        public static StoreCardValidated ValidateCard(View_StoreCardValidationandLimits card)
        {
            var result = new StoreCardValidated();
            if (card == null)
            {
                result.RejectReason = "This storecard does not exist on database.";
                return result;
            };

            DateTime exp = new DateTime(card.ExpirationYear, card.ExpirationMonth, 1);
            exp = exp.AddMonths(1).AddSeconds(-1);

            if (DateTime.Now > exp)
            {
                result.RejectReason = "This storecard has expired.";
                return result;
            }

            if (!StoreCardAccountStatus_Lookup.Active.Equals(card.StatusCode))
            {
                result.RejectReason = "This storecard is not active.";
                return result;
            }

            if (Convert.ToBoolean(card.creditblocked))
            {
                result.RejectReason = "This storecard is blocked.";
                return result;
            }

            if (card.suspended !=null && card.suspended.Value)
            {
                result.RejectReason = "This storecard is suspended.";
                return result;
            }

            result.Name = card.CardName;
            result.Valid = true;

            result.StoreCardAvailable = card.StoreCardAvailable;
            result.StoreCardLimit = card.StoreCardLimit;
            result.ExpDate = exp;
            result.Acctno = card.Acctno;
            result.CardNo = card.CardNumber;
            result.StoreCardInterest = card.InterestRate;
            result.StoreCardBalance = card.Balance;

            return result;
        }

        private static DateTime StoreCardExpDate(byte month, short year)
        {
            return new DateTime(year, month, 1).AddMonths(1).AddSeconds(-1);
           
        }

        public static ValidatationResult ValidateCardNumber(string storeCardNo)
        {

            var isValid = IsStoreCardValid(storeCardNo);
            var valid = new ValidatationResult();

            if (isValid == false)
            {
                valid.isValid = false;
                valid.Message = "The card number is invalid";
            }
            else
            {
                valid.isValid = true;
                valid.Message = string.Empty;
            }

            return valid;
        }

        [Serializable]
        public class ValidatationResult
        {
            public bool isValid { get; set; }

            public string Message { get; set; }
        }

    }
}
