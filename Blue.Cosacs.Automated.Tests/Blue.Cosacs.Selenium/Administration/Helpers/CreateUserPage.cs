using MbUnit.Framework;
using OpenQA.Selenium;
using System;
using Blue.Selenium;
using Branch = System.Configuration.ConfigurationManager;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Administration.Helpers
{
    public static class CreateUserPage
    {
        public static void SelectBranchNameForNewUser(this IWebDriver webDriver, string label)
        {
            webDriver.SelectFromDropDown(".chzn-done", label);
        }

        public static void CreateUser(this IWebDriver webDriver, out string firstName, out string lastName, out string userName)
        {
            firstName = DateTime.Now.ToString("ddMMyyyyhhmmssff");
            lastName = DateTime.Now.ToString("ddMMyyyyhhmmssfff");
            userName = DateTime.Now.ToString("ddMMyyyyhhmmssfffff");
            webDriver.FindElement(By.Id("FirstName")).SendKeys(firstName);
            webDriver.FindElement(By.Id("LastName")).SendKeys(lastName);
            webDriver.FindElement(By.Id("Login")).SendKeys(userName);
            webDriver.FindElement(By.Id("eMail")).SendKeys("delete@bluebridgeltd.com");
            webDriver.SelectBranchNameForNewUser(Branch.AppSettings["Branch14"]);
            webDriver.FindElement(By.Id("Password")).SendKeys("test**123");
            webDriver.ClickSubmitInCreateUserPage();
        }

        public static void CreateUser(this IWebDriver webDriver, string firstName, out string lastName, out string userName)
        {
            lastName = DateTime.Now.ToString("ddMMyyyyhhmmssfff");
            userName = DateTime.Now.ToString("ddMMyyyyhhmmssfffff");
            webDriver.FindElement(By.Id("FirstName")).SendKeys(firstName);
            webDriver.FindElement(By.Id("LastName")).SendKeys(lastName);
            webDriver.FindElement(By.Id("Login")).SendKeys(userName);
            webDriver.FindElement(By.Id("eMail")).SendKeys("delete@bluebridgeltd.com");
            webDriver.SelectBranchNameForNewUser(Branch.AppSettings["Branch14"]);
            webDriver.FindElement(By.Id("Password")).SendKeys("test**123");
            webDriver.ClickSubmitInCreateUserPage();
        }

        public static void ClickSubmitInCreateUserPage(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.Name("submit"));
            webDriver.FindElement(By.Name("submit")).Click();
            Sleep(1000);
        }

        public static void CheckNewUserDetails(this IWebDriver webDriver, string firstName, string lastName, string userName)
        {
            webDriver.CheckFirstName(firstName);
            webDriver.CheckLastName(lastName);
            webDriver.CheckUserName(userName);
            webDriver.CheckEmail("delete@bluebridgeltd.com");
            webDriver.CheckBranchName("COURTS 960");
        }

        public static void CheckValidationSummaryInCreateUserPage(this IWebDriver webDriver)
        {
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div.has-error:nth-child(3) > div span[data-valmsg-for]"), "Login Name is required");
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div.has-error:nth-child(8) > div span[data-valmsg-for] span"), "Password is required");
        }

        public static void CheckLabelNamesInCreateUserPage(this IWebDriver webDriver)
        {
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(1) > label"), "First Name");
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(2) > label"), "Last Name");
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(3) > label"), "User Name");
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(4) > label"), "Email");
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(5) > label"), "External Directory Login");
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(6) > label"), "Fact Employee Id");
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(7) > label"), "Branch Name");
            webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(8) > label"), "Default Password");
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
