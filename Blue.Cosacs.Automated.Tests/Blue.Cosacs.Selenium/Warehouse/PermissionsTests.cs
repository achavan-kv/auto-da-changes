using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;
using Blue.Cosacs.Selenium.Warehouse.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Permission = Blue.Cosacs.Selenium.Common.PermissionsPage.PermissionCategory;
using Branch = System.Configuration.ConfigurationManager;
using System.Text.RegularExpressions;
using System.Threading;

namespace Blue.Cosacs.Selenium.Warehouse
{
    [TestFixture]
    public class PermissionsTests
    {
        [Test]
        public void AllowBookingSearchScreenPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1401");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1401"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickLogisticsMenu();
                webDriver.IsElementPresent(By.LinkText("Search Shipments"));
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyBookingSearchScreenPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1401");
                Thread.Sleep(2000);
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1401"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                //webDriver.IsElementNotPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Shipment Search Screen (Warehouse)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowPickingPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1402");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1402"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickLogisticsMenu();
                webDriver.IsElementPresent(By.LinkText("Picking"));
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyPickingPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1402");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1402"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
               // webDriver.IsElementNotPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Picking (Warehouse)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowSearchPickListsPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1422");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1422"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickLogisticsMenu();
                webDriver.IsElementPresent(By.LinkText("Search Pick Lists"));
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenySearchPickListsPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1422");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1422"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
//              webDriver.IsElementNotPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Search Pick Lists (Warehouse)");
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Confirmation/1", session);
                if (webDriver.FindElement(By.Id("page-heading")).Text == "Forbidden")
                {
                    webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                    webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Search Pick Lists (Warehouse)");
                }
                else
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowConfirmPickListsPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1422");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1422"));
                webDriver.AllowPermission(Permission.Warehouse, "1404");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1404"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                webDriver.FindElement(By.CssSelector(".results .result:first-child")).Click();
                webDriver.WaitForElementPresent(By.Id("pickedOn"));
                webDriver.IsElementPresent(By.CssSelector("button#save"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyConfirmPickListPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1422");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1422"));
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1404");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1404"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                webDriver.FindElement(By.CssSelector(".results .result:first-child")).Click();
                webDriver.WaitForElementPresent(By.Id("pickedOn"));
                webDriver.IsElementNotPresent(By.CssSelector("button#save"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowReprintPickListsPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1422");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1422"));
                webDriver.AllowPermission(Permission.Warehouse, "1403");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1403"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.FindElement(By.CssSelector(".search-result:first-child")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.WaitForElementPresent(By.CssSelector(".booking "));
                webDriver.IsElementPresent(By.CssSelector("#reprint"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyReprintPickListPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1422");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1422"));
                webDriver.DenyPermission(Permission.Warehouse, "1403");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1403"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.FindElement(By.CssSelector(".search-result:first-child")).Click();
                System.Threading.Thread.Sleep(1000);
                webDriver.WaitForElementPresent(By.CssSelector(".booking "));
                webDriver.IsElementNotPresent(By.CssSelector("#reprint"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowSchedulingPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1424");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1424"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickLogisticsMenu();
                webDriver.IsElementPresent(By.LinkText("Scheduling"));
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenySchedulingPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1424");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1424"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
             //   webDriver.IsElementNotPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Scheduling (Warehouse)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowCreateSchedulesPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1424");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1424"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.AllowPermission(Permission.Warehouse, "1406");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1406"));
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch3"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var deliveryScheduleNo = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.PickItemsAndConfirmPickList(pickListNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(deliveryLocation);
                webDriver.SelectTruck(selectedTruckName);
                webDriver.IsElementPresent(By.Id("saveDeliverySchedule"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyCreateSchedulesPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1424");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1424"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.DenyPermission(Permission.Warehouse, "1406");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1406"));
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch7"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var deliveryScheduleNo = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.PickItemsAndConfirmPickList(pickListNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(deliveryLocation);
                Assert.IsFalse(webDriver.FindElement(By.Id("saveDeliverySchedule")).Displayed);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowPrintLoadPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1424");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1424"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.AllowPermission(Permission.Warehouse, "1405");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1405"));
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch7"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var deliveryScheduleNo = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.PickItemsAndConfirmPickList(pickListNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(deliveryLocation);
                webDriver.SelectTruck(selectedTruckName);
                webDriver.IsElementPresent(By.Id("print"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyPrintLoadPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1424");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1424"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.DenyPermission(Permission.Warehouse, "1405");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1405"));
                var currentUser = webDriver.QueryDataBase("SELECT Login + ' - ' + FullName FROM Admin.[User] WHERE Login = '" + session.Username + "'", string.Empty);
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                var deliveryLocation = Branch.AppSettings["Branch8"];
                var bookingNo = string.Empty;
                var originalQuantity = string.Empty;
                var delCol = string.Empty;
                var selectedTruckName = string.Empty;
                var pickListNo = string.Empty;
                var deliveryScheduleNo = string.Empty;
                webDriver.AddBookingToTruckAndCreatePickList(out bookingNo, out originalQuantity, out selectedTruckName, out pickListNo, deliveryLocation, out delCol, currentUser);
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.PickItemsAndConfirmPickList(pickListNo, bookingNo, originalQuantity, deliveryLocation.Substring(0,3), "Created");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(deliveryLocation);
                webDriver.SelectTruck(selectedTruckName);
                webDriver.IsElementNotPresent(By.Id("print"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowSearchDeliverySchedulesPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1423");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1423"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickLogisticsMenu();
                webDriver.IsElementPresent(By.LinkText("Search Delivery Schedules"));
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenySearchDeliverySchedulesPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1423");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1423"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
//                webDriver.IsElementNotPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Search Delivery Schedules (Warehouse)");
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Confirmation/1", session);
                if (webDriver.FindElement(By.Id("page-heading")).Text == "Forbidden")
                {
                    webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                    webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Search Delivery Schedules (Warehouse)");
                }
                else
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowDeliveryConfirmationPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1423");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1423"));
                webDriver.AllowPermission(Permission.Warehouse, "1408");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1408"));
                webDriver.AllowPermission(Permission.Warehouse, "1432");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1432"));
                var deliveryScheduleNo = string.Empty;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                webDriver.ApplyFacetFilters("StockBranchName", "COURTS 969");
                //webDriver.ApplyFacetFilters("StockBranchName", "LUCKY DOLLAR 751");
                var pickListNo = webDriver.FindElement(By.CssSelector("div.pick-list.search-result:first-child")).GetAttribute("data-id");
                var deliveryLocation = webDriver.GetText(By.CssSelector("[data-id='" + pickListNo + "'] .branch"));
                deliveryLocation = Regex.Match(deliveryLocation, @"\d+").Value;
                var selectedTruckName = webDriver.TruckName(pickListNo);
                webDriver.ConfirmPickList(pickListNo, deliveryLocation, "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, out deliveryScheduleNo);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchDeliverySchedule(deliveryScheduleNo, deliveryLocation, "Scheduled");
                webDriver.OpenDeliverySchedule(deliveryScheduleNo);
                webDriver.IsElementPresent(By.CssSelector("button#save"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyDeliveryConfirmationPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1423");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1423"));
                webDriver.DenyPermission(Permission.Warehouse, "1408");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1408"));
                var deliveryScheduleNo = string.Empty;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                webDriver.ApplyFacetFilters("StockBranchName", "LUCKY DOLLAR 751");
                var pickListNo = webDriver.FindElement(By.CssSelector("div.pick-list.search-result:nth-child(2)")).GetAttribute("data-id");
                var deliveryLocation = webDriver.GetText(By.CssSelector("[data-id='" + pickListNo + "'] .branch"));
                deliveryLocation = Regex.Match(deliveryLocation, @"\d+").Value;
                var selectedTruckName = webDriver.TruckName(pickListNo);
                webDriver.ConfirmPickList(pickListNo, deliveryLocation, "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, out deliveryScheduleNo);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchDeliverySchedule(deliveryScheduleNo, deliveryLocation, "Scheduled");
                webDriver.OpenDeliverySchedule(deliveryScheduleNo);
                webDriver.IsElementNotPresent(By.CssSelector("button#save"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }
        [Test]
        public void AllowReprintSchedulePermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1423");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1423"));
                webDriver.AllowPermission(Permission.Warehouse, "1407");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1407"));
                var deliveryScheduleNo = string.Empty;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                //var pickListNo = webDriver.FindElement(By.XPath(".//*[@id='resultsContainer']/div[4]")).GetAttribute("data-id"); // added sk
                var pickListNo = webDriver.FindElement(By.CssSelector("div.pick-list.search-result:first-child")).GetAttribute("data-id");
                var deliveryLocation = webDriver.GetText(By.CssSelector("[data-id='" + pickListNo + "'] .branch"));
                deliveryLocation = Regex.Match(deliveryLocation, @"\d+").Value;
                var selectedTruckName = webDriver.TruckName(pickListNo);
                webDriver.ConfirmPickList(pickListNo, deliveryLocation, "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, out deliveryScheduleNo);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchDeliverySchedule(deliveryScheduleNo, deliveryLocation, "Scheduled");
                webDriver.OpenDeliverySchedule(deliveryScheduleNo);
                webDriver.IsElementPresent(By.Id("reprint"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyReprintSchedulePermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1423");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1423"));
                webDriver.DenyPermission(Permission.Warehouse, "1407");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1407"));
                var deliveryScheduleNo = string.Empty;
                webDriver.GoTo("Logistics", "Search Pick Lists", "Warehouse/Picking/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("PickingStatus", "Created");
                var pickListNo = webDriver.FindElement(By.CssSelector("div.pick-list.search-result:nth-child(2)")).GetAttribute("data-id");
                var deliveryLocation = webDriver.GetText(By.CssSelector("[data-id='" + pickListNo + "'] .branch"));
                deliveryLocation = Regex.Match(deliveryLocation, @"\d+").Value;
                var selectedTruckName = webDriver.TruckName(pickListNo);
                webDriver.ConfirmPickList(pickListNo, deliveryLocation, "Created");
                webDriver.GoTo("Logistics", "Scheduling", "Warehouse/Delivery/Schedule", session);
                webDriver.WaitForElementPresent(By.CssSelector("#deliveryBranch_chzn"));
                webDriver.CreateDeliverySchedule(deliveryLocation, selectedTruckName, out deliveryScheduleNo);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Delivery Schedules", "Warehouse/Delivery/Search", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchDeliverySchedule(deliveryScheduleNo, deliveryLocation, "Scheduled");
                webDriver.OpenDeliverySchedule(deliveryScheduleNo);
                webDriver.IsElementNotPresent(By.Id("reprint"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowCustomerPickUpPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickLogisticsMenu();
                webDriver.IsElementPresent(By.LinkText("Customer Pick Up"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyCustomerPickUpPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1425"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
//                webDriver.IsElementNotPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Customer Pick Up (Warehouse)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowCustomerPickUpFailedPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.AllowPermission(Permission.Warehouse, "1417");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1417"));
                webDriver.AllowPermission(Permission.Warehouse, "1420");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1420"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch1"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch1"]);
                webDriver.IsElementNotPresent(By.CssSelector("tr[data-id='" + bookingNo + "'] .chzn-container.chzn-container-single.chzn-disabled"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch1"]);
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
            }
        }

        [Test]
        public void DenyCustomerPickUpFailedPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.AllowPermission(Permission.Warehouse, "1417");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1417"));
                webDriver.DenyPermission(Permission.Warehouse, "1420");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1420"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch12"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch12"]);
                webDriver.WaitForElementPresent(By.CssSelector("[data-id='" + bookingNo + "'] .rejectItem"));
                webDriver.IsElementPresent(By.CssSelector("[data-id='" + bookingNo + "'] .rejectItem .chzn-disabled"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch12"]);
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
            }
        }

        [Test]
        public void AllowCustomerPickUpNotifyPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.AllowPermission(Permission.Warehouse, "1417");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1417"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch7"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                //webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch13"]);
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch7"]);
                webDriver.IsElementPresent(By.Id("confirmPickUp#" + bookingNo));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                //webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch13"]);
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch7"]);
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
            }
        }

        [Test]
        public void DenyCustomerPickUpNotifyPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.DenyPermission(Permission.Warehouse, "1417");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1417"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch6"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch6"]);
                webDriver.IsElementNotPresent(By.Id("confirmPickUp#" + bookingNo));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch6"]);
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
            }
        }

        [Test]
        public void AllowPrintCustomerPickUpNotePermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.AllowPermission(Permission.Warehouse, "1426");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1426"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch15"]);
                webDriver.IsElementPresent(By.CssSelector("[id*=printPickUp]"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyPrintCustomerPickUpNotePermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.DenyPermission(Permission.Warehouse, "1426");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1426"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch16"]);
                webDriver.IsElementNotPresent(By.CssSelector("[id*=printPickUp]"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowReprintCustomerPickUpNotePermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.AllowPermission(Permission.Warehouse, "1427");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1427"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch2"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch2"]);
                webDriver.IsElementPresent(By.CssSelector("[data-id='" + bookingNo + "'] .reprintButton"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch2"]);
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
            }
        }

        [Test]
        public void DenyReprintCustomerPickUpNotePermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1425");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1425"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.DenyPermission(Permission.Warehouse, "1427");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1427"));
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch9"]);
                var bookingNo = webDriver.GetBookingNoInCustomerPickUpPage();
                var delOrCol = string.Empty;
                webDriver.PrintCustomerPickUp(bookingNo, out delOrCol);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch9"]);
                webDriver.IsElementNotPresent(By.CssSelector("[data-id='" + bookingNo + "'] .reprintButton"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
                webDriver.GoTo("Logistics", "Customer Pick Up", "Warehouse/CustomerPickUps/Print", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.SelectDeliveryLocation(Branch.AppSettings["Branch9"]);
                webDriver.ConfirmCustomerPickUp(bookingNo, delOrCol);
            }
        }

        [Test]
        public void AllowAmendBookingPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1401");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1401"));
                webDriver.AllowPermission(Permission.Warehouse, "1428");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1428"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("BookingStatus", "Booked");
                var bookedBookingNo = webDriver.GetBookingNoOfSearchResult();
                webDriver.GoTo(By.ClassName("Booking-Id"), "Warehouse/Bookings/detail/" + bookedBookingNo, session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookedBookingNo);
                webDriver.IsAmendDetailsLinkPresent();
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("BookingStatus", "Exception");
                var exceptionBookingNo = webDriver.GetBookingNoOfSearchResult();
                webDriver.GoTo(By.ClassName("Booking-Id"), "Warehouse/Bookings/detail/" + exceptionBookingNo, session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + exceptionBookingNo);
                webDriver.IsResolveExceptionLinkPresent();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyAmendBookingPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1401");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1401"));
                webDriver.DenyPermission(Permission.Warehouse, "1428");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1428"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("BookingStatus", "Booked");
                var bookedBookingNo = webDriver.GetBookingNoOfSearchResult();
                webDriver.GoTo(By.ClassName("Booking-Id"), "Warehouse/Bookings/detail/" + bookedBookingNo, session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + bookedBookingNo);
                webDriver.IsElementNotPresent(By.ClassName("showAmend"));
                webDriver.GoTo("Logistics", "Search Shipments", "Warehouse/Bookings/", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.ApplyFacetFilters("BookingStatus", "Exception");
                var exceptionBookingNo = webDriver.GetBookingNoOfSearchResult();
                webDriver.GoTo(By.ClassName("Booking-Id"), "Warehouse/Bookings/detail/" + exceptionBookingNo, session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Shipment #" + exceptionBookingNo);
                webDriver.IsElementNotPresent(By.ClassName("showException"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowWarehouseAllBranchesPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1402");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1402"));
                webDriver.AllowPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1409"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.IsElementNotPresent(By.ClassName("chzn-disabled"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyWarehouseAllBranchesPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1402");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1402"));
                webDriver.DenyPermission(Permission.Warehouse, "1409");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1409"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Picking", "Warehouse/Picking", session);
                webDriver.WaitForElementPresent(By.Id("deliveryBranch_chzn"));
                webDriver.IsElementPresent(By.ClassName("chzn-disabled"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowDriverMaintenanceViewPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1410");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1410"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickLogisticsMenu();
                webDriver.IsElementPresent(By.LinkText("Drivers"));
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyDriverMaintenanceViewPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1410");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1410"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
              //  webDriver.IsElementNotPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Driver Maintenance - View (Warehouse)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowDriverMaintenanceEditPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1410");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1410"));
                webDriver.AllowPermission(Permission.Warehouse, "1411");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1411"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.IsElementPresent(By.CssSelector("a[title='New']"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyDriverMaintenanceEditPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1410");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1410"));
                webDriver.DenyPermission(Permission.Warehouse, "1411");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1411"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Drivers", "Warehouse/Drivers", session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.IsElementNotPresent(By.CssSelector("a[title='New']"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowTruckMaintenanceViewPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1412");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1412"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.ClickLogisticsMenu();
                webDriver.IsElementPresent(By.LinkText("Trucks"));
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyTruckMaintenanceViewPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.DenyPermission(Permission.Warehouse, "1412");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1412"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
              //webDriver.IsElementNotPresent(By.LinkText("Logistics"));
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Forbidden");
                webDriver.IsTextPresent(By.Id("center"), "Forbidden\r\nYou are not allowed to perform the following action: Truck Maintenance - View (Warehouse)");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowTruckMaintenanceEditPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1412");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1412"));
                webDriver.AllowPermission(Permission.Warehouse, "1413");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1413"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.IsElementPresent(By.CssSelector("a[title='New']"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyTruckMaintenanceEditPermission()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Warehouse, "1412");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Warehouse, "1412"));
                webDriver.DenyPermission(Permission.Warehouse, "1413");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Warehouse, "1413"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Logistics", "Trucks", "Warehouse/Trucks", session);
                webDriver.WaitForElementPresent(By.Id("s_Name"));
                webDriver.IsElementNotPresent(By.CssSelector("a[title='New']"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }
    }
}
