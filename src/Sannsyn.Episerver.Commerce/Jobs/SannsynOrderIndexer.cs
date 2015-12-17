using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Jobs
{
    [ScheduledPlugIn(DisplayName = "Sannsyn Content Indexer")]
    class SannsynOrderIndexer : ScheduledJobBase
    {
        public override string Execute()
        {
            ISannsynOrderIndexerService sannsynContentIndexer = ServiceLocator.Current.GetInstance<ISannsynOrderIndexerService>();
            

            return "Ok";
        }
    }
}
