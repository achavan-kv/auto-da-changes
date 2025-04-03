using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using Blue.Cosacs.Selenium.Service.Helpers;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace Blue.Cosacs.Selenium
{

    [TestFixture]
    public class ResolutionsTests
    {
        
        [Test]

        public void ResolutionsE2E()
        {

            using (var session = Session.Get())
            {

                var webDriver = session.WebDriver;
                webDriver.GoTo("Service", "Resolutions", "Service/Resolutions", session);
                webDriver.HasPermission(session);
                webDriver.IsElementPresent(By.CssSelector("#page-heading"));
                //Add Resolution
                webDriver.ScrollToEndOfPage();
                webDriver.FindElement(By.CssSelector(".glyphicons.glyph-btn.action-new.plus")).Click();
                webDriver.IsElementPresent(By.CssSelector("#Description"));
                
                
                var r = new Random();
                

                var resolutiontxt = "TestResolution" + r.Next().ToString();
                webDriver.FindElement(By.CssSelector("#Description")).SendKeys(resolutiontxt);
                webDriver.FindElement(By.CssSelector(".glyphicons.glyph-btn.action-update.floppy_disk")).Click();
                
                //search Resolution
                webDriver.FindElement(By.CssSelector("#s_Description")).SendKeys(resolutiontxt);
                Thread.Sleep(1000);
                webDriver.FindElement(By.CssSelector(".btn.btn-default")).Click();
                Thread.Sleep(2000);
                var actualtxt = webDriver.FindElement(By.XPath("//*[@id='center']/div[2]/div/div[1]/table/tbody/tr/td[2]"));
                Assert.AreEqual(resolutiontxt, actualtxt.Text);
                
                //Edit
                webDriver.FindElement(By.CssSelector(".glyphicons.glyph-btn.action-edit.pencil")).Click();
                var editedtext = "EditedTestResolution" + r.Next().ToString();
                webDriver.FindElement(By.CssSelector("#Description")).Clear();
                webDriver.FindElement(By.CssSelector("#Description")).SendKeys(editedtext);
                webDriver.FindElement(By.CssSelector(".glyphicons.glyph-btn.action-update.floppy_disk")).Click();
                Thread.Sleep(1000);
                Assert.AreEqual(editedtext, webDriver.FindElement(By.XPath("//*[@id='center']/div[2]/div/div[1]/table/tbody/tr[2]/td[2]")).Text);


                // Added Resolution Verification in Service Requests screen
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
                var wait = new WebDriverWait(webDriver,TimeSpan.FromSeconds(60));
                wait.Until(d => d.FindElement(By.CssSelector(".growlstatus")).Text.Contains("Payment saved successfully"));

                //Resolution
                webDriver.click_on_cancel_on_pay_pop_up();
                //webDriver.WaitForElementPresent(By.XPath(".//*[@id='paymentReceipt-modal']/div/div/div[3]/button[2]"));//Cancel on pay print pop up
                //webDriver.FindElement(By.XPath(".//*[@id='paymentReceipt-modal']/div/div/div[3]/button[2]")).Click();//Cancel on pay print pop up
                //Thread.Sleep(2000);


                webDriver.FindElement(By.XPath("//*[@id='navBar']/ul/li[5]/a")).Click();

                var ResolutionList = webDriver.FindElements(By.CssSelector("#Resolution option"));

                List<string> items = new List<string>();

                foreach (var category in ResolutionList)
                {
                    items.Add(category.Text);
                }

                Assert.IsTrue(items.Contains(editedtext));
                webDriver.Navigate().Refresh();
                webDriver.GoTo("Service", "Resolutions", "Service/Resolutions", session);
                webDriver.FindElement(By.CssSelector("#s_Description")).SendKeys(editedtext);
                webDriver.FindElement(By.CssSelector(".btn.btn-default")).Click();
                Thread.Sleep(2000);

                //Delete

                webDriver.FindElement(By.XPath("//*[@id='center']/div[2]/div/div[1]/table/tbody/tr/td[1]/a[1]")).Click();
                Thread.Sleep(2000);

                webDriver.FindElement(By.CssSelector(".ok.btn.btn-primary")).Click();
                Thread.Sleep(1000);

                var numberOfRows = webDriver.FindElements(By.CssSelector(".col-lg-8 table tbody tr"));


                Assert.IsTrue(numberOfRows.Count==0);

                 }

        }


       
    }
}

