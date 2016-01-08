using System;
using System.Net.Http;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Sannsyn.Episerver.Commerce.Backend;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Models;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(ISannsynAdminService))]
    public class SannsynAdminService : ISannsynAdminService
    {

        private readonly BackendService _backendService;
        private readonly ILogger _log;
        private readonly SannsynConfiguration _configuration;
        

        public SannsynAdminService(global::EPiServer.Logging.ILogger log, SannsynConfiguration configuration, BackendService backendService)
        {
            _log = log;
            _configuration = configuration;
            _backendService = backendService;
        }

        public SannsynServiceStartStopModel CreateService()
        {
            // This method does not require the service name
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Start, _configuration.Configuration, _configuration.Service);
            HttpClient client = _backendService.GetConfiguredClient();

            var model = _backendService.GetResult<SannsynServiceStartStopModel>(serviceUrl, client);

            _log.Debug("Created service with configuration: {0}", _configuration.Configuration);

            return model;
        }

        public SannsynServiceStatusModel GetServiceStatus()
        {
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Servicestatus, null, _configuration.Service);
            HttpClient client = _backendService.GetConfiguredClient();
            var model = _backendService.GetResult<SannsynServiceStatusModel>(serviceUrl, client);

            return model;
        }

        public bool StopService()
        {
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Stop, null, _configuration.Service);
            HttpClient client = _backendService.GetConfiguredClient();
            var responseMessage = _backendService.GetResult(serviceUrl, client);

            return responseMessage.IsSuccessStatusCode;
        }
    }
}
