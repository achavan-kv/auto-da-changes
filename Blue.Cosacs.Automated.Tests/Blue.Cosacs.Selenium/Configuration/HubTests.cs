using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Configuration.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using System.Threading;

namespace Blue.Cosacs.Selenium.Configuration
{
    [TestFixture]
    public class HubTests
    {
        [Test]
        public void CheckHubTableColumnHeaders()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.CheckColumnHeadersForHubTable();
            }
        }

        [Test]
        public void CheckWarehouseBookingSubmit()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenWarehouseBookingSubmit();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckWarehouseBookingCancel()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenWarehouseBookingCancel();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckCosacsBookingDeliver()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenCosacsBookingDeliver();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckCosacsBookingCancel()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenCosacsBookingCancel();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckCosacsServiceSummary()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenCosacsServiceSummary();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckServiceRequestSubmit()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenServiceRequestSubmit();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckCosacsServicePayment()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenCosacsServicePayment();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckCosacsServiceCharges()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenCosacsServiceCharges();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckOpenCosacsServiceParts()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenCosacsServiceParts();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckWarrantySaleSubmit()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenWarrantySaleSubmit();
                Thread.Sleep(1000);
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckCosacsServiceWarrantyServiceCompleted()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenCosacsServiceWarrantyServiceCompleted();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckWarrantySaleCancel()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenWarrantySaleCancel();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }

        [Test]
        public void CheckWarrantySaleCancelItem()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Configuration"));
                webDriver.GoTo("Configuration", "Hub", "Hub", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Hub Message Queues");
                webDriver.OpenWarrantySaleCancelItem();
                webDriver.CheckPoisonMessagesColumnHeaders();
                webDriver.CheckPendingMessagesColumnHeaders();
            }
        }


    }
}
