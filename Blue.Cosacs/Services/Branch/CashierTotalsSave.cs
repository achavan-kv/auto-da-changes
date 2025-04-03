using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.Branch;

namespace Blue.Cosacs.Services.Branch
{
    partial class Server
    {
        public CashierTotalsSaveResponse Call(CashierTotalsSaveRequest request)
        {
            new CashierTotalRepository().CashierTotalsSave(request);
            EventStore.Instance.Log(request, "CashierTotal", EventCategory.CashierTotals, new { BranchNo = request.branch });
            return new CashierTotalsSaveResponse { result = "OK" };
        }
    }
}
