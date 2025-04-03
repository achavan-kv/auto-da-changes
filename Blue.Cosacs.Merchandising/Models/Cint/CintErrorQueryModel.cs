namespace Blue.Cosacs.Merchandising.Models
{
    using Newtonsoft.Json;
    using System;

    public class CintErrorQueryModel
    {
        [JsonProperty("runNo")]
        public int? RunNo { get; set; }

        [JsonProperty("primaryReference")]
        public string PrimaryReference { get; set; }

        [JsonProperty("bulk")]
        public bool Bulk { get; set; }

        [JsonProperty("fromDate")]
        public DateTime? FromDate { get; set; }

        [JsonProperty("toDate")]
        public DateTime? ToDate { get; set; }

        [JsonProperty("filter")]
        public bool Filter { get; set; }
    }
}