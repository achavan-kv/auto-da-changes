using Blue.Cosacs.Payments.Models;
using Blue.Glaucous.Client.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Payments.Api.Controllers
{
    [RoutePrefix("api/ExchangeRate")]
    public class ExchangeRateController : ApiController
    {
        private readonly ExchangeRateRepository exchangeRateRepository;

        public ExchangeRateController(ExchangeRateRepository exchangeRateRepository)
        {
            this.exchangeRateRepository = exchangeRateRepository;
        }

        [HttpPost]
        public CustomResponseMessage Post(ExchangeRateDataDto currencyDetail)
        {
            return exchangeRateRepository.InsertNewExchangeRate(currencyDetail, this.GetUser().Id);
        }

        [HttpPut]
        public CustomResponseMessage Put(ExchangeRateDataDto currencyDetail)
        {
            return exchangeRateRepository.UpdateExchangeRate(currencyDetail, this.GetUser().Id);
        }

        [HttpDelete]
        public CustomResponseMessage Delete(string currencyCode, DateTime dateFrom)
        {
            return exchangeRateRepository.DeleteExchangeRate(currencyCode, dateFrom);
        }

        [Route("GetRates")]
        [HttpGet]
        public List<string> GetRates()
        {
            return exchangeRateRepository.GetRates();
        }

        [Route("GetActiveRates")]
        [HttpGet]
        public List<string> GetActiveRates()
        {
            return exchangeRateRepository.GetActiveRates();
        }

        [HttpGet]
        public HttpResponseMessage GetByCode(string currencyCode, string dateFrom)
        {
            var searchDate = dateFrom.Equals("All") ? (DateTime?)null : Convert.ToDateTime(dateFrom);
            currencyCode = currencyCode.Equals("All") ? null : currencyCode;
            var exchangeRateDetails = exchangeRateRepository.GetExchangeRateDetails(currencyCode, searchDate);
            return Request.CreateResponse(new { Result = "Done", @ExchangeRateDetails = exchangeRateDetails });
        }

    }
}
