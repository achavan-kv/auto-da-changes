using STL.Common.Constants.Tags;
using STL.PL.Cache;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;

namespace STL.PL.WebRefAuth
{
    public delegate TResult Func<out TResult>();



    public class CheckCache
    {
        private object Clone(object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (obj.GetType().IsSerializable)
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, obj);
                    stream.Position = 0;
                    return formatter.Deserialize(stream);
                }
                return null;
            }
        }

        public object[] Get(string methodName, object[] parameters, Func<object[]> call)
        {
            var caches = new List<string>() {"TermsType","TermsTypeBandList","TermsTypeBand","SourceOfAttraction","SalesStaff","AllStaff","CommStaff","SalesCommStaff","MethodOfPayment",
                "AccountType","BranchNumber","CustomerCodes","AccountCodes","UserTypes","UserFunctions","AddressType","Bank","ApplicationType","DDDueDate","ProductCategories","Deposits",
                "NonDeposits","GeneralTransactions","WriteOffCodes","EndPeriods","CountryParameterCategories","EODConfigurations","InsuranceTypes","InstallationItemCat","CashLoanDisbursementMethods","Villages"};

            string key = string.Empty;
            var wCache = new WCache.Cache(true);
            var clientCache = Cache.Cache.Instance;

            // Name before the first - is the lookup on the cacheTableChange table.
            switch (methodName)
            {
                case "GetSetsForTNameBranch":
                    key = string.Format("GetSetsForTNameBranch-{0}-{1}-{2}", methodName, parameters[0], parameters[1]);
                    break;
                case "GetSetsForTNameBranchAll":
                    key = string.Format("GetSetsForTNameBranch-{0}-{1}", methodName, parameters[0]);
                    break;
                case "GetDynamicMenus":
                    key = string.Format("GetDynamicMenus-{0}-{1}-{2}", methodName, parameters[0], parameters[1]);
                    break;
                case "GetDropDownData":
                    var nodes = ((XmlNode)parameters[0]);
                    var child = nodes.FirstChild;
                    string p = string.Empty;
                    if (nodes.ChildNodes.Count == 1 && caches.Contains(child.Attributes[Tags.Name].Value))
                    {
                        do
                        {
                            if (child.NextSibling != null)
                            {
                                p += child.NextSibling.Attributes[Tags.Name].Value;
                            }
                        }
                        while (child.NextSibling != null);

                        key = string.Format("{0}-GetDropDownData-{1}", nodes.FirstChild.Attributes[Tags.Name].Value, p);
                    }
                    break;
                case "GetCountryMaintenanceParameters":
                    key = string.Format("GetCountryMaintenanceParameters-{0}", parameters[0]);
                    break;
                default:
                    return call();
            }

            var local = clientCache.Get(key);
            var serverDate = wCache.Get(key);
            if (serverDate.HasValue && (local == null || serverDate.Value != local.CacheDate))
            {
                var ws = call();
                clientCache.Add(key, new CacheItem { CacheDate = serverDate.Value, Value = ws });
                return ws;
            }
            else
            {
                return (local == null) ? call() : new object[] { Clone(local.Value[0]), string.Empty };
            }
        }
    }
}

