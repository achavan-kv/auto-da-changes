namespace Blue.Cosacs.Merchandising.Solr
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Solr;

    public interface IPromotionSolrIndexer
    {
        void Index(int[] ids = null);
    }

    public class PromotionSolrIndexer : IPromotionSolrIndexer
    {
        private const string SolrType = "MerchandisePromotion";

        public void Index(int[] ids = null)
        {
            using (var scope = Context.Read())
            {
                var promotions = scope.Context.ForcePromotionIndexView.AsNoTracking().AsQueryable();
                
                if (ids != null)
                {
                    promotions = promotions.Where(p => ids.Contains(p.PromotionId));
                }
                
                var documents = promotions.ToList().GroupBy(p => p.PromotionId).Select(g =>
                {
                    var skus = g.Select(p => p.Sku);
                    var descriptions = g.Select(p => p.LongDescription);
                    var hierarchy = DeserializeHierarchy(g);

                    return CreateDocument(g.First(), skus, descriptions, hierarchy);
                });

                new WebClient().Update(documents);
            }
        }

        private dynamic CreateDocument(ForcePromotionIndexView promotion, IEnumerable<string> skus, IEnumerable<string> descriptions, Dictionary<int, IEnumerable<string>> hierarchy)
        {
            dynamic document = new ExpandoObject();

            document.Id = string.Format("{0}:{1}", SolrType, promotion.PromotionId);
            document.PromotionId = promotion.PromotionId;
            document.Name = promotion.Name;
            document.StartDate = promotion.StartDate.ToSolrDate();
            document.EndDate = promotion.EndDate.ToSolrDate();
            document.PromotionType = promotion.PromotionType;
            document.LocationName = promotion.LocationName == null && string.IsNullOrWhiteSpace(promotion.Fascia) ? "All Locations" : promotion.LocationName;
            document.Fascia = promotion.Fascia;
            document.Type = SolrType;
            document.ProductSkus = skus;
            document.ProductDescriptions = descriptions;
            
            AddHierarchyLevels(document, hierarchy);
            return document;
        }

        private static Dictionary<int, IEnumerable<string>> DeserializeHierarchy(IGrouping<int, ForcePromotionIndexView> grouping)
        {
            return grouping.Where(p => string.IsNullOrEmpty(p.Hierarchy))
                .Select(p => JsonConvertHelper.DeserializeObjectOrDefault<List<Level>>(p.Hierarchy))
                .SelectMany(l => l)
                .GroupBy(l => l.Id)
                .ToDictionary(g => g.Key, g => g.Select(l => l.Name));
        }

        private void AddHierarchyLevels(dynamic document, Dictionary<int, IEnumerable<string>> hierarchy)
        {
            IDictionary<string, object> dic = document;
            hierarchy.ToList().ForEach(l => dic.Add("MerchandisingLevel_" + l.Key, l.Value.Any() ? l.Value.ToList() : new List<string> { "Unassigned" }));
        }
    }
}
