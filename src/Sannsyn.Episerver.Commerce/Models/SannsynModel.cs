using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sannsyn.Episerver.Commerce.Models
{
    public class SannsynModel
    {
        [JsonProperty(PropertyName = "service")]
        public string Service { get; set; }
        [JsonProperty(PropertyName = "updates")]
        public List<SannsynObjectModel> Updates { get; set; }
    }
}
