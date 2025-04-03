using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;
using System;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Branch = System.Configuration.ConfigurationManager;

namespace Blue.Cosacs.Selenium.Administration
{
    [TestFixture]
    public class CreateUserTests
    {
        [Test]
        public void CreateNewUser()
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
                webDriver.CheckNewUserDetails(firstName, lastName, userName);
                webDriver.LockUser();
            }
        }

        [Test]
        public void CreateUserWithOutAllRequiredFields()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                webDriver.ClickSubmitInCreateUserPage();
                webDriver.CheckValidationSummaryInCreateUserPage();
            }
        }

        [Test]
        public void CreateUserWithOutFirstName()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                webDriver.FindElement(By.Id("LastName")).SendKeys("LastName");
                webDriver.FindElement(By.Id("Login")).SendKeys("UserName");
                webDriver.FindElement(By.Id("eMail")).SendKeys("delete@bluebridgeltd.com");
                webDriver.FindElement(By.Id("ExternalLogin")).SendKeys("ExternalDirectoryLogin");
                webDriver.SelectBranchNameForNewUser(Branch.AppSettings["Branch1"]);
                webDriver.FindElement(By.Id("Password")).SendKeys("test**123");
                webDriver.ClickSubmitInCreateUserPage();
                webDriver.IsElementPresent(By.CssSelector("div.createUser > div.has-error:nth-child(1) > div > input.input-validation-error"));
            }
        }

        [Test]
        public void CreateUserWithOutLastName()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                webDriver.FindElement(By.Id("FirstName")).SendKeys("FirstName");
                webDriver.FindElement(By.Id("Login")).SendKeys("UserName");
                webDriver.FindElement(By.Id("eMail")).SendKeys("delete@bluebridgeltd.com");
                webDriver.FindElement(By.Id("ExternalLogin")).SendKeys("ExternalDirectoryLogin");
                webDriver.SelectBranchNameForNewUser(Branch.AppSettings["Branch12"]);
                webDriver.FindElement(By.Id("Password")).SendKeys("test**123");
                webDriver.ClickSubmitInCreateUserPage();
                webDriver.IsElementPresent(By.CssSelector("div.createUser > div.has-error:nth-child(2) > div > input.input-validation-error"));
            }
        }

        [Test]
        public void CreateUserWithOutUserName()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                webDriver.FindElement(By.Id("FirstName")).SendKeys("FirstName");
                webDriver.FindElement(By.Id("LastName")).SendKeys("LastName");
                webDriver.FindElement(By.Id("eMail")).SendKeys("delete@bluebridgeltd.com");
                webDriver.FindElement(By.Id("ExternalLogin")).SendKeys("ExternalDirectoryLogin");
                webDriver.SelectBranchNameForNewUser(Branch.AppSettings["Branch13"]);
                webDriver.FindElement(By.Id("Password")).SendKeys("test**123");
                webDriver.ClickSubmitInCreateUserPage();
                webDriver.IsTextPresent(By.CssSelector("div.createUser > div:nth-child(3) > div span[data-valmsg-for]"), "Login Name is required");
            }
        }

        [Test]
        public void CreateUserWithOutBranchName()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                webDriver.FindElement(By.Id("FirstName")).SendKeys("FirstName");
                webDriver.FindElement(By.Id("LastName")).SendKeys("LastName");
                webDriver.FindElement(By.Id("Login")).SendKeys("UserName");
                webDriver.FindElement(By.Id("eMail")).SendKeys("delete@bluebridgeltd.com");
                webDriver.FindElement(By.Id("ExternalLogin")).SendKeys("ExternalDirectoryLogin");
                webDriver.FindElement(By.Id("Password")).SendKeys("test**123");
                webDriver.ClickSubmitInCreateUserPage();
                webDriver.IsElementPresent(By.CssSelector("div.createUser > div:nth-child(7) > div > div.pickListInputValidationClass.input-validation-error"));
            }
        }

        [Test]
        public void CreateUserWithOutDefaultPassword()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                webDriver.FindElement(By.Id("FirstName")).SendKeys("FirstName");
                webDriver.FindElement(By.Id("LastName")).SendKeys("LastName");
                webDriver.FindElement(By.Id("Login")).SendKeys("UserName");
                webDriver.FindElement(By.Id("eMail")).SendKeys("delete@bluebridgeltd.com");
                webDriver.FindElement(By.Id("ExternalLogin")).SendKeys("ExternalDirectoryLogin");
                webDriver.SelectBranchNameForNewUser(Branch.AppSettings["Branch14"]);
                webDriver.ClickSubmitInCreateUserPage();
                webDriver.IsTextPresent(By.CssSelector("div.createUser > div.has-error:nth-child(8) > div span[data-valmsg-for] span"), "Password is required");
            }
        }

        [Test]
        public void CheckLabels()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Create User", "Admin/Users/Create", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Create New User");
                webDriver.CheckLabelNamesInCreateUserPage();
            }
        }
    }
}
