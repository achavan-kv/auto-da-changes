using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using System.Threading;

namespace Blue.Cosacs.Selenium.Service.Helpers
{
    public static class PartsCostMatrixPage
    {
        public static void SelectSupplierInPartsMatrixForProductGroup(this IWebDriver webDriver, string supplier)
        {
            webDriver.ClickCssSelector("[ng-show='productItem.IsGroupFilter'] [ng-model='productItem.Supplier'] .list-container");
            webDriver.ScrollToEndOfPage();
            Thread.Sleep(500);
            webDriver.FindElement(By.XPath("//div[text()='" + supplier + "']")).Click();
        }

        public static void SelectSupplierInPartsMatrixForSpecificProducts(this IWebDriver webDriver, string supplier)
        {
            webDriver.ClickCssSelector("[ng-show='!productItem.IsGroupFilter'] [ng-model='productItem.Supplier'] .list-container");
            webDriver.ScrollToEndOfPage();
            Thread.Sleep(500);
            webDriver.FindElement(By.XPath("//div[text()='" + supplier + "']")).Click();
        }

        public static void SelectRepairTypeInPartsMatrixForProductGroup(this IWebDriver webDriver, string repairType)
        {
            webDriver.ClickCssSelector("[ng-show='productItem.IsGroupFilter'] [ng-model='productItem.RepairType'] .list-container");
            webDriver.ScrollToEndOfPage();
            Thread.Sleep(500);
            webDriver.FindElement(By.XPath("//div[text()='" + repairType + "']")).Click();
        }

        public static void SelectRepairTypeInPartsMatrixForSpecificProducts(this IWebDriver webDriver, string repairType)
        {
            webDriver.ClickCssSelector("[ng-show='!productItem.IsGroupFilter'] [ng-model='productItem.RepairType'] .list-container");
            webDriver.ScrollToEndOfPage();
            Thread.Sleep(500);
            webDriver.FindElement(By.XPath("//div[text()='" + repairType + "']")).Click();
        }

        public static void FillPartsMarkup(this IWebDriver webDriver, string internalMarkup, string fywMarkup, string ewMarkup, string cusomerMarkup)
        {
            webDriver.Type(By.CssSelector("[name='newRecordChargeInternal']"), internalMarkup);
            webDriver.Type(By.CssSelector("[name='newRecordChargeFirstYearWarranty']"), fywMarkup);
            webDriver.Type(By.CssSelector("[name='newRecordChargeExtendedWarranty']"), ewMarkup);
            webDriver.Type(By.CssSelector("[name='newRecordChargeCustomer']"), cusomerMarkup);
        }
    }
}
