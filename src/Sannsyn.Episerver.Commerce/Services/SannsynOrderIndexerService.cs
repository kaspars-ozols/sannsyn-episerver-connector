using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Backend;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Models;
using Sannsyn.Episerver.Commerce.Extensions;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(ISannsynOrderIndexerService))]
    public class SannsynOrderIndexerService : ISannsynOrderIndexerService
    {
        private readonly ILogger _log;
        private readonly SannsynConfiguration _configuration;
        private readonly BackendService _backendService;
        private bool _logSendData = false; 

        public SannsynOrderIndexerService(global::EPiServer.Logging.ILogger log, SannsynConfiguration configuration, BackendService backendService)
        {
            _log = log;
            _configuration = configuration;
            _backendService = backendService;
            _logSendData = _configuration.LogSendData;
        }

        public void AddLineItemsToSannsyn(OrderGroup orderGroup)
        {
            LineItemCollection lineItems = orderGroup.OrderForms.First().LineItems;

            List<SannsynUpdateEntityModel> sannsynObjects = new List<SannsynUpdateEntityModel>();
            foreach (LineItem lineItem in lineItems)
            {
                sannsynObjects.Add(CreateSannsynObject(lineItem, orderGroup.CustomerId,orderGroup.Modified));
            }

            SannsynUpdateModel sannsynModel = new SannsynUpdateModel();
            sannsynModel.Service = _configuration.Service;
            sannsynModel.Updates = sannsynObjects;
            SendToSannsyn(sannsynModel);
        }

        private HttpResponseMessage SendToSannsyn(SannsynUpdateModel sannsynModel)
        {
            var jsonData = JsonConvert.SerializeObject(sannsynModel);
            // This method does not require the service name
            Uri serviceUrl = _backendService.GetServiceMethodUri("update", null, null);
            HttpClient client = _backendService.GetConfiguredClient();

            HttpContent content = new StringContent(jsonData);
            HttpResponseMessage response =  client.PutAsync(serviceUrl, content).Result;
            _log.Debug("Sent to Sannsyn. Result: {0}", response.StatusCode);
            if(response.IsSuccessStatusCode == false)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                _log.Warning("Send to Sannsyn failed: {0}", resultContent);
            }

            if(_logSendData)
            {
                _log.Debug("Data: {0}", jsonData);
            }

            return response;
        }

        private SannsynUpdateEntityModel CreateSannsynObject(LineItem lineItem, Guid customerId, DateTime modified)
        {
            SannsynUpdateEntityModel model = new SannsynUpdateEntityModel();
            model.Customer = customerId.ToString();
            model.Tags = new List<string> {"buy"};
            model.EntityIDs = new List<string> {lineItem.Code};
            model.Time = lineItem.Modified.ToJavaTimeStamp();
            model.Boost = (float) 0.0;
            model.Number = (int) lineItem.Quantity;
            return model;
        }

    }
}
