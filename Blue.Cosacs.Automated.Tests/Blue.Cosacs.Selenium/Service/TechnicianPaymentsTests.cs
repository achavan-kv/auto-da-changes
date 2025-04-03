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

namespace Blue.Cosacs.Selenium.Service
{

    [TestFixture]
    public class TechnicianPaymentsTests
    {


        [Test]

        public void TechnicianPaymentsDefaultView()
        {

            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                {
                    webDriver.GoTo("Service", "Technician Payments", "Service/TechnicianPayments", session);
                    webDriver.WaitForElementPresent(By.CssSelector("#page-heading"));
                    webDriver.IsElementPresent(By.CssSelector("#techFilter"));
                    webDriver.IsElementPresent(By.CssSelector(".col-lg-6.techPaymentsTechs"));
                    webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[1]/div[1]/div"));
                    webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[1]/div[2]/div"));
                    webDriver.FindElement(By.XPath("//*[@id='s2id_statusFilter']/a"));
                    webDriver.FindElement(By.XPath("//*[@id='requestFilter']"));
                    webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[2]/button"));
                    webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[3]/button"));
                    webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[4]/button"));
                    webDriver.FindElement(By.XPath("//*[@id='paymentView']/table/tbody/tr/td/div[2]"));
                    webDriver.FindElement(By.CssSelector("#paymentView"));


                }
            }
        }


        [Test]

        public void TechnicianPaymentsAllSearchTests()
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
                webDriver.MakePayment("Cash", "250");

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

                webDriver.CompleteAllocationSection("Selenium Technician External");

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

                webDriver.MakePayment("Cash", "152.50");
                webDriver.SaveSR();

                //Finalise
                webDriver.FindElement(By.LinkText("Finalise")).Click();
                webDriver.FindElement(By.CssSelector("#s2id_FinalisedFailure")).Click();
                webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Cosmetic Defect']")).Click();
                webDriver.FindElement(By.Name("FinaliseReturnDate")).Click();
                webDriver.SelectTodayFromDatePicker();
                Thread.Sleep(5000);
                webDriver.CLickButtonByText("button", "Save");
                Thread.Sleep(5000);


                webDriver.GoTo("Service", "Technician Payments", "Service/TechnicianPayments", session);
                webDriver.FindElement(By.XPath("//*[@id='techFilter']")).Clear();
                webDriver.FindElement(By.XPath("//*[@id='techFilter']")).SendKeys("Selenium Technician External");
                var techname = webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[1]/table/tbody/tr/td[2]/a")).Text;
                Assert.AreEqual("Selenium Technician External", techname);
                //Searching for a Particular Technician                
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[1]/table/tbody/tr/td[1]/button")).Click();
                Assert.IsTrue(webDriver.FindElement(By.XPath(".//*[@id='allTechnicianPayments']/div[2]/div[1]")).Text.Contains(techname));


                //Date Search
                webDriver.FindElement(By.XPath("//*[@id='paymentsFrom']")).Click();
                webDriver.SelectYesterdayFromDatePicker();
                webDriver.FindElement(By.XPath("//*[@id='paymentsTo']")).Click();
                webDriver.SelectTodayFromDatePicker();
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[2]/button")).Click();
                Thread.Sleep(1000);
                var tableRows = webDriver.FindElements(By.CssSelector("#paymentView table tbody .ng-scope"));
                var rowWithSrNum = tableRows.First(ee => ee.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));
                Assert.IsTrue(rowWithSrNum.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));


                //Holdon Status        

                var holdUnHoldButton = rowWithSrNum.FindElement(By.CssSelector("#paymentView table tbody .ng-scope td:nth-child(7)"));
                var statusCheckBox = rowWithSrNum.FindElement(By.CssSelector("#paymentView table tbody .ng-scope td:nth-child(8)"));
                var deleteButton = rowWithSrNum.FindElement(By.CssSelector("#paymentView table tbody .ng-scope td:nth-child(1)"));
                var statusFilter = webDriver.FindElement(By.XPath("//*[@id='s2id_statusFilter']/a"));
                var searchBox = webDriver.FindElement(By.XPath("//*[@id='requestFilter']"));
                holdUnHoldButton.Click();
                statusFilter.Click();
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[3]/div")).Click();
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[2]/button")).Click();
                Thread.Sleep(1000);
                var tableRowsForHoldOn = webDriver.FindElements(By.CssSelector("#paymentView table tbody .ng-scope"));
                var rowWithSrNumForHoldOn = tableRowsForHoldOn.First(ee => ee.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));
                var holdUnHoldButton1 = rowWithSrNumForHoldOn.FindElement(By.CssSelector("#paymentView table tbody .ng-scope td:nth-child(7)"));
                var statusCheckBox1 = rowWithSrNumForHoldOn.FindElement(By.CssSelector("#paymentView table tbody .ng-scope td:nth-child(8)"));
                var deleteButton1 = rowWithSrNumForHoldOn.FindElement(By.CssSelector("#paymentView table tbody .ng-scope td:nth-child(1)"));
                Assert.IsTrue(rowWithSrNumForHoldOn.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));
                Assert.IsTrue(statusCheckBox1.Text.Contains("Held"));

                //paid
                holdUnHoldButton1.Click();
                statusCheckBox1.Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[4]/button")).Click();
                statusFilter.Click();
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[4]/div")).Click();
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[2]/button")).Click();
                Thread.Sleep(5000);
                var tableRowsForPaid = webDriver.FindElements(By.CssSelector("#paymentView table tbody .ng-scope"));
                var rowWithSrNumForPaid = tableRowsForPaid.First(ee => ee.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));
                Assert.IsTrue(rowWithSrNumForPaid.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));
                var statusCheckBox2 = rowWithSrNumForPaid.FindElement(By.CssSelector("#paymentView table tbody .ng-scope td:nth-child(8)"));
                var deleteButton2 = rowWithSrNumForPaid.FindElement(By.CssSelector("#paymentView table tbody .ng-scope td:nth-child(1)"));
                Assert.IsTrue(statusCheckBox2.Text.Contains("Paid"));
            }

        }





  //      [Test]

        public void TechnicianPaymentsDeleteTests()
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

                webDriver.MakePayment("Cash", "250");

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

                webDriver.FindElement(By.XPath("//*[@id='s2id_techy']/a/span")).Click();
                webDriver.WaitForElementPresent(By.XPath("//*[@id='select2-drop']/ul/li[34]/div"));
                webDriver.WaitForElementPresent(By.XPath("//*[@id='select2-drop']/ul/li[34]/div"));
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[35]/div")).Click();
                Random r = new Random();
                int n = r.Next(1, 30);

                webDriver.ScrollElementInToView(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child("+n+") > td:nth-child(1)"));
                webDriver.FindElement(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child("+n+") > td:nth-child(1)")).Click();
                webDriver.SaveSR();
                Thread.Sleep(5000);

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
                Thread.Sleep(5000);
                webDriver.CLickButtonByText("button", "Save");
                Thread.Sleep(5000);

                //Technician Payments Screen

                webDriver.GoTo("Service", "Technician Payments", "Service/TechnicianPayments", session);
                webDriver.FindElement(By.XPath("//*[@id='techFilter']")).Clear();
                webDriver.FindElement(By.XPath("//*[@id='techFilter']")).SendKeys("Selenium Technician External");
                //Searching for a Particular Technician 
                var techname = webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[1]/table/tbody/tr/td[2]/a")).Text;
                Assert.AreEqual("Selenium Technician External", techname);
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[1]/table/tbody/tr/td[1]/button")).Click();
                Assert.IsTrue(webDriver.FindElement(By.XPath(".//*[@id='allTechnicianPayments']/div[2]/div[1]")).Text.Contains(techname));


                //Searching for the record to delete


                webDriver.FindElement(By.CssSelector("#requestFilter")).Clear();
                webDriver.FindElement(By.CssSelector("#requestFilter")).SendKeys(srNumber);
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[2]/button")).Click();

                var tableRowsForDelete = webDriver.FindElements(By.CssSelector("#paymentView table tbody .ng-scope"));
                var rowWithSrNumForDelete = tableRowsForDelete.First(ee => ee.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));
                Assert.IsTrue(rowWithSrNumForDelete.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));

                
                webDriver.IsElementPresent(By.XPath("//*[@id='paymentView']/table/tbody/tr[1]/td[1]/div"));
                webDriver.FindElement(By.XPath("//*[@id='paymentView']/table/tbody/tr[1]/td[1]/div")).Click();
                Thread.Sleep(5000);


           //     Assert.IsTrue(tableRowsForDelete.Count == 0);

                var statusCheckBox3 = rowWithSrNumForDelete.FindElement(By.XPath("//*[@id='paymentView']/table/tbody/tr[1]/td[8]/div"));
                Assert.IsTrue(statusCheckBox3.Text.Contains("Deleted"));


            }



        }


    //    [Test]

        public void TechnicianPaymentsPrint()
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

                webDriver.MakePayment("Cash", "250");

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

                webDriver.FindElement(By.XPath("//*[@id='s2id_techy']/a/span")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[35]/div")).Click();
                webDriver.ScrollElementInToView(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1)"));
                webDriver.FindElement(By.CssSelector("td.text-center:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1)")).Click();
                webDriver.SaveSR();
                Thread.Sleep(5000);

                // Resolution



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
                Thread.Sleep(5000);
                webDriver.CLickButtonByText("button", "Save");
                Thread.Sleep(5000);

                //Technician Payments Screen

                webDriver.GoTo("Service", "Technician Payments", "Service/TechnicianPayments", session);
                webDriver.FindElement(By.XPath("//*[@id='techFilter']")).Clear();
                webDriver.FindElement(By.XPath("//*[@id='techFilter']")).SendKeys("Selenium Technician External");
                //Searching for a Particular Technician 
                var techname = webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[1]/table/tbody/tr/td[2]/a")).Text;
                Assert.AreEqual("Selenium Technician External", techname);
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[1]/table/tbody/tr/td[1]/button")).Click();
                Assert.IsTrue(webDriver.FindElement(By.XPath(".//*[@id='allTechnicianPayments']/div[2]/div[1]")).Text.Contains(techname));


                //Search Records to print

                webDriver.FindElement(By.XPath("//*[@id='paymentsFrom']")).Click();
                webDriver.SelectYesterdayFromDatePicker();
                webDriver.FindElement(By.XPath("//*[@id='paymentsTo']")).Click();
                webDriver.SelectTodayFromDatePicker();
                webDriver.FindElement(By.XPath("//*[@id='allTechnicianPayments']/div[2]/div[2]/div[2]/div[2]/button")).Click();
                Thread.Sleep(1000);
                var tableRows = webDriver.FindElements(By.CssSelector("#paymentView table tbody .ng-scope"));
                var rowWithSrNum = tableRows.First(ee => ee.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));
                Assert.IsTrue(rowWithSrNum.FindElement(By.CssSelector("td a")).Text.Contains(srNumber));


                var srNumbers = tableRows;

                //Print 

                List<string> numbers = new List<string>();
                foreach (var number in srNumbers)
                {
                    numbers.Add(number.Text);

                }


                var currentWindow = webDriver.CurrentWindowHandle;

                webDriver.FindElement(By.CssSelector(".btn.external-link")).Click();

                Thread.Sleep(2000);

                var windowHandles = webDriver.WindowHandles;

                foreach (var handle in windowHandles)
                {

                    if (handle != currentWindow)
                    {
                        webDriver.SwitchTo().Window(handle);
                        break;
                    }
                }             












            }
        }



    }
}