using Blue.Cosacs.Sales.Api.Extensions;
using Blue.Cosacs.Sales.Models;
using Blue.Cosacs.Sales.Repositories;
using Blue.Glaucous.Client.Api;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Blue.Cosacs.Sales.Api.Controllers
{
    public class WarrantyContractsController : ApiController
    {
        private readonly IWarrantyContractRepository repository;
        private readonly ISalesLookupRepository salesLookUpRepository;

        public WarrantyContractsController(IWarrantyContractRepository repo, ISalesLookupRepository salesLookupRepo)
        {
            repository = repo;
            salesLookUpRepository = salesLookupRepo;
        }

        public List<string> Get(string agreementNo, [FromUri] string[] contractNos, bool multiple)//CR 2018-13
        {
            agreementNo = agreementNo.Replace("-", "");
            var retLst = new List<string>();
            
            var models = repository.GetWarrantyContractDetails(agreementNo, contractNos, multiple);
            var settings = new Settings();
            var user = this.GetUser();

            var fascia = salesLookUpRepository.GetBranchFascia(user.Id, user.LocationId);

            var currentViewName = "WarrantyContract";

            // Try load fascia specific view by appending fascia
            var fasciaViewName = currentViewName + "_" + fascia.Trim().Replace(" ", string.Empty);
            if (this.CheckViewExists(fasciaViewName))
            {
                currentViewName = fasciaViewName;
            }

            // Try load specific country view by prepending country
            var countryName = settings.CountryName.Replace(" ", "").ToUpper().Trim();
            countryName = (countryName == "COUNTRYNAME") ? "" : countryName + "_";

            var countryViewName = countryName + currentViewName;

            if (this.CheckViewExists(countryViewName))
            {
                currentViewName = countryViewName;
            }

            foreach (var model in models)
            {
                var documentCopies = model as DocumentCopy<WarrantyContractDetailsResult>[] ?? model.ToArray();
                if (model == null || !documentCopies.Any())
                {
                    continue;
                }

                var body = this.RenderViewToString(currentViewName, documentCopies);
                retLst.Add(body);
            }

            return retLst;
        }
    }
}
