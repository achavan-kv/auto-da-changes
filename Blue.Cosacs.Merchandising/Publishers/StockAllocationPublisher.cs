using Blue.Cosacs.Merchandising.Models;

namespace Blue.Cosacs.Merchandising.Publishers
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Messages.Merchandising.Transfer;
    using Blue.Cosacs.Messages.Warehouse;
    using Blue.Hub.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IStockAllocationPublisher
    {
        void PublishCreated(List<StockAllocationProductViewModel> stockAllocation);

        void PublishReceived(StockAllocationProductViewModel stockAllocationProducts, int quantityReceived, int? currentBookingId);
    }

    public class StockAllocationPublisher : IStockAllocationPublisher
    {
        private readonly IPublisher publisher;
        private readonly ILocationRepository locationRepository;
        private readonly IProductRepository productRepository;

        public StockAllocationPublisher(IPublisher publisher, ILocationRepository locationRepository, IProductRepository productRepository)
        {
            this.publisher = publisher;
            this.locationRepository = locationRepository;
            this.productRepository = productRepository;
        }

        public void PublishCreated(List<StockAllocationProductViewModel> stockAllocation)
        {
            PublishWarehouseBooking(stockAllocation);
        }

        public void PublishReceived(StockAllocationProductViewModel stockAllocationProduct, int quantityReceived, int? currentBookingId)
        {
            PublishFinancials(stockAllocationProduct, quantityReceived, currentBookingId);
        }

        private void PublishFinancials(StockAllocationProductViewModel stockAllocationProduct, int quantityReceived, int? currentBookingId)
        {
            var awc = quantityReceived * stockAllocationProduct.AverageWeightedCost;
            var product = productRepository.GetProductMessages(new List<int> { stockAllocationProduct.ProductId }).Single();
            var transferProduct = Mapper.Map<TransferMessageProduct>(product);
            transferProduct.Cost = awc;
            transferProduct.Units = quantityReceived;

            var message = new TransferMessage
            {
                Id = stockAllocationProduct.BookingId,
                Type = "Allocation",
                AWC = awc,
                WarehouseLocationId = stockAllocationProduct.WarehouseLocationId,
                WarehouseLocationSalesId = stockAllocationProduct.WarehouseSalesLocationId,
                ReceivingLocationId = stockAllocationProduct.ReceivingLocationId,
                ReceivingLocationSalesId = stockAllocationProduct.ReceivingSalesLocationId,
                CreatedDate = stockAllocationProduct.CreatedDate,
                Products = new[] { transferProduct },
            };

            message.Description = string.Format("SHP#{0}", currentBookingId ?? message.Id);
            publisher.Publish("Merchandising.Transfer", message);
        }

        private void PublishWarehouseBooking(List<StockAllocationProductViewModel> stockAllocationProducts)
        {
            var locationIds = stockAllocationProducts.Select(x => x.ReceivingLocationId)
                .Union(stockAllocationProducts.Select(x => x.WarehouseLocationId)).ToList();
            var locations = locationRepository.Get(locationIds);

            foreach (var product in stockAllocationProducts)
            {
                var receivingLocation = locations.Single(l => l.Id == product.ReceivingLocationId);
                var warehouseLocation = locations.Single(l => l.Id == product.WarehouseLocationId);
                var salesLocationId = short.Parse(warehouseLocation.SalesId);

                var message = new BookingSubmit
                {
                    Recipient = receivingLocation.Name,
                    AddressLine1 = receivingLocation.AddressLine1,
                    AddressLine2 = receivingLocation.AddressLine2,
                    PostCode = receivingLocation.PostCode,
                    Fascia = receivingLocation.Fascia == "Courts" ? "C" : "N",
                    RequestedDate = product.CreatedDate,
                    Type = BookingTypes.Allocation,
                    OrderedOn = product.CreatedDate,
                    Reference = string.Empty,
                    CreatedBy = product.CreatedById,
                    Id = product.BookingId,
                    SKU = product.Sku,
                    ItemId = product.ProductId,
                    ProductDescription = product.Description,
                    Quantity = (short)product.Quantity,
                    UnitPrice = product.AverageWeightedCost,
                    ItemUPC = product.CorporateUPC.SafeSubstring(0, 18),
                    ProductBrand = product.Brand ?? string.Empty,
                    ProductModel = product.Model,
                    ProductCategory = product.Category,
                    Comment = product.Comments,
                    StockBranch = salesLocationId,
                    DeliveryBranch = salesLocationId,
                    ReceivingLocation = Convert.ToInt16(receivingLocation.SalesId),
                    ReceivingLocationSpecified = true,
                };

                publisher.Publish<Context, BookingSubmit>("Warehouse.Booking.Submit", message);
            }
        }
    }
}