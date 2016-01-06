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
    [MenuItem("/global/sannsyn/Overview", Text = "Overview", SortIndex = 10)]
    public class SannsynOverviewController : Controller
    {
        private readonly RecommendationService _recommendationService;
        private readonly ICurrentCustomerService _currentCustomerService;
        private readonly ISannsynAdminService _sannsynAdminService;

        public SannsynOverviewController(RecommendationService recommendationService, ICurrentCustomerService currentCustomerService,
            ISannsynAdminService sannsynAdminService)
        {
            _recommendationService = recommendationService;
            _currentCustomerService = currentCustomerService;
            _sannsynAdminService = sannsynAdminService;
        }

        public ActionResult Index()
        {
            var statusmodel = _sannsynAdminService.GetServiceStatus();

            var model = new SannsynOverviewViewModel();
            model.ServiceStatus = statusmodel;

            var userId = _currentCustomerService.GetCurrentUserId();
            model.CurrentUserId = userId;
            model.CurrentUserRecommendations = _recommendationService.GetRecommendationsForCustomer(userId);

            return View(string.Format("{0}{1}/Views/SannsynOverview/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"), model);

        }

       
    }
}
