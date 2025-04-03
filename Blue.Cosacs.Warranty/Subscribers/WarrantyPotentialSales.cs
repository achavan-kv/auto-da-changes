using System.Collections.Generic;
using Blue.Cosacs.Warranty.Repositories;
using System;
using Blue.Cosacs.Stock.Repositories;
using Blue.Caching;
using Merch = Blue.Cosacs.Merchandising;

namespace Blue.Cosacs.Warranty.Subscribers
{
    public class WarrantyPotentialSales : Hub.Client.Subscriber
    {
        private IClock clock;
        private readonly WarrantyLinkRepository warrantyLinkRepository;
        private WarrantyPotentialSaleRepository warrantyPotentialSaleRepository;
        private readonly Merch.Settings merchandisingSettings;
        private readonly Merch.Repositories.TaxRepository merchandisingTaxRepo;

        public WarrantyPotentialSales()
        {
            var eventStore = new EventStore("Default");
            var config = new Config.Repositories.Settings();
            var cache = new Blue.Caching.MemoryCacheClient();

            this.clock = new DateTimeClock();
            this.warrantyPotentialSaleRepository = new WarrantyPotentialSaleRepository();
            this.warrantyLinkRepository = new WarrantyLinkRepository(eventStore,
                clock,
                config,
                new WarrantyPriceRepository(eventStore, clock, config, cache, merchandisingSettings, merchandisingTaxRepo),
                new WarrantyPromotionRepository(eventStore, clock),
                new ProductRepository(),
                merchandisingTaxRepo);
        }

        public override void Sink(int id, System.Xml.XmlReader message)
        {
            var saleMessage = Deserialize<Blue.Cosacs.Messages.Warranty.PotentialSales>(message);
            var potentialSales = GetPotentialSales(saleMessage);
            warrantyPotentialSaleRepository.Add(potentialSales);
        }

        private List<Model.WarrantyPotentialSaleModel> GetPotentialSales(Blue.Cosacs.Messages.Warranty.PotentialSales potentialSalesMessage)
        {
            var potentialSales = new List<Model.WarrantyPotentialSaleModel>();
            if (potentialSalesMessage.Items == null)
            {
                return potentialSales;
            }

            foreach (var item in potentialSalesMessage.Items)
            {
                var warrantyLinks = GetAvailableWarranties(potentialSalesMessage, item);
                if (warrantyLinks == null)
                {
                    continue;
                }

                foreach (var warrantyLink in warrantyLinks.Items)
                {
                    potentialSales.Add(GetWarrantyPotentialSaleModel(potentialSalesMessage, item, warrantyLink));
                }
            }

            return potentialSales;
        }

        private Model.WarrantySearchByProductResult GetAvailableWarranties(Blue.Cosacs.Messages.Warranty.PotentialSales potentialSalesMessage, Blue.Cosacs.Messages.Warranty.Item item)
        {
            var search = new Model.WarrantySearchByProduct
            {
                Product = item.Number,
                PriceVATEx = item.Price,
                Location = potentialSalesMessage.SaleBranch,
                Date = clock.UtcNow
            };

            var warrantyLinks = warrantyLinkRepository.SearchByProduct(search);
            return warrantyLinks;
        }

        private static Model.WarrantyPotentialSaleModel GetWarrantyPotentialSaleModel(Blue.Cosacs.Messages.Warranty.PotentialSales potentialSalesMessage, Blue.Cosacs.Messages.Warranty.Item item, Model.WarrantyLinkResult warrantyLink)
        {
            return new Model.WarrantyPotentialSaleModel
            {
                AccountNumber = potentialSalesMessage.AccountNumber,                   //#18534
                InvoiceNumber = potentialSalesMessage.InvoiceNumber,
                SaleBranch = potentialSalesMessage.SaleBranch,
                SoldBy = new Model.WarrantyPotentialSaleModel.SoldByUser
                {
                    SoldById = potentialSalesMessage.SoldBy
                },
                SoldOn = potentialSalesMessage.SoldOn,
                DeliveredOn = potentialSalesMessage.DeliveredOn,
                CustomerId = potentialSalesMessage.Customer.CustomerId,
                ItemId = item.Id,
                ItemNumber = item.Number,
                ItemPrice = item.Price,
                ItemCostPrice = item.CostPrice,
                IsSecondEffort = potentialSalesMessage.SecondEffort,
                Warranty = new Model.WarrantyPotentialSaleModel.ItemWarranty
                {
                    WarrantyCostPrice = warrantyLink.price != null ? Convert.ToDecimal(warrantyLink.price.CostPrice) : 0,       // #16795 ??
                    WarrantyId = warrantyLink.warrantyLink.Id,
                    WarrantyType = warrantyLink.warrantyLink.TypeCode,
                    WarrantyLength = warrantyLink.warrantyLink.Length,
                    WarrantyNumber = warrantyLink.warrantyLink.Number,
                    WarrantyRetailPrice = warrantyLink.price != null ? Convert.ToDecimal(warrantyLink.price.RetailPrice) : 0,       // #16795 ??
                    WarrantyTaxRate = warrantyLink.warrantyLink.TaxRate,
                    WarrantySalePrice = (warrantyLink.promotion != null && warrantyLink.promotion.Price.HasValue) ? warrantyLink.promotion.Price.Value : 0
                },
                IsItemReturned = false,
                SecondEffort = potentialSalesMessage.SecondEffort       // #17609
            };
        }

        private class EventStore : Blue.Events.EventStoreBase
        {
            public EventStore(string connectionStringName) : base(connectionStringName) { }

            protected override string EventBy
            {
                get
                {
                    return "HubMessage";
                }
            }
        }
    }
}
