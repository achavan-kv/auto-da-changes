namespace Blue.Cosacs.Merchandising.Repositories
{
    using System.Collections.Generic;

    using Blue.Cosacs.Merchandising.Infrastructure;

    public interface IServiceRequestRepository
    {
        List<dynamic> GetServiceRequestDetails(string q);
    }

    public class ServiceRequestRepository : IServiceRequestRepository
    {
        public List<dynamic> GetServiceRequestDetails(string q)
        {
            const string Filter = "Type:ServiceRequest AND (SRType:\"Service Request Internal\" OR SRType:\"Service Request External\")";

            return new Lunr().Search(q, Filter, "RequestId,CustomerId,Customer");
        }
    }
}