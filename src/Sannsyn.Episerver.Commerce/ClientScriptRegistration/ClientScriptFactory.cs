using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Security;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Extensions;

namespace Sannsyn.Episerver.Commerce.ClientScriptRegistration
{
    [ServiceConfiguration(typeof(IClientScriptFactory))]
    public class ClientScriptFactory : IClientScriptFactory
    {
        protected readonly SannsynConfiguration _configuration;
        private readonly IContentLoader _contentLoader;
        public const string CrecScriptName = "SannsynCrecScript";

        public ClientScriptFactory(SannsynConfiguration configuration, IContentLoader contentLoader)
        {
            _configuration = configuration;
            _contentLoader = contentLoader;
        }

        public virtual void RegisterResources(IRequiredClientResourceList requiredResources, HttpContextBase context)
        {
            if (_configuration.ModuleEnabled)
            {
                var instance = ServiceLocator.Current.GetInstance<IPageRouteHelper>();
                if (instance.Content != null)
                {
                    // The user we're tracking
                    var userId = EPiServer.Security.PrincipalInfo.CurrentPrincipal.GetContactId();

                    // This is the main Sannsyn Recommendation script, it should go on all pages
                    requiredResources.RequireScriptInline(GenerateCrecScript(userId), CrecScriptName, null).AtHeader();

                    // Register product view only for product / variant content
                    if (instance.Content is EntryContentBase)
                    {
                        requiredResources.RequireScript(GetProductViewTrackingScript(instance.Content, userId));
                    }
                }
            }
        }

        protected virtual string GetProductViewTrackingScript(IContent content, Guid userId)
        {
            if (content == null) throw new ArgumentNullException("content");
            EntryContentBase entry = content as EntryContentBase;

            if (entry is VariationContent)
            {
                // Get parent product
                var parentProducts = GetParentProductsForVariation(entry as VariationContent);
                var productContent = parentProducts.FirstOrDefault();
                // This implementation will use the first one
                if (productContent != null)
                {
                    entry = productContent;
                }
            }

            string productCode = entry.Code;
            List<string> parentCategories = entry.GetParentCategoryCodes(entry.Language.Name);

            return GenerateClickUrl(userId, productCode, parentCategories);
        }

        /// <summary>
        /// Will get a variation's parent product. If the variant has more than one product, it will get
        /// the first one returned by Commerce.
        /// </summary>
        /// <remarks>
        /// If you want to change how this logic works, please override this method and register your
        /// class for the IClientScriptFactory interface in the service locator.
        /// </remarks>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ProductContent> GetParentProductsForVariation(VariationContent entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");

            var parentProductLinks = entry.GetParentProducts();

            IEnumerable<ProductContent> products = _contentLoader.GetItems(parentProductLinks, entry.Language).OfType<ProductContent>();

            return products;
        }

        protected virtual string GenerateClickUrl(Guid userId, string productCode, List<string> parentCategories)
        {
            SannsynConfiguration config = ServiceLocator.Current.GetInstance<SannsynConfiguration>();

            //Example url:
            // http://episerver.sannsyn.com/jsrecapi/1.0/tupleupdate/epicphoto/admin/canon-5d-m3/click/photo/catclick/dslr/catclick

            string serviceUrl = config.ServiceUrl.ToString() + "jsrecapi/1.0/tupleupdate/" + config.Service;
            string clickUrl = string.Format("{0}/{1}/{2}/click", serviceUrl, userId, productCode);
            foreach (string category in parentCategories)
            {
                clickUrl = string.Format("{0}/{1}/catclick", clickUrl, category);
            }
            return clickUrl;
        }


        /// <summary>
        /// Generates the recognition script that will load the main script async from the Sannsyn servers
        /// It also sets up a timeout to prevent the script from causing delays on the site.
        /// </summary>
        protected virtual string GenerateCrecScript(Guid userId)
        {
            string script = @"
    var crecReq = new XMLHttpRequest();
    crecReq.withCredentials = true;
    crecReq.timeout = " + _configuration.ScriptTimeout + @"; // Timeout set in milliseconds
    crecReq.open('GET', '" + _configuration.ServiceUrl + @"jsrecapi/1.0/crec?service=" + _configuration.Service + "&euid=" + userId.ToString() + @"');
    crecReq.setRequestHeader('x-ssasid', localStorage.ssasid);
    crecReq.onreadystatechange = function() 
    {
        if (crecReq.readyState == XMLHttpRequest.DONE)
        {
            if (crecReq.status == 200 || crecReq.status == 304)
            {
                var script = document.createElement('SCRIPT');
                script.type = 'text/javascript';
                var innerText = document.createTextNode(crecReq.responseText);
                script.appendChild(innerText);
                document.head.appendChild(script);
            }
        }
    }
    crecReq.send();
";
            return script;
        }

    }
}
