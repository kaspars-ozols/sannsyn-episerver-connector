using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders;

namespace Sannsyn.Episerver.Commerce.Extensions
{
    public static class LineItemExtensions
    {
		public static Injected<UrlResolver> UrlResolver { get; set; }
		public static Injected<ReferenceConverter> ReferenceConverter { get; set; }
		public static Injected<ICurrentMarket> CurrentMarket { get; set; }
		public static Injected<IContentLoader> ContentLoader { get; set; }
		
		/// <summary>
		/// Gets the content of the entry.
		/// </summary>
		/// <param name="lineItem">The line item.</param>
		/// <returns></returns>
		public static T GetEntryContent<T>(this LineItem lineItem) where T : EntryContentBase
		{
            return GetEntryContent<T>(lineItem.Code);
		}


		/// <summary>
		/// Gets the content of the entry by code.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="code">The code.</param>
		/// <returns>The Entry if found, null if not</returns>
		private static T GetEntryContent<T>(string code) where T : EntryContentBase
		{
		    var contentLink = ReferenceConverter.Service.GetContentLink(code, CatalogContentType.CatalogEntry);
            if(ContentReference.IsNullOrEmpty(contentLink))
            {
                return null;
            }

		    T entry;
            bool result = ContentLoader.Service.TryGet(contentLink, out entry);
            if(result)
            {
                return entry;
            }
		    return null;
		}
    }
}
