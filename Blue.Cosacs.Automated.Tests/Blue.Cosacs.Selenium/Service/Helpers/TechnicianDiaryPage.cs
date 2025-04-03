using System;
using OpenQA.Selenium;
using MbUnit.Framework;
using Blue.Selenium;
using OpenQA.Selenium.Interactions;
using Blue.Cosacs.Selenium.Common;
using System.Threading;

namespace Blue.Cosacs.Selenium.Service.Helpers
{
    public static class TechnicianDiaryPage
    {
        public static void SelectTechnicianInTechnicianDiaryPage(this IWebDriver webDriver, string label)
        {
            webDriver.IsElementPresent(By.CssSelector(".select2-container.ng-valid.ng-dirty"));
            Sleep(1000);
            webDriver.FindElement(By.CssSelector(".select2-container.ng-valid.ng-dirty")).Click();
            Sleep(500);
            webDriver.WaitForElementPresent(By.XPath("//div/ul/li/div[contains(text(), '" + label + "')]"));
            webDriver.FindElement(By.XPath("//div/ul/li/div[contains(text(), '" + label + "')]")).Click();
        }

        public static void SelectDateOfStartWeekToView(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.CssSelector("input[ui-date]"));
            webDriver.FindElement(By.CssSelector("input[ui-date]")).Click();
            webDriver.SelectTodayFromDatePicker();
        }

        public static void SelectVisibleWeeks(this IWebDriver webDriver, string visibleWeeks)
        {
            webDriver.IsElementPresent(By.CssSelector("[ng-model='displayWeeks']"));
            webDriver.FindElement(By.CssSelector("[ng-model='displayWeeks']")).Clear();
            webDriver.FindElement(By.CssSelector("[ng-model='displayWeeks']")).SendKeys(visibleWeeks);
        }

        public static void DeleteJob(this IWebDriver webDriver, string srNo)
        {
            webDriver.FindElement(By.CssSelector("[title='Delete']")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#dialogueCommon .modal-content h3"), "Delete Booking " + srNo);
            webDriver.FindElement(By.CssSelector("[ng-hide='dialogueCommon.hideSelect'] .select2-container")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Technician taking too long']")).Click();
            webDriver.CLickButtonByText("button", "Delete");
            Sleep(1000);
        }

        public static void RejectJob(this IWebDriver webDriver, string srNo)
        {
            webDriver.WaitForTextPresent(By.CssSelector("#dialogueCommon .modal-content h3"), "Reject Booking " + srNo);
            webDriver.FindElement(By.CssSelector("[ng-hide='dialogueCommon.hideSelect'] .select2-container")).Click();
            webDriver.FindElement(By.XPath("//div/ul/li/div[text()='Leave Requested']")).Click();
            webDriver.CLickButtonByText("button", "Reject");
            Sleep(1000);
        }

        public static void AssignJob(this IWebDriver webDriver)
        {
            //webDriver.FindElement(By.CssSelector("#s2id_availableRequest")).Click();
            //webDriver.FindElement(By.CssSelector("div#select2-drop > ul > li:nth-child(2)")).Click();
            //Sleep(500);
            //webDriver.CLickButtonByText("button", "Assign");
            //Sleep(1000);
        }

        public static void AddTechnicianAvailability(this IWebDriver webDriver)
        {

            webDriver.ScrollToEndOfPage();
            Assert.IsFalse(webDriver.FindButtonByText("button", "Submit").Enabled);

            IWebElement RequestStart=null;
            IWebElement RequestEnd=null;

            var elements = webDriver.FindElements(By.CssSelector(".col-lg-5>.form-control.ng-pristine.ng-valid.hasDatepicker"));

            foreach (var element in elements) {

                if (element.GetAttribute("ng-model").Equals("holiday.requestStart"))
                {

                    RequestStart = element;
                }
                else {

                    RequestEnd = element;
                }

            }

           ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", RequestStart);
           Thread.Sleep(1000);

           RequestStart.Click();

            webDriver.SelectTomorrowFromDatePicker();
            RequestEnd.Click();
            webDriver.SelectDayAfterTomorrowFromDatePicker();
            Thread.Sleep(2000);
            Assert.IsTrue(webDriver.FindButtonByText("button", "Submit").Enabled);
            webDriver.CLickButtonByText("button", "Submit");
           Sleep(1000);
        }

      public static void DeleteTechnicianAvailability(this IWebDriver webDriver)
        {
            webDriver.ScrollElementInToView(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(4) [ng-repeat='slot in day.slots']:nth-child(1)"));
            webDriver.FindElement(By.CssSelector("[ng-repeat='week in weeks']:nth-of-type(2) [ng-repeat='day in week']:nth-child(4) [ng-repeat='slot in day.slots']:nth-child(1)")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#dialogueCommon .modal-content h3"), "Delete Unavailability?");
            webDriver.CLickButtonByText("button", "Delete");
            Sleep(1000);
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
