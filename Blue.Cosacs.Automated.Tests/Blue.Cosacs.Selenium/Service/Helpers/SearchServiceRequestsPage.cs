using System.Diagnostics;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Service.Helpers
{
    public static class SearchServiceRequestsPage
    {
        public static void SearchSR(this IWebDriver webDriver, string srNo, string srType, string status)
        {
            webDriver.FindElement(By.ClassName("text-search")).Clear();
            Sleep(1000);
            webDriver.FindElement(By.ClassName("text-search")).SendKeys(srNo);
            webDriver.ApplyFacetFilters("SRType", srType);
            webDriver.ApplyFacetFilters("ServiceStatus", status);
            Sleep(500);
            if (srNo != "")
            {
                webDriver.WaitForElementPresent(By.CssSelector("div[data-request-id='" + srNo + "']"));
            }
            else
                webDriver.WaitForElementPresent(By.CssSelector(".search-result:first-child"));
        }

        public static string GetSRNoOfSearchResult(this IWebDriver webDriver)
        {
            var srNo = webDriver.FindElement(By.CssSelector(".search-result:first-child")).GetAttribute("data-request-id");
            return srNo;
        }

        public static void CheckSRStatus(this IWebDriver webDriver, string srNo, string status, string srType)
        {
            webDriver.SearchSR(srNo, srType, status);
            webDriver.ApplyFacetFilters("ServiceStatus", status);
            webDriver.CheckSearchResultInSearchSRPage(srNo);
            Assert.AreEqual(status, webDriver.FindElement(By.CssSelector("[data-request-id='" + srNo + "'] .status")).Text);
        }

        public static void CheckSearchResultInSearchSRPage(this IWebDriver webDriver, string originalsrNo)
        {
                Assert.AreEqual(originalsrNo, webDriver.GetSRNoOfSearchResult());
        }

        public static void OpenSR(this IWebDriver webDriver, string srNo, string status)
        {
            webDriver.FindElement(By.CssSelector("[data-request-id = '" + srNo + "']")).Click();
            webDriver.WaitForTextPresent(By.CssSelector("#service-heading"), "Service Request " + srNo + " [" + status +"]");
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
