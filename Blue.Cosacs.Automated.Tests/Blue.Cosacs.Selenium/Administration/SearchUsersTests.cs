using Blue.Cosacs.Selenium.Administration.Helpers;
using MbUnit.Framework;
using OpenQA.Selenium;
using System.Threading;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Administration
{
    [TestFixture]
    public class SearchUsersTests
    {
        [Test]
        public void AreSearchFiltersLoaded()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.AreSearchFiltersInSearchUsersPageLoaded();
            }
        }

        [Test]
        public void CheckSearchFilterNames()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSearchFilterNamesInSearchUsersPage();
            }
        }

        [Test]
        public void SearchUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Search Users", "Admin/Users", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchForUser("Selenium User 1 Test", "seleniumuser");
                webDriver.IsTextPresent(By.CssSelector("div.searchUsers:nth-child(1) .name"), "Selenium User 1 Test");
                webDriver.IsTextPresent(By.CssSelector("div.searchUsers:nth-child(1) .login"), "seleniumuser");
                webDriver.IsTextPresent(By.CssSelector("div.searchUsers:nth-child(1) .branch"), "COURTS 960");
                webDriver.IsTextPresent(By.CssSelector("div.searchUsers:nth-child(1) .roles"), "Roles: Selenium User");
            }
        }
    }
}
