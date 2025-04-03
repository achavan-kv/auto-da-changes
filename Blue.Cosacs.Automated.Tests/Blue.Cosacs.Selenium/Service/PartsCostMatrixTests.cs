using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;

namespace Blue.Cosacs.Selenium.Service
{
    public class PartsCostMatrixTests
    {
        [Test]
        public void CreatePartsCostMatrixForProductGroup()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "Parts Cost Matrix", "Service/Parts", session);
                webDriver.WaitForElementPresent(By.CssSelector("#searchName"));
                Thread.Sleep(2000);
                webDriver.ScrollToEndOfPage();
                webDriver.WaitForElementPresent(By.CssSelector("[title='New']"));
                webDriver.ClickCssSelector("[title='New']");
                webDriver.ScrollToEndOfPage();
                webDriver.WaitForElementPresent(By.CssSelector("[ng-model='productItem.Label']"));
                Assert.IsTrue(webDriver.FindElement(By.CssSelector(".create [title='Save']")).GetAttribute("class").Contains("disabled"));
                var label = "Selenium Test " + DateTime.Now.ToString("ddMMyyyyhhmmss");
                webDriver.Type(By.CssSelector("[ng-model='productItem.Label']"), label);
                webDriver.Type(By.CssSelector("[ng-model='productItem.Label']"), Keys.Tab);
                webDriver.SelectProductHierarchyInMatrix("Other", "76 - Category76 ", "LO  - Category76 ");
                webDriver.SelectSupplierInPartsMatrixForProductGroup("HP");
                webDriver.SelectRepairTypeInPartsMatrixForProductGroup("Assessment");
                webDriver.FillPartsMarkup("10", "20", "30", "40");
                Assert.IsFalse(webDriver.FindElement(By.CssSelector(".create [title='Save']")).GetAttribute("class").Contains("disabled"));
                webDriver.SaveMatrix();
            }
        }

        [Test]
        public void CreatePartsCostMatrixForSpecificProducts()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "Parts Cost Matrix", "Service/Parts", session);
                webDriver.WaitForElementPresent(By.CssSelector("[ng-model='labourItem.Label']"));
                Thread.Sleep(2000);
                webDriver.ScrollToEndOfPage();
                webDriver.ClickCssSelector("[title='New']");
                webDriver.ScrollToEndOfPage();
                webDriver.WaitForElementPresent(By.CssSelector("[ng-model='productItem.Label']"));
                Assert.IsTrue(webDriver.FindElement(By.CssSelector(".create [title='Save']")).GetAttribute("class").Contains("disabled"));
                var label = "Selenium Test " + DateTime.Now.ToString("ddMMyyyyhhmmss");
                webDriver.Type(By.CssSelector("[ng-model='productItem.Label']"), label);
                webDriver.ClickCssSelector("#product");
                webDriver.Type(By.CssSelector("[ng-model='productItem.ItemList']"), "203119");
                webDriver.Type(By.CssSelector("[ng-model='productItem.ItemList']"), Keys.Tab);
                webDriver.SelectRepairTypeInPartsMatrixForSpecificProducts("Assessment");
                webDriver.FillPartsMarkup("10", "20", "30", "40");
                Assert.IsFalse(webDriver.FindElement(By.CssSelector(".create [title='Save']")).GetAttribute("class").Contains("disabled"));
                webDriver.SaveMatrix();
            }
        }
    }
}
