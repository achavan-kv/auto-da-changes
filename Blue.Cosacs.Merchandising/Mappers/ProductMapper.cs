namespace Blue.Cosacs.Merchandising.Mappers
{
    using System.Linq;
    using Blue.Admin;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Infrastructure;

    using Models;
    using Repositories;
    using System.Collections.Generic;


    public class ProductMapper : IProductMapper
    {
        private readonly MerchandisingHierarchyRepository hierarchyRepository;
        private readonly IProductRepository productRepository;
        private readonly IProductStatusRepository productStatusRepository;
        private readonly IProductStockRepository productStockRepository;
        private readonly ITaxRepository taxRepository;
        private readonly ICostRepository costRepository;
        private readonly IMerchandisingConfiguration config;
        private readonly ILocationRepository locationRepository;
        private readonly IProductSalesRepository productSalesRepository;        
        private readonly IPromotionRepository promotionRepository;

        private readonly IBrandRepository brandRepository;

        private readonly IRetailPriceRepository retailPriceRepository;

        private readonly IProductRangeRepository productRangeRepository;
        private readonly IAshleySetUpRepository ashleySetUpRepository;
        private readonly IAdditionalCostRepository additionalCostRepository;
        public ProductMapper(
            MerchandisingHierarchyRepository hierarchyRepository, 
            IProductRepository productRepository, 
            IProductStatusRepository productStatusRepository,
            IProductStockRepository productStockRepository,
            ITaxRepository taxRepository,
            IMerchandisingConfiguration config,
            ICostRepository costRepository,
            ILocationRepository locationRepository,
            IRetailPriceRepository retailPriceRepository,
            IBrandRepository brandRepository,
            IProductSalesRepository productSalesRepository,
            IPromotionRepository promotionRepository,
            IProductRangeRepository productRangeRepository,
            IAshleySetUpRepository ashleySetUpRepository,
            IAdditionalCostRepository additionalCostRepository)
        {
            this.hierarchyRepository = hierarchyRepository;
            this.productRepository = productRepository;
            this.productStatusRepository = productStatusRepository;
            this.productStockRepository = productStockRepository;
            this.taxRepository = taxRepository;
            this.costRepository = costRepository;
            this.config = config;
            this.locationRepository = locationRepository;
            this.retailPriceRepository = retailPriceRepository;
            this.brandRepository = brandRepository;
            this.productSalesRepository = productSalesRepository;
            this.promotionRepository = promotionRepository;
            this.productRangeRepository = productRangeRepository;
            this.ashleySetUpRepository = ashleySetUpRepository;
            this.additionalCostRepository = additionalCostRepository;
        }

        public ProductViewModel CreateViewModel(Product product, UserSession user)
        {
            if (product.Id.HasValue)
            {
                product.StockLevel = productStockRepository.Get(product.Id.Value);
                product.TaxRates = taxRepository.GetByProduct(product.Id.Value);
                product.CostPrice = costRepository.GetLatestByProduct(product.Id.Value);
                product.Sales = productSalesRepository.Get(product.Id.Value, false, null);

                if (user.HasPermission(MerchandisingPermissionEnum.RetailPriceView))
                {
                    switch (product.ProductType)
                    {
                        case ProductTypes.RegularStock:
                        case ProductTypes.SparePart:
                        case ProductTypes.ProductWithoutStock:
                            {
                                product.RetailPrices = retailPriceRepository.GetCurrentAndPendingForProduct(product.Id.Value);
                                break;
                            }
                    }
                }

                if (user.HasPermission(MerchandisingPermissionEnum.IncotermView))
                {
                    switch (product.ProductType)
                    {
                        case ProductTypes.RegularStock:
                        case ProductTypes.ProductWithoutStock:
                        {
                            product.Incoterms = retailPriceRepository.GetIncoterms(product.Id.Value);
                            break;
                        }
                    }
                }

                ////Binding Data to Ashley product range, product attribute, and multiple costprice
                product.ProductStockRanges = (productRangeRepository.Get(product.Id.Value));
                product.ProductAttributes = (ashleySetUpRepository.Get(product.Id.Value));
                product.AddtionalCPs = additionalCostRepository.GetByProduct(product.Id.Value);
                //
                var hierarchy = productRepository.GetProductHierarchyJson(product.Id.Value);
            
                product.Promotions = promotionRepository.GetForProduct(product.Id.Value, product.SKU, hierarchy, product.RetailPrices);
            }
            else
            {
                product.TaxRates = new List<TaxRateModel>();
            }

            var hierarchyOptions = hierarchyRepository.GetSortedList().ToList();

            return new ProductViewModel
            {
                Product = product,
                Locations = locationRepository.GetDictionary(),
                HierarchyOptions = hierarchyOptions,
                Statuses = productStatusRepository.Get().ToList(),
                IsMasterInstance = config.IsMaster,
                Brands = brandRepository.GetAll()
            };
        }

        public RepossessedProductViewModel CreateRepossessedViewModel(Product product)
        {
            product.TaxRates = taxRepository.GetByProduct(product.Id.Value);
            product.RetailPrices = retailPriceRepository.GetForRepossessedProduct(product.Id.Value);
            product.CostPrice = costRepository.GetLatestByProduct(product.Id.Value);
            product.StockLevel = productStockRepository.Get(product.Id.Value);
            product.Sales = productSalesRepository.Get(product.Id.Value, false, null);
                
            return new RepossessedProductViewModel
            {
                Product = product,
                HierarchyOptions = hierarchyRepository.GetSortedList(),
                RepossessionDetails = productRepository.GetRepossessionDetails(product != null ? product.Id : null),
                Statuses = productStatusRepository.Get(),
                Brands = brandRepository.GetAll()
            };
        }

        public SetViewModel CreateSetViewModel(SetModel set)
        {
            var hierarchyOptions = hierarchyRepository.GetSortedList().ToList();

            return new SetViewModel
            {
                Set = set,
                Statuses = productStatusRepository.Get(),
                Locations = locationRepository.GetDictionary(),
                HierarchyOptions = hierarchyRepository.GetSortedList()
            };
        }

        public ComboViewModel CreateComboViewModel(ComboModel combo)
        {
            return new ComboViewModel
            {
                Combo = combo,
                Statuses = productStatusRepository.Get(),
                Locations = locationRepository.GetDictionary(),
                HierarchyOptions = hierarchyRepository.GetSortedList()
            };
        }
    }
}
