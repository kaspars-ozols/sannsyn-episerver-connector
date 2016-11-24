using System.Web;
using EPiServer.Framework.Web.Resources;

namespace Sannsyn.Episerver.Commerce.ClientScriptRegistration
{
    public interface IClientScriptFactory
    {
        void RegisterResources(IRequiredClientResourceList requiredResources, HttpContextBase context);
    }
}