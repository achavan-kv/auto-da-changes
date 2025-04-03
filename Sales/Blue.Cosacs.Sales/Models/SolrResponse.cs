using Newtonsoft.Json;
using System.Collections.Generic;

namespace Blue.Cosacs.Sales.Models
{
    public class SolrResponse<T> : Blue.Solr.Response
    {
        [JsonProperty("docs")]
        public new IEnumerable<T> Docs { get; set; }
    }
}