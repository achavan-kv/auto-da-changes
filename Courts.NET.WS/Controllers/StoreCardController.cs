using System.Web.Mvc;
using Blue.Cosacs.Repositories;
using System;
using System.Web;
using System.Collections.Generic;
using Blue.Cosacs.Shared;
using StructureMap;
using Blue.Events;
using System.Globalization;

namespace Blue.Cosacs.Web.Controllers
{
    public class StoreCardController : Controller
    {

        private readonly StoreCardRepository repository = new StoreCardRepository();

        public ActionResult Statement(int id)
        {
            return View(repository.GetStatements(id));
        }

        //#12200 - CR11571
        public ActionResult BatchStatement(string batchNos, string batchDateRun, string reprint, string runNo)
        {
            List<SCStatement> statements = new List<SCStatement>();

            string[] arrBatchNos = batchNos.Split('-');
            var startNo = Convert.ToInt32(arrBatchNos[0].Trim());
            var endNo = Convert.ToInt32(arrBatchNos[1].Trim());

            repository.batchStatementsDatePrinted = DateTime.ParseExact(batchDateRun, "s", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); //#12390
            repository.reprintBatch = Convert.ToBoolean(reprint);
            repository.batchRunNo = Convert.ToInt32(runNo);

            var batchOrigDatePrinted = repository.GetStatementDatePrinted(startNo, Convert.ToInt32(runNo));

            //#12424
            EventStore.Instance.LogAsync(new { RunNo = runNo, StatementNo = batchNos, DatePrinted = Convert.ToBoolean(reprint) == false ? repository.batchStatementsDatePrinted.ToLocalTime().ToString() : Convert.ToDateTime(batchOrigDatePrinted).ToLocalTime().ToString(), DateReprinted = Convert.ToBoolean(reprint) == true ? repository.batchStatementsDatePrinted.ToLocalTime().ToString() : "" }, Convert.ToBoolean(reprint) == false ? "StoreCardBatchPrint" : "StoreCardBatchReprint", EventCategory.StoreCard);

            for (int i = startNo; i <= endNo; i++)
            {
   
                var statement = repository.GetStatementsBatchPrint(i);

                if (statement != null)
                {
                    statements.Add(statement);
                }
            }
            return View(statements);
        }


        public ActionResult Agreement(string acctno)
        {
            var model = repository.GetAgreement(acctno);
            return this.Stylesheet("StoreCardAgreement", model);
        }

        public ActionResult Calculator(string acctno, decimal balance, decimal interestRate, int term, decimal monthyRepayments, decimal total)
        {
            ViewBag.balance = balance;
            ViewBag.interestRate = interestRate;
            ViewBag.term = term;
            ViewBag.monthyRepayments = monthyRepayments;
            ViewBag.total = total;
            ViewBag.accto = acctno;
            return View(repository.GetCalc(acctno));
        }

        public ActionResult NewCardAgreement(string acctno, string newcustid)
        {
            ViewBag.acctno = acctno;
            return View(repository.GetNewCardAgreement(acctno, newcustid));
        }
    }
}
