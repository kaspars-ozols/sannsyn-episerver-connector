using System;
using System.Collections.Generic;
using System.Web;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Security;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Extensions;

namespace Sannsyn.Episerver.Commerce.ClientScriptRegistration
{
    [ClientResourceRegister]
    public class SannsynResourceRegister : IClientResourceRegister
    {
        public void RegisterResources(IRequiredClientResourceList requiredResources, HttpContextBase context)
        {
            SannsynConfiguration config = ServiceLocator.Current.GetInstance<SannsynConfiguration>();
            if (config.ModuleEnabled)
            {
                PageRouteHelper instance = ServiceLocator.Current.GetInstance<PageRouteHelper>();
                if (instance.Content != null)
                {
                    // Add Crec on all Content
                    // requiredResources.RequireScript(GenerateCrecUrl(), "SannsynCrec", null).AtHeader();
                    requiredResources.RequireScriptInline(GenerateCrecScript(config), "SannsynCrecScript", null).AtHeader();

                    // Click url is only for product / variant content
                    var content = instance.Content;
                    if (content is EntryContentBase)
                    {
                        var userId = EPiServer.Security.PrincipalInfo.CurrentPrincipal.GetContactId();
                        EntryContentBase entry = content as EntryContentBase;
                        string productCode = entry.Code;
                        List<string> parentCategories = entry.GetParentCategoryCodes(entry.Language.Name);

                        string sannsynClickUrl = GenerateClickUrl(userId, productCode, parentCategories);
                        ClientResources.RequireScript(sannsynClickUrl);
                    }
                }
            }
        }

        private string GenerateClickUrl(Guid userId, string productCode, List<string> parentCategories)
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
        /// Generates the customer recognition url
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productCode"></param>
        /// <param name="parentCategories"></param>
        /// <returns></returns>
        private string GenerateCrecUrl()
        {
            SannsynConfiguration config = ServiceLocator.Current.GetInstance<SannsynConfiguration>();

            //Example url:
            // <script type="application/javascript" src="https://episerver.sannsyn.com/jsrecapi/1.0/crec"></script>
            string serviceUrl = config.ServiceUrl + "jsrecapi/1.0/crec";
            return serviceUrl;
        }

        private string GenerateCrecScript(SannsynConfiguration config)
        {
            string script = @"
    var crecReq = new XMLHttpRequest();
    crecReq.withCredentials = true;
    crecReq.timeout = " + config.ScriptTimeout + @"; // Timeout set in milliseconds
    crecReq.open('GET', '" + config.ServiceUrl + @"jsrecapi/1.0/crec');
    crecReq.setRequestHeader('x-ssasid', localStorage.ssasid);
    crecReq.onreadystatechange = function() 
    {
        if (crecReq.readyState == XMLHttpRequest.DONE)
        {
            if (crecReq.status == 200 || crecReq.status == 304)
            {
                var script = document.createElement('SCRIPT');
                var innerText = document.createTextNode(crecReq.responseText);
                script.appendChild(innerText);
                document.head.appendChild(script);
                set_ssas_host('" + config.ServiceUrl.Host + @"');
            }
        }
    }
    crecReq.send();
";
            return script;
        }


    }
}
