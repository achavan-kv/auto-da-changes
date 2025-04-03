using Newtonsoft.Json;
using System.Collections.Generic;

namespace Blue.Cosacs.Sales.Models
{
    public class CustomSolrResponse : Blue.Solr.Response
    {
        [JsonProperty("docs")]
        public new IEnumerable<IndexedProducts> Docs { get; set; }
    }
}
