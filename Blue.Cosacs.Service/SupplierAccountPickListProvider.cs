using System.Linq;

namespace Blue.Cosacs.Service
{
    public class SupplierAccountPickListProvider : LinqPickListProvider
    {
        public SupplierAccountPickListProvider()
            : base("ServiceSupplierAccount", "Accounts for each supplier for use in Service module.", () =>
            {
                using (var scope = Context.Read())
                {
                    var suppliers = (from s in scope.Context.ServiceSupplier
                            select new { s.Supplier }).ToList();
                    suppliers.Add(new { Supplier = "Other" });

                    return suppliers.Select(d => new PickListRow(d.Supplier, d.Supplier)).OrderBy(s => s.k).ToArray();
                }
            }) { }
    }
}
