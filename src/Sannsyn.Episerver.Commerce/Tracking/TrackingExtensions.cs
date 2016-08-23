using System;
using System.Web;
using System.Web.Mvc;
using EPiServer.ServiceLocation;
using Sannsyn.Episerver.Commerce.ClientScriptRegistration;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Tracking
{
    public static class TrackingExtensions
    {
        public static void AddRecommendationExposure(this HttpContext context, ITrackedRecommendation recommendation)
        {
            new HttpContextWrapper(context).AddRecommendationExposure(recommendation);
        }

        public static void AddRecommendationExposure(this HttpContextBase context, ITrackedRecommendation recommendation) 
        {
            ExposureTracker tracker = new ExposureTracker();
            tracker.AddExposure(context, recommendation);
        }

        public static IHtmlString TrackSannsynProductExposures(this HtmlHelper helper)
        {
            SannsynConfiguration config = ServiceLocator.Current.GetInstance<SannsynConfiguration>();
            if (config.ModuleEnabled)
            {

                ICustomerService customerService = ServiceLocator.Current.GetInstance<ICustomerService>();
                ExposedRecommendationsScript scriptHandler =
                    new ExposedRecommendationsScript(helper.ViewContext.HttpContext, config, customerService);
                string recScript = scriptHandler.GetExposedRecommendationsScript();

                if (string.IsNullOrEmpty(recScript) == false)
                {
                    string htmlString = String.Format("<script type=\"text/javascript\">\r\n\r{0}</script>\r", recScript);
                    return new HtmlString(htmlString);
                }
            }
            return new HtmlString(string.Empty);
        }
    }
}