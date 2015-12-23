using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface IRecommendationService
    {
        IEnumerable<string> GetRecommendationsForCustomer(string customerId, int maxCount = 10);

        IEnumerable<string> GetRecommendationsForProduct(string productCode, int maxCount = 10);

        IEnumerable<string> GetRecommendationsForCart(IEnumerable<string> productCodes, int maxCount = 10);
    }
}
