using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Repositories
{
    public sealed class DashboardRepository : IDashboardRepository
    {
        private readonly IClock clock;
        private readonly IDatabase redisDataBase;

        public DashboardRepository(IClock clock, IDatabase redisDataBase)
        {
            this.clock = clock;
            this.redisDataBase = redisDataBase;
        }

        internal List<TotalPerSalesPerson> JoinWithDefault(List<TotalPerSalesPerson> valuesToJoin, IList<int> all, string property)
        {
            var result = new HashSet<int>(all);
            result.ExceptWith(valuesToJoin.Select(p => p.SalesPerson).ToList());

            return result
                .Select(p => new TotalPerSalesPerson
                {
                    Count = 0,
                    PrpertyName = property,
                    SalesPerson = p
                })
                .ToList()
                .Union(valuesToJoin)
                .ToList();
        }

        public IDictionary<int, Hashtable> CallSummary(IList<int> salesPersonId)
        {
            using (var scope = Context.Read())
            {
                var todayDate = clock.Now.Date;
                var todayDateBeginning = clock.Now.Date.AddMilliseconds(-1);
                var todayDateEnd = clock.Now.Date.AddDays(1).AddMilliseconds(-1);
                var lastSevenDays = todayDate.AddDays(-7).Date;

                //LateCalls
                var lateCalls = JoinWithDefault(
                    scope.Context.Call
                        .Where(p => p.CallClosedReasonId == null && p.SalesPersonId != null && salesPersonId.Contains(p.SalesPersonId.Value) && p.ToCallAt < todayDateBeginning)
                        .GroupBy(p => p.SalesPersonId)
                        .Select(p => new TotalPerSalesPerson
                        {
                            SalesPerson = p.Key.Value,
                            Count = p.Count(),
                            PrpertyName = "Late"
                        })
                        .ToList(),
                    salesPersonId,
                    "Late");

                //scheduled in total
                var scheduledTotal = JoinWithDefault(
                    scope.Context.Call
                        .Where(p => p.CallClosedReasonId == null && p.SalesPersonId != null && salesPersonId.Contains(p.SalesPersonId.Value))
                        .GroupBy(p => p.SalesPersonId)
                        .Select(p => new TotalPerSalesPerson
                        {
                            SalesPerson = p.Key.Value,
                            Count = p.Count(),
                            PrpertyName = "ScheduledTotal"
                        })
                        .ToList(),
                    salesPersonId,
                    "ScheduledTotal");

                //scheduled for today
                var scheduledToday = JoinWithDefault(
                    scope.Context.Call
                        .Where(p => p.CallClosedReasonId == null
                            && p.SalesPersonId != null
                            && salesPersonId.Contains(p.SalesPersonId.Value)
                            && p.ToCallAt >= todayDateBeginning && p.ToCallAt < todayDateEnd)
                        .GroupBy(p => p.SalesPersonId)
                        .Select(p => new TotalPerSalesPerson
                        {
                            SalesPerson = p.Key.Value,
                            Count = p.Count(),
                            PrpertyName = "ScheduledToday"
                        })
                        .ToList(),
                    salesPersonId,
                    "ScheduledToday");

                //flushed in the last week
                var flushedLastWeek = JoinWithDefault(
                    scope.Context.Call
                        .Where(p => p.CallClosedReasonId == (int)CallClosedReason.CallClosedReasonEnum.FlushedCall
                            && p.SalesPersonId != null
                            && salesPersonId.Contains(p.SalesPersonId.Value)
                            && p.ToCallAt >= lastSevenDays
                            && p.ToCallAt < todayDateBeginning)
                        .GroupBy(p => p.SalesPersonId)
                        .Select(p => new TotalPerSalesPerson
                        {
                            SalesPerson = p.Key.Value,
                            Count = p.Count(),
                            PrpertyName = "FlushedLastWeek"
                        })
                        .ToList(),
                    salesPersonId,
                    "FlushedLastWeek");

                // logged in the last week
                var loggedLastWeek = JoinWithDefault(
                    scope.Context.Call
                        .Where(p => (p.CallClosedReasonId == (int)CallClosedReason.CallClosedReasonEnum.CalledTheCustomer || p.CallClosedReasonId == (int)CallClosedReason.CallClosedReasonEnum.ClosedInBulkByCSR)
                            && p.CalledBy != null
                            && salesPersonId.Contains(p.CalledBy.Value)
                            && p.ToCallAt >= lastSevenDays)
                        .GroupBy(p => p.SalesPersonId)
                        .Select(p => new TotalPerSalesPerson
                        {
                            SalesPerson = p.Key.Value,
                            Count = p.Count(),
                            PrpertyName = "LoggedLastWeek"
                        })
                        .ToList(),
                    salesPersonId,
                    "LoggedLastWeek");

                //This is not my code
                return MergeDictionary(MergeDictionary(MergeDictionary(MergeDictionary(MergeDictionary(new Dictionary<int, Hashtable>(), lateCalls), scheduledTotal), scheduledToday), flushedLastWeek), loggedLastWeek);
            }
        }

        public IDictionary<int, Tuple<decimal, decimal, decimal, decimal>> SalesPersonTargets(int year)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.SalesPersonTargets
                    .Where(p=> p.TargetYear == year)
                    .ToList()
                    .Select(p => new
                    {
                        Tergets = new Tuple<decimal, decimal, decimal, decimal>(
                            p.TargetYear,
                            Math.Round(p.TargetYear / 12, 2), 
                            Math.Round(p.TargetYear / 52, 2), 
                            Math.Round(p.TargetYear / 365, 2)),
                        p.CreatedBy
                    })
                    .ToDictionary(p => p.CreatedBy, t => t.Tergets);
            }
        }

        internal Dictionary<int, Hashtable> MergeDictionary(Dictionary<int, Hashtable> dic, List<TotalPerSalesPerson> data)
        {
            foreach (TotalPerSalesPerson item in data)
            {
                if (dic.ContainsKey(item.SalesPerson))
                {
                    dic[item.SalesPerson].Add(item.PrpertyName, item.Count);
                }
                else
                {
                    var hash = new Hashtable();

                    hash.Add(item.PrpertyName, item.Count);
                    dic.Add(item.SalesPerson, hash);
                }
            }

            return dic;
        }

        public bool IsLoadingDashboard
        {
            get
            {
                var isLoading = redisDataBase.StringGet((RedisKey)"SalesM:Dashboard:Loading", CommandFlags.None);

                return isLoading.IsNull || (bool)isLoading;
            }
        }

        public void ManageDashboardLock(bool lockIt)
        {
            var key = string.Format("{0}:Dashboard:Loading", Consts.RedisMainKey);

            redisDataBase.StringSet((RedisKey)key, lockIt);
        }

        public void UpdateRedis()
        {
        }
    }
}
