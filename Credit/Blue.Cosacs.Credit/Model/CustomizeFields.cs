using Newtonsoft.Json;
using System.Collections.Generic;
namespace Blue.Cosacs.Credit.Model
{
    public class CustomizeFields
    {
        [JsonProperty("screenLabel")]
        public string ScreenLabel { get; set; }

        [JsonProperty("screenId")]
        public string ScreenId { get; set; }

        [JsonProperty("sections")]
        public List<Section> Sections { get; set; }
    }

    public class Field
    {
        [JsonProperty("screenId")]
        public string ScreenId { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("validation")]
        public string Validation { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("readonly")]
        public bool Readonly { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("values")]
        public string Values { get; set; }

        [JsonProperty("lookup")]
        public string Lookup { get; set; }

        [JsonProperty("fieldLookup")]
        public string FieldLookup { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("fieldOrder")]
        public int FieldOrder { get; set; }

        [JsonProperty("requiredValues")]
        public string RequiredValues { get; set; }
    }

    public class Section
    {
        [JsonProperty("sectionName")]
        public string SectionName { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
    }
}
