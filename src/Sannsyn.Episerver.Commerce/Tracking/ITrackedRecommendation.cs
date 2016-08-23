namespace Sannsyn.Episerver.Commerce.Tracking
{
    public interface ITrackedRecommendation
    {
        string RecommenderName { get; set; }
        string ProductCode { get; set; }
        bool IsEqual(ITrackedRecommendation trackedRecommendation);
    }
}