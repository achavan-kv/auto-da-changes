using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;

namespace Blue.Cosacs.Selenium.Administration.Helpers
{
    public static class AuditPage
    {
        public static void CheckEventTableHeaders(this IWebDriver webDriver)
        {
            webDriver.IsTextPresent(By.CssSelector(".table > thead > tr > th:nth-child(1)"), "ID");
            webDriver.IsTextPresent(By.CssSelector(".table > thead > tr > th:nth-child(2)"), "Event On");
            webDriver.IsTextPresent(By.CssSelector(".table > thead > tr > th:nth-child(3)"), "Event By");
            webDriver.IsTextPresent(By.CssSelector(".table > thead > tr > th:nth-child(4)"), "Client Address");
            webDriver.IsTextPresent(By.CssSelector(".table > thead > tr > th:nth-child(5)"), "Category");
            webDriver.IsTextPresent(By.CssSelector(".table > thead > tr > th:nth-child(6)"), "Type");
            webDriver.IsTextPresent(By.CssSelector(".table > thead > tr > th:nth-child(7)"), "Data");
        }

        public static void ClickSearchButtonInAuditPage(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector("button.search")).Click();
            Sleep(1000);
        }

        public static void SearchForAudit(this IWebDriver webDriver, string userName, string maxResults)
        {
            webDriver.SelectDateRange();
            webDriver.IsElementPresent(By.Id("EventBy"));
            webDriver.FindElement(By.Id("EventBy")).SendKeys(userName);
            webDriver.FindElement(By.Id("Top")).Clear();
            webDriver.FindElement(By.Id("Top")).SendKeys(maxResults);
            webDriver.ClickSearchButtonInAuditPage();
        }

        public static void SelectDateRange(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.Id("DateStart"));
            webDriver.IsElementPresent(By.Id("DateEnd"));
            webDriver.FindElement(By.Id("DateStart")).Clear();
            webDriver.FindElement(By.Id("DateStart")).SendKeys("2012-06-01");
            webDriver.FindElement(By.Id("DateEnd")).Clear();
            webDriver.FindElement(By.Id("DateEnd")).SendKeys(System.DateTime.Now.ToString("yyyy-MM-dd"));
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}
