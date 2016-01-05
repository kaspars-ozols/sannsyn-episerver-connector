using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Creates a list of parent category names for a product
        /// </summary>
        /// <param name="productContent">Product to get parent category names from</param>
        /// <param name="language">Current language name</param>
        /// <returns>List of category names</returns>
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

        /// <summary>
        /// Creates a list of Parent categories for a product
        /// </summary>
        /// <param name="productContent">Product to get parent categories from</param>
        /// <param name="language">Current language name</param>
        /// <returns>List of Categories as CatalogContentBase</returns>
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

        /// <summary>
        /// Get the parent of a catalog entry
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static EntryContentBase GetParent(this EntryContentBase content)
        {
            if (content != null)
            {
                IEnumerable<Relation> parentRelations = linksRepository.GetRelationsByTarget(content.ContentLink);
                if (parentRelations.Any())
                {
                    Relation firstRelation = parentRelations.FirstOrDefault();
                    if (firstRelation != null)
                    {
                        var ParentProductContent = contentLoader.Get<EntryContentBase>(firstRelation.Source);
                        return ParentProductContent;
                    }
                }
            }
            return null;
        }
    }
}
