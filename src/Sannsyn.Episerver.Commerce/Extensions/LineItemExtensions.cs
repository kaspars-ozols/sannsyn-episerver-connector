using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
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
		/// <returns></returns>
		private static T GetEntryContent<T>(string code) where T : EntryContentBase
		{
			return ContentLoader.Service.Get<T>(ReferenceConverter.Service.GetContentLink(code, CatalogContentType.CatalogEntry));
		}

    }
}
