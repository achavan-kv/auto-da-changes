using System.Diagnostics;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using System;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class SearchDeliverySchedulesPage
    {
        public static void SearchDeliverySchedule(this IWebDriver webDriver, string delScheduleNo, string delBranch, string status)
        {
            webDriver.FindElement(By.ClassName("text-search")).Clear();
            Sleep(1000);
            webDriver.FindElement(By.ClassName("text-search")).SendKeys(delScheduleNo);
            webDriver.ApplyFacetFilters("DeliveryBranchName", delBranch);
            webDriver.ApplyFacetFilters("DeliveryStatus", status);
            Sleep(500);
            webDriver.WaitForElementPresent(By.CssSelector("div[data-id = '" + delScheduleNo + "']"));
        }

        public static void OpenDeliverySchedule(this IWebDriver webDriver, string delScheduleNo)
        {
            webDriver.FindElement(By.CssSelector("div[data-id = '" + delScheduleNo + "']")).Click();
            webDriver.WaitForElementPresent(By.CssSelector("div.rejectItem.max"));
        }

        public static string GetBookingNoFromDeliveryList(this IWebDriver webDriver, string originalBookingNo)
        {
            var bookingNo = webDriver.FindElement(By.CssSelector("td.Booking-Columnn > a[href*='" + originalBookingNo + "']")).Text.ToString();
            var hash = new char[]{'#'};
            bookingNo = bookingNo.TrimStart(hash);
            return bookingNo;
        }

        public static string GetFirstBookingNoFromDeliveryList(this IWebDriver webDriver)
        {
            var bookingNo = webDriver.FindElement(By.CssSelector("td.Booking-Columnn")).Text.ToString();
            var hash = new char[] { '#' };
            bookingNo = bookingNo.TrimStart(hash);
            return bookingNo;
        }

        public static void CheckBookingNoInDeliverySchedule(this IWebDriver webDriver, string bookingNo)
        {
            Assert.AreEqual(bookingNo, webDriver.GetBookingNoFromDeliveryList(bookingNo));
        }

        public static string GetItemQuantityFromDeliveryList(this IWebDriver webDriver, string bookingNo)
        {
            var quantity = webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .itemQuantity")).Text.ToString();
            return quantity;
        }

        public static string GetDeliveredItemQuantity(this IWebDriver webDriver, string bookingNo)
        {
            var quantity = webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .itemQuantity")).Text.ToString();
            return quantity;
        }

        public static void CheckBookingQuantityIsSameInDeliverySchdule(this IWebDriver webDriver, string originalQuantity, string bookingNo)
        {
            var quantityFromDeliveryList = webDriver.GetItemQuantityFromDeliveryList(bookingNo);
            Assert.AreEqual(originalQuantity, quantityFromDeliveryList);
        }

        public static void CheckDeliveredQuantityIsSame(this IWebDriver webDriver, string originalQuantity, string bookingNo)
        {
            var deliveredItemQuantity = webDriver.GetDeliveredItemQuantity(bookingNo);
            Assert.AreEqual(originalQuantity, deliveredItemQuantity);
        }

        public static void SaveDeliveryListConfirmation(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector("button#save")).Click();
            Sleep(1000);
        }

        public static void AreSearchFiltersLoadedInSearchDeliverySchedulesPage(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'DeliveryBranchName']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'Truck']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'Driver']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'DeliveryEmployees']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'DeliveryStatus']")).Displayed);
        }

        public static void CheckSearchFilterNamesInSearchDeliverySchedulesPage(this IWebDriver webDriver)
        {
            Assert.AreEqual("Delivery Branch", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(1) > div.section")).Text);
            Assert.AreEqual("Truck", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(2) > div.section")).Text);
            Assert.AreEqual("Driver", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(3) > div.section")).Text);
            Assert.AreEqual("Employees", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(4) > div.section")).Text);
            Assert.AreEqual("Status", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(5) > div.section")).Text);
        }
        
        public static void CheckDeliveryScheduleColumnHeaders(this IWebDriver webDriver)
        {
            Assert.AreEqual("Created", webDriver.FindElement(By.CssSelector(".created.title.cell")).Text);
            Assert.AreEqual("Truck", webDriver.FindElement(By.CssSelector(".truck.title.cell")).Text);
            Assert.AreEqual("Driver", webDriver.FindElement(By.CssSelector(".driver.title.cell")).Text);
            Assert.AreEqual("Items", webDriver.FindElement(By.CssSelector(".items.title.cell")).Text);
            Assert.AreEqual("Status", webDriver.FindElement(By.CssSelector(".title.cell:nth-child(5)")).Text);
        }

        public static void CheckBookingCannotBeDeliveredBeforeScheduledDate(this IWebDriver webDriver)
        {
            int today = (int)DateTime.Now.Day;
            var yesterday = (today - 1).ToString();
            webDriver.ScrollToEndOfPage();
            webDriver.FindElement(By.Id("DeliveryOn")).Click();
            Assert.IsFalse(webDriver.ElementPresent(By.LinkText(yesterday)));
            Assert.IsTrue(webDriver.FindElement(By.LinkText(today.ToString())).Enabled);
        }

        public static void DeliverBookingsInDeliverySchedule(this IWebDriver webDriver, string deliveryScheduleNo, string bookingNo, string originalQuantity, string delBranch, string status, string delOrCol)
        {
            webDriver.SearchDeliverySchedule(deliveryScheduleNo, delBranch, status);
            webDriver.FindElement(By.CssSelector("div.load.search-result:first-child")).Click();
            webDriver.WaitForElementPresent(By.CssSelector("div.rejectItem.max"));
            webDriver.CheckBookingNoInDeliverySchedule(bookingNo);
            webDriver.CheckBookingQuantityIsSameInDeliverySchdule(originalQuantity, bookingNo);
            webDriver.ScrollToEndOfPage();
            webDriver.IsElementPresent(By.CssSelector("button#save"));
            webDriver.IsElementPresent(By.CssSelector("button#reprint"));
            webDriver.CheckBookingCannotBeDeliveredBeforeScheduledDate();
            webDriver.SaveDeliveryListConfirmation();
            if (delOrCol == "Collection")
            {
                Assert.AreEqual("All Item(s) Collected Successfully", webDriver.FindElement(By.CssSelector("[data-id = '" + bookingNo + "'] td:nth-child(6)")).Text);
            }
            else if (delOrCol == "Delivery")
            {
                Assert.AreEqual("All Item(s) Delivered Successfully", webDriver.FindElement(By.CssSelector("[data-id = '" + bookingNo + "'] td:nth-child(6)")).Text);
            }
            webDriver.CheckDeliveredQuantityIsSame(originalQuantity, bookingNo);
        }

        public static void SelectDeliveryRejectionReason(this IWebDriver webDriver, string label, string originalBookingNo)
        {
            webDriver.SelectFromDropDown("[data-id='" + originalBookingNo + "'] .rejectItemSel", label);
        }

        public static void RejectBookingInDeliveryList(this IWebDriver webDriver, string deliveryScheduleNo, string bookingNo, string originalQuantity, string delBranch, string status)
        {
            webDriver.SearchDeliverySchedule(deliveryScheduleNo, delBranch, status);
            webDriver.OpenDeliverySchedule(deliveryScheduleNo);
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Delivery List #" + deliveryScheduleNo + " Confirmation");
            webDriver.CheckBookingNoInDeliverySchedule(bookingNo);
            webDriver.CheckBookingQuantityIsSameInDeliverySchdule(originalQuantity, bookingNo);
            webDriver.ScrollToEndOfPage();
            webDriver.IsElementPresent(By.CssSelector("button#save"));
            webDriver.IsElementPresent(By.CssSelector("button#reprint"));
            webDriver.IsElementPresent(By.Id("DeliveryOn"));
            webDriver.FindElement(By.Id("DeliveryOn")).Click();
            webDriver.SelectTodayFromDatePicker();
            webDriver.SelectDeliveryRejectionReason("Delivery Rejected", bookingNo);
            webDriver.SaveDeliveryListConfirmation();
            Assert.AreEqual("Delivery Rejected", webDriver.FindElement(By.CssSelector("[data-id = '" + bookingNo + "'] .reasonDisplay")).Text);
        }

        public static string GetBookedDate(this IWebDriver webDriver)
        {
            var bookedDate = Convert.ToDateTime(webDriver.FindElement(By.CssSelector(".product div:last-child")).Text.Replace("Booked ", "")).ToString("dddd dd-MMMM-yyyy");
            return bookedDate;
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}

