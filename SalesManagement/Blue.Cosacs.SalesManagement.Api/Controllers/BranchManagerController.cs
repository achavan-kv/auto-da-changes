using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Api.Models;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/BranchManager")]
    public class BranchManagerController : ApiController
    {
        private ISalesManagementRepository salesManagementRepository;
        private IClock clock;
        private IEventStore audit;

        public BranchManagerController(ISalesManagementRepository salesManagementRepository, IClock clock, IEventStore audit)
        {
            this.salesManagementRepository = salesManagementRepository;
            this.clock = clock;
            this.audit = audit;
        }

        [Route("GetCallTypesForBranchManager")]
        [Permission(SalesManagementPermissionEnum.BranchManagerReallocateCalls)]
        public HttpResponseMessage GetCallTypesForBranchManager()
        {
            var callTypes = salesManagementRepository.GetCallTypesForBranchManager()
                .Select(p => new
                {
                    p.Id,
                    p.Name
                })
                .ToList();

            return Request.CreateResponse(callTypes);
        }

        [Route("GetBranchManagerCustomers")]
        [Permission(SalesManagementPermissionEnum.BranchManagerReallocateCalls)]
        public HttpResponseMessage GetBranchManagerCustomers(byte? callTypeId, DateTime? fromScheduledDate, DateTime? toScheduledDate, string customerName, string reasonForCalling, int? csrId, int take)
        {
            var searchFilter = new CallSearchFilter()
            {
                Branch = this.GetUser().Branch,
                CallTypeId = callTypeId,
                ScheduledDateFrom = fromScheduledDate,
                ScheduledDateTo = toScheduledDate,
                CustomerName = customerName,
                ReasonForCalling = reasonForCalling,
                Take = take,
                CSRId = csrId
            };

            var customers = salesManagementRepository.GetBranchManagerCalls(searchFilter);

            return Request.CreateResponse(customers);
        }

        [Permission(SalesManagementPermissionEnum.BranchManagerReallocateCalls)]
        public HttpResponseMessage Get([FromUri]SearchFilter searchFilter)
        {
            var customers = salesManagementRepository.GetBranchManagerCalls(searchFilter.ToCallSearchFilter(this.GetUser().Branch));

            return Request.CreateResponse(customers);
        }

        [Route("GetUnavailableCSR")]
        [Permission(SalesManagementPermissionEnum.BranchManagerReallocateCalls)]
        public HttpResponseMessage GetUnavailableCSR()
        {
            var unavailableCSR = salesManagementRepository.GetUnavailableCSR(this.GetUser().Branch);

            return Request.CreateResponse(unavailableCSR);
        }

        [Permission(SalesManagementPermissionEnum.BranchManagerReallocateCalls)]
        public HttpResponseMessage Post(UnallocatedCalls unallocatedCalls)
        {
            var selectedCalls = unallocatedCalls.SelectedCalls;
            var salesPersonId = unallocatedCalls.SalesPersonId;

            salesManagementRepository.AllocateUnallocatedCalls(selectedCalls, salesPersonId, audit);
            return Request.CreateResponse(new
            {
                Message = "Calls updated"
            });
        }

        public class SearchFilter
        {
            public byte? CallTypeId { get; set; }
            public DateTime? ScheduledDateFrom { get; set; }
            public DateTime? ScheduledDateTo { get; set; }
            public string CustomerName { get; set; }
            public string ReasonForCalling { get; set; }
            public int SalesPersonId { get; set; }
            public int? CSRId { get; set; }
            public int? Take { get; set; }
            public bool UnavailableCSR { get; set; }
            public bool NoCSR { get; set; }
            public bool LockedCSR { get; set; }
            public int[] LockedCSRList { get; set; }

            internal CallSearchFilter ToCallSearchFilter(short branch)
            {
                return new CallSearchFilter
                {
                    Branch = branch,
                    CallTypeId = this.CallTypeId,
                    CSRId = this.CSRId,
                    CustomerName = this.CustomerName,
                    LockedCSR = this.LockedCSR,
                    LockedCSRList = this.LockedCSRList,
                    NoCSR = this.NoCSR,
                    ReasonForCalling = this.ReasonForCalling,
                    SalesPersonId = this.SalesPersonId,
                    ScheduledDateFrom = this.ScheduledDateFrom,
                    ScheduledDateTo = this.ScheduledDateTo,
                    Take = this.Take,
                    UnavailableCSR = this.UnavailableCSR
                };
            }
        }
    }
}