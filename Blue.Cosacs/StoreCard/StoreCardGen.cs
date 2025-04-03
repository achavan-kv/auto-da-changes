using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Shared;

namespace Blue.Cosacs.StoreCardUtil 
{
    public static class StoreCardGen
    {
        public static long GenerateAndSaveCountryStoreCardNumber()
        {
            int? nextno = 0;
            
            new CountryUpdateStoreCardNextNo()
                .Execute(out nextno);

            nextno = nextno ?? 0;
            var prefix = new CountryMaintenanceGetValue { codename = StoreCM.StoreCardPrefix }.ExecuteScalar().ToString();

            return  GenerateCardNumber(prefix, nextno.Value);
        }

        public static long GenerateCardNumber(string prefix, int nextNumber)
        {
            var newcardnum = prefix + nextNumber.ToString().PadLeft(15 - prefix.Length, '0');
            var plus = 0;
            var sum  = 0;
            var multi = 2;

            for (var i = newcardnum.Length - 1; i >= 0; i--)
            {
                plus = multi * int.Parse(newcardnum[i].ToString());
                multi = 3 - multi;
                sum += plus / 10 + plus % 10;
            }

           var validchar = sum % 10 == 0 ? "0" : (10 - sum % 10).ToString();
           return long.Parse(newcardnum + validchar);
        }

        public static string PadStoreCard(this long storeCardNo)
        {
            return storeCardNo.ToString().PadLeft(16, '0');
        }
    }
}
