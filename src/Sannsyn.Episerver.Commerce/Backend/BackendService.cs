using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Configuration;

namespace Sannsyn.Episerver.Commerce.Backend
{
    public class BackendService
    {
        private const string API_VERSION = "/recapi/1.0/";
        private readonly SannsynConfiguration _configuration;

        public BackendService(SannsynConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpClient GetConfiguredClient()
        {
            var username = _configuration.Username;
            var password = _configuration.Password;
            HttpClient client = new HttpClient();
            byte[] cred = Encoding.UTF8.GetBytes(username + ":" + password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));

            return client;
        }

        public Uri GetServiceMethodUri(string serviceMethod)
        {
            return GetServiceMethodUri(serviceMethod, null);
        }

        public Uri GetServiceMethodUri(string serviceMethod, string parameters)
        {
            var url = API_VERSION + serviceMethod + "/" + _configuration.Service;
            if(string.IsNullOrEmpty(parameters) == false)
            {
                url = url + "/" + parameters;
            }

            Uri serviceUrl = new Uri(_configuration.ServiceUrl, url);
            return serviceUrl;
        }

        public T GetResult<T>(Uri serviceUrl, HttpClient client)
        {
            HttpResponseMessage response = client.GetAsync(serviceUrl).Result;
            var data = response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<T>(data.Result);
            return model;

        }

    }
}
