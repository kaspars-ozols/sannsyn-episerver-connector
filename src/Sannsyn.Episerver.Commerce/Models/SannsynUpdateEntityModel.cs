using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sannsyn.Episerver.Commerce.Models
{
    public class SannsynUpdateEntityModel
    {
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        [JsonProperty(PropertyName = "entityIDs")]
        public List<string> EntityIDs { get; set; }

        [JsonProperty(PropertyName = "time")]
        public double Time { get; set; }

        [JsonProperty(PropertyName = "boost")]
        public float Boost  { get; set; }

        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }
    }
}
