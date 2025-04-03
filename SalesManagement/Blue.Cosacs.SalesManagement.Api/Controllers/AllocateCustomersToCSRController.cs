using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Permission(SalesManagementPermissionEnum.AllocateCustomersToCSRJob)]
    public class AllocateCustomersToCSRController : ApiController
    {
        private readonly IClock clock;
        private readonly Blue.Cosacs.SalesManagement.Settings cosacsSettings;
        private readonly ISalesManagementRepository repository;
        private readonly IHttpClientJson httpClientJson;

        public AllocateCustomersToCSRController(IClock clock, ISalesManagementRepository repository, IHttpClientJson httpClientJson)
        {
            this.cosacsSettings = new Blue.Cosacs.SalesManagement.Settings();
            this.clock = clock;
            this.repository = repository;
            this.httpClientJson = httpClientJson;
        }

        [HttpGet]
        [LongRunningQueries]
        [CronJob]
        public HttpResponseMessage Get()
        {
            var allocationDate = GetAllocationDate();
            var customersFromWinCosacs = ExternalHttpSources.Post<List<AllocateCustomers>>(
                "/Courts.NET.WS/SalesManagement/GetAllocatedCustomersToCSR?allocationDate=" + allocationDate.ToSolrDate(),
                httpClientJson);

            var allocatedCustomers = repository.GetCustomersSalesPerson(null, customersFromWinCosacs.Select(p => p.CustomerId).ToList()).ToList();
            var customers = customersFromWinCosacs.Where(p => allocatedCustomers.Select(c => c.CustomerId).Contains(p.CustomerId));
            var allocatedCustomersToUpdate = customers
                .Select(p => new CustomerSalesPerson
                {
                    CustomerId = p.CustomerId,
                    SalesPersonId = p.SalesPerson,
                    CustomerBranch = p.CustomerBranch,
                    Email = p.Email == null ? (string)null : p.Email.Trim(),
                    DoNotCallAgain = false,
                    LandLinePhone = p.LandLinePhone,
                    MobileNumber = p.MobileNumber
                })
                .ToList();

            var newAllocatedCustomers = new HashSet<string>(customersFromWinCosacs.Select(p => p.CustomerId));
            newAllocatedCustomers.ExceptWith(allocatedCustomersToUpdate.Select(p => p.CustomerId));

            var allocatedCustomersToInsert = GetAllocatedCustomersToInsert(customersFromWinCosacs, newAllocatedCustomers);

            repository.InsertCustomersSalesPerson(allocatedCustomersToInsert);
            repository.UpdateCustomersSalesPerson(allocatedCustomersToUpdate.ToDictionary(p => p.CustomerId));

            return Request.CreateResponse();
        }

        internal IList<CustomerSalesPerson> GetAllocatedCustomersToInsert(IList<AllocateCustomers> customersFromWinCosacs, HashSet<string> newAllocatedCustomers)
        {
            return customersFromWinCosacs
                 .Join(
                    newAllocatedCustomers,
                    left => left.CustomerId,
                    right => right,
                    (l, r) => new CustomerSalesPerson
                    {
                        CustomerId = l.CustomerId,
                        SalesPersonId = l.SalesPerson,
                        DoNotCallAgain = false,
                        CustomerBranch = l.CustomerBranch,
                        Email = l.Email,
                        LandLinePhone = l.LandLinePhone,
                        MobileNumber = l.MobileNumber
                    })
                 .ToList();
        }

        private DateTime GetAllocationDate()
        {
            return this.clock.Now.AddDays(-1);
        }
    }
}