using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using Blue.Networking;

namespace Blue.Cosacs.Customer
{
    public class SolrIndex
    {
        const string DocType = "Customer";

        public static void CustomerDetails(string customerId = null)
        {
            var maxId = 0;
            const int step = 10000;
            var numberOfRecords = step;

            //delete everything first
            if (string.IsNullOrEmpty(customerId))
            {
                new Blue.Solr.WebClient().DeleteByType(DocType);
            }

            //first lets get the customers from win cosacs
            while (numberOfRecords == step)
            {
                var sp = new DataForReindexSolr();

                sp.CustomerId = customerId;
                sp.MaxId = maxId;

                var results = sp.Fill();

                numberOfRecords = results.Count();
                if (!string.IsNullOrEmpty(customerId) && results.Any())
                {
                    DeleteCustomer(results.Single());
                }

                SendDataToSolr(results);
                maxId += step;
            }

            //now lets get the customers from sales
            using (var scope = Context.Read())
            {
                var customers = scope.Context.CustomerSearchView
                    .Select(p => p);

                if (!string.IsNullOrEmpty(customerId))
                {
                    customers = customers
                        .Where(p => p.CustomerId == customerId);
                }

                var dataFromView = customers
                    .ToList()
                    .Select(p => (CustomerSearchResult)p)
                    .ToList();


                if (!string.IsNullOrEmpty(customerId) && dataFromView.Any())
                {
                    DeleteCustomer(dataFromView.Single());
                }

                SendDataToSolr(dataFromView);
            }
        }

        public static void IndexSalesCustomer(string firstName, string lastName)
        {
            using (var scope = Context.Read())
            {
                var dataFromView = scope.Context.CustomerSearchView
                    .Where(c => c.FirstName == firstName && c.LastName == lastName && c.IsSalesCustomer).ToList()
                    //.Where(c => c.FirstName == firstName && c.LastName == lastName && string.IsNullOrEmpty(c.CustomerId)).ToList()
                    .Select(p => (CustomerSearchResult)p).FirstOrDefault();

                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) && dataFromView != null)
                {
                    DeleteCustomer(dataFromView);
                }

                SendDataToSolr(new List<CustomerSearchResult> { dataFromView });
            }
        }

        private static void DeleteCustomer(CustomerSearchResult customer)
        {
            new Blue.Solr.WebClient().DeleteById(string.Format("{0}:{1}:{2}", DocType, customer.ID.ToString(), customer.CustomerSource));
        }

        private static void SendDataToSolr(List<CustomerSearchResult> data)
        {
            //this is not making much sense, the class CustomerSearchResult should have all the logic needed so no other loop have to be done 
            //we have to fix this to improve performance
            var values = data
                     .Select(customer => new
                     {
                         Id = string.Format("{0}:{1}:{2}", DocType, customer.ID.ToString(), customer.CustomerSource),
                         Type = DocType,
                         CustomerId = !string.IsNullOrEmpty(customer.CustomerId) ? customer.CustomerId.Trim() : string.Empty,
                         Title = !string.IsNullOrEmpty(customer.Title) ? customer.Title.Trim() : string.Empty,
                         FirstName = !string.IsNullOrEmpty(customer.FirstName) ? customer.FirstName.Trim() : string.Empty,
                         LastName = !string.IsNullOrEmpty(customer.LastName) ? customer.LastName.Trim() : string.Empty,
                         Alias = !string.IsNullOrEmpty(customer.Alias) ? customer.Alias.Trim() : string.Empty,
                         DOB = customer.DOB.HasValue ? customer.DOB.Value : (DateTime?)null,
                         HomePhoneNumber = !string.IsNullOrEmpty(customer.HomePhoneNumber) ? customer.HomePhoneNumber.Trim() : string.Empty,
                         MobileNumber = !string.IsNullOrEmpty(customer.MobileNumber) ? customer.MobileNumber.Trim() : string.Empty,
                         Email = !string.IsNullOrEmpty(customer.Email) ? customer.Email.Trim() : string.Empty,
                         AddressLine1 = !string.IsNullOrEmpty(customer.HomeAddressLine1) ? customer.HomeAddressLine1.Trim() : string.Empty,
                         AddressLine2 = !string.IsNullOrEmpty(customer.HomeAddressLine2) ? customer.HomeAddressLine2.Trim() : string.Empty,
                         TownOrCity = !string.IsNullOrEmpty(customer.City) ? customer.City.Trim() : string.Empty,
                         PostCode = !string.IsNullOrEmpty(customer.PostCode) ? customer.PostCode.Trim() : string.Empty,
                         CreditSource = customer.HasRCreditSource || customer.HasOCreditSource ? "Credit" : string.Empty,
                         CashSource = customer.HasCashSource ? "Cash" : string.Empty,
                         StoreCardSource = customer.HasStoreCardSource ? "StoreCard" : string.Empty,
                         WarrantySource = customer.HasWarrantySource ? "Warranty" : string.Empty,
                         InstallationSource = customer.HasInstallationSource ? "Installation" : string.Empty,
                         Priority = (customer.HasStoreCardSource || customer.HasRCreditSource || customer.HasOCreditSource || customer.HasCashSource)
                                     ? 1
                                     : ((customer.HasWarrantySource || customer.HasInstallationSource)
                                         ? 2
                                         : 3),
                         LatestDate = GetLatestDate(customer),
                         customer.AvailableSpend,
                         customer.DateLastBought,
                         customer.CustomerBranchNo,
                         customer.CustomerBranchName,
                         customer.SalesPersonId,
                         HasCrs = customer.SalesPersonId.HasValue ? "Yes" : "No",
                         HasPendingCalls = customer.HasPendingCalls ? "Yes" : "No",
                         customer.SalesPerson,
                         customer.CustomerSource,
                         DoNotCallAgain = customer.DoNotCallAgain,
                         CalledAt = !string.IsNullOrEmpty(customer.CalledAt) ? customer.CalledAt : null,
                         DateLastBoughtRange = !string.IsNullOrEmpty(customer.DateLastBoughtRange) ? customer.DateLastBoughtRange : string.Empty,
                         IsActiveCashCustomer = customer.IsActiveCashCustomer ? "Yes" : "No",
                         IsActiveCreditCustomer = customer.IsActiveCreditCustomer ? "Yes" : "No",
                         CustomerCallType = !string.IsNullOrEmpty(customer.CustomerCallType) ? customer.CustomerCallType : string.Empty,
                         HasBirthday = !string.IsNullOrEmpty(customer.HasBirthday) ? customer.HasBirthday : string.Empty,
                         ReceiveEmails = customer.ReceiveEmails ? "Yes" : "No",
                         ReceiveSms = customer.ResieveSms ? "Yes" : "No",
                         LastEmailSentOn = customer.LastEmailSentOn.HasValue ? customer.LastEmailSentOn.Value.ToString("dd-MM-yyyy") : string.Empty,
                         LastSmsSentOn = customer.LastSmsSentOn.HasValue ? customer.LastSmsSentOn.Value.ToString("dd-MM-yyyy") : string.Empty,
                     }).ToList();

            new Blue.Solr.WebClient().Update(values);
        }

        private static string GetLatestDate(CustomerSearchResult customer)
        {
            List<DateTime> dateList = new List<DateTime>();
            if (customer.RCreditSourceDate.HasValue)
            {
                dateList.Add(customer.RCreditSourceDate.Value);
            }
            if (customer.OCreditSourceDate.HasValue)
            {
                dateList.Add(customer.OCreditSourceDate.Value);
            }
            if (customer.CashSourceDate.HasValue)
            {
                dateList.Add(customer.CashSourceDate.Value);
            }
            if (customer.StoreCardSourceDate.HasValue)
            {
                dateList.Add(customer.StoreCardSourceDate.Value);
            }
            if (customer.WarrantySourceDate.HasValue)
            {
                dateList.Add(customer.WarrantySourceDate.Value);
            }
            if (customer.InstallationSourceDate.HasValue)
            {
                dateList.Add(customer.InstallationSourceDate.Value);
            }
            if (dateList.Count > 0)
            {
                return dateList.Max().ToSolrDate();
            }
            else
            {
                return null;
            }
        }
    }
}