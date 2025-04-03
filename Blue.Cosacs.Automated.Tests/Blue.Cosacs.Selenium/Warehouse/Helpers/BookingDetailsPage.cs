using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class BookingDetailsPage
    {
        public static void CheckOriginalBookingQuantity(this IWebDriver webDriver, string originalQuantity)
        {
            Assert.AreEqual(originalQuantity, webDriver.FindElement(By.CssSelector(".itemQuantity")).Text);
        }

        public static void CheckBookingStatusInBookingDetails(this IWebDriver webDriver, string lastStatus, string bookingNo)
        {
            Assert.AreEqual(lastStatus, webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .status")).Text);
        }

        public static void CheckCurrentQuantityOfBooking(this IWebDriver webDriver, string currentQuantity, string bookingNo)
        {
            Assert.AreEqual(currentQuantity, webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .qty")).Text);
        }

        public static void IsAmendDetailsLinkPresent(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.ClassName("showAmend"));
        }

        public static void IsResolveExceptionLinkPresent(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.ClassName("showException"));
        }

        public static string GetBookingQuantityinBookingDetails(this IWebDriver webDriver, string bookingNo)
        {
            var newBookingQuantity = webDriver.FindElement(By.CssSelector("tr[data-id='" + bookingNo + "'] > td.qty")).Text.ToString();
            return newBookingQuantity;
        }
                
        public static void SelectAM(this IWebDriver webDriver)
        {
            new SelectElement(webDriver.FindElement(By.CssSelector("select"))).SelectByText("AM");
        }

        public static void SelectPM(this IWebDriver webDriver)
        {
            new SelectElement(webDriver.FindElement(By.CssSelector("select"))).SelectByText("PM");
        }

        public static void ResolveException(this IWebDriver webDriver)
        {
            webDriver.ScrollElementInToView(By.ClassName("showException"));
            webDriver.FindElement(By.ClassName("showException")).Click();
            Sleep(1000);
            webDriver.ScrollElementInToView(By.CssSelector(".resolveDate.hasDatepicker"));
            Assert.IsFalse(webDriver.FindElement(By.CssSelector("button.btnResolve")).Enabled);
            Assert.IsFalse(webDriver.FindElement(By.CssSelector("button.btnCancel")).Enabled);
            webDriver.IsElementPresent(By.CssSelector(".resolveDate.hasDatepicker"));
            webDriver.FindElement(By.CssSelector(".resolveDate.hasDatepicker")).Click();
            Sleep(1000);
            webDriver.SelectTomorrowFromDatePicker();
            webDriver.SelectPM();
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button.btnResolve")).Enabled);
            webDriver.FindElement(By.CssSelector("button.btnResolve")).Click();
            Sleep(2000);
        }

        public static void CancelException(this IWebDriver webDriver)
        {
            webDriver.ScrollElementInToView(By.ClassName("showException"));
            webDriver.FindElement(By.ClassName("showException")).Click();
            Sleep(1000);
            webDriver.ScrollElementInToView(By.CssSelector(".cancelNotes"));
            Assert.IsFalse(webDriver.FindElement(By.CssSelector("button.btnResolve")).Enabled);
            Assert.IsFalse(webDriver.FindElement(By.CssSelector("button.btnCancel")).Enabled);
            webDriver.FindElement(By.ClassName("cancelNotes")).SendKeys("This booking is cancelled by Selenium");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button.btnCancel")).Enabled);
            webDriver.FindElement(By.CssSelector("button.btnCancel")).Click();
            Sleep(1000);
        }

        public static void CancelBooking(this IWebDriver webDriver)
        {
            webDriver.ScrollElementInToView(By.ClassName("showAmend"));
            webDriver.FindElement(By.ClassName("showAmend")).Click();
            Sleep(1000);
            webDriver.ScrollElementInToView(By.CssSelector(".cancelNotes"));
            Assert.IsFalse(webDriver.FindElement(By.CssSelector("button.btnResolve")).Enabled);
            Assert.IsFalse(webDriver.FindElement(By.CssSelector("button.btnCancel")).Enabled);
            webDriver.FindElement(By.ClassName("cancelNotes")).SendKeys("This booking is cancelled by Selenium");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button.btnCancel")).Enabled);
            webDriver.FindElement(By.CssSelector("button.btnCancel")).Click();
            Sleep(1000);
        }

        public static void OpenBookingTracking(this IWebDriver webDriver, string bookingNo)
        {
            webDriver.ScrollToEndOfPage();
            webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .expandable.itemDetail")).Click();
            Sleep(1000);
        }

        public static void CheckIfDeliveryAndNotificationDatesAreDifferent(this IWebDriver webDriver, string notificationDate, string deliveredDate)
        {
            webDriver.ScrollToEndOfPage();
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:last-child :first-child"), "Delivery Confirmed");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:last-child :nth-child(2)")).Text.Contains(notificationDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:last-child :nth-child(4)")).Text.Contains(deliveredDate));
        }

        public static string GetDeliveryBranchOfBooking(this IWebDriver webDriver)
        {
            var deliveryBranch = webDriver.FindElement(By.ClassName("delbranch")).Text;
            return deliveryBranch;
        }

        public static void CheckBookingHistory(this IWebDriver webDriver, string bookedDate, string dueDate, string pickListAssignedDate, string pickListAssignedBy, string truckName, 
                                                string pickListCreatedDate, string pickListCreatedBy, string pickedBy, string checkedBy, string pickListConfirmedDate, 
                                                string pickListConfirmedBy, string scheduleCreatedDate, string scheduleCreatedBy, string driver, string notificationDate, 
                                                string deliveryConfirmedBy, string deliveredDate, string quantity)
        {
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:first-child td:first-child"), "Booked");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:first-child td:nth-child(2)")).Text.Contains(bookedDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:first-child td:nth-child(4)")).Text.Contains("Due on " + dueDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:first-child td:nth-child(5)")).Text.Contains(quantity));
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:nth-child(2) td:nth-child(1)"), "Pick List Assigned");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(2) td:nth-child(2)")).Text.Contains(pickListAssignedDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(2) td:nth-child(3)")).Text.Contains(pickListAssignedBy));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(2) td:nth-child(4)")).Text.Contains(truckName));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(2) td:nth-child(5)")).Text.Contains(quantity));
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:nth-child(3) td:nth-child(1)"), "PickList Created");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(3) td:nth-child(2)")).Text.Contains(pickListCreatedDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(3) td:nth-child(3)")).Text.Contains(pickListCreatedBy));
            Assert.IsTrue(webDriver.ElementPresent(By.LinkText("Show PickList")));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(3) td:nth-child(5)")).Text.Contains(quantity));
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:nth-child(4) td:nth-child(1)"), "Picked by");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(4) td:nth-child(3)")).Text.Contains(pickedBy));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(4) td:nth-child(5)")).Text.Contains(quantity));
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:nth-child(5) td:nth-child(1)"), "Picked Item Checked");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(5) td:nth-child(3)")).Text.Contains(checkedBy));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(5) td:nth-child(5)")).Text.Contains(quantity));
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:nth-child(6) td:nth-child(1)"), "PickList Confirmed");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(6) td:nth-child(2)")).Text.Contains(pickListConfirmedDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(6) td:nth-child(3)")).Text.Contains(pickListConfirmedBy));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(6) td:nth-child(5)")).Text.Contains(quantity));
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:nth-child(7) td:nth-child(1)"), "Schedule Created");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(7) td:nth-child(2)")).Text.Contains(scheduleCreatedDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(7) td:nth-child(3)")).Text.Contains(scheduleCreatedBy));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(7) td:nth-child(4)")).Text.Contains(driver));
            Assert.IsTrue(webDriver.ElementPresent(By.LinkText("Show Schedule")));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:nth-child(7) td:nth-child(5)")).Text.Contains(quantity));
            webDriver.IsTextPresent(By.CssSelector(".bookingsHistory tbody tr:last-child td:nth-child(1)"), "Delivery Confirmed");
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:last-child td:nth-child(2)")).Text.Contains(notificationDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:last-child td:nth-child(3)")).Text.Contains(deliveryConfirmedBy));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:last-child td:nth-child(4)")).Text.Contains(deliveredDate));
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".bookingsHistory tbody tr:last-child td:nth-child(5)")).Text.Contains(quantity));
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}

