using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Sannsyn.Episerver.Commerce.Models
{
    public class SannsynDdsModel : IDynamicData
    {

        public SannsynDdsModel()
        {
            Id = Identity.NewIdentity(Guid.NewGuid());
            
        }

        // Required to implement IDynamicData
        public Identity Id { get; set; }

        public int LastIndexedOrderId { get; set; }

        public void Save()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(SannsynDdsModel));
            store.Save(this);
        }


        public static SannsynDdsModel GetLastOrderIndexed()
        {
            
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(SannsynDdsModel));
         
            var lastOrderIndexed = store.Items<SannsynDdsModel>().FirstOrDefault();

            if (lastOrderIndexed == null)
                return new SannsynDdsModel();
            return lastOrderIndexed;
            
        }
    }

}
