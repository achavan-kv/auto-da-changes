using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using Blue.Selenium;
using Blue.Cosacs.Selenium.Common;
using System.Linq;

namespace Blue.Cosacs.Selenium.Warehouse.Helpers
{
    public static class SchedulingPage
    {
        public static void OpenPrintLoadInNewWindow(this IWebDriver webDriver)
        {
            webDriver.OpenLinkInNewWindow(By.Id("print"));
        }

        public static void ClickCreateDeliverySchedule(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.Id("createDeliverySchedule")).Click();
            Sleep(1000);
        }

        public static void SaveDeliverySchedule(this IWebDriver webDriver)
        {
            webDriver.FindElement(By.Id("saveDeliverySchedule")).Click();
        }

        public static void OpenDeliveryScheduleInNewWindow(this IWebDriver webDriver)
        {
            webDriver.OpenLinkInNewWindow(By.LinkText("new delivery schedule"));
        }

        public static string GetDeliveryScheduleNo(this IWebDriver webDriver)
        {
            var deliveryScheduleNo = webDriver.Url.Split('/');
            if (deliveryScheduleNo.Count() == 8)
                return deliveryScheduleNo[7];
            else
                return deliveryScheduleNo[6];
        }

        public static string GetBookingNoFromLoad(this IWebDriver webDriver, string originalBookingNo)
        {
            var bookingNo = webDriver.FindElement(By.CssSelector("[data-id='" + originalBookingNo + "'] .Booking-Id")).Text.ToString();
            var hash = new char[]{'#'};
            bookingNo = bookingNo.TrimStart(hash);
            return bookingNo;
        }

        public static string GetItemQuantityFromLoad(this IWebDriver webDriver, string bookingNo)
        {
            var quantity = webDriver.FindElement(By.CssSelector("[data-id='" + bookingNo + "'] .itemQuantity")).Text.ToString();
            return quantity;
        }

        public static void IsBookingQuantitySameInLoad(this IWebDriver webDriver, string originalQuantity, string bookingNo)
        {
            var quantityFromLoad = webDriver.GetItemQuantityFromLoad(bookingNo);
            Assert.AreEqual(originalQuantity, quantityFromLoad);
        }

        public static void CheckBookingNoInLoad(this IWebDriver webDriver, string bookingNo)
        {
            Assert.AreEqual(bookingNo, webDriver.GetBookingNoFromLoad(bookingNo));
        }

        public static void PrintLoad(this IWebDriver webDriver, string truckName, string currentUser, string branchNo, string delCol)
        {
            webDriver.OpenPrintLoadInNewWindow();
            webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
            webDriver.IsElementPresent(By.ClassName("document-id"));
            webDriver.CheckLoad(truckName, currentUser, branchNo, delCol);
            webDriver.ClosePopupWindow();
        }

        public static void CreateDeliverySchedule(this IWebDriver webDriver, string selectedDeliveryLocation, string selectedTruckName, string bookingNo, string originalQuantity, out string deliveryScheduleNo, string currentUser, string delCol)
        {
            webDriver.SelectDeliveryLocation(selectedDeliveryLocation);
            webDriver.SelectTruck(selectedTruckName);
            var splitTruckName = selectedTruckName.Split(' ');
            var truckName = splitTruckName[0];
            var driverName = truckName.Replace("Truck", "Driver");
            webDriver.PrintLoad(truckName, currentUser, selectedDeliveryLocation, delCol);
            webDriver.CheckBookingNoInDeliverySchedule(bookingNo);
            webDriver.IsBookingQuantitySameInLoad(originalQuantity, bookingNo);
            webDriver.ClickCreateDeliverySchedule();
            Assert.IsTrue(webDriver.FindElement(By.ClassName("tooltip-inner")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.Id("saveDeliverySchedule")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.Id("cancel")).Displayed);
            webDriver.SaveDeliverySchedule();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "A new delivery schedule has been created and is ready to be printed.");
            webDriver.OpenDeliveryScheduleInNewWindow();
            webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
            webDriver.IsElementPresent(By.ClassName("document-id"));
            deliveryScheduleNo = webDriver.GetDeliveryScheduleNo();
            webDriver.CheckDeliverySchedule(deliveryScheduleNo, truckName, driverName, currentUser, selectedDeliveryLocation, delCol);
            webDriver.ClosePopupWindow();
            webDriver.ClickCssSelector(".ok");
        }

        public static void CreateDeliverySchedule(this IWebDriver webDriver, string selectedDeliveryLocation, string selectedTruckName, out string deliveryScheduleNo)
        {
            webDriver.SelectDeliveryLocation(selectedDeliveryLocation);
            webDriver.SelectTruck(selectedTruckName);
            webDriver.ClickCreateDeliverySchedule();
            Assert.IsTrue(webDriver.FindElement(By.ClassName("tooltip-inner")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.Id("saveDeliverySchedule")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.Id("cancel")).Displayed);
            webDriver.SaveDeliverySchedule();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "A new delivery schedule has been created and is ready to be printed.");
            webDriver.OpenDeliveryScheduleInNewWindow();
            webDriver.SwitchToPopupWindow(By.ClassName("document-id"), webDriver.CurrentWindowHandle);
            webDriver.IsElementPresent(By.ClassName("document-id"));
            deliveryScheduleNo = webDriver.GetDeliveryScheduleNo();
            webDriver.ClosePopupWindow();
            webDriver.ClickCssSelector(".ok");
        }

        public static void SelectLoadRejectionReason(this IWebDriver webDriver, string label, string originalBookingNo)
        {
            webDriver.SelectFromDropDown("[data-id='" + originalBookingNo + "'] [name='Items.RejectionReason']", label);
        }

        public static void RejectBookingInLoad(this IWebDriver webDriver, string selectedDeliveryLocation, string selectedTruckName, string bookingNo, string originalQuantity)
        {
            webDriver.SelectDeliveryLocation(selectedDeliveryLocation);
            webDriver.SelectTruck(selectedTruckName);
            webDriver.CheckBookingNoInLoad(bookingNo);
            webDriver.IsBookingQuantitySameInLoad(originalQuantity, bookingNo);
            webDriver.ClickCreateDeliverySchedule();
            Assert.IsTrue(webDriver.FindElement(By.ClassName("tooltip-inner")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.Id("saveDeliverySchedule")).Displayed);
            Assert.IsTrue(webDriver.FindElement(By.Id("cancel")).Displayed);
            webDriver.SelectLoadRejectionReason("Load Rejected", bookingNo);
            webDriver.SaveDeliverySchedule();
            webDriver.WaitForTextPresent(By.CssSelector("#confirm p"), "There are no items to be scheduled.");
            webDriver.ClickCssSelector(".ok");
        }

        public static void CheckLoad(this IWebDriver webDriver, string truckName, string currentUser, string deliveryLocation, string delCol)
        {
            Assert.IsTrue(webDriver.FindElement(By.ClassName("ref")).Text.Contains("Truck Load: " + truckName));
            webDriver.IsTextPresent(By.ClassName("createdBy"), currentUser);
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:first-child > th"), "Scheduling Branch:");
            webDriver.IsTextPresent(By.CssSelector(".branchNo"), deliveryLocation);
            webDriver.IsTextPresent(By.CssSelector(".row > .main > tbody > tr." + delCol + " > td > div.del-col > div"), delCol);
        }

        public static void CheckDeliverySchedule(this IWebDriver webDriver, string deliveryScheduleNo, string truckName, string driverName, string currentUser, string deliveryLocation, string delCol)
        {
            webDriver.IsTextPresent(By.ClassName("ref"), "Delivery Schedule #" + deliveryScheduleNo);
            webDriver.IsTextPresent(By.ClassName("createdBy"), currentUser);
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:first-child > th"), "Scheduling Branch:");
            webDriver.IsTextPresent(By.CssSelector(".branchNo"), deliveryLocation);
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:nth-child(2) > th"), "Number of Deliveries:");
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:nth-child(3) > th"), "Number of Collections:");
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:nth-child(4) > th"), "Driver Name:");
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:nth-child(4) > td"), driverName);
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:last-child > th"), "Vehicle:");
            webDriver.IsTextPresent(By.CssSelector(".header-info > tbody > tr:last-child > td"), truckName);
            webDriver.IsTextPresent(By.CssSelector(".main > tbody > tr." + delCol + " > td > div.del-col > div"), delCol);
        }



        private static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }
}

