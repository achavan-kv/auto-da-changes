using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;
using Blue.Selenium;
using System;
using Blue.Cosacs.Selenium.Common;
using OpenQA.Selenium.Interactions;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Blue.Cosacs.Selenium.Administration
{
    [TestFixture]
    public class AdminGenericTests
    {
        [Test]
        public void AreAdministrationMenusLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.ClickAdministrationMenu();
                webDriver.CheckAdministrationMenus();
            }
        }

        [Test]
        public void CheckSessionsPage()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Administration", "Sessions", "Admin/Sessions", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Active Sessions");
                webDriver.IsTextPresent(By.CssSelector("table.table > thead > tr > th:nth-child(1)"), "User Name");
                webDriver.IsTextPresent(By.CssSelector("table.table > thead > tr > th:nth-child(2)"), "Last Request On");
                webDriver.IsTextPresent(By.CssSelector("table.table > thead > tr > th:nth-child(3)"), "Machine IP Address");
                webDriver.IsElementPresent(By.CssSelector("table.table > tbody"));
            }
        }

        [Test]
        public void GoToHomePageFromAdminPages()
        {
            using(var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.GoTo("Administration", "Sessions", "Admin/Sessions", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Active Sessions");
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.GoTo("Administration", "Audit", "Audit", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Business Events Audit");
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
            }
        }

        [Test]
        public void CommonPasswordTest()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.TurnOffPasswordComplexity();
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                var firstName = string.Empty;
                var lastName = string.Empty;
                var userName = string.Empty;
                var externalDirectoryLogin = string.Empty;
                webDriver.CreateUser(out firstName, out lastName, out userName);
                webDriver.WaitForTextPresent(By.Id("page-heading"), firstName + " " + lastName);
                webDriver.CheckNewUserDetails(firstName, lastName, userName);
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).SendKeys(userName);
                webDriver.FindElement(By.Name("password")).SendKeys("test**123");
                webDriver.FindElement(By.CssSelector(".login")).Click();
                webDriver.WaitForElementPresent(By.Name("confirmPassword"));
                webDriver.IsElementPresent(By.Name("newPassword"));
                webDriver.FindElement(By.Name("newPassword")).SendKeys("password");
                webDriver.FindElement(By.Name("confirmPassword")).SendKeys("password");
                webDriver.FindElement(By.CssSelector(".login")).Click();
                System.Threading.Thread.Sleep(500);
                List<string> passwords = webDriver.GetCommonPasswords();
                passwords.ForEach(p =>
                {
                    webDriver.IsElementPresent(By.Name("newPassword"));
                    webDriver.FindElement(By.Name("newPassword")).Clear();
                    webDriver.FindElement(By.Name("confirmPassword")).Clear();
                    webDriver.FindElement(By.Name("newPassword")).SendKeys(p);
                    webDriver.FindElement(By.Name("confirmPassword")).SendKeys(p);
                    webDriver.FindElement(By.CssSelector(".login")).Click();
                    System.Threading.Thread.Sleep(1000);
                });
                webDriver.TurnOnPasswordComplexity();
                webDriver.Navigate().Refresh();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }
    }
}