using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;

namespace Blue.Cosacs.Selenium.Configuration.Helpers
{
    public static class HubPage
    {
        public static void CheckColumnHeadersForHubTable(this IWebDriver webDriver)
        {
            webDriver.IsTextPresent(By.CssSelector("[data-module='Hub'] th:nth-child(1)"), "Queue");
            webDriver.IsTextPresent(By.CssSelector("[data-module='Hub'] th:nth-child(2)"), "Initial");
            webDriver.IsTextPresent(By.CssSelector("[data-module='Hub'] th:nth-child(3)"), "Success");
            webDriver.IsTextPresent(By.CssSelector("[data-module='Hub'] th:nth-child(4)"), "Poison");
        }

        public static void OpenWarehouseBookingSubmit(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Warehouse.Booking.Submit"));
            webDriver.FindElement(By.PartialLinkText("Warehouse.Booking.Submit")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 1 (Warehouse.Booking.Submit)");
        }

        public static void OpenWarehouseBookingCancel(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Warehouse.Booking.Cancel"));
            webDriver.FindElement(By.PartialLinkText("Warehouse.Booking.Cancel")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 2 (Warehouse.Booking.Cancel)");
        }

        public static void OpenCosacsBookingDeliver(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Cosacs.Booking.Deliver"));
            webDriver.FindElement(By.PartialLinkText("Cosacs.Booking.Deliver")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 3 (Cosacs.Booking.Deliver)");
        }

        public static void OpenCosacsBookingCancel(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Cosacs.Booking.Cancel"));
            webDriver.FindElement(By.PartialLinkText("Cosacs.Booking.Cancel")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 4 (Cosacs.Booking.Cancel)");
        }

        public static void OpenCosacsServiceSummary(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Cosacs.Service.Summary"));
            webDriver.FindElement(By.PartialLinkText("Cosacs.Service.Summary")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 5 (Cosacs.Service.Summary)");
        }

        public static void OpenServiceRequestSubmit(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Service.Request.Submit"));
            webDriver.FindElement(By.PartialLinkText("Service.Request.Submit")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 6 (Service.Request.Submit)");
        }

        public static void OpenCosacsServicePayment(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Cosacs.Service.Payment"));
            webDriver.FindElement(By.PartialLinkText("Cosacs.Service.Payment")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 7 (Cosacs.Service.Payment)");
        }

        public static void OpenCosacsServiceCharges(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Cosacs.Service.Charges"));
            webDriver.FindElement(By.PartialLinkText("Cosacs.Service.Charges")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 8 (Cosacs.Service.Charges)");
        }

        public static void OpenCosacsServiceParts(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Cosacs.Service.Parts"));
            webDriver.FindElement(By.PartialLinkText("Cosacs.Service.Parts")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 9 (Cosacs.Service.Parts)");
        }

        public static void OpenWarrantySaleSubmit(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Warranty.Sale.Submit"));
            webDriver.FindElement(By.PartialLinkText("Warranty.Sale.Submit")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 10 (Warranty.Sale.Submit)");
        }

        public static void OpenCosacsServiceWarrantyServiceCompleted(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Cosacs.Service.WarrantyServiceCompleted"));
            webDriver.FindElement(By.PartialLinkText("Cosacs.Service.WarrantyServiceCompleted")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 11 (Cosacs.Service.WarrantyServiceCompleted)");
        }

        public static void OpenWarrantySaleCancel(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Warranty.Sale.Cancel"));
            webDriver.FindElement(By.PartialLinkText("Warranty.Sale.Cancel")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 12 (Warranty.Sale.Cancel)");
        }

        public static void OpenWarrantyPotentialCancel(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Warranty.Potential.Cancel"));
            webDriver.FindElement(By.PartialLinkText("Warranty.Potential.Cancel")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 13 (Warranty.Potential.Cancel)");
        }

        public static void OpenWarrantySaleCancelItem(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.PartialLinkText("Warranty.Sale.CancelItem"));
            webDriver.FindElement(By.PartialLinkText("Warranty.Sale.CancelItem")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Messages for Queue 14 (Warranty.Sale.CancelItem)");
        }

        public static void CheckPoisonMessagesColumnHeaders(this IWebDriver webDriver)
        {
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(1) th:nth-child(2)"), "Msg ID");
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(1) th:nth-child(3)"), "Created On");
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(1) th:nth-child(4)"), "Correlation ID");
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(1) th:nth-child(5)"), "Content");
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(1) th:nth-child(6)"), "Exception");
        }

        public static void CheckPendingMessagesColumnHeaders(this IWebDriver webDriver)
        {
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(4) th:nth-child(1)"), "Msg ID");
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(4) th:nth-child(2)"), "Created On");
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(4) th:nth-child(3)"), "Correlation ID");
            webDriver.IsTextPresent(By.CssSelector(".table:nth-child(4) th:nth-child(4)"), "Content");
        }
    }
}
