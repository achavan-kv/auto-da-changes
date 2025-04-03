namespace Blue.Cosacs.Merchandising.Solr
{
    using System.Collections.Generic;

    public interface IStockSolrIndexer
    {
        void Index(IEnumerable<int> productIds = null);
		void ReIndex(bool updateonly = false);
    }

    public class StockSolrIndexer : IStockSolrIndexer
    {
        private readonly IStockSummarySolrIndexer summary;
        private readonly IStockLevelsSolrIndexer levels;

        public StockSolrIndexer(IStockSummarySolrIndexer summary, IStockLevelsSolrIndexer levels)
        {
            this.summary = summary;
            this.levels = levels;
        }

        public void Index(IEnumerable<int> productIds = null)
        {
            var summaryTask = summary.Index(productIds);
            levels.Index(productIds);

            summaryTask.Wait();
        }
		public void ReIndex(bool updateonly = false)
        {
            var summaryTask = summary.ReIndex(updateonly);
            levels.IndexLevel(updateonly);
            summaryTask.Wait();
        }
    }
}