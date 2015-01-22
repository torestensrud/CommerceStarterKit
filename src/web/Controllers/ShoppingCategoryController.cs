/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Mvc;
using OxxCommerceStarterKit.Web.Extensions;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class ShoppingCategoryController : PageController<ShoppingCategoryPage>
	{
	    private const int DefaultNumProductsInList = 9;
	    private readonly IContentLoader _contentLoader;

	    public ShoppingCategoryController(IContentLoader contentLoader)
	    {
	        _contentLoader = contentLoader;
	    }

		public ActionResult Index(ShoppingCategoryPage currentPage)
		{
			var model = new ShoppingCategoryViewModel(currentPage);
			model.Language = currentPage.Language.Name;
			
		    if (currentPage.CatalogNodes != null)
		    {
		        model.CommerceCategoryIds = GetCommerceNodeIds(currentPage);
		    }

			model.NumberOfProductsToShow = currentPage.NumberOfProductsToShow > 0 ? currentPage.NumberOfProductsToShow : DefaultNumProductsInList;

			if (currentPage.ParentLink != null)
			{
				var languageSelector = new LanguageSelector(model.Language);
                var parent = _contentLoader.Get<IContent>(currentPage.ParentLink, languageSelector);
				ShoppingCategoryPage topNode = null;

				while (!(parent is HomePage))
				{
					topNode = parent as ShoppingCategoryPage;
                    parent = _contentLoader.Get<IContent>(parent.ParentLink, languageSelector);
				}

				if (topNode == null && parent is HomePage)
				{
					topNode = currentPage;
					model.CommerceCategoryIds = string.Empty;
				}
				if (topNode != null)
				{
					model.ParentName = topNode.Name;
                    model.CategoryPages = _contentLoader.GetChildren<ShoppingCategoryPage>(topNode.ContentLink);
					model.CommerceRootCategoryName = GetCommerceNodeNames(topNode);
				}
			}

			return View(model);
		}
		private string GetMainCategoryFromParentPage(ContentReference parentReference, CultureInfo languageInfo)
		{
            var parentPage = _contentLoader.Get<PageData>(parentReference, new LanguageSelector(languageInfo.Name)) as ShoppingCategoryPage;
		    if (parentPage != null)
		    {
				return GetCommerceNodeNames(parentPage);
		    }
		    return string.Empty;
		}

		private string GetCommerceNodeNames(ShoppingCategoryPage pageData)
		{
		    List<string> nodeNames = new List<string>();

            // We need to load the catalog nodes, and get the name from them
            // since the link text in the CatalogNodes Link Item Collection is
            // not updated if the node changes name or code.
            // Fixes: https://github.com/OXXAS/CommerceStarterKit/issues/21
		    IEnumerable<NodeContent> nodeContents = pageData.CatalogNodes.ToContent<NodeContent>();
		    foreach (NodeContent nodeContent in nodeContents)
		    {
		        nodeNames.Add(nodeContent.Name);
		    }

		    return string.Join(",", nodeNames);
		}

        private List<ContentReference> GetCommerceNodeIdList(ShoppingCategoryPage pageData)
        {
            List<ContentReference> idList = new List<ContentReference>();

            if (pageData.CatalogNodes != null)
            {
                
                foreach (LinkItem catalogNodeLinkItem in pageData.CatalogNodes)
                {
                    string linkUrl;
                    if (!EPiServer.Web.PermanentLinkMapStore.TryToMapped(catalogNodeLinkItem.Href, out linkUrl))
                        continue;

                    if (string.IsNullOrEmpty(linkUrl))
                        continue;

                    ContentReference contentReference = PageReference.ParseUrl(linkUrl);
                    if(ContentReference.IsNullOrEmpty(contentReference) == false)
                    {
                        idList.Add(contentReference);
                    }

                }
            }
            return idList;
        }

		private string GetCommerceNodeIds(ShoppingCategoryPage pageData)
	    {
		    List<ContentReference> idList = GetCommerceNodeIdList(pageData);

            if (idList.Any())
            {
                string commerceCategories = string.Empty;
                foreach (ContentReference reference in idList)
                {
                    string id = reference.ID.ToString();

					if (string.IsNullOrEmpty(commerceCategories))
					{
						commerceCategories = id;
					}
					else
					{
						commerceCategories += "," + id;
					}

                }
                return commerceCategories;
            }
            return string.Empty;
	    }
	}
}
