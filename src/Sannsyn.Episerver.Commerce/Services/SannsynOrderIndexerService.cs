using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Search;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Models;
using Sannsyn.Episerver.Commerce.Extensions;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(ISannsynOrderIndexerService))]
    public class SannsynOrderIndexerService : ISannsynOrderIndexerService
    {
        private readonly ILogger _log;
        private readonly SannsynConfiguration _configuration;
        private readonly ISannsynUpdateService _sannsynUpdateService;


        public SannsynOrderIndexerService(global::EPiServer.Logging.ILogger log, 
            SannsynConfiguration configuration,
            ISannsynUpdateService sannsynUpdateService)
        {
            _log = log;
            _configuration = configuration;
            _sannsynUpdateService = sannsynUpdateService;
        }

        public List<PurchaseOrder> GetOrders(string sqlWhereClause, string sqlMetaWhereClause, int recordCount = int.MaxValue)
        {
            var orderSearchParameters = new OrderSearchParameters();
            if (!string.IsNullOrEmpty(sqlWhereClause))
            {
                orderSearchParameters.SqlWhereClause = sqlWhereClause;
            }

            if (!string.IsNullOrEmpty(sqlMetaWhereClause))
            {
                orderSearchParameters.SqlMetaWhereClause = sqlMetaWhereClause;
            }

            var orderSearchOptions = new OrderSearchOptions();
            orderSearchOptions.Namespace = "Mediachase.Commerce.Orders";
            orderSearchOptions.Classes.Add("PurchaseOrder");
            orderSearchOptions.Classes.Add("Shipment");
            orderSearchOptions.CacheResults = false;
            orderSearchOptions.RecordsToRetrieve = recordCount;

            return OrderContext.Current.FindPurchaseOrders(orderSearchParameters, orderSearchOptions).ToList();
        }


        /// <summary>
        /// Creates a update model to Sannsyn, with a list of entry/variation codes, and service name
        /// </summary>
        /// <param name="orderGroup">Order to get lineitems from</param>
        public void AddLineItemsToSannsyn(OrderGroup orderGroup)
        {
            LineItemCollection lineItems = orderGroup.OrderForms.First().LineItems;

            List<SannsynUpdateEntityModel> sannsynObjects = new List<SannsynUpdateEntityModel>();
            foreach (LineItem lineItem in lineItems)
            {
                SannsynUpdateEntityModel model = CreateSannsynObject(lineItem, orderGroup.CustomerId);
                if(model != null)
                {
                    sannsynObjects.Add(model);
                }
            }

            // Make sure we have something to index
            if(sannsynObjects.Any())
            {
                SannsynUpdateModel sannsynModel = new SannsynUpdateModel();
                sannsynModel.Service = _configuration.Service;
                sannsynModel.Updates = sannsynObjects;
                _sannsynUpdateService.SendToSannsyn(sannsynModel);
            }
        }

       

        /// <summary>
        /// Creates a SannsynUpdateEntityModel with customerId, buy tag and list of entries
        /// </summary>
        /// <param name="lineItem">LineItem with entry</param>
        /// <param name="customerId">Customer buying</param>
        /// <returns></returns>
        private SannsynUpdateEntityModel CreateSannsynObject(LineItem lineItem, Guid customerId)
        {
            var entry = lineItem.GetEntryContent<EntryContentBase>();
            if (entry == null)
            {
                // Entry could have been deleted after order was placed
                return null;
            } 
            var parent = entry.GetParent();
            string code = parent == null ? entry.Code : parent.Code;

            SannsynUpdateEntityModel model = new SannsynUpdateEntityModel();
            model.Customer = customerId.ToString();
            model.Tags = new List<string> {Constants.Tags.Buy};
            model.EntityIDs = new List<string> { code };
            model.Time = lineItem.Modified.ToJavaTimeStamp();
            model.Boost = (float) 0.0;
            model.Number = (int) lineItem.Quantity;
            return model;
        }

    }
}
