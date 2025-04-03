using System;
using System.Linq;
using System.Web.Mvc;
using Blue.Cosacs.Repositories;
using System.Collections.Generic;
using Blue.Glaucous.Client.Mvc;

namespace Cosacs.Web.Controllers
{
    public class SalesManagementController : Controller
    {
        [LongRunningQueries]
        public JsonResult GetCustomersInstalments(DateTime dateToSchedule)
        {
            var repository = new SalesManagementRepository();

            return Json(repository.GetCustomersInstalments(dateToSchedule), JsonRequestBehavior.AllowGet);
        }

        [LongRunningQueries]
        public JsonResult GetInactiveCustomers(DateTime howOldCash, DateTime howOldCredit, int numberOfDaysBefore, int numberOfRecordsToReturn)
        {
            var repository = new SalesManagementRepository();

            return Json(repository.GetInactiveCustomers(howOldCash, howOldCredit, numberOfDaysBefore, numberOfRecordsToReturn), JsonRequestBehavior.AllowGet);
        }

        [LongRunningQueries]
        public JsonResult GetAllocatedCustomersToCSR(DateTime allocationDate)
        {
            var repository = new SalesManagementRepository();

            return Json(repository.GetAllocatedCustomersToCSR(allocationDate), JsonRequestBehavior.AllowGet);
        }

        [LongRunningQueries]
        public JsonResult GetCustomer(string accountNuber = null, string[] customerId = null)
        {
            var repository = new SalesManagementRepository();

            return Json(repository.GetCustomer(accountNuber, customerId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerInArrearsAndUndeliveredAccount(string customerId)
        {
            var repository = new SalesManagementRepository();

            return Json(repository.IsCustomerInArrearrsAndHasUndeliveredAccount(customerId), JsonRequestBehavior.AllowGet);
        }

        [LongRunningQueries]
        public JsonResult UndeliveredCashCreditBranch(short branchNo)
        {
            var repository = new SalesManagementRepository();
            return new LargeJsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = repository.GetUndeliveredCashCreditBranch(branchNo)
            };
        }

        public JsonResult LoadUsersBranches(int? userId)
        {
            var repository = new SalesManagementRepository();

            return Json(repository.LoadUsersBranches(userId), JsonRequestBehavior.AllowGet);
        }

        [LongRunningQueries]
        public JsonResult SummaryTableData(DateTime todayDate)
        {
            var repository = new SalesManagementRepository();

            return Json(repository.SummaryTableData(todayDate), JsonRequestBehavior.AllowGet);
        }

        [LongRunningQueries]
        [CronJob]
        public void GetSalesKPI()
        {
            new SalesManagementRepository().GetSalesKPI();
        }
    }
}
