using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Blue.Cosacs.Test
{
    [TestFixture]
    public class StoreCardCalcTest
    {
        [Test]
        public void TestRemainingBalanceStillPositive()
        {
            var remaining = Shared.StoreCardCalc.RemainingBalance(1000m, 90m, 0.25m, 12);
            Assert.AreEqual(67.31m, Math.Round(remaining, 2));
        }

        [Test]
        public void TestRemainingBalanceBarelyNegative()
        {
            var remaining = Shared.StoreCardCalc.RemainingBalance(1000m, 95.175m, 0.25m, 12);
            Assert.AreEqual(-2.38m, Math.Round(remaining, 2));
        }

        [Test]
        public void TestMonthlyPaymentsWithinTolerance()
        {
            var monthly = Shared.StoreCardCalc.CalculatePayments(1000m, 0.25m, 12);
            Assert.AreEqual(95m, Math.Round(monthly, 2));
        }

        [Test]
        public void TestMonthsWithinTolerance()
        {
            var result = Shared.StoreCardCalc.CalculateMonths(1000m, monthlyAmount: 95.17m, interestRate: 0.25m);
            Assert.AreEqual(12, result.Months);
        }

        [Test]
        public void TestMonthlyAmountNotEnoughToPayBalanceIn60Months()
        {
            var months = Shared.StoreCardCalc.CalculateMonths(1000m, monthlyAmount: 1000 / 60.0m, interestRate: 0.25m);
            Assert.IsNull(months);
        }

        [Test]
        public void TestMonthlyAmountForOnly1Month()
        {
            var result = Shared.StoreCardCalc.CalculateMonths(1000m, monthlyAmount: 1000m, interestRate: 0.25m);
            Assert.AreEqual(1, result.Months);
        }

        [Test]
        public void TestMonthlyAmountGreaterThanBalance()
        {
            var result = Shared.StoreCardCalc.CalculateMonths(1000m, monthlyAmount: 1001m, interestRate: 0.25m);
            Assert.AreEqual(1, result.Months);
        }

        [Test]
        public void TestMonthlyAmountVeryHighForVeryShortTerm()
        {
            var result = Shared.StoreCardCalc.CalculateMonths(1500m, monthlyAmount: 1000m, interestRate: 0.25m);
            Assert.AreEqual(2, result.Months);
            Assert.AreEqual(1542.15, Math.Round(result.TotalAmount,2));
        }
    }
}
