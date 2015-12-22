using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sannsyn.Episerver.Commerce.Admin
{
    public class SannsynOverviewController : Controller
    {
        private readonly SannsynConfiguration _configuration;

        public SannsynOverviewController(SannsynConfiguration configuration)
        {
            _configuration = configuration;
        }

        [MenuItem("/global/sannsyn/Overview", Text = "Overview")]
        public ActionResult Index()
        {
            Uri serviceUrl = new Uri(_configuration.ServiceUrl, "/recapi/1.0/servicestatus/"+_configuration.Service);
            var username = _configuration.Username;
            var password = _configuration.Password;
            HttpClient client = new HttpClient();
            client.BaseAddress = serviceUrl;
            byte[] cred = UTF8Encoding.UTF8.GetBytes(username + ":" + password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));

            HttpResponseMessage response = client.GetAsync(serviceUrl).Result;
            var data = response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<StatusObject>(data.Result);

            return View(string.Format("{0}{1}/Views/SannsynOverview/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"),model);

        }

       
    }
    public class StatusObject
    {
        public string messageId { get; set; }
        public string accountId { get; set; }
        public string requestMessageId { get; set; }
        public string status { get; set; }
        public string serviceDescription { get; set; }
        public string configurationName { get; set; }
        public string numEntities { get; set; }
        public string runningSince { get; set; }
        public string statusDescription { get; set; }


    }
}
