using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Permission = Blue.Cosacs.Selenium.Common.PermissionsPage.PermissionCategory;

namespace Blue.Cosacs.Selenium.Configuration
{
    [TestFixture]
    public class PermissionsTests
    {
        [Test]
        public void AllowReIndexEveryThingPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1430");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1430"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickConfigurationMenu();
                webDriver.IsElementPresent(By.LinkText("Re-Indexing"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyReIndexEveryThingPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1430");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1430"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.IsElementNotPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Re-Indexing", "Indexing", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: ReIndex Everything (Warehouse)");
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowHubViewPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1429");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1429"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickConfigurationMenu();
                webDriver.IsElementPresent(By.LinkText("Hub"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyHubViewPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1429");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1429"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.IsElementNotPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Hub - View (Warehouse)");
                webDriver.GoTo("Configuration", "Hub", "Hub/Messages/1", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Hub - View (Warehouse)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }
    }
}
