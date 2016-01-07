using System;
using System.Net.Http;
using System.Text;
using EPiServer.Logging;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Configuration;

namespace Sannsyn.Episerver.Commerce.Backend
{
    public class BackendService
    {
        private const string API_VERSION = "/recapi/1.0/";
        private readonly ILogger _log;
        private readonly SannsynConfiguration _configuration;

        public BackendService(ILogger log, SannsynConfiguration configuration)
        {
            _log = log;
            _configuration = configuration;
        }

        public HttpClient GetConfiguredClient()
        {
            var username = _configuration.Username;
            var password = _configuration.Password;
            HttpClient client = new HttpClient();
            byte[] cred = Encoding.UTF8.GetBytes(username + ":" + password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(cred));

            return client;
        }

        public Uri GetServiceMethodUri(string serviceMethod)
        {
            return GetServiceMethodUri(serviceMethod, null);
        }

        public Uri GetServiceMethodUri(string serviceMethod, string parameters)
        {
            return GetServiceMethodUri(serviceMethod, parameters, _configuration.Service);
        }

        public Uri GetServiceMethodUri(string serviceMethod, string parameters, string service)
        {
            var url = API_VERSION + serviceMethod;

            if (string.IsNullOrEmpty(service) == false)
            {
                url = url + "/" + service;
            }

            if (string.IsNullOrEmpty(parameters) == false)
            {
                url = url + "/" + parameters;
            }

            Uri serviceUrl = new Uri(_configuration.ServiceUrl, url);
            return serviceUrl;
        }

        public T GetResult<T>(Uri serviceUrl, HttpClient client)
        {

            return GetResult<T>(serviceUrl, client, null);
        }


        public T GetResult<T>(Uri serviceUrl, HttpClient client, HttpContent content)
        {
            if (_configuration.LogSendData && _log.IsDebugEnabled())
            {
                _log.Debug("Calling: {0}", serviceUrl.ToString());
            }
            HttpResponseMessage response;
            if (content == null)
            {
                response = GetResult(serviceUrl, client);
            }
            else
            {
                response = GetResult(serviceUrl, client, content);
            }

            var data = response.Content.ReadAsStringAsync();
            var result = data.Result;

            if (_configuration.LogSendData && _log.IsDebugEnabled())
            {
                _log.Debug("Data recieved: {0}", result);
            }

            // Before we can convert to json, we need to make sure we have valid data
            // This will throw if we get an error back from the server
            response.EnsureSuccessStatusCode();

            var model = JsonConvert.DeserializeObject<T>(result);
            return model;
        }


        public HttpResponseMessage GetResult(Uri serviceUrl, HttpClient client)
        {
            HttpResponseMessage response = client.GetAsync(serviceUrl).Result;

            if (_configuration.LogSendData && _log.IsDebugEnabled())
            {
                _log.Debug("Sent to Sannsyn. Result: {0} - {1}", response.StatusCode, response.ReasonPhrase);
            }

            return response;
        }

        public HttpResponseMessage GetResult(Uri serviceUrl, HttpClient client, HttpContent content)
        {
            HttpResponseMessage response = client.PutAsync(serviceUrl, content).Result; ;

            if (_configuration.LogSendData && _log.IsDebugEnabled())
            {
                _log.Debug("Sent to Sannsyn. Result: {0} - {1}", response.StatusCode, response.ReasonPhrase);
            }

            return response;
        }
    }
}
