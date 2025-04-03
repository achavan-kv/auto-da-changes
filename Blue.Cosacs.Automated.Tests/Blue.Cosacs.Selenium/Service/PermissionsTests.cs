using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using Permission = Blue.Cosacs.Selenium.Common.PermissionsPage.PermissionCategory;
using System.Threading;

namespace Blue.Cosacs.Selenium.Service
{
    [TestFixture]
    public class PermissionsTests
    {
        [Test]
        public void AllowAddOrChangeTechnicianDiaryBookings()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1615");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1615"));
                webDriver.AllowPermission(Permission.Service, "1622");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1622"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician : 221049");
                webDriver.SelectDateOfStartWeekToView();
                webDriver.SelectVisibleWeeks("2");
                webDriver.WaitForElementPresent(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']"));
                webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                Assert.IsFalse(webDriver.PageSource.Contains("You do not have permission to add or modify bookings."));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyAddOrChangeTechnicianDiaryBookings()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1615");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1615"));
                webDriver.DenyPermission(Permission.Service, "1622");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Service, "1622"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician : 221049");
                webDriver.SelectDateOfStartWeekToView();
                webDriver.SelectVisibleWeeks("2");
                webDriver.WaitForElementPresent(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']"));
                webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                Thread.Sleep(2000);
                webDriver.IsTextPresent(By.CssSelector("#confirm p"), "You do not have permission to add or modify bookings.");
                webDriver.FindElement(By.CssSelector("div#confirm button.ok")).Click();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowAuthoriseChargeToChanges()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1604");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1604"));
                webDriver.AllowPermission(Permission.Service, "1603");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1603"));
                webDriver.AllowPermission(Permission.Service, "1611");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1611"));
                webDriver.AllowPermission(Permission.Service, "1631");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1631"));
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                Thread.Sleep(2000);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.FindElement(By.CssSelector("button")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("input[name=customerId]"));
                webDriver.CompleteCustomerAndProductSections();
                webDriver.SaveSR();
                webDriver.CompleteEvaluationSection("Misuse by the Customer");
                webDriver.FindElement(By.Name("ItemSerialNumber")).SendKeys("12345");
                webDriver.SaveSR();
                var srNo = webDriver.GetSRNumber();
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchSR(srNo, "Service Request External", "Awaiting allocation");
                webDriver.GoTo(By.CssSelector(".search-result"), "Service/Requests/" + srNo, session);
                webDriver.WaitForTextPresent(By.Id("s2id_resolutionPrimaryCharge"), "Customer");
                webDriver.FindElement(By.LinkText("Resolution")).Click();
                webDriver.FindElement(By.CssSelector("div:nth-child(4) > table:nth-child(2) > tbody > tr:nth-child(4) > td:nth-child(2) > div > a > div > b")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Supplier']")).Click();
                Thread.Sleep(2000);
                webDriver.IsTextPresent(By.CssSelector("div.modal.fade.in > .modal-body > .authorisation-reason.ng-binding"), "The primary charge to is being changed from Customer to Supplier. This change requires authorisation.");
                Assert.IsFalse(webDriver.FindElement(By.CssSelector("footer button.btn")).Enabled);
                webDriver.FindElement(By.Id("username")).SendKeys(userName);
                webDriver.FindElement(By.Id("password")).SendKeys("123**test");
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("footer button.btn")).Enabled);
                webDriver.FindElement(By.CssSelector("footer button.btn")).Click();
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div:nth-child(4) > table:nth-child(2) > tbody > tr:nth-child(5) > td:nth-child(2) > div > a > div > b")).Displayed);
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div:nth-child(4) > table:nth-child(2) > tbody > tr:nth-child(7) > td:nth-child(2) > div > a > div > b")).Displayed);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyAuthoriseChargeToChanges()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1604");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1604"));
                webDriver.AllowPermission(Permission.Service, "1603");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1603"));
                webDriver.AllowPermission(Permission.Service, "1611");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1611"));
                webDriver.DenyPermission(Permission.Service, "1631");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Service, "1631"));
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                Thread.Sleep(2000);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.IsCreateButtonEnabled();
                webDriver.FindElement(By.CssSelector("button")).Click();
                webDriver.CompleteCustomerAndProductSections();
                webDriver.SaveSR();
                webDriver.CompleteEvaluationSection("Misuse by the Customer");
                webDriver.FindElement(By.Name("ItemSerialNumber")).SendKeys("12345");
                webDriver.SaveSR();
                var srNo = webDriver.GetSRNumber();
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "Search Service Requests", "Service/Requests", session);
                webDriver.WaitForElementPresent(By.ClassName("text-search"));
                webDriver.SearchSR(srNo, "Service Request External", "Awaiting allocation");
                webDriver.GoTo(By.CssSelector(".search-result"), "Service/Requests/" + srNo, session);
                webDriver.WaitForTextPresent(By.Id("s2id_resolutionPrimaryCharge"), "Customer");
                webDriver.FindElement(By.LinkText("Resolution")).Click();
                webDriver.FindElement(By.CssSelector("div:nth-child(4) > table:nth-child(2) > tbody > tr:nth-child(4) > td:nth-child(2) > div > a > div > b")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Supplier']")).Click();
                Thread.Sleep(2000);
                webDriver.IsTextPresent(By.CssSelector("div.modal.fade.in > .modal-body > .authorisation-reason.ng-binding"), "The primary charge to is being changed from Customer to Supplier. This change requires authorisation.");
                Assert.IsFalse(webDriver.FindElement(By.CssSelector("footer button.btn")).Enabled);
                webDriver.FindElement(By.Id("username")).SendKeys(userName);
                webDriver.FindElement(By.Id("password")).SendKeys("123**test");
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("footer button.btn")).Enabled);
                webDriver.FindElement(By.CssSelector("footer button.btn")).Click();
                Thread.Sleep(1000);
                webDriver.IsTextPresent(By.CssSelector("div.failure-message.ng-binding"), "Authorisation failed. Either your username/password is incorrect or you do not have permission to authorise this change.");
                webDriver.FindElement(By.CssSelector("footer button.cancel")).Click();
                Assert.IsFalse(webDriver.FindElement(By.CssSelector("div:nth-child(4) > table:nth-child(2) > tbody > tr:nth-child(5) > td:nth-child(2) > div > a > div > b")).Displayed);
                Assert.IsFalse(webDriver.FindElement(By.CssSelector("div:nth-child(4) > table:nth-child(2) > tbody > tr:nth-child(7) > td:nth-child(2) > div > a > div > b")).Displayed);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowChangeBranchStockSearch()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1633");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1633"));
                webDriver.AllowPermission(Permission.Service, "1604");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1604"));
                webDriver.AllowPermission(Permission.Service, "1623");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1623"));
                webDriver.AllowPermission(Permission.Service, "1628");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1628"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                Thread.Sleep(2000);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.FindElement(By.CssSelector("button")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.CssSelector("div.ui-icon.ui-icon-search.click")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.popup-body.modalStock"));
                Assert.IsFalse(webDriver.ElementPresent(By.CssSelector("div.popup-body.modalStock > div.modal-header > div.search > div:nth-child(2) > div.select2-container.select2-container-disabled.ng-valid.ng-dirty")));
                webDriver.FindElement(By.CssSelector("span.close.ui-icon.ui-icon-closethick")).Click();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyChangeBranchStockSearch()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1633");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1633"));
                webDriver.AllowPermission(Permission.Service, "1604");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1604"));
                webDriver.AllowPermission(Permission.Service, "1628");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1628"));
                webDriver.DenyPermission(Permission.Service, "1623");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Service, "1623"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                Thread.Sleep(2000);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.FindElement(By.CssSelector("button")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.CssSelector("div.ui-icon.ui-icon-search.click")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.popup-body.modalStock"));
                Assert.IsTrue(webDriver.ElementPresent(By.CssSelector("div.popup-body.modalStock > div.modal-header > div.search > div:nth-child(2) > div.select2-container.select2-container-disabled.ng-valid.ng-dirty")));
                webDriver.FindElement(By.CssSelector("span.close.ui-icon.ui-icon-closethick")).Click();
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowChangeTechicianAvailability()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1615");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1615"));
                webDriver.AllowPermission(Permission.Service, "1624");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1624"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician : 221049");
                webDriver.WaitForElementPresent(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']"));
                Assert.IsFalse(webDriver.FindElement(By.CssSelector("div.technicianDiary > div > div:nth-child(3) button.btn")).Enabled);
                webDriver.FindElement(By.CssSelector("div.technicianDiary > div > div:nth-child(3) input:nth-child(2)")).Click();
                webDriver.SelectTodayFromDatePicker();
                webDriver.FindElement(By.CssSelector("div.technicianDiary > div > div:nth-child(3) input:nth-child(3)")).Click();
                webDriver.SelectTomorrowFromDatePicker();
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div.technicianDiary > div > div:nth-child(3) button.btn")).Enabled);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyChangeTechicianAvailability()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1615");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1615"));
                webDriver.DenyPermission(Permission.Service, "1624");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Service, "1624"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician : 221049");
                webDriver.WaitForElementPresent(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']"));
                Assert.IsFalse(webDriver.FindElement(By.CssSelector("div.technicianDiary > div > div:nth-child(3) button.btn")).Enabled);
                webDriver.FindElement(By.CssSelector("div.technicianDiary > div > div:nth-child(3) input:nth-child(2)")).Click();
                webDriver.SelectTodayFromDatePicker();
                webDriver.FindElement(By.CssSelector("div.technicianDiary > div > div:nth-child(3) input:nth-child(3)")).Click();
                webDriver.SelectTomorrowFromDatePicker();
                Assert.IsFalse(webDriver.FindElement(By.CssSelector("div.technicianDiary > div > div:nth-child(3) button.btn")).Enabled);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowDeleteTechnicianDiaryBookings()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1615");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1615"));
                webDriver.AllowPermission(Permission.Service, "1618");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1618"));
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Tech 2 : 234669");
                webDriver.SelectDateOfStartWeekToView();
                webDriver.SelectVisibleWeeks("2");
                webDriver.WaitForElementPresent(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']"));
                webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                if (webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div > h3 > div[title='Delete']")).Displayed == true)
                {
                    webDriver.DeleteJob("");
                    webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                    webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                    webDriver.AssignJob();
                }
                else if (webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div:nth-child(3) > h3")).Text == "Assign New")
                {
                    webDriver.AssignJob();
                }
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Tech 2 : 234669");
                webDriver.SelectDateOfStartWeekToView();
                webDriver.SelectVisibleWeeks("2");
                webDriver.WaitForElementPresent(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']"));
                webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                if (webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div > h3 > div[title='Delete']")).Displayed == true)
                {
                    webDriver.DeleteJob("");
                }
                else if (webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div:nth-child(3) > h3")).Text == "Assign New")
                {
                    webDriver.AssignJob();
                    webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                    webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                    webDriver.DeleteJob("");
                }
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyDeleteTechnicianDiaryBookings()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1615");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1615"));
                webDriver.DenyPermission(Permission.Service, "1618");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Service, "1618"));
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Tech 1 : 232670");
                webDriver.SelectDateOfStartWeekToView();
                webDriver.SelectVisibleWeeks("2");
                webDriver.WaitForElementPresent(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']"));
                webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                if (webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div > h3 > div[title='Delete']")).Displayed == true)
                {
                    webDriver.DeleteJob("");
                    webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                    webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                    webDriver.AssignJob();
                }
                else if (webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div:nth-child(3) > h3")).Text == "Assign New")
                {
                    webDriver.AssignJob();
                }
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Tech 1 : 232670");
                webDriver.SelectDateOfStartWeekToView();
                webDriver.SelectVisibleWeeks("2");
                webDriver.WaitForElementPresent(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']"));
                webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                if (webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div > h3 > div[title='Delete']")).Displayed == true || webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div > h3 > div[title='Delete (No Permission)']")).Displayed == true)
                {
                    Assert.IsTrue(webDriver.ElementPresent(By.CssSelector("div.ui-icon.ui-icon-trash.ui-icon-disabled")));
                }
                else if (webDriver.FindElement(By.CssSelector("div.modal.fade.in > div:nth-child(1) > div:nth-child(3) > h3")).Text == "Assign New")
                {
                    webDriver.AssignJob();
                    webDriver.FindElement(By.XPath("//div[@class='ng-scope'][1]/table[@class='diaryTable']/tbody/tr/td[5]/table/tbody/tr[7]")).Click();
                    webDriver.WaitForElementPresent(By.CssSelector("div.modal.fade.in"));
                    Assert.IsTrue(webDriver.ElementPresent(By.CssSelector("div.ui-icon.ui-icon-trash.ui-icon-disabled")));
                }
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void AllowEnableAllocation()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1633");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1633"));
                webDriver.AllowPermission(Permission.Service, "1604");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1604"));
                webDriver.AllowPermission(Permission.Service, "1608");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1608"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                Thread.Sleep(2000);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.FindElement(By.CssSelector("button")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.LinkText("Allocation")).Click();
                Assert.IsFalse(webDriver.ElementPresent(By.CssSelector("input[name='AllocationItemReceivedOn'][disabled='disabled']")));
                Assert.IsFalse(webDriver.ElementPresent(By.CssSelector("input[name='AllocationPartExpectOn'][disabled='disabled']")));
                Assert.IsFalse(webDriver.ElementPresent(By.CssSelector(".ui-resetwrap > [disabled='disabled']")));
                Assert.IsFalse(webDriver.ElementPresent(By.CssSelector("div:nth-child(3) > table > tbody:nth-child(2) > tr > td:nth-child(2) > div.select2-container.select2-container-disabled")));
                Assert.IsFalse(webDriver.ElementPresent(By.CssSelector("div:nth-child(3) > table > tbody:nth-child(2) > tr > td:nth-child(3) > div.select2-container.select2-container-disabled")));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

        [Test]
        public void DenyEnableAllocation()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                var userName = string.Empty;
                var roleName = string.Empty;
                webDriver.CreatePermissionsTestRole(out roleName, session);
                webDriver.CreatePermissionsTestUser(out userName, roleName, session);
                webDriver.GoToPermissionsPage(roleName, session);
                webDriver.AllowPermission(Permission.Service, "1633");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1633"));
                webDriver.AllowPermission(Permission.Service, "1604");
                Assert.IsTrue(webDriver.IsPermissionAllowed(Permission.Service, "1604"));
                webDriver.DenyPermission(Permission.Service, "1608");
                Assert.IsTrue(webDriver.IsPermissionDenied(Permission.Service, "1608"));
                webDriver.GoTo(By.Id("home"), string.Empty, session);
                webDriver.WaitForElementPresent(By.CssSelector(".logo"));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.LoginAsNewUser(userName, "test**123", "123**test");
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                Thread.Sleep(2000);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.FindElement(By.CssSelector("button")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.LinkText("Allocation")).Click();
                Assert.IsTrue(webDriver.ElementPresent(By.CssSelector("input[name='AllocationItemReceivedOn'][disabled='disabled']")));
                Assert.IsTrue(webDriver.ElementPresent(By.CssSelector("input[name='AllocationPartExpectOn'][disabled='disabled']")));
                Assert.IsTrue(webDriver.ElementPresent(By.CssSelector(".ui-resetwrap > [disabled='disabled']")));
                Assert.IsTrue(webDriver.ElementPresent(By.CssSelector("div:nth-child(3) > table > tbody:nth-child(2) > tr > td:nth-child(2) > div.select2-container.select2-container-disabled")));
                Assert.IsTrue(webDriver.ElementPresent(By.CssSelector("div:nth-child(3) > table > tbody:nth-child(2) > tr > td:nth-child(3) > div.select2-container.select2-container-disabled")));
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.WaitForElementPresent(By.Name("username"));
                webDriver.FindElement(By.Name("username")).Clear();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }
    }
}
