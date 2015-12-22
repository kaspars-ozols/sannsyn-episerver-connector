using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders;
using Sannsyn.Episerver.Commerce.Models;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Initialization
{
    [ModuleDependency(typeof(global::EPiServer.Commerce.Initialization.InitializationModule))]
    public class IndexEventInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            OrderContext.Current.OrderGroupUpdated += Current_OrderGroupUpdated;
        }


        private void Current_OrderGroupUpdated(object sender, OrderGroupEventArgs e)
        {
        
            GenerateSannsynData(sender as OrderGroup, e);
        }

        private void GenerateSannsynData(OrderGroup order, OrderGroupEventArgs e)
        {
            if (order.Status.Equals("InProgress"))
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



        }

        public void Uninitialize(InitializationEngine context)
        {

        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {

        }
    }
}
