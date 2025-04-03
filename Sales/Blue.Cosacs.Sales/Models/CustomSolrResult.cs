using Blue.Solr;
using Newtonsoft.Json;

namespace Blue.Cosacs.Sales.Models
{
    public class CustomSolrResult
    {
        [JsonProperty("facet_counts")]
        public ResponseFacets Facets { get; set; }
        [JsonProperty("response")]
        public CustomSolrResponse Response { get; set; }
        [JsonProperty("responseHeader")]
        public ResponseHeader ResponseHeader { get; set; }
    }
}
