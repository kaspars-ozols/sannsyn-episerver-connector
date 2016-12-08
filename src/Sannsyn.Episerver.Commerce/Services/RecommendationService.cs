using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Backend;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Models;

namespace Sannsyn.Episerver.Commerce.Services
{
#pragma warning disable CS0618

    [ServiceConfiguration(typeof(IRecommendationService))]
    [ServiceConfiguration(typeof(ITrackedRecommendationService))]
    public class RecommendationService : IRecommendationService, ITrackedRecommendationService
    {
        private readonly BackendService _backendService;
        private readonly SannsynConfiguration _configuration;

        public RecommendationService(BackendService backendService, SannsynConfiguration configuration)
        {
            _backendService = backendService;
            _configuration = configuration;
        }

        /// <summary>
        /// Get recommended products for a customer
        /// </summary>
        /// <param name="customerId">Customer id to get Recommendations for</param>
        /// <param name="maxCount">Number of recommendations to return</param>
        /// <returns>A list of entry codes</returns>
        [Obsolete("Use the ITrackedRecommendationService instead")]
        public IEnumerable<string> GetRecommendationsForCustomer(string customerId, int maxCount = 10)
        {
            string recommender = Constants.Recommenders.UserItemClickBuy;
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Recommend, recommender +"/" + customerId + "/" +maxCount);
           
            HttpClient client = _backendService.GetConfiguredClient();
            var model = _backendService.GetResult<SannsynRecommendModel>(serviceUrl, client);

            return model.Result;

        }

        IRecommendations ITrackedRecommendationService.GetRecommendationsForCustomer(string customerId, int maxCount)
        {
            IRecommendations recommendations = new Recommendations(Constants.Recommenders.UserItemClickBuy, GetRecommendationsForCustomer(customerId, maxCount));
            return recommendations;
        }

        /// <summary>
        /// Get recommended products for a given product
        /// </summary>
        /// <param name="productCode">Product code to get recommendations for</param>
        /// <param name="maxCount">Number of recommendations to return</param>
        /// <returns>A list of entry codes</returns>
        [Obsolete("Use the ITrackedRecommendationService instead")]
        public IEnumerable<string> GetRecommendationsForProduct(string productCode, int maxCount = 10)
        {
            string recommender = Constants.Recommenders.ItemItemClickBuy;
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Recommend, recommender +"/" + productCode + "/" + maxCount);
            HttpClient client = _backendService.GetConfiguredClient();
            var model = _backendService.GetResult<SannsynRecommendModel>(serviceUrl, client);

            return model.Result;
        }

        IRecommendations ITrackedRecommendationService.GetRecommendationsForProduct(string productCode, int maxCount)
        {
            Recommendations recommendations = new Recommendations(Constants.Recommenders.ItemItemClickBuy,
                GetRecommendationsForProduct(productCode, maxCount));
            return recommendations;
        }

        /// <summary>
        /// Get recommended products based on what a customer has in the cart
        /// </summary>
        /// <param name="customerId">Customer id to get Recommendations for</param>
        /// <param name="productCodes">The products to get recommendations for</param>
        /// <param name="maxCount">Number of recommendations to return</param>
        /// <returns>A list of entry codes</returns>
        [Obsolete("Use the ITrackedRecommendationService instead")]
        public IEnumerable<string> GetRecommendationsForCart(string customerId, IEnumerable<string> productCodes, int maxCount = 10)
        {
            return MipRecommend(customerId, productCodes, maxCount, Constants.Recommenders.CartItemsRecommender);
        }

        IRecommendations ITrackedRecommendationService.GetRecommendationsForCart(string customerId, IEnumerable<string> productCodes, int maxCount)
        {
            IRecommendations recommendations = new Recommendations(Constants.Recommenders.CartItemsRecommender,
                GetRecommendationsForCart(customerId, productCodes, maxCount));
            return recommendations;
        }

        [Obsolete("Use the ITrackedRecommendationService instead")]
        public IEnumerable<string> GetRecommendationsForCustomerByCategory(string customerId,List<string> categories, int maxCount = 10)
        {
            return MipRecommend(customerId, categories, maxCount, Constants.Recommenders.UserItemCategoryClickBuy);
        }

        IRecommendations ITrackedRecommendationService.GetRecommendationsForCustomerByCategory(string customerId, List<string> categories, int maxCount)
        {
            IRecommendations recommendations = new Recommendations(Constants.Recommenders.UserItemCategoryClickBuy,
                GetRecommendationsForCustomerByCategory(customerId, categories, maxCount));
            return recommendations;
        }


        public Dictionary<string,double> GetScoreForItems(int maxCount = 10000)
        {
            Dictionary<string, double> itemScores = new Dictionary<string, double>();
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Recommend, "ScoredItems/a/" + maxCount);
            HttpClient client = _backendService.GetConfiguredClient();
            var model = _backendService.GetResult<ScoredItemsModel>(serviceUrl, client);
            if (model.result != null && model.result.Any())
            {
                foreach (var scoredItem in model.result)
                {
                    itemScores.Add(scoredItem.id, scoredItem.w);
                }
            }
            return itemScores;
        }

        protected IEnumerable<string> MipRecommend(string customerId, IEnumerable<string> externalIds, int maxCount, string recommender)
        {
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.MipRecommend, null, null);
            SannsynMipRecommendModel mipRecommendModel = new SannsynMipRecommendModel();
            mipRecommendModel.Service = _configuration.Service;
            mipRecommendModel.Recommender = recommender;
            // customer id is first in external id list
            mipRecommendModel.ExternalIds = new List<string>();
            mipRecommendModel.ExternalIds.Add(customerId);
            mipRecommendModel.ExternalIds.AddRange(externalIds);

            mipRecommendModel.Number = maxCount;
            mipRecommendModel.Tags = new List<string>();

            HttpClient client = _backendService.GetConfiguredClient();
            var jsonData = JsonConvert.SerializeObject(mipRecommendModel);
            HttpContent content = new StringContent(jsonData);
            SannsynRecommendModel model = _backendService.GetResult<SannsynRecommendModel>(serviceUrl, client, content);

            return model.Result;
        }


    }
    #pragma warning restore CS0618
}
