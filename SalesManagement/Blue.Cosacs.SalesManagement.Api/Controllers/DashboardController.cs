using Blue.Cosacs.SalesManagement.Hub.Subscribers;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;
using Blue.Networking;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Permission(SalesManagementPermissionEnum.ReloadDashboardJob)]
    public class DashboardController : ApiController
    {
        private readonly IDashboardRepository dashboardRepository;
        private readonly HttpClientJson httpClientJson;
        private readonly ISalesManagementRepository repository;
        private readonly IDatabase redisDataBase;
        private readonly Settings cosacsSettings;
        private readonly IClock clock;
        private readonly CallSummarySubscriber callSummarySubscriber;

        public DashboardController(
            IDashboardRepository dashboardRepository,
            HttpClientJson httpClientJson,
            ISalesManagementRepository repository,
            IDatabase redisDataBase,
            Blue.Config.ISettings cosacsSettings,
            IClock clock,
            CallSummarySubscriber callSummarySubscriber)
        {
            this.cosacsSettings = cosacsSettings as Settings;
            this.dashboardRepository = dashboardRepository;
            this.httpClientJson = httpClientJson;
            this.repository = repository;
            this.redisDataBase = redisDataBase;
            this.clock = clock;
            this.callSummarySubscriber = callSummarySubscriber;
        }

        [HttpGet]
        [LongRunningQueries]
        [CronJob]
        public HttpResponseMessage Get()
        {
            this.Process();
            return Request.CreateResponse();
        }

        private void Process()
        {
            callSummarySubscriber.FillCallSummary();
            UndeliveredAccounts();
            SlowServiceRequests();
            SalesSummary();
            //inform redis that the load process has ended
            dashboardRepository.ManageDashboardLock(false);
        }

        private Dictionary<string, int> GetCustomersSalesPerson(IList<string> customers)
        {
            return repository.GetCustomersSalesPerson(null, customers)
                    .Select(p => new
                    {
                        p.CustomerId,
                        p.SalesPersonId
                    })
                    .ToDictionary(k => k.CustomerId, v => v.SalesPersonId);
        }

        private void UndeliveredAccounts()
        {
            var allBranches = repository.AllBranchesNumbers();
            //get the the sales person for each customer
            var salesPerson = GetCustomersSalesPerson(null);

            foreach (var sp in salesPerson)
            {
                redisDataBase.KeyDelete(string.Format("{0}:{1}", Consts.UndeliveredAccountsDashboardKey, sp.Value));
            }

            foreach (var item in allBranches)
            {
                var data = ExternalHttpSources.Post<List<UndeliveredCashCreditBranch>>(
                    string.Format(
                        "/Courts.NET.WS/SalesManagement/UndeliveredCashCreditBranch?branchNo={0}",
                        item.ToString()),
                    httpClientJson);

                foreach (var current in data.Where(p => salesPerson.ContainsKey(p.CustomerId)))
                {
                    var key = string.Format("{0}:{1}", Consts.UndeliveredAccountsDashboardKey, salesPerson[current.CustomerId]);
                    redisDataBase.SetAdd((RedisKey)key, (RedisValue)current.ToJson(), CommandFlags.FireAndForget);
                }
            }
        }

        private void FillCallSummary(int? userId)
        {
            var usersPerBranch = ExternalHttpSources.Post<Hashtable>("/Courts.NET.WS/SalesManagement/LoadUsersBranches", httpClientJson);

            foreach (DictionaryEntry item in usersPerBranch)
            {
                var users = ((Newtonsoft.Json.Linq.JArray)item.Value).ToObject<int[]>();
                var userInBranch = dashboardRepository.CallSummary(users);
                var fieldsPerBranch = new Dictionary<string, int>();

                var branchKey = string.Format("SalesM:CallSum:{0}", item.Key);

                foreach (var singleUser in userInBranch)
                {
                    var count = singleUser.Value.Count;
                    var metricsPerUser = new List<HashEntry>(count);
                    var fields = new List<RedisValue>(count);

                    var key = string.Format("SalesM:CallSum:{0}:{1}", item.Key, singleUser.Key.ToString());

                    foreach (DictionaryEntry userMetric in singleUser.Value)
                    {
                        var value = int.Parse(userMetric.Value.ToString());

                        metricsPerUser.Add(new HashEntry((RedisValue)userMetric.Key.ToString(), (RedisValue)value));
                        fields.Add((RedisValue)userMetric.Key.ToString());

                        if (!fieldsPerBranch.ContainsKey(userMetric.Key.ToString()))
                        {
                            fieldsPerBranch.Add(userMetric.Key.ToString(), 0);
                        }

                        redisDataBase.HashDelete(key, fields.ToArray(), CommandFlags.FireAndForget);
                        redisDataBase.HashSet(key, metricsPerUser.ToArray(), CommandFlags.FireAndForget);
                        fieldsPerBranch[userMetric.Key.ToString()] = fieldsPerBranch[userMetric.Key.ToString()] + value;
                    }
                }

                redisDataBase.HashDelete(
                    branchKey,
                    fieldsPerBranch.Keys.Select(p => (RedisValue)p).ToList().ToArray(),
                    CommandFlags.FireAndForget);

                var branchMetrics = fieldsPerBranch.Select(p => new HashEntry((RedisValue)p.Key.ToString(), (RedisValue)p.Value))
                    .ToArray();

                redisDataBase.HashSet(
                    branchKey,
                    branchMetrics,
                    CommandFlags.FireAndForget);
            }
        }

        private void SlowServiceRequests()
        {
            var url = string.Format(
                "/cosacs/Service/requests/CustomersWithOpenSr?sinceWhen={0}",
                this.clock.Now.AddDays(cosacsSettings.SlowServiceRequests * -1).ToSolrDate().ToString(CultureInfo.InvariantCulture));

            var usersPerBranch = ExternalHttpSources.Get<Hashtable>("/Courts.NET.WS/SalesManagement/LoadUsersBranches", httpClientJson);
            var usersPerBranchDi = new Dictionary<int, short>();

            //convert the hastable into dictionary of <UserId, BranchNo>
            foreach (DictionaryEntry branch in usersPerBranch)
            {
                foreach (var user in ((Newtonsoft.Json.Linq.JArray)branch.Value).ToObject<int[]>())
                {
                    usersPerBranchDi.Add(user, short.Parse(branch.Key.ToString()));
                }
            }

            var slowSr = ExternalHttpSources.Get<List<CustomerWithOpenSr>>(url, httpClientJson);
            var salesPerson = GetCustomersSalesPerson(slowSr.Select(p => p.CustomerId).ToList());

            Parallel.ForEach<KeyValuePair<string, int>>(
                salesPerson,
                sp =>
                {
                    if (usersPerBranchDi.ContainsKey(sp.Value))
                    {
                        redisDataBase.KeyDelete(string.Format("{0}:{1}:{2}", Consts.SlowServiceRequestKey, usersPerBranchDi[sp.Value], sp.Value));
                    }
                });

            var slowCrs = slowSr
                .Where(p => salesPerson.ContainsKey(p.CustomerId))
                .Select(p => new SlowCsrGroup
                {
                    Csr = salesPerson[p.CustomerId],
                    Data = p
                })
                .GroupBy(p => p.Csr)
                .ToList();

            Parallel.ForEach<IGrouping<int, SlowCsrGroup>>(
                slowCrs,
                item =>
                {
                    if (usersPerBranchDi.ContainsKey(item.Key))
                    {
                        var key = string.Format("{0}:{1}:{2}", Consts.SlowServiceRequestKey, usersPerBranchDi[item.Key], item.Key);

                        redisDataBase.SetAdd((RedisKey)key, (RedisValue)item.Select(p => p.Data).ToList().ToJson(), CommandFlags.FireAndForget);
                    }
                });
        }

        private class SlowCsrGroup
        {
            public CustomerWithOpenSr Data
            {
                get;
                set;
            }
            public int Csr
            {
                get;
                set;
            }
        }

        private void SalesSummary()
        {
            var salesPeople = dashboardRepository.SalesPersonTargets(clock.Now.Year);
            var data = SalesSummaryPerYear.GetData(clock.Now.Date);
            
            var batch = redisDataBase.CreateBatch();
            const string TargetSales = "Target Sales";

            //will be used to create empty lines for the values that weren't returned 
            var allAreas = new List<string>
            {
                "Actual Booking",
                "Actual Delivered",
                "Average Transaction Value",
                "Average Term Length", 
                "Undelivered"
            };

            //store the keys that will be deleted
            var redisKeys = new List<RedisKey>();

            foreach (var branchData in data.GroupBy(p => p.BranchNo))
            {
                var metricsPerBranch = (from i in branchData
                                        group i by i.Area into g
                                        select new SalesSummaryPerYear
                                        {
                                            Week = g.Sum(x => x.Week),
                                            Month = g.Sum(x => x.Month),
                                            Year = g.Sum(x => x.Year),
                                            Today = g.Sum(x => x.Today),
                                            Area = g.Key
                                        }).ToList();

                foreach (var item in data.GroupBy(p => p.SalesPerson))
                {
                    var key = string.Format("{0}:{1}", Consts.SalesSummaryDashboardKey, item.Key);

                    redisKeys.Add((RedisKey)key);

                    //the target sales are in this module
                    var targetSales = new SalesSummaryPerYear
                    {
                        Area = TargetSales,
                    };

                    if (salesPeople.ContainsKey(item.Key))
                    {
                        targetSales.Today = salesPeople[item.Key].Item4;
                        targetSales.Week = salesPeople[item.Key].Item3;
                        targetSales.Month = salesPeople[item.Key].Item2;
                        targetSales.Year = salesPeople[item.Key].Item1;
                    }

                    var valuesPerSalesPerson = JoinWithDefault(item.Select(p => p), allAreas, targetSales);
                    batch.SetAddAsync((RedisKey)key, (RedisValue)valuesPerSalesPerson.ToJson());
                }

                var branchKey = string.Format("{0}:{1}", Consts.SalesSummaryDashboardKey, branchData.Key);
                redisKeys.Add((RedisKey)branchKey);

                var totalTargetSales = new SalesSummaryPerYear()
                {
                    Area = string.Empty
                };

                var dataPerBranch = JoinWithDefault(metricsPerBranch.Select(p => p), allAreas, totalTargetSales);
                dataPerBranch.RemoveAt(5);
                batch.SetAddAsync((RedisKey)branchKey, (RedisValue)dataPerBranch.ToJson());
            }

            redisDataBase.KeyDelete(redisKeys.ToArray());
            batch.Execute();
        }

        private IList<SalesSummaryPerYear> JoinWithDefault(IEnumerable<SalesSummaryPerYear> valuesToJoin, IList<string> all, SalesSummaryPerYear targetSales)
        {
            var result = new HashSet<string>(all);
            result.ExceptWith(valuesToJoin.Select(p => p.Area).ToList());

            //create empty lines for non existing data
            var returnValue = result
                .Select(p => new SalesSummaryPerYear
                {
                    Area = p
                })
                .ToList()
                .Union(valuesToJoin.Select(p => new SalesSummaryPerYear
                {
                    Area = p.Area,
                    Today = p.Today,
                    Week = p.Week,
                    Month = p.Month,
                    Year = p.Year
                }))
                .ToList();

            returnValue.Add(targetSales);

            return returnValue;
        }
    }
}