using System.Web;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;

namespace Sannsyn.Episerver.Commerce.ClientScriptRegistration
{
    [ClientResourceRegister]
    public class SannsynResourceRegister : IClientResourceRegister
    {
        public void RegisterResources(IRequiredClientResourceList requiredResources, HttpContextBase context)
        {
            var scriptFactory = ServiceLocator.Current.GetInstance<IClientScriptFactory>();
            scriptFactory.RegisterResources(requiredResources, context);

        }
    }
}
