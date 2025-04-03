using System;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Blue.Selenium;
using System.Data.SqlClient;
using Blue.Cosacs.Selenium.Administration.Helpers;
using System.Data;
using System.Collections;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace Blue.Cosacs.Selenium.Common
{
    public static class CosacsCommonExtensions
    {
        public static void LoginToCosacs(this IWebDriver webDriver, string username, string password)
        {
            webDriver.WaitForElementPresent(By.CssSelector("#username"));
            webDriver.ClearTextInputField(By.CssSelector("#username"));
            webDriver.ClearTextInputField(By.CssSelector("#password"));
            webDriver.Type(By.CssSelector("#username"), username);
            webDriver.Type(By.CssSelector("#password"), password);
            webDriver.CLickButtonByText("button", "Log In");
            if (webDriver.ElementPresent(By.CssSelector("#divError")) == false)
            {
                webDriver.WaitForElementPresent(By.CssSelector("#logoff"));
                Sleep(500);
            }
            else
                Sleep(500);
        }

        public static void LoginAsNewUser(this IWebDriver webDriver, string username, string password, string newPassword)
        {
            webDriver.ClearTextInputField(By.CssSelector("#username"));
            webDriver.ClearTextInputField(By.CssSelector("#password"));
            webDriver.Type(By.CssSelector("#username"), username);
            webDriver.Type(By.CssSelector("#password"), password);
            webDriver.ClickCssSelector(".login");
            webDriver.WaitForElementPresent(By.CssSelector("#confirmPassword"));
            webDriver.IsElementPresent(By.CssSelector("#newPassword"));
            webDriver.Type(By.CssSelector("#newPassword"), newPassword);
            webDriver.Type(By.CssSelector("#confirmPassword"), newPassword);
            webDriver.ClickCssSelector(".login");
            webDriver.WaitForElementPresent(By.CssSelector("#logoff"));
            Sleep(500);
        }

        public static void LogOffAndLoginAsCurrentUser(this IWebDriver webDriver, Session session)
        {
            webDriver.FindElement(By.CssSelector("#logoff")).Click();
            webDriver.WaitForElementPresent(By.CssSelector("#username"));
            webDriver.LoginToCosacs(session.Username, session.Password);
        }

        public static void HasPermission(this IWebDriver webDriver, Session session)
        {
            Sleep(500);
            if (webDriver.ElementPresent(By.CssSelector("#page-heading")) && webDriver.GetText(By.CssSelector("#page-heading")) == "Forbidden")
                webDriver.LogOffAndLoginAsCurrentUser(session);
        }

        public static void RecoverPassword(this IWebDriver webDriver, string activeUserName)
        {
            webDriver.ClickCssSelector("#aForgetPassword");
            Sleep(1000);
            webDriver.WaitForElementPresent(By.CssSelector("#inputChangePassword"));
            webDriver.IsElementPresent(By.CssSelector("button#buttonCancel"));
            webDriver.IsElementPresent(By.CssSelector("button#buttonRecover"));
            webDriver.FindElement(By.Id("inputChangePassword")).SendKeys(activeUserName);
            webDriver.FindElement(By.CssSelector("button#buttonRecover")).Click();
            Sleep(1000);
        }

        public static void CheckResetPasswordEmail(this IWebDriver webDriver, out string resetPasswordLink, Session session)
        {
            webDriver.Navigate().GoToUrl("http://www.gmail.com");
            webDriver.WaitForElementPresent(By.Id("Passwd"));
            if (webDriver.ElementPresent(By.CssSelector("#reauthEmail")))
            {
                webDriver.FindElement(By.Id("Passwd")).SendKeys("$Selenium2012");
                webDriver.FindElement(By.Id("signIn")).Click();
            }
            else
            {
                webDriver.FindElement(By.Id("Email")).SendKeys("selenium.bbs");
                webDriver.FindElement(By.Id("Passwd")).SendKeys("$Selenium2012");
                webDriver.FindElement(By.Id("signIn")).Click();
            }
            webDriver.WaitForElementPresent(By.PartialLinkText("Inbox"));
            webDriver.IsElementPresent(By.PartialLinkText("Inbox"));
            webDriver.IsElementPresent(By.PartialLinkText("CosacsCurrent - Forgot Password"));
            if (session.BaseUrl.Contains("192.168.30.170"))
            {
                webDriver.FindElement(By.PartialLinkText("Inbox")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.ae4.aDM > div.Cp > div > table > tbody > tr.zA.zE"));
                webDriver.IsElementPresent(By.CssSelector("div.ae4.aDM > div.Cp > div > table > tbody > tr.zA.zE"));
                webDriver.FindElement(By.CssSelector("div.ae4.aDM > div.Cp > div > table > tbody > tr.zA.zE")).Click();
                Sleep(500);
            }
            else
            {
                webDriver.FindElement(By.PartialLinkText("CosacsCurrent - Forgot Password")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.ae4.UI.UJ > div.Cp > div > table > tbody > tr.zA.zE"));
                webDriver.IsElementPresent(By.CssSelector("div.ae4.UI.UJ > div.Cp > div > table > tbody > tr.zA.zE"));
                webDriver.FindElement(By.CssSelector("div.ae4.UI.UJ > div.Cp > div > table > tbody > tr.zA.zE")).Click();
                Sleep(500);
            }
            webDriver.IsTextPresent(By.CssSelector("div.adn.ads > div.gs > div.ii.gt.adP.adO > div > p"), "We have received a request to reset your password.");
            webDriver.IsElementPresent(By.CssSelector("div.adn.ads > div.gs > div.ii.gt.adP.adO > div > a[href*='Login/RecoverPassword']"));
            resetPasswordLink = webDriver.FindElement(By.CssSelector("div.adn.ads > div.gs > div.ii.gt.adP.adO > div > a[href*='Login/RecoverPassword']")).Text.ToString();
            webDriver.IsElementPresent(By.CssSelector("[title*='selenium.bbs@gmail.com']"));
            webDriver.FindElement(By.CssSelector("[title*='selenium.bbs@gmail.com']")).Click();
            webDriver.IsElementPresent(By.LinkText("Sign out"));
            webDriver.FindElement(By.LinkText("Sign out")).Click();
            webDriver.WaitForElementPresent(By.Id("Passwd"));
        }


        public static void CheckResetPwdEmail(this IWebDriver webDriver, out string resetPasswordLink, Session session)
        {
            webDriver.Navigate().GoToUrl("http://www.gmail.com");
            webDriver.WaitForElementPresent(By.Id("Passwd"));
                webDriver.FindElement(By.Id("Email")).SendKeys("selenium.bbs");
                webDriver.FindElement(By.Id("Passwd")).SendKeys("$Selenium2012");
                webDriver.FindElement(By.Id("signIn")).Click();
            
            webDriver.WaitForElementPresent(By.PartialLinkText("Inbox"));
            webDriver.IsElementPresent(By.PartialLinkText("Inbox"));
            webDriver.IsElementPresent(By.PartialLinkText("CosacsCurrent - Forgot Password"));
            if (session.BaseUrl.Contains("192.168.30.170"))
            {
                webDriver.FindElement(By.PartialLinkText("Inbox")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.ae4.aDM > div.Cp > div > table > tbody > tr.zA.zE"));
                webDriver.IsElementPresent(By.CssSelector("div.ae4.aDM > div.Cp > div > table > tbody > tr.zA.zE"));
                webDriver.FindElement(By.CssSelector("div.ae4.aDM > div.Cp > div > table > tbody > tr.zA.zE")).Click();
                Sleep(500);
            }
            else
            {
                webDriver.FindElement(By.PartialLinkText("CosacsCurrent - Forgot Password")).Click();
                webDriver.WaitForElementPresent(By.CssSelector("div.ae4.UI.UJ > div.Cp > div > table > tbody > tr.zA.zE"));
                webDriver.IsElementPresent(By.CssSelector("div.ae4.UI.UJ > div.Cp > div > table > tbody > tr.zA.zE"));
                webDriver.FindElement(By.CssSelector("div.ae4.UI.UJ > div.Cp > div > table > tbody > tr.zA.zE")).Click();
                Sleep(500);
            }
            webDriver.IsTextPresent(By.CssSelector("div.adn.ads > div.gs > div.ii.gt.adP.adO > div > p"), "We have received a request to reset your password.");
            webDriver.IsElementPresent(By.CssSelector("div.adn.ads > div.gs > div.ii.gt.adP.adO > div > a[href*='Login/RecoverPassword']"));
            resetPasswordLink = webDriver.FindElement(By.CssSelector("div.adn.ads > div.gs > div.ii.gt.adP.adO > div > a[href*='Login/RecoverPassword']")).Text.ToString();
            webDriver.IsElementPresent(By.CssSelector("[title*='selenium.bbs@gmail.com']"));
            webDriver.FindElement(By.CssSelector("[title*='selenium.bbs@gmail.com']")).Click();
            webDriver.IsElementPresent(By.LinkText("Sign out"));
            webDriver.FindElement(By.LinkText("Sign out")).Click();
            webDriver.WaitForElementPresent(By.Id("Passwd"));
        }


        public static void ResetPassword(this IWebDriver webDriver, string newPassword)
        {
            webDriver.WaitForElementPresent(By.Name("confirmPassword"));
            webDriver.IsElementPresent(By.Name("newPassword"));
            webDriver.FindElement(By.Name("newPassword")).SendKeys(newPassword);
            webDriver.FindElement(By.Name("confirmPassword")).SendKeys(newPassword);
            webDriver.FindElement(By.CssSelector(".login")).Click();
            webDriver.WaitForElementPresent(By.Id("logoff"));
        }

        public static void IsDeliveryLocationDropDownLoaded(this IWebDriver webDriver)
        {
            try
            {
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("div#deliveryBranch_chzn > a.chzn-single")).Displayed);
            }
            catch (Exception e)
            {
                throw new Exception("Delivery Location dropdown is not displayed", e.InnerException);
            }
        }

        public static void IsTrucksDropDownLoaded(this IWebDriver webDriver)
        {
            try
            {
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("#trucks_chzn > a.chzn-single")).Displayed);
            }
            catch (Exception e)
            {
                throw new Exception("Trucks dropdown is not displayed", e.InnerException);
            }
        }

        public static void IsSearchBarPresent(this IWebDriver webDriver)
        {
            try
            {
                Assert.IsTrue(webDriver.FindElement(By.ClassName("text-search")).Displayed);
            }
            catch (Exception e)
            {
                throw new Exception("Search bar is not displayed", e.InnerException);
            }
        }

        public static void IsClearButtonPresent(this IWebDriver webDriver)
        {
            try
            {
                Assert.IsTrue(webDriver.FindElement(By.CssSelector("button.btn.clear")).Displayed);
            }
            catch (Exception e)
            {
                throw new Exception("Clear button is not displayed", e.InnerException);
            }
        }

        public static void CheckWarehouseMenus(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.LinkText("Search Shipments"));
            webDriver.IsElementPresent(By.LinkText("Picking"));
            webDriver.IsElementPresent(By.LinkText("Search Pick Lists"));
            webDriver.IsElementPresent(By.LinkText("Scheduling"));
            webDriver.IsElementPresent(By.LinkText("Search Delivery Schedules"));
            webDriver.IsElementPresent(By.LinkText("Customer Pick Up"));
            webDriver.IsElementPresent(By.LinkText("Drivers"));
            webDriver.IsElementPresent(By.LinkText("Trucks"));
        }

        public static void CheckAdministrationMenus(this IWebDriver webDriver)
        {
            webDriver.IsElementPresent(By.LinkText("Search Users"));
            webDriver.IsElementPresent(By.LinkText("Roles"));
            webDriver.IsElementPresent(By.LinkText("Sessions"));
            webDriver.IsElementPresent(By.LinkText("Blocked Clients"));
            webDriver.IsElementPresent(By.LinkText("Create User"));
            webDriver.IsElementPresent(By.LinkText("Audit"));
            
        }

        public static void SelectTodayFromDatePicker(this IWebDriver webDriver)
        {
            Thread.Sleep(1000);
            webDriver.FindElement(By.CssSelector("div#ui-datepicker-div > table.ui-datepicker-calendar > tbody > tr > td.ui-datepicker-today")).Click();
        }

        public static void SelectTomorrowFromDatePicker(this IWebDriver webDriver)
        {
            int today = (int)DateTime.Now.Day;
            var tomorrow = (DateTime.Today.AddDays(+1)).ToString("dd").TrimStart('0');
            webDriver.FindElement(By.LinkText(tomorrow)).Click();
        }

        public static void SelectDayAfterTomorrowFromDatePicker(this IWebDriver webDriver)
        {
            int today = (int)DateTime.Now.Day;
            var dayAfterTomorrow = (DateTime.Today.AddDays(+2)).ToString("dd").TrimStart('0');
            var todayDate = DateTime.Today.ToString("dd").TrimStart('0');

            if (webDriver.FindElement(By.LinkText(todayDate)).Text == "30" || webDriver.FindElement(By.LinkText(todayDate)).Text == "31")
            {
                webDriver.FindElement(By.CssSelector(".ui-datepicker-next.ui-corner-all")).Click();
                webDriver.FindElement(By.LinkText(dayAfterTomorrow)).Click();
            }
            else
            {
                webDriver.FindElement(By.LinkText(dayAfterTomorrow)).Click();
            }


                 }


        public static void SelectDayFromTodaysDate(this IWebDriver webDriver,int numberOfDaysToAdd ) 
        { 
            var dayToAdd = (DateTime.Today.AddDays(+numberOfDaysToAdd)).ToString("dd").TrimStart('0');
            var todayDate = DateTime.Today.ToString("dd").TrimStart('0');

            if (webDriver.FindElement(By.LinkText(todayDate)).Text == "30" || webDriver.FindElement(By.LinkText(todayDate)).Text == "31")
            {
                webDriver.FindElement(By.CssSelector(".ui-datepicker-next.ui-corner-all")).Click();
                webDriver.FindElement(By.LinkText(dayToAdd)).Click();
            }
            else
            {
                webDriver.FindElement(By.LinkText(dayToAdd)).Click();
            }
        }


        public static void SelectDaySpecifiedDateFromDatePicker(this IWebDriver webDriver,int day)
        {
            int today = (int)DateTime.Now.Day;
            var dayAfterTomorrow = (DateTime.Today.AddDays(+day)).ToString("dd").TrimStart('0');
            webDriver.FindElement(By.LinkText(dayAfterTomorrow)).Click();
        }

        public static void SelectYesterdayFromDatePicker(this IWebDriver webDriver)
        {
            var today = (int)DateTime.Now.Day;
            var yesterday = (DateTime.Today.AddDays(-1)).ToString("dd").TrimStart('0');
            if (today == 1)
            {
                webDriver.FindElement(By.ClassName("ui-datepicker-prev")).Click();
                webDriver.FindElement(By.LinkText(yesterday)).Click();
            }
            else
                webDriver.FindElement(By.LinkText(yesterday)).Click();
        }

        public static void ClickAdministrationMenu(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.LinkText("Administration")).Click();
        }

        public static void ClickLogisticsMenu(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.LinkText("Logistics")).Click();
        }

        public static void ClickConfigurationMenu(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.LinkText("Configuration")).Click();
        }

        public static void GoTo(this IWebDriver webDriver, string menu, string subMenu, string url, Session currentSession)
        {
            var baseUrl = currentSession.BaseUrl;
            var userName = currentSession.Username;
            var password = currentSession.Password;
            var browserName = currentSession.BrowserName;

            webDriver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 5));

            #region FireFox
            if (browserName == "firefox")
            {
                webDriver.Navigate().GoToUrl(baseUrl + url);
                if (webDriver.ElementPresent(By.Name("username")) && webDriver.FindElement((By.Name("username"))).Displayed && webDriver.ElementPresent(By.Name("password")) && webDriver.FindElement(By.Name("password")).Displayed == true)
                {
                    webDriver.LoginToCosacs(userName, password);
                    webDriver.WaitForElementPresent(By.Id("logoff"));
                }
            }
            #endregion

            #region Munu Displayed
            else if (webDriver.ElementPresent(By.LinkText(menu)) == true)
            {
                if (webDriver.ElementPresent(By.CssSelector("div.modal-backdrop.fade.in")) == true)
                {
                    webDriver.Navigate().GoToUrl(baseUrl + url);
                    if (webDriver.ElementPresent(By.Id("username")) && webDriver.FindElement((By.Id("username"))).Displayed && webDriver.ElementPresent(By.Id("password")) && webDriver.FindElement(By.Id("password")).Displayed == true)
                    {
                        webDriver.LoginToCosacs(userName, password);
                        webDriver.WaitForElementPresent(By.Id("logoff"));
                    }
                }
                else
                {
                    webDriver.FindElement(By.LinkText(menu)).Click();
                    if (webDriver.ElementPresent(By.LinkText(subMenu)) == true)
                    {
                        webDriver.FindElement(By.LinkText(subMenu)).Click();
                    }
                    else
                    {
                        webDriver.Navigate().GoToUrl(baseUrl + url);
                        if (webDriver.ElementPresent(By.Id("username")) && webDriver.FindElement((By.Id("username"))).Displayed && webDriver.ElementPresent(By.Id("password")) && webDriver.FindElement(By.Id("password")).Displayed == true)
                        {
                            webDriver.LoginToCosacs(userName, password);
                            webDriver.WaitForElementPresent(By.Id("logoff"));
                        }
                    }
                    Sleep(1000);
                }
            }
            #endregion

            #region Menu Not Displayed
            else
            {
                webDriver.Navigate().GoToUrl(baseUrl + url);
                if (webDriver.ElementPresent(By.Id("username")) && webDriver.FindElement((By.Id("username"))).Displayed && webDriver.ElementPresent(By.Id("password")) && webDriver.FindElement(By.Id("password")).Displayed == true)
                {
                    webDriver.LoginToCosacs(userName, password);
                    webDriver.WaitForElementPresent(By.Id("logoff"));
                }
            }
            #endregion
        }

        public static void GoTo(this IWebDriver webDriver, By element, string url, Session currentSession)
        {
            var baseUrl = currentSession.BaseUrl;
            var userName = currentSession.Username;
            var password = currentSession.Password;

            webDriver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 0, 0, 500));
            if (webDriver.ElementPresent(element) == true)
            {
                if (webDriver.ElementPresent(By.CssSelector("div.modal-backdrop.fade.in")) == true)
                {
                    webDriver.Navigate().GoToUrl(baseUrl + url);
                    if (webDriver.ElementPresent(By.Id("username")) && webDriver.FindElement((By.Id("username"))).Displayed && webDriver.ElementPresent(By.Id("password")) && webDriver.FindElement(By.Id("password")).Displayed == true)
                    {
                        webDriver.LoginToCosacs(userName, password);
                        webDriver.WaitForElementPresent(By.Id("logoff"));
                    }
                }
                else
                {
                    webDriver.FindElement(element).Click();
                }
            }
            else
            {
                webDriver.Navigate().GoToUrl(baseUrl + url);
                if (webDriver.ElementPresent(By.Id("username")) && webDriver.FindElement((By.Id("username"))).Displayed && webDriver.ElementPresent(By.Id("password")) && webDriver.FindElement(By.Id("password")).Displayed == true)
                {
                    webDriver.LoginToCosacs(userName, password);
                    webDriver.WaitForElementPresent(By.Id("logoff"));
                }
            }
        }

        public static void CheckLegends(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.ClassName("rejected")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.ClassName("locked")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.ClassName("cancelled")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.ClassName("express")).Displayed);
        }

        public static void SelectDeliveryLocation(this IWebDriver webDriver, string deliveryLocation)
        {
            webDriver.SelectFromDropDown("[name='deliveryBranch']", deliveryLocation);
            Sleep(1000);
        }

        public static void SelectTruck(this IWebDriver webDriver, string truck)
        {
            webDriver.SelectFromDropDown("[id='trucks']", truck);
        }

        public static void TurnOffPasswordComplexity(this IWebDriver webDriver)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        var sqlScript = "UPDATE CountryMaintenance SET Value = 0 WHERE CodeName IN ('MinRequiredNonalphanumericChar', 'PasswordMinLength')";
                        var command = new SqlCommand(sqlScript, con, tran);
                        command.CommandType = System.Data.CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        System.Console.WriteLine(e.Message);
                        return;
                    }
                    tran.Commit();
                }
            }
        }

        public static void TurnOnPasswordComplexity(this IWebDriver webDriver)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        var sqlScript = "UPDATE CountryMaintenance SET Value = 2 WHERE CodeName = 'MinRequiredNonalphanumericChar'"
                                        + "UPDATE CountryMaintenance SET Value = 5 WHERE CodeName = 'PasswordMinLength'";
                        var command = new SqlCommand(sqlScript, con, tran);
                        command.CommandType = System.Data.CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        System.Console.WriteLine(e.Message);
                        return;
                    }
                    tran.Commit();
                }
            }
        }

        public static  List<string> GetCommonPasswords(this IWebDriver webDriver)
        {
            List<string> list = new List<string>();
            using (var con = new System.Data.SqlClient.SqlConnection())
            {
                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        var sqlScript = "SELECT TOP 25 * FROM Admin.CommonPassword ORDER BY NEWID()";
                        var command = new System.Data.SqlClient.SqlCommand(sqlScript, con, tran);
                        command.CommandType = System.Data.CommandType.Text;
                        command.ExecuteNonQuery();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            list = (from IDataRecord r in reader select (string)r["value"]).ToList();
                        }
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        System.Console.WriteLine(e.Message);
                        return null;
                    }
                    tran.Commit();
                }
            }
            return list;
        }

        public static string QueryDataBase(this IWebDriver webDriver, string sqlStatement, string columnName)
        {
            var list = string.Empty;
            using (var con = new System.Data.SqlClient.SqlConnection())
            {
                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        var sqlScript = sqlStatement;
                        var command = new System.Data.SqlClient.SqlCommand(sqlScript, con, tran);
                        command.CommandType = System.Data.CommandType.Text;
                        command.ExecuteNonQuery();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            list = reader[columnName].ToString();
                        }
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        System.Console.WriteLine(e.Message);
                        return null;
                    }
                    tran.Commit();
                }
            }
            return list;
        }

        public static void CheckEventAudit(this IWebDriver webDriver, Session session , string auditEvent)
        {
            webDriver.GoTo("Administration", "Audit", "Audit", session);
            webDriver.HasPermission(session);
            webDriver.WaitForElementPresent(By.CssSelector("div.events > table.data"));
            webDriver.SearchForAudit(session.Username, "1");
        }

        public static string GetUserProfileId(this IWebDriver webDriver, string userName)
        {
            var Id = webDriver.QueryDataBase("SELECT Id FROM Admin.[User] WHERE Login = '" + userName + "'", "Id");
            return Id;
        }

        public static string Today(this IWebDriver webDriver)
        {
            DateTime date = DateTime.Today;
            var today = date.ToString("dddd dd-MMMM-yyyy");
            return today;
        }

        public static void ApplyFacetFilters(this IWebDriver webDriver, string facetField, string value)
        {
            Sleep(500);
            webDriver.FindElement(By.XPath("//ul[@data-field = '" + facetField + "']/li[contains(text(), '" + value + "')]")).Click();
            Sleep(1000);
        }

        public static void CloseNotification(this IWebDriver webDriver)
        {
            webDriver.WaitForElementPresent(By.ClassName("growlstatus-close"));
            webDriver.FindElement(By.ClassName("growlstatus-close")).Click();
            Sleep(1000);
        }

        public static void CheckNotification(this IWebDriver webDriver, string message)
        {
            webDriver.WaitForTextPresent(By.ClassName("growlstatus"), message);
        }

        public static void MoveToElement(this IWebDriver webDriver, By selector)
        {
            var element = webDriver.FindElement(selector);
            Actions moveToElement = new Actions(webDriver);
            moveToElement.MoveToElement(element).Perform();
        }

        public static void ScrollElementInToView(this IWebDriver webDriver, By locator)
        {
            var element = webDriver.FindElement(locator);
            ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].scrollIntoView(true); window.scrollBy(0,-50);", element);
        }



        /// <summary>
        /// caviet: http://stackoverflow.com/questions/18218743/continue-the-while-loop-when-executescriptsetinterval-is-finished
        /// </summary>
        /// <param name="webDriver"></param>
        public static void ScrollToEndofGrowingPage(this IWebDriver webDriver)
        {
            IJavaScriptExecutor js = webDriver as IJavaScriptExecutor;
            bool run = true;
            while (run)
            {
                System.Threading.Thread.Sleep(5000);
                run = (bool)js.ExecuteScript("if(window.scrollY<(document.body.scrollHeight-window.screen.availHeight)){window.scrollTo(0,document.body.scrollHeight);return true;}else{window.scrollTo(0,0);return false;}");
            }

            webDriver.ScrollToEndOfPage();
        }


        public static void OpenLinkInNewWindow(this IWebDriver webDriver, By locator)
        {
            Actions shiftClick = new Actions(webDriver);
            shiftClick.MoveToElement(webDriver.FindElement(locator)).KeyDown(Keys.Shift).Click().Build().Perform();
            shiftClick.KeyUp(Keys.Shift).Perform();
        }

        public static void SelectFromDropDown(this IWebDriver webDriver, string dropdownLocator, string option)
        {
            var id = webDriver.FindElement(By.CssSelector(dropdownLocator)).GetAttribute("id");
            id = "#" + id + "_chzn";
            webDriver.FindElement(By.CssSelector(id)).Click();
            webDriver.FindElement(By.CssSelector(id + " input")).SendKeys(option);
            webDriver.FindElement(By.CssSelector(id + " input")).SendKeys(Keys.Enter);
            Sleep(500);
        }

        // added

            public static void ClickUsingJavaScript(this IWebDriver webDriver, IWebElement element)
    {

        var jse = webDriver as IJavaScriptExecutor;

        const string script = @"arguments[0].click()";

        jse.ExecuteScript(script, element);
    }

        public static void DropDownSelection(this IWebDriver webdriver, string locator, string value)
        {
            new SelectElement(webdriver.FindElement(By.Id(locator))).SelectByValue(value);
        }


        public static void CLickButtonByText(this IWebDriver webDriver, string tag, string text)
        {
            IList list = webDriver.FindElements(By.TagName(tag));
            foreach (IWebElement item in list)
            {
                if (item.Displayed && item.Text == text)
                {
                    item.Click();
                    return;
                }
            }
        }

        public static IWebElement FindButtonByText(this IWebDriver webDriver, string tag, string text)
        {
            IList list = webDriver.FindElements(By.TagName(tag));
            foreach (IWebElement item in list)
            {
                if (item.Displayed && item.Text == text)
                {
                    return item;
                }
            }
            return null;
        }

        public static void ClickPrimaryButton(this IWebDriver webDriver)
        {
            webDriver.ClickCssSelector(".btn-primary");
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

    }
}
