using System;
using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Service.Models;
using Blue.Cosacs.Service.Util;

namespace Blue.Cosacs.Service.Repositories
{
    //Hack until we get the hub working.
    public class CosacsRepository
    {
        public CosacsRepository(IClock clock)
        {
            this.clock = clock;
        }

        private readonly IClock clock;
        public CustomerSearchResult[] Search(string type, string value, string branch, string srtype, bool isInternal = false)
        {
            if (type == null || value == null)
                throw new Exception("No search strings specified");

            if (type == "Invoice")
                return SearchInvoice(value, branch);

            var customerId = (type == "Customer") ? value : null;
            var customerAccount = (type == "Account") ? value : null;


            var custTemp = GetNewInternalServiceRequests(srtype, customerId, customerAccount);

            using (var scope = Context.Read())
            {
                var cust = custTemp.Distinct().ToArray();

                if (!cust.Any()) return cust;

                var custid = cust[0].CustomerId;
                var contact = (from t in scope.Context.CustTelView
                               where t.custid == custid
                               select new CustomerSearchResult.Contact()
                               {
                                   Type = t.Type,
                                   Value = t.telno
                               }).ToList();

                if (!contact.Any())
                {
                    contact.Add(new CustomerSearchResult.Contact { Type = "HomePhone", Value = "" });
                }

                foreach (var result in cust)
                {
                    result.Contacts = contact.ToArray();

                    var Address = (from t in scope.Context.AddressCode
                                  join a in scope.Context.custaddress on t.code.Trim() equals a.addtype.Trim()
                                 where t.category == "CA1" && a.custid == custid
                                   select new CustomerSearchResult.Addresses()
                                   {
                                       codedescript = t.codedescript,
                                       code = t.code,
                                       addtype=t.code,
                                       CustomerAddressLine1=a.cusaddr1,
                                       CustomerAddressLine2 = a.cusaddr2,
                                       CustomerAddressLine3 = a.cusaddr3,
                                       CustomerPostcode = a.cuspocode,
                                       CustomerNotes = a.notes
                                   }).ToList();

                    result.Address = Address.ToArray();

                    result.History = (from sr in scope.Context.Request
                                      where sr.ItemNumber == result.Iupc &&
                                            sr.ItemSupplier == result.ItemSupplier &&
                                            sr.Account == result.Account
                                      select new CustomerSearchResult.HistoryItem()
                                      {
                                          UpdatedOn = sr.LastUpdatedOn,
                                          CreatedOn = sr.CreatedOn,
                                          RequestId = sr.Id,
                                          Status = sr.State,
                                          SerialNumber = sr.ItemSerialNumber
                                      }).OrderByDescending(h => h.UpdatedOn).ToArray();

                    var historyIds = result.History.Select(h => h.RequestId).ToArray();

                    result.HistoryCharges = (from c in scope.Context.Charge
                                             where historyIds.Contains(c.RequestId)
                                             select new CustomerSearchResult.ChargeItem
                                             {
                                                 Account = c.Account,
                                                 ChargeType = c.Type,
                                                 Tax = c.Tax,
                                                 Value = c.Value,
                                                 RequestId = c.RequestId
                                             }).ToArray();
                }

                return cust;
            }
        }

        // CR2018-008 by tosif ali 17/10/2018*@
        public CustomerSearchResult[] ExternalSearch(string type, string value, string branch, string srtype, bool isInternal = false)
        {
		    //log no- 6747341
            if (type == null || value == null)
                return null;

            if (type == "Invoice")
                return SearchInvoice(value, branch);

            var customerId = (type == "Customer") ? value : null;
            var customerAccount = (type == "Account") ? value : null;


            var custTemp = GetNewExternallServiceRequests(customerId);

            using (var scope = Context.Read())
            {
                var cust = custTemp.Distinct().ToArray();

                if (!cust.Any()) return cust;

                var custid = cust[0].CustomerId;
                var contact = (from t in scope.Context.CustTelView
                               where t.custid == custid
                               select new CustomerSearchResult.Contact()
                               {
                                   Type = t.Type,
                                   Value = t.telno
                               }).ToList();

                if (!contact.Any())
                {
                    contact.Add(new CustomerSearchResult.Contact { Type = "HomePhone", Value = "" });
                }

                foreach (var result in cust)
                {
                    result.Contacts = contact.ToArray();

                    var Address = (from t in scope.Context.AddressCode
                                   join a in scope.Context.custaddress on t.code.Trim() equals a.addtype.Trim()
                                   where t.category == "CA1" && a.custid== custid
                                   select new CustomerSearchResult.Addresses()
                                   {
                                       codedescript = t.codedescript,
                                       code = t.code
                                   }).ToList();

                    result.Address = Address.ToArray();

                    result.History = (from sr in scope.Context.Request
                                    where sr.CustomerId == result.CustomerId
                                      select new CustomerSearchResult.HistoryItem()
                                      {
                                          UpdatedOn = sr.LastUpdatedOn,
                                          CreatedOn = sr.CreatedOn,
                                          RequestId = sr.Id,
                                          Status = sr.State,
                                          SerialNumber = sr.ItemSerialNumber
                                      }).OrderByDescending(h => h.UpdatedOn).ToArray();

                    var historyIds = result.History.Select(h => h.RequestId).ToArray();

                    result.HistoryCharges = (from c in scope.Context.Charge
                                             where historyIds.Contains(c.RequestId)
                                             select new CustomerSearchResult.ChargeItem
                                             {
                                                 Account = c.Account,
                                                 ChargeType = c.Type,
                                                 Tax = c.Tax,
                                                 Value = c.Value,
                                                 RequestId = c.RequestId
                                             }).ToArray();
                }

                return cust;
            }
        }
        //End Hear
 private CustomerSearchResult[] SearchInvoice(string invoice, string branch)
        {
            int i;
            int b;
            int.TryParse(invoice, out i);
            int.TryParse(branch, out b);

            using (var scope = Context.Read())
            {
               
                // new code addded for newly type invoice done tosif
                if (invoice.Length>13)
                {
                    
                    Int32 Id =Convert.ToInt32(scope.Context.Order.Where(x => x.AgreementInvoiceNumber == invoice.Replace("-", "")).SingleOrDefault().Id);
                    var iview = (from iv in scope.Context.ItemsByInvoiceNoSearchView
                                 where iv.InvoiceNumber == Id && iv.StockLocation == b && iv.TotalRequests < iv.TotalItemCount
                                 select iv).Distinct().ToList();
                    if (invoice.Contains("-"))
                    {
                        invoice = invoice.Replace("-", "").Replace("--", "").Trim();
                    }

                    var retLst = (from v in iview
                                  join brn in scope.Context.BranchLookup on v.StockLocation equals brn.branchno
                                  select new CustomerSearchResult()
                                  {
                                      Item = v.ItemDescription1,
                                      ItemAmount = Convert.ToDecimal(v.Price),
                                      ItemDeliveredOn = Convert.ToDateTime(v.DeliveryDate),
                                      ItemId = Convert.ToInt32(v.ItemId),
                                      ItemNumber = v.itemNo,
                                      ItemStockLocation = v.StockLocation,
                                      ItemStockLocationName = brn.BranchNameLong,
                                      ItemSupplier = v.Supplier,
                                      ItemSoldOn = v.SoldOn.Value,
                                      ItemSoldBy = v.SoldBy,
                                      ItemSoldByName = v.SoldByName,
                                      WarrantyNumber = v.WarrantyNumber,
                                      WarrantyLength = (int?)v.WarrantyLength,
                                      WarrantyContractNumber = v.WarrantyContractNo,
                                      ManufacturerWarrantyLength = Convert.ToInt32(v.ManWarrantyLength),
                                      ManufacturerWarrantyContractNumber = v.ManufacturerWarrantyContractNo,
                                      CustomerId = v.CustomerID,
                                      Account = v.AccountNo,
                                      ItemInvoiceNo = invoice.Insert(3, "-"),
                                      TotalItemCount = v.TotalItemCount.GetValueOrDefault(),
                                      TotalRequests = v.TotalRequests,
                                      CustomerTitle = v.CustomerTitle,
                                      CustomerFirstName = v.CustomerFirstName,
                                      CustomerLastName = v.CustomerLastName,
                                      CustomerAddressLine1 = v.CustomerAddressLine1,
                                      CustomerAddressLine2 = v.CustomerAddressLine2,
                                      CustomerAddressLine3 = v.CustomerAddressLine3,
                                      CustomerPostcode = v.CustomerPostcode,
                                      CustomerNotes = v.CustomerNotes
                                  }).ToArray();


                    return retLst;

                }
                // end h
                else
                {
                    var iview = (from iv in scope.Context.ItemsByInvoiceNoSearchView
                                 where iv.InvoiceNumber == i && iv.StockLocation == b && iv.TotalRequests < iv.TotalItemCount
                                 select iv).Distinct().ToList();
                    var retLst = (from v in iview
                                  join brn in scope.Context.BranchLookup on v.StockLocation equals brn.branchno
                                  select new CustomerSearchResult()
                                  {
                                      Item = v.ItemDescription1,
                                      ItemAmount = Convert.ToDecimal(v.Price),
                                      ItemDeliveredOn = Convert.ToDateTime(v.DeliveryDate),
                                      ItemId = Convert.ToInt32(v.ItemId),
                                      ItemNumber = v.itemNo,
                                      ItemStockLocation = v.StockLocation,
                                      ItemStockLocationName = brn.BranchNameLong,
                                      ItemSupplier = v.Supplier,
                                      ItemSoldOn = v.SoldOn.Value,
                                      ItemSoldBy = v.SoldBy,
                                      ItemSoldByName = v.SoldByName,
                                      WarrantyNumber = v.WarrantyNumber,
                                      WarrantyLength = (int?)v.WarrantyLength,
                                      WarrantyContractNumber = v.WarrantyContractNo,
                                      ManufacturerWarrantyLength = Convert.ToInt32(v.ManWarrantyLength),
                                      ManufacturerWarrantyContractNumber = v.ManufacturerWarrantyContractNo,
                                      CustomerId = v.CustomerID,
                                      Account = v.AccountNo,
                                      ItemInvoiceNo =Convert.ToString(v.InvoiceNumber.GetValueOrDefault()),
                                      TotalItemCount = v.TotalItemCount.GetValueOrDefault(),
                                      TotalRequests = v.TotalRequests,
                                      CustomerTitle = v.CustomerTitle,
                                      CustomerFirstName = v.CustomerFirstName,
                                      CustomerLastName = v.CustomerLastName,
                                      CustomerAddressLine1 = v.CustomerAddressLine1,
                                      CustomerAddressLine2 = v.CustomerAddressLine2,
                                      CustomerAddressLine3 = v.CustomerAddressLine3,
                                      CustomerPostcode = v.CustomerPostcode,
                                      CustomerNotes = v.CustomerNotes
                                  }).ToArray();


                    return retLst;
                }
                
                
            }
        }

        public string GetCountryCode()
        {
            using (var scope = Context.Read())
                return scope.Context.CountryView.FirstOrDefault().countrycode;
        }

        public double GetExchangeRate(string payMethod)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ExchangeRateView.Where(e => e.Currency == payMethod)
                       .OrderByDescending(e => e.DateFrom)
                       .Select(e => e.Rate).FirstOrDefault();
            }
        }

        public IEnumerable<CustomerSearchResult> GetNewInternalServiceRequests(string strType, string customerId, string customerAccount)
        {
            return NewInternalServiceRequest.Fill(strType, customerId, customerAccount);
        }
        // CR2018-008 by tosif ali 17/10/2018*@
        public IEnumerable<CustomerSearchResult> GetNewExternallServiceRequests(string customerId)
        {


            using (var scope = Context.Read())
            {
                

                var retLst = (from C in scope.Context.customer
                              join CA in scope.Context.custaddress on C.custid equals CA.custid
                              where C.custid== customerId
                              select new CustomerSearchResult()
                              {
                                  CustomerId = C.custid,
                                  CustomerTitle = C.title,
                                  CustomerFirstName = C.firstname,
                                  CustomerLastName = C.name,
                                  CustomerAddressLine1 = CA.cusaddr1,
                                  CustomerAddressLine2 = CA.cusaddr2,
                                  CustomerAddressLine3 = CA.cusaddr3,
                                  CustomerPostcode = CA.cuspocode,
                                  addtype = CA.addtype,
                                  CustomerNotes = CA.notes
                              }).ToArray();


                return retLst;
            }
        }
        //End Hear
    }
}
