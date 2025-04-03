using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using System;
using System.Text.RegularExpressions;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class SearchDeliverySchedulesTests
    {
        [Test]
        public void IsSearchBarLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.IsSearchBarPresent();
            }
        }

        [Test]
        public void IsClearbuttonLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.IsClearButtonPresent();
            }
        }

        [Test]
        public void AreSearchFiltersLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.AreSearchFiltersLoadedInSearchDeliverySchedulesPage();
            }
        }

        [Test]
        public void AreDeliverySchedulesLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.IsElementPresent(By.ClassName("result"));
            }
        }

        [Test]
        public void CheckSearchFilterNames()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSearchFilterNamesInSearchDeliverySchedulesPage();
            }
        }

        [Test]
        public void DifferentDeliveryAndNotificationDatesTest()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                var deliveryScheduleNo = string.Empty;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                var pickListNo = webDriver.FindElement(By.CssSelector("div.pick-list.search-result:nth-child(3)")).GetAttribute("data-id");
                var deliveryLocation = webDriver.GetText(By.CssSelector("[data-id='" + pickListNo + "'] .branch"));
                deliveryLocation = Regex.Match(deliveryLocation, @"\d+").Value;
                var selectedTruckName = webDriver.TruckName(pickListNo);
                webDriver.ConfirmPickList(pickListNo, deliveryLocation, "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, out deliveryScheduleNo);
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchDeliverySchedule(deliveryScheduleNo, deliveryLocation, "Scheduled");
                webDriver.OpenDeliverySchedule(deliveryScheduleNo);
                var bookingNo = webDriver.GetFirstBookingNoFromDeliveryList();
                webDriver.ScrollToEndOfPage();
                webDriver.FindElement(By.Id("DeliveryOn")).Click();
                webDriver.SelectYesterdayFromDatePicker();
                webDriver.SaveDeliveryListConfirmation();
                webDriver.FindElement(By.LinkText("#" + bookingNo)).Click();
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookingNo);
                webDriver.OpenBookingTracking(bookingNo);
                var notificationDate = DateTime.Now.ToString("dddd dd-MMMM-yyy");
                var deliveredDate = (DateTime.Today.AddDays(-1)).ToString("dddd dd-MMMM-yyy");
                System.Threading.Thread.Sleep(1000);
                webDriver.CheckIfDeliveryAndNotificationDatesAreDifferent(notificationDate, deliveredDate);
            }
        }

        [Test]
        public void BookingsCannotBeDeliveredInFutureTest()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                var deliveryScheduleNo = string.Empty;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                var pickListNo = webDriver.FindElement(By.CssSelector("div.pick-list.search-result:nth-child(4)")).GetAttribute("data-id");
                var deliveryLocation = webDriver.GetText(By.CssSelector("[data-id='" + pickListNo + "'] .branch"));
                deliveryLocation = Regex.Match(deliveryLocation, @"\d+").Value;
                var selectedTruckName = webDriver.TruckName(pickListNo);
                webDriver.ConfirmPickList(pickListNo, deliveryLocation, "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, out deliveryScheduleNo);
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchDeliverySchedule(deliveryScheduleNo, deliveryLocation, "Scheduled");
                webDriver.OpenDeliverySchedule(deliveryScheduleNo);
                int today = (int)DateTime.Now.Day;
                var tomorrow = (DateTime.Today.AddDays(+1)).ToString("dd").TrimStart('0');
                webDriver.ScrollToEndOfPage();
                webDriver.FindElement(By.Id("DeliveryOn")).Click();
                Assert.IsFalse(webDriver.ElementPresent(By.LinkText(tomorrow)));
                Assert.IsTrue(webDriver.FindElement(By.LinkText(today.ToString())).Enabled);
                var readOnly = webDriver.FindElement(By.Id("DeliveryOn")).GetAttribute("readonly");
                Assert.IsTrue(readOnly == "true");
                //webDriver.FindElement(By.Id("DeliveryOn")).SendKeys((DateTime.Today.AddDays(+1)).ToString("dddd, d MMMM, yyy"));
            }
        }
    }
}
