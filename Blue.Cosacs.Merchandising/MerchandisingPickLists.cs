using System.Linq;

namespace Blue.Cosacs.Merchandising
{
    public class LocationsListProvider : LinqPickListProvider
    {
        public LocationsListProvider() : 
                base(
            "MerchandisingLocations",
                "All Merchandising Location's",
                () =>
                    {
                        using (var scope = Context.Read())
                        {
                            return scope.Context.Location.Select(l => new { l.Name }).ToList().Select(l => new PickListRow(l.Name, l.Name)).ToList();
                        }
                    })
        {
        }
    }
}