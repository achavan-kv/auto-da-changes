using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;
using ChargeTo = Blue.Cosacs.Selenium.Service.Helpers.ServiceCharges.ChargeTos;

namespace Blue.Cosacs.Selenium.Service
{
    [TestFixture]
    public class ServiceChargesTests
    {
   //   [Test]
        public void RepairChargedToCustomer()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#si"));
                webDriver.SelectSrType("#si");
                webDriver.IsSearchButtonDisabled();
       //         webDriver.TypeSearchSelectorParameter(webDriver.GetAccountNo(@"SELECT TOP 1 CSV.CUSTOMERACCOUNT FROM SERVICE.CUSTOMERSEARCHVIEW CSV 
           //                                                                    WHERE CSV.TYPE IS NULL AND CSV.ITEMDELIVEREDON = '2012-12-19' AND WARRANTYCONTRACTNO IS NULL ORDER BY NEWID()"));
                
                        webDriver.TypeSearchSelectorParameter(webDriver.GetAccountNo(@"select top 1 acctno from delivery where delorcoll='C' and datedel between '2013-07-11 00:00:00' and '2014-07-11 00:00:00'
                                                                                        and itemno in (select itemno from stockitem where category not in ('12','82'))"));
                
                webDriver.IsSearchButtonEnabled();
                webDriver.CLickButtonByText("button", "Search");
                webDriver.WaitForElementPresent(By.CssSelector("div.searchResults"));
                webDriver.IsSearchResultsSectionDisplayed();
                webDriver.CLickButtonByText("btn", "Select");
                webDriver.SelectManufacturer("Brother");
                webDriver.FillRequiredFields();
                webDriver.SaveSR();
                var srNo = webDriver.GetSRNumber();
                var internalPartCashPrice = string.Empty;
                var externalPartCashPrice = string.Empty;
                Thread.Sleep(5000);
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                Thread.Sleep(2000);
                webDriver.MakePayment("Cash", "250");
                webDriver.CompleteEvaluationSection("Misuse by the Customer");
                webDriver.CompleteAllocationSection("Selenium Technician External");
                webDriver.CompleteResolutionSection("Major", string.Empty);
                webDriver.AddExternalOrSalvagedPart("External", "45", out externalPartCashPrice);
                webDriver.AddInternalPart("712348", out internalPartCashPrice);
                webDriver.SaveSR();


                webDriver.AssertCharges(externalPartCashPrice, "2", ChargeTo.Customer);
                webDriver.AssertCharges(internalPartCashPrice, "1", ChargeTo.Customer);
                webDriver.AssertCharges("250.00", "3", ChargeTo.Customer);
                webDriver.AssertTotalCost("30.00", "56.25", "250.00", "0", "0", "336.25");
            }
        }

  //     [Test]
        public void RepairChargedToSupplierWithinFYWPeriod()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#si"));
                webDriver.SelectSrType("#si");
                webDriver.IsSearchButtonDisabled();
                webDriver.TypeSearchSelectorParameter(webDriver.GetAccountNo(@"SELECT TOP 1 CSV.CUSTOMERACCOUNT FROM SERVICE.CUSTOMERSEARCHVIEW CSV 
                                                                               WHERE CSV.TYPE IS NULL AND CSV.ITEMDELIVEREDON = '2013-06-19' AND WARRANTYCONTRACTNO IS NOT NULL ORDER BY NEWID()"));
                webDriver.IsSearchButtonEnabled();
                webDriver.CLickButtonByText("button", "Search");
                webDriver.WaitForElementPresent(By.CssSelector("div.searchResults"));
                webDriver.IsSearchResultsSectionDisplayed();
                webDriver.CLickButtonByText("btn", "Select");
                webDriver.SelectManufacturer("Brother");
                webDriver.FillRequiredFields();
                webDriver.SaveSR();
                var srNo = webDriver.GetSRNumber();
                var internalPartCashPrice = string.Empty;
                webDriver.CompleteEvaluationSection("Warranty Covered");
                webDriver.CompleteAllocationSection("Selenium Technician Internal");
                webDriver.CompleteResolutionSection("Major", "Supplier");
                webDriver.AddInternalPart("712348", out internalPartCashPrice);
                webDriver.SaveSR();
                webDriver.AssertCharges("22.00", "1", ChargeTo.Supplier);
                webDriver.AssertCharges("28.00", "2", ChargeTo.Supplier);
                webDriver.AssertCharges("72.00", "2", ChargeTo.FYW);
                webDriver.AssertTotalCost("22.00", "0", "100.00", "0", "0", "122.00");
            }
        }

//  [Test]
        public void RepairChargedToSupplierNonCourtsSale()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#si"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSectionsWithFreeWarranty();
                var srNo = webDriver.GetSRNumber();
                var internalPartCashPrice = string.Empty;
                webDriver.CompleteEvaluationSection("Warranty Covered");
                webDriver.CompleteAllocationSection("Selenium Techncian Internal");
                webDriver.CompleteResolutionSectionWithAuthorisation("Major", "Supplier");
                webDriver.AddInternalPart("712348", out internalPartCashPrice);
                webDriver.SaveSR();
                webDriver.AssertCharges("6.94", "1", ChargeTo.Supplier);
                webDriver.AssertCharges("0.00", "2", ChargeTo.Supplier);
                webDriver.AssertCharges("250.00", "2", ChargeTo.Customer);
                webDriver.AssertTotalCost("6.94", "0", "250.00", "0", "37.50", "294.44");
            }
        }

//        [Test]
        public void RepairChargedToSupplierWithinEWPeriod()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#si"));
                webDriver.SelectSrType("#si");
                webDriver.IsSearchButtonDisabled();
                webDriver.TypeSearchSelectorParameter(webDriver.GetAccountNo(@"SELECT TOP 1 CSV.CUSTOMERACCOUNT FROM SERVICE.CUSTOMERSEARCHVIEW CSV 
                                                                               WHERE CSV.TYPE IS NULL AND CSV.ITEMDELIVEREDON = '2012-06-01' AND WARRANTYCONTRACTNO IS NOT NULL ORDER BY NEWID()"));
                webDriver.IsSearchButtonEnabled();
                webDriver.CLickButtonByText("button", "Search");
                webDriver.WaitForElementPresent(By.CssSelector("div.searchResults"));
                webDriver.IsSearchResultsSectionDisplayed();
                webDriver.CLickButtonByText("btn", "Select");
                webDriver.SelectManufacturer("Brother");
                webDriver.FillRequiredFields();
                webDriver.SaveSR();
                var srNo = webDriver.GetSRNumber();
                var externalPartCashPrice = string.Empty;
                var internalPartCashPrice = string.Empty;
                webDriver.CompleteEvaluationSection("Warranty Covered");
                webDriver.CompleteAllocationSection("Selenium Technician External");
                webDriver.CompleteResolutionSection("Major", "Supplier");
                webDriver.AddExternalOrSalvagedPart("External", "60", out externalPartCashPrice);
                webDriver.AddInternalPart("712348", out internalPartCashPrice);
                webDriver.SaveSR();
                webDriver.AssertCharges("24.00", "1", ChargeTo.EW);
                webDriver.AssertCharges("75.00", "2", ChargeTo.EW);
                webDriver.AssertCharges("72.00", "3", ChargeTo.EW);
                webDriver.AssertCharges("45.00", "2", ChargeTo.FYW);
                webDriver.AssertTotalCost("24.00", "72.00", "120.00", "0", "0", "216.00");
            }
        }
    }
}
