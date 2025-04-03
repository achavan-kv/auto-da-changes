using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Branch = System.Configuration.ConfigurationManager;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class TrucksPage
    {
        public static void IsSearchButtonPresentInTrucksPage(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button[type='submit']")).Displayed);
        }

        public static void SearchTruck(this IWebDriver webDriver, string searchTerm, string branch, string driver)
        {
            webDriver.WaitForElementPresent(By.Id("s_Name"));
            webDriver.FindElement(By.Id("s_Name")).Clear();
            webDriver.FindElement(By.Id("s_Name")).SendKeys(searchTerm);
            webDriver.SelectFromDropDown("#s_Branch", branch);
            webDriver.SelectFromDropDown("#s_DriverId", driver);
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Sleep(1000);
        }

        public static void CreateNewTruck(this IWebDriver webDriver, string truckName, out string branch, out string driver)
        {
            webDriver.ScrollToEndOfPage();
            webDriver.IsElementPresent(By.CssSelector("[title='New']"));
            webDriver.FindElement(By.CssSelector("[title='New']")).Click();
            Sleep(1000);
            webDriver.IsElementPresent(By.CssSelector("[data-id='0']"));
            webDriver.IsElementPresent(By.CssSelector("[data-id='0'] [title='Save']"));
            webDriver.IsElementPresent(By.CssSelector("[data-id='0'] [title='Cancel']"));
            webDriver.ScrollToEndOfPage();
            webDriver.IsElementPresent(By.CssSelector("[data-id='0'] label[for='Name']"));
            webDriver.IsElementPresent(By.CssSelector("[data-id='0'] #Name"));
            webDriver.IsElementPresent(By.CssSelector("[data-id='0'] label[for='Branch']"));
            webDriver.IsElementPresent(By.CssSelector("[data-id='0'] label[for='Driver']"));
            branch = Branch.AppSettings["Branch8"];
            driver = "SeleniumDriver8";
            webDriver.FindElement(By.CssSelector("[data-id='0'] #Name")).SendKeys(truckName);
            webDriver.SelectFromDropDown("[data-id='0'] .row:nth-child(2) .chzn-done", branch);
            Sleep(500);
            webDriver.SelectFromDropDown("[data-id='0'] .row:nth-child(3) .chzn-done", driver);
            Sleep(500);
            webDriver.FindElement(By.CssSelector("[data-id='0'] [title='Save']")).Click();
            webDriver.CheckNotification("×\r\nTruck added successfully");
            webDriver.CloseNotification();
            webDriver.SearchTruck(truckName, branch, driver);
            webDriver.IsTextPresent(By.CssSelector(".view:first-child > td:nth-child(2)"), truckName);
            webDriver.IsTextPresent(By.CssSelector(".view:first-child > td:nth-child(3)"), "747 COURTS");
            webDriver.IsTextPresent(By.CssSelector(".view:first-child > td:nth-child(4)"), "SeleniumDriver8");
        }

        public static void DeleteTruck(this IWebDriver webDriver, string truckName, string branch, string driver)
        {
            webDriver.SearchTruck(truckName, branch, driver);
            webDriver.IsTextPresent(By.CssSelector(".view:first-child > td:nth-child(2)"), truckName);
            webDriver.IsElementPresent(By.CssSelector(".view:first-child [title='Delete']"));
            webDriver.IsElementPresent(By.CssSelector(".view:first-child [title='Edit']"));
            webDriver.FindElement(By.CssSelector(".view:first-child [title='Delete']")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to delete this Truck?");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            webDriver.CheckNotification("×\r\nTruck deleted successfully");
            webDriver.CloseNotification();
            webDriver.FindElement(By.Id("s_Name")).Clear();
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            webDriver.Navigate().Refresh();
            Assert.IsFalse(webDriver.PageSource.Contains(truckName));
        }

        public static void EditTruck(this IWebDriver webDriver, string truckName, string newTruckName, string branch, string driver, out string newBranch, out string newDriver)
        {
            webDriver.SearchTruck(truckName, branch, driver);
            newBranch = Branch.AppSettings["Branch5"];
            newDriver = "SeleniumDriver5";
            webDriver.IsTextPresent(By.CssSelector(".view:first-child > td:nth-child(2)"), truckName);
            webDriver.IsElementPresent(By.CssSelector(".view:first-child [title='Edit']"));
            webDriver.FindElement(By.CssSelector(".view:first-child [title='Edit']")).Click();
            Sleep(1000);
            webDriver.FindElement(By.CssSelector(".edit #Name")).Clear();
            webDriver.FindElement(By.CssSelector(".edit #Name")).SendKeys(newTruckName);
            webDriver.SelectFromDropDown(".edit .row:nth-child(2) .chzn-done", newBranch);
            webDriver.SelectFromDropDown(".edit .row:nth-child(3) .chzn-done", newDriver);
            webDriver.FindElement(By.CssSelector(".edit [title='Save']")).Click();
            webDriver.CheckNotification("×\r\nTruck saved successfully");
            webDriver.CloseNotification();
            webDriver.SearchTruck(newTruckName, newBranch, newDriver);
            webDriver.IsTextPresent(By.CssSelector(".view:first-child > td:nth-child(2)"), newTruckName);
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
