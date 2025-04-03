using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;

namespace Cosacs.Web.Controllers
{
    public class ContractsController : Controller
    {

        private readonly AccountRepository repository = new AccountRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintContracts(string lineItemIds)
        {
            string[] arrLineItemIds = lineItemIds.Split(',');
            List<int> listLineItemIds = new List<int>();

            foreach (string l in arrLineItemIds)
            {
                listLineItemIds.Add(Convert.ToInt32(l));
            }

            var contractsToPrint = repository.GetContractsToPrint(listLineItemIds);

            return View(contractsToPrint);
        }

    }
}
