using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Branch = System.Configuration.ConfigurationManager;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class SchedulingTests
    {
        [Test]
        public void AreAllDropDownsLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.IsDeliveryLocationDropDownLoaded();
                webDriver.IsTrucksDropDownLoaded();
            }
        }

        //[Test]
        public void ReOrderBookingsOnTruck()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch6"]);
                webDriver.WaitForElementPresent(By.ClassName("booking"));
                var firstBookingNo = webDriver.FindElement(By.CssSelector(".booking:first-child .Booking-Id")).Text;
                firstBookingNo = firstBookingNo.TrimStart('#');
                var secondBookingNO = webDriver.FindElement(By.CssSelector(".booking:nth-child(2) .Booking-Id")).Text;
                secondBookingNO = secondBookingNO.TrimStart('#');
                var lastBookingNo = webDriver.FindElement(By.CssSelector(".booking:last-child .Booking-Id")).Text;
                lastBookingNo = lastBookingNo.TrimStart('#');
                webDriver.ClickCreateDeliverySchedule();
                Assert.IsTrue(webDriver.FindElement(By.ClassName("tooltip-inner")).Displayed);
                Assert.IsTrue(webDriver.FindElement(By.Id("saveDeliverySchedule")).Displayed);
                Assert.IsTrue(webDriver.FindElement(By.Id("cancel")).Displayed);
                webDriver.IsElementPresent(By.ClassName("highlight-table"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsTextPresent(By.CssSelector(".booking:first-child .Booking-Id"), "#" + firstBookingNo);
                webDriver.IsTextPresent(By.CssSelector(".booking[data-id='" + firstBookingNo + "'] .order"), "1");
                webDriver.IsTextPresent(By.CssSelector(".booking:nth-child(2) .Booking-Id"), "#" + secondBookingNO);
                webDriver.IsTextPresent(By.CssSelector(".booking[data-id='" + secondBookingNO + "'] .order"), "2");
                webDriver.DragAndDrop(By.CssSelector(".booking[data-id='" + firstBookingNo + "']"), By.CssSelector(".booking[data-id='" + secondBookingNO + "']"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsTextPresent(By.CssSelector(".booking:first-child .Booking-Id"), "#" + secondBookingNO);
                webDriver.IsTextPresent(By.CssSelector(".booking[data-id='" + firstBookingNo + "'] .order"), "2");
                webDriver.IsTextPresent(By.CssSelector(".booking:nth-child(2) .Booking-Id"), "#" + firstBookingNo);
                webDriver.IsTextPresent(By.CssSelector(".booking[data-id='" + secondBookingNO + "'] .order"), "1");
                webDriver.Navigate().Refresh();
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch6"]);
                webDriver.WaitForElementPresent(By.ClassName("booking"));
                webDriver.ClickCreateDeliverySchedule();
                webDriver.ScrollToEndOfPage();
                webDriver.IsTextPresent(By.CssSelector(".booking:first-child .Booking-Id"), "#" + firstBookingNo);
                webDriver.IsTextPresent(By.CssSelector(".booking[data-id='" + firstBookingNo + "'] .order"), "1");
                webDriver.IsTextPresent(By.CssSelector(".booking:nth-child(2) > td.Booking-Columnn"), "#" + secondBookingNO);
                webDriver.IsTextPresent(By.CssSelector(".booking[data-id='" + secondBookingNO + "'] .order"), "2");
                webDriver.SelectLoadRejectionReason("Load Rejected", firstBookingNo);
                webDriver.IsTextPresent(By.CssSelector(".booking[data-id='" + firstBookingNo + "'] > .order"), "X");
                webDriver.IsTextPresent(By.CssSelector(".booking[data-id='" + secondBookingNO + "'] > .order"), "1");
            }
        }
    }
}
