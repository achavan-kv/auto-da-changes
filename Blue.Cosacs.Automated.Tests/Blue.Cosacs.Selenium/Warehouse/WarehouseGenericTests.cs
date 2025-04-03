using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using System;
using System.Text;
using Branch = System.Configuration.ConfigurationManager;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class WarehouseGenericTests
    {
        [Test]
        public void DeliverBookingE2E()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var currentUserProfileId = webDriver.GetUserProfileId(session.Username);
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                var userFullName = webDriver.QueryDataBase("SELECT FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", "FullName");
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch13"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var deliveryScheduleNo = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.PickItemsAndConfirmPickList(pickListNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, bookingNo, originalQuantity, out deliveryScheduleNo, currentUser, delCol);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchBooking(bookingNo, deliveryLocation.Substring(0,3), "Scheduled");
                var dueDate = webDriver.GetDueDate(bookingNo);
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.DeliverBookingsInDeliverySchedule(deliveryScheduleNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Scheduled", delCol);
                var bookedDate = webDriver.GetBookedDate();
                var today = webDriver.Today();
                var splitTruckName = selectedTruckName.Split(' ');
                var truckName = splitTruckName[0];
                var driverName = truckName.Replace("Truck", "Driver");
                webDriver.FindElement(By.LinkText("#" + bookingNo)).Click();
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookingNo);
                webDriver.OpenBookingTracking(bookingNo);
                webDriver.CheckBookingHistory(bookedDate, dueDate, today, userFullName + " (" + currentUserProfileId + ")", truckName, today, userFullName + " (" + currentUserProfileId + ")",
                                            "Selenium Tester1 (221065)", "Selenium Tester1 (221065)", today, userFullName + " (" + currentUserProfileId + ")", today, userFullName + " (" + currentUserProfileId + ")", 
                                            driverName, today, userFullName + " (" + currentUserProfileId + ")", today, originalQuantity);
            }
        }

        [Test]
        public void CheckLegendsTest()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckLegends();
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.CheckLegends();
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckLegends();
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CheckLegends();
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckLegends();
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.CheckLegends();
            }
        }

        [Test]
        public void BookingStatusTest()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch5"]);
                var bookingNoOfBooked = webDriver.GetBookingNoOfFirstBookingInPickingPage();
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNoOfBooked, Branch.AppSettings["Branch5"].Substring(0, 3), "Booked");
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch5"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var deliveryScheduleNo = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNo, deliveryLocation.Substring(0,3), "Picking");
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.PickItemsAndConfirmPickList(pickListNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNo, deliveryLocation.Substring(0,3), "Picked");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, bookingNo, originalQuantity, out deliveryScheduleNo, currentUser, delCol);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNo, deliveryLocation.Substring(0,3), "Scheduled");
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.DeliverBookingsInDeliverySchedule(deliveryScheduleNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Scheduled", delCol);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNo, deliveryLocation.Substring(0,3), "Delivered");
            }
        }

        [Test]
        public void CheckBookingDetailsOfBookedBooking()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch16"]);
                var bookingNoOfBooked = webDriver.GetBookingNoOfFirstBookingInPickingPage();
                var originalQuantity = webDriver.GetOriginalItemQuantity(bookingNoOfBooked);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchBooking(bookingNoOfBooked, Branch.AppSettings["Branch16"].Substring(0, 3), "Booked");
                webDriver.CheckSearchResultInSearchBookingsPage(bookingNoOfBooked);
                webDriver.GoTo(By.ClassName("Booking-Id"), "Warehouse/Bookings/detail/" + bookingNoOfBooked, session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookingNoOfBooked);
                webDriver.CheckOriginalBookingQuantity(originalQuantity);
                webDriver.CheckCurrentQuantityOfBooking(originalQuantity, bookingNoOfBooked);
                webDriver.CheckBookingStatusInBookingDetails("Booked", bookingNoOfBooked);
                webDriver.IsAmendDetailsLinkPresent();
            }
        }

        [Test]
        public void RejectBookingWhilePickingAndResolveException()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch15"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var pickedQuantity = string.Empty;
                int oriqty;
                int picqty;
                var rejectedQuantity = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.RejectBookingInPickListConfirmationScreen(pickListNo, bookingNo, out pickedQuantity, originalQuantity, out oriqty, out picqty, out rejectedQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.FindElement(By.LinkText("#" + bookingNo)).Click();
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookingNo);
                webDriver.ResolveException();
            }
        }

        [Test]
        public void RejectBookingWhileCreatingDeliveryScheduleAndResolveException()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch6"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var deliveryScheduleNo = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.PickItemsAndConfirmPickList(pickListNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.RejectBookingInLoad(deliveryLocation, selectedTruckName, bookingNo, originalQuantity);
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckBookingStatus(bookingNo, deliveryLocation.Substring(0,3), "Closed");
                if (webDriver.ElementPresent(By.CssSelector("div.search-result:nth-child(2)")) == true)
                    webDriver.GoTo(By.CssSelector("div.search-result:nth-child(2) > div.Booking-Columnn > a.Booking-Id"), "Warehouse/Bookings/detail/" + bookingNo, session);
                else
                    webDriver.GoTo(By.ClassName("Booking-Id"), "Warehouse/Bookings/detail/" + bookingNo, session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookingNo);
                webDriver.IsResolveExceptionLinkPresent();
                webDriver.ResolveException();
            }
        }

        [Test]
        public void RejectBookingInDeliveryListAndResolveException()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch2"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var deliveryScheduleNo = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.PickItemsAndConfirmPickList(pickListNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, bookingNo, originalQuantity, out deliveryScheduleNo, currentUser, delCol);
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.RejectBookingInDeliveryList(deliveryScheduleNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Scheduled");
                webDriver.FindElement(By.LinkText("#" + bookingNo)).Click();
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookingNo);
                webDriver.IsResolveExceptionLinkPresent();
                webDriver.ResolveException();
            }
        }

        [Test]
        public void ForgotMyPassword()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                System.Threading.Thread.Sleep(1000);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.IsElementPresent(By.Id("aForgetPassword"));
                webDriver.RecoverPassword(session.Username);
                webDriver.WaitForElementPresent(By.Name("username"));
                var resetPasswordLink = string.Empty;
                webDriver.CheckResetPwdEmail(out resetPasswordLink, session);
                //webDriver.CheckResetPasswordEmail(out resetPasswordLink, session);
                webDriver.Navigate().GoToUrl(resetPasswordLink);
                webDriver.ResetPassword(session.Password);
            }
        }

        [Test]
        public void CancelException()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("BookingStatus", "Exception");
                var bookingNo = webDriver.GetBookingNoOfSearchResult();
                webDriver.GoTo(By.CssSelector("[data-booking-no='" + bookingNo + "'] .Booking-Id"), "Warehouse/Bookings/detail/" + bookingNo, session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookingNo);
                webDriver.CheckBookingStatusInBookingDetails("Exception", bookingNo);
                webDriver.IsResolveExceptionLinkPresent();
                webDriver.CancelException();
                webDriver.CheckBookingStatusInBookingDetails("Closed", bookingNo);
            }
        }

        [Test]
        public void CollectBookingE2E()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch4"];
                var originalQuantity = string.Empty;
                var selectedTruckName = string.Empty;
                var deliveryScheduleNo = string.Empty;
                var bookingNo = string.Empty;
                webDriver.AddCollectionToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, deliveryLocation);
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, bookingNo, originalQuantity, out deliveryScheduleNo, currentUser, "Collection");
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.DeliverBookingsInDeliverySchedule(deliveryScheduleNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Scheduled", "Collection");
            }
        }

        #region Handy test to confirm all picklists with Created status
        //[Test]
        public void HandyConfirmAllAvailablePickLists()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                bool isCreatedPresent = webDriver.ElementPresent(By.XPath("//ul[@data-field = 'PickingStatus']/li[contains(text(), 'Created')]"));
                do
                {
                    int retry = 0;
                    while (isCreatedPresent == true)
                    {
                        webDriver.ApplyFacetFilters("PickingStatus", "Created");
                        webDriver.FindElement(By.CssSelector(".search-result:first-child")).Click();
                        System.Threading.Thread.Sleep(1000);
                        webDriver.SelectPickedBy("Selenium Tester1 : 221031");
                        webDriver.SelectCheckedBy("Selenium Tester1 : 221031");
                        webDriver.FindElement(By.CssSelector(".booking ")).Click();
                        webDriver.ScrollToEndOfPage();
                        webDriver.SavePickList();
                        webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                        webDriver.WaitForElementPresent(By.ClassName("text-search"));
                    }
                    if (isCreatedPresent == false) break;
                    else retry++;
                }
                while (true);
            }
        }
        #endregion

        #region Handy tests to Resolve all Exceptions
        //[Test]
        public void HandyResolveAllAvailableExceptions()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                bool isExceptionPresent = webDriver.ElementPresent(By.CssSelector(".facet-field[data-field = 'Status'] > li[data-value='Exception']"));
                do
                {
                    int retry = 0;
                    while (isExceptionPresent == true)
                    {
                        webDriver.ApplyFacetFilters("BookingStatus", "Exception");
                        var bookingNo = webDriver.GetBookingNoOfSearchResult();
                        webDriver.FindElement(By.CssSelector("div.search-result:first-child > div.Booking-Columnn > a.Booking-Id")).Click();
                        webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookingNo);
                        webDriver.IsResolveExceptionLinkPresent();
                        webDriver.ResolveException();
                        webDriver.CheckBookingStatusInBookingDetails("Booked", bookingNo);
                        webDriver.IsAmendDetailsLinkPresent();
                        webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                        webDriver.WaitForElementPresent(By.ClassName("text-search"));
                    }
                    if (isExceptionPresent == false) break;
                    else retry++;
                }
                while (true);
            }
        }
        #endregion

        #region Handy test to confirm all Delivery Schedules with Scheduled status
        //[Test]
        public void HandyConfirmAllAvailableDeliverySchedules()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                bool isScheduledPresent = webDriver.ElementPresent(By.CssSelector(".facet-field[data-field = 'Status'] > li[data-value='Scheduled']"));
                do
                {
                    int retry = 0;
                    while (isScheduledPresent == true)
                    {
                        webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'Status'] > li[data-value='Scheduled']")).Click();
                        System.Threading.Thread.Sleep(1000);
                        webDriver.FindElement(By.CssSelector("div.load.search-result:first-child")).Click();
                        webDriver.WaitForElementPresent(By.CssSelector("div.rejectItem.max"));
                        webDriver.IsElementPresent(By.CssSelector("button#save"));
                        webDriver.SaveDeliveryListConfirmation();
                        Assert.AreEqual("All Item(s) Delivered Successfully", webDriver.FindElement(By.CssSelector("table#pickingItems > tbody > tr > td:nth-child(6)")).Text);
                        webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                        webDriver.WaitForElementPresent(By.ClassName("text-search"));
                    }
                    if (isScheduledPresent == false) break;
                    else retry++;
                }
                while (true);
            }
        }
        #endregion

        #region Handy test to confirm all available Customer PickUps
        //[Test]
        public void HandyConfirmCustomerPickUps()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch13"]);
                bool isConfirmPresent = webDriver.ElementPresent(By.CssSelector("#confirmPickUp"));
                do
                {
                    int retry = 0;
                    while (isConfirmPresent == true)
                    {
                        webDriver.IsElementPresent(By.CssSelector("#confirmPickUp"));
                        webDriver.FindElement(By.CssSelector("#confirmPickUp")).Click();
                        System.Threading.Thread.Sleep(2000);
                    }
                    if (isConfirmPresent == false) break;
                    else retry++;
                }
                while (true);
            }
        }
        #endregion
    }
}
