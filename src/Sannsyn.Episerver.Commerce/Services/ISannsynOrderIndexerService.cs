using System.Collections.Generic;
using Mediachase.Commerce.Orders;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface ISannsynOrderIndexerService
    {
        void AddLineItemsToSannsyn(OrderGroup orderGroup);
        List<PurchaseOrder> GetOrders(string sqlWhereClause, string sqlMetaWhereClause, int recordCount = int.MaxValue);
    }
}
