using Blue.Solr;
using Newtonsoft.Json;

namespace Blue.Cosacs.Sales.Models
{
    public class SolrResult<T> : Solr.Result
    {
        [JsonProperty("response")]
        public new SolrResponse<T> Response { get; set; }
    }
}
