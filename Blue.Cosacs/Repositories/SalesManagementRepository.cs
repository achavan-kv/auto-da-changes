using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.SalesManagement;
using Blue.Cosacs.Shared;
using System.Data;
using System.Collections;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Blue.Cosacs.Repositories
{
    public class SalesManagementRepository
    {
        private readonly IClock clock;
        private readonly IDatabase redisDataBase;

        public SalesManagementRepository()
        {
            this.clock = StructureMap.ObjectFactory.GetInstance<IClock>();
            this.redisDataBase = RedisConnection.Database();
        }

        public CustomersInstalmentResult GetCustomersInstalments(DateTime dateToSchedule)
        {
            var returnValue = new CustomersInstalmentResult();
            var source = new Blue.Cosacs.SalesManagement.CustomerInstalments();

            returnValue.CustomersExactDate = source.GetData(dateToSchedule.Date, dateToSchedule.Date.AddDays(1).Date);
            returnValue.CustomersWithinTheRange = source.GetData(clock.Now.Date, dateToSchedule.Date);

            return returnValue;
        }

        public IList<CustomersLastPurchase> GetInactiveCustomers(DateTime howOldCash, DateTime howOldCredit, int numberOfDaysBefore, int numberOfRecordsToReturn)
        {
            var beginningOfRangeCash = howOldCash.AddDays(0 - numberOfDaysBefore).Date;
            var beginningOfRangeCredit = howOldCash.AddDays(0 - numberOfDaysBefore).Date;

            return new CustomersLastPurchase().GetData(howOldCash, beginningOfRangeCash, howOldCredit, beginningOfRangeCredit, numberOfRecordsToReturn);
        }

        public IList<AllocatedCustomers> GetAllocatedCustomersToCSR(DateTime dateToSchedule)
        {
            using (var ctx = Context.Create())
            {
                var minDate = dateToSchedule.Date;
                var maxDate = dateToSchedule.Date.AddDays(1);

                return ctx.AllocateCustomersToCSR
                    .Where(p => p.DateChanged >= minDate && p.DateChanged <= maxDate)
                    .Select(p => new AllocatedCustomers
                        {
                            CustomerId = p.CustomerId,
                            SalesPerson = p.SalesPersonId,
                            LandLinePhone = p.LandLinePhone,
                            MobileNumber = p.MobileNumber,
                            CustomerBranch = p.CustomerBranch,
                            Email = p.Email
                        })
                    .ToList();
            }
        }

        public IList<CustomerSalesManagementView> GetCustomer(string accountNuber = null, string[] customerId = null)
        {
            using (var context = Context.Create())
            {
                var query = context.CustomerSalesManagementView
                    .Select(p => p);

                if (!string.IsNullOrEmpty(accountNuber))
                {
                    query = query
                        .Where(p => p.CustomerAccount == accountNuber);
                }

                if (customerId != null && customerId.Any())
                {
                    //for the record...
                    //   this is not correct, but this will be rebuild for version 10 so it's fine for now

                    query = query
                        .Where(p => customerId.Contains(p.CustomerId))
                        .Select(p => new CustomerSalesManagementView
                        {
                            CustomerAccount = string.Empty,
                            CustomerId = p.CustomerId,
                            Email = p.Email,
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            LandLinePhone = p.LandLinePhone,
                            MobileNumber = p.MobileNumber,
                            ReceiveSms = p.ReceiveSms,
                            SalesPerson = 0, //i think there is a bug in the view getting the sales person. it should be the last person that interact with the customers, not the crs the sold this account
                            TotalAmount = 0
                        })
                        .Distinct();
                }

                return query.ToList();
            }
        }

        public Hashtable LoadUsersBranches(int? userId)
        {
            using (var contex = Context.Create())
            {
                var result = new Hashtable();
                var query = contex.UsersPerBranch
                    .Select(p => new
                    {
                        p.Id,
                        p.BranchNo
                    });

                if (userId.HasValue)
                {
                    query = query
                        .Where(p => p.Id == userId.Value);
                }

                var values = query
                    .GroupBy(p => p.BranchNo)
                    .ToList()
                    .Select(p => new
                    {
                        Key = p.Key,
                        Ids = p.Select(b => b.Id).ToArray()
                    })
                    .ToList();

                foreach (var item in values)
                {
                    result.Add(item.Key.ToString(), item.Ids);
                }

                return result;
            }
        }

        public CustomerInArrearsAndUndeliveredAccount IsCustomerInArrearrsAndHasUndeliveredAccount(string customerId)
        {
            return new CustomerInArrearsAndUndeliveredAccount().GetData(customerId);
        }

        public List<UndeliveredCashCreditBranch> GetUndeliveredCashCreditBranch(short branchNo)
        {
            return UndeliveredCashCreditBranch.GetData(branchNo);
        }

        public Hashtable SummaryTableData(DateTime todayDate)
        {
            //var values = new ConcurrentBag<List<DictionaryEntry>>();
            var result = new Hashtable();

            var sources = new List<Blue.Cosacs.Procedure>(4);
            sources.Add(new SalesManagemntSummaryTableAverageTransactionValue { Today = todayDate });
            sources.Add(new SalesManagemntSummaryTableAverageTermLength { Today = todayDate });
            sources.Add(new SalesManagemntSummaryTableActualDelivered { Today = todayDate });
            sources.Add(new SalesManagemntSummaryTableUndelivered { Today = todayDate });

            Parallel.ForEach(sources, current =>
                {
                    var data = SalesSummary.GetData(current);
                    if (data.Any())
                    {
                        var key = data.First().Area;
                        var values = data
                            .Select(p => new
                            {
                                p.Amount,
                                p.BranchNo,
                                p.SalesPerson,
                            })
                            .ToList();

                        lock (result.SyncRoot)
                        {
                            result.Add(key, values);
                        }
                    }
                });

            return result;
        }

        public Hashtable CustomersRecieveSms()
        {
            using (var contex = Context.Create())
            {
                var result = new Hashtable();

                foreach (var item in contex.Customer.Select(p => new { p.custid, p.ResieveSms }))
                {
                    result.Add(item.custid, item.ResieveSms);
                }

                return result;
            }
        }

        public void GetSalesKPI()
        {
            var values = SalesKPI.GetSalesKPIData(clock.Now.Date);

            const string FirstWeekKey = "SalesM:SalesKpiFirstWeek";

            foreach (DictionaryEntry item in values)
            {
                var list = item.Value as List<SalesKpiDto<decimal>>;

                if (list != null)
                {
                    InsertIntoRedis<decimal>(list, item.Key.ToString());
                }
                else
                {
                    InsertIntoRedis<int>(item.Value as List<SalesKpiDto<int>>, item.Key.ToString());
                }
            }

            redisDataBase.KeyDelete(FirstWeekKey, CommandFlags.FireAndForget);

            if (values[FirstWeekKey] != null)
            {
                redisDataBase.SetAdd(FirstWeekKey, (RedisValue)((DateTime)values[FirstWeekKey]).ToString(), CommandFlags.FireAndForget);
            }
        }

        private void InsertIntoRedis<T>(List<SalesKpiDto<T>> list, string kpiKey)
        {
            if (list != null)
            {
                HashEntry[] metricsPerUser = null;

                foreach (var item in list.GroupBy(p => p.BranchNo))
                {
                    // This is for Warranty and Installations because they return percentage. This will be refactored
                    if (kpiKey != "SalesM:CsrWarrantyHitRate" &&
                        kpiKey != "SalesM:BranchWarrantyHitRate" &&
                        kpiKey != "SalesM:CsrInstallationHitRate" &&
                        kpiKey != "SalesM:BranchInstallationHitRate")
                    {
                        var metricsPerBranch = item.GroupBy(p => p.WeekNo)
                          .Select(p => new
                          {
                              Week = p.Key,
                              Total = p.Sum(v => decimal.Parse(v.Total.ToString()))
                          })
                          .ToList();

                        var branchKey = string.Format("{0}:{1}", kpiKey, item.First().BranchNo);
                        redisDataBase.HashDelete(branchKey, metricsPerBranch.Select(p => (RedisValue)p.Week).ToArray());

                        var branchMetrics = metricsPerBranch.Select(p => new HashEntry((RedisValue)p.Week.ToString(), (RedisValue)p.Total));

                        redisDataBase.HashSet(branchKey, branchMetrics.ToArray());
                    }

                    foreach (var data in list.GroupBy(p => p.CSR))
                    {
                        var key = string.Format("{0}:{1}:{2}", kpiKey, data.First().BranchNo, data.Key);
                        metricsPerUser = data.Select(p => new HashEntry((RedisValue)p.WeekNo, (RedisValue)p.Total.ToString())).ToArray();

                        redisDataBase.HashDelete(key, metricsPerUser.Select(p => p.Value).ToArray(), CommandFlags.FireAndForget);
                        redisDataBase.HashSet(key, metricsPerUser, CommandFlags.FireAndForget);
                    }
                }
            }
        }
    }
}
