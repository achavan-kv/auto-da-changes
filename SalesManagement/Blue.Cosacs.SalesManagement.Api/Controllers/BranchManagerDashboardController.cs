using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Hub.Client;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/BranchManagerDashboard")]
    public class BranchManagerDashboardController : ApiController
    {
        private readonly IDashboardRepository dashboardRepository;
        private readonly IPublisher publisher;
        private readonly IDatabase redisDataBase;
        private readonly ISalesManagementRepository repository;

        public BranchManagerDashboardController(IDashboardRepository dashboardRepository, IPublisher publisher, IDatabase redisDataBase, ISalesManagementRepository repository)
        {
            this.dashboardRepository = dashboardRepository;
            this.publisher = publisher;
            this.redisDataBase = redisDataBase;
            this.repository = repository;
        }

        [Permission(SalesManagementPermissionEnum.BranchManagerDashboard)]
        public Hashtable Get()
        {
            var branchNo = this.GetUser().Branch;
            var callSUmmaryBranchKey = string.Format("{0}:{1}", SalesManagement.Consts.CallSummaryDashboardKey, branchNo);

            if (!redisDataBase.KeyExists(callSUmmaryBranchKey))
            {
                var hashTable = new Hashtable();

                dashboardRepository.ManageDashboardLock(true);

                publisher.Publish<string>("SalesManagement.Dashboard", string.Empty);

                hashTable.Add("IsLoading", true);

                return hashTable;
            }

            return GetDashboardData(branchNo, callSUmmaryBranchKey);
        }

        [Permission(SalesManagementPermissionEnum.BranchManagerDashboard)]
        public Hashtable Get(int userId)
        {
            var branchNo = this.GetUser().Branch;

            var callSUmmaryKey = string.Format("{0}:{1}:{2}", SalesManagement.Consts.CallSummaryDashboardKey, branchNo, userId);

            var hashTable = new Hashtable();

            if (!redisDataBase.KeyExists(callSUmmaryKey))
            {
                dashboardRepository.ManageDashboardLock(true);

                publisher.Publish<string>("SalesManagement.Dashboard", string.Empty);

                hashTable.Add("IsLoading", true);
            }
            else
            {
                var callSummaryResults = redisDataBase.HashGetAll((RedisKey)callSUmmaryKey);
                var callSummaryItems = new Hashtable();
                var undeliveredAccounts = new Hashtable();

                hashTable.Add("IsLoading", false);
                hashTable.Add("IsRefreshing", dashboardRepository.IsLoadingDashboard);

                //Call Summary
                foreach (var item in callSummaryResults)
                {
                    callSummaryItems.Add(item.Name, item.Value);
                }

                //Undelivered Accounts
                var key = string.Format("{0}:{1}", SalesManagement.Consts.UndeliveredAccountsDashboardKey, userId);
                var undeliveredAccountsResults = redisDataBase.SetMembers((RedisKey)key);

                //slowSr's
                key = string.Format("{0}:{1}:{2}", SalesManagement.Consts.SlowServiceRequestKey, branchNo, userId);
                var slowSrsResults = redisDataBase.SetMembers((RedisKey)key);

                //Sales Summary
                key = string.Format("{0}:{1}", Consts.SalesSummaryDashboardKey, userId);
                var salesSummaryResults = redisDataBase.SetMembers((RedisKey)key);

                //Sales KPI First Week
                var firstWeekKey = "SalesM:SalesKpiFirstWeek";
                var salesKPIFirstWeek = redisDataBase.SetMembers((RedisKey)firstWeekKey);

                //New Customer Acquisition KPI
                key = string.Format("{0}:{1}:{2}", Consts.NewCustomerAcquisitionKPIDashboardKey, branchNo, userId);
                var newCustomerAcquisitionResults = new List<KeyValuePair<int, int>>();

                foreach (var item in redisDataBase.HashGetAll((RedisKey)key))
                {
                    newCustomerAcquisitionResults.Add(new KeyValuePair<int, int>(
                       int.Parse(item.Name),
                       int.Parse(item.Value)));
                }

                //Cancellations KPI
                key = string.Format("{0}:{1}:{2}", Consts.CancellationKPIDashboardKey, branchNo, userId);
                var cancellationsKPIResults = new List<KeyValuePair<int, int>>();

                foreach (var item in redisDataBase.HashGetAll((RedisKey)key))
                {
                    cancellationsKPIResults.Add(new KeyValuePair<int, int>(
                       int.Parse(item.Name),
                       int.Parse(item.Value)));
                }

                var newCustomerAcquisitionAndCancellations = newCustomerAcquisitionResults
                    .Join(
                        cancellationsKPIResults,
                        left => left.Key,
                        right => right.Key,
                        (nC, can) => new
                        {
                            Week = nC.Key,
                            Acquisition = nC.Value,
                            Cancelations = can.Value
                        })
                    .ToList()
                    .OrderBy(p => p.Week);

                //Rewrites
                key = string.Format("{0}:{1}:{2}", Consts.RewritesKPIDashboardKey, branchNo, userId);
                var rewritesKPIResults = new List<object>();

                foreach (var item in redisDataBase.HashGetAll((RedisKey)key))
                {
                    rewritesKPIResults.Add(new
                    {
                        Week = int.Parse(item.Name),
                        Total = decimal.Parse(item.Value)
                    });
                }

                //Credit Mix
                key = string.Format("{0}:{1}:{2}", Consts.CreditMixKPIDashboardKey, branchNo, userId);
                var creaditMixResults = new List<object>();

                foreach (var item in redisDataBase.HashGetAll((RedisKey)key))
                {
                    creaditMixResults.Add(new
                    {
                        Week = int.Parse(item.Name),
                        Total = decimal.Parse(item.Value)
                    });
                }

                //Hit Rate CSR
                //warranty
                key = string.Format("{0}:{1}:{2}", Consts.WarrantyHitRateKPIDashboardKey, branchNo, userId);
                var warrantyHitRate = redisDataBase.HashGetAll((RedisKey)key)
                    .Select(p => new KeyValuePair<int, decimal>(int.Parse(p.Name), decimal.Parse(p.Value)))
                    .ToList();
                //installations
                key = string.Format("{0}:{1}:{2}", Consts.InstallationHitRateKPIDashboardKey, branchNo, userId);
                var installationHitRate = redisDataBase.HashGetAll((RedisKey)key)
                    .Select(p => new KeyValuePair<int, decimal>(int.Parse(p.Name), decimal.Parse(p.Value)))
                    .ToList();

                var hitRate = warrantyHitRate
                    .Join(
                        installationHitRate,
                        left => left.Key,
                        right => right.Key,
                        (w, i) => new
                        {
                            Week = w.Key,
                            Warranties = w.Value,
                            Installations = i.Value
                        })
                    .ToList();

                hashTable.Add("CallsSummary", callSummaryItems);
                hashTable.Add("UndeliveredAccounts", undeliveredAccountsResults);
                hashTable.Add("SlowSrs", slowSrsResults);
                hashTable.Add("SalesSummary", salesSummaryResults);
                hashTable.Add("NewCustomerAcquisitionAndCancellations", newCustomerAcquisitionAndCancellations);
                hashTable.Add("SalesKPIFirstWeek", salesKPIFirstWeek);
                hashTable.Add("RewritesKPI", rewritesKPIResults);
                hashTable.Add("CreditMix", creaditMixResults);
                hashTable.Add("HitRate", hitRate);
            }

            return hashTable;
        }

        private Hashtable GetDashboardData(short branchNo, string callSUmmaryBranchKey)
        {
            var hashTable = new Hashtable();

            var callSummaryBranchResults = redisDataBase.HashGetAll((RedisKey)callSUmmaryBranchKey);
            var items = new Hashtable();

            hashTable.Add("IsLoading", false);
            hashTable.Add("IsRefreshing", dashboardRepository.IsLoadingDashboard);

            foreach (var item in callSummaryBranchResults)
            {
                items.Add(item.Name, item.Value);
            }

            hashTable.Add("CallsSummary", items);

            //Sales KPI First Week
            var firstWeekKey = "SalesM:SalesKpiFirstWeek";
            var salesKPIFirstWeek = redisDataBase.SetMembers((RedisKey)firstWeekKey);

            //New Customer Acquisition KPI
            var key = string.Format("{0}:{1}", Consts.NewCustomerAcquisitionKPIDashboardKey, branchNo);
            var newCustomerAcquisitionResults = new List<KeyValuePair<int, decimal>>();

            foreach (var item in redisDataBase.HashGetAll((RedisKey)key))
            {
                newCustomerAcquisitionResults.Add(new KeyValuePair<int, decimal>(
                   int.Parse(item.Name),
                   int.Parse(item.Value)));
            }

            //Cancellations KPI
            key = string.Format("{0}:{1}", Consts.CancellationKPIDashboardKey, branchNo);
            var cancellationsKPIResults = new List<KeyValuePair<int, decimal>>();

            foreach (var item in redisDataBase.HashGetAll((RedisKey)key))
            {
                cancellationsKPIResults.Add(new KeyValuePair<int, decimal>(
                   int.Parse(item.Name),
                   int.Parse(item.Value)));
            }

            var newCustomerAcquisitionAndCancellations = newCustomerAcquisitionResults
                .Join(
                    cancellationsKPIResults,
                    left => left.Key,
                    right => right.Key,
                    (nC, can) => new
                    {
                        Week = nC.Key,
                        Acquisition = nC.Value,
                        Cancelations = can.Value
                    })
                .ToList()
                .OrderBy(p => p.Week);

            //Rewrites
            key = string.Format("{0}:{1}", Consts.RewritesKPIDashboardKey, branchNo);
            var rewritesKPIResults = new List<object>();

            foreach (var item in redisDataBase.HashGetAll((RedisKey)key))
            {
                rewritesKPIResults.Add(new
                {
                    Week = int.Parse(item.Name),
                    Total = decimal.Parse(item.Value)
                });
            }

            //Credit Mix
            key = string.Format("{0}:{1}", Consts.CreditMixKPIDashboardKey, branchNo);
            var creaditMixResults = new List<object>();

            foreach (var item in redisDataBase.HashGetAll((RedisKey)key))
            {
                creaditMixResults.Add(new
                {
                    Week = int.Parse(item.Name),
                    Total = decimal.Parse(item.Value)
                });
            }

            //Hit Rate Branch
            //warranty
            key = string.Format("{0}:{1}:{2}", Consts.BranchWarrantyHitRateKPIDashboardKey, branchNo, branchNo);
            var warrantyHitRate = redisDataBase.HashGetAll((RedisKey)key)
                .Select(p => new KeyValuePair<int, decimal>(int.Parse(p.Name), decimal.Parse(p.Value)))
                .ToList();
            //installations
            key = string.Format("{0}:{1}:{2}", Consts.BranchInstallationHitRateKPIDashboardKey, branchNo, branchNo);
            var installationHitRate = redisDataBase.HashGetAll((RedisKey)key)
                .Select(p => new KeyValuePair<int, decimal>(int.Parse(p.Name), decimal.Parse(p.Value)))
                .ToList();

            var hitRateBranch = warrantyHitRate
                .Join(
                    installationHitRate,
                    left => left.Key,
                    right => right.Key,
                    (w, i) => new
                    {
                        Week = w.Key,
                        Warranties = w.Value,
                        Installations = i.Value
                    })
                .ToList();

            //Sales Summary
            key = string.Format("{0}:{1}", Consts.SalesSummaryDashboardKey, branchNo);
            var salesSummaryResults = redisDataBase.SetMembers((RedisKey)key);

            hashTable.Add("NewCustomerAcquisitionAndCancellations", newCustomerAcquisitionAndCancellations);
            hashTable.Add("SalesKPIFirstWeek", salesKPIFirstWeek);
            hashTable.Add("RewritesKPI", rewritesKPIResults);
            hashTable.Add("CreditMix", creaditMixResults);
            hashTable.Add("HitRate", hitRateBranch);
            hashTable.Add("SalesSummary", salesSummaryResults);

            return hashTable;
        }
    }
}