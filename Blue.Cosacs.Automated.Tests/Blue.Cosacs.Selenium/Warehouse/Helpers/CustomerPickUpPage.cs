using System.Diagnostics;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using OpenQA.Selenium.Interactions;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class CustomerPickUpPage
    {
        #region Search functionality is currently hidden
        /*public static void SearchBookingInCustomerPickUpPage(this IWebDriver webDriver, string searchTerm)
        {
            webDriver.FindElement(By.Id("searchBox")).Clear();
            webDriver.FindElement(By.Id("searchBox")).SendKeys(searchTerm);
            Sleep(1000);
        }*/
        #endregion

        public static string GetBookingNoInCustomerPickUpPage(this IWebDriver webDriver)
        {
            var bookingNo = webDriver.FindElement(By.CssSelector(".printPickUp:first-child")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.XPath("..")).FindElement(By.CssSelector(".Booking-Id")).Text;
            var hash = new char[] { '#' };
            bookingNo = bookingNo.TrimStart(hash);
            return bookingNo;
        }

        public static void PrintCustomerPickUp(this IWebDriver webDriver, string bookingNo, out string delOrCol)
        {
            delOrCol = webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .DelColDesc")).Text;
            webDriver.ScrollElementInToView(By.Id("printPickUp#" + bookingNo));
            webDriver.FindElement(By.Id("printPickUp#" + bookingNo)).Click();
            if (delOrCol == "Collection")
            {
                webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "A new return note has been created and is ready to be printed.");
                webDriver.OpenLinkInNewWindow(By.LinkText("new return note"));
            }
            else if (delOrCol == "Delivery")
            {
                webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "A new pick up note has been created and is ready to be printed.");
                webDriver.OpenLinkInNewWindow(By.LinkText("new pick up note"));
            }
            webDriver.SwitchToPopupWindow(By.CssSelector("div.container_12"), webDriver.CurrentWindowHandle);
            webDriver.IsElementPresent(By.CssSelector("div.container_12"));
            webDriver.ClosePopupWindow();
            webDriver.ClickCssSelector(".ok");
        }

        public static void OpenRePrintInNewWindow(this IWebDriver webDriver, string bookingNo, string delOrCol)
        {
            webDriver.ScrollElementInToView(By.CssSelector("[data-id='" + bookingNo + "'] .reprintButton"));
            webDriver.ClickCssSelector("[data-id='" + bookingNo + "'] .reprintButton");
            if (delOrCol == "Collection")
            {
                webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "You have chosen to reprint the Return Note. If you continue, this will be audited. Would you like to reprint the Return Note?");
                webDriver.OpenLinkInNewWindow(By.CssSelector(".ok"));
            }
            else if (delOrCol == "Delivery")
            {
                webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "You have chosen to reprint the Pick Up Note. If you continue, this will be audited. Would you like to reprint the Pick Up Note?");
                webDriver.OpenLinkInNewWindow(By.CssSelector(".ok"));
            }
        }

        public static void SelectRejectionReasonForCustomerPickUp(this IWebDriver webDriver, string bookingNo, string delOrCol)
        {
            if (delOrCol == "Collection")
            {
                webDriver.SelectFromDropDown("[data-id='" + bookingNo + "'] [name='Items.RejectionReason']", "Return Rejected");
            }
            else if (delOrCol == "Delivery")
            {
                webDriver.SelectFromDropDown("[data-id='" + bookingNo + "'] [name='Items.RejectionReason']", "PickUp Rejected");
            }
        }

        public static void ConfirmCustomerPickUp(this IWebDriver webDriver, string bookingNo, string delOrCol)
        {
            webDriver.IsElementPresent(By.Id("confirmPickUp#" + bookingNo));
            webDriver.ScrollElementInToView(By.Id("confirmPickUp#" + bookingNo));
            webDriver.FindElement(By.Id("confirmPickUp#" + bookingNo)).Click();
            if (delOrCol == "Collection")
            {
                webDriver.CheckNotification("×\r\nCustomer return for shipment #" + bookingNo + " confirmed successfully");
            }
            else if (delOrCol == "Delivery")
            {
                webDriver.CheckNotification("×\r\nCustomer pick up for shipment #" + bookingNo + " confirmed successfully");
            }
            webDriver.CloseNotification();
        }

        public static void RejectCustomerPickUp(this IWebDriver webDriver, string bookingNo, string delOrCol)
        {
            webDriver.IsElementPresent(By.Id("confirmPickUp#" + bookingNo));
            webDriver.ScrollElementInToView(By.Id("confirmPickUp#" + bookingNo));
            webDriver.FindElement(By.Id("confirmPickUp#" + bookingNo)).Click();
            if (delOrCol == "Collection")
            {
                webDriver.CheckNotification("×\r\nCustomer return for shipment #" + bookingNo + " rejected successfully");
            }
            else if (delOrCol == "Delivery")
            {
                webDriver.CheckNotification("×\r\nCustomer pick up for shipment #" + bookingNo + " rejected successfully");
            }
            webDriver.CloseNotification();
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
