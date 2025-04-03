using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;

namespace Blue.Cosacs.Selenium
{


  [TestFixture]
    public class DairyExceptionsTests
    {

   //     [Test]


        public void VerifyExceptionCreation()
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
                webDriver.CreateServiceRequestForJobData("SeleniumJob1", "Mr", "Mark", "Taylor", "Address1", "Address2", "Town", "Lo1 3TD", "test@testmail.com", "Electrical", "1 - Vision", "10  - Vision", "12345", "ItemDesc", "Supplier", "Brother", "749 COURTS", "5000");
                var srNumber = webDriver.GetSRNumber();
                webDriver.CompleteAllocationSection("Selenium Technician External");
                webDriver.FindElement(By.CssSelector("#logoff")).Click();
                Thread.Sleep(1000);
                webDriver.WaitForElementPresent(By.CssSelector("#username"));
                webDriver.LoginToCosacs("seltechext", "test**123");
                webDriver.GoTo("Service", "My Dairy", "Service/TechnicianDiaries/MyDiary", session);

                // Rejecting the job

                webDriver.FindElement(By.CssSelector(".technicianDiary > div:nth-child(1) > div:nth-child(2) > div:nth-child(2) > table:nth-child(1) > tbody:nth-child(2) > tr:nth-child(1) > td:nth-child(8) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1)")).Click();
                webDriver.FindElement(By.CssSelector(".select2-choice.select2-default")).Click();
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li/div")).Click();
                webDriver.FindElement(By.XPath("//*[@id='dialogueCommon']/div/div/div[3]/button[1]")).Click();
                webDriver.FindElement(By.CssSelector("#logoff")).Click();
                // logging as a supervisor and verifying whether the exception is raising or not
                webDriver.GoTo("Service", "Diary Exceptions", "Service/TechnicianDiaries/DiaryExceptions", session);
                webDriver.FindElement(By.XPath("//*[@id='diary-view']/div[1]/div[2]/div[2]/div[2]/div[2]/a")).Text.Contains(srNumber);

       


            }
        }





    }
}
