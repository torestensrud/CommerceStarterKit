/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using EPiServer.Commerce.Catalog;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Localization;
using EPiServer.Framework.Web.Mvc;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Pricing;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    [RequireClientResources]
    public class BeerSKUContentController : CommerceControllerBase<BeerSKUContent>
    {
        private readonly ICurrentMarket _currentMarket;
        private IWarehouseInventoryService _warehouseInventoryService;
        private LocalizationService _localizationService;
        private ReadOnlyPricingLoader _readOnlyPricingLoader;
        private readonly IPriceDetailService _priceDetailService;

        public BeerSKUContentController()
			: this(ServiceLocator.Current.GetInstance<IWarehouseInventoryService>(),
			ServiceLocator.Current.GetInstance<LocalizationService>(),
			ServiceLocator.Current.GetInstance<ReadOnlyPricingLoader>(),
			ServiceLocator.Current.GetInstance<ICurrentMarket>(),
            ServiceLocator.Current.GetInstance<IPriceDetailService>()
			)
		{
		}

        public BeerSKUContentController(IWarehouseInventoryService warehouseInventoryService, LocalizationService localizationService, ReadOnlyPricingLoader readOnlyPricingLoader, ICurrentMarket currentMarket, IPriceDetailService priceDetailService)
        {
            _warehouseInventoryService = warehouseInventoryService;
            _localizationService = localizationService;
            _readOnlyPricingLoader = readOnlyPricingLoader;
            _currentMarket = currentMarket;
            _priceDetailService = priceDetailService;
        }


        // GET: BeerSKUContent
        public ActionResult Index(BeerSKUContent currentContent)
        {
            BeerSKUViewModel beerSkuViewModel = new BeerSKUViewModel(currentContent);
            beerSkuViewModel.PriceViewModel = GetPriceModel(currentContent);

            //TrackAnalytics(beerSkuViewModel);

            beerSkuViewModel.IsSellable = IsSellable(currentContent);
            return View(beerSkuViewModel);
        }

    }
}
