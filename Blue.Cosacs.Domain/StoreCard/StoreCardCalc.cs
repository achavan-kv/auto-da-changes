using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public static class StoreCardCalc
    {
        #region Fixed Months, Variable Monthly Payment
        public static decimal CalculatePayments(decimal balance, decimal interestRate, int termInMonths)
        {
            //if (termInMonths < 2)
            //    throw new ArgumentException("Term in months must be greater than 1.");
            //if (interestRate < 0 || interestRate > 1)
            //    throw new ArgumentException("interestRate");

            return BinarySearchPayments(balance, balance / 2.0m, interestRate, termInMonths, 0, balance);
        }

        private static decimal BinarySearchPayments(decimal balance, decimal monthlyAmount, decimal interestRate, int termInMonths, decimal min, decimal max)
        {
            const decimal tolerance = -0.01m;

            if (min - tolerance >= max)
                return max;

            var remaining = RemainingBalanceImpl(balance, monthlyAmount, interestRate, termInMonths);

            if (tolerance <= remaining && remaining < 0)
                return monthlyAmount;

            if (remaining > 0)
                return BinarySearchPayments(balance, Round(monthlyAmount + (max - monthlyAmount) / 2.0m), interestRate, termInMonths, monthlyAmount, max);
            else
                return BinarySearchPayments(balance, Round(min + (monthlyAmount - min) / 2.0m), interestRate, termInMonths, min, monthlyAmount);
        }

        private static decimal Round(decimal d)
        {
            return Math.Round(d, 2);
        }

        public static decimal RemainingBalance(decimal balance, decimal monthlyAmount, decimal interestRate, int termInMonths)
        {
            if (balance < 0)
                throw new ArgumentException("Balance has to be positive.");
            if (monthlyAmount < 0)
                throw new ArgumentException("Monthly amount has to be positive.");
            if (interestRate < 0 || interestRate > 1)
                throw new ArgumentException("Interest rate has to be between 0 and 1.");
            if (termInMonths < 2)
                throw new ArgumentException("Term in months must be more than 1 month.");

            return RemainingBalanceImpl(balance, monthlyAmount, interestRate, termInMonths);
        }

        private static decimal RemainingBalanceImpl(decimal balance, decimal monthlyAmount, decimal interestRate, int termInMonths)
        {
            if (termInMonths <= 0)
                return balance;

            var newBalance = StoreCardInterest.BalanceAfterInterestCalc(interestRate, balance, monthDuration) - monthlyAmount;
            return RemainingBalanceImpl(newBalance, monthlyAmount, interestRate, termInMonths - 1);
        }

        private const int monthDuration = 30; //Round(365 / 12.0m);
        #endregion

        public class FixedMonthtlyResult
        {
            public int Months { get; set; }
            public decimal TotalAmount { get; set; }
        }

        public static FixedMonthtlyResult CalculateMonths(decimal balance, decimal monthlyAmount, decimal interestRate)
        {
            if (monthlyAmount < 0)
                throw new ArgumentException("Monthly amount must be greater than 0.");
            if (interestRate < 0 || interestRate > 1)
                throw new ArgumentException("Interest rate must be between 0.0 and 1.0.");

            return CalculateMonthsImpl(balance, monthlyAmount, interestRate, termInMonths: 1, maximumTermInMonths: 60, result: new FixedMonthtlyResult());
        }

        private static FixedMonthtlyResult CalculateMonthsImpl(decimal balance, decimal monthlyAmount, decimal interestRate, int termInMonths, int maximumTermInMonths, FixedMonthtlyResult result)
        {
            if (balance <= monthlyAmount)
            {
                result.Months = termInMonths;
                result.TotalAmount += StoreCardInterest.BalanceAfterInterestCalc(interestRate, balance, monthDuration);
                return result;
            }

            if (termInMonths + 1 > maximumTermInMonths)
                return null;

            var newBalance = StoreCardInterest.BalanceAfterInterestCalc(interestRate, balance, monthDuration) - monthlyAmount;
            result.TotalAmount += monthlyAmount;
            return CalculateMonthsImpl(newBalance, monthlyAmount, interestRate, termInMonths + 1, maximumTermInMonths, result);
        }
    }
}
