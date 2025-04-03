using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;
using OpenQA.Selenium.Interactions;

namespace Blue.Cosacs.Selenium.Service
{
    [TestFixture]
    public class MyDiaryTests
    {
      
        public void RejectJob()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician Internal");
                webDriver.SelectDateOfStartWeekToView();
                webDriver.SelectVisibleWeeks("1");
                webDriver.WaitForElementPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2)"));
                Thread.Sleep(500);
                var allocation = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(6) [ng-repeat='slot in day.slots']:nth-child(1) td")).GetAttribute("class");
                if (allocation == "fixed Service click" || allocation == "fixed stockRepair click" || allocation == "fixed Installation click")
                {
                    var srNo = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(6) [ng-repeat='slot in day.slots']:nth-child(1) a")).Text;
                    var element = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(6) [ng-repeat='slot in day.slots']:nth-child(1)"));
                    Actions click = new Actions(webDriver);
                    click.MoveToElement(element, 60, 0).Click().Build().Perform();
                    webDriver.WaitForElementPresent(By.CssSelector("#dialogueEditBooking .modal-content"));
                    webDriver.DeleteJob(srNo);
                }
                if (allocation == "fixed Service click rejected")
                {
                    var srNo = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(6) [ng-repeat='slot in day.slots']:nth-child(1) a")).Text;
                    webDriver.FindElement(By.PartialLinkText(srNo + " (ServiceRequestInternal)")).Click();
                    webDriver.WaitForElementPresent(By.CssSelector("#dialogueEditBooking .modal-content"));
                    webDriver.DeleteJob(srNo);
                }
                webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(6) [ng-repeat='slot in day.slots']:nth-child(1)")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("#dialogueEditBooking .modal-content"));
                webDriver.AssignJob();
                Thread.Sleep(1000);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.LoginToCosacs("seltechint", "test**123");
                webDriver.GoTo("Service", "My Diary", "Service/TechnicianDiaries/MyDiary", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("[ng-model='displayStartDate']"));
                var srN = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(6) [ng-repeat='slot in day.slots']:nth-child(1) a")).Text;
                webDriver.IsTextPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(6) [ng-repeat='slot in day.slots']:nth-child(1) a"), srN);
                var ele = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(6) [ng-repeat='slot in day.slots']:nth-child(1)"));
                Actions clk = new Actions(webDriver);
                clk.MoveToElement(ele, 60, 0).Click().Build().Perform();
                webDriver.RejectJob(srN);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }

      
        
        public void TestJobRejectionLimit()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician Internal");
                webDriver.SelectDateOfStartWeekToView();
                webDriver.SelectVisibleWeeks("1");
                webDriver.WaitForElementPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2)"));
                Thread.Sleep(500);
                var allocation = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(3) [ng-repeat='slot in day.slots']:nth-child(1) td")).GetAttribute("class");
                if (allocation == "fixed Service click" || allocation == "fixed stockRepair click" || allocation == "fixed Installation click")
                {
                    var srNo = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(3) [ng-repeat='slot in day.slots']:nth-child(1) a")).Text;
                    var element = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(3) [ng-repeat='slot in day.slots']:nth-child(1)"));
                    Actions click = new Actions(webDriver);
                    click.MoveToElement(element, 60, 0).Click().Build().Perform();
                    webDriver.WaitForElementPresent(By.CssSelector("#dialogueEditBooking .modal-content"));
                    webDriver.DeleteJob(srNo);
                }
                webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(3) [ng-repeat='slot in day.slots']:nth-child(1)")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("#dialogueEditBooking .modal-content"));
                webDriver.AssignJob();
                Thread.Sleep(1000);
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.LoginToCosacs("seltechint", "test**123");
                webDriver.GoTo("Service", "My Diary", "Service/TechnicianDiaries/MyDiary", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("[ng-model='displayStartDate']"));
                var srN = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(3) [ng-repeat='slot in day.slots']:nth-child(1) a")).Text;
                webDriver.IsTextPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(3) [ng-repeat='slot in day.slots']:nth-child(1) a"), srN);
                var ele = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(3) [ng-repeat='slot in day.slots']:nth-child(1)"));
                Actions clk = new Actions(webDriver);
                clk.MoveToElement(ele, 60, 0).Click().Build().Perform();
                webDriver.CheckNotification("×\r\nThis job is within 2 days and cannot be rejected");
                webDriver.FindElement(By.Id("logoff")).Click();
                webDriver.LoginToCosacs(session.Username, session.Password);
            }
        }
    }
}
