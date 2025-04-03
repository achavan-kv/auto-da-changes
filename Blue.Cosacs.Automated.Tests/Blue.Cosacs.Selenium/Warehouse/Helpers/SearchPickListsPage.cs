using System.Diagnostics;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Blue.Selenium;
using System;
using Blue.Cosacs.Selenium.Common;
using System.Threading;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class SearchPickListsPage
    {
        public static void AreSearchFiltersInSearchPickListsPageLoaded(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'StockBranchName']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'Trucks']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'PickingEmployees']")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.CssSelector(".facet-field[data-field = 'PickingStatus']")).Displayed);
        }

        public static void CheckSearchFilterNamesInSearchPickListsPage(this IWebDriver webDriver)
        {
            Assert.AreEqual("Stock Branch", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(1) > div.section")).Text);
            Assert.AreEqual("Trucks", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(2) > div.section")).Text);
            Assert.AreEqual("Employees", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(3) > div.section")).Text);
            Assert.AreEqual("Status", webDriver.FindElement(By.CssSelector(".searchFields > div:nth-child(4) > div.section")).Text);
        }

        public static void SearchPickList(this IWebDriver webDriver, string pickListNo, string stockBranch, string status)
        {
            webDriver.FindElement(By.ClassName("text-search")).Clear();
            Sleep(1000);
            webDriver.FindElement(By.ClassName("text-search")).SendKeys(pickListNo);
            Sleep(500);
            webDriver.ApplyFacetFilters("StockBranchName", stockBranch);
            webDriver.ApplyFacetFilters("PickingStatus", status);
            Sleep(500);
            webDriver.WaitForElementPresent(By.CssSelector("div[data-id = '" + pickListNo + "']"));
        }

        public static void OpenPickListConfirmation(this IWebDriver webDriver, string pickListNo)
        {
            webDriver.FindElement(By.CssSelector("div[data-id = '" + pickListNo + "']")).Click();
            Sleep(1000);
        }

        public static void SelectPickedBy(this IWebDriver webDriver, string label)
        {
            webDriver.FindElement(By.CssSelector("div.view.pickedBy.chosenwidth.checkPick > div > a > div > b")).Click();
            webDriver.WaitForElementPresent(By.XPath("//div[contains(@class, 'select2-drop')]/ul/li/div[text()='" + label + "']"));
            webDriver.FindElement(By.XPath("//div[contains(@class, 'select2-drop')]/ul/li/div[text()='" + label + "']")).Click();
            Sleep(500);
        }

        public static void SelectCheckedBy(this IWebDriver webDriver, string label)
        {
            webDriver.FindElement(By.CssSelector("div.view.checkedBy.chosenwidth.checkConfirm > div > a > div > b")).Click();
            webDriver.WaitForElementPresent(By.XPath("//div[contains(@class, 'select2-drop')]/ul/li/div[text()='" + label + "']"));
            webDriver.FindElement(By.XPath("//div[contains(@class, 'select2-drop')]/ul/li/div[text()='" + label + "']")).Click();
            Sleep(500);
        }

        public static void SavePickList(this IWebDriver webDriver)
        {
            webDriver.ScrollElementInToView(By.CssSelector("button#save"));
            webDriver.FindElement(By.CssSelector("button#save")).Click();
            Sleep(2000);
        }

        public static string GetBookingNoFromPickList(this IWebDriver webDriver, string originalBookingNo)
        {
            var bookingNo = webDriver.FindElement(By.CssSelector("tr.booking[data-id='" + originalBookingNo + "'] > td.Booking-Columnn")).Text.ToString();
            var hash = new char[]{'#'};
            bookingNo = bookingNo.TrimStart(hash);
            return bookingNo;
        }

        public static void CheckBookingNoInPickList(this IWebDriver webDriver, string bookingNo)
        {
            Assert.AreEqual(bookingNo, webDriver.GetBookingNoFromPickList(bookingNo));
        }

        public static string GetProductQuantityFromPickList(this IWebDriver webDriver, string bookingNo)
        {
            var quantity = webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .itemQuantity")).Text.ToString();
            return quantity;
        }

        public static void IsBookingQuantitySameInPickList(this IWebDriver webDriver, string originalQuantity, string bookingNo)
        {
            var quantityAfterPicked = webDriver.GetProductQuantityFromPickList(bookingNo);
            Assert.AreEqual(originalQuantity, quantityAfterPicked);
        }

        public static string GetPickedQuantity(this IWebDriver webDriver)
        {
            var rejectedQuantity = webDriver.FindElement(By.CssSelector(".booking > td:nth-child(6) > .qty")).Text.ToString();
            return rejectedQuantity;
        }

        public static void CheckConfirmedPickListColumnHeaders(this IWebDriver webDriver)
        {
            Assert.AreEqual("Created", webDriver.FindElement(By.CssSelector(".created.title.cell")).Text);
            Assert.AreEqual("Picked", webDriver.FindElement(By.CssSelector(".picked.title.cell")).Text);
            Assert.AreEqual("Checked", webDriver.FindElement(By.CssSelector(".checked.title.cell")).Text);
            Assert.AreEqual("Confirmed", webDriver.FindElement(By.CssSelector(".confirmed.title.cell")).Text);
            Assert.AreEqual("Trucks", webDriver.FindElement(By.CssSelector(".title.cell:nth-child(5)")).Text);
            Assert.AreEqual("Branch", webDriver.FindElement(By.CssSelector(".title.cell:nth-child(6)")).Text);
            Assert.AreEqual("Items", webDriver.FindElement(By.CssSelector(".title.cell:nth-child(7)")).Text);
        }

        public static void CheckCreatedPickListColumnHeaders(this IWebDriver webDriver)
        {
            Assert.AreEqual("Created", webDriver.FindElement(By.CssSelector(".created.title.cell")).Text);
            Assert.AreEqual("Trucks", webDriver.FindElement(By.CssSelector(".title.cell:nth-child(5)")).Text);
            Assert.AreEqual("Branch", webDriver.FindElement(By.CssSelector(".title.cell:nth-child(6)")).Text);
            Assert.AreEqual("Items", webDriver.FindElement(By.CssSelector(".title.cell:nth-child(7)")).Text);
        }

        public static string TruckName(this IWebDriver webDriver, string pickListNo)
        {
            var truckName = webDriver.GetText(By.CssSelector("[data-id='" + pickListNo + "'] .trucks"));
            if (truckName.Contains(","))
            {
                string[] split = truckName.Split(',');
                var name = split.GetValue(0).ToString();
                return name;
            }
            else
                return truckName;
        }

        public static void PickItemsAndConfirmPickList(this IWebDriver webDriver, string pickListNo, string bookingNo, string originalQuantity, string stockBranch, string status)
        {
            webDriver.SearchPickList(pickListNo, stockBranch, status);
            webDriver.OpenPickListConfirmation(pickListNo);
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Pick List #" + pickListNo + " Confirmation");
            webDriver.CheckBookingNoInPickList(bookingNo);
            int today = (int)DateTime.Now.Day;
            var yesterday = (today - 1).ToString();
            Assert.IsFalse(webDriver.ElementPresent(By.LinkText(yesterday)));
            webDriver.SelectPickedBy("Selenium Tester1 : 221065");
            webDriver.SelectCheckedBy("Selenium Tester1 : 221065");
            webDriver.SavePickList();
            webDriver.IsBookingQuantitySameInPickList(originalQuantity, bookingNo);
            Assert.IsFalse(webDriver.FindElement(By.CssSelector("button#save")).Displayed);
            Assert.AreEqual("All Item(s) Picked Successfully", webDriver.FindElement(By.CssSelector(".booking > td:nth-child(6)")).Text);
        }

        public static void ConfirmPickList(this IWebDriver webDriver, string pickListNo, string stockBranch, string status)
        {
            webDriver.SearchPickList(pickListNo, stockBranch, status);
            webDriver.OpenPickListConfirmation(pickListNo);
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Pick List #" + pickListNo + " Confirmation");
            webDriver.SelectPickedBy("Selenium Tester1 : 221065");
            webDriver.SelectCheckedBy("Selenium Tester1 : 221065");
            
            var element = webDriver.FindElement(By.CssSelector("#save")); //added sk
            ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].scrollIntoView(true);",element); //added sk
            element.Click();  //added sk

            Thread.Sleep(2000);
    //        webDriver.SavePickList(); //sk
     //       Assert.IsFalse(webDriver.FindElement(By.CssSelector("button#save")).Displayed); //sk
            Assert.AreEqual("All Item(s) Picked Successfully", webDriver.FindElement(By.CssSelector(".booking > td:nth-child(6)")).Text);
        }

        public static void SelectPickingRejectionReason(this IWebDriver webDriver, string label, string originalBookingNo)
        {
            webDriver.SelectFromDropDown("[data-id='" + originalBookingNo + "'] [name='Items.RejectionReason']", label);
        }

        public static void SelectPickedQuantity(this IWebDriver webDriver, string label, string originalBookingNo)
        {
            webDriver.SelectFromDropDown("[data-id='" + originalBookingNo + "'] [name='Items.Quantity']", label);
        }

        public static void RejectBookingInPickListConfirmationScreen(this IWebDriver webDriver, string pickListNo, string bookingNo, out string pickedQuantity,
                                                                        string originalQuantity, out int oriqty, out int picqty, out string rejectedQuantity, string stockBranch, string status)
        {            
            webDriver.SearchPickList(pickListNo, stockBranch, status);
            webDriver.OpenPickListConfirmation(pickListNo);
            webDriver.WaitForTextPresent(By.Id("page-heading"), "Pick List #" + pickListNo + " Confirmation");
            webDriver.CheckBookingNoInPickList(bookingNo);
            webDriver.SelectPickedBy("Selenium Tester1 : 221065");
            webDriver.SelectCheckedBy("Selenium Tester1 : 221065");
            oriqty = int.Parse(originalQuantity);
            if (oriqty == 1)
            {
                webDriver.SelectPickingRejectionReason("Picking Rejected", bookingNo);
                webDriver.SavePickList();
            }
            else if (oriqty > 1)
            {
                webDriver.SelectPickingRejectionReason("Picking Rejected", bookingNo);
                webDriver.SelectPickedQuantity("1 Picked ", bookingNo);
                webDriver.SavePickList();
            }
            pickedQuantity = webDriver.GetPickedQuantity();
            picqty = int.Parse(pickedQuantity);
            int rejqty = oriqty - picqty;
            rejectedQuantity = rejqty.ToString();
            Assert.AreNotEqual(originalQuantity, pickedQuantity);
            Assert.AreEqual(pickedQuantity, webDriver.GetProductQuantityFromPickList(bookingNo));
            Assert.AreEqual("Picking Rejected", webDriver.FindElement(By.CssSelector("[data-id = '" + bookingNo + "'] .reasonDisplay")).Text);
        }

        public static void RePrintPickList(this IWebDriver webDriver)
        {
            webDriver.ClickCssSelector("#reprint");
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "You have chosen to reprint the pick list. If you continue, this will be audited. Would you like to reprint the pick list?");
            webDriver.OpenLinkInNewWindow(By.CssSelector(".ok"));
        }

        public static void CheckPickList(this IWebDriver webDriver, string pickListNo, string currentUser, string deliveryLocation, string truckName, string quantity)
        {
            webDriver.IsTextPresent(By.ClassName("ref"), "Pick List #" + pickListNo);
            webDriver.IsTextPresent(By.ClassName("createdBy"), currentUser);
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:first-child > th"), "Picking Branch:");
            webDriver.IsTextPresent(By.CssSelector(".branchNo"), deliveryLocation);
            webDriver.IsTextPresent(By.CssSelector(".row > .main > tbody > tr.item > td:nth-child(4)"), truckName);
            webDriver.IsTextPresent(By.ClassName("qty"), quantity);
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}

