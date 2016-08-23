using System.Collections.Generic;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface ITrackedRecommendationService
    {
        IRecommendations GetRecommendationsForCustomer(string customerId, int maxCount = 10);

        IRecommendations GetRecommendationsForProduct(string productCode, int maxCount = 10);

        IRecommendations GetRecommendationsForCart(string customerId, IEnumerable<string> productCodes, int maxCount = 10);

        IRecommendations GetRecommendationsForCustomerByCategory(string customerId, List<string> categories, int maxCount = 10);

    }
}