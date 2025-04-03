using System;
using OpenQA.Selenium;
using MbUnit.Framework;
using Blue.Selenium;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using Blue.Cosacs.Selenium.Common;
using Branch = System.Configuration.ConfigurationManager;
using System.Collections;

namespace Blue.Cosacs.Selenium.Service.Helpers
{
    public static class ServiceCharges
    {
        public enum ChargeTos
        {
            Cost = 1,
            ActualCost = 2,
            Customer = 3,
            Deliverer = 4,
            EW = 5,
            FYW = 6,
            Internal = 7,
            Supplier = 8
        }

        public static bool AssertCharges(this IWebDriver webDriver, string expectedCharge, string chargeType, ChargeTos primaryChargeTo)
        {
            var charge = webDriver.GetCharges(chargeType, primaryChargeTo);
            try
            {
                Assert.AreEqual(expectedCharge, charge);
                return true;
            }
            catch (Exception e)
            {

                throw new Exception("Charges donot match. Expected value is " + expectedCharge + ". Actual value is " + charge, e.InnerException);
            }
        }

        public static string GetCharges(this IWebDriver webDriver, string chargeType, ChargeTos primaryChargeTo)
        {
            decimal crg;
            var locator = "[ng-repeat='charge in serviceRequest.DisplayCharges']:nth-child({0}) .ng-binding:nth-child({1})";
            var pct = ((int)primaryChargeTo).ToString();
            var elementlocator = string.Format(locator, chargeType, pct);
            var charge = webDriver.GetText(By.CssSelector(elementlocator));
            charge = charge.TrimStart('$');
            if (!decimal.TryParse(charge.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out crg))
            {
                throw new ArgumentException("Charges should cast to decimal");
            }
            return crg.ToString();
        }

        public static bool AssertTotalCost(this IWebDriver webDriver, string partsCosacs, string partsExternal, string labour, string additional, string taxes, string expectedTotalCost)
        {
            decimal expected;
            decimal partsCosacsCost;
            decimal partsExternalCost;
            decimal labourCost;
            decimal additionalCost;
            decimal taxesCost;

            if (!decimal.TryParse(partsCosacs.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out partsCosacsCost))
            {
                throw new ArgumentException("Parts Cosacs should cast to decimal");
            }
            if (!decimal.TryParse(partsExternal.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out partsExternalCost))
            {
                throw new ArgumentException("Parts Cosacs should cast to decimal");
            }
            if (!decimal.TryParse(labour.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out labourCost))
            {
                throw new ArgumentException("Parts Cosacs should cast to decimal");
            }
            if (!decimal.TryParse(additional.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out additionalCost))
            {
                throw new ArgumentException("Parts Cosacs should cast to decimal");
            }
            if (!decimal.TryParse(taxes.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out taxesCost))
            {
                throw new ArgumentException("Parts Cosacs should cast to decimal");
            }
            if (!decimal.TryParse(expectedTotalCost.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out expected))
            {
                throw new ArgumentException("Parts Cosacs should cast to decimal");
            }
            var totalCost = partsCosacsCost + partsExternalCost + labourCost + additionalCost + taxesCost;
            try
            {
                Assert.AreEqual(expected, totalCost);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Total Cost donot match. Expected value is " + expectedTotalCost + ". Actual value is " + totalCost, e.InnerException);
            }

        }
    }
}
