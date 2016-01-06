using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Search;
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Extensions;
using Sannsyn.Episerver.Commerce.Models;

namespace Sannsyn.Episerver.Commerce.Services
{
    [ServiceConfiguration(typeof(ISannsynCatalogIndexService))]
    public class SannsynCatalogIndexService : ISannsynCatalogIndexService
    {

        private readonly ReferenceConverter _referenceConverter;
        private readonly IContentLoader _contentLoader;
        private readonly SannsynConfiguration _configuration;
        private readonly ISannsynUpdateService _sannsynUpdateService;
        private readonly ICurrentMarket _currentMarket;
        private readonly ILogger _logger;
        private int _bulkSize;

        public SannsynCatalogIndexService(ReferenceConverter referenceConverter,
            IContentLoader contentLoader,
            SannsynConfiguration sannsynConfiguration,
            ISannsynUpdateService sannsynUpdateService,
            ICurrentMarket currentMarket,
            ILogger logger
           )
        {
            _referenceConverter = referenceConverter;
            _contentLoader = contentLoader;
            _configuration = sannsynConfiguration;
            _sannsynUpdateService = sannsynUpdateService;
            _currentMarket = currentMarket;
            _logger = logger;
            _bulkSize = 100;
        }

        public virtual int BulkSize
        {
            get { return _bulkSize; }
            set { _bulkSize = value; }
        }


        public virtual int IndexProductsWithCategories()
        {
            int numberOfProductsSentToSannsyn = 0;
            IEnumerable<ContentReference> contentLinks = _contentLoader.GetDescendents(GetCatalogRoot());

            var availableLocalizations = GetAvailableLocalizations();

            foreach (CultureInfo culture in availableLocalizations)
            {
                int allContentsCount = contentLinks.Count();
                for (var i = 0; i < allContentsCount; i += _bulkSize)
                {
                    IEnumerable<EntryContentBase> products = GetEntriesToIndex(_bulkSize, contentLinks, culture, i);

                    // First get indexable content items
                    Dictionary<string, EntryContentBase> indexableContentItems = GetIndexableContentItems(products);

                    // Get models Sannsyn can index
                    List<SannsynUpdateEntityModel> sannsynObjects = GetUpdateModels(indexableContentItems);

                    numberOfProductsSentToSannsyn = numberOfProductsSentToSannsyn + sannsynObjects.Count;

                    _logger.Debug("Sending {0} entries to Sannsyn index", sannsynObjects.Count);

                    SannsynUpdateModel sannsynModel = new SannsynUpdateModel();
                    sannsynModel.Service = _configuration.Service;
                    sannsynModel.Updates = sannsynObjects;
                    _sannsynUpdateService.SendToSannsyn(sannsynModel);
                }
            }

            _logger.Debug("Done sending {0} entries to Sannsyn index", numberOfProductsSentToSannsyn);

            return numberOfProductsSentToSannsyn;
        }

        protected virtual IEnumerable<CultureInfo> GetAvailableLocalizations()
        {
            // Multimarket is not supported in v1
            // _localizationService.AvailableLocalizations;

            List<CultureInfo> localizations = new List<CultureInfo>();

            var defaultMarket = _currentMarket.GetCurrentMarket();
            if (defaultMarket == null) throw new ArgumentNullException(nameof(defaultMarket));

            localizations.Add(defaultMarket.DefaultLanguage);

            return localizations;
        }

        protected IEnumerable<EntryContentBase> GetEntriesToIndex(int bulkSize, IEnumerable<ContentReference> contentLinks, CultureInfo culture, int skipCount)
        {
            var items = _contentLoader.GetItems(contentLinks.Skip(skipCount).Take(bulkSize),
                new LanguageSelector(culture.Name));

            // We only care about entires, not nodes, packages etc.
            var entriesToIndex = items.OfType<EntryContentBase>();
            return entriesToIndex;
        }

        protected List<SannsynUpdateEntityModel> GetUpdateModels(Dictionary<string, EntryContentBase> indexableContentItems)
        {
            List<SannsynUpdateEntityModel> sannsynObjects = new List<SannsynUpdateEntityModel>();
            foreach (var indexableContentItem in indexableContentItems)
            {
                EntryContentBase product = indexableContentItem.Value;
                List<string> parentCategories = product.GetParentCategoryCodes(product.Language.Name);
                SannsynUpdateEntityModel model = new SannsynUpdateEntityModel();
                model.Customer = product.Code;
                model.Tags = new List<string> { "itemcat" };
                model.EntityIDs = parentCategories;
                sannsynObjects.Add(model);

            }
            return sannsynObjects;
        }

        protected Dictionary<string, EntryContentBase> GetIndexableContentItems(IEnumerable<EntryContentBase> products)
        {
            Dictionary<string, EntryContentBase> indexableContentItems = new Dictionary<string, EntryContentBase>();
            foreach (var product in products)
            {
                EntryContentBase indexableProduct = product;
                string code = indexableProduct.Code;
                // Get "indexable" content
                var parentProduct = indexableProduct.GetParent();
                if (parentProduct != null)
                {
                    indexableProduct = parentProduct;
                    code = parentProduct.Code;
                }

                if (indexableContentItems.ContainsKey(code) == false)
                {
                    indexableContentItems.Add(code, indexableProduct);
                }
            }
            return indexableContentItems;
        }

        protected ContentReference GetCatalogRoot()
        {
            var ids = GetCatalogIds().ToList();
            if (ids.Any())
            {
                return _referenceConverter.GetContentLink(ids.First(), CatalogContentType.Catalog, 0);
            }

            return ContentReference.EmptyReference;
        }

        private IEnumerable<int> GetCatalogIds()
        {
            ICatalogSystem catalogSystem = ServiceLocator.Current.GetInstance<ICatalogSystem>();
            CatalogDto catalogDto = catalogSystem.GetCatalogDto();
            foreach (CatalogDto.CatalogRow row in catalogDto.Catalog)
            {
                yield return row.CatalogId;
            }

        }
    }
}
