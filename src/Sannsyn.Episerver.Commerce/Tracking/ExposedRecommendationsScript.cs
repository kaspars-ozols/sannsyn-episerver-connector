using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Tracking
{
    public class ExposedRecommendationsScript
    {
        private readonly HttpContextBase _context;
        private readonly SannsynConfiguration _config;
        private readonly ICustomerService _customerService;

        public ExposedRecommendationsScript(HttpContextBase context, SannsynConfiguration config, ICustomerService customerService)
        {
            _context = context;
            _config = config;
            _customerService = customerService;
        }

        public virtual string GetExposedRecommendationsScript()
        {
            return GetExposedRecommendationsScriptImpl(_context, _config, _customerService);
        }

        protected string GetExposedRecommendationsScriptImpl(HttpContextBase context, SannsynConfiguration config, ICustomerService customerService)
        {
            StringBuilder sb = new StringBuilder();
            ExposureTracker tracker = new ExposureTracker();
            var recommendations = tracker.GetTrackedExposures(context);
            if (recommendations.Any() == false)
                return null;

            sb.AppendLine("var sannsynService = '" + config.Service + "';");
            sb.AppendLine("var sannsynTrackedId = '" + customerService.GetCurrentUserId() + "';");

            string script = @"
    function trackRecommendationExposure(recommendations, recommenderName) {
      var ouronscroll = function(){
        ssas_track_visibility(recommendations, recommenderName, sannsynService, sannsynTrackedId);
      };
      var oldonscroll = window.onscroll; // don't overwrite old onScroll method, (if it exists)
      if (oldonscroll != null) {
        window.onscroll = function() {
          oldonscroll.apply(window);
          ouronscroll.apply(window)
        };
      } else {
        window.onscroll = function() {
          ouronscroll.apply(window);
        };
      }

      // finally, do an initial check for visibility, since onscroll could be never happening:
      ouronscroll.apply(window);
    }

    function trackRecClick(recommendation)
    {
        ssas_click(sannsynService, sannsynTrackedId, recommendation);
    }
";

            sb.Append(script);

            foreach (var group in recommendations.GroupBy(r => r.RecommenderName))
            {
                List<string> productCodes = new List<string>();
                foreach (var value in group)
                {
                    productCodes.Add(value.ProductCode);
                }
                string recName = "rec_" + group.Key;
                string exposures = string.Format("var {0} = ['{1}'];", recName, string.Join("', '", productCodes));
                string call = string.Format("trackRecommendationExposure({0}, '{1}');", recName, group.Key);
                sb.AppendLine(exposures);
                sb.AppendLine(call);
            }

            return sb.ToString();
        }

    }
}