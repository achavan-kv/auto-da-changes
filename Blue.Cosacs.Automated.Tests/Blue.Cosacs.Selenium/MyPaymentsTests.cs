using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using System.Linq;


namespace Blue.Cosacs.Selenium
{
    
    
    [TestFixture]
    

   public class MyPaymentsTsts
    {
        [Test]

        public void VerifyMyPaymentsDefaultView()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "My Payments", "Service/TechnicianPayments/MyPayments", session);
                webDriver.HasPermission(session);
                webDriver.IsElementPresent(By.CssSelector("#page-heading"));
                webDriver.IsElementPresent(By.CssSelector("#paymentsFrom"));
                webDriver.IsElementPresent(By.CssSelector("#paymentsTo"));
                webDriver.IsElementPresent(By.CssSelector(".select2-choice"));
                webDriver.FindElement(By.CssSelector("#requestFilter"));
                webDriver.FindElement(By.CssSelector(".btn.btn-default.form-control.click"));
                webDriver.FindElement(By.XPath("//*[@id='technicianPayments']/div[2]/div[2]/div[2]/button"));
                webDriver.FindElement(By.XPath("//*[@id='paymentView']"));


           }

        }




 //       [Test]
        public void VerifyTechnicianPaymentsInMyPaymentsE2E()
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


                //Default Payment 250

                webDriver.MakePayment("Cash","250");
                
                //Evaluation Section

                webDriver.FindElement(By.LinkText("Evaluation")).Click();
                webDriver.FindElement(By.CssSelector("#s2id_ServiceEvaluation")).Click();
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[3]/div")).Click();
                webDriver.FindElement(By.CssSelector("#s2id_EvaluationLocation")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='SERVICE']")).Click();
                webDriver.FindElement(By.CssSelector("#s2id_EvaluationAction")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Collected']")).Click();
                webDriver.FindElement(By.CssSelector("#s2id_EvaluationClaimFoodLoss")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='No']")).Click();
                webDriver.FindElement(By.XPath("//*[@id='ItemSerialNumber']")).Clear();
                webDriver.FindElement(By.XPath("//*[@id='ItemSerialNumber']")).SendKeys("123456");
                webDriver.SaveSR();


                Thread.Sleep(1000);

                //Allocation Section

                webDriver.FindElement(By.LinkText("Allocation")).Click();
                webDriver.FindElement(By.Name("AllocationItemReceivedOn")).Click();
                webDriver.SelectTodayFromDatePicker();
                
                webDriver.FindElement(By.CssSelector("[ng-model='techSelect.AllocationServiceScheduledOn']")).Click();
                webDriver.SelectTodayFromDatePicker();

                webDriver.FindElement(By.XPath("//*[@id='s2id_techy']/a")).Click();
               
                webDriver.WaitForElementPresent(By.XPath("//*[@id='select2-drop']/ul/li[34]/div"));
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[34]/div")).Click();
                Random r = new Random();
                int n = r.Next(1, 40);
                webDriver.ScrollElementInToView(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child("+n+") > td:nth-child(1)"));
                webDriver.FindElement(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child("+n+") > td:nth-child(1)")).Click();


             
           //     webDriver.ScrollElementInToView(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1)"));
            //    webDriver.FindElement(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1)")).Click();

                webDriver.SaveSR();


                Thread.Sleep(1000);

                // Resolution


                webDriver.ScrollElementInToView(By.XPath("//*[@id='s2id_RepairType']/a"));
                webDriver.FindElement(By.XPath("//*[@id='s2id_RepairType']/a")).Click();

                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[1]/div")).Click();

                webDriver.FindElement(By.XPath("//*[@id='s2id_Resolution']/a")).Click();
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li/div")).Click();

                webDriver.FindElement(By.XPath("//*[@id='ResolutionDate']")).Click();
                webDriver.SelectTodayFromDatePicker();


                var additionalCost = "250";
                var transportCost = "100";

                webDriver.FindElement(By.XPath("//*[@id='serviceForm']/div/div[7]/div/div/table/tbody/tr[1]/td[2]/input")).SendKeys(additionalCost);
                webDriver.FindElement(By.XPath("//*[@id='serviceForm']/div/div[7]/div/div/table/tbody/tr[1]/td[3]/input")).SendKeys(transportCost);


                webDriver.SaveSR();

                Thread.Sleep(1000);

                //Payment

                webDriver.MakePayment("Cash", "100");


                webDriver.SaveSR();

               

              
        //Finalise

                webDriver.FindElement(By.LinkText("Finalise")).Click();
                webDriver.FindElement(By.CssSelector("#s2id_FinalisedFailure")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Cosmetic Defect']")).Click();
                webDriver.FindElement(By.Name("FinaliseReturnDate")).Click();
                webDriver.SelectTodayFromDatePicker();
                webDriver.FindElement(By.LinkText("Comments")).Click();
                webDriver.FindElement(By.CssSelector("textarea[name=Comment]")).SendKeys("These are Selenium test comments");
                webDriver.FindElement(By.XPath("//*[@id='serviceForm']/div/div[12]/div[3]/div/button")).Click();
                Thread.Sleep(1000);
                webDriver.CLickButtonByText("button", "Save");
                webDriver.FindElement(By.CssSelector("#logoff")).Click();


                //Login as technican
                
                webDriver.WaitForElementPresent(By.CssSelector("#username"));
                
                webDriver.LoginToCosacs("seltechext", "test**123");
                webDriver.GoTo("Service", "My Payments", "Service/TechnicianPayments/MyPayments", session);
                webDriver.IsElementPresent(By.CssSelector("#page-heading"));


                Thread.Sleep(1000);

               var tableRows = webDriver.FindElements(By.CssSelector("#paymentView table tbody .ng-scope"));

                var rowWithSrNum = tableRows.First(ee => ee.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));

                var totalFromTable = rowWithSrNum.FindElement(By.CssSelector(" td:nth-child(6)")).Text.Replace("$",String.Empty).Trim().Split('.')[0];

               var totalCost = Int32.Parse(transportCost.Trim()) + Int32.Parse(additionalCost.Trim());

                Assert.IsTrue(totalCost == Int32.Parse(totalFromTable));

               var initialStatus = rowWithSrNum.FindElement(By.CssSelector(" td:nth-child(9)")).Text;

                Assert.AreEqual("Pending", initialStatus);

                // Searching through Service Request

                webDriver.FindElement(By.XPath("//*[@id='requestFilter']")).Clear();
                webDriver.FindElement(By.XPath("//*[@id='requestFilter']")).SendKeys(srNumber);

                webDriver.FindElement(By.XPath("//*[@id='technicianPayments']/div[2]/div[2]/div[1]/button")).Click();

                Thread.Sleep(1000);

                var tableRowsInSearch = webDriver.FindElements(By.CssSelector("#paymentView table tbody .ng-scope"));

                Assert.IsTrue(tableRowsInSearch.Count == 1);

                Assert.IsTrue(tableRowsInSearch[0].FindElement(By.CssSelector("td a")).Text.Contains(srNumber));


         //  Searching By ALL

                webDriver.FindElement(By.XPath("//*[@id='requestFilter']")).Clear();

                webDriver.FindElement(By.XPath("//*[@id='s2id_statusFilter']/a")).Click();
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[1]/div")).Click();

                webDriver.FindElement(By.XPath("//*[@id='technicianPayments']/div[2]/div[2]/div[1]/button")).Click();

                Thread.Sleep(1000);

                var tableRowsForAll = webDriver.FindElements(By.CssSelector("#paymentView table tbody .ng-scope"));
                var rowWithSrNumForAll = tableRowsForAll.First(ee => ee.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));
                var totalFromTableForAll = rowWithSrNumForAll.FindElement(By.CssSelector(" td:nth-child(6)")).Text.Replace("$", String.Empty).Trim().Split('.')[0];
                var totalCostForAll = Int32.Parse(transportCost.Trim()) + Int32.Parse(additionalCost.Trim());
                Assert.IsTrue(totalCost == Int32.Parse(totalFromTable));
                var initialStatusForAll = rowWithSrNumForAll.FindElement(By.CssSelector(" td:nth-child(9)")).Text;
                Assert.AreEqual("Pending", initialStatus);




            }
        }



    }
}
