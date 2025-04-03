using MbUnit.Framework;
using OpenQA.Selenium;
using System.Diagnostics;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using OpenQA.Selenium.Interactions;

namespace Blue.Cosacs.Selenium.Administration.Helpers
{
    public static class UsersProfilePage
    {
        public static void CheckFirstName(this IWebDriver webDriver, string firstName)
        {
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(1) label"), "First Name");
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(1) div"), firstName);
        }

        public static void CheckLastName(this IWebDriver webDriver, string lastName)
        {
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(2) label"), "Last Name");
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(2) div"), lastName);
        }

        public static void CheckUserName(this IWebDriver webDriver, string userName)
        {
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(3) label"), "User Name");
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(3) div"), userName);
        }

        public static void CheckEmail(this IWebDriver webDriver, string email)
        {
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(4) label"), "Email");
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(4) div"), email);
        }

        public static void CheckExternalDirectoryLogin(this IWebDriver webDriver, string externalDirectoryLogin)
        {
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(5) label"), "External Directory Login");
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(5) div"), externalDirectoryLogin);
        }

        public static void CheckBranchName(this IWebDriver webDriver, string branchName)
        {
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(7) label"), "Branch Name");
            webDriver.IsTextPresent(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(7) div"), branchName);
        }

        public static void IsUserDetailsSectionPresent(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.CssSelector("div[data-module='admin/userDetails']"));
            webDriver.IsTextPresent(By.CssSelector("[data-module='admin/userDetails'] .section"), "User Details");
        }

        public static void IsChangePasswordSectionPresent(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.Id("divChangePassword"));
            webDriver.IsTextPresent(By.CssSelector("div#divChangePassword > div.section"), "Change Password");
        }

        public static void IsResetPasswordSectionPresent(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.Id("divResetPassword"));
            webDriver.IsTextPresent(By.CssSelector("#divResetPassword .section"), "Password Reset");
        }

        public static void IsSecurityAuditSectionPresent(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.CssSelector(".row .col-lg-4 .section"));
            webDriver.IsTextPresent(By.CssSelector(".row .col-lg-4 .section"), "Security Audit (last 50 entries from past week)");
        }

        public static void IsPermissionsSectionPresent(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.CssSelector(".row .col-lg-8 .section:nth-child(1)"));
            webDriver.IsTextPresent(By.CssSelector(".row .col-lg-8 .section:nth-child(1)"), "Permissions");
        }

        public static void CheckAuditTableColumnHeadersInUsersProfilePage(this IWebDriver webDriver)
        {
            Assert.AreEqual("Event On", webDriver.FindElement(By.CssSelector(".row .col-lg-4 .table th:nth-child(1)")).Text);
            Assert.AreEqual("Client Address", webDriver.FindElement(By.CssSelector(".row .col-lg-4 .table th:nth-child(2)")).Text);
            Assert.AreEqual("Event", webDriver.FindElement(By.CssSelector(".row .col-lg-4 .table th:nth-child(3)")).Text);
        }

        public static void ChangePasswordForCurrentUser(this IWebDriver webDriver, string activeUserPassword, out string newPassword)
        {
            webDriver.IsElementPresent(By.Id("newPassword"));
            webDriver.IsElementPresent(By.Id("confirmPassword"));
            webDriver.FindElement(By.Id("currentPassword")).SendKeys(activeUserPassword);
            newPassword = "12345**";
            webDriver.FindElement(By.Id("newPassword")).SendKeys(newPassword);
            webDriver.FindElement(By.Id("confirmPassword")).SendKeys(newPassword);
            webDriver.IsElementPresent(By.CssSelector("button#buttonChangePassword"));
            webDriver.FindElement(By.CssSelector("button#buttonChangePassword")).Click();
            Sleep(1000);
            webDriver.IsTextPresent(By.CssSelector("#confirm p"), "Password changed");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            Sleep(1000);
        }

        public static void ChangeCurrentUserPasswordBackToActiveUserPassword(this IWebDriver webDriver, string activeUserPassword, string newPassword)
        {
            webDriver.IsElementPresent(By.Id("newPassword"));
            webDriver.IsElementPresent(By.Id("confirmPassword"));
            webDriver.FindElement(By.Id("currentPassword")).SendKeys(newPassword);
            webDriver.FindElement(By.Id("newPassword")).SendKeys(activeUserPassword);
            webDriver.FindElement(By.Id("confirmPassword")).SendKeys(activeUserPassword);
            webDriver.IsElementPresent(By.CssSelector("button#buttonChangePassword"));
            webDriver.FindElement(By.CssSelector("button#buttonChangePassword")).Click();
            Sleep(1000);
            webDriver.IsTextPresent(By.CssSelector("#confirm p"), "Password changed");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            Sleep(1000);
        }

        public static void ChangePasswordForDifferentUser(this IWebDriver webDriver, out string newPassword)
        {
            webDriver.IsElementPresent(By.Id("newPassword"));
            webDriver.IsElementPresent(By.Id("confirmPassword"));
            newPassword = "12345**";
            webDriver.FindElement(By.Id("newPassword")).SendKeys(newPassword);
            webDriver.FindElement(By.Id("confirmPassword")).SendKeys(newPassword);
            webDriver.IsElementPresent(By.CssSelector("button#buttonChangePassword"));
            webDriver.FindElement(By.CssSelector("button#buttonChangePassword")).Click();
            Sleep(1000);
            webDriver.IsTextPresent(By.CssSelector("#confirm p"), "Password changed");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            Sleep(1000);
        }

        public static void SendPasswordResetEmail(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.CssSelector("button#buttonResetPassword"));
            webDriver.FindElement(By.CssSelector("button#buttonResetPassword")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "We've sent password reset instructions to your email address.");
            webDriver.IsTextPresent(By.CssSelector("#confirm p"), "We've sent password reset instructions to your email address.");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            Sleep(1000);
        }

        public static void UpdateUserDetailsToNew(this IWebDriver webDriver, out string originalFirstName, out string originalLastName, out string originalUserName, out string originalEmail, 
                                                                      out string newFirstName, out string newLastName, out string newUserName, out string newEmail)
        {
            originalFirstName = webDriver.FindElement(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(1) div")).Text.ToString();
            originalLastName = webDriver.FindElement(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(2) div")).Text.ToString();
            originalUserName = webDriver.FindElement(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(3) div")).Text.ToString();
            originalEmail = webDriver.FindElement(By.CssSelector("div#viewer > div:nth-child(3) > div.row:nth-child(4) div")).Text.ToString();
            webDriver.IsElementPresent(By.CssSelector("span#edit"));
            webDriver.FindElement(By.CssSelector("span#edit")).Click();
            Sleep(1000);
            newFirstName = "New First Name";
            newLastName = "New Last Name";
            newUserName = System.DateTime.Now.ToString("ddMMyyyyhhmmss");
            newEmail = "newemail@email.com";
            webDriver.FindElement(By.Id("FirstName")).Clear();
            webDriver.FindElement(By.Id("FirstName")).SendKeys(newFirstName);
            webDriver.FindElement(By.Id("LastName")).Clear();
            webDriver.FindElement(By.Id("LastName")).SendKeys(newLastName);
            webDriver.FindElement(By.Id("userLogin")).Clear();
            webDriver.FindElement(By.Id("userLogin")).SendKeys(newUserName);
            webDriver.FindElement(By.Id("eMail")).Clear();
            webDriver.FindElement(By.Id("eMail")).SendKeys(newEmail);
            webDriver.MoveToElement(By.CssSelector("#profileSelector"));
            webDriver.FindElement(By.CssSelector("#save")).Click();
            webDriver.CheckNotification("×\r\nUser details saved successfully.");
            webDriver.CloseNotification();
        }

        public static void ChangeUserDetailsBackToOriginal(this IWebDriver webDriver, string originalFirstName, string originalLastName, string originalUserName, string originalEmail)
        {
            Sleep(500);
            webDriver.FindElement(By.CssSelector("span#edit")).Click();
            System.Threading.Thread.Sleep(1000);
            webDriver.FindElement(By.Id("FirstName")).Clear();
            webDriver.FindElement(By.Id("FirstName")).SendKeys(originalFirstName);
            webDriver.FindElement(By.Id("LastName")).Clear();
            webDriver.FindElement(By.Id("LastName")).SendKeys(originalLastName);
            webDriver.FindElement(By.Id("userLogin")).Clear();
            webDriver.FindElement(By.Id("userLogin")).SendKeys(originalUserName);
            webDriver.FindElement(By.Id("eMail")).Clear();
            webDriver.FindElement(By.Id("eMail")).SendKeys(originalEmail);
            webDriver.MoveToElement(By.CssSelector("#profileSelector"));
            webDriver.FindElement(By.CssSelector("#save")).Click();
            webDriver.CheckNotification("×\r\nUser details saved successfully.");
            webDriver.CloseNotification();
        }

        public static void AddNewRole(this IWebDriver webDriver, string label)
        {
            webDriver.SelectFromDropDown("[id='addRole']", label);
        }

        public static void RemoveRole(this IWebDriver webDriver)
        {
            webDriver.WaitForElementPresent(By.LinkText("Selenium User"));
            webDriver.IsTextPresent(By.CssSelector("div#permissions > table:first-of-type > thead > tr > th:nth-child(3) > a"), "Selenium User");
            webDriver.IsElementPresent(By.CssSelector("div#permissions > table:first-of-type > thead > tr > th:nth-child(3) > span"));
            var moveToElement = new Actions(webDriver);
            moveToElement.SendKeys(Keys.Down).Perform();
            webDriver.FindElement(By.CssSelector("div#permissions > table:first-of-type > thead > tr > th:nth-child(3) > span")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to remove the role Selenium User from this user?");
            webDriver.IsTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to remove the role Selenium User from this user?");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            Sleep(2000);
        }

        public static void LockUser(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.CssSelector("button#buttonLockUser"));
            bool userLocked = webDriver.FindElement(By.CssSelector("button#buttonLockUser")).Text.Equals("Unlock User");
            if (userLocked == true)
            {
                webDriver.UnlockUser();
                webDriver.IsTextPresent(By.CssSelector("button#buttonLockUser"), "Lock User");
                webDriver.FindElement(By.CssSelector("button#buttonLockUser")).Click();
                webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to lock this user?");
                webDriver.IsTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to lock this user?");
                webDriver.FindElement(By.CssSelector("button.ok")).Click();
                webDriver.WaitForTextPresent(By.CssSelector("button#buttonLockUser"), "Unlock User");
            }
            else
            {
                webDriver.IsTextPresent(By.CssSelector("button#buttonLockUser"), "Lock User");
                webDriver.FindElement(By.CssSelector("button#buttonLockUser")).Click();
                webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to lock this user?");
                webDriver.IsTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to lock this user?");
                webDriver.FindElement(By.CssSelector("button.ok")).Click();
                webDriver.WaitForTextPresent(By.CssSelector("button#buttonLockUser"), "Unlock User");
            }
        }

        public static void UnlockUser(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.CssSelector("button#buttonLockUser"));
            webDriver.IsTextPresent(By.CssSelector("button#buttonLockUser"), "Unlock User");
            webDriver.FindElement(By.CssSelector("button#buttonLockUser")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to unlock this user?");
            webDriver.IsTextPresent(By.CssSelector("#confirm p"), "Are you sure you want to unlock this user?");
            webDriver.FindElement(By.CssSelector("button.ok")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("button#buttonLockUser"), "Lock User");
        }

        public static void GoToRolePermissions(this IWebDriver webDriver, string roleName)
        {
            webDriver.IsElementPresent(By.CssSelector("div#permissions > table:first-of-type > thead > tr > th:nth-child(3) > span"));
            webDriver.IsTextPresent(By.CssSelector("div#permissions > table:first-of-type > thead > tr > th:nth-child(3) > a"), roleName);
            webDriver.FindElement(By.CssSelector("div#permissions > table:first-of-type > thead > tr > th:nth-child(3) > a")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), roleName + " - Permissions");
            webDriver.IsElementPresent(By.Id("searchBox"));
        }

        public static void SearchPermissionInUserProfilePage(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.Id("searchBox"));
            webDriver.FindElement(By.Id("searchBox")).SendKeys("Scheduling");
            webDriver.IsElementPresent(By.CssSelector("div#permissions > table:first-of-type > thead > tr > th:nth-child(3)"));
            webDriver.IsTextPresent(By.CssSelector("div#permissions > table:first-of-type > thead > tr > th:nth-child(3) > a"), "Selenium User");
            webDriver.IsTextPresent(By.CssSelector("td[title='Allows access to the Load and Delivery Schedule screen']"), "Scheduling");
        }

        public static void GoToAuditScreenFromUserProfileScreen(this IWebDriver webDriver, string userName)
        {
            webDriver.IsElementPresent(By.LinkText("Remaining audit records for this user"));
            webDriver.MoveToElement(By.LinkText("Remaining audit records for this user"));
            webDriver.FindElement(By.LinkText("Remaining audit records for this user")).SendKeys(Keys.Down);
            webDriver.FindElement(By.LinkText("Remaining audit records for this user")).Click();
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Business Events Audit");
            var eventBy = webDriver.FindElement(By.Id("EventBy")).GetAttribute("value").ToString();
            Assert.AreEqual(userName, eventBy);
            webDriver.IsElementPresent(By.CssSelector("button.search"));
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
