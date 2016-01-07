using System.Collections.Generic;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface IRecommendationService
    {
        IEnumerable<string> GetRecommendationsForCustomer(string customerId, int maxCount = 10);

        IEnumerable<string> GetRecommendationsForProduct(string productCode, int maxCount = 10);

        IEnumerable<string> GetRecommendationsForCart(IEnumerable<string> productCodes, int maxCount = 10);

        IEnumerable<string> GetRecommendationsForCustomerByCategory(string customerId, string category,
            int maxCount = 10);
    }
}
