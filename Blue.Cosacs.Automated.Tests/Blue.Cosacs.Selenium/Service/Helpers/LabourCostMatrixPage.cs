using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Service.Helpers
{
    public static class LabourAndPartsMatrixPage
    {
        public static void SelectSupplierInLabourMatrixForProductGroup(this IWebDriver webDriver, string supplier)
        {
            webDriver.ClickCssSelector(".create [ng-show='productItem.IsGroupFilter'] .row:nth-child(2) .select2-container.ng-valid.ng-dirty");
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + supplier + "']")).Click();
        }

        public static void SelectSupplierInLabourMatrixForSpecificProducts(this IWebDriver webDriver, string supplier)
        {
            webDriver.ClickCssSelector(".create [ng-show='!productItem.IsGroupFilter'] .select2-container.ng-valid.ng-dirty");
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + supplier + "']")).Click();
        }

        public static void SelectRepairTypeInLabourMatrixForProductGroup(this IWebDriver webDriver, string repairType)
        {
            webDriver.ClickCssSelector(".create [ng-show='productItem.IsGroupFilter'] .row:nth-child(2) .select2-container.ng-invalid-required");
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + repairType + "']")).Click();
        }

        public static void SelectRepairTypeInLabourMatrixForSpecificProducts(this IWebDriver webDriver, string repairType)
        {
            webDriver.ClickCssSelector(".create [ng-show='!productItem.IsGroupFilter'] .select2-container.ng-invalid-required");
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + repairType + "']")).Click();
        }

        public static void FillLabourCharges(this IWebDriver webDriver, string internalTechnician, string contractedTechnician, string ew, string cusomer)
        {
            webDriver.Type(By.CssSelector("[name='ChargeInternalTech']"), internalTechnician);
            webDriver.Type(By.CssSelector("[name='ChargesContractedTech']"), contractedTechnician);
            webDriver.Type(By.CssSelector("[name='ChargesEWClaim']"), ew);
            webDriver.Type(By.CssSelector("[name='ChargesCustomer']"), cusomer);
        }

        public static void SaveMatrix(this IWebDriver webDriver)
        {
            webDriver.ClickCssSelector(".create [title='Save']");
            webDriver.CheckNotification("×\r\nRecord saved successfully.");
            webDriver.CloseNotification();
        }

        public static void SelectProductHierarchyInMatrix(this IWebDriver webDriver, string department, string category, string clas)
        {
            webDriver.FindElement(By.CssSelector(".create ng-include[src='productHierarchy.productDepartmentTemplate']")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + department + "']")).Click();
            webDriver.FindElement(By.CssSelector(".create ng-include[src='productHierarchy.productCategoryTemplate']")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + category + "']")).Click();
            webDriver.FindElement(By.CssSelector(".create ng-include[src='productHierarchy.productClassTemplate']")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + clas + "']")).Click();
        }
    }
}
