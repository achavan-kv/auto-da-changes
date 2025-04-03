using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Api.Models;
using Blue.Cosacs.SalesManagement.EventTypes;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using Blue.Networking;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/ScheduleCallsBulk")]
    public class ScheduleCallsBulkController : ApiController
    {
        private readonly ISalesManagementRepository repository;
        private readonly IClock clock;
        private readonly IHttpClient httpClient;
        private readonly IEventStore audit;

        public ScheduleCallsBulkController(IClock clock, ISalesManagementRepository repository, IHttpClient httpClient, IEventStore audit)
        {
            this.clock = clock;
            this.repository = repository;
            this.httpClient = httpClient;
            this.audit = audit;
        }

        [Route("BranchManagerBulk")]
        [HttpPut]
        [Permission(SalesManagementPermissionEnum.CustomersearchBranchCustomers)]
        public HttpResponseMessage BranchManagerBulk(CustomerBulk data)
        {
            InsertCalls(data);
            return Request.CreateResponse();
        }

        [Route("BranchManagerCallsAll")]
        [HttpPut]
        [Permission(SalesManagementPermissionEnum.CustomersearchBranchCustomers)]
        public HttpResponseMessage BranchManagerCallsAll(BranchManagerCallsAll data)
        {
            var qs = System.Web.HttpUtility.UrlEncode(data.CustomerFilter);
            var query = new CustomersQuery(this.clock, this.httpClient);
            var csr = repository.GetCustomersSalesPerson(null, null).ToDictionary(p => p.CustomerId, v => v, StringComparer.InvariantCultureIgnoreCase);
            
            var values = query.QueryCustomers(qs, this.GetUser().Branch, this.GetUser().Id)
                .Where(p => !p.DoNotCallAgain)
                .Select(p => new CustomerBulkInserCall
                {
                    CustomerFirstName = p.FirstName,
                    LandLinePhone = p.HomePhoneNumber,
                    CustomerId = p.CustomerId,
                    CustomerLastName = p.LastName,
                    Email = p.Email,
                    MobileNumber = p.MobileNumber,
                    SalesPersonId = csr[p.CustomerId].SalesPersonId
                })
                .ToArray();

            InsertCalls(new CustomerBulk
            {
                Customers = values,
                ReasonForCalling = data.ReasonForCalling,
                ToCallAt = data.ToCallAt
            });

            return this.Request.CreateResponse();
        }

        private void InsertCalls(CustomerBulk data)
        {
            var existingCustomers = repository.GetCustomersSalesPerson(null, null).ToDictionary(p => p.CustomerId);
            var custDictionary = data.Customers
                .ToDictionary(p => p.CustomerId, v => v, StringComparer.InvariantCultureIgnoreCase);

            var customersToInsert = custDictionary.Keys.Except(existingCustomers.Keys)
                .Select(p => new CustomerSalesPerson
                {
                    CustomerBranch = this.GetUser().Branch,
                    CustomerId = p,
                    DoNotCallAgain = false,
                    LandLinePhone = custDictionary[p].LandLinePhone,
                    MobileNumber = custDictionary[p].MobileNumber,
                    SalesPersonId = custDictionary[p].SalesPersonId,
                    TempSalesPersonId = null,
                    TempSalesPersonIdBegin = null,
                    TempSalesPersonIdEnd = null,
                })
                .ToList();

            var excludedCustomers = existingCustomers
                    .Where(p => p.Value.DoNotCallAgain)
                    .Select(p => p.Key)
                    .ToDictionary(p => p);

            var callsToInsert = data.Customers
                .Where(p => !excludedCustomers.ContainsKey(p.CustomerId))
                .Select(p => new Call
                {
                    CallClosedReasonId = null,
                    CalledAt = (DateTime?)null,
                    SalesPersonId = existingCustomers.ContainsKey(p.CustomerId) ? existingCustomers[p.CustomerId].SalesPersonId : p.SalesPersonId,
                    CallTypeId = (int)CallTypeEnum.BranchManagerCustom,
                    Comments = null,
                    CreatedBy = null,
                    CreatedOn = clock.Now,
                    CustomerFirstName = p.CustomerFirstName,
                    CustomerId = p.CustomerId,
                    CustomerLastName = p.CustomerLastName,
                    PreviousCallId = (int?)null,
                    ReasonToCall = data.ReasonForCalling,
                    SpokeToCustomer = false,
                    ToCallAt = data.ToCallAt,
                    Source = (byte)CallSourceEnum.UserInterface,
                    LandLinePhone = p.LandLinePhone,
                    MobileNumber = p.MobileNumber,
                })
                .ToList();

            repository.InsertCustomersSalesPerson(customersToInsert);
            repository.SaveCalls(callsToInsert);

            audit.LogAsync(
                       new
                       {
                           ToCallAt = data.ToCallAt,
                           ReasonForCalling = data.ReasonForCalling,
                           TotalCalls = data.Customers.Count()
                       },
                       EventType.BulkScheduleEmails);
        }

        [Route("CSRBulk")]
        [HttpPut]
        [Permission(SalesManagementPermissionEnum.CustomersearchMyCustomers)]
        public HttpResponseMessage CSRBulk(CustomerBulk data)
        {
            var existingCustomers = repository.GetCustomersSalesPerson(null, null).ToDictionary(p => p.CustomerId);
            var custDictionary = data.Customers
                .ToDictionary(p => p.CustomerId);

            var customersToInsert = custDictionary.Keys.Except(existingCustomers.Keys)
                .Select(p => new CustomerSalesPerson
                {
                    CustomerBranch = this.GetUser().Branch,
                    CustomerId = p,
                    DoNotCallAgain = false,
                    MobileNumber = custDictionary[p].MobileNumber,
                    LandLinePhone = custDictionary[p].LandLinePhone,
                    SalesPersonId = custDictionary[p].SalesPersonId,
                    Email = custDictionary[p].Email,
                    TempSalesPersonId = null,
                    TempSalesPersonIdBegin = null,
                    TempSalesPersonIdEnd = null
                })
                .ToList();

            var excludedCustomers = existingCustomers
                    .Where(p => p.Value.DoNotCallAgain)
                    .Select(p => p.Key)
                    .ToDictionary(p => p);

            var callsToInsert = data.Customers
                .Where(p => !excludedCustomers.ContainsKey(p.CustomerId))
                .Select(p => new Call
                {
                    CallClosedReasonId = null,
                    CalledAt = (DateTime?)null,
                    SalesPersonId = existingCustomers.ContainsKey(p.CustomerId) ? existingCustomers[p.CustomerId].SalesPersonId : p.SalesPersonId,
                    CallTypeId = (int)CallTypeEnum.CsrCustom,
                    Comments = null,
                    CreatedBy = null,
                    CreatedOn = clock.Now,
                    CustomerFirstName = p.CustomerFirstName,
                    CustomerId = p.CustomerId,
                    CustomerLastName = p.CustomerLastName,
                    PreviousCallId = (int?)null,
                    ReasonToCall = data.ReasonForCalling,
                    SpokeToCustomer = false,
                    ToCallAt = data.ToCallAt,
                    Source = (byte)CallSourceEnum.UserInterface,
                    MobileNumber = p.MobileNumber,
                    LandLinePhone = p.LandLinePhone,
                })
                .ToList();

            repository.InsertCustomersSalesPerson(customersToInsert);
            repository.SaveCalls(callsToInsert);

            return Request.CreateResponse();
        }
    }
}