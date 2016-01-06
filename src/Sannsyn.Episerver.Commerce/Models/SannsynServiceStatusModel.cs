using System;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Extensions;

namespace Sannsyn.Episerver.Commerce.Models
{
    public class SannsynServiceStatusModel
    {
        public string messageId { get; set; }
        public string accountId { get; set; }
        public string requestMessageId { get; set; }
        public string status { get; set; }
        public string serviceDescription { get; set; }
        public string configurationName { get; set; }
        public string numEntities { get; set; }
        public double runningSince { get; set; }
        [JsonIgnore]
        public DateTime ServiceRunningSince {
            get { return DateTimeExtensions.JavaTimeStampToDateTime(runningSince); } }
        public string statusDescription { get; set; }
        public string serviceName { get; set; }
        


    }
}