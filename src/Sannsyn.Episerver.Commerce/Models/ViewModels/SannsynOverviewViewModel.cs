using System.Collections.Generic;

namespace Sannsyn.Episerver.Commerce.Models.ViewModels
{
    public class SannsynOverviewViewModel
    {
        public ServiceStatusViewModel ServiceStatus { get; set; }
        public IEnumerable<string> CurrentUserRecommendations { get; set; }
        public string CurrentUserId { get; set; }
    }
}
