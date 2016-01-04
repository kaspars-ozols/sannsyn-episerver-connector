using Mediachase.Commerce.Orders;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface ISannsynOrderIndexerService
    {
        void AddLineItemsToSannsyn(OrderGroup orderGroup);
    }
}
