using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue.Cosacs.Repositories;

namespace Cosacs.Web.Controllers
{
    public class CashLoanController : Controller
    {
        private readonly AccountRepository repository = new AccountRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintElectronicBankTransferSheet(string acctNo)
        {
            var cashLoanDisbursementDetails = repository.GetCashLoanDisbursementDetails(acctNo);

            return View(cashLoanDisbursementDetails);
        }

    }
}
