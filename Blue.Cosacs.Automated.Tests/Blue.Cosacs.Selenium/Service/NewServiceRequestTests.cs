using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;
using Branch = System.Configuration.ConfigurationManager;

namespace Blue.Cosacs.Selenium.Service
{
    [TestFixture]
    public class NewServiceRequestTests
    {
        [Test]
        public void CheckNewSRTypes()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#si"));
                webDriver.CheckNewSRTypes();
                webDriver.IsCreateButtonDisabled();
            }
        }
      
        [Test]
        public void CreateNewInternalSR()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#si"));
                webDriver.SelectSrType("#si");
                webDriver.IsSearchButtonDisabled();
                webDriver.TypeSearchSelectorParameter(webDriver.GetAccountNo());
                webDriver.IsSearchButtonEnabled();
                webDriver.CLickButtonByText("button", "Search");
                webDriver.WaitForElementPresent(By.CssSelector("div.searchResults"));
                webDriver.IsSearchResultsSectionDisplayed();
                webDriver.CheckSearchResults();
                webDriver.CLickButtonByText("btn", "Select");
                webDriver.SelectManufacturer("Brother");
                webDriver.FillRequiredFields();
                webDriver.SaveSR();
            }
        }
       
        [Test]
        public void CreateNewExternalSR()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
            }
        }

        [Test]
        public void CreateNewStockRepair()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#s"));
                webDriver.SelectSrType("#s");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.SelectProductHierarchy("Electrical", "1 - Vision", "10  - Vision");
                webDriver.FindElement(By.CssSelector("input[name=ItemNumber]")).SendKeys("12345");
                webDriver.FindElement(By.CssSelector("input[name=Item]")).SendKeys("Test Item");
                webDriver.FindElement(By.CssSelector("input[name=ItemSupplier]")).SendKeys("Supplier"); webDriver.SelectManufacturer("Brother");
                webDriver.SelectStockLocation(Branch.AppSettings["Branch3"]);
                webDriver.FindElement(By.CssSelector("input[name=ItemAmount]")).SendKeys("5000");
                webDriver.SaveSR();
            }
        }

       [Test]
        public void CreateNewExternalInstallation()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#ie"));
                webDriver.SelectSrType("#ie");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
            }
        }

        [Test]
     public void CreateNewInternalInstallation()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#si"));
                webDriver.SelectSrType("#ii");
                webDriver.IsSearchButtonDisabled();
                webDriver.TypeSearchSelectorParameter(webDriver.GetAccountNo());
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CLickButtonByText("btn", "Select");
                webDriver.WaitForElementPresent(By.CssSelector("#s2id_Manufacturer"));
                webDriver.SelectManufacturer("Brother");
                webDriver.FillRequiredFields();
                webDriver.SaveSR();
            }
        }
    }
}
