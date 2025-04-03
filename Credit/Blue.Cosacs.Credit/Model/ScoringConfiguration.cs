using Newtonsoft.Json;

namespace Blue.Cosacs.Credit.Model
{
    public class ScoringConfiguration
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("lookupField")]
        public string LookupField { get; set; }

        internal ScoringConfiguration()
        {
        }

        internal ScoringConfiguration(Credit.ScoreCardConfiguration sc)
        {
            this.Name = sc.RuleName;
            this.Type = sc.Type;
            this.LookupField = sc.LookupField;
        }
    }
}
