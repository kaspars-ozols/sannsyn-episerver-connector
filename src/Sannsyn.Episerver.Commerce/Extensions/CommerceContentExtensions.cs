using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthorizeNet.Util;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;

namespace Sannsyn.Episerver.Commerce.Extensions
{
    public static class CommerceContentExtensions
    {
        static readonly ILinksRepository linksRepository = ServiceLocator.Current.GetInstance<ILinksRepository>();
        static readonly IContentLoader contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        static readonly ReferenceConverter referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();

        public static List<string> GetParentCategoryNames(this CatalogContentBase productContent, string language)
        {
            var parentCategories = productContent.GetProductCategories(language);
            List<string> names = new List<string>();
            foreach (var category in parentCategories)
            {
                names.Add(category.Name);
            }
            return names;
        }
        public static List<string> GetParentCategoryCodes(this CatalogContentBase productContent, string language)
        {
            var parentCategories = productContent.GetProductCategories(language);
            List<string> names = new List<string>();
            foreach (var category in parentCategories)
            {
                NodeContent node = category as NodeContent;
                if(node != null)
                {
                    string code = node.Code;
                    if(names.Contains(code) == false)
                    {
                        names.Add(code);
                    }
                }
            }
            return names;
        }

        public static List<CatalogContentBase> GetProductCategories(this CatalogContentBase productContent, string language)
        {

            var allRelations = linksRepository.GetRelationsBySource(productContent.ContentLink);
            var categories = allRelations.OfType<NodeRelation>().ToList();
            List<CatalogContentBase> parentCategories = new List<CatalogContentBase>();
            try
            {
                if (categories.Any())
                {
                    // Add all categories (nodes) that this product is part of
                    foreach (var nodeRelation in categories)
                    {
                        if (nodeRelation.Target != referenceConverter.GetRootLink())
                        {
                            CatalogContentBase parentCategory =
                                contentLoader.Get<CatalogContentBase>(nodeRelation.Target,
                                    new LanguageSelector(language));
                            if (parentCategory != null && parentCategory.ContentType != CatalogContentType.Catalog)
                            {
                                parentCategories.Add(parentCategory);
                            }
                        }
                    }
                }

                var content = productContent;

                // Now walk the category tree until we hit the catalog node itself
                while (content.ParentLink != null && content.ParentLink != referenceConverter.GetRootLink())
                {
                    CatalogContentBase parentCategory =
                      contentLoader.Get<CatalogContentBase>(content.ParentLink, new LanguageSelector(language));
                    if (parentCategory.ContentType != CatalogContentType.Catalog)
                    {
                        parentCategories.Add(parentCategory);
                    }
                    content = parentCategory;
                }
            }
            catch (Exception ex)
            {
                // TODO: Fix this empty catch, it is too greedy
               
            }
            return parentCategories;
        }
    }
}
