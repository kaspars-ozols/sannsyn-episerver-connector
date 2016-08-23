using System.Collections.Generic;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface IRecommendations
    {
        IEnumerable<string> ProductCodes { get; }
        string RecommenderName { get; }
    }
}