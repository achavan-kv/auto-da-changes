using System.Diagnostics;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using System.Linq;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class PickingPage
    {
        public static void SelectDeliveryLocationInPickingPage(this IWebDriver webDriver, string deliveryLocation)
        {
            webDriver.SelectDeliveryLocation(deliveryLocation);
            webDriver.FindElement(By.CssSelector("button.search")).Click();
            Sleep(1000);
        }

        public static void IsPrintingGroupCriteriaDropDownLoaded(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("#printAllType_chzn")).Displayed);
        }

        public static void IsPrintingGroupCriteriaDropDownPlaceholderLoaded(this IWebDriver webDriver)
        {
            Assert.AreEqual("Printing Group Criteria", webDriver.FindElement(By.CssSelector("#printAllType_chzn .chzn-default")).Text);
        }
        
        public static void IsSearchButtonLoaded(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button.search")).Displayed);
        }
        
        public static void IsPrintAllButtonLoaded(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button#printAll")).Displayed);
        }

        public static void IsPrintTruckButtonLoaded(this IWebDriver webDriver)
        {
            Assert.IsTrue(webDriver.FindElement(By.CssSelector("button#printByTruck")).Displayed);
        }

        public static void ClickPrintTruckButton(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector("button#printByTruck")).Click();
            webDriver.WaitForElementPresent(By.CssSelector("#confirm"));
        }

        public static string GetBookingNoOfFirstBookingInPickingPage(this IWebDriver webDriver)
        {
            var bookingNo = webDriver.FindElement(By.CssSelector("#bookings :nth-child(1) .Booking-Id")).Text.ToString();
            var hash = new char[] { '#' };
            bookingNo = bookingNo.TrimStart(hash);
            Debug.WriteLine(bookingNo);
            return bookingNo;
        }

        public static void OpenPickListInNewWindow(this IWebDriver webDriver)
        {
            if (webDriver.ElementPresent(By.LinkText("new pick list")))
            {
                webDriver.OpenLinkInNewWindow(By.LinkText("new pick list"));
            }
            else if (webDriver.ElementPresent(By.LinkText("new set of pick lists")))
            {
                webDriver.OpenLinkInNewWindow(By.LinkText("new set of pick lists"));
            }
        }

        public static void CheckPickListCreatedAlert(this IWebDriver webDriver)
        {
            var Alert = webDriver.SwitchTo().Alert();
            var PicklistAlert = Alert.Text;
        }

        public static string GetPickListNo(this IWebDriver webDriver)
        {
            var picklistNo = webDriver.Url.Split('/');
            if (picklistNo.Count() == 8)
                return picklistNo[7];
            else
                return picklistNo[6];
        }

        public static string PickListNoInPrintOut(this IWebDriver webDriver, string pickList)
        {
            var pickListNo = pickList.PadLeft(8, '0');
            return pickListNo;
        }

        public static string GetOriginalItemQuantity(this IWebDriver webDriver, string bookingNo)
        {
            var quantity = webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .itemQuantity")).Text.ToString();
            return quantity;
        }

        public static string GetItemQuantityAfterAddedToTruck(this IWebDriver webDriver)
        {
            var quantity = webDriver.FindElement(By.CssSelector("#pickingItems .pickingItem:first-child .qty")).Text.ToString();
            return quantity;
        }

        public static void CheckIfBookingQuantityIsSame(this IWebDriver webDriver, string originalQuantity)
        {            
            var quantityAfterAddedToTruck = webDriver.GetItemQuantityAfterAddedToTruck();
            Assert.AreEqual(originalQuantity, quantityAfterAddedToTruck);
        }

        public static void AddBookingToTruck(this IWebDriver webDriver, string bookingNo)
        {
            webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .action-pick")).Click();
            Sleep(1000);
            if (webDriver.FindElement(By.CssSelector("#confirm p")).Text.Length > 0)
                webDriver.ClickCancelOnAddRelatedStockPrompt();
            Sleep(2000);
        }

        public static void AddFirstBookingToTruck(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector("#bookings :nth-child(1) .action-pick")).Click();
            Sleep(1000);
            if (webDriver.ElementPresent(By.CssSelector("#confirm")) == true)
                webDriver.ClickCancelOnAddRelatedStockPrompt();
            Sleep(1000);
        }

        public static void RemoveFirstBookingFromTruck(this IWebDriver webDriver)
        {
            var element = webDriver.FindElement(By.CssSelector("#pickingItems .pickingItem:first-child .action-unpick"));
            var removeBooking = new Actions(webDriver);
            removeBooking.MoveToElement(element).Click().Build().Perform();
            Sleep(1000);
        }

        public static string GetSelectedTruckName(this IWebDriver webDriver)
        {
            var truck = webDriver.FindElement(By.CssSelector("#trucks_chzn span")).Text;
            return truck;
        }

        public static void AddBookingToTruckAndCreatePickList(this IWebDriver webDriver, out string bookingNo, out string originalQuantity, out string selectedTruckName, out string pickListNo, string deliveryLocation, out string delCol, string currentUser)
        {
            webDriver.SelectDeliveryLocationInPickingPage(deliveryLocation);
            bookingNo = webDriver.GetBookingNoOfFirstBookingInPickingPage();
            originalQuantity = webDriver.GetOriginalItemQuantity(bookingNo);
            delCol = webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .del-col")).Text;
            webDriver.AddBookingToTruck(bookingNo);
            webDriver.CheckIfBookingQuantityIsSame(originalQuantity);
            selectedTruckName = webDriver.GetSelectedTruckName();
            var splitTruckName = selectedTruckName.Split(' ');
            var truckName = splitTruckName[0];
            webDriver.ClickPrintTruckButton();
            Sleep(2000);
            if (webDriver.FindElement(By.CssSelector("#confirm p")).Text.Contains("No pick lists were printed as there are currently no pending items for branch"))
            {
                webDriver.ClickCssSelector(".ok");
                pickListNo = null;
                return;
            }
            else
            webDriver.OpenPickListInNewWindow();
            webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
            if (webDriver.PageSource.Contains("There are no pick lists to print probably because you haven't selected any bookings for picking or all of them have a different stock location."))
            {
                pickListNo = webDriver.GetPickListNo();
                webDriver.ClosePopupWindow();
                webDriver.ClickCssSelector(".ok");
                return;
            }
            else
            webDriver.IsElementPresent(By.ClassName("document-id"));
            pickListNo = webDriver.GetPickListNo();
            webDriver.CheckPickList(webDriver.PickListNoInPrintOut(pickListNo), currentUser, deliveryLocation, truckName, originalQuantity);
            webDriver.ClosePopupWindow();
            webDriver.ClickCssSelector(".ok");
        }

        public static void AddCollectionToTruckAndCreatePickList(this IWebDriver webDriver, out string bookingNo, out string originalQuantity, out string selectedTruckName, string deliveryLocation)
        {
            webDriver.SelectDeliveryLocationInPickingPage(deliveryLocation);
            webDriver.SelectProductCategory("Vision");
            webDriver.FindElement(By.CssSelector("button.search")).Click();
            Sleep(1000);
            bookingNo = webDriver.GetBookingNoOfFirstBookingInPickingPage();
            originalQuantity = webDriver.GetOriginalItemQuantity(bookingNo);
            webDriver.AddBookingToTruck(bookingNo);
            webDriver.IsElementPresent(By.CssSelector("#pickingItems .pickingItem:first-child .action-unpick"));
            selectedTruckName = webDriver.GetSelectedTruckName();
            webDriver.ClickPrintTruckButton();
            Sleep(2000);
            webDriver.ClickCssSelector(".ok");
        }

        public static void ClickCancelOnAddRelatedStockPrompt(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector("button.cancel")).Click();
        }

        public static void SelectPrintGroupCriteria(this IWebDriver webDriver, string label)
        {
            webDriver.SelectFromDropDown("#printAllType", label);
        }

        public static void ClickPrintAllButton(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.CssSelector("#printAll")).Click();
        }

        public static void SelectProductCategory(this IWebDriver webDriver, string category)
        {
            webDriver.SelectFromDropDown("[name='productCategory']", category);
        }

        public static void SelectCourtsAsFascia(this IWebDriver webDriver)
        {
            webDriver.SelectFromDropDown("[name='fascia']", "Courts Store");
        }

        public static void SelectNonCourtsAsFascia(this IWebDriver webDriver)
        {
            webDriver.SelectFromDropDown("[name='fascia']", "Non Courts Store");
        }

        public static void SelectDeliveryZone(this IWebDriver webDriver, string zone)
        {
            webDriver.SelectFromDropDown("[name='deliveryZone']", zone);
        }

        public static void CheckBookingCount(this IWebDriver webDriver, string text)
        {
            webDriver.IsTextPresent(By.Id("bookingCount"), text);
        }

        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}

