using System.Threading;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Branch = System.Configuration.ConfigurationManager;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class PickingTests
    {
        [Test]
        public void AreAllDropDownsLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.IsDeliveryLocationDropDownLoaded();
                webDriver.IsPrintingGroupCriteriaDropDownLoaded();
                webDriver.IsTrucksDropDownLoaded();
            }
        }

        [Test]
        public void AreAllButtonsLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.IsClearButtonPresent();
                webDriver.IsSearchButtonLoaded();
                webDriver.IsPrintAllButtonLoaded();
                webDriver.IsPrintTruckButtonLoaded();
            }
        }

        [Test]
        public void AddBookingToTruckAndRemoveIt()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                var selectedDeliveryLocation = webDriver.FindElement(By.CssSelector("#deliveryBranch_chzn > .chzn-single > span")).Text;
                var bookingNo = webDriver.GetBookingNoOfFirstBookingInPickingPage();
                var originalQuantity = webDriver.GetOriginalItemQuantity(bookingNo);
                webDriver.AddFirstBookingToTruck();
                webDriver.IsElementPresent(By.CssSelector("#pickingItems .pickingItem:first-child .action-unpick"));
                webDriver.CheckIfBookingQuantityIsSame(originalQuantity);
                webDriver.RemoveFirstBookingFromTruck();
                webDriver.Navigate().Refresh();
                Thread.Sleep(2500);
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                var bookingNoAfterRemovedFromTruck = webDriver.GetBookingNoOfFirstBookingInPickingPage();
                Assert.AreEqual(bookingNoAfterRemovedFromTruck, bookingNo);
            }
        }

        [Test]
        public void PickingSearchTest()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                webDriver.SelectCourtsAsFascia();
                webDriver.SelectProductCategory("PCs & Notebooks");
                webDriver.FindElement(By.CssSelector("button.search")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.CssSelector("button.clear.btn")).Click();
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                webDriver.SelectNonCourtsAsFascia();
                webDriver.FindElement(By.CssSelector("button.search")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.CssSelector("button.clear.btn")).Click();
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                webDriver.SelectCourtsAsFascia();
                webDriver.SelectProductCategory("PCs & Notebooks");
                webDriver.FindElement(By.CssSelector("button.search")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.CssSelector("button.clear.btn")).Click();
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                webDriver.SelectCourtsAsFascia();
                webDriver.SelectProductCategory("Vision");
                webDriver.FindElement(By.CssSelector("button.search")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.CssSelector("button.clear.btn")).Click();
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                webDriver.SelectCourtsAsFascia();
                webDriver.SelectDeliveryZone("ON HOLD");
                webDriver.SelectProductCategory("PCs & Notebooks");
                webDriver.FindElement(By.CssSelector("button.search")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.CssSelector("button.clear.btn")).Click();
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                webDriver.SelectCourtsAsFascia();
                webDriver.SelectProductCategory("Vision");
                webDriver.FindElement(By.CssSelector("button.search")).Click();
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void PrintAllByTruckLoad()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch15"]);
                webDriver.SelectTruck("SeleniumTruck15");
                webDriver.AddFirstBookingToTruck();
                webDriver.SelectTruck("SeleniumTruck99");
                webDriver.AddFirstBookingToTruck();
                webDriver.SelectPrintGroupCriteria("By Truck Load");
                webDriver.ClickPrintAllButton();
                System.Threading.Thread.Sleep(2000);
                webDriver.OpenPickListInNewWindow();
                webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
                webDriver.ClosePopupWindow();
                webDriver.ClickCssSelector(".ok");
            }
        }

        [Test]
        public void PrintAllByAllPending()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch8"]);
                webDriver.SelectTruck("SeleniumTruck8");
                webDriver.AddFirstBookingToTruck();
                webDriver.SelectTruck("SeleniumTruck100");
                webDriver.AddFirstBookingToTruck();
                webDriver.SelectPrintGroupCriteria("All Pending");
                webDriver.ClickPrintAllButton();
                System.Threading.Thread.Sleep(2000);
                webDriver.OpenPickListInNewWindow();
                webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
                webDriver.ClosePopupWindow();
                webDriver.ClickCssSelector(".ok");
            }
        }

        [Test]
        public void PrintAllByProductCategory()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                webDriver.SelectProductCategory("PCs & Notebooks");
                webDriver.FindElement(By.CssSelector("button.search")).Click();
                Thread.Sleep(1000);
                webDriver.AddFirstBookingToTruck();
                webDriver.FindElement(By.CssSelector("button.clear.btn")).Click();
                webDriver.SelectDeliveryLocationInPickingPage(Branch.AppSettings["Branch12"]);
                webDriver.SelectProductCategory("Vision");
                webDriver.FindElement(By.CssSelector("button.search")).Click();
                Thread.Sleep(1000);
                webDriver.AddFirstBookingToTruck();
                webDriver.SelectPrintGroupCriteria("By Product Category");
                webDriver.ClickPrintAllButton();
                System.Threading.Thread.Sleep(2000);
                webDriver.OpenPickListInNewWindow();
                webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
                webDriver.ClosePopupWindow();
                webDriver.ClickCssSelector(".ok");
            }
        }
    }
}

