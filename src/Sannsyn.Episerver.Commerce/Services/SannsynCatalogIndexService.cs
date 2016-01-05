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
using EPiServer.ServiceLocation;
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
        private readonly LocalizationService _localizationService;
        private readonly SannsynConfiguration _configuration;
        private readonly ISannsynUpdateService _sannsynUpdateService;

        public SannsynCatalogIndexService(ReferenceConverter referenceConverter,
            IContentLoader contentLoader,
            LocalizationService localizationService,
            SannsynConfiguration sannsynConfiguration,
            ISannsynUpdateService sannsynUpdateService
           )
        {
            _referenceConverter = referenceConverter;
            _contentLoader = contentLoader;
            _localizationService = localizationService;
            _configuration = sannsynConfiguration;
            _sannsynUpdateService = sannsynUpdateService;
        }

       

        public int IndexProductsWithCategories()
        {
            int bulkSize = 100;
            int numberOfProductsSentToSannsyn = 0;
            IEnumerable<ContentReference> contentLinks = _contentLoader.GetDescendents(Root);
            foreach (CultureInfo availableLocalization in _localizationService.AvailableLocalizations)
            {
                int allContentsCount = contentLinks.Count();
                for (var i = 0; i < allContentsCount; i += bulkSize)
                {
                    List<SannsynUpdateEntityModel> sannsynObjects = new List<SannsynUpdateEntityModel>();
                    var items = _contentLoader.GetItems(contentLinks.Skip(i).Take(bulkSize),
                        new LanguageSelector(availableLocalization.Name));
                    var products = items.OfType<ProductContent>().ToList();
                    foreach (var product in products)
                    {
                        List<string> parentCategories = product.GetParentCategoryCodes(product.Language.Name);
                        SannsynUpdateEntityModel model = new SannsynUpdateEntityModel();
                        model.Customer = product.Code;
                        model.Tags = new List<string> { "itemcat" };
                        model.EntityIDs = parentCategories;
                        sannsynObjects.Add(model);
                        numberOfProductsSentToSannsyn++;
                    }
                    SannsynUpdateModel sannsynModel = new SannsynUpdateModel();
                    sannsynModel.Service = _configuration.Service;
                    sannsynModel.Updates = sannsynObjects;
                    _sannsynUpdateService.SendToSannsyn(sannsynModel);
                }
            }
            return numberOfProductsSentToSannsyn;
        }

        private ContentReference Root
        {
            get
            {
                var ids = GetCatalogIds().ToList();
                if (ids.Any())
                {
                    return _referenceConverter.GetContentLink(ids.First(), CatalogContentType.Catalog, 0);
                }

                return ContentReference.EmptyReference;

            }
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
