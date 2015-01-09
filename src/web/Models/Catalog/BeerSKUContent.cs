using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.GoogleAnalytics.Models.MetricImplementations;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using OxxCommerceStarterKit.Web.Models.Blocks.Contracts;
using OxxCommerceStarterKit.Web.Models.FindModels;
using OxxCommerceStarterKit.Web.Models.ViewModels;
using OxxCommerceStarterKit.Core.Extensions;

namespace OxxCommerceStarterKit.Web.Models.Catalog
{
    [CatalogContentType(GUID = "55DD1967-CF0E-486E-93FC-C8B6FD0D08A7", MetaClassName = "BeerSKU",
        DisplayName = "Beer", Description = "A variation of beer", GroupName = "Beer") 
    ]
    public class BeerSKUContent : VariationContent, IFacetBrand, IInfoModelNumber, IIndexableContent, IProductListViewModelInitializer
    {
        [DefaultValue(true)]
        public virtual bool ShowInList { get; set; }

        [Display(Name = "IBU",
           Description = "",
           Order = 2)]
        [Searchable]
        public virtual string IBU { get; set; }

        // Same for all languages
        [Display(Name = "Alcohol",
            Description = "The alcoholic percentage",
            Order = 3)]
        [Searchable]
        public virtual decimal Alcohol { get; set; }

        // Same for all languages
        [Display(Name = "Humle",
            Description = "Humletyper",
            Order = 3)]
        [Searchable]
        public virtual string Humle { get; set; }


        [Display(Name = "Model Number", Order = 9)]
        public virtual string Info_ModelNumber { get; set; }

        // Multi lang
        [Display(Name = "Description", Order = 10)]
        [CultureSpecific]
        [Searchable]
        public virtual XhtmlString Beer_Description { get; set; }

        // Same for all languages
        [Display(Name = "Facet Brand",
            Order = 18)]
        public virtual string Facet_Brand { get; set; }

       

        public FindProduct GetFindProduct(IMarket market)
        {
            var language = (Language == null ? string.Empty : Language.Name);
            var findProduct = new BeerFindProduct(this, language);

            findProduct.ShowInList = ShowInList;
            EPiServer.Commerce.SpecializedProperties.Price defaultPrice = this.GetDefaultPrice();
            findProduct.DefaultPrice = this.GetDisplayPrice(market);
            findProduct.DefaultPriceAmount = this.GetDefaultPriceAmount(market);
            findProduct.DiscountedPrice = this.GetDiscountDisplayPrice(defaultPrice, market);
            findProduct.CustomerClubPrice = this.GetCustomerClubDisplayPrice(market);

            return findProduct;
        }

        public bool ShouldIndex()
        {
            return !(StopPublish != null && StopPublish < DateTime.Now);
        }

        public ProductListViewModel Populate(IMarket market)
        {
            UrlResolver urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

            ProductListViewModel productListViewModel = new ProductListViewModel
            {
                Code = this.Code,
                ContentLink = this.ContentLink,
                DisplayName = this.DisplayName,
                Description = Beer_Description,
                ProductUrl = urlResolver.GetUrl(ContentLink),
                ImageUrl = this.GetDefaultImage(),
                PriceString = this.GetDisplayPrice(market),
                BrandName = Facet_Brand,
            };
            var currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            productListViewModel.PriceAmount = this.GetDefaultPriceAmount(currentMarket.GetCurrentMarket());
            return productListViewModel;
        }
    }
}