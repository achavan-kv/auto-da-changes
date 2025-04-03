using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;
using OpenQA.Selenium.Interactions;
using Branch = System.Configuration.ConfigurationManager;

namespace Blue.Cosacs.Selenium.Service
{
    [TestFixture]
    public class ServiceRequestStatusTests
    {
        [Test]
        public void CheckNewStatus()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.HasPermission(session);
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
                var srNo = webDriver.GetSRNumber();
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                webDriver.MakePayment("Cash", "250");
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSRStatus(srNo, "New", "Service Request External");
            }
        }
        
        [Test]
        public void CheckAwaitingDepositStatus()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
                webDriver.IsElementPresent(By.CssSelector("[ng-show='sections.payment.visible']"));
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                var srNo = webDriver.GetSRNumber();
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSRStatus(srNo, "Awaiting deposit", "Service Request External");
            }
        }

        [Test]
        public void CheckAwaitingAllocationStatus()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
                var srNo = webDriver.GetSRNumber();
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                webDriver.MakePayment("Cash", "250");
                webDriver.CompleteEvaluationSection("Warranty Covered");
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSRStatus(srNo, "Awaiting allocation", "Service Request External");
                
            }
        }

        [Test]
        public void CheckAwaitingSparePartsStatus()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
                var srNo = webDriver.GetSRNumber();
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                webDriver.MakePayment("Cash", "250");
                webDriver.CompleteEvaluationSection("Warranty Covered");
                webDriver.CompleteAllocationSectionWithAwaitingParts("Selenium Technician Internal");
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSRStatus(srNo, "Awaiting spare parts", "Service Request External");
            }
        }

        [Test]
        public void CheckAwaitingRepairStatus()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
                var srNo = webDriver.GetSRNumber();
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                webDriver.MakePayment("Cash", "250");
                webDriver.CompleteEvaluationSection("Warranty Covered");
                webDriver.CompleteAllocationSection("Selenium Technician External");
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSRStatus(srNo, "Awaiting repair", "Service Request External");
            }
        }

        [Test]
        public void CheckResolvedStatus()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
                var srNo = webDriver.GetSRNumber();
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                webDriver.MakePayment("Cash", "250");
                webDriver.CompleteResolutionSection("Major", "Customer");
                webDriver.SaveSR();
 //               webDriver.MakePayment("Cash", "37.50");
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSRStatus(srNo, "Resolved", "Service Request External");
            }
        }



        [Test]
        public void CheckClosedStatus()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
                var srNo = webDriver.GetSRNumber();
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                webDriver.MakePayment("Cash", "250");
                webDriver.FinaliseSR();
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSRStatus(srNo, "Closed", "Service Request External");
            }
        }

        [Test]
        public void CheckAwaitingPaymentStatus()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                Thread.Sleep(2000);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CompleteCustomerAndProductSections();
                var srNo = webDriver.GetSRNumber();
                Assert.IsTrue(webDriver.FindButtonByText("button", "Make Payment").Enabled);
                webDriver.MakePayment("Cash", "250");
                webDriver.CompleteEvaluationSection("Warranty Covered");
                webDriver.CompleteAllocationSection("Selenium Technician External");
                webDriver.CompleteResolutionSection("Major", "Customer");
                var cashPrice = string.Empty;
                webDriver.AddExternalOrSalvagedPart("External", "10", out cashPrice);
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.CheckSRStatus(srNo, "Awaiting payment", "Service Request External");
            }
        }
    }
}