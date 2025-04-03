using MbUnit.Framework;
using OpenQA.Selenium;
using System;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Administration.Helpers
{
    public static class RolesPage
    {
        public static void IsSearchButtonPresentInRolesPage(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button[type='submit']")).Displayed);
        }

        public static void SearchRole(this IWebDriver webDriver, string searchTerm)
        {
            webDriver.WaitForElementPresent(By.Id("s_Name"));
            webDriver.FindElement(By.Id("s_Name")).SendKeys(searchTerm);
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), searchTerm);
        }

        public static void CreateNewRole(this IWebDriver webDriver, string roleName)
        {
            webDriver.ScrollToEndOfPage();
            webDriver.WaitForElementPresent(By.CssSelector("a[title='New']"));
            webDriver.IsElementPresent(By.CssSelector("a[title='New']"));
            webDriver.FindElement(By.CssSelector("a[title='New']")).Click();
            Sleep(1000);
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0']"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] a[title='Save']"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] a[title='Cancel']"));
            webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] #Name"));
            webDriver.FindElement(By.CssSelector("tr[data-id='0'] #Name")).SendKeys(roleName);
            webDriver.FindElement(By.CssSelector("tr[data-id='0'] a[title='Save']")).Click();
            webDriver.CheckNotification("×\r\nRole added successfully");
            webDriver.CloseNotification();
            webDriver.SearchRole(roleName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), roleName);
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Permissions for this Role']"));
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Users with this Role']"));
            webDriver.FindElement(By.CssSelector("#s_Name")).Clear();
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Sleep(1000);
        }

        public static void DeleteRole(this IWebDriver webDriver, string roleName)
        {
            webDriver.SearchRole(roleName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), roleName);
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Delete']"));
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Edit']"));
            webDriver.FindElement(By.CssSelector("tr.view:first-child a[title='Delete']")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to delete this Role?");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            webDriver.CheckNotification("×\r\nRole deleted successfully");
            webDriver.CloseNotification();
            webDriver.FindElement(By.CssSelector("#s_Name")).Clear();
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            webDriver.Navigate().Refresh();
            Assert.IsFalse(webDriver.PageSource.Contains(roleName));
        }

        public static void EditRole(this IWebDriver webDriver, string roleName, string newRoleName)
        {
            webDriver.SearchRole(roleName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), roleName);
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Edit']"));
            webDriver.FindElement(By.CssSelector("tr.view:first-child a[title='Edit']")).Click();
            Sleep(1000);
            webDriver.FindElement(By.CssSelector("tr.edit #Name")).Clear();
            webDriver.FindElement(By.CssSelector("tr.edit #Name")).SendKeys(newRoleName);
            webDriver.FindElement(By.CssSelector("tr.edit a[title='Save']")).Click();
            webDriver.CheckNotification("×\r\nRole saved successfully");
            webDriver.CloseNotification();
            webDriver.FindElement(By.CssSelector("#s_Name")).Clear();
            webDriver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Sleep(1000);
            webDriver.SearchRole(newRoleName);
            webDriver.IsTextPresent(By.CssSelector("tr.view:first-child > td:nth-child(2)"), newRoleName);
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Permissions for this Role']"));
            webDriver.IsElementPresent(By.CssSelector("tr.view:first-child a[title='Users with this Role']"));
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
