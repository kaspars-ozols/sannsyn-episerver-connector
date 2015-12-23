using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Backend;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Models;
using Sannsyn.Episerver.Commerce.Models.ViewModels;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(IRecommendationService))]
    public class RecommendationService : IRecommendationService
    {
        private readonly BackendService _backendService;

        public RecommendationService(BackendService backendService )
        {
            _backendService = backendService;
        }

        /// <summary>
        /// Get recommended products for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public IEnumerable<string> GetRecommendationsForCustomer(string customerId, int maxCount = 10)
        {
            // Uses aggregate MostPopularClickItems
            Uri serviceUrl = _backendService.GetServiceMethodUri("recommend", "MostPopularClickItems/" + customerId);
            HttpClient client = _backendService.GetConfiguredClient();
            var model = _backendService.GetResult<SannsynRecommendModel>(serviceUrl, client);

            return model.Result;

        }

        /// <summary>
        /// Get recommended products for a given product
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public IEnumerable<string> GetRecommendationsForProduct(string productCode, int maxCount = 10)
        {
            Uri serviceUrl = _backendService.GetServiceMethodUri("recommend", "MostPopularClickItems/" + productCode);
            HttpClient client = _backendService.GetConfiguredClient();
            var model = _backendService.GetResult<SannsynRecommendModel>(serviceUrl, client);

            return model.Result;
        }

        /// <summary>
        /// Recommended products based on what other customers have bought
        /// </summary>
        /// <param name="productCodes"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public IEnumerable<string> GetRecommendationsForCart(IEnumerable<string> productCodes, int maxCount = 10)
        {
            return null;
        }



    }
}
