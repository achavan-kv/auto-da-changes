using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Blue.Cosacs.NonStocks.Solr
{
    public class SolrUtils
    {
        public const string NonStocksType = "NonStock";

        public string GetNonStocks(int branch, string q = "", int start = 0, int rows = 25)
        {
            return GetSolrQueryResults(q, start, rows, NonStocksType, true);
        }

        private string GetSolrQueryResults(string q = "", int start = 0, int rows = 0, string type = NonStocksType, bool showEmpty = true)
        {
            var facets = new[] { "NonStockType", "Active", "Division", "Department", "Class" };

            var jsObject = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(q);
            var query = jsObject["query"].ToString();

            if (query != null && query.Length > 0)
            {
                var jsonQuery = @"{ ""query"" : """ + query + @""" }";
                var result =
                    new Blue.Solr.Query()
                    .SelectJsonWithJsonQuery(
                    jsonQuery,
                    "Type:" + type,
                    facetFields: facets,
                    start: start,
                    rows: rows,
                    showEmpty: false);

                return result;
            }
            else
            {
                return new Blue.Solr.Query()
                    .SelectJsonWithJsonQuery(
                        q,
                        "Type:" + type,
                        facetFields: facets,
                        showEmpty: showEmpty,
                        start: start,
                        rows: rows
                    );
            }
        }

        public void DeleteAllSolrNonStockDocuments()
        {
            try
            {
                new Blue.Solr.WebClient().DeleteByType(NonStocksType);
            }
            catch (Exception) { }
        }
    }
}
