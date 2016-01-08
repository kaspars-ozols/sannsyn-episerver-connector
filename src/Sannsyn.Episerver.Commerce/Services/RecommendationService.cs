using System;
using System.Collections.Generic;
using System.Net.Http;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Sannsyn.Episerver.Commerce.Backend;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Models;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(IRecommendationService))]
    public class RecommendationService : IRecommendationService
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
        public IEnumerable<string> GetRecommendationsForCustomer(string customerId, int maxCount = 10)
        {
            string recommender = Constants.Recommenders.UserItemClickBuy;
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Recommend, recommender +"/" + customerId + "/" +maxCount);
           
            HttpClient client = _backendService.GetConfiguredClient();
            var model = _backendService.GetResult<SannsynRecommendModel>(serviceUrl, client);

            return model.Result;

        }

        public IEnumerable<string> GetRecommendationsForCustomerByCategory(string customerId,List<string> categories, int maxCount = 10)
        {
            string recommender = Constants.Recommenders.UserItemClickBuy;
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.MipRecommend, null, null);
            SannsynMipRecommendModel mipRecommendModel = new SannsynMipRecommendModel();
            mipRecommendModel.Service = _configuration.Service;
            mipRecommendModel.Recommender = recommender;
            mipRecommendModel.MainID = customerId;
            mipRecommendModel.AuxiliaryIDs = categories;
            mipRecommendModel.Number = maxCount;
            mipRecommendModel.Tags = new List<string>();
          
            HttpClient client = _backendService.GetConfiguredClient();
            var jsonData = JsonConvert.SerializeObject(mipRecommendModel);
            HttpContent content = new StringContent(jsonData);
            SannsynRecommendModel model = _backendService.GetResult<SannsynRecommendModel>(serviceUrl, client, content);
            
            return model.Result;

        }


        /// <summary>
        /// Get recommended products for a given product
        /// </summary>
        /// <param name="productCode">Product code to get recommendations for</param>
        /// <param name="maxCount">Number of recommendations to return</param>
        /// <returns>A list of entry codes</returns>
        public IEnumerable<string> GetRecommendationsForProduct(string productCode, int maxCount = 10)
        {
            string recommender = Constants.Recommenders.ItemItemClickBuy;
            Uri serviceUrl = _backendService.GetServiceMethodUri(Constants.ServiceMethod.Recommend, recommender +"/" + productCode + "/" + maxCount);
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
