using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse
{
    public class TruckPickListProvider : LinqPickListProvider
    {
        public TruckPickListProvider()
        : base("Truck", "Warehouse Trucks", () =>
        {
            using (var scope = Context.Read())
                return (from t in scope.Context.Truck
                        orderby t.Name
                        select new { t.Id, t.Name }).ToList()
                        .Select(t => new PickListRow(t.Id.ToString(), t.Name)).ToArray();
        }) { }
    }
}
