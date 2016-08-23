using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sannsyn.Episerver.Commerce.Tracking
{
    public class ExposureTracker
    {
        private const string ContextName = "Sannsyn:RecommendationTracker";

        public virtual ICollection<ITrackedRecommendation> GetTrackedExposures(HttpContextBase context)
        {
            ICollection<ITrackedRecommendation> exposures = context.Items[ContextName] as ICollection<ITrackedRecommendation>;
            if (exposures == null)
            {
                exposures = new List<ITrackedRecommendation>();
                context.Items[ContextName] = exposures;
            }
            return exposures;
        }

        public virtual void AddExposure(HttpContextBase context, ITrackedRecommendation recommendation)
        {
            ICollection<ITrackedRecommendation> exposures = GetTrackedExposures(context);

            if (exposures.Where<ITrackedRecommendation>((Func<ITrackedRecommendation, bool>)(i => i.IsEqual(recommendation))).Any())
                return;
            exposures.Add(recommendation);
        }
    }
}
