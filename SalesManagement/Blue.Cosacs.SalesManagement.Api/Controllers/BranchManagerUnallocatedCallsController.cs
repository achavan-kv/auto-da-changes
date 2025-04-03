using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/BranchManagerUnallocatedCalls")]
    public class BranchManagerUnallocatedCallsController : System.Web.Http.ApiController
    {
        private ISalesManagementRepository salesManagementRepository;
        private IClock clock;
        private IEventStore audit;

        public BranchManagerUnallocatedCallsController(ISalesManagementRepository salesManagementRepository, IClock clock, IEventStore audit)
        {
            this.salesManagementRepository = salesManagementRepository;
            this.clock = clock;
            this.audit = audit;
        }

        [Permission(SalesManagementPermissionEnum.BranchManagerUnallocatedCalls)]
        public HttpResponseMessage Put(CallSearchFilter searchFilter)
        {
            searchFilter.Branch = this.GetUser().Branch;
            var unallocatedCalls = salesManagementRepository.GetBranchManagerUnallocatedCalls(searchFilter);

            return Request.CreateResponse(unallocatedCalls);
        }
    }
}