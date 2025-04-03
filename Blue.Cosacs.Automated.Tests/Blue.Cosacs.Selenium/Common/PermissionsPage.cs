using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;

namespace Blue.Cosacs.Selenium.Common
{
    public static class PermissionsPage
    {
        public enum PermissionCategory
        {
            SuperUser = 1,
            AccountFunctions = 2,
            Cashier = 3,
            Configuration = 4,
            CreditCollections = 5,
            CreditSanctioning = 6,
            CustomerFunctions = 7,
            EndofDay = 8,
            Finance = 9,
            Payments = 10,
            Reports = 11,
            Sales = 12,
            Scheduling = 13,
            Service = 14,
            ServiceCosacsClient = 15,
            SystemAdministration = 16,
            Warehouse = 17,
            Warranty = 18,
            WebReports = 19
        }

        public static void CreatePermissionsTestRole(this IWebDriver webDriver, out string roleName, Session session)
        {
            Sleep(2000);
            webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
            webDriver.HasPermission(session);
            webDriver.WaitForElementPresent(By.Id("s_Name"));
            roleName = "SelPerm" + System.DateTime.Now.ToString("ddMMyyyyhhmmssff");
            webDriver.CreateNewRole(roleName);
        }

        public static void CreatePermissionsTestUser(this IWebDriver webDriver, out string userName, string roleName, Session session)
        {
            Sleep(1000);
            webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
            var firstName = "SelPerm";
            var lastName = string.Empty;
            userName = string.Empty;
            var externalDirectoryLogin = string.Empty;
            webDriver.CreateUser(firstName, out lastName, out userName);
            webDriver.WaitForTextPresent(By.Id("page-heading"), "SelPerm " + lastName);
            webDriver.CheckNewUserDetails(firstName, lastName, userName);
            webDriver.Navigate().Refresh();
            webDriver.AddNewRole(roleName);
        }

        public static void CreatePermissionsTestUser(this IWebDriver webDriver, out string firstName, out string lastName, out string userName, string roleName, Session session)
        {
            Sleep(1000);
            webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
            firstName = "SelPerm";
            lastName = string.Empty;
            userName = string.Empty;
            var externalDirectoryLogin = string.Empty;
            webDriver.CreateUser(firstName, out lastName, out userName);
            webDriver.WaitForTextPresent(By.Id("page-heading"), "SelPerm " + lastName);
            webDriver.CheckNewUserDetails(firstName, lastName, userName);
            webDriver.AddNewRole(roleName);
        }

        public static void GoToPermissionsPage(this IWebDriver webDriver, string roleName, Session session)
        {
            webDriver.GoTo("Administration", "Roles", "Admin/Roles", session);
            webDriver.WaitForElementPresent(By.Id("s_Name"));
            webDriver.SearchRole(roleName);
            webDriver.GoTo(By.CssSelector("tr.view:first-child a[title='Permissions for this Role']"), string.Empty, session);
            webDriver.WaitForElementPresent(By.Id("searchBox"));
        }

        public static void SearchPermission(this IWebDriver webDriver, string permission)
        {
            webDriver.IsElementPresent(By.Id("searchBox"));
            webDriver.FindElement(By.Id("searchBox")).SendKeys(permission);
        }

        private const string checkboxLocator = ".rolePermissions > .table:nth-of-type({0}) > tbody > tr[data-id='{1}'] > td ";
        private const string Allow = "#Allow";
        private const string Deny = "#Deny";

        private static void ChangePermission(this IWebDriver webDriver, PermissionCategory permissionCategory, string permissionId, bool allowPermission)
        {
            var mainAction = allowPermission ? Allow : Deny;
            var secondAction = allowPermission ? Deny : Allow;
            var per = ((int)permissionCategory).ToString();
            var localLocator = string.Format(checkboxLocator, per, permissionId);

            webDriver.ScrollElementInToView(By.CssSelector(localLocator + mainAction));
            
            if (webDriver.FindElement(By.CssSelector(localLocator + mainAction)).Selected)
            {
                webDriver.FindElement(By.CssSelector(localLocator + secondAction)).Click();
            }

            webDriver.FindElement(By.CssSelector(localLocator + mainAction)).Click();
        }

        public static void AllowPermission(this IWebDriver webDriver, PermissionCategory permissionCategory, string permissionId)
        {
            webDriver.ChangePermission(permissionCategory, permissionId, true);
        }

        public static void DenyPermission(this IWebDriver webDriver, PermissionCategory permissionCategory, string permissionId)
        {
            ChangePermission(webDriver, permissionCategory, permissionId, false);
        }

        public static bool IsPermissionAllowed(this IWebDriver webDriver, PermissionCategory permissionCategory, string permissionId)
        {
            var per = ((int)permissionCategory).ToString();
            var localLocator = string.Format(checkboxLocator, per, permissionId);
            bool isAllowed = webDriver.FindElement(By.CssSelector(localLocator + Allow)).Selected;
            return isAllowed;
        }

        public static bool IsPermissionDenied(this IWebDriver webDriver, PermissionCategory permissionCategory, string permissionId)
        {
            var per = ((int)permissionCategory).ToString();
            var localLocator = string.Format(checkboxLocator, per, permissionId);
            bool isDenied = webDriver.FindElement(By.CssSelector(localLocator + Deny)).Selected;
            return isDenied;
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
