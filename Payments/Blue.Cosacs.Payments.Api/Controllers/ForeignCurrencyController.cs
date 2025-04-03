using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Blue.Cosacs.Payments.Models;
using System.Net.Http;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.Payments.Api.Controllers
{
    public class ForeignCurrencyController : ApiController
    {
        private readonly ForeignCurrencyRepository foreignCurrencyRepository;

        public ForeignCurrencyController(ForeignCurrencyRepository foreignCurrencyRepository)
        {
            this.foreignCurrencyRepository = foreignCurrencyRepository;
        }

        public IEnumerable<dynamic> Get()
        {
            return foreignCurrencyRepository.GetCurrencyCodes();
        }

        public decimal Get(string currencyCode)
        {
            return foreignCurrencyRepository.GetCurrencyRate(currencyCode);
        }
      
    }
}
