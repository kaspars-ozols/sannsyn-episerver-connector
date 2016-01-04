using System;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders;
using Sannsyn.Episerver.Commerce.Models;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Initialization
{
    [ModuleDependency(typeof(global::EPiServer.Commerce.Initialization.InitializationModule))]
    public class IndexEventInitialization : IConfigurableModule
    {
        private ILogger _log = LogManager.GetLogger();
        public void Initialize(InitializationEngine context)
        {
            OrderContext.Current.OrderGroupUpdated += Current_OrderGroupUpdated;
        }


        private void Current_OrderGroupUpdated(object sender, OrderGroupEventArgs e)
        {
        
            GenerateSannsynData(sender as OrderGroup, e);
        }

        /// <summary>
        /// Adding lineitem entries to Sannsyn. Since OrderGroupUpdated is called often, we add last 
        /// order indexed in DDS, so we don't index duplicate orders. All code is in a try, then the order 
        /// will still be completed if error occures when calling sannsyn or DDS
        /// </summary>
        /// <param name="order"></param>
        /// <param name="e"></param>
        private void GenerateSannsynData(OrderGroup order, OrderGroupEventArgs e)
        {
            if (order.Status.Equals("InProgress"))
            {
                try
                {
                    SannsynDdsModel lastOrderIndexed = SannsynDdsModel.GetLastOrderIndexed();
                    if (lastOrderIndexed.LastIndexedOrderId < order.OrderGroupId)
                    {
                        ISannsynOrderIndexerService sannsynOrderIndexerService = ServiceLocator.Current.GetInstance<ISannsynOrderIndexerService>();
                        sannsynOrderIndexerService.AddLineItemsToSannsyn(order);

                        lastOrderIndexed.LastIndexedOrderId = order.OrderGroupId;
                        lastOrderIndexed.Save();
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Could not add order items to Sannsyn", ex);
                }
               
            }



        }

        public void Uninitialize(InitializationEngine context)
        {

        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {

        }
    }
}
