using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannsyn.Episerver.Commerce.Models.ViewModels
{
    public class SannsynOverviewViewModel
    {
        public ServiceStatusViewModel ServiceStatus { get; set; }
        public IEnumerable<string> CurrentUserRecommendations { get; set; }
        public string CurrentUserId { get; set; }
    }
}
