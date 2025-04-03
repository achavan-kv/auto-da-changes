using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.CosacsWeb.Models
{
    public class RepossessedCondition
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("skuSuffix")]
        public string SKUSuffix { get; set; }
    }
}
