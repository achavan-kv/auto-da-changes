using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class DriversTests
    {
        [Test]
        public void CreateNewDriverAndDeleteHim()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                var driverName = string.Empty;
                webDriver.CreateNewDriver(out driverName);
                webDriver.DeleteDriver(driverName);
            }
        }
        
        [Test]
        public void EditDriver()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                var driverName = string.Empty;
                webDriver.CreateNewDriver(out driverName);
                webDriver.EditDriver(driverName, "NewSeleniumDriver22");
                webDriver.DeleteDriver("NewSeleniumDriver22");
            }
        }

        [Test]
        public void CreateDriverWithOutAllRequiredFields()
        {
            using (var session=Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsElementPresent(By.CssSelector("a[title='New']"));
                webDriver.FindElement(By.CssSelector("a[title='New']")).Click();
                webDriver.ScrollToEndOfPage();
                System.Threading.Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] a[title='Save']"));
                webDriver.FindElement(By.CssSelector("tr[data-id='0'] a[title='Save']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#Name.input-validation-error"));
                webDriver.IsElementPresent(By.CssSelector("#PhoneNumber.input-validation-error"));
            }
        }

        [Test]
        public void CreateDriverWithOutPhoneNumber()
        {
            using (var session=Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsElementPresent(By.CssSelector("a[title='New']"));
                webDriver.FindElement(By.CssSelector("a[title='New']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.ScrollToEndOfPage();
                webDriver.FindElement(By.CssSelector("tr[data-id='0'] #Name")).SendKeys("DriverName");
                webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] a[title='Save']"));
                webDriver.FindElement(By.CssSelector("tr[data-id='0'] a[title='Save']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#PhoneNumber.input-validation-error"));
                webDriver.IsElementNotPresent(By.CssSelector("#Name.input-validation-error"));
            }
        }

        [Test]
        public void CreateDriverWithOutName()
        {
            using (var session=Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.ScrollToEndOfPage();
                webDriver.IsElementPresent(By.CssSelector("a[title='New']"));
                webDriver.FindElement(By.CssSelector("a[title='New']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.ScrollToEndOfPage();
                webDriver.FindElement(By.CssSelector("tr[data-id='0'] #PhoneNumber")).SendKeys("12345");
                webDriver.IsElementPresent(By.CssSelector("tr[data-id='0'] a[title='Save']"));
                webDriver.FindElement(By.CssSelector("tr[data-id='0'] a[title='Save']")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#Name.input-validation-error"));
                webDriver.IsElementNotPresent(By.CssSelector("#PhoneNumber.input-validation-error"));
            }
        }
    }
}
