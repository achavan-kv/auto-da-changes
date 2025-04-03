using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Networking;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Hub.Subscribers
{
    public class CallSummarySubscriber : ICallSummarySubscriber
    {
        private readonly IDashboardRepository dashboardRepository;
        private readonly HttpClientJson httpClientJson;
        private readonly IDatabase redisDataBase;
        private readonly Settings cosacsSettings;
        private readonly IClock clock;

        public CallSummarySubscriber(
            HttpClientJson httpClientJson,
            IDatabase redisDataBase,
            IDashboardRepository dashboardRepository,
            Blue.Config.ISettings cosacsSettings,
            IClock clock)
        {
            this.cosacsSettings = cosacsSettings as Settings;
            this.httpClientJson = httpClientJson;
            this.dashboardRepository = dashboardRepository;
            this.redisDataBase = redisDataBase;
            this.clock = clock;
        }

        public void FillCallSummary()
        {
            var data = ExternalHttpSources.Post<Hashtable>("/Courts.NET.WS/SalesManagement/LoadUsersBranches", httpClientJson);

            foreach (DictionaryEntry item in data)
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
    }
}
