using System.Collections.Generic;

namespace Sannsyn.Episerver.Commerce.Models.ViewModels
{
    public class SannsynOverviewViewModel
    {
        public SannsynServiceStatusModel ServiceStatus { get; set; }
        public IEnumerable<string> CurrentUserRecommendations { get; set; }
        public string CurrentUserId { get; set; }
    }
}
