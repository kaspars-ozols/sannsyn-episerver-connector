using System.Collections.Generic;

namespace Sannsyn.Episerver.Commerce.Services
{
    public class Recommendations : IRecommendations
    {
        public Recommendations(string recommenderName, IEnumerable<string> productCodes)
        {
            RecommenderName = recommenderName;
            ProductCodes = productCodes;
        }

        public string RecommenderName { get; }
        public IEnumerable<string> ProductCodes { get; }
    }
}