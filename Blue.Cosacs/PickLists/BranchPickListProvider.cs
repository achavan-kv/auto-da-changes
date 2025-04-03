using System.Linq;

namespace Blue.Cosacs.PickLists
{
    public class BranchPickListProvider : LinqPickListProvider
    {
        public BranchPickListProvider() : base(
            "BRANCH",
             "All Physical Branches",
             () =>
                {
                    using (var ctx = Context.Create())
                    {
                        return (from b in ctx.Branch
                                select new { b.branchno, b.branchname }).ToList()
                            .Select(
                                b => new PickListRow(b.branchno.ToString(), b.branchno.ToString() + ' ' + b.branchname))
                            .ToArray();
                    }
                })
        {
       }
    }
}
