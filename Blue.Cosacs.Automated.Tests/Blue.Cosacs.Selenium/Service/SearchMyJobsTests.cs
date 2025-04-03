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
  public   class SearchMyJobsTests
    {

        [Test]

        public void CheckSearchMyJobsDefaultView()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs", session);
                webDriver.HasPermission(session);
                webDriver.WaitForElementPresent(By.CssSelector("#page-heading"));

                webDriver.IsElementPresent(By.CssSelector("button.action:nth-child(1)"));
                webDriver.IsElementPresent(By.CssSelector("button.action:nth-child(2)"));
                webDriver.IsElementPresent(By.CssSelector("button.action:nth-child(3)"));
                webDriver.IsElementPresent(By.CssSelector("#searchString"));
                webDriver.IsElementPresent(By.CssSelector("#TechAllocatedDateSearchFrom"));
                webDriver.IsElementPresent(By.CssSelector("#TechAllocatedDateSearchTo"));
                webDriver.IsElementPresent(By.CssSelector("div.text-right:nth-child(3) > button:nth-child(1)"));
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div.facet:nth-child(1) > ul:nth-child(2)")).Displayed);
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div.facet:nth-child(2) > ul:nth-child(2)")).Displayed);
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div.facet:nth-child(3) > ul:nth-child(2)")).Displayed);
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div.facet:nth-child(4) > ul:nth-child(2)")).Displayed);
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div.facet:nth-child(5) > ul:nth-child(2)")).Displayed);
                Assert.AreEqual("Schedule / Repair Date", webDriver.FindElement(By.CssSelector("article.col-lg-2 > div:nth-child(1) > label:nth-child(1)")).Text);
                Assert.AreEqual("Branch", webDriver.FindElement(By.CssSelector("div.facet:nth-child(1) > div:nth-child(1) > label:nth-child(1)")).Text);
                Assert.AreEqual("Status", webDriver.FindElement(By.CssSelector("div.facet:nth-child(2) > div:nth-child(1) > label:nth-child(1)")).Text);
                Assert.AreEqual("Type", webDriver.FindElement(By.CssSelector("div.facet:nth-child(3) > div:nth-child(1) > label:nth-child(1)")).Text);
                Assert.AreEqual("Printed", webDriver.FindElement(By.CssSelector("div.facet:nth-child(4) > div:nth-child(1) > label:nth-child(1)")).Text);
                Assert.AreEqual("Fault Tags", webDriver.FindElement(By.CssSelector("div.facet:nth-child(5) > div:nth-child(1) > label:nth-child(1)")).Text);

            }
        }


       [Test]
        public void VerifyAddedJobsInSearchMyJobsByDefault()
        {
            using (var session = Session.Get()) {

                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.WaitForElementPresent(By.CssSelector("#se"));
                webDriver.HasPermission(session);
                webDriver.SelectSrType("#se");
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CreateServiceRequestForJobData("SeleniumJob1", "Mr", "Mark", "Taylor", "Address1", "Address2","Town", "Lo1 3TD", "test@testmail.com", "Electrical", "1 - Vision", "10  - Vision", "12345", "ItemDesc", "Supplier", "Brother", "749 COURTS", "5000");
                var srNumber = webDriver.GetSRNumber();
                webDriver.CompleteAllocationSection("Selenium Technician External");
                webDriver.FindElement(By.CssSelector("#logoff")).Click();
                Thread.Sleep(1000);
                webDriver.WaitForElementPresent(By.CssSelector("#username"));
                webDriver.LoginToCosacs("seltechext","test**123");
                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs",session);

                webDriver.ScrollToEndofGrowingPage();

                Thread.Sleep(5000);
                var srNumbers = webDriver.FindElements(By.CssSelector(".refLink.ng-binding"));
                List<string> numbers = new List<string>(); 
                foreach (var number in srNumbers)
                {
                numbers.Add(number.Text.Remove(0,1));
                
                }
                Assert.IsTrue(numbers.Contains(srNumber), "srNumber: " + srNumber + "is not displayed");
                srNumber = "#" + srNumber;
                webDriver.ScrollElementInToView(By.LinkText(srNumber));
                webDriver.FindElement(By.LinkText(srNumber)).Click();
                Thread.Sleep(1000);
                webDriver.DeleteCreatedJob();

            }
        }




        [Test]   // Searching the job using search Text Box in the search my jobs screens
        public  void SearchingAParticularJob()
        {
        using (var session = Session.Get())
        {

                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);  
                webDriver.HasPermission(session);
                webDriver.SelectSrType("#se");                  //Creating a job  
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CreateServiceRequestForJobData("SeleniumJob1", "Mr", "Mark", "Taylor", "Address1", "Address2","Town", "Lo1 3TD", "test@testmail.com", "Electrical", "1 - Vision", "10  - Vision", "12345", "ItemDesc", "Supplier", "Brother", "749 COURTS", "5000");
                var srNumber = webDriver.GetSRNumber();   //storing the job Number in SrNumber(the sr number will be included with #)
                webDriver.CompleteAllocationSection("Selenium Technician External");
                webDriver.FindElement(By.CssSelector("#logoff")).Click();
                Thread.Sleep(1000);
                webDriver.WaitForElementPresent(By.CssSelector("#username")); //Logging in as technician
                webDriver.LoginToCosacs("seltechext", "test**123");
                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs", session);
                Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#page-heading"));
                webDriver.FindElement(By.CssSelector("#searchString")).SendKeys(srNumber);  //Passing the sr number to search text box
                srNumber = "#" + srNumber; // Adding # to the SR NUMBEr
                Assert.IsTrue( webDriver.FindElement(By.LinkText(srNumber)).Displayed);
                webDriver.ScrollElementInToView(By.LinkText(srNumber));
                webDriver.FindElement(By.LinkText(srNumber)).Click();
                Thread.Sleep(1000);
                webDriver.DeleteCreatedJob();

            
                }
         }

        
        
        [Test]
        public void SearchJobsAccordingToDateSearch()
               {

               using (var session = Session.Get())
               {
                    
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);  
                webDriver.HasPermission(session);
                webDriver.SelectSrType("#se");                  //Creating  job1   
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");
                webDriver.CreateServiceRequestForJobData("SeleniumJob1", "Mr", "Mark", "Taylor", "Address1", "Address2","Town", "Lo1 3TD", "test@testmail.com", "Electrical", "1 - Vision", "10  - Vision", "12345", "ItemDesc", "Supplier", "Brother", "749 COURTS", "5000");
                var srNumber1 = webDriver.GetSRNumber();
                webDriver.CompleteAllocationSection("Selenium Technician External");
                webDriver.FindElement(By.CssSelector("#logoff")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("#username"));
                webDriver.LoginToCosacs("seltechext", "test**123");
                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs", session);
                Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#page-heading"));

                // Date Search -Ve testcase
                   
                webDriver.FindElement(By.Id("TechAllocatedDateSearchFrom")).Clear();
                webDriver.FindElement(By.Id("TechAllocatedDateSearchFrom")).Click();
                webDriver.SelectDayFromTodaysDate(3);
                webDriver.FindElement(By.Id("TechAllocatedDateSearchTo")).Clear();
                webDriver.FindElement(By.Id("TechAllocatedDateSearchTo")).Click();
                webDriver.SelectDayFromTodaysDate(4);
                Thread.Sleep(1000);
                var srNumbers = webDriver.FindElements(By.CssSelector(".refLink.ng-binding"));
                List<string> numbers = new List<string>();
                foreach (var number in srNumbers)
                {
                    numbers.Add(number.Text.Remove(0, 1));

                }
                Assert.IsFalse(numbers.Contains(srNumber1));

                webDriver.Navigate().Refresh();

                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs", session);
                webDriver.FindElement(By.Id("TechAllocatedDateSearchFrom")).Clear();
                webDriver.FindElement(By.Id("TechAllocatedDateSearchFrom")).Click();
                webDriver.SelectTodayFromDatePicker();
                Thread.Sleep(1000);

                webDriver.FindElement(By.Id("TechAllocatedDateSearchTo")).Clear();
                webDriver.FindElement(By.Id("TechAllocatedDateSearchTo")).Click();
                webDriver.SelectDayAfterTomorrowFromDatePicker();
                Thread.Sleep(2000);

                  Assert.IsTrue(webDriver.FindElement(By.LinkText("#" + srNumber1)).Displayed);
                  webDriver.ScrollElementInToView(By.LinkText("#"+srNumber1));
                  webDriver.FindElement(By.LinkText("#" + srNumber1)).Click();
                  Thread.Sleep(1000);
                  webDriver.DeleteCreatedJob();
        
                 }
               
                }


        [Test]
        public void VerifyFacetSearchFiltersWithStatusBranchType()
        {

            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "New Service Request", "Service/Requests/New", session);
                webDriver.HasPermission(session);
                webDriver.SelectSrType("#se");                  //Creating a job  
                webDriver.IsCreateButtonEnabled();
                webDriver.CLickButtonByText("button", "Create");



                webDriver.CreateServiceRequestForJobData("SeleniumJob1", "Mr", "Mark", "Taylor", "Address1", "Address2", "Town", "Lo1 3TD", "test@testmail.com", "Electrical", "1 - Vision", "10  - Vision", "12345", "ItemDesc", "Supplier", "Brother", "749 COURTS", "5000");
             //   var status = webDriver.FindElement(By.XPath("//*[@id='service-heading']/span[2]")).Text.Replace("[",String.Empty).Replace("]",String.Empty);

                webDriver.CompleteAllocationSection("Selenium Technician External");  //Allocating job to techncian
                var srNumber = webDriver.GetSRNumber();
                

                webDriver.FindElement(By.CssSelector("#logoff")).Click();
                Thread.Sleep(1000);
                webDriver.WaitForElementPresent(By.CssSelector("#username")); //Logging in as technician
                webDriver.LoginToCosacs("seltechext", "test**123");
                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs", session);
                Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#page-heading"));

                var branchNumbers = webDriver.FindElements(By.XPath("//*[@id='facetSearchContainer']/section/div[2]/div/div[1]/div[1]/div[1]/ul/li"));
                foreach (var number in branchNumbers)
                {
                    if (number.Text.Contains("960"))
                    {
                        number.Click();
                        break;
                    }
                }
                Thread.Sleep(5000);

              webDriver.ScrollToEndofGrowingPage();
                

               // webDriver.ScrollElementInToView(By.LinkText("#" + srNumber));
                Assert.IsTrue(webDriver.FindElement(By.LinkText("#" + srNumber)).Displayed);
                
// Facet Branch filter
                webDriver.Navigate().Refresh();
                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs", session);
                Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#page-heading"));


                // Facet Status filter
                var statustexts = webDriver.FindElements(By.XPath("//*[@id='facetSearchContainer']/section/div[2]/div/div[1]/div[1]/div[2]/ul/li"));
                foreach (var status in statustexts)
                {
                    if (status.Text.Contains("Awaiting deposit"))
                    {
                        status.Click();
                        break;
                    }
                }
                Thread.Sleep(3000);

               webDriver.ScrollToEndofGrowingPage();

               // webDriver.ScrollElementInToView(By.LinkText("#" + srNumber));
                Assert.IsTrue(webDriver.FindElement(By.LinkText("#" + srNumber)).Displayed);



                //Search Facet using Type

                webDriver.Navigate().Refresh();
                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs", session);
                Thread.Sleep(1000);
                webDriver.IsElementPresent(By.CssSelector("#page-heading"));

                var Types = webDriver.FindElements(By.XPath("//*[@id='facetSearchContainer']/section/div[2]/div/div[1]/div[1]/div[3]/ul/li"));
               
                foreach (var type in Types)
                {
                    if (type.Text.Contains("Service Request External"))
                    {
                        type.Click();
                        break;
                    }
                }
                Thread.Sleep(1000);
              //  webDriver.ScrollElementInToView(By.LinkText("#" + srNumber));
                webDriver.ScrollToEndofGrowingPage();
                Assert.IsTrue(webDriver.FindElement(By.LinkText("#" + srNumber)).Displayed);

                
                //Delete
                webDriver.ScrollElementInToView(By.LinkText("#" + srNumber));

                webDriver.FindElement(By.LinkText("#" + srNumber)).Click();
                Thread.Sleep(1000);
                webDriver.DeleteCreatedJob();

                }
        }


        
   //    [Test]
        public void VerifySummaryPrintOptions()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.FindElement(By.CssSelector("#logoff")).Click();
                webDriver.LoginToCosacs("seltechext", "test**123");
                webDriver.GoTo("Service", "Search My Jobs", "Service/TechnicianDiaries/SearchMyJobs", session);

          //      webDriver.ScrollToEndofGrowingPage();
                var srNumbers = webDriver.FindElements(By.CssSelector(".refLink.ng-binding"));

               
        
                //Summary Print
                
                
                List<string> numbers = new List<string>();
                foreach (var number in srNumbers)
                {
                    numbers.Add(number.Text.Remove(0, 1));

                }

                var currentWindow = webDriver.CurrentWindowHandle;

   
                webDriver.ScrollElementInToView(By.XPath("//*[@id='facetSearchContainer']/section/div[1]/div[1]/button[1]"));
                webDriver.FindElement(By.XPath("//*[@id='facetSearchContainer']/section/div[1]/div[1]/button[1]")).Click();
                Thread.Sleep(2000);

               var windowHandles = webDriver.WindowHandles;

                foreach (var handle in windowHandles) {

                    if (handle != currentWindow) {
                        webDriver.SwitchTo().Window(handle);
                        break;
                    }
                }             
                
            var printSrNums =    webDriver.FindElements(By.XPath("//table[@class='summaryTable']/tbody/tr/td[1]"));

            List<string> printNums = new List<string>();

            foreach (var num in printSrNums) {

                printNums.Add(num.Text);
            }

            Assert.IsTrue(numbers.Count == printNums.Count);

                    foreach(var num in numbers){
                    Assert.IsTrue(printNums.Contains(num));
                    }

                    webDriver.Close();
            
                //  Batch Print
                webDriver.SwitchTo().Window(currentWindow);
                webDriver.FindElement(By.XPath("//*[@id='facetSearchContainer']/section/div[1]/div[1]/button[2]")).Click();
                    webDriver.FindElement(By.XPath("//*[@id='confirm']/div/div/div[3]/button[1]")).Click();

                    var windowhandles2 = webDriver.WindowHandles;

                    foreach (var handle in windowhandles2)
                    {

                        if (handle != currentWindow)
                        {
                            webDriver.SwitchTo().Window(handle);
                            break;
                        }
                    }


                Thread.Sleep(2000);

                webDriver.ScrollToEndofGrowingPage();

                var batchSrNumbers = webDriver.FindElements(By.CssSelector(".ref"));

                List<string> printBatchTexts = new List<string>() ;

                foreach (var num in batchSrNumbers)
                {
                
                    printBatchTexts.Add(num.Text.Split('#')[1].Remove(0,2));

                }

                Assert.IsTrue(numbers.Count == printBatchTexts.Count);



                foreach (var num in numbers)
                {
                    Assert.IsTrue(printBatchTexts.Contains(num));
                }          
             }

            }          
                
    }
}
