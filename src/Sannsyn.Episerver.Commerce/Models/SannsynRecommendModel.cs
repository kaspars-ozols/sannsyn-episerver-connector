using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sannsyn.Episerver.Commerce.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Ignoring messageId, accountId, requestMessageId, debugInfo
    /// </remarks>
    public class SannsynRecommendModel
    {
        [JsonProperty(PropertyName = "result")]
        public List<string> Result { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

    }
}
