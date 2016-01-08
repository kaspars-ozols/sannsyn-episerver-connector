using System;
using System.Net.Http;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Backend;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Models;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(ISannsynUpdateService))]
    public class SannsynUpdateService : ISannsynUpdateService
    {

        private readonly BackendService _backendService;
        private readonly ILogger _log;
        private readonly SannsynConfiguration _configuration;
        private bool _logSendData = false;

        public SannsynUpdateService(global::EPiServer.Logging.ILogger log, SannsynConfiguration configuration, BackendService backendService)
        {
            _log = log;
            _configuration = configuration;
            _backendService = backendService;
            _logSendData = _configuration.LogSendData;
        }
        /// <summary>
        /// Sending data to Sannsyn
        /// </summary>
        /// <param name="sannsynModel">Model with entry codes</param>
        /// <returns>A HttpResponseMessage with result from the put request to Sannsyn</returns>
        public HttpResponseMessage SendToSannsyn(SannsynUpdateModel sannsynModel)
        {
            var jsonData = JsonConvert.SerializeObject(sannsynModel);
            // This method does not require the service name
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Update, null, null);
            HttpClient client = _backendService.GetConfiguredClient();

            HttpContent content = new StringContent(jsonData);
            HttpResponseMessage response = client.PutAsync(serviceUrl, content).Result;
            _log.Debug("Sent to Sannsyn. Result: {0}", response.StatusCode);
            if (response.IsSuccessStatusCode == false)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                _log.Warning("Send to Sannsyn failed: {0}", resultContent);
            }

            if (_logSendData)
            {
                _log.Debug("Data: {0}", jsonData);
            }

            return response;
        }
    }
}
