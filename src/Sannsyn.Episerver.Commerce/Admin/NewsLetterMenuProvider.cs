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

namespace Sannsyn.Episerver.Commerce.Admin
{
    [MenuProvider]
    public class NewsLetterMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var sannsynMainMenu = new SectionMenuItem("Sannsyn", "/global/sannsyn");
            sannsynMainMenu.IsAvailable = ((RequestContext request) => true); //((RequestContext request) => PrincipalInfo.Current.HasPathAccess(UriSupport.Combine("/AdminPage", "")));

            //var adminMenuItem = new UrlMenuItem("Admin", "/global/sannsyn/admin", "/sannsyn/sannsynAdmin");
            //adminMenuItem.IsAvailable = ((RequestContext request) => true);
            //adminMenuItem.SortIndex = 100;

            return new MenuItem[]
            {
                sannsynMainMenu
                //adminMenuItem
            };
        }
    }
}
