using System.Diagnostics;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using System;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class SearchBookingsPage
    {
        public static void AreSearchFiltersInSearchBookingsPageLoaded(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'StockBranchName']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'DeliveryBranchName']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'DelCol']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'DeliveryZone']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'Fascia']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'BookingStatus']")).Displayed);
        }

        public static void CheckSearchFilterNamesInSearchBookingsPage(this IWebDriver webDriver)
        {
            Assert.AreEqual("Delivery Branch", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(1) > div.section")).Text);
            Assert.AreEqual("Stock Branch", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(2) > div.section")).Text);
            Assert.AreEqual("Fascia", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(3) > div.section")).Text);
            Assert.AreEqual("Delivery/Collection", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(4) > div.section")).Text);
            Assert.AreEqual("Delivery Zone", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(5) > div.section")).Text);
            Assert.AreEqual("Status", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(6) > div.section")).Text);
        }

        public static void CheckBookingColumnHeadersInSearchBookingsPage(this IWebDriver webDriver)
        {
            Assert.AreEqual("Booking No", webDriver.FindElement(By.CssSelector("div.Booking-Columnn > div.row > div.title.cell")).Text);
            Assert.AreEqual("Ordered On", webDriver.FindElement(By.CssSelector("div.details-table > div.row > div.ordered.title.cell")).Text);
            Assert.AreEqual("Required Del/Col Date", webDriver.FindElement(By.CssSelector("div.details-table >div.row > div:nth-child(2)")).Text);
            Assert.AreEqual("Customer", webDriver.FindElement(By.CssSelector("div.details-table >div.row > div:nth-child(3)")).Text);
            Assert.AreEqual("Address", webDriver.FindElement(By.CssSelector("div.details-table >div.row > div:nth-child(4)")).Text);
            Assert.AreEqual("Item Details", webDriver.FindElement(By.CssSelector("div.details-table >div.row > div:nth-child(5)")).Text);
            Assert.AreEqual("Description", webDriver.FindElement(By.CssSelector("div.details-table >div.row > div:nth-child(6)")).Text);
            Assert.AreEqual("Quantity", webDriver.FindElement(By.CssSelector("div.details-table >div.row > div:nth-child(7)")).Text);
            Assert.AreEqual("Status", webDriver.FindElement(By.CssSelector("div.details-table >div.row > div:nth-child(8)")).Text);
        }

        public static void SearchBooking(this IWebDriver webDriver, string bookingNo, string delBranch, string status)
        {
            webDriver.FindElement(By.ClassName("text-search")).Clear();
            Sleep(1000);
            webDriver.FindElement(By.ClassName("text-search")).SendKeys(bookingNo);
            webDriver.ApplyFacetFilters("DeliveryBranchName", delBranch);
            webDriver.ApplyFacetFilters("BookingStatus", status);
            Sleep(500);
            webDriver.WaitForElementPresent(By.CssSelector("div[data-booking-no='" + bookingNo + "']"));
        }

        public static string GetBookingNoOfSearchResult(this IWebDriver webDriver)
        {
            var bookingNo = webDriver.FindElement(By.CssSelector(".Booking-Id")).Text.ToString();
            char[] hash = new char[] { '#' };
            bookingNo = bookingNo.TrimStart(hash);
            Debug.WriteLine(bookingNo);
            return bookingNo;
        }

        public static void CheckBookingStatus(this IWebDriver webDriver, string bookingNo, string delBranch, string status)
        {
            webDriver.SearchBooking(bookingNo, delBranch, status);
            webDriver.CheckSearchResultInSearchBookingsPage(bookingNo);
            Assert.AreEqual(status, webDriver.FindElement(By.CssSelector("[data-booking-no='" + bookingNo + "'] .panel-body .text-right")).Text);
        }

        public static void CheckSearchResultInSearchBookingsPage(this IWebDriver webDriver, string originalBookingNo)
        {
            Assert.AreEqual(originalBookingNo, webDriver.GetBookingNoOfSearchResult());
        }

        public static string GetDueDate(this IWebDriver webDriver, string bookingNo)
        {
            var day = webDriver.FindElement(By.CssSelector("[data-booking-no = '" + bookingNo + "'] .DeliveryCollection .weekday")).Text;
            var date = webDriver.FindElement(By.CssSelector("[data-booking-no = '" + bookingNo + "'] .DeliveryCollection .day")).Text;
            var month = webDriver.FindElement(By.CssSelector("[data-booking-no = '" + bookingNo + "'] .DeliveryCollection .month")).Text;
            var dueDate = Convert.ToDateTime(day + date + month).ToString("dddd dd-MMMM-yyyy");
            return dueDate;
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}

