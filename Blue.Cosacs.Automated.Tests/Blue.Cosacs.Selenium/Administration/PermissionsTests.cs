using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Permission = Blue.Cosacs.Selenium.Common.PermissionsPage.PermissionCategory;

namespace Blue.Cosacs.Selenium.Administration
{
    [TestFixture]
    public class PermissionsTests
    {
        [Test]
        public void AllowUserSessionsPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "386");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "386"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickAdministrationMenu();
                webDriver.IsElementPresent(By.LinkText("Sessions"));
                webDriver.GoTo("Administration", "Sessions", "Admin/Sessions", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Active Sessions");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyUserSessionsPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.SystemAdministration, "386");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "386"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.IsElementNotPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Sessions", "Admin/Sessions", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Sessions - User Sessions (System Administration)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowKillUserSessionPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var firstName = string.Empty;
                var lastName = string.Empty;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out firstName, out lastName, out userName, roleName, session);
                var currentUserProfileId = webDriver.GetUserProfileId(userName);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "386");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "386"));
                webDriver.AllowPermission(Permission.SystemAdministration, "387");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "387"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Sessions", "Admin/Sessions", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Active Sessions");
                webDriver.IsTextPresent(By.CssSelector("[data-userid='" + currentUserProfileId + "'] > td:first-child"), firstName + " " + lastName);
                webDriver.ScrollElementInToView(By.CssSelector("[data-userid='" + currentUserProfileId + "'] > td > [title='Kill Session']"));
                webDriver.IsElementPresent(By.CssSelector("[data-userid='" + currentUserProfileId + "'] > td > [title='Kill Session']"));
                webDriver.FindElement(By.CssSelector("[data-userid='" + currentUserProfileId + "'] > td > [title='Kill Session']")).Click();
                webDriver.IsElementNotPresent(By.LinkText(firstName + " " + lastName));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyKillUserSessionPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var firstName = string.Empty;
                var lastName = string.Empty;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out firstName, out lastName, out userName, roleName, session);
                var currentUserProfileId = webDriver.GetUserProfileId(userName);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "386");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "386"));
                webDriver.DenyPermission(Permission.SystemAdministration, "387");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "387"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Sessions", "Admin/Sessions", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Active Sessions");
                webDriver.IsTextPresent(By.CssSelector("[data-userid='" + currentUserProfileId + "'] > td:first-child"), firstName + " " + lastName);
                webDriver.IsElementPresent(By.CssSelector("[data-userid='" + currentUserProfileId + "'] > td > [title='Kill Session']"));
                webDriver.ScrollElementInToView(By.CssSelector("[data-userid='" + currentUserProfileId + "'] > td > [title='Kill Session']"));
                webDriver.FindElement(By.CssSelector("[data-userid='" + currentUserProfileId + "'] > td > [title='Kill Session']")).Click();
                webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "You are not allowed to perform the following action: Sessions - Kill User Session (System Administration)");
                webDriver.IsTextPresent(By.CssSelector("#confirm p"), "You are not allowed to perform the following action: Sessions - Kill User Session (System Administration)");
                webDriver.FindElement(By.CssSelector("button.ok")).Click();
                webDriver.IsElementPresent(By.LinkText(firstName + " " + lastName));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowAuditPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "390");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "390"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickAdministrationMenu();
                webDriver.IsElementPresent(By.LinkText("Audit"));
                webDriver.GoTo("Administration", "Audit", "Audit", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Business Events Audit");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyAuditPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.SystemAdministration, "390");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "390"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.IsElementNotPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Audit", "Audit", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: SysConfig - Audit (System Administration)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowAuditUserPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "389");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "389"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsSecurityAuditSectionPresent();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyAuditUserPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.SystemAdministration, "389");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "389"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsElementNotPresent(By.CssSelector("div.grid_3"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowChangePasswordPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.AllowPermission(Permission.SystemAdministration, "382");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "382"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsChangePasswordSectionPresent();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyChangePasswordPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.DenyPermission(Permission.SystemAdministration, "382");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "382"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsElementNotPresent(By.Id("divChangePassword"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }
        
        [Test]
        public void AllowCreateUserPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "388");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "388"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickAdministrationMenu();
                webDriver.IsElementPresent(By.LinkText("Create User"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                var firstName = string.Empty;
                var lastName = string.Empty;
                var newUserName = string.Empty;
                var externalDirectoryLogin = string.Empty;
                webDriver.CreateUser(out firstName, out lastName, out newUserName);
                webDriver.WaitForTextPresent(By.Id("page-heading"), firstName + " " + lastName);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyCreateUserPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.SystemAdministration, "388");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "388"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.IsElementNotPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Staff Maintainance - Create User (System Administration)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowEditUserPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.AllowPermission(Permission.SystemAdministration, "384");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "384"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[@class='col-lg-3 login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                webDriver.IsUserDetailsSectionPresent();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyEditUserPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.DenyPermission(Permission.SystemAdministration, "384");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "384"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[@class='col-lg-3 login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                webDriver.IsElementNotPresent(By.CssSelector("span#edit"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowLockUserPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.AllowPermission(Permission.SystemAdministration, "379");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "379"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[@class='col-lg-3 login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                webDriver.IsElementPresent(By.CssSelector("button#buttonLockUser"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyLockUserPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.DenyPermission(Permission.SystemAdministration, "379");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "379"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[@class='col-lg-3 login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                webDriver.IsElementNotPresent(By.CssSelector("button#buttonLockUser"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowResetPasswordPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.AllowPermission(Permission.SystemAdministration, "383");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "383"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[@class='col-lg-3 login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                webDriver.IsResetPasswordSectionPresent();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyResetPasswordPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.DenyPermission(Permission.SystemAdministration, "383");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "383"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[@class='col-lg-3 login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                webDriver.IsElementNotPresent(By.Id("divResetPassword"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowViewRolesPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "392");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "392"));
                webDriver.AllowPermission(Permission.SystemAdministration, "1205");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "1205"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.IsElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.SearchRole("Selenium User");
                webDriver.GoTo(By.CssSelector("tr.view:first-child a[title='Permissions for this Role']"), "Admin/Roles/Permissions/33", session);
                webDriver.WaitForElementPresent(By.Id("searchBox"));
                webDriver.IsElementPresent(By.Id("searchBox"));
                webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.SearchRole("Selenium User");
                webDriver.GoTo(By.CssSelector("tr.view:first-child a[title='Users with this Role']"), "Admin/Roles/Users/33", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User - Users");
                webDriver.IsElementPresent(By.Id("searchBox"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyViewRolesPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.SystemAdministration, "392");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "392"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.IsElementNotPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Sys Config - View Roles (System Administration)");
                webDriver.GoTo("Administration", "Roles", "Admin/Roles/Permissions/33", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Sys Config - View Roles (System Administration)");
                webDriver.GoTo("Administration", "Roles", "Admin/Roles/Users/33", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Sys Config - View Roles (System Administration)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowSearchUsersPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.SystemAdministration, "391"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickAdministrationMenu();
                webDriver.IsElementPresent(By.LinkText("Search Users"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenySearchUsersPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.SystemAdministration, "391");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.SystemAdministration, "391"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.IsElementNotPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Users - Search (System Administration)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }
    }
}
