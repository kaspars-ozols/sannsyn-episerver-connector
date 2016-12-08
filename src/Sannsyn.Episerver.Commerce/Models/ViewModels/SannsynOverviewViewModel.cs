using System.Collections.Generic;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Models.ViewModels
{
    public class SannsynOverviewViewModel
    {
        public SannsynServiceStatusModel ServiceStatus { get; set; }
        public IRecommendations CurrentUserRecommendations { get; set; }
        public string CurrentUserId { get; set; }
    }
}
