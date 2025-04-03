namespace Blue.Cosacs.Merchandising.Solr
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Solr;

    public interface IVendorSolrIndexer
    {
        void Index(int[] ids = null);
    }

    public class VendorSolrIndexer : IVendorSolrIndexer
    {
        private const string SolrType = "MerchandiseVendor";

        public void Index(int[] ids = null)
        {
            using (var scope = Context.Read())
            {
                var statuses = scope.Context.SupplierStatus.AsNoTracking().ToList();
                var vendors = scope.Context.Supplier.AsNoTracking().AsQueryable();

                if (ids != null)
                {
                    vendors = vendors.Where(v => ids.Contains(v.Id));
                }

                var documents = vendors.ToList().Select(v =>
                {
                    var contacts = JsonConvertHelper.DeserializeObjectOrDefault<List<StringKeyValue>>(v.Contacts);

                    return new
                    {
                        Id = string.Format("{0}:{1}", SolrType, v.Id),
                        VendorId = v.Id,
                        v.Code,
                        v.Name,
                        Address = HttpUtility.HtmlEncode(string.Format("{0}\n{1}\n{2}\n{3}\n{4}", v.AddressLine1, v.AddressLine2, v.City, v.PostCode, v.Country)),
                        v.OrderEmail,
                        v.PaymentTerms,
                        Status = statuses.Single(st => st.Id == v.Status).Name,
                        VendorType = v.Type,
                        Type = SolrType,
                        ContactType = contacts.Select(c => c.Key),
                        ContactValue = contacts.Select(c => c.Value),
                        Tags = JsonConvertHelper.DeserializeObject<string[]>(v.Tags),
                    };
                });

                new WebClient().Update(documents);
            }
        }
    }
}
