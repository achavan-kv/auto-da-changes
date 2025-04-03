using Blue.Cosacs.Payments.Models;
using Blue.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Payments
{
    public class ForeignCurrencyRepository
    {
        private readonly IClock clock;
        private readonly IEventStore audit;

        public ForeignCurrencyRepository(IClock clock, IEventStore audit)
        {
            this.clock = clock;
            this.audit = audit;
        }

        public IEnumerable<dynamic> GetCurrencyCodes()
        {
            using (var scope = Context.Read())
            {
                var currencyList = (from ec in scope.Context.ExchangeRate
                                    join c in scope.Context.CurrencyCodes
                                    on ec.CurrencyCode equals c.CurrencyCode
                                    where ec.DateFrom <= clock.Now.Date
                                    select new
                                    {
                                        Code = ec.CurrencyCode,
                                        Name = c.CurrencyName
                                    })
                                    .Distinct()
                                    .ToList();

                return currencyList;
            }
        }

        public decimal GetCurrencyRate(string currencyCode)
        {
            using (var scope = Context.Read())
            {
                var currencyRate = from ec in scope.Context.ExchangeRate
                                   join c in scope.Context.CurrencyCodes
                                       on ec.CurrencyCode equals c.CurrencyCode
                                   where ec.DateFrom <= clock.Now.Date &&
                                         ec.CurrencyCode.Trim().ToLower() == currencyCode.Trim().ToLower()
                                   select ec;
                var date = currencyRate.Max(x => x.DateFrom);

                return (from ec in scope.Context.ExchangeRate
                        join c in scope.Context.CurrencyCodes
                            on ec.CurrencyCode equals c.CurrencyCode
                        where ec.DateFrom == date &&
                              ec.CurrencyCode.Trim().ToLower() == currencyCode.Trim().ToLower()
                        select ec.Rate).SingleOrDefault();
            }
        }

    }
}
