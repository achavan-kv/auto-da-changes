using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.CosacsWeb.Models
{
    public class RepossessedConditions : RepossessedCondition
    {

        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("data")]
        public RepossessedCondition[] Data { get; set; }
    }
}
