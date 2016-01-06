using System;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Extensions;

namespace Sannsyn.Episerver.Commerce.Models
{
    public class SannsynServiceStartStopModel
    {
        public string status { get; set; }

        [JsonProperty(PropertyName = "servicedescription")]
        public string serviceDescription { get; set; }

        [JsonProperty(PropertyName = "configurationname")]
        public string configurationName { get; set; }

        [JsonProperty(PropertyName = "noofentities")]
        public string numEntities { get; set; }

        [JsonProperty(PropertyName = "servicename")]
        public string serviceName { get; set; }
        


    }
}