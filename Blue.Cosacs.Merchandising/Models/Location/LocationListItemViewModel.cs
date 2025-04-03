using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// If using the list control it requires the objects to have the fields k and v
    /// </summary>
    public class LocationListItemViewModel
    {
        [JsonProperty(PropertyName = "k")]
        public int LocationId { get; set; }

        [JsonProperty(PropertyName = "v")]
        public string LocationName { get; set; }
    }
}
