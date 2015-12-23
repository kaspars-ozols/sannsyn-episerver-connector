using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using EPiServer;
using EPiServer.Security;
using EPiServer.Shell.Navigation;
using System.Web.Mvc;
using Sannsyn.Episerver.Commerce.Configuration;

namespace Sannsyn.Episerver.Commerce.Admin
{
    [MenuProvider]
    public class SannsynMenuProvider : IMenuProvider
    {
        private readonly SannsynConfiguration _configuration;

        public SannsynMenuProvider(SannsynConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<MenuItem> GetMenuItems()
        {
            if (_configuration.ModuleEnabled)
            {
                var sannsynMainMenu = new SectionMenuItem("Sannsyn", "/global/sannsyn");
                sannsynMainMenu.IsAvailable = ((RequestContext request) => true);

                return new MenuItem[]
                {
                    sannsynMainMenu
                    //adminMenuItem
                };
            }
            else
            {
                return new MenuItem[] {};
            }

    }
    }
}
