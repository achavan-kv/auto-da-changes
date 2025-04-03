using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class DriversPage
    {
        public static void IsSearchButtonPresentInDriversPage(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button[type='submit']")).Displayed);
        }

        public static void SearchDriver(this IWebDriver webDriver, string searchTerm)
        {
            webDriver.FindElement(By.Id("s_Name")).SendKeys(searchTerm);
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Sleep(1000);
        }

        public static void CreateNewDriver(this IWebDriver webDriver, out string driverName)
        {
            var r = new Random();
            var tsp = "TestResolution" + r.Next(0, 100).ToString();
            driverName = "SelDriver" + r.Next(0, 100).ToString();   //System.DateTime.Now.ToString("ddMMyyyyhhmmssff");
            webDriver.ScrollToEndOfPage();
            webDriver.IsElementPresent(By.CssSelector("a[title='New']"));
            webDriver.FindElement(By.CssSelector("a[title='New']")).Click();
            Sleep(1000);
            webDriver.ScrollToEndOfPage();
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0']"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] a[title='Save']"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] a[title='Cancel']"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] label[for='Name']"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] #Name"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] label[for='PhoneNumber']"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] #PhoneNumber"));
            webDriver.FindElement(By.CssSelector("tr[data-id='0'] #Name")).SendKeys(driverName);
            webDriver.FindElement(By.CssSelector("tr[data-id='0'] #PhoneNumber")).SendKeys("12345");
            webDriver.FindElement(By.CssSelector("tr[data-id='0'] a[title='Save']")).Click();
            webDriver.CheckNotification("×\r\nDriver added successfully");
            webDriver.CloseNotification();
            webDriver.SearchDriver(driverName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), driverName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(3)"), "12345");
            webDriver.FindElement(By.CssSelector("#s_Name")).Clear();
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Sleep(1000);
        }

        public static void DeleteDriver(this IWebDriver webDriver, string driverName)
        {
            webDriver.SearchRole(driverName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), driverName);
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Delete']"));
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Edit']"));
            webDriver.FindElement(By.CssSelector("tr.view:first-child a[title='Delete']")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to delete this Driver?");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            webDriver.CheckNotification("×\r\nDriver deleted successfully");
            webDriver.CloseNotification();
            webDriver.FindElement(By.CssSelector("#s_Name")).Clear();
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            webDriver.Navigate().Refresh();
            Assert.IsFalse(webDriver.PageSource.Contains(driverName));
        }

        public static void EditDriver(this IWebDriver webDriver, string driverName, string newDriverName)
        {
            webDriver.SearchRole(driverName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), driverName);
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Edit']"));
            webDriver.FindElement(By.CssSelector("tr.view:first-child a[title='Edit']")).Click();
            Sleep(1000);
            webDriver.FindElement(By.CssSelector("tr.edit #Name")).Clear();
            webDriver.FindElement(By.CssSelector("tr.edit #Name")).SendKeys(newDriverName);
            webDriver.FindElement(By.CssSelector("tr.edit a[title='Save']")).Click();
            webDriver.CheckNotification("×\r\nDriver saved successfully");
            webDriver.CloseNotification();
            webDriver.FindElement(By.CssSelector("#s_Name")).Clear();
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Sleep(1000);
            webDriver.SearchRole(newDriverName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), newDriverName);
            webDriver.FindElement(By.CssSelector("#s_Name")).Clear();
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Sleep(1000);
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
