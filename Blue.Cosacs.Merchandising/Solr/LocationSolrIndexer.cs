namespace Blue.Cosacs.Merchandising.Solr
{
    using System.Collections.Generic;
    using System.Linq;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Solr;

    public interface ILocationSolrIndexer
    {
        void Index(int[] ids = null);
    }

    public class LocationSolrIndexer : ILocationSolrIndexer
    {
        private const string SolrType = "location";

        public void Index(int[] ids = null)
        {
            using (var scope = Context.Read())
            {
                var locations = scope.Context.Location.AsNoTracking().AsQueryable();
                
                if (ids != null)
                {
                    locations = locations.Where(l => ids.Contains(l.Id));
                }
                
                var documents = locations.ToList().Select(l =>
                {
                    var contacts = JsonConvertHelper.DeserializeObjectOrDefault<List<StringKeyValue>>(l.Contacts);

                    return new
                    {
                        Id = string.Format("{0}:{1}", SolrType, l.Id),
                        LocationId = l.Id,
                        LocationNumber = l.LocationId,
                        SalesLocationId = l.SalesId,
                        Type = SolrType,
                        l.Name,
                        l.StoreType,
                        l.Fascia,
                        Address = string.Join("\n", l.AddressLine1, l.AddressLine2, l.City, l.PostCode),
                        Warehouse = l.Warehouse ? "Yes" : "No",
                        Active = l.Active ? "Yes" : "No",
                        VirtualWarehouse = l.VirtualWarehouse ? "Yes" : "No",
                        ContactType = contacts.Select(c => c.Key),
                        ContactValue = contacts.Select(c => c.Value)
                    };
                });

                new WebClient().Update(documents);
            }
        }
    }
}
