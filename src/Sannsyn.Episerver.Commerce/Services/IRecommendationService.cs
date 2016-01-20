using System.Collections.Generic;
using Sannsyn.Episerver.Commerce.Models;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface IRecommendationService
    {
        IEnumerable<string> GetRecommendationsForCustomer(string customerId, int maxCount = 10);

        IEnumerable<string> GetRecommendationsForProduct(string productCode, int maxCount = 10);

        IEnumerable<string> GetRecommendationsForCart(IEnumerable<string> productCodes, int maxCount = 10);

        IEnumerable<string> GetRecommendationsForCustomerByCategory(string customerId, List<string> categories,
            int maxCount = 10);

        Dictionary<string, double> GetScoreForItems(int maxCount = 10000);
    }
}
