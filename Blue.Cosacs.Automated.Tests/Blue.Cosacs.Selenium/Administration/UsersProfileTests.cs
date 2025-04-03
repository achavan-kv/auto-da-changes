using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using OpenQA.Selenium.Interactions;
using System.Threading;

namespace Blue.Cosacs.Selenium.Administration
{
    [TestFixture]
    public class UsersProfileTests
    {
        [Test]
        public void CheckUserDetailsSection()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User Test", "-11");
                webDriver.GoTo(By.XPath(".//*[@id='body']//div[contains(@class, 'login')][contains(text(), '-11')]"), "Admin/Users/Profile/221048", session);//sk added
         //       webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), '-11')]"), "Admin/Users/Profile/221048", session);//sk commented
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User Test");
                webDriver.IsUserDetailsSectionPresent();
                webDriver.IsElementPresent(By.Id("edit"));
                webDriver.CheckFirstName("Selenium User");
                webDriver.CheckLastName("Test");
                webDriver.CheckUserName("-11");
                webDriver.CheckEmail("selenium.bbs@bluebridgeltd.com");
                webDriver.CheckExternalDirectoryLogin("");
                webDriver.CheckBranchName("COURTS 960");
            }
        }

        [Test]
        public void CheckChangePasswordSectionForCurrentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsChangePasswordSectionPresent();
                webDriver.IsElementPresent(By.Id("currentPassword"));
                webDriver.IsElementPresent(By.Id("newPassword"));
                webDriver.IsElementPresent(By.Id("confirmPassword"));
                webDriver.IsElementPresent(By.CssSelector("button#buttonChangePassword"));
            }
        }

        [Test]
        public void CheckChangePasswordSectionForDifferentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[contains(@class, 'login')][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);//added sk
                //webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);//commented sk
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                webDriver.IsChangePasswordSectionPresent();
                webDriver.IsElementPresent(By.Id("newPassword"));
                webDriver.IsElementPresent(By.Id("confirmPassword"));
                webDriver.IsElementPresent(By.CssSelector("button#buttonChangePassword"));
            }
        }

        [Test]
        public void CheckPasswordResetSection()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsResetPasswordSectionPresent();
                webDriver.IsElementPresent(By.CssSelector("button#buttonResetPassword"));
            }
        }

        [Test]
        public void CheckLockOrUnlockUserSectionForDifferentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[contains(@class, 'login')][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);//added sk
                //webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);//commented sk
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                webDriver.IsElementPresent(By.CssSelector("button#buttonLockUser"));
            }
        }

        [Test]
        public void CheckLockOrUnlockUserSectionForCurrentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                Assert.IsFalse(webDriver.FindElements(By.CssSelector("button#buttonLockUser")).Count > 0);
            }
        }

        [Test]
        public void CheckSecurityAuditSection()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsSecurityAuditSectionPresent();
                webDriver.CheckAuditTableColumnHeadersInUsersProfilePage();
                Assert.IsTrue(webDriver.FindElement(By.LinkText("Remaining audit records for this user")).Displayed);
            }
        }

        [Test]
        public void CheckPermissionsSection()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsPermissionsSectionPresent();
                webDriver.IsElementPresent(By.Id("searchBox"));
                webDriver.IsElementPresent(By.CssSelector("div#addRole_chzn > a.chzn-single.chzn-default"));
                webDriver.IsTextPresent(By.CssSelector("div#addRole_chzn > a.chzn-single.chzn-default"), "Add New Role");
            }
        }

        [Test]
        public void ChangePasswordForCurrentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsElementPresent(By.Id("currentPassword"));
                var newPassword = string.Empty;
                webDriver.ChangePasswordForCurrentUser(session.Password, out newPassword);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginToCosacs(session.Username, newPassword);
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsElementPresent(By.Id("currentPassword"));
                webDriver.ChangeCurrentUserPasswordBackToActiveUserPassword(session.Password, newPassword);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void ChangePasswordForDifferentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.GoTo(By.XPath("//div[contains(@class, 'login')][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);//added sk
                //webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), 'seleniumuser')]"), "Admin/Users/Profile/221046", session);//commented sk
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User 1 Test");
                var newPassword = string.Empty;
                webDriver.ChangePasswordForDifferentUser(out newPassword);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginToCosacs("seleniumuser", newPassword);
                webDriver.ResetPassword("ingres**");
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.IsElementPresent(By.Id("currentPassword"));
                webDriver.ChangeCurrentUserPasswordBackToActiveUserPassword("test**123", "ingres**");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginToCosacs("seleniumuser", "test**123");
                System.Threading.Thread.Sleep(1000);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.FindElement(By.Name("password")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
                System.Threading.Thread.Sleep(1000);
            }
        }

        [Test]
        public void ResetPassword()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.SendPasswordResetEmail();
                var resetPasswordLink = string.Empty;
                webDriver.CheckResetPasswordEmail(out resetPasswordLink, session);
                webDriver.Navigate().GoToUrl(resetPasswordLink);
                webDriver.ResetPassword(session.Password);
            }
        }

        [Test]
        public void EditUserDetails()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User Test", "-11");
                webDriver.GoTo(By.XPath(".//*[@id='body']//div[contains(@class, 'login')][contains(text(), '-11')]"), "Admin/Users/Profile/221048", session);//sk added
                //webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), '-11')]"), "Admin/Users/Profile/221048", session); //commented sk
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User Test");
                var originalFirstName = string.Empty;
                var originalLastName = string.Empty;
                var originalUserName = string.Empty;
                var originalEmail = string.Empty;
                var newFirstName = string.Empty;
                var newLastName = string.Empty;
                var newUserName = string.Empty;
                var newEmail = string.Empty;
                webDriver.UpdateUserDetailsToNew(out originalFirstName, out originalLastName, out originalUserName, out originalEmail, out newFirstName, out newLastName, out newUserName, out newEmail);
                Actions scrollto = new Actions(webDriver);
                scrollto.SendKeys(Keys.Control).SendKeys(Keys.Home).Build().Perform();
                scrollto.KeyUp(Keys.Control).Build().Perform();
                webDriver.Navigate().Refresh();
                webDriver.ChangeUserDetailsBackToOriginal(originalFirstName, originalLastName, originalUserName, originalEmail);
            }
        }

        [Test]
        public void AddNewRoleToUserAndDeleteIt()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                var firstName = string.Empty;
                var lastName = string.Empty;
                var userName = string.Empty;
                var externalDirectoryLogin = string.Empty;
                webDriver.CreateUser(out firstName, out lastName, out userName);
                webDriver.WaitForTextPresent(By.Id("page-heading"), firstName + " " + lastName);
                webDriver.AddNewRole("Selenium User");
                webDriver.RemoveRole();
                webDriver.LockUser();
            }
        }

        [Test]
        public void LockAndUnlockUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium Permissions User", "seleniumpermissions");
                webDriver.GoTo(By.XPath("//div[contains(@class, 'login')][contains(text(), 'seleniumpermissions')]"), "Admin/Users/Profile/221047", session); //added sk
             //   webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), 'seleniumpermissions')]"), "Admin/Users/Profile/221047", session); commented sk
                Thread.Sleep(500);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium Permissions User");
                webDriver.LockUser();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginToCosacs("seleniumpermissions", "test**123");
                webDriver.IsTextPresent(By.CssSelector("div#divError"), "Your account is currently locked. Please contact your System Administrator.");
                Thread.Sleep(100);
                webDriver.LoginToCosacs(session.Username, session.Password);
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium Permissions User", "seleniumpermissions");
                webDriver.GoTo(By.XPath("//div[contains(@class, 'login')][contains(text(), 'seleniumpermissions')]"), "Admin/Users/Profile/221047", session); //added sk
                //webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), 'seleniumpermissions')]"), "Admin/Users/Profile/221047", session); //commented sk
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium Permissions User");
                webDriver.UnlockUser();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginToCosacs("seleniumpermissions", "test**123");
                Thread.Sleep(500);
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                Thread.Sleep(500);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AccessRolePermissionsPageFromUserProfilesPage()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.GoToRolePermissions("Selenium User");
            }
        }

        [Test]
        public void SearchPermissionInUserProfilesPage()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.SearchPermissionInUserProfilePage();
            }
        }

        [Test]
        public void ViewAuditForCurrentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.Id("profile"));
                webDriver.GoTo(By.Id("profile"), "Admin/Profile", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
                webDriver.GoToAuditScreenFromUserProfileScreen(session.Username);
            }
        }

        [Test]
        public void ViewAuditForDifferentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium Permissions User", "seleniumpermissions");
                webDriver.GoTo(By.XPath("//div[contains(@class, 'login')][contains(text(), 'seleniumpermissions')]"), "Admin/Users/Profile/221047", session); //added sk
                //webDriver.GoTo(By.XPath("//div[@class='login ng-binding'][contains(text(), 'seleniumpermissions')]"), "Admin/Users/Profile/221047", session); //commented sk
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium Permissions User");
                webDriver.GoToAuditScreenFromUserProfileScreen("seleniumpermissions");
            }
        }
    }
}
