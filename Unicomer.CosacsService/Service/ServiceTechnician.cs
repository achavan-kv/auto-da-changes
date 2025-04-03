using Blue.Events;
using STL.DAL;
using System;
using System.Data;


namespace Unicomer.CosacsService
{
    public class ServiceTechnician
    {
        private readonly IEventStore audit;
        //public ServiceTechnician() { }

        public ServiceTechnician(IEventStore audit)
        {
            this.audit = audit;
        }
        
        
        public DataTable GetAllocatedTechnicianJobs(int technicianId)
        {
            DServiceRequest dr = new DServiceRequest();
            return dr.GetAllocatedTechnicianJobs(technicianId);

        }

        public void OverrideBookingByRequestId(int techID,int jobId)
        {
            DServiceRequest dr = new DServiceRequest();
            dr.OverrideBookingByRequestId(techID,jobId);
        }

        public void JobOverrideAudit(int oldRequestId, int overrideByUserId, DateTime overideDate, int newRequestId)
        {
            DServiceRequest dr = new DServiceRequest();
            dr.JobOverrideAudit(oldRequestId,overrideByUserId,overideDate,newRequestId);
            audit.LogAsync(new { ServiceRequest = Convert.ToString(newRequestId), OldServiceRequest = Convert.ToString(oldRequestId)}, "Allocation", "Service");
        }

        public DataTable GetUserAuthForOverride(int id, string pwd)
        {
            DServiceRequest dr = new DServiceRequest();
            return dr.GetUserAuthForOverride(id,pwd);
        }
    }
}
