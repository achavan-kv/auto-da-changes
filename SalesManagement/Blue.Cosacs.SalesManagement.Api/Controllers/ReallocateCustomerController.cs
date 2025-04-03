using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Repositories;
using System.Net.Http;
using Blue.Cosacs.SalesManagement.Api.Models;
using Blue.Events;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/ReallocateCustomer")]
    public class ReallocateCustomerController : ApiController
    {
        private readonly ISalesManagementRepository repository;
        private readonly IClock clock;
        private readonly IEventStore audit;

        public ReallocateCustomerController(IClock clock, ISalesManagementRepository repository, IEventStore audit)
        {
            this.clock = clock;
            this.repository = repository;
            this.audit = audit;
        }

        [Route("AllocateCustomersToCSR")]
        [HttpPut]
        [Permission(SalesManagementPermissionEnum.CustomersearchReallocateCustomers)]
        public HttpResponseMessage AllocateCustomersToCSR(ReallocateCustomer reallocateCustomer)
        {
            var branchNo = this.GetUser().Branch;
            var customersToReallocate = reallocateCustomer.Customers.Select(c => c.CustomerId).ToList();

            var existingCustomers = repository.GetCustomersSalesPerson(null, customersToReallocate).ToDictionary(p => p.CustomerId);
            var oldCustomers = repository.GetCustomersSalesPerson(null, customersToReallocate).ToList();

            var custDictionary = reallocateCustomer.Customers
                .ToDictionary(p => p.CustomerId);

            var customersToInsert = custDictionary.Keys.Except(existingCustomers.Keys)
                .Select(p => new CustomerSalesPerson
                {
                    CustomerBranch = branchNo,
                    CustomerId = p,
                    DoNotCallAgain = false,
                    LandLinePhone = custDictionary[p].LandLinePhone,
                    MobileNumber = custDictionary[p].MobileNumber,
                    SalesPersonId = custDictionary[p].SalesPersonId,
                    TempSalesPersonId = null,
                    TempSalesPersonIdBegin = null,
                    TempSalesPersonIdEnd = null
                })
                .ToList();

            if (customersToInsert.Any())
            {
                repository.InsertCustomersSalesPerson(customersToInsert);
            }

            var allocateCustomersPermanent = reallocateCustomer.Customers.Where(p => p.AllocateTo == null &&
                                                                                p.AllocateFrom == null)
                                                                                .ToDictionary(p => p.CustomerId);

            var allocateCustomersTemporary = reallocateCustomer.Customers.Where(p => p.AllocateTo != null &&
                                                                                p.AllocateFrom != null)
                                                                                .ToDictionary(p => p.CustomerId);

            var updatePermanentAllocatedCustomers = allocateCustomersPermanent.Keys.Select(p => new CustomerSalesPerson
            {
                CustomerBranch = branchNo,
                CustomerId = p,
                DoNotCallAgain = false,
                LandLinePhone = custDictionary[p].LandLinePhone,
                MobileNumber = custDictionary[p].MobileNumber,
                SalesPersonId = custDictionary[p].CSRId,
                TempSalesPersonId = null,
                TempSalesPersonIdBegin = null,
                TempSalesPersonIdEnd = null
            })
            .ToList();

            var updateTemporaryAllocatedCustomers = allocateCustomersTemporary.Keys.Select(p => new CustomerSalesPerson
            {
                CustomerBranch = branchNo,
                CustomerId = p,
                DoNotCallAgain = false,
                LandLinePhone = custDictionary[p].LandLinePhone,
                MobileNumber = custDictionary[p].MobileNumber,
                SalesPersonId = custDictionary[p].SalesPersonId,
                TempSalesPersonId = custDictionary[p].CSRId,
                TempSalesPersonIdBegin = custDictionary[p].AllocateFrom,
                TempSalesPersonIdEnd = custDictionary[p].AllocateTo
            })
            .ToList();

             updatePermanentAllocatedCustomers.AddRange(updateTemporaryAllocatedCustomers);

             repository.AllocateCustomersToCSR(updatePermanentAllocatedCustomers, oldCustomers, audit);

            return Request.CreateResponse();
        }
    }
}