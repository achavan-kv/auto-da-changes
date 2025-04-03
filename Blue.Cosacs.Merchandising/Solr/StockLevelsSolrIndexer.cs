namespace Blue.Cosacs.Merchandising.Solr
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Helpers;
    using System.Collections.Generic;
     using System.Linq;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using System.Configuration;
    using Blue.Cosacs.Merchandising.Models;
	using System.IO;

    public interface IStockLevelsSolrIndexer
    {
        void Index(IEnumerable<int> productIds = null);
		void IndexLevel(bool updateonly);
    }

    public class StockLevelsSolrIndexer : IStockLevelsSolrIndexer
    {
        private const string SolrType = "MerchandiseStock";
		
		 private async Task<T> ExecuteAsync<T>(Func<T> toExecute) where T : class
        {
            return await Task<T>.Run<T>(toExecute).ConfigureAwait(false);
        }
        public void IndexLevel(bool updateonly)
        {
            var client = new Blue.Solr.WebClient();
            ReIndexingLevels objReindexingLevels = new ReIndexingLevels();
            List<string> defaultValue = null;
            try
            {
                var reIndexingLevels = objReindexingLevels.GetReIndexedLevels(updateonly);
                List<ReIndexedStockLevelModel> finalResult = (from DataRow dr in reIndexingLevels.AsEnumerable()
                                                                select new ReIndexedStockLevelModel
                                                                {
                                                                    Id = string.Format("{0}:{1}-{2}", SolrType, Convert.ToInt32(dr.Field<int?>("ProductId")), Convert.ToInt32(dr.Field<int?>("LocationId"))),
                                                                    ProductId = Convert.ToInt32(dr.Field<int?>("ProductId")),
                                                                    Sku = dr.Field<string>("SKU"),
                                                                    LongDescription = dr.Field<string>("LongDescription"),
                                                                    ProductType = dr.Field<string>("ProductType"),
                                                                    PreviousProductType = dr.Field<string>("PreviousProductType"),
                                                                    ProductStatus = dr.Field<string>("ProductStatus"),
                                                                    RepossessedCondition = dr.IsNull("RepossessedCondition") ? string.Empty : dr.Field<string>("RepossessedCondition"),
                                                                    LocationNumber = dr.Field<string>("LocationNumber"),
                                                                    LocationId = Convert.ToInt32(dr.Field<int?>("LocationId")),
                                                                    Type = SolrType,
                                                                    LocationName = dr.Field<string>("LocationName"),
                                                                    Fascia = dr.Field<string>("Fascia"),
                                                                    CreatedOn = dr.IsNull("CreatedDate") ? Convert.ToDateTime("01/01/1990").ToSolrDate() : dr.Field<DateTime>("CreatedDate").ToSolrDate(),
                                                                    AverageWeightedCost = Convert.ToDecimal(dr.Field<decimal?>("AverageWeightedCost")),
                                                                    StockAvailable = Convert.ToInt32(dr.Field<int?>("StockAvailable")),
                                                                    StockOnHand = Convert.ToInt32(dr.Field<int?>("StockOnHand")),
                                                                    StockOnOrder = Convert.ToInt32(dr.Field<int?>("StockOnOrder")),
                                                                    StockAllocated = Convert.ToInt32(dr.Field<int?>("StockAllocated")),
                                                                    Warehouse = dr.Field<string>("Warehouse"),
                                                                    Tags = dr.IsNull("Tags") ? defaultValue : JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(dr.Field<string>("Tags")).ToList(),
                                                                    StoreTypes = dr.IsNull("StoreTypes") ? defaultValue : JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(dr.Field<string>("StoreTypes")).ToList(),
                                                                    Vendors = dr.IsNull("Vendors") ? defaultValue : JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(dr.Field<string>("Vendors")).ToList()
                                                                }).ToList();
           
                    client.Update(finalResult);
            }
            catch (Exception ex)
            {
               throw new Exception("See inner exception details.", ex);
            }
                    
        }

        public void Index(IEnumerable<int> productIds = null)
        {
            using (var scope = Context.Read())
            {
                var client = new Blue.Solr.WebClient();

                var productQuery = scope.Context.ForceMerchandiseStockLevelsView.AsNoTracking().AsQueryable();

                if (productIds != null)
                {
                    productQuery = productQuery.Where(p => productIds.Contains(p.ProductId));
                }
                var products = productQuery.ToList();

                var totals = products.Union(SummariseAll(products))
                    .Union(SummariseAllWarehouses(products))
                    .Union(SummariseFascias(products))
                    .Union(SummariseFasciaWarehouses(products))
                    .ToList();

                client.Update(CreateDocuments(totals));
            }
        }

        public List<object> CreateDocuments(List<ForceMerchandiseStockLevelsView> products)
        {
            var documents = products.Select(p =>
            {
                var vendors = JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(p.Suppliers).ToList();
                vendors.Add(p.PrimaryVendor);

                return new
                {
                    Id = string.Format("{0}:{1}-{2}", SolrType, p.ProductId, p.LocationId),
                    p.ProductId,
                    Sku = p.SKU,
                    p.LongDescription,
                    p.ProductType,
                    ProductStatus = p.Status,
                    RepossessedCondition = p.Condition,
                    p.LocationNumber,
                    p.LocationId,
                    Type = SolrType,
                    LocationName = p.Location,
                    p.Fascia,
                    CreatedOn = p.CreatedDate.ToSolrDate(),
                    p.AverageWeightedCost,
                    p.StockAvailable,
                    p.StockOnHand,
                    p.StockOnOrder,
                    p.StockAllocated,
                    Warehouse = p.Warehouse ? "Yes" : "No",
                    Tags = JsonConvertHelper.DeserializeObject<string[]>(p.Tags),
                    StoreTypes = JsonConvertHelper.DeserializeObject<string[]>(p.StoreTypes),
                    Vendors = vendors.ToArray(),
                };
            }).Select(p => (object)p).ToList();

            return documents;
        }

        private List<ForceMerchandiseStockLevelsView> SummariseAll(List<ForceMerchandiseStockLevelsView> products)
        {
            return products
                .Where(p => !p.VirtualWarehouse.Value)
                .GroupBy(p => new { p.ProductId })
                .Select(group =>
                 {
                     var product = Mapper.Map<ForceMerchandiseStockLevelsView>(group.First()); //clone product
                     product.Location = "All Locations";
                     product.LocationId = 0;
                     product.LocationNumber = "ALL";
                     product.Warehouse = group.Any(p => p.Warehouse);
                     product.StockAllocated = group.Sum(p => p.StockAllocated);
                     product.StockOnHand = group.Sum(p => p.StockOnHand);
                     product.StockOnOrder = group.Sum(p => p.StockOnOrder);
                     product.StockAvailable = group.Sum(p => p.StockAvailable);
                     return product;
                 })
                .ToList();
        }

        private List<ForceMerchandiseStockLevelsView> SummariseAllWarehouses(List<ForceMerchandiseStockLevelsView> products)
        {
            return products.Where(p => p.Warehouse).GroupBy(p => new { p.ProductId }).Select(group =>
                 {
                     var product = Mapper.Map<ForceMerchandiseStockLevelsView>(group.First()); //clone product
                    product.Location = "All Warehouses";
                     product.LocationId = 0;
                     product.LocationNumber = "ALL";
                     product.StockAllocated = group.Sum(p => p.StockAllocated);
                     product.StockOnHand = group.Sum(p => p.StockOnHand);
                     product.StockOnOrder = group.Sum(p => p.StockOnOrder);
                     product.StockAvailable = group.Sum(p => p.StockAvailable);
                     return product;
                 }).ToList();
        }

        private List<ForceMerchandiseStockLevelsView> SummariseFascias(List<ForceMerchandiseStockLevelsView> products)
        {
            return products.GroupBy(p => new { p.ProductId, p.Fascia }).Select(group =>
                {
                    var product = Mapper.Map<ForceMerchandiseStockLevelsView>(group.First()); //clone product
                    product.Location = product.Fascia + " Locations";
                    product.LocationId = 0;
                    product.Warehouse = group.Any(p => p.Warehouse);
                    product.LocationNumber = product.Fascia;
                    product.StockAllocated = group.Sum(p => p.StockAllocated);
                    product.StockOnHand = group.Sum(p => p.StockOnHand);
                    product.StockOnOrder = group.Sum(p => p.StockOnOrder);
                    product.StockAvailable = group.Sum(p => p.StockAvailable);
                    return product;
                }).ToList();
        }
        private List<ForceMerchandiseStockLevelsView> SummariseFasciaWarehouses(List<ForceMerchandiseStockLevelsView> products)
        {
            return products.Where(p => p.Warehouse).GroupBy(p => new { p.ProductId, p.Fascia }).Select(group =>
                {
                    var product = Mapper.Map<ForceMerchandiseStockLevelsView>(group.First()); //clone product
                    product.Location = product.Fascia + " Warehouses";
                    product.LocationId = 0;
                    product.LocationNumber = product.Fascia;
                    product.StockAllocated = group.Sum(p => p.StockAllocated);
                    product.StockOnHand = group.Sum(p => p.StockOnHand);
                    product.StockOnOrder = group.Sum(p => p.StockOnOrder);
                    product.StockAvailable = group.Sum(p => p.StockAvailable);
                    return product;
                }).ToList();
        }
    }
}