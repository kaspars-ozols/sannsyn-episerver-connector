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
using Sannsyn.Episerver.Commerce.Models;
using Sannsyn.Episerver.Commerce.Extensions;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(ISannsynOrderIndexerService))]
    public class SannsynOrderIndexerService : ISannsynOrderIndexerService
    {
        private readonly ILogger _log;
        private bool _logSendData = false; 
        readonly ReferenceConverter referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();
        readonly IContentLoader contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

        public SannsynOrderIndexerService(global::EPiServer.Logging.ILogger log)
        {
            _log = log;
            var sendDataFlag = ConfigurationManager.AppSettings["Sannsyn:LogSendData"];
            bool.TryParse(sendDataFlag, out _logSendData);

        }

        public async void AddLineItemsToSannsyn(OrderGroup orderGroup)
        {
            LineItemCollection lineItems = orderGroup.OrderForms.First().LineItems;

            List<SannsynObjectModel> sannsynObjects = new List<SannsynObjectModel>();
            foreach (LineItem lineItem in lineItems)
            {
                sannsynObjects.Add(CreateSannsynObject(lineItem, orderGroup.CustomerId,orderGroup.Modified));
            }

            SannsynModel sannsynModel = new SannsynModel();
            sannsynModel.Service = "epicphoto";
            sannsynModel.Updates = sannsynObjects;
            await SendToSannsyn(sannsynModel);


        }

        private async Task<HttpResponseMessage> SendToSannsyn(SannsynModel sannsynModel)
        {
            var serviceUrl = "http://episerver.sannsyn.com/recapi/1.0/update";
            var username = "episerver";
            var password = "aDmnGspinache";

            var jsonData = JsonConvert.SerializeObject(sannsynModel);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serviceUrl);
            byte[] cred = UTF8Encoding.UTF8.GetBytes(username+":"+password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpContent content = new StringContent(jsonData);
            HttpResponseMessage response =  await client.PutAsync(serviceUrl, content);
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

        private SannsynObjectModel CreateSannsynObject(LineItem lineItem, Guid customerId, DateTime modified)
        {
            SannsynObjectModel model = new SannsynObjectModel();
            model.Customer = customerId.ToString();
            model.Tags = GetCatalogNodesForVariation(lineItem.Code);
            model.Tags.Add("purchased");
            model.EntityIDs = new List<string> {lineItem.Code};
            model.Time = ConvertToUnixTimeStamp(lineItem.Modified);
            model.Boost = (float) 0.0;
            model.Number = (int) lineItem.Quantity;
            return model;
        }

        private long ConvertToUnixTimeStamp(DateTime modified)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = modified.ToUniversalTime() - origin;
            return (long) Math.Floor(diff.TotalSeconds);
        }

        private List<string> GetCatalogNodesForVariation(string code)
        {           
            ContentReference cf = referenceConverter.GetContentLink(code);
            CatalogContentBase entry = contentLoader.Get<CatalogContentBase>(cf);
            if (entry != null)
            {
                return entry.GetParentCategoryCodes(entry.Language.Name);
            }
            return new List<string>();
        }
    }
}
