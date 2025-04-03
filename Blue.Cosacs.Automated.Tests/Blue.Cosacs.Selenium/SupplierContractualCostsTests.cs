using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Selenium
{

    [TestFixture]

    public class SupplierContractualCostsTests
    {
        private int defaultCount;
       
        [Test]

        public void VerifyDefaultView()
        {         

            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "Supplier Contractual Costs", "Service/SupplierCosts", session);
                webDriver.HasPermission(session);
                webDriver.FindElement(By.CssSelector("#page-heading")).Text.Contains("Supplier Contractual Costs");
                webDriver.IsElementPresent(By.CssSelector(".col-lg-1.control-label"));
                webDriver.IsElementPresent(By.CssSelector(".select2-choice.select2-default"));
                webDriver.IsElementPresent(By.CssSelector(".form-control.ng-pristine.ng-valid.ng-valid-number"));
                webDriver.IsElementPresent(By.CssSelector(".btn.btn-default"));
                webDriver.IsElementPresent(By.CssSelector(".btn.btn-primary"));
                webDriver.IsSaveButtonDisabled();

           }
        }


        [Test]

        public void SupplierContractualCostsE2E()  
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "Supplier Contractual Costs", "Service/SupplierCosts", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#page-heading"));
                webDriver.FindElement(By.CssSelector(".select2-choice")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[3]/div")).Click();
                var  supplierCostsTable = webDriver.FindElements(By.CssSelector(".supplierCostsTable.table >tbody>tr"));
                var defaultRowCount = supplierCostsTable.Count;
                defaultCount = defaultRowCount;
                webDriver.FindElement(By.CssSelector("div.col-lg-1:nth-child(1) > input:nth-child(1)")).Clear();
                webDriver.FindElement(By.CssSelector("div.col-lg-1:nth-child(1) > input:nth-child(1)")).SendKeys("1");
                var addRowsButton = webDriver.FindElement(By.XPath("//*[@id='supplierCosts']/div/form/div[2]/div[2]/button"));
                webDriver.ClickUsingJavaScript(addRowsButton);
                var supplierCostsTableAfterAdd = webDriver.FindElements(By.CssSelector(".supplierCostsTable.table >tbody>tr"));
                var  rows = new Dictionary<IWebElement, Int32>();
                var position=0;
                foreach (var row in supplierCostsTableAfterAdd)
                {             
                var rowPosition = Int32.Parse(row.FindElement(By.CssSelector("td:nth-child(2) input")).GetAttribute("name").Split('_')[1]);
                rows.Add(row,rowPosition);                               
                }

               foreach(var record in rows){
               if (record.Value >= position){
               position=record.Value;
               }          
               }

                var tableRow = rows.FirstOrDefault(ee=>ee.Value==position);
 
                var AddedRow = tableRow.Key;




                var r = new Random();


                var tsp = "SeleniumTest" + r.Next(0,100).ToString();
                    
                


                    //product
                        AddedRow.FindElement(By.CssSelector("td:nth-child(2) input")).SendKeys(tsp);
                    // month 
                        AddedRow.FindElement(By.CssSelector("td:nth-child(3)>div>div a")).Click();

                        var months = webDriver.FindElements(By.CssSelector(".select2-results li div"));

                        foreach (var month in months)
                        {
                            if (month.Text == "1 - 12")
                            {
                                month.Click();
                                break;
                            }
                        }
                        

                   // part type
                        AddedRow.FindElement(By.CssSelector("td:nth-child(4) input")).SendKeys("TestSelPartType");

                        // part %

                        AddedRow.FindElement(By.CssSelector("td:nth-child(5) input")).SendKeys("0");
                        
                        //part Value

                        AddedRow.FindElement(By.CssSelector("td:nth-child(6) input")).SendKeys("0");

                        //Labour %

                        AddedRow.FindElement(By.CssSelector("td:nth-child(7) input")).SendKeys("0");

                        //Labour Value

                        AddedRow.FindElement(By.CssSelector("td:nth-child(8) input")).SendKeys("30");

                        //Additional %

                        AddedRow.FindElement(By.CssSelector("td:nth-child(9) input")).SendKeys("0");

                        //Additional Value

                        AddedRow.FindElement(By.CssSelector("td:nth-child(10) input")).SendKeys("0");                
                                 
                  var savebtn = webDriver.FindElement(By.CssSelector(".btn.btn-primary"));

                webDriver.ClickUsingJavaScript(savebtn);

                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));

                wait.Until(d => d.FindElement(By.CssSelector(".growlstatus")).Text.Contains("Supplier Contractual Costs saved successfully"));

                var supplierCostsTableAfterSave = webDriver.FindElements(By.CssSelector(".supplierCostsTable.table >tbody>tr"));

                var RowCountAfterSave = supplierCostsTableAfterSave.Count;

                Assert.IsTrue(RowCountAfterSave == defaultRowCount + 1);



                //Verifying Added product in New Service Request 

                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.HasPermission(session);
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CreateServiceRequestForJobData("SeleniumJob1", "Mr", "Mark", "Taylor", "Address1", "Address2", "Town", "Lo1 3TD", "test@testmail.com", "Electrical", "1 - Vision", "10  - Vision", "12345", "ItemDesc", "Brother", "Brother", "749 COURTS", "5000");
                
                var makeAPaymentBtn = webDriver.FindElement(By.XPath("//*[@id='navBar']/div[3]/div[2]/div/button"));

                webDriver.ClickUsingJavaScript(makeAPaymentBtn);

                 Thread.Sleep(2000);

                //selecting payment method

                 webDriver.FindElement(By.CssSelector(".popup-body.modal-body  .payMethod.form-group a")).Click();
                 Thread.Sleep(1000);
                 var options = webDriver.FindElements(By.CssSelector(".payMethod.form-group select option"));

                 foreach (var option in options)
                 {

                     if (option.Text == "Cash")
                     {
                         option.Click();
                         break;
                     }

                 }
                 webDriver.FindElement(By.XPath("//*[@id='payment-modal']/div/div/div[2]/div[2]/div[7]/div/input")).SendKeys("250");
                 webDriver.FindElement(By.XPath("//*[@id='payment-modal']/div/div/div[3]/button[1]")).Click();
                 webDriver.click_on_cancel_on_pay_pop_up();
                 wait.Until(d => d.FindElement(By.CssSelector(".growlstatus")).Text.Contains("Payment saved successfully"));

                //Resolution

               

                 webDriver.FindElement(By.XPath("//*[@id='navBar']/ul/li[5]/a")).Click();  // Clicking on Resolution Link

                 //Primary Charge
                 webDriver.FindElement(By.XPath("//*[@id='s2id_resolutionPrimaryCharge']/a")).Click();
                 webDriver.FindElement(By.XPath(".//*[@id='select2-drop']/ul/li[4]/div")).Click();

                //Supplier To Charge
                 webDriver.FindElement(By.XPath("//*[@id='s2id_ResolutionSupplierToCharge']/a")).Click();
                 webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[3]/div")).Click();

                //Product Category
                 webDriver.FindElement(By.XPath("//*[@id='s2id_ResolutionCategory']/a")).Click();
                 Thread.Sleep(1000);
                var productCategoryList =  webDriver.FindElements(By.CssSelector("#ResolutionCategory option"));

                List<string> items = new List<string>();

                foreach (var category in productCategoryList)
                {
                    Thread.Sleep(1000);
                    items.Add(category.Text.Trim());
                }

                Assert.IsTrue(items.Contains(tsp));

                

                webDriver.Navigate().Refresh();
                Thread.Sleep(2000);

                webDriver.GoTo("Service", "Supplier Contractual Costs", "Service/SupplierCosts", session);
                webDriver.FindElement(By.CssSelector(".select2-choice")).Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.XPath("//*[@id='select2-drop']/ul/li[3]/div")).Click();
                

                //Delete Row Scenario

                var supplierCostsTbleToDelete = webDriver.FindElements(By.CssSelector(".supplierCostsTable.table >tbody>tr"));

                var allRows = new Dictionary<IWebElement, Int32>();
                var position1 = 0;
                foreach (var row in supplierCostsTbleToDelete)
                {
                    var rowPosition = Int32.Parse(row.FindElement(By.CssSelector("td:nth-child(2) input")).GetAttribute("name").Split('_')[1]);
                    allRows.Add(row, rowPosition);

                }

                foreach (var record in allRows)
                {
                    if (record.Value >= position1)
                    {
                        position1 = record.Value;
                    }
                }

                var tableRowToDelete = allRows.FirstOrDefault(ee => ee.Value == position1);
                tableRowToDelete.Key.FindElement(By.CssSelector("td>a")).Click();
                var savebtn1 = webDriver.FindElement(By.CssSelector(".btn.btn-primary"));
                webDriver.ClickUsingJavaScript(savebtn1);
                wait.Until(d => d.FindElement(By.CssSelector(".growlstatus")).Text.Contains("Supplier Contractual Costs saved successfully"));
                var supplierCostsTableAfterDelete = webDriver.FindElements(By.CssSelector(".supplierCostsTable.table >tbody>tr"));
                var RowCountAfterDelete = supplierCostsTableAfterDelete.Count;

                Assert.IsTrue(RowCountAfterDelete == defaultCount);                      

            }
        }











    }
}