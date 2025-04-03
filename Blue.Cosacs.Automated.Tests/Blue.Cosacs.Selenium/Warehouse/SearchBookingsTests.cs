using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class SearchBookingsTests
    {
        [Test]
        public void IsSearchBarLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Search Shipments");
                webDriver.IsSearchBarPresent();
            }
        }

        [Test]
        public void IsClearbuttonLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
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
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.AreSearchFiltersInSearchBookingsPageLoaded();
            }
        }

        [Test]
        public void AreBookingsLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
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
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSearchFilterNamesInSearchBookingsPage();
            }
        }
    }
}
