using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class SearchPickListsTests
    {
        [Test]
        public void IsSearchBarLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
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
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
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
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.AreSearchFiltersInSearchPickListsPageLoaded();
            }
        }

        [Test]
        public void ArePickListsLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
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
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSearchFilterNamesInSearchPickListsPage();
            }
        }

        [Test]
        public void CheckConfirmedPickListsCannotbeChanged()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Confirmed");
                webDriver.ApplyFacetFilters("PickingEmployees", "Selenium Tester2");
                webDriver.FindElement(By.CssSelector(".search-result:first-child")).Click();
                System.Threading.Thread.Sleep(1000);
                Assert.IsFalse(webDriver.ElementPresent(By.CssSelector("div.view.pickedBy.chosenwidth.checkPick > div > a > div > b")));
                Assert.IsFalse(webDriver.ElementPresent(By.CssSelector("div.view.checkedBy.chosenwidth.checkConfirm > div > a > div > b")));
                Assert.IsFalse(webDriver.FindElement(By.Id("pickedOn")).Enabled);
                Assert.IsFalse(webDriver.FindElement(By.Id("save")).Displayed);
            }
        }

        [Test]
        public void ReprintPendingPickList()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                webDriver.FindElement(By.CssSelector(".search-result:first-child")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.RePrintPickList();
                webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
                webDriver.ClosePopupWindow();
            }
        }

        [Test]
        public void ReprintConfirmedPickList()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Confirmed");
                webDriver.ApplyFacetFilters("PickingEmployees", "System Administrator");
             //   webDriver.ApplyFacetFilters("PickingEmployees", "Selenium Technician External");
                webDriver.FindElement(By.CssSelector(".search-result:first-child")).Click();
                System.Threading.Thread.Sleep(1000);
                Assert.IsFalse(webDriver.FindElement(By.Id("save")).Displayed);
                webDriver.RePrintPickList();
                webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
                webDriver.ClosePopupWindow();
            }
        }
    }
}
