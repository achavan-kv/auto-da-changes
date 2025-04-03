using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blue.Cosacs.Merchandising.Models;
using Blue.Cosacs.Merchandising.Solr;
using Blue.Data;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface INonWarrantableItemsRepository
    {
        void Update(int productId, bool warrantable);
        IPagedSearchResults<BasicProductDetails> Get(IPagedSearch filter);
        bool ValidateWarrantableProduct(string SKU);
    }

    public class NonWarrantableItemsRepository : INonWarrantableItemsRepository
    {
        //CR - Product warranty association need to populate based on warrantable status of product.
        private readonly IStockSolrIndexer stockSolrIndexer;
        public NonWarrantableItemsRepository(IStockSolrIndexer stockSolrIndexer)
        {
            this.stockSolrIndexer = stockSolrIndexer;
        }
        public void Update(int productId, bool warrantable)
        {
            using (var scope = Context.Write())
            {
                var product = scope.Context.Product
                              .Where(p => p.Id == productId).ToList();

                product[0].Warrantable = warrantable;
                scope.Context.SaveChanges();
                scope.Complete();

                //CR - Product warranty association need to populate based on warrantable status of product.
                // When warratable status changed for product execute solr indexing for respective product.
                var ids = new[] { Convert.ToInt32(productId) };
                this.stockSolrIndexer.Index(ids);
            }
        }

        public IPagedSearchResults<BasicProductDetails> Get(IPagedSearch filter)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.Product
                                         .Where(p => p.Warrantable == false)
                                         .OrderBy(p => p.Id)
                                         .Select(p => new BasicProductDetails
                                         {
                                             ProductId = p.Id,
                                             Sku = p.SKU,
                                             LongDescription = p.LongDescription,
                                             Warrantable = p.Warrantable
                                         });
                return filter.Page(query);
            }
        }

        //CR - Product warranty association need to populate based on warrantable status of product.
        public bool ValidateWarrantableProduct(string SKU)
        {
            bool isWarrantableProduct = true;
            using (var scope = Context.Read())
            {
                isWarrantableProduct = scope.Context.ValidateWarrantableProduct(SKU).Result;                
            }
            return isWarrantableProduct;
        }
    }
}
