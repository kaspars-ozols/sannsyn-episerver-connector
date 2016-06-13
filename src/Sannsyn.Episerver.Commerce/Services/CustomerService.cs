using System;
using System.Net.Http;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Security;
using Sannsyn.Episerver.Commerce.Backend;
using Sannsyn.Episerver.Commerce.Configuration;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(ICustomerService))]
    public class CustomerService : ICustomerService
    {
        private readonly BackendService _backendService;
        private readonly ILogger _log;
        private readonly SannsynConfiguration _configuration;


        public CustomerService(global::EPiServer.Logging.ILogger log, SannsynConfiguration configuration, BackendService backendService)
        {
            _log = log;
            _configuration = configuration;
            _backendService = backendService;
        }

        /// <summary>
        /// Returns Commerce contact id for logged on users, and profile guid for anonymous users
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserId()
        {
            var userId = EPiServer.Security.PrincipalInfo.CurrentPrincipal.GetContactId().ToString();
            return userId;
        }


        public void MigrateUser(string oldId, string newId)
        {
            string parameters = "customer/" + newId + "/" + oldId;
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Merge, parameters);
            HttpClient client = _backendService.GetConfiguredClient();
            var responseMessage = _backendService.GetResult(serviceUrl, client);
        }

    }
}
