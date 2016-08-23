using System;

namespace Sannsyn.Episerver.Commerce.Tracking
{
    public class TrackedRecommendation : ITrackedRecommendation
    {
        public string RecommenderName { get; set; }
        public string ProductCode { get; set; }

        public bool IsEqual(ITrackedRecommendation trackedRecommendation)
        {
            if (string.Compare(RecommenderName, trackedRecommendation.RecommenderName,
                StringComparison.InvariantCultureIgnoreCase) == 0 &&
                string.Compare(ProductCode, trackedRecommendation.ProductCode,
                    StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return true;
            }
            return false;
        }
    }
}