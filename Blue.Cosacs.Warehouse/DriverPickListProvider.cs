using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse
{
    public class DriverPickListProvider : LinqPickListProvider
    {
        public DriverPickListProvider()
        : base("Driver", "Warehouse Drivers", () =>
        {
            using (var scope = Context.Read())
                return (from d in scope.Context.Driver
                        orderby d.Name
                        select new { d.Id, d.Name }).ToList()
                        .Select(d => new PickListRow(d.Id.ToString(), d.Name)).ToArray();
        }) { }
    }
}
