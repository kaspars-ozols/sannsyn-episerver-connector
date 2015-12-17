using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediachase.Commerce.Orders;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface ISannsynOrderIndexerService
    {
        void AddLineItemsToSannsyn(OrderGroup orderGroup);
    }
}
