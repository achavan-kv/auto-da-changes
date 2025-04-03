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
    public class ExchangeRateRepository
    {
        private readonly IClock clock;
        private readonly IEventStore audit;

        public ExchangeRateRepository(IClock clock, IEventStore audit)
        {
            this.clock = clock;
            this.audit = audit;
        }

        public CustomResponseMessage InsertNewExchangeRate(ExchangeRateDataDto currencyDetail, int currentUserId)
        {
            currencyDetail.DateFrom = currencyDetail.DateFrom.ToLocalTime();
            var response = new CustomResponseMessage();
            try
            {
                using (var scope = Context.Write())
                {
                    var existingData = (from c in scope.Context.ExchangeRate
                                        where c.CurrencyCode.Equals(currencyDetail.CurrencyCode)
                                        && (System.Data.Entity.DbFunctions.DiffDays(c.DateFrom, currencyDetail.DateFrom) == 0)
                                        select c).FirstOrDefault();

                    if (existingData == null)
                    {
                        var newExchangeRateEntry = new ExchangeRate
                        {
                            CurrencyCode = currencyDetail.CurrencyCode,
                            Rate = currencyDetail.Rate,
                            DateFrom = currencyDetail.DateFrom,
                            CreatedOn = clock.Now,
                            CreatedBy = currentUserId
                        };

                        scope.Context.ExchangeRate.Add(newExchangeRateEntry);
                        scope.Context.SaveChanges();
                        scope.Complete();

                        audit.LogAsync(
                          new
                          {
                              newExchangeRateEntry.Id,
                              newExchangeRateEntry.CurrencyCode,
                              newExchangeRateEntry.Rate,
                              newExchangeRateEntry.DateFrom
                          },
                         AuditEventTypes.InsertExchageRate,
                        AuditCategories.ExchangeRateData);
                    }
                    else
                    {
                        response.Valid = false;
                        response.CustomError = "The Currency Code with same DateFrom value exists";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Valid = false;
                response.Errors = new[] { ex.Message };
            }
            return response;
        }

        public CustomResponseMessage UpdateExchangeRate(ExchangeRateDataDto currencyDetail, int currentUserId)
        {
            var response = new CustomResponseMessage();
            try
            {
                currencyDetail.DateFromChanged = currencyDetail.DateFromChanged.Value.ToLocalTime();
                currencyDetail.DateFrom = currencyDetail.DateFrom.ToLocalTime();
                using (var scope = Context.Write())
                {
                    var sameCurrencyData = (from c in scope.Context.ExchangeRate
                                            where c.CurrencyCode.Equals(currencyDetail.CurrencyCode)
                                                  && (System.Data.Entity.DbFunctions.DiffDays(c.DateFrom, currencyDetail.DateFromChanged) == 0)
                                                  && (System.Data.Entity.DbFunctions.DiffDays(currencyDetail.DateFrom, currencyDetail.DateFromChanged.Value) != 0)
                                            select c).SingleOrDefault();

                    if (sameCurrencyData == null)
                    {
                        var existingData = (from c in scope.Context.ExchangeRate
                                            where c.CurrencyCode.Equals(currencyDetail.CurrencyCode)
                                                  && (System.Data.Entity.DbFunctions.DiffDays(c.DateFrom, currencyDetail.DateFrom) == 0)
                                            select c).Single();

                        existingData.Rate = currencyDetail.RateChanged;
                        existingData.DateFrom = currencyDetail.DateFromChanged.Value;
                        existingData.CreatedOn = clock.Now;
                        existingData.CreatedBy = currentUserId;

                        scope.Context.SaveChanges();
                        scope.Complete();

                        audit.LogAsync(
                            new
                            {
                                existingData.Id,
                                existingData.CurrencyCode,
                                existingData.Rate,
                                existingData.DateFrom
                            },
                            AuditEventTypes.UpdateExchangeRate,
                            AuditCategories.ExchangeRateData);
                    }
                    else
                    {
                        response.Valid = false;
                        response.CustomError = "The Currency Code with same DateFrom value exists";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Valid = false;
                response.Errors = new[] { ex.Message };
            }
            return response;
        }

        public CustomResponseMessage DeleteExchangeRate(string currencyCode, DateTime dateFrom)
        {
            try
            {
                dateFrom = dateFrom.ToLocalTime();
                using (var scope = Context.Write())
                {
                    var existingData = (from c in scope.Context.ExchangeRate
                                        where c.CurrencyCode.Equals(currencyCode)
                                              && (System.Data.Entity.DbFunctions.DiffDays(c.DateFrom, dateFrom) == 0)
                                        select c).Single();

                    scope.Context.ExchangeRate.Remove(existingData);
                    scope.Context.SaveChanges();
                    scope.Complete();

                    audit.LogAsync(
                        new
                        {
                            existingData.Id,
                            existingData.CurrencyCode,
                            existingData.Rate,
                            existingData.DateFrom
                        },
                        AuditEventTypes.DeleteExchangeRate,
                        AuditCategories.ExchangeRateData);
                }
            }
            catch (Exception ex)
            {
                return new CustomResponseMessage
                {
                    Valid = false,
                    Errors = new[] { ex.Message }
                };
            }
            return new CustomResponseMessage();
        }

        public List<string> GetRates()
        {
            using (var scope = Context.Read())
            {
                var currencyList = from c in scope.Context.CurrencyCodes
                                   select c.CurrencyCode + " - " + c.CurrencyName;

                return currencyList.ToList();
            }
        }

        public List<string> GetActiveRates()
        {
            using (var scope = Context.Read())
            {
                var currencyList = from c in scope.Context.ActiveExchangeRates
                                   select c.CurrencyCode + " - " + c.CurrencyName;

                return currencyList.ToList();
            }
        }

        public List<ExchangeRateDataDto> GetExchangeRateDetails(string currencyCode, DateTime? dateFrom)
        {
            using (var scope = Context.Read())
            {
                var currencyList = (from ec in scope.Context.ExchangeRate
                                    join c in scope.Context.CurrencyCodes
                                    on ec.CurrencyCode equals c.CurrencyCode
                                    where (currencyCode == null || c.CurrencyCode.Equals(currencyCode))
                                    && (dateFrom == null || ec.DateFrom <= dateFrom)
                                    select new ExchangeRateDataDto
                                    {
                                        CurrencyCode = ec.CurrencyCode,
                                        CurrencyName = c.CurrencyName,
                                        Rate = ec.Rate,
                                        DateFrom = ec.DateFrom,
                                        DateFromChanged = null,
                                        CreatedOn = ec.CreatedOn,
                                        CreatedBy = ec.CreatedBy
                                    })
                                    .OrderByDescending(x => x.DateFrom)
                                    .Take(50)
                                    .ToList();

                return currencyList;
            }
        }


    }
}
