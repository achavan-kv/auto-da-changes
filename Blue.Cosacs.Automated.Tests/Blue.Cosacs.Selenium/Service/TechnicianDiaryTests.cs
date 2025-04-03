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
   public class TechnicianDiaryTests
   {
       [Test]
      public void CheckTechnicianDiaryDefaultView()
       {
                      using (var session = Session.Get())
                     {
                          var webDriver = session.WebDriver;
                          webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
                          webDriver.HasPermission(session);
                          webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
                          webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician Internal");
                          webDriver.SelectDateOfStartWeekToView();
                          webDriver.SelectVisibleWeeks("2");
                          webDriver.WaitForElementPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2)"));
                          webDriver.IsElementPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2)"));
                          webDriver.IsElementPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(3)"));
                          webDriver.IsElementPresent(By.CssSelector("[ng-model='holiday.requestEnd']"));
                          webDriver.IsElementPresent(By.CssSelector("[ng-model='holiday.requestStart']"));
                          webDriver.IsElementPresent(By.CssSelector("div.technicianDiary > div > div:nth-child(3) button.btn"));
                          Assert.IsFalse(webDriver.FindButtonByText("button", "Submit").Enabled);
                      } 
       }

     [Test]
       public void AssignNewJob()
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
               Thread.Sleep(2000);

               webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
               webDriver.HasPermission(session);
               webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
               webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician Internal");
               webDriver.SelectDateOfStartWeekToView();
               webDriver.SelectVisibleWeeks("2");
               webDriver.WaitForElementPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2)"));




               var allocation = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(2) [ng-repeat='slot in day.slots']:nth-child(1) td")).GetAttribute("class");
               if (allocation == "fixed Service click" || allocation == "fixed stockRepair click" || allocation == "fixed Installation click")
               {
                   var srNo1 = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(2) [ng-repeat='slot in day.slots']:nth-child(1) a")).Text;
                   var element = webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(2) [ng-repeat='slot in day.slots']:nth-child(1)"));
                   Actions click = new Actions(webDriver);
                   click.MoveToElement(element, 60, 0).Click().Build().Perform();
                   webDriver.WaitForElementPresent(By.CssSelector("#dialogueEditBooking .modal-content"));
                   webDriver.DeleteJob(srNo1);
               }
               


               webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(2) [ng-repeat='slot in day.slots']:nth-child(1)")).Click();
               webDriver.WaitForElementPresent(By.CssSelector("#dialogueEditBooking .modal-content"));
                          
               //Assigning Job
               webDriver.FindElement(By.CssSelector(".select2-choice.select2-default")).Click();
               webDriver.WaitForElementPresent(By.XPath("//*[@id='select2-drop']/div/input"));
               webDriver.FindElement(By.XPath("//*[@id='select2-drop']/div/input")).SendKeys(srNo);
               Thread.Sleep(5000);
               webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li/div")).Click();
               webDriver.FindElement(By.XPath("//*[@id='dialogueEditBooking']/div/div/div[3]/button[1]")).Click();



       } 
       }


  //   [Test]
       public void TechnicianAvailabilityTest()
       {
           using (var session = Session.Get())
           {
               var webDriver = session.WebDriver;
               webDriver.GoTo("Service", "Technician Diary", "Service/Technicians/Diary", session);
               webDriver.HasPermission(session);
               
               webDriver.WaitForElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
               webDriver.SelectTechnicianInTechnicianDiaryPage("Selenium Technician External");
               webDriver.FindElement(By.CssSelector(".col-lg-4.DateStartSelection .form-control.ng-pristine.ng-valid.hasDatepicker")).Click();
               webDriver.SelectDateOfStartWeekToView();
               webDriver.FindElement(By.XPath("//input[@type='number']")).Clear();
               webDriver.FindElement(By.XPath("//input[@type='number']")).SendKeys("1");
               webDriver.WaitForElementPresent(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2)"));
               Thread.Sleep(500);
               if (webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(4) [ng-repeat='slot in day.slots']:nth-child(1) td")).GetAttribute("class") == "fixed holidayApproved click")
               {
                   webDriver.DeleteTechnicianAvailability();
               }

               webDriver.AddTechnicianAvailability();
               webDriver.DeleteTechnicianAvailability();
           } 
       }
   }
}  