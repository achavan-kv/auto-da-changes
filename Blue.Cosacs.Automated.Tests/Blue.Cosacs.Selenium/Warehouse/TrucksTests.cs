using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Branch = System.Configuration.ConfigurationManager;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class TrucksTests
    {
        [Test]
        public void CreateNewTruckAndDeleteIt()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                var branch = string.Empty;
                var driver = string.Empty;
                webDriver.CreateNewTruck("SeleniumTruck17", out branch, out driver);
                webDriver.DeleteTruck("SeleniumTruck17", branch, driver);
            }
        }

        [Test]
        public void EditTruck()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                var branch = string.Empty;
                var driver = string.Empty;
                var newBranch = string.Empty;
                var newDriver = string.Empty;
                webDriver.CreateNewTruck("SeleniumTruck18", out branch, out driver);
                webDriver.EditTruck("SeleniumTruck18", "NewSeleniumTruck19", branch, driver, out newBranch, out newDriver);
                webDriver.DeleteTruck("NewSeleniumTruck19", newBranch, newDriver);
            }
        }

        [Test]
        public void CreateTruckWithOutAllRequiredFields()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsElementPresent(By.CssSelector("[title='New']"));
                webDriver.FindElement(By.CssSelector("[title='New']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.ScrollToEndOfPage();
                webDriver.IsElementPresent(By.CssSelector("[data-id='0'] [title='Save']"));
                webDriver.FindElement(By.CssSelector("[data-id='0'] [title='Save']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#Name.input-validation-error"));
                webDriver.IsElementPresent(By.CssSelector("#Branch.input-validation-error"));
                webDriver.IsElementPresent(By.CssSelector("#DriverId.input-validation-error"));
            }
        }

        [Test]
        public void CreateTruckWithOutBranchAndDriver()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsElementPresent(By.CssSelector("[title='New']"));
                webDriver.FindElement(By.CssSelector("[title='New']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.ScrollToEndOfPage();
                webDriver.FindElement(By.CssSelector("[data-id='0'] #Name")).SendKeys("TruckName");
                webDriver.IsElementPresent(By.CssSelector("[data-id='0'] [title='Save']"));
                webDriver.FindElement(By.CssSelector("[data-id='0'] [title='Save']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#Branch.input-validation-error"));
                webDriver.IsElementPresent(By.CssSelector("#DriverId.input-validation-error"));
            }
        }

        [Test]
        public void CreateTruckWithOutNameAndDriver()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsElementPresent(By.CssSelector("[title='New']"));
                webDriver.FindElement(By.CssSelector("[title='New']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.ScrollToEndOfPage();
                webDriver.SelectFromDropDown("[data-id='0'] .row:nth-child(2) .chzn-done", Branch.AppSettings["Branch8"]);
                webDriver.IsElementPresent(By.CssSelector("[data-id='0'] [title='Save']"));
                webDriver.FindElement(By.CssSelector("[data-id='0'] [title='Save']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#Name.input-validation-error"));
                webDriver.IsElementPresent(By.CssSelector("#DriverId.input-validation-error"));
            }
        }

        [Test]
        public void CreateTruckWithOutNameAndBranch()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsElementPresent(By.CssSelector("[title='New']"));
                webDriver.FindElement(By.CssSelector("[title='New']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.ScrollToEndOfPage();
                webDriver.SelectFromDropDown("[data-id='0'] .row:nth-child(3) .chzn-done", "SeleniumDriver8");
                webDriver.IsElementPresent(By.CssSelector("[data-id='0'] [title='Save']"));
                webDriver.FindElement(By.CssSelector("[data-id='0'] [title='Save']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#Name.input-validation-error"));
                webDriver.IsElementPresent(By.CssSelector("#Branch.input-validation-error"));
            }
        }
    }
}
