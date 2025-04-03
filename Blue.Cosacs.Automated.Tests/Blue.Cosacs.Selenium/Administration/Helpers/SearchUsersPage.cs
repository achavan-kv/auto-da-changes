using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Selenium;

namespace Blue.Cosacs.Selenium.Administration.Helpers
{
    public static class SearchUsersPage
    {
        public static void AreSearchFiltersInSearchUsersPageLoaded(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'HomeBranchName']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'Roles']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'Locked']")).Displayed);
        }

        public static void CheckSearchFilterNamesInSearchUsersPage(this IWebDriver webDriver)
        {
            Assert.AreEqual("Default Branch", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(1) > div.section")).Text);
            Assert.AreEqual("Roles", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(2) > div.section")).Text);
            Assert.AreEqual("Locked", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(3) > div.section")).Text);
        }

        public static void SearchForUser(this IWebDriver webDriver, string userFullName, string userName)
        {
            webDriver.FindElement(By.ClassName("text-search")).Clear();
            webDriver.FindElement(By.ClassName("text-search")).SendKeys(userFullName);
            webDriver.WaitForElementPresent(By.XPath("//div[contains(@class, 'login')][contains(text(), '" + userName + "')]"));
            Sleep(1000);
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

    }
}
