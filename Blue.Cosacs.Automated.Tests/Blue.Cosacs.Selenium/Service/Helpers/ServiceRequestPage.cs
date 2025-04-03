using System.Threading;
using System;
using OpenQA.Selenium;
using MbUnit.Framework;
using Blue.Selenium;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using Blue.Cosacs.Selenium.Common;
using Branch = System.Configuration.ConfigurationManager;
using System.Collections;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace Blue.Cosacs.Selenium.Service.Helpers
{
    public static class ServiceRequestPage
    {
        public static void CheckNewSRTypes(this IWebDriver webDriver)
        {
            webDriver.IsTextPresent(By.CssSelector("div.newSelector > div:first-child"), "Please select type of the New Service Request.");
            webDriver.IsTextPresent(By.CssSelector("div.newSelector > div.selector > div:nth-child(1)"), "Internal Customer");
            webDriver.IsTextPresent(By.CssSelector("div.newSelector > div.selector > div:nth-child(2)"), "External Customer");
            webDriver.IsTextPresent(By.CssSelector("div.newSelector > div.selector > div:nth-child(3)"), "Stock Repair");
            webDriver.IsTextPresent(By.CssSelector("div.newSelector > div.selector > div:nth-child(4)"), "Internal Installation");
            webDriver.IsTextPresent(By.CssSelector("div.newSelector > div.selector > div:nth-child(5)"), "External Installation");
        }

        public static void IsCreateButtonDisabled(this IWebDriver webDriver)
        {
            Assert.IsFalse(webDriver.FindElement(By.CssSelector(".btn.ng-binding")).Enabled);
        }

        public static void IsCreateButtonEnabled(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".btn.ng-binding")).Enabled);
        }

        public static void IsSearchButtonDisabled(this IWebDriver webDriver)
        {
            Assert.IsFalse(webDriver.FindElement(By.CssSelector(".btn.ng-binding")).Enabled);
        }

        public static void IsSearchButtonEnabled(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".btn.ng-binding")).Enabled);
        }

        public static void ClickSearchButton(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector(".btn.ng-binding")).Click();
            Sleep(500);
        }

        public static void SelectSrType(this IWebDriver webDriver, string srType)
        {
            webDriver.ClickCssSelector(srType);
        }

        public static void SelectStockLocation(this IWebDriver webDriver, string stockLocation)
        {
            webDriver.ScrollElementInToView(By.CssSelector("#s2id_ItemStockLocation"));
            webDriver.FindElement(By.CssSelector("#s2id_ItemStockLocation")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + stockLocation + "']")).Click();
        }

        public static void SelectSearchSelectorOption(this IWebDriver webDriver, string label)
        {
            webDriver.WaitForElementPresent(By.CssSelector("div.selectorSearch > div > div > a.select2-choice > div b"));
            webDriver.FindElement(By.CssSelector("div.selectorSearch > div > div > a.select2-choice > div b")).Click();
            webDriver.WaitForElementPresent(By.XPath("//div/ul/li/div[text()='" + label + "']"));
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + label + "']")).Click();
            new Actions(webDriver).SendKeys(Keys.Tab).Build().Perform();
        }

        public static void TypeSearchSelectorParameter(this IWebDriver webDriver, string searchParameter)
        {
            webDriver.IsElementPresent(By.CssSelector("#customerSearch"));
            webDriver.FindElement(By.CssSelector("#customerSearch")).SendKeys(searchParameter);
        }

        public static void IsSearchResultsSectionDisplayed(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("div.searchResults")).Displayed);
        }

        public static void CheckSearchResults(this IWebDriver webDriver)
        {
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(1)"), "Item Number");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(2)"), "Item Description");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(3)"), "Item Sold By");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(4)"), "Item Delivered On");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(5)"), "Serial Number");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(6)"), "Stock Location");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(7)"), "Supplier");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(8)"), "Amount");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(9)"), "Contract Number");
            webDriver.IsTextPresent(By.CssSelector("div.searchResults > table > thead > tr > th:nth-child(10)"), "Covered By");
        }

        /// <summary>
        /// Use this method it you want deposit to be 0
        /// </summary>
        /// <param name="webDriver"></param>
        public static void CompleteCustomerAndProductSections(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector("input[name=customerId]")).SendKeys("SeleniumTest");
            webDriver.FindElement(By.CssSelector("input[name=CustomerTitle]")).SendKeys("Mr");
            webDriver.FindElement(By.CssSelector("input[name=CustomerFirstName]")).SendKeys("Antony");
            webDriver.FindElement(By.CssSelector("input[name=CustomerLastName]")).SendKeys("James");
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine1]")).SendKeys("Address1");
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine2]")).SendKeys("Address2");
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine3]")).SendKeys("Town");
            webDriver.FindElement(By.CssSelector("input[name=CustomerPostcode]")).SendKeys("Sr12345");
            webDriver.FindElement(By.CssSelector("input[name=contact_0_Value]")).SendKeys("abc");
            webDriver.SelectProductHierarchy("Electrical", "1 - Vision", "10  - Vision");
            webDriver.FindElement(By.CssSelector("input[name=ItemNumber]")).SendKeys("12345");
            webDriver.FindElement(By.CssSelector("input[name=Item]")).SendKeys("Test Item");
            webDriver.FindElement(By.CssSelector("input[name=ItemSupplier]")).SendKeys("Supplier");
            webDriver.SelectManufacturer("Brother");
            webDriver.SelectStockLocation(Branch.AppSettings["Branch3"]);
            webDriver.FindElement(By.CssSelector("input[name=ItemAmount]")).SendKeys("5000");
            webDriver.SaveSR();
        }

        /// <summary>
        /// Use this method if you want to create an external SR with free warranty
        /// </summary>
        /// <param name="webDriver"></param>
        public static void CompleteCustomerAndProductSectionsWithFreeWarranty(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector("input[name=customerId]")).SendKeys("SeleniumTest");
            webDriver.FindElement(By.CssSelector("input[name=CustomerTitle]")).SendKeys("Mr");
            webDriver.FindElement(By.CssSelector("input[name=CustomerFirstName]")).SendKeys("Antony");
            webDriver.FindElement(By.CssSelector("input[name=CustomerLastName]")).SendKeys("James");
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine1]")).SendKeys("Address1");
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine2]")).SendKeys("Address2");
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine3]")).SendKeys("Town");
            webDriver.FindElement(By.CssSelector("input[name=CustomerPostcode]")).SendKeys("Sr12345");
            webDriver.FindElement(By.CssSelector("input[name=contact_0_Value]")).SendKeys("abc");
            webDriver.SelectProductHierarchy("Furniture", "70 - Bedroom", "BW  - Bedroom");
            webDriver.FindElement(By.CssSelector("input[name=ItemNumber]")).SendKeys("12345");
            webDriver.ClickCssSelector("#ItemDeliveredOn");
            webDriver.SelectTodayFromDatePicker();
            webDriver.FindElement(By.CssSelector("input[name=Item]")).SendKeys("Test Item");
            webDriver.FindElement(By.CssSelector("input[name=ItemSupplier]")).SendKeys("Supplier");
            webDriver.SelectManufacturer("Admiral");
            webDriver.SelectStockLocation(Branch.AppSettings["Branch3"]);
            webDriver.FindElement(By.CssSelector("input[name=ItemAmount]")).SendKeys("5000");
            webDriver.CompleteWarrantySectionWithFreeWarranty();
            webDriver.SaveSR();
        }

        public static void CompleteWarrantySectionWithFreeWarranty(this IWebDriver webDriver)
        {
            webDriver.Type(By.CssSelector("#ManufacturerWarrantyContractNo"), "12345");
            webDriver.ClearTextInputField(By.CssSelector("#ManufacturerWarrantyLength"));
            webDriver.Type(By.CssSelector("#ManufacturerWarrantyLength"), "12");
        }

        public static void CompleteWarrantySectionWithExtendedWarranty(this IWebDriver webDriver)
        {
            webDriver.Type(By.CssSelector("#WarrantyContractNo"), DateTime.Now.ToString("ddMMyyyyhhmmss"));
            webDriver.Type(By.CssSelector("#WarrantyLength"), "36");
        }

        public static void CheckCustomerMandatoryFields(this IWebDriver webDriver)
        {
            if (webDriver.FindElement(By.CssSelector("input[name=customerId]")).Text == "")
            {
                webDriver.FindElement(By.CssSelector("input[name=customerId]")).SendKeys("SeleniumTest");
            }
            if (webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine3]")).Text == "")
            {
                webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine3]")).SendKeys("Town");
            }
            if (webDriver.FindElement(By.CssSelector("input[name=CustomerPostcode]")).Text == "")
            {
                webDriver.FindElement(By.CssSelector("input[name=CustomerPostcode]")).SendKeys("Sr12345");
            }
        }

        public static void SaveSR(this IWebDriver webDriver)
        {
            webDriver.CLickButtonByText("button", "Save");
            webDriver.CheckNotification("×\r\nService Request saved successfully."); 
            webDriver.CloseNotification();
        }

        public static void SelectProductHierarchy(this IWebDriver webDriver, string department, string category, string clas)
        {
            webDriver.FindElement(By.CssSelector("ng-include[src='productHierarchy.productDepartmentTemplate']")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + department + "']")).Click();
            webDriver.FindElement(By.CssSelector("ng-include[src='productHierarchy.productCategoryTemplate']")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + category + "']")).Click();
            webDriver.FindElement(By.CssSelector("ng-include[src='productHierarchy.productClassTemplate']")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + clas + "']")).Click();
        }

        public static void SelectManufacturer(this IWebDriver webDriver, string manufacturer)
        {
           
           
            webDriver.ScrollElementInToView(By.CssSelector("#s2id_Manufacturer"));
            webDriver.FindElement(By.CssSelector("#s2id_Manufacturer")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + manufacturer + "']")).Click();
            
        }

        public static string GetSRNumber(this IWebDriver webDriver)
        {
            var url = webDriver.Url;
            var getSRNo = url.Split('/');
            var srNo = getSRNo.GetValue(6).ToString();
            return srNo;
        }

        public static void CompleteEvaluationSection(this IWebDriver webDriver, string serviceEvaluation)
        {
            webDriver.FindElement(By.LinkText("Evaluation")).Click();
            webDriver.FindElement(By.CssSelector("#s2id_ServiceEvaluation")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + serviceEvaluation + "']")).Click();
            webDriver.FindElement(By.CssSelector("#s2id_EvaluationLocation")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='SERVICE']")).Click();
            webDriver.FindElement(By.CssSelector("#s2id_EvaluationAction")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Collected']")).Click();
            webDriver.FindElement(By.CssSelector("#s2id_EvaluationClaimFoodLoss")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='No']")).Click();
            if (serviceEvaluation == "Misuse by the Customer")
            {
                webDriver.Type(By.CssSelector("#ItemSerialNumber"), "Selenium Test");
            }
            webDriver.SaveSR();
        }

        public static void  CompleteAllocationSection(this IWebDriver webDriver, string technician)
        {
            webDriver.FindElement(By.LinkText("Allocation")).Click();
            webDriver.FindElement(By.Name("AllocationItemReceivedOn")).Click();
            webDriver.SelectTodayFromDatePicker();
            Sleep(500);
            webDriver.FindElement(By.CssSelector("[ng-model='techSelect.AllocationServiceScheduledOn']")).Click();
            webDriver.SelectTodayFromDatePicker();
            Sleep(500);
            webDriver.FindElement(By.CssSelector("#s2id_techy")).Click();
            Thread.Sleep(5000);

           
            webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), '" + technician + "')]")).Click();
            
            
           
            Random r = new Random();
            int n = r.Next(1, 40);
            webDriver.ScrollElementInToView(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child("+n+") > td:nth-child(1)"));
            webDriver.FindElement(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child("+n+") > td:nth-child(1)")).Click();
            Thread.Sleep(2000);
            webDriver.SaveSR();
        }

        public static void CompleteAllocationSectionWithRandomDate(this IWebDriver webDriver, string technician)
        {
            webDriver.FindElement(By.LinkText("Allocation")).Click();
            webDriver.FindElement(By.Name("AllocationItemReceivedOn")).Click();
            webDriver.SelectTodayFromDatePicker();
            Sleep(500);
            webDriver.FindElement(By.CssSelector("[ng-model='techSelect.AllocationServiceScheduledOn']")).Click();
            var r = new Random();
            webDriver.SelectDaySpecifiedDateFromDatePicker(r.Next(3, 15));
            Sleep(500);
            webDriver.FindElement(By.CssSelector("#s2id_techy")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), '" + technician + "')]")).Click();
            webDriver.ScrollElementInToView(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1)"));
            webDriver.FindElement(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1)")).Click();
            webDriver.SaveSR();
        }



        public static void CompleteAllocationSectionWithAwaitingParts(this IWebDriver webDriver, string technician)
        {
            webDriver.FindElement(By.LinkText("Allocation")).Click();
            webDriver.FindElement(By.Name("AllocationItemReceivedOn")).Click();
            webDriver.SelectTodayFromDatePicker();
            Sleep(500);
            webDriver.FindElement(By.Name("AllocationPartExpectOn")).Click();
            webDriver.SelectTomorrowFromDatePicker();
            Sleep(500);
            webDriver.FindElement(By.CssSelector("[ng-model='techSelect.AllocationServiceScheduledOn']")).Click();
            webDriver.SelectTomorrowFromDatePicker();
            Sleep(500);
            webDriver.FindElement(By.CssSelector("#s2id_techy")).Click();
            Thread.Sleep(5000);


            webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), '" + technician + "')]")).Click();
            Random r = new Random();
            int n = r.Next(1, 40);
            webDriver.ScrollElementInToView(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(" + n + ") > td:nth-child(1)"));
            webDriver.FindElement(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(" + n + ") > td:nth-child(1)")).Click();
            Thread.Sleep(2000);
            //webDriver.FindElement(By.CssSelector(".fixed.click:nth-child(1)")).Click();
            webDriver.SaveSR();
        }

        public static void AddInternalPart(this IWebDriver webDriver, string internalPart, out string cashPrice)
        {
            if (webDriver.ElementPresent(By.CssSelector(".resolutionPart [ng-click='addResolutionPart()']:last-child")))
            {
                webDriver.ScrollElementInToView(By.CssSelector(".resolutionPart [ng-click='addResolutionPart()']:last-child"));
                webDriver.FindElement(By.CssSelector(".resolutionPart [ng-click='addResolutionPart()']:last-child")).Click();
            }
            else
            {
                webDriver.ScrollElementInToView(By.CssSelector("[ng-click='addResolutionPart()']"));
                webDriver.FindElement(By.CssSelector("[ng-click='addResolutionPart()']")).Click();
            }
            webDriver.ScrollElementInToView(By.CssSelector("[ng-click='searchNewPart()']"));
            webDriver.FindElement(By.CssSelector("[ng-click='searchNewPart()']")).Click();
            Thread.Sleep(5000);
            webDriver.WaitForElementPresent(By.CssSelector(".popup-body.modalStock"));
            webDriver.FindElement(By.CssSelector(".popup-body.modalStock #itemNo")).SendKeys(internalPart);
            webDriver.FindElement(By.CssSelector(".popup-body.modalStock [title='Product Search']")).Click();
            Thread.Sleep(5000);
            webDriver.WaitForElementPresent(By.CssSelector("[ng-repeat='stock in stockResult']:first-child"));
            webDriver.FindElement(By.CssSelector("[ng-repeat='stock in stockResult']:first-child td:first-child")).Click();
            Thread.Sleep(5000);
            webDriver.Type(By.CssSelector("#part-quantity"), "1");
            if (webDriver.ElementPresent(By.CssSelector(".add-part .select2-container.ng-invalid-required")))
            {
                webDriver.SelectPartType("All");
            }
            webDriver.CLickButtonByText("button", "Add Part");
            cashPrice = webDriver.GetText(By.CssSelector(".resolutionPart:last-child .ng-binding:nth-child(5)"));
            cashPrice = cashPrice.TrimStart('$');
        }

        public static void SelectPartSource(this IWebDriver webDriver, string partSource)
        {
            webDriver.ScrollElementInToView(By.CssSelector("#s2id_partSourceDropdown"));
            webDriver.FindElement(By.CssSelector("#s2id_partSourceDropdown")).Click();
            webDriver.FindElement(By.XPath(" //div/ul/li/div[contains(text(), '" + partSource + "')]")).Click();
        }

        public static void SelectPartType(this IWebDriver webDriver, string partType)
        {
            webDriver.ScrollElementInToView(By.CssSelector(".add-part .select2-container.ng-invalid-required"));
            webDriver.FindElement(By.CssSelector(".add-part .select2-container.ng-invalid-required")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), '"+ partType +"')]")).Click();
        }

        public static void AddExternalOrSalvagedPart(this IWebDriver webDriver, string partSource, string costPrice, out string cashPrice)
        {
            if (webDriver.ElementPresent(By.CssSelector(".resolutionPart [ng-click='addResolutionPart()']:last-child")))
            {
                webDriver.ScrollElementInToView(By.CssSelector(".resolutionPart [ng-click='addResolutionPart()']:last-child"));
                webDriver.FindElement(By.CssSelector(".resolutionPart [ng-click='addResolutionPart()']:last-child")).Click();
            }
            else
            {
                webDriver.ScrollElementInToView(By.CssSelector("[ng-click='addResolutionPart()']"));
                webDriver.FindElement(By.CssSelector("[ng-click='addResolutionPart()']")).Click();
            }
      //      webDriver.ScrollElementInToView(By.CssSelector("[ng-click='addResolutionPart()']"));
            webDriver.FindElement(By.CssSelector("[ng-click='addResolutionPart()']")).Click();
            webDriver.Type(By.CssSelector("#part-description"), partSource);
            webDriver.SelectPartSource(partSource);
            webDriver.Type(By.CssSelector("#part-quantity"), "1");
            webDriver.Type(By.CssSelector("#part-price"), costPrice);
            if (webDriver.ElementPresent(By.CssSelector(".add-part .select2-container.ng-invalid-required")))
            {
                webDriver.SelectPartType("All");
            }
            webDriver.CLickButtonByText("button", "Add Part");
            webDriver.SaveSR();
            cashPrice = webDriver.GetText(By.CssSelector(".resolutionPart .ng-binding:nth-child(5)"));
            cashPrice = cashPrice.TrimStart('$');
        }

        public static void AddAditionalAndTransportCost(this IWebDriver webDriver, string additionalCost, string transportCost)
        {
            webDriver.Type(By.CssSelector("[ng-model='serviceRequest.ResolutionAdditionalCost']"), additionalCost);
            webDriver.Type(By.CssSelector("[ng-model='serviceRequest.ResolutionTransportCost']"), transportCost);
        }

        public static void CompleteResolutionSection(this IWebDriver webDriver, string repairType, string primaryChargeTo)
        {
            webDriver.FindElement(By.LinkText("Resolution")).Click();
            webDriver.Type(By.CssSelector("#ItemSerialNumber"), "Selenium Test");
            webDriver.FindElement(By.CssSelector("#s2id_RepairType")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), '" + repairType + "')]")).Click();
            webDriver.FindElement(By.CssSelector("#s2id_Resolution")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Cosmetic Defect']")).Click();
            webDriver.FindElement(By.Name("ResolutionDate")).Click();
            webDriver.SelectTodayFromDatePicker();
            Sleep(500);
            if (string.IsNullOrEmpty(primaryChargeTo))
                return;
            webDriver.FindElement(By.CssSelector("#s2id_resolutionPrimaryCharge")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + primaryChargeTo + "']")).Click();
            if (primaryChargeTo == "Supplier")
            {
                webDriver.FindElement(By.CssSelector("#s2id_ResolutionSupplierToCharge")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), 'Sony')]")).Click();
                webDriver.FindElement(By.CssSelector("#s2id_ResolutionCategory")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), 'Selenium Test 1-12')]")).Click();
            }
            else if (primaryChargeTo == "Deliverer")
            {
                webDriver.FindElement(By.CssSelector("#s2id_ResolutionDelivererToCharge")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Selenium Deliverer']")).Click();
            }
        }

        public static void CompleteResolutionSectionWithAuthorisation(this IWebDriver webDriver, string repairType, string primaryChargeTo)
        {
            webDriver.FindElement(By.LinkText("Resolution")).Click();
            webDriver.Type(By.CssSelector("#ItemSerialNumber"), "Selenium Test");
            webDriver.FindElement(By.CssSelector("#s2id_RepairType")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), '" + repairType + "')]")).Click();
            webDriver.FindElement(By.CssSelector("#s2id_Resolution")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Cosmetic Defect']")).Click();
            webDriver.FindElement(By.Name("ResolutionDate")).Click();
         //   webDriver.SelectTomorrowFromDatePicker();
            webDriver.SelectTodayFromDatePicker();
            Sleep(500);
            if (string.IsNullOrEmpty(primaryChargeTo))
                return;
            webDriver.FindElement(By.CssSelector("#s2id_resolutionPrimaryCharge")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='" + primaryChargeTo + "']")).Click();
            if (primaryChargeTo == "Supplier" && webDriver.FindButtonByText("button", "Authorise").Displayed)
            {
                if (webDriver.FindButtonByText("button", "Authorise").Displayed)
                {
                    webDriver.Type(By.CssSelector("#username"), "99999");
                    webDriver.Type(By.CssSelector("#password"), "ingres##");
                    webDriver.CLickButtonByText("button", "Authorise");
                    Sleep(1000);
                }
                webDriver.FindElement(By.CssSelector("#s2id_ResolutionSupplierToCharge")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), 'Sony')]")).Click();
                webDriver.FindElement(By.CssSelector("#s2id_ResolutionCategory")).Click();
                webDriver.FindElement(By.XPath(" //div/ul/li/div[contains(text(), ' Selenium Test 1-12')]")).Click();
            }
            else if (primaryChargeTo == "Deliverer")
            {
                webDriver.FindElement(By.CssSelector("#s2id_ResolutionDelivererToCharge")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Selenium Deliverer']")).Click();
            }
        }

        public static void FinaliseSR(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.LinkText("Finalise")).Click();
            webDriver.FindElement(By.CssSelector("#s2id_FinalisedFailure")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Cosmetic Defect']")).Click();
            webDriver.FindElement(By.Name("FinaliseReturnDate")).Click();
            webDriver.SelectTodayFromDatePicker();
            webDriver.FindElement(By.LinkText("Comments")).Click();
            webDriver.FindElement(By.CssSelector("textarea[name=Comment]")).SendKeys("These are Selenium test comments");
            Thread.Sleep(5000);

            webDriver.SaveSR();
        }

        private static List<string> sd;

        /// <summary>
        /// Use this method to get random account number.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <returns></returns>
        public static string GetAccountNo(this IWebDriver webDriver)
        {
      //      if (sd == null || sd.Count == 0)
        //    {
                
          //  }

            var accountNo = webDriver.QueryDataBase("SELECT TOP 1 CSV.CustomerAccount FROM Service.CustomerSearchView CSV WHERE CSV.Type is NULL ORDER BY NEWID()", "CustomerAccount");
            return accountNo;
           
            }
           

        /// <summary>
        /// Use this method to get do a custom query and obtain account number with given perameters
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GetAccountNo(this IWebDriver webDriver, string query)
        {
            var accountNo = webDriver.QueryDataBase(query, "CustomerAccount");
            return accountNo;
        }

        public static void MakePayment(this IWebDriver webDriver, string paymentType, string tenderedAmount)
        {
            webDriver.CLickButtonByText("button", "Make Payment");
            Thread.Sleep(1000);
            webDriver.WaitForElementPresent(By.CssSelector(".payMethod"));
            Thread.Sleep(1000);
            webDriver.ProcessServicePayment(paymentType, tenderedAmount);
            Thread.Sleep(1000);
        }

        public static void ProcessServicePayment(this IWebDriver webDriver, string paymentType, string tenderedAmount)
        {
            if (paymentType == "Cash")
            {
                decimal decimalTenderedAmount;
                decimal decimalamountToPay;
                decimal ac;
                webDriver.FindElement(By.CssSelector(".payMethod")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Cash']")).Click();
                var amountToPay = ((IJavaScriptExecutor)webDriver).ExecuteScript("return angular.element(arguments[0]).scope().amount;", webDriver.FindElement(By.CssSelector("[ng-model='amount']")));

           //     webDriver.FindElement(By.CssSelector("250")).Clear();   
                webDriver.FindElement(By.CssSelector("[ng-model='tendered']")).Clear(); //  added suresh
                webDriver.Type(By.CssSelector("[ng-model='tendered']"), tenderedAmount);
                if (!decimal.TryParse(tenderedAmount, System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out decimalTenderedAmount))
                {
                    throw new ArgumentException("TenderedAmount should cast to decimal");
                }
                if (!decimal.TryParse(amountToPay.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out decimalamountToPay))
                {
                    throw new ArgumentException("AmountToPay should cast to decimal");
                }
                var expectedChange = decimalTenderedAmount - decimalamountToPay;
                var actualChange = ((IJavaScriptExecutor)webDriver).ExecuteScript("return angular.element(arguments[0]).scope().calChange();", webDriver.FindElement(By.CssSelector(".payOptions .form-control-static.ng-binding")));
                if (!decimal.TryParse(actualChange.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Threading.Thread.CurrentThread.CurrentCulture, out ac))
                {
                    throw new ArgumentException("Change should cast to decimal");
                }
         //       Assert.AreEqual(expectedChange, ac);
                webDriver.CLickButtonByText("button", "Pay");
                webDriver.CheckNotification("Save Successful.\r\n×\r\nPayment saved successfully.");
        
                webDriver.click_on_cancel_on_pay_pop_up();
                webDriver.CloseNotification();
                
            }
           
           webDriver.SaveSR();
        }

        public static void click_on_cancel_on_pay_pop_up(this IWebDriver webDriver)
        {
            webDriver.WaitForElementPresent(By.XPath(".//*[@id='paymentReceipt-modal']/div/div/div[3]/button[2]"));
            webDriver.FindElement(By.XPath(".//*[@id='paymentReceipt-modal']/div/div/div[3]/button[2]")).Click();
            Thread.Sleep(2000);
        }

        public static void FillRequiredFields(this IWebDriver webDriver)
        {
            if (!webDriver.FindButtonByText("button", "Save").Enabled)
            {
                IList list = webDriver.FindElements(By.CssSelector("input.ng-invalid-required"));
                foreach (IWebElement item in list)
                {
                    if (item.Displayed)
                    {
                        item.SendKeys("Required");
                    }
                }
            }
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
   
    // added -suresh

        public static void CreateServiceRequestForJobData(this IWebDriver webDriver, string customerId, string CustomerTitle,
            
            string FirstName ,string LastName, string Address1, string Address2, string townORcity, string PostCode, string email,
            string department, string category,string clas,string itemNumber, string itemDescription, string Supplier,
            string Manufacturer, string StockLocation,string ItemAmount)
        {
            webDriver.FindElement(By.CssSelector("input[name=customerId]")).SendKeys(customerId);
            webDriver.FindElement(By.CssSelector("input[name=CustomerTitle]")).SendKeys(CustomerTitle);
            webDriver.FindElement(By.CssSelector("input[name=CustomerFirstName]")).SendKeys(FirstName);
            webDriver.FindElement(By.CssSelector("input[name=CustomerLastName]")).SendKeys(LastName);
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine1]")).SendKeys(Address1);
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine2]")).SendKeys(Address2);
            webDriver.FindElement(By.CssSelector("input[name=CustomerAddressLine3]")).SendKeys(townORcity);
            webDriver.FindElement(By.CssSelector("input[name=CustomerPostcode]")).SendKeys(PostCode);
            webDriver.FindElement(By.CssSelector("input[name=contact_0_Value]")).SendKeys(email);
            webDriver.SelectProductHierarchy(department, category, clas);
            webDriver.FindElement(By.CssSelector("input[name=ItemNumber]")).SendKeys(itemNumber);
      //      webDriver.ClickCssSelector("#ItemDeliveredOn");
     //       webDriver.SelectTodayFromDatePicker();
            webDriver.FindElement(By.CssSelector("input[name=Item]")).SendKeys(itemDescription);
            webDriver.FindElement(By.CssSelector("input[name=ItemSupplier]")).SendKeys(Supplier);
            webDriver.SelectManufacturer(Manufacturer);
            webDriver.SelectStockLocation(StockLocation);
            webDriver.FindElement(By.CssSelector("input[name=ItemAmount]")).SendKeys(ItemAmount);
            webDriver.SaveSR();
        }

        public static void DeleteCreatedJob(this IWebDriver webDriver)
        {
            webDriver.ScrollElementInToView(By.CssSelector(".halflings.trash.click"));
            webDriver.FindElement(By.CssSelector(".halflings.trash.click")).Click();
            webDriver.IsElementPresent(By.CssSelector(".modal-header"));
            webDriver.FindElement(By.Id("s2id_autogen36")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Technician taking too long']")).Click();
            Thread.Sleep(1000);
            webDriver.FindElement(By.XPath("//*[@id='dialogueCommon']/div/div/div[3]/button[1]")).Click();
            webDriver.FindElement(By.XPath("//*[@id='navBar']/button")).Click();            
        }

        public static void CompleteAllocationSection2(this IWebDriver webDriver, string technician)
        {
            webDriver.FindElement(By.LinkText("Allocation")).Click();
            webDriver.FindElement(By.Name("AllocationItemReceivedOn")).Click();
            webDriver.SelectTodayFromDatePicker();
            Sleep(500);
            webDriver.FindElement(By.CssSelector("[ng-model='techSelect.AllocationServiceScheduledOn']")).Click();
            webDriver.SelectDayAfterTomorrowFromDatePicker();
            Sleep(500);
            webDriver.FindElement(By.CssSelector("#s2id_techy")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), '" + technician + "')]")).Click();
            webDriver.ScrollElementInToView(By.CssSelector(".fixed.click:nth-child(1)"));
            webDriver.FindElement(By.CssSelector(".fixed.click:nth-child(1)")).Click();
            webDriver.SaveSR();
        }

        public static string GetStatus(this IWebDriver webDriver)
        {
            var status = webDriver.FindElement(By.CssSelector("#service-heading > span:nth-child(2)")).Text;
            return status;

        }

        public static void IsSaveButtonDisabled(this IWebDriver webDriver)
        {
            Assert.IsFalse(webDriver.FindElement(By.CssSelector(".btn.btn-primary")).Enabled);
        }

       

        public static void SendingDataToTheSupplierContractualCostsRows(this IWebDriver webDriver, string Product, string Month,string PartType,string Partpercentage,
                                                                        string PartValue, string LabourPercentage,string LabourValue,string AdditionalPercentage,string AdditionalValue)

            
        {
            
        }


        public static void IsSaveButtonEnabled(this IWebDriver webDriver)
        {

            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".btn.btn-primary")).Enabled);
        }

        public static void SelectMonth(IWebDriver webDriver,string Month)
        {
           


        }

             
    


    
    }

}
