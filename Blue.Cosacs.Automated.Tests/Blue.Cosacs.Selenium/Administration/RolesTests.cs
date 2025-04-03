using Blue.Cosacs.Selenium.Administration.Helpers;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Administration
{
    [TestFixture]
    public class RolesTests
    {
        [Test]
        public void CreateNewRoleAndDeleteIt()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.DeleteRole(roleName);
            }
        }

        [Test]
        public void EditRole()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.EditRole(roleName, "NewSeleniumRole2");
                webDriver.DeleteRole("NewSeleniumRole2");
            }
        }

        [Test]
        public void AccessPermissionsPageForARole()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.SearchRole("Selenium User");
                webDriver.GoTo(By.CssSelector("tr.view:first-child a[title='Permissions for this Role']"), "Admin/Roles/Permissions/33", session);
                webDriver.WaitForElementPresent(By.Id("searchBox"));
                webDriver.IsElementPresent(By.Id("searchBox"));
            }
        }

        [Test]
        public void AccessUsersPageForARole()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.SearchRole("Selenium User");
                webDriver.GoTo(By.CssSelector("tr.view:first-child a[title='Users with this Role']"), "Admin/Roles/Users/33", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Selenium User - Users");
                webDriver.IsElementPresent(By.Id("searchBox"));
                webDriver.IsTextPresent(By.CssSelector("h4"), "Users");
                webDriver.IsTextPresent(By.CssSelector("div.userRole th:nth-child(1)"), "First Name");
                webDriver.IsTextPresent(By.CssSelector("div.userRole th:nth-child(2)"), "Last Name");
                webDriver.IsTextPresent(By.CssSelector("div.userRole th:nth-child(3)"), "User Name");
                webDriver.IsTextPresent(By.CssSelector("div.userRole th:nth-child(4)"), "Delete");
            }
        }
    }
}
