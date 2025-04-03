using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public static class StoreCardInterest
    {
        //public double interestrate { get; set; }
        //public double averageBalance { get; set; }
        //public TimeSpan ts { get; set; }
        
        public static double GetInterest(double interestRate, double averageBalance, DateTime datefrom, DateTime dateto)
        {
            TimeSpan ts = dateto - datefrom;
            return InterestCalc(interestRate, averageBalance, ts);
        }

        //public static double GetInterest(double interestRate, double averageBalance, double monthlyPayments)
        //{
        //    var interest = 0d;
        //    return Math.Log((interest / Math.Round(averageBalance, 2)) + 1) / (1.0f + interestRate / 100 / 365.0f); 
        //}

        public static double InterestCalc(double interestRate, double averageBalance, TimeSpan term)
        {
            return (Math.Pow((1.0d + interestRate / 100 / 365.0d), term.Days) - 1) * Math.Round(averageBalance, 2);
        }

        public static decimal BalanceAfterInterestCalc(decimal interestRate, decimal averageBalance, int termInDays) 
        {
            return Math.Round(Pow((1.0m + interestRate / 365.0m), termInDays) * averageBalance, 2);
        }

        private static decimal Pow(decimal basis, int power)
        {
            decimal res = 1;
            for (int i = 0; i < power; i++, res *= basis) ;
            return res;
        }

        public static double EffectiveInterest(double interestRate, TimeSpan t)
        {
            return (Math.Pow((1.0d + interestRate / 100 / 365.0d), t.Days) - 1) ;
        }
    }
}
