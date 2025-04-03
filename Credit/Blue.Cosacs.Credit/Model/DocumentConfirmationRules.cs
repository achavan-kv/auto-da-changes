using Newtonsoft.Json;
using System.Collections.Generic;

namespace Blue.Cosacs.Credit.Model
{
    public class DocumentConfirmationRules
    {
        [JsonProperty("customerType")]
        public string CustomerType { get; set; }

        [JsonProperty("fields")]
        public List<DocumentConfirmationField> Fields { get; set; }
    }

    public class DocumentConfirmationField
    {
        [JsonProperty("fieldID")]
        public string FieldID { get; set; }

        [JsonProperty("fieldDescription")]
        public string FieldDescription { get; set; }

        [JsonProperty("fieldType")]
        public string FieldType { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("validation")]
        public bool Validation { get; set; }

        [JsonProperty("upload")]
        public bool Upload { get; set; }
    }
}
