using Blue.Cosacs.Sales.Models;
using Blue.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Sales.Repositories
{
    public class SalesLookupRepository : BaseRepository, ISalesLookupRepository
    {
        public SalesLookupRepository(IClock clock, IHttpClient httpClient)
            : base(clock, httpClient)
        {
        }

        public IEnumerable<BranchDetails> GetBranches(int userId)
        {
            var url = "/Cosacs/Merchandising/Locations/Get";

            var data = GetRemoteData<JSendResult<BranchDetails>>(url, userId);
            return data.Data as List<BranchDetails>;
        }

        public BranchDetails GetBranches(int userId, short branchNo)
        {
            var data = GetBranches(userId);

            return !data.Any() ? null : data.SingleOrDefault(u => u.BranchNumber == branchNo);
        }

        //public HiLo GetHiLoGeneratedId(string module, int userId)
        //{
        //    var url = string.Format("/cosacs/HiLo/Allocate?sequence={0}", module);

        //    var data = PostRemoteData<HiLo>(url, userId);
        //    return data as HiLo;
        //}

        public string GetBranchName(short id, IEnumerable<BranchDetails> branchList)
        {
            if (branchList == null)
            {
                return string.Empty;
            }

            var branch = branchList.FirstOrDefault(r => r.SalesId == id.ToString());

            return branch == null ? string.Empty : branch.Name;
        }

        public string GetBranchName(int userId, short id)
        {
            var branchList = GetBranches(userId);

            return GetBranchName(id, branchList);
        }

        public string GetBranchFascia(int userId, int locationId)
        {
            var branch = GetBranches(userId).FirstOrDefault(r => r.Id == locationId);

            return branch == null ? string.Empty : branch.Fascia;
        }
    }
}
