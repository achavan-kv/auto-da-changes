namespace Blue.Cosacs.Merchandising.Solr
{
    using System.Collections.Generic;
    using System.Linq;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Solr;

    public interface IPurchaseOrderSolrIndexer
    {
        void Index(int[] ids = null);
    }

    public class PurchaseOrderSolrIndexer : IPurchaseOrderSolrIndexer
    {
        private readonly IStockSolrIndexer stockSolrIndexer;
        private readonly Settings settings;
        
        public PurchaseOrderSolrIndexer(IStockSolrIndexer stockSolrIndexer, Settings settings)
        {
            this.stockSolrIndexer = stockSolrIndexer;
            this.settings = settings;
        }
        
        public void Index(int[] ids = null)
        {
            using (var scope = Context.Read())
            {
                var purchaseOrders = scope.Context.ForcePurchaseOrderIndexView.AsNoTracking().AsQueryable();
                
                if (ids != null)
                {
                    purchaseOrders = purchaseOrders.Where(o => ids.Contains(o.Id));
                }

                var purchaseOrderDeliveryDates = scope.Context.PurchaseOrderProduct
                    .AsNoTracking()
                    .Where(p => purchaseOrders.Select(x => x.Id).Contains(p.PurchaseOrderId))
                    .Select(p => new { p.PurchaseOrderId, p.EstimatedDeliveryDate })
                    .ToLookup(p=> p.PurchaseOrderId);

                const string SolrType = "MerchandisePurchaseOrder";

                var documents = purchaseOrders.ToList()
                    .Select(o =>
                    {
                        var referenceNumbers = JsonConvertHelper.DeserializeObjectOrDefault<List<StringKeyValue>>(o.ReferenceNumbers);
                        var dates = purchaseOrderDeliveryDates[o.Id];

                        return new
                        {
                            Id = string.Format("{0}:{1}", SolrType, o.Id),
                            PurchaseOrderId = o.Id,
                            Type = SolrType,
                            o.Vendor,
                            o.VendorId,
                            o.ReceivingLocation,
                            o.Status,
                            o.CorporatePoNumber,
                            o.OriginSystem,
                            o.CreatedBy,
                            o.TotalCost,
                            Summary = FormatSummary(o, referenceNumbers),
                            ReferenceType = referenceNumbers.Select(c => c.Key),
                            ReferenceValue = referenceNumbers.Select(c => c.Value),
                            ExpiryDate = dates
                                .OrderByDescending(d => d.EstimatedDeliveryDate)
                                .FirstOrDefault()
                                .EstimatedDeliveryDate.Value.Date.ToSolrDate(),
                            ExpectedDeliveryDate = dates
                                .Select(p => p.EstimatedDeliveryDate.Value.Date.ToSolrDate())
                                .ToList()
                        };
                    })
                    .ToList();

                new WebClient().Update(documents);
                                
                if (ids != null)
                {
                    IndexProducts(purchaseOrders.Select(p => p.Id).ToArray());
                }
            }
        }

        private void IndexProducts(IEnumerable<int> purchaseOrderProductIds)
        {
            using (var scope = Context.Read())
            {
                var productIds =
                    scope.Context.PurchaseOrderProduct.Where(o => purchaseOrderProductIds.Contains(o.PurchaseOrderId))
                        .Select(p => p.ProductId)
                        .Distinct()
                        .ToArray();
                this.stockSolrIndexer.Index(productIds);
            }
        }

        private static string FormatSummary(ForcePurchaseOrderIndexView o, List<StringKeyValue> referenceNumbers)
        {
            return string.Format(
                "#{0}, Vendor: {1}, Total: {2}, Ref: {3}",
                o.Id,
                o.Vendor,
                o.TotalCost,
                string.Join(", ", referenceNumbers.Select(r => r.Value)));
        }
    }
}
