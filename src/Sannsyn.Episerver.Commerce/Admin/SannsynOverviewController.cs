using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Web.Mvc;
using Mediachase.Commerce.Security;
using Sannsyn.Episerver.Commerce.Backend;
using Sannsyn.Episerver.Commerce.Models.ViewModels;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Admin
{
    public class SannsynOverviewController : Controller
    {
        private readonly BackendService _backendService;
        private readonly RecommendationService _recommendationService;
        private readonly ICurrentCustomerService _currentCustomerService;

        public SannsynOverviewController(BackendService backendService, RecommendationService recommendationService, ICurrentCustomerService currentCustomerService)
        {
            _backendService = backendService;
            _recommendationService = recommendationService;
            _currentCustomerService = currentCustomerService;
        }

        [MenuItem("/global/sannsyn/Overview", Text = "Overview", SortIndex = 10)]
        public ActionResult Index()
        {
            Uri serviceUrl = _backendService.GetServiceMethodUri("servicestatus");
            HttpClient client = _backendService.GetConfiguredClient();

            HttpResponseMessage response = client.GetAsync(serviceUrl).Result;
            var data = response.Content.ReadAsStringAsync();
            var statusmodel = JsonConvert.DeserializeObject<ServiceStatusViewModel>(data.Result);

            var model = new SannsynOverviewViewModel();
            model.ServiceStatus = statusmodel;

            var userId = _currentCustomerService.GetCurrentUserId();
            model.CurrentUserRecommendations = _recommendationService.GetRecommendationsForCustomer(userId);

            return View(string.Format("{0}{1}/Views/SannsynOverview/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"), model);

        }

       
    }
}
