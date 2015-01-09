﻿/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Commerce.Catalog.Provider;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.WebControls;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Inventory;
using OxxCommerceStarterKit.Core.Models;


namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class CommerceContentExtensions
    {
        private static ILogger Log = LogManager.GetLogger();

        public static Injected<ILinksRepository> LinksRepository { get; set; }
        public static Injected<IContentLoader> ContentLoader { get; set; }
        public static Injected<ReferenceConverter> ReferenceConverter { get; set; }

        public static List<ContentReference> AssetImageUrls(this EntryContentBase entry)
        {
            var output = new List<ContentReference>();
            if (entry != null)
            {
                var permanentLinkMapper = ServiceLocator.Current.GetInstance<IPermanentLinkMapper>();

                foreach (var commerceMedia in entry.CommerceMediaCollection)
                {
                    if (commerceMedia.GroupName == null || (commerceMedia.GroupName != null && commerceMedia.GroupName.ToLower() != "swatch"))
                    {
                        var contentLink = commerceMedia.AssetContentLink(permanentLinkMapper);
                        output.Add(contentLink);
                    }
                }
            }
            return output;
        }

        public static string AssetSwatchUrl(this EntryContentBase entry)
        {
            var output = new List<string>();
            if (entry != null)
            {
                var permanentLinkMapper = ServiceLocator.Current.GetInstance<IPermanentLinkMapper>();
                var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

                foreach (var commerceMedia in entry.CommerceMediaCollection)
                {
                    if (commerceMedia.GroupName != null && commerceMedia.GroupName.ToLower() == "swatch")
                    {
                        var contentLink = commerceMedia.AssetContentLink(permanentLinkMapper);
                        output.Add(urlResolver.GetUrl(contentLink));
                    }
                }
            }
            return output.FirstOrDefault();
        }

        public static CommerceMedia GetCommerceMedia(this EntryContentBase entry)
        {
            return GetCommerceMedia(entry, 0);
        }

        public static CommerceMedia GetCommerceMedia(this EntryContentBase entry, int index)
        {
            if (entry == null)
                return null;
            return entry.CommerceMediaCollection.OrderBy(m => m.SortOrder).Skip(index).FirstOrDefault();
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
                IEnumerable<Relation> parentRelations = LinksRepository.Service.GetRelationsByTarget(content.ContentLink);
                if (parentRelations.Any())
                {
                    Relation firstRelation = parentRelations.FirstOrDefault();
                    if (firstRelation != null)
                    {
                        var ParentProductContent = ContentLoader.Service.Get<EntryContentBase>(firstRelation.Source);
                        return ParentProductContent;
                    }
                }
            }
            return null;
        }


        public static CatalogContentBase GetParent(this CatalogContentBase content)
        {
            if (content != null)
            {
                return ContentLoader.Service.Get<CatalogContentBase>(content.ParentLink);
            }

            return null;
        }


        public static VariationContent GetFirstVariation(this ProductContent product)
        {
            var variationRelations = LinksRepository.Service.GetRelationsBySource<ProductVariation>(product.ContentLink).FirstOrDefault();

            if (variationRelations != null)
            {
                var variation = ContentLoader.Service.Get<VariationContent>(variationRelations.Target);
                return variation;
            }

            return null;
        }


        /// <summary>
        /// Gets the main category for a product. This is the category
        /// closest to the catalog node itself or any category if the type
        /// SiteCategoryNode
        /// </summary>
        /// <param name="content">The content we want to check.</param>
        /// <param name="language">The language to use when loading the category</param>
        /// <returns>The name of the main category</returns>
        public static string GetMainCategory(this CatalogContentBase content, string language)
        {
            var referenceConverter = ReferenceConverter.Service;
            var contentLoader = ContentLoader.Service;
            const string invalidMainCategory = "Undefined";

            /// TODO: Possible bug, we cannot rely on the order of categories returned from this one
            CatalogContentBase parentCategory = GetProductCategories(content, language).FirstOrDefault();

            if (parentCategory != null)
            {
                while (parentCategory.ParentLink != null && parentCategory.ParentLink != referenceConverter.GetRootLink())
                {
                    if (parentCategory is SiteCategoryContent)
                    {
                        return parentCategory.Name;
                    }
                    var previousCategory = parentCategory;
                    parentCategory = contentLoader.Get<CatalogContentBase>(parentCategory.ParentLink, new LanguageSelector(language));
                    if (parentCategory == null)
                    {
                        return previousCategory.Name;
                    }
                }
                return parentCategory.Name;
            }
            return invalidMainCategory;
        }


        /// <summary>
        /// Gets all parent category names, including the whole category trees a product
        /// resides in.
        /// </summary>
        /// <remarks>
        /// This method will return parent categories recursively, and not just direct parents
        /// </remarks>
        /// <param name="productContent">The actual product.</param>
        /// <param name="language">The language to use when loading category names</param>
        /// <returns>A list of category names that this product resides on, directly or indirectly</returns>
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

        /// <summary>
        /// Gets the name of the parent category for the product
        /// </summary>
        /// <param name="productContent">The product itself.</param>
        /// <param name="language">The language to use when getting the name.</param>
        /// <returns>The name of the immediate parent category.</returns>
        public static string GetCategoryName(this CatalogContentBase productContent, string language)
        {
            CatalogContentBase parentCategory =
                ContentLoader.Service.Get<CatalogContentBase>(productContent.ParentLink, new LanguageSelector(language));
            if (parentCategory != null)
                return parentCategory.Name;
            return string.Empty;

        }


        public static List<int> GetProductCategoryIds(this CatalogContentBase productContent, string language)
        {
            return productContent.GetProductCategories(language).Select(p => p.ContentLink.ID).ToList();
        }

        /// <summary>
        /// Gets the categories for the product and language. It will return all nodes that the product
        /// is part of (could be more than one) and also all parent categories indirectly.
        /// </summary>
        /// <remarks>
        /// Example:
        /// Root
        ///   Catalog
        ///     Category 1
        ///       Product A
        ///     Category 2
        ///       Category 2.1
        ///         Product A
        /// Returns: ["Category 1", "Category 2.1", "Category 2"]
        /// </remarks>
        /// <param name="productContent">The product to get categories for</param>
        /// <param name="language">The language to use</param>
        /// <returns>A list of Categories in the type of CatalogContentBase</returns>
        public static List<CatalogContentBase> GetProductCategories(this CatalogContentBase productContent, string language)
        {

            var allRelations = LinksRepository.Service.GetRelationsBySource(productContent.ContentLink);
            var categories = allRelations.OfType<NodeRelation>().ToList();
            List<CatalogContentBase> parentCategories = new List<CatalogContentBase>();
            try
            {
                if (categories.Any())
                {
                    // Add all categories (nodes) that this product is part of
                    foreach (var nodeRelation in categories)
                    {
                        if (nodeRelation.Target != ReferenceConverter.Service.GetRootLink())
                        {
                            CatalogContentBase parentCategory =
                                ContentLoader.Service.Get<CatalogContentBase>(nodeRelation.Target,
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
                while (content.ParentLink != null && content.ParentLink != ReferenceConverter.Service.GetRootLink())
                {
                    CatalogContentBase parentCategory =
                      ContentLoader.Service.Get<CatalogContentBase>(content.ParentLink, new LanguageSelector(language));
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
                Log.Debug(string.Format("Failed to get categories from product {0}, Code: {1}.", productContent.Name, productContent.ContentLink), ex);
            }
            return parentCategories.DistinctBy(a => a.ContentLink.ID).ToList();
        }

        public static decimal GetStock(this VariationContent content)
        {
            var inventoryService = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>();
            var inventory = inventoryService.GetTotal(new CatalogKey(AppContext.Current.ApplicationId, content.Code));

            if (inventory != null)
            {
                return inventory.InStockQuantity - inventory.ReservedQuantity;
            }
            return 0;
        }
       

        public static List<PriceAndMarket> GetPricesWithMarket(this VariationContent content, IMarket market)
        {

            List<PriceAndMarket> priceAndMarkets = new List<PriceAndMarket>();
            ItemCollection<Price> itemCollection = null;
            try
            {
                itemCollection = content.GetPrices();
            }
            catch (Exception ex)
            {
                Log.Error("GetPrices returned an error at product with id " + content.Code, ex);
            }
            if (itemCollection != null)
            {
                foreach (var price in itemCollection)
                {
                    priceAndMarkets.Add(new PriceAndMarket()
                    {
                        MarkedId = price.MarketId.Value,
                        PriceTypeId = price.CustomerPricing.PriceTypeId.ToString(),
                        PriceCode = price.CustomerPricing.PriceCode,
                        Price = GetPriceString(price),
                        CurrencyCode = price.UnitPrice.Currency.CurrencyCode,
                        CurrencySymbol = price.UnitPrice.Currency.Format.CurrencySymbol//Pricecode??
                    });
                }
            }
            return priceAndMarkets;
        }

        private static string GetPriceString(Price price)
        {
            if (price != null)
            {
                return Math.Round(price.UnitPrice.Amount, 2).ToString();
            }
            return string.Empty;

        }


        public static string GetDefaultImage(this EntryContentBase productContent)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var permanentLinkMapper = ServiceLocator.Current.GetInstance<IPermanentLinkMapper>();

            var commerceMedia = productContent.CommerceMediaCollection.OrderBy(m => m.SortOrder).FirstOrDefault(z => z.GroupName != null && z.GroupName.ToLower() != "swatch");
            if (commerceMedia != null)
            {
                var contentReference = commerceMedia.AssetLink;
                return urlResolver.GetUrl(contentReference, null, new VirtualPathArguments() { ContextMode = ContextMode.Default });
            }

            return "/siteassets/system/no-image.png";
        }


    }
}
