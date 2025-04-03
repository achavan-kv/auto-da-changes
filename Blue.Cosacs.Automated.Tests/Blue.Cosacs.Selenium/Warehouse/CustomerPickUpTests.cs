using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Branch = System.Configuration.ConfigurationManager;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class CustomerPickUpTests
    {
        [Test]
        public void CustomerPickUpE2E()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch10"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.IsElementPresent(By.CssSelector("[data-id='" + bookingNo + "'] .rejectItem"));
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                if (delOrCol == "Delivery")
                    webDriver.CheckBookingStatus(bookingNo, Branch.AppSettings["Branch10"].Substring(0, 3), "Delivered");
                else if (delOrCol == "Collection")
                    webDriver.CheckBookingStatus(bookingNo, Branch.AppSettings["Branch10"].Substring(0, 3), "Collected");
            }
        }

        [Test]
        public void CustomerPickUpStatusTest()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch14"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNo, Branch.AppSettings["Branch14"].Substring(0, 3), "Booked");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch14"]);
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNo, Branch.AppSettings["Branch14"].Substring(0, 3), "Printed");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch14"]);
                webDriver.IsElementPresent(By.CssSelector("[data-id='" + bookingNo + "'] .rejectItem"));
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                if (delOrCol == "Delivery")
                    webDriver.CheckBookingStatus(bookingNo, Branch.AppSettings["Branch14"].Substring(0, 3), "Delivered");
                else if (delOrCol == "Collection")
                    webDriver.CheckBookingStatus(bookingNo, Branch.AppSettings["Branch14"].Substring(0, 3), "Collected");
            }
        }

        [Test]
        public void RejectCustomerPickUp()
        {
            using (var session =Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch11"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.IsElementPresent(By.CssSelector("[data-id='" + bookingNo + "'] .rejectItem"));
                webDriver.SelectRejectionReasonForCustomerPickUp(bookingNo, delOrCol);
                webDriver.RejectCustomerPickUp(bookingNo, delOrCol);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNo, Branch.AppSettings["Branch11"].Substring(0, 3), "Closed");
            }
        }

        [Test]
        public void ReprintCustomerPickUp()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch1"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.IsElementPresent(By.CssSelector("[data-id='" + bookingNo + "'] .reprintButton"));
                webDriver.OpenRePrintInNewWindow(bookingNo, delOrCol);
                webDriver.SwitchToPopupWindow(By.CssSelector("div.container_12"), webDriver.CurrentWindowHandle);
                webDriver.IsElementPresent(By.CssSelector("div.container_12"));
                webDriver.ClosePopupWindow();
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
            }
        }

        [Test]
        public void CheckCancelledBookingIsNotShownInCustomerPickUpScreen()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch3"]);
                var bookingNo = webDriver.FindElement(By.CssSelector(".panel:first-child .Booking-Id")).Text;
                bookingNo = bookingNo.TrimStart('#');
                webDriver.IsElementPresent(By.CssSelector("[data-id='" + bookingNo + "']"));
                webDriver.GoTo(By.CssSelector("[data-id='" + bookingNo + "'] .Booking-Id"), "Warehouse/Bookings/detail/" + bookingNo, session);
                webDriver.WaitForTextPresent(By.CssSelector("#page-heading"), "Shipment #" + bookingNo);
                webDriver.CancelBooking();
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch3"]);
                webDriver.IsElementNotPresent(By.CssSelector("[data-id='" + bookingNo + "']"));
            }
        }
    }
}
