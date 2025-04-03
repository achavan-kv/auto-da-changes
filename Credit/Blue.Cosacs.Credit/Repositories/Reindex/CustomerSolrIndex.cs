using Blue.Cosacs.Credit.Extensions;
using Blue.Cosacs.Credit.Model;
using Blue.Cosacs.Credit.Repositories.Reindex;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Blue.Cosacs.Credit.Repositories
{
    public class CustomerSolrIndex : ICustomerSolrIndex
    {
        private const int MAXRECORDS = 10000;
        private const string HOME = "Home";
        private const string BranchDataApi = "/Courts.NET.WS/DBOInfo/Branches";
        private readonly IHttpClientJson httpClientJson;
        private readonly Dictionary<short, string> mappedBranches;

        public CustomerSolrIndex(IHttpClientJson httpClientJson)
        {
            this.httpClientJson = httpClientJson;
            this.mappedBranches = GetMappedBranches();
        }

        public int Reindex()
        {
            using (var scope = Context.Read())
            {
                var customer = scope.Context.Customer;
                var top = customer.Max(c => c.Id);
                var count = 0;
                var recordId = 0;
                while (recordId < top)
                {
                    var records = customer.Where(c => c.Id > recordId && c.Id <= recordId + MAXRECORDS).ToList();
                    var ids = records.Select(r => r.Id).ToList();
                    var contacts = scope.Context.CustomerContact.Where(c => ids.Contains(c.CustomerId)).ToLookup(c => c.CustomerId);
                    var addresses = scope.Context.CustomerAddress.Where(a => ids.Contains(a.CustomerId) && a.AddressType == HOME).ToDictionary(a => a.CustomerId);
                    var tags = scope.Context.CustomerTag.Where(tag => ids.Contains(tag.CustomerId)).ToLookup(customerTag => customerTag.CustomerId);
                    if (records.Count() > 0)
                    {
                        PushSolr(records, contacts, addresses, tags);
                    }
                    recordId = records.Max(r => r.Id);
                    count += records.Count();
                }
                return count;
            }
        }

        public void Reindex(int[] customerIds)
        {
            using (var scope = Context.Read())
            {
                var customers = scope.Context.Customer.Where(c => customerIds.Contains(c.Id)).ToList();
                var contacts = scope.Context.CustomerContact.Where(c => customerIds.Contains(c.CustomerId)).ToLookup(c => c.CustomerId);
                var tags = scope.Context.CustomerTag.Where(c => customerIds.Contains(c.CustomerId)).ToLookup(c => c.CustomerId);
                var addresses = scope.Context.CustomerAddress.Where(a => customerIds.Contains(a.CustomerId) && a.AddressType == HOME).ToDictionary(a => a.CustomerId);
                if (customers.Count() > 0)
                {
                    PushSolr(customers, contacts, addresses, tags);
                }
            }
        }

        private Dictionary<short, string> GetMappedBranches()
        {
            var results = httpClientJson.Do<byte[], List<BranchInfo>>(RequestJson<byte[]>.Create(BranchDataApi, WebRequestMethods.Http.Post));

            return results.Body.ToDictionary(item => item.BranchNumber, item => item.BranchNumber + " " + item.BranchName);
        }

        private void PushSolr(IEnumerable<Customer> customers, ILookup<int, CustomerContact> contacts = null, IDictionary<int, CustomerAddress> addresses = null,  ILookup<int, CustomerTag> tags = null)
        {
            const string DocType = "CustomerCredit";
            const string HOME = "HomePhone";
            const string WORK = "WorkPhone";
            const string EMAIL = "Email";

            new Blue.Solr.WebClient().Update(customers
                     .Select(customer => new
                     {
                         Id = string.Format("{0}:{1}", DocType, customer.Id.ToString()),
                         Type = DocType,
                         CustomerId = customer.Id,
                         Title = customer.Title.SafeTrim(),
                         FirstName = customer.FirstName,
                         LastName = customer.LastName,
                         Alias = customer.Alias.SafeTrim(),
                         HomeBranchName = mappedBranches.ContainsKey(customer.Branch) ? mappedBranches[customer.Branch] : customer.Branch.ToString(),
                         StaffMember = customer.AdminUserId.HasValue ? "Yes" : "No",
                         Tags = tags[customer.Id].Count() > 0 ? tags[customer.Id].Select(t => t.Tag).ToArray<string>() : null,
                         DOB = customer.DateOfBirth.HasValue ? customer.DateOfBirth.Value : (DateTime?)null,
                         HomePhoneNumber = contacts[customer.Id].Where(c => c.ContactType == HOME).Count() > 0 ? contacts[customer.Id].Where(c => c.ContactType == HOME).FirstOrDefault().Contact : null,
                         MobileNumber = contacts[customer.Id].Where(c => c.ContactType == WORK).Count() > 0 ? contacts[customer.Id].Where(c => c.ContactType == WORK).FirstOrDefault().Contact : null,
                         Email = contacts[customer.Id].Where(c => c.ContactType == EMAIL).Count() > 0 ? contacts[customer.Id].Where(c => c.ContactType == EMAIL).FirstOrDefault().Contact : null,
                         AddressLine1 = addresses.ContainsKey(customer.Id) ? addresses[customer.Id].AddressLine1 : null,
                         AddressLine2 = addresses.ContainsKey(customer.Id) ? addresses[customer.Id].AddressLine2 : null,
                         TownOrCity = addresses.ContainsKey(customer.Id) ? addresses[customer.Id].City : null,
                         PostCode = addresses.ContainsKey(customer.Id) ? addresses[customer.Id].PostCode : null,
                         DeliveryArea = addresses.ContainsKey(customer.Id) ? addresses[customer.Id].DeliveryArea : null
                     }).ToList());
        }
    }
}