using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Service;
using Blue.Cosacs.Service.Utils;
using System.Collections;

namespace Blue.Cosacs.Service.Solr
{
    public static class SolrIndex
    {
        // Do not increase above 5000 without testing SolrIndex indexing on a big database.
        // Specifically test Re-Indexing for Service on the page: Administration -> Re-Indexing.
        // Failed for Trinidad (one of the bigger databases) when indexingReadSize = 50000.
        private const int indexingReadSize = 5000; // CAUTION!!! Don't change without reading above :(

        public static IEnumerable<Request> IndexRequest(int[] requestIds = null)
        {
            var srCount = 0L; // service request count

            using (var scope = Context.Read())
            {
                if (requestIds != null)
                {
                    srCount = scope.Context.RequestIndexView
                        .Where(e => requestIds.Contains(e.Id))
                        .Select(p => p.Id)
                        .Max();
                }
                else
                {
                    srCount = scope.Context.RequestIndexView
                        .Select(p => p.Id)
                        .Max();
                }
            }

            var serviceRequests = new List<Request>();

            var rowsToSkip = 0;
            var correctNonSequential = 0;
            for (int i = 0; i * indexingReadSize < srCount; i++)
            {
                rowsToSkip = correctNonSequential != 0 ? correctNonSequential : i * indexingReadSize;
                correctNonSequential = 0;

                using (var scope = Context.Read())
                {
                    var tmpRequestIndexView = new List<RequestIndexView>();
                    if (requestIds != null)
                    {
                        tmpRequestIndexView = (from rv in scope.Context.RequestIndexView
                                                   .Where(e => requestIds.Contains(e.Id) && e.Id > rowsToSkip)
                                                   .Take(indexingReadSize)
                                               select rv).ToList();
                    }
                    else
                    {
                        tmpRequestIndexView = (from rv in scope.Context.RequestIndexView
                                                   .Where(e => e.Id > rowsToSkip)
                                                   .Take(indexingReadSize)
                                               select rv).ToList();
                    }

                    serviceRequests = tmpRequestIndexView
                        .Select(e => ProjectRequest(e))
                        .ToList();

                    if (serviceRequests.Count == 0)
                        break;

                    var inMemoryRequestIds = serviceRequests
                        .Select(e => e.RequestId)
                        .Distinct()
                        .ToList();

                    var contacts = (from c in scope.Context.RequestContact
                                    where inMemoryRequestIds.Contains(c.RequestId)
                                    select c);

                    var faultTags = (from ft in scope.Context.FaultTag
                                     where inMemoryRequestIds.Contains(ft.RequestId)
                                     select new { ft.RequestId, ft.Tag });

                    var techs = (from t in scope.Context.TechnicianBooking
                                 join u in scope.Context.AdminUserView on t.UserId equals u.Id
                                 where inMemoryRequestIds.Contains(t.RequestId)
                                 select new { t, u });

                    contacts = contacts.Where(c => inMemoryRequestIds.Contains(c.RequestId));
                    faultTags = faultTags.Where(ft => inMemoryRequestIds.Contains(ft.RequestId));
                    techs = techs.Where(t => inMemoryRequestIds.Contains(t.t.RequestId));

                    var contactsLookup = contacts.ToLookup(c => c.RequestId);
                    var faultTagsLookp = faultTags.ToLookup(ft => ft.RequestId);
                    var techsLookup = techs.ToLookup(t => t.t.RequestId);

                    foreach (var r in serviceRequests)
                    {
                        var tId = new List<int>();
                        r.ContactValue = contactsLookup[r.RequestId].Select(c => c.Value).ToArray();
                        r.ContactType = contactsLookup[r.RequestId].Select(c => c.Type).ToArray();
                        if (faultTagsLookp.Contains(r.RequestId))
                            r.FaultTags = faultTagsLookp[r.RequestId].Select(c => c.Tag).ToArray();
                        var tech = techsLookup[r.RequestId].Where(t => !t.t.Reject).FirstOrDefault();
                        if (tech != null)
                        {
                            r.TechName = tech.u.FullName;
                            r.TechAllocated = tech.t.Date.ToSolrDate();
                            tId.Add(tech.u.Id);
                        }
                        r.TechId = tId.ToArray();
                    }

                }

                // because the LoadId is not sequential
                correctNonSequential = serviceRequests
                    .Max(e => e.RequestId);

                new Blue.Solr.WebClient().Update(serviceRequests);

                if (requestIds != null)
                {
                    // I'm assuming here that when indexing service requests by id, the number of id's
                    // will be in the order of tenths (maybe a couple of hundred max), and this should
                    // ALLWAYS be smaller than indexingReadSize (which will be in the order of thousands).
                    // This is why all indexing results are being returned in this first for iteration.
                    return serviceRequests;
                }
            }

            return null;
        }

        private static Request ProjectRequest(RequestIndexView r)
        {
            return new Request
            {
                ServiceHomeBranch = r.Branch,
                HomeBranchName = r.BranchNameLong,
                Reference = string.IsNullOrEmpty(r.InvoiceNumber) ? r.Account : r.InvoiceNumber,
                CreatedBy = r.CreatedBy,
                CreatedById = r.CreatedById,
                CreatedOn = r.CreatedOn.ToSolrDate(),
                CustomerId = r.CustomerId,
                Customer = string.Format("{0} {1} {2}", r.CustomerTitle, r.CustomerFirstName, r.CustomerLastName),
                SRType = r.Type,
                ItemNo = r.ItemNumber,
                ItemDescription = r.Item,
                SerialNo = r.ItemSerialNumber,
                Address = string.Format("{0}, {1}, {2}, {3}", r.CustomerAddressLine1, r.CustomerAddressLine2, r.CustomerAddressLine3, r.CustomerPostcode),
                RequestId = r.Id,
                ServiceStatus = r.State,
                LastUpdatedOn = Convert.ToDateTime(r.LastUpdatedOn).ToSolrDate(),
                LastUpdatedBy = Convert.ToInt32(r.LastUpdatedUser),
                LastUpdatedByName = r.LastUpdatedUserName,
                Printed = r.Printed.HasValue && r.Printed.Value ? "Yes" : "No",
                RepairLimitWarning = r.RepairLimitWarning ? "Yes" : "No",
                ResolutionDate = Convert.ToDateTime(r.ResolutionDate).ToSolrDate()
            };
        }
    }

    public class Request
    {
        public string Id { get { return String.Format("serviceRequest:{0}", RequestId); } }
        public string Type { get { return "serviceRequest"; } }
        public string CustomerId { get; set; }
        public int RequestId { get; set; }
        public string Reference { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Int32 CreatedById { get; set; }
        public string Customer { get; set; }
        public short ServiceHomeBranch { get; set; }
        public string HomeBranchName { get; set; }
        private string srType;

        public string SRType
        {
            get { return ServiceType.FromString(srType.Trim()).Name; }
            set { srType = value; }
        }
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string ServiceStatus { get; set; }
        public string SerialNo { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string LastUpdatedOn { get; set; }
        public string Address { get; set; }
        public string[] ContactType { get; set; }
        public string[] ContactValue { get; set; }
        public string Printed { get; set; }
        public string[] FaultTags { get; set; }
        public string RepairLimitWarning { get; set; }
        public int[] TechId { get; set; }
        public string TechName { get; set; }
        public string TechAllocated { get; set; }
        public string ResolutionDate { get; set; }
    }
}


