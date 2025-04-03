using Newtonsoft.Json;

namespace Blue.Cosacs.Credit.Model
{
    public class ScoreCard
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("decline")]
        public int Decline { get; set; }
        [JsonProperty("refer")]
        public int Refer { get; set; }
        [JsonProperty("scoringRules")]
        public RuleSet[] ScoringRules { get; set; }
        [JsonProperty("referRules")]
        public RuleSet[] ReferRules { get; set; }
        [JsonProperty("declineRules")]
        public RuleSet[] DeclineRules { get; set; }

        public class RuleSet
        {
            [JsonProperty("result")]
            public string Result { get; set; }
            [JsonProperty("rules")]
            public Rule Rules { get; set; }

            public class Rule
            {
                [JsonProperty("class")]
                public string Class { get; set; }
                [JsonProperty("expression")]
                public Rule[] Expression { get; set; }
                [JsonProperty("operator")]
                public string Operator { get; set; }
                [JsonProperty("variable")]
                public string Variable { get; set; }
            }
        }
    }
}
