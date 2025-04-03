using Blue.Cosacs.Stock.Repositories;
using Blue.Cosacs.Warranty.Model;
using Blue.Cosacs.Warranty.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Merch = Blue.Cosacs.Merchandising;

namespace Blue.Cosacs.Warranty.Subscribers
{
    public class WarrantySaleSubmit : Hub.Client.Subscriber
    {
        private IClock clock;
        private readonly WarrantySaleRepository warrantySaleRepository;
        private readonly WarrantyLinkRepository warrantyLinkRepository;
        private readonly WarrantyPotentialSaleRepository warrantyPotentialSaleRepository;
        private readonly Merch.Settings merchandisingSettings;
        private readonly Merch.Repositories.TaxRepository merchandisingTaxRepo;

        public WarrantySaleSubmit()
        {
            var eventStore = new EventStore("Default");
            var config = new Config.Repositories.Settings();
            var cache = new Blue.Caching.MemoryCacheClient();

            merchandisingSettings = new Merch.Settings();
            merchandisingTaxRepo = new Merch.Repositories.TaxRepository(eventStore);

            this.clock = new DateTimeClock();
            this.warrantySaleRepository = new WarrantySaleRepository();
            this.warrantyLinkRepository = new WarrantyLinkRepository(eventStore,
                clock,
                config,
                new WarrantyPriceRepository(eventStore, clock, config, cache, merchandisingSettings, merchandisingTaxRepo),
                new WarrantyPromotionRepository(eventStore, clock),
                new ProductRepository(),
                merchandisingTaxRepo);

            this.warrantyPotentialSaleRepository = new WarrantyPotentialSaleRepository();
        }

        public override void Sink(int id, System.Xml.XmlReader message)
        {
            var saleMessage = Deserialize<Blue.Cosacs.Messages.Warranty.SalesOrder>(message);

            if (saleMessage.Customer.CustomerId.Equals("ServiceStock", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var SaleOrderExtracted = GetSaleOrderFromMessage(saleMessage);
            var saleOrderOriginal = warrantySaleRepository.Save(SaleOrderExtracted);

            SaveWarrantyPotentialSales(saleMessage);
        }

        private void SaveWarrantyPotentialSales(Blue.Cosacs.Messages.Warranty.SalesOrder saleMessage)
        {
            var search = new Model.WarrantySearchByProduct
            {
                Product = saleMessage.Item.Number,
                PriceVATEx = saleMessage.Item.Price,
                Location = saleMessage.SaleBranch,
                Date = clock.UtcNow
            };

            var warrantyLinks = warrantyLinkRepository.SearchByProduct(search);
            if (warrantyLinks != null)
            {
                var warrantyPotentialSaleQuantity = this.warrantySaleRepository.GetWarrantyPotentialSaleQuantity(saleMessage);

                warrantyPotentialSaleRepository.Save(
                    warrantyLinks.Items
                        .Select(p => GetPotentialSaleFromMessageAndWarrantyLink(saleMessage, p, warrantyPotentialSaleQuantity))
                        .ToList());
            }
        }

        private Model.WarrantyPotentialSaleModel GetPotentialSaleFromMessageAndWarrantyLink(Blue.Cosacs.Messages.Warranty.SalesOrder saleMessage, WarrantyLinkResult warrantyLink, int warrantyPotentialSaleQuantity)
        {
            var potentialSaleQuantity = warrantyLink.warrantyLink.TypeCode == WarrantyType.Free ? 0 : warrantyPotentialSaleQuantity;

            var potentialSale = new Model.WarrantyPotentialSaleModel
            {
                InvoiceNumber = saleMessage.InvoiceNumber,
                AccountNumber = saleMessage.Customer.AccountNumber,
                SaleBranch = saleMessage.SaleBranch,
                SoldBy = new Model.WarrantyPotentialSaleModel.SoldByUser
                {
                    SoldById = saleMessage.SoldBy.SoldById
                },
                SoldOn = saleMessage.SoldOn,
                DeliveredOn = saleMessage.DeliveredOn,
                CustomerId = saleMessage.Customer.CustomerId,
                ItemId = saleMessage.Item.Id,
                ItemNumber = saleMessage.Item.Number,
                ItemPrice = saleMessage.Item.Price,
                ItemCostPrice = saleMessage.Item.CostPrice,
                Warranty = new Model.WarrantyPotentialSaleModel.ItemWarranty
                {
                    WarrantyCostPrice = warrantyLink.price != null && warrantyLink.price.CostPrice.HasValue ? warrantyLink.price.CostPrice.Value : 0,
                    WarrantyId = warrantyLink.warrantyLink.Id,
                    WarrantyType = warrantyLink.warrantyLink.TypeCode,
                    WarrantyLength = warrantyLink.warrantyLink.Length,
                    WarrantyNumber = warrantyLink.warrantyLink.Number,
                    WarrantyRetailPrice = warrantyLink.price != null && warrantyLink.price.RetailPrice.HasValue ? warrantyLink.price.RetailPrice.Value : 0,
                    WarrantyTaxRate = warrantyLink.warrantyLink.TaxRate,
                    WarrantySalePrice = (warrantyLink.promotion != null && warrantyLink.promotion.Price.HasValue) ? warrantyLink.promotion.Price.Value : 0
                },
                IsItemReturned = false,
                SecondEffort = saleMessage.SecondEffort,
                Quantity = potentialSaleQuantity
            };

            return potentialSale;
        }

        private Model.WarrantySaleOrder GetSaleOrderFromMessage(Blue.Cosacs.Messages.Warranty.SalesOrder saleMessage)
        {
            var saleOrder = new Model.WarrantySaleOrder
            {
                InvoiceNumber = saleMessage.InvoiceNumber,
                SaleBranch = saleMessage.SaleBranch,
                SoldBy = new Model.WarrantySaleOrder.SoldByUser
                {
                    SoldById = saleMessage.SoldBy.SoldById,
                    Value = saleMessage.SoldBy.Value,
                },
                SoldOn = saleMessage.SoldOn,
                DeliveredOn = saleMessage.DeliveredOn,
                CustomerAccount = saleMessage.Customer.AccountNumber,
                CustomerId = saleMessage.Customer.CustomerId,
                CustomerTitle = saleMessage.Customer.Title,
                CustomerFirstName = saleMessage.Customer.FirstName,
                CustomerLastName = saleMessage.Customer.LastName,
                CustomerAddressLine1 = saleMessage.Customer.AddressLine1,
                CustomerAddressLine2 = saleMessage.Customer.AddressLine2,
                CustomerAddressLine3 = saleMessage.Customer.AddressLine3,
                CustomerPostcode = saleMessage.Customer.PostCode,
                CustomerNotes = saleMessage.Customer.Notes,
                ItemBrand = saleMessage.Item.Brand,
                ItemDescription = saleMessage.Item.Description,
                ItemId = saleMessage.Item.Id,
                ItemModel = saleMessage.Item.Model,
                ItemNumber = saleMessage.Item.Number,
                ItemPrice = saleMessage.Item.Price,
                ItemCostPrice = saleMessage.Item.CostPrice,
                ItemSupplier = saleMessage.Item.Supplier,
                ItemUPC = saleMessage.Item.UPC,
                ItemStockLocation = saleMessage.Item.StockLocation,
                ItemQuantity = saleMessage.Item.Quantity

            };

            var warranties = new List<Model.WarrantySaleOrder.ItemWarranty>();
            if (saleMessage.Item.Warranty != null)
            {
                foreach (var warranty in saleMessage.Item.Warranty)
                {
                    warranties.Add(new Model.WarrantySaleOrder.ItemWarranty
                    {
                        WarrantyContractNo = warranty.ContractNumber,
                        WarrantyCostPrice = warranty.CostPrice,
                        WarrantyItemId = warranty.Id,
                        WarrantyType = warranty.TypeCode,
                        WarrantyLength = warranty.Length,
                        WarrantyNumber = warranty.Number,
                        WarrantyRetailPrice = warranty.RetailPrice,
                        WarrantySalePrice = warranty.SalePrice,
                        WarrantyTaxRate = warranty.TaxRate,
                        WarrantyStatus = "Active",
                        WarrantyGroupId = warranty.WarrantyGroupId,
                        WarrantyEffectiveDate = warranty.EffectiveDate,
                        RedeemContractNo = warranty.RedeemContractNumber,
                        ReLinkContractNo = warranty.ReLinkContractNo,
                        WarrantyDeliveredOn = warranty.WarrantyDeliveredOn
                    });
                }
            }

            saleOrder.Warranty = warranties.ToArray();

            var contacts = new List<Blue.Cosacs.Warranty.Model.WarrantySaleOrder.ContactItem>();
            if (!string.IsNullOrWhiteSpace(saleMessage.Customer.HomePhone))
            {
                contacts.Add(new Model.WarrantySaleOrder.ContactItem { Type = "HomePhone", Value = saleMessage.Customer.HomePhone });
            }
            if (!string.IsNullOrWhiteSpace(saleMessage.Customer.WorkPhone))
            {
                contacts.Add(new Model.WarrantySaleOrder.ContactItem { Type = "WorkPhone", Value = saleMessage.Customer.WorkPhone });
            }
            if (!string.IsNullOrWhiteSpace(saleMessage.Customer.MobilePhone))
            {
                contacts.Add(new Model.WarrantySaleOrder.ContactItem { Type = "MobilePhone", Value = saleMessage.Customer.MobilePhone });
            }
            if (!string.IsNullOrWhiteSpace(saleMessage.Customer.Email))
            {
                contacts.Add(new Model.WarrantySaleOrder.ContactItem { Type = "Email", Value = saleMessage.Customer.Email });
            }

            saleOrder.Contacts = contacts.ToArray();

            return saleOrder;
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
