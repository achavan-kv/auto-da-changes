using Blue.Cosacs.Merchandising.Models;

namespace Blue.Cosacs.Merchandising.Publishers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Messages.Merchandising.Transfer;
    using Blue.Cosacs.Messages.Warehouse;
    using Blue.Hub.Client;
    using System;

    public interface IStockRequisitionPublisher
    {
        void PublishCreated(List<StockRequisitionProductViewModel> stockRequisitionProducts);

        void PublishReceived(StockRequisitionProductViewModel stockRequisitionProducts, int quantityReceived, int? currentBookingId);
    }

    public class StockRequisitionPublisher : IStockRequisitionPublisher
    {
        private readonly IPublisher publisher;
        private readonly ILocationRepository locationRepository;
        private readonly IProductRepository productRepository;

        public StockRequisitionPublisher(IPublisher publisher, ILocationRepository locationRepository, IProductRepository productRepository)
        {
            this.publisher = publisher;
            this.locationRepository = locationRepository;
            this.productRepository = productRepository;
        }

        public void PublishCreated(List<StockRequisitionProductViewModel> stockRequisitionProducts)
        {
            PublishWarehouseBooking(stockRequisitionProducts);
        }

        public void PublishReceived(StockRequisitionProductViewModel stockRequisitionProduct, int quantityReceived, int? currentBookingId)
        {
            PublishFinancials(stockRequisitionProduct, quantityReceived, currentBookingId);
        }

        private void PublishFinancials(StockRequisitionProductViewModel stockRequisitionProduct, int quantityReceived, int? currentBookingId)
        {
            var awc = (decimal)(quantityReceived * stockRequisitionProduct.AverageWeightedCost);
            var product = productRepository.GetProductMessages(new List<int> { stockRequisitionProduct.ProductId }).Single();
            var transferProduct = Mapper.Map<TransferMessageProduct>(product);
            transferProduct.Cost = awc;
            transferProduct.Units = quantityReceived;

            var message = new TransferMessage
            {
                Id = stockRequisitionProduct.BookingId,
                Type = "Requisition",
                AWC = awc,
                WarehouseLocationId = stockRequisitionProduct.WarehouseLocationId,
                WarehouseLocationSalesId = stockRequisitionProduct.WarehouseSalesLocationId,
                ReceivingLocationId = stockRequisitionProduct.ReceivingLocationId,
                ReceivingLocationSalesId = stockRequisitionProduct.ReceivingSalesLocationId,
                CreatedDate = stockRequisitionProduct.CreatedDate,
                Products = new[] { transferProduct },
            };

            message.Description = string.Format("SHP#{0}", currentBookingId.HasValue ? currentBookingId.Value : message.Id);
            publisher.Publish("Merchandising.Transfer", message);
        }

        private void PublishWarehouseBooking(List<StockRequisitionProductViewModel> stockRequisitionProducts)
        {
            var locationIds = stockRequisitionProducts.Select(x => x.ReceivingLocationId)
                    .Union(stockRequisitionProducts.Select(x => x.WarehouseLocationId)).ToList();
            var locations = locationRepository.Get(locationIds);

            foreach (var product in stockRequisitionProducts)
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
                    Type = BookingTypes.Requisition,
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
                    ProductCategory = product.Category ?? string.Empty,
                    Comment = product.Comments,
                    StockBranch = salesLocationId,
                    DeliveryBranch = salesLocationId,
                    ReceivingLocation = Convert.ToInt16(receivingLocation.SalesId),
                    ReceivingLocationSpecified = true, // thank you for your attrocious XML serializer MS
                };

                publisher.Publish<Context, BookingSubmit>("Warehouse.Booking.Submit", message);
            }
        }
    }
}