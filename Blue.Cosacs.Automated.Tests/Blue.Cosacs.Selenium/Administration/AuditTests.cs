using MbUnit.Framework;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium.Administration.Helpers;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;

namespace Blue.Cosacs.Selenium.Administration
{
    [TestFixture]
    public class AuditTests
    {
        [Test]
        public void CheckEvenDataTableColumnHeaders()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Audit", "Audit", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Business Events Audit");
                webDriver.CheckEventTableHeaders();
            }
        }

        [Test]
        public void CheckAuditForCurrentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Audit", "Audit", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Business Events Audit");
                webDriver.SearchForAudit(session.Username, "100");
                webDriver.IsTextPresent(By.CssSelector(".event:nth-child(1) .by"), session.Username);
            }
        }

        [Test]
        public void CheckAuditForDifferentUser()
        {
            using (var session = Session.Get())
            {
                var webDriver = session.WebDriver;
                webDriver.WaitForElementPresent(By.LinkText("Administration"));
                webDriver.GoTo("Administration", "Audit", "Audit", session);
                webDriver.HasPermission(session);
                webDriver.WaitForTextPresent(By.Id("page-heading"), "Business Events Audit");
                webDriver.SearchForAudit("99999", "100");
                webDriver.IsTextPresent(By.CssSelector(".event:nth-child(1) .by"), "99999");
            }
        }
    }
}
