using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue.Cosacs.Repositories;

namespace Cosacs.Web.Controllers
{
    public class CashierController : Controller
    {
        private readonly CashierTotalRepository repository = new CashierTotalRepository();

        public ActionResult DailyReconciliation(string empeeName, int empeeNo, short branchNo, string machineName, DateTime dateFrom, DateTime dateTo)
        {
            ViewBag.empeeName = empeeName;
            ViewBag.empeeNo = empeeNo;
            ViewBag.branchNo = branchNo;
            ViewBag.machineName = machineName;
            ViewBag.dateFrom = dateFrom;
            ViewBag.dateTo = dateTo;

            return View(repository.LoadCashiersTotalled(empeeNo, dateFrom, dateTo));
        }

    }
}
