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

    public interface IStockTransferPublisher
    {
        void PublishCreated(StockTransferViewModel stockTransfer);
        void PublishFinancials(StockTransferViewModel stockTransfer, IEnumerable<KeyValuePair<int, int>> productQuantities);
        void PublishProductFinancials(StockTransferViewModel stockTransfer, StockTransferProductViewModel stockTransferProduct, int quantityReceived, int? currentBookingId);
    }

    public class StockTransferPublisher : IStockTransferPublisher
    {
        private readonly IPublisher publisher;
        private readonly ILocationRepository locationRepository;
        private readonly IProductRepository productRepository;

        public StockTransferPublisher(
            IPublisher publisher,
            ILocationRepository locationRepository,
            IProductRepository productRepository)
        {
            this.publisher = publisher;
            this.locationRepository = locationRepository;
            this.productRepository = productRepository;
        }

        public void PublishCreated(StockTransferViewModel stockTransfer)
        {
            PublishWarehouseBooking(stockTransfer);
        }

        public void PublishFinancials(StockTransferViewModel stockTransfer, IEnumerable<KeyValuePair<int, int>> productQuantities)
        {
            var message = Mapper.Map<TransferMessage>(stockTransfer);

            message.WarehouseLocationId = stockTransfer.SendingLocationId;
            message.WarehouseLocationSalesId = stockTransfer.SendingLocationSalesId;

            if (stockTransfer.ViaLocationId.HasValue && stockTransfer.ViaLocationId.Value != 0)
            {
                message.ReceivingLocationId = stockTransfer.ViaLocationId.Value;
                message.ReceivingLocationSalesId = stockTransfer.ViaLocationSalesId;
            }
            else
            {
                message.ReceivingLocationId = stockTransfer.ReceivingLocationId;
                message.ReceivingLocationSalesId = stockTransfer.ReceivingLocationSalesId;
            }

            var products = productRepository.GetProductMessages(stockTransfer.Products.Select(p => p.ProductId).Distinct().ToList());
            message.Products = Mapper.Map<List<TransferMessageProduct>>(products).ToArray();

            foreach (var product in message.Products)
            {
                var productList = stockTransfer.Products.Where(p2 => p2.ProductId == product.Id).ToList();
                product.Cost = productList.Sum(p => p.AverageWeightedCost * productQuantities.First(q => q.Key == p.ProductId).Value).Value;
                product.Units = productList.Sum(u => u.Quantity);
            }

            publisher.Publish("Merchandising.Transfer", message);
        }

        public void PublishProductFinancials(StockTransferViewModel stockTransfer, StockTransferProductViewModel stockTransferProduct, int quantityReceived, int? currentBookingId)
        {
            var awc = quantityReceived * stockTransferProduct.AverageWeightedCost;
            var product = productRepository.GetProductMessages(new List<int> { stockTransferProduct.ProductId }).Single();
            var transferProduct = Mapper.Map<TransferMessageProduct>(product);
            transferProduct.Cost = awc.HasValue ? awc.Value : 0;
            transferProduct.Units = quantityReceived;

            var message = new TransferMessage
            {
                Id = stockTransfer.Id,
                Type = "Transfer",
                AWC = awc.HasValue ? awc.Value : 0,
                WarehouseLocationId = stockTransfer.ViaLocationId.Value,
                WarehouseLocationSalesId = stockTransfer.ViaLocationSalesId,
                ReceivingLocationId = stockTransfer.ReceivingLocationId,
                ReceivingLocationSalesId = stockTransfer.ReceivingLocationSalesId,
                CreatedDate = DateTime.UtcNow,
                Products = new[] { transferProduct },
            };
            message.Description = string.Format("ST#{0}", currentBookingId ?? message.Id);
            publisher.Publish("Merchandising.Transfer", message);
        }

        private void PublishWarehouseBooking(StockTransferViewModel stockTransfer)
        {
            if (stockTransfer.ViaLocationId == null)
            {
                throw new InvalidOperationException(
                    "Only stock transfers that have a 'Via Location' should be sent to the Warehouse module.");
            }

            var receivingLocation = locationRepository.Get(stockTransfer.ReceivingLocationId);

            var message = new BookingSubmit
            {
                Recipient = receivingLocation.Name,
                AddressLine1 = receivingLocation.AddressLine1,
                AddressLine2 = receivingLocation.AddressLine2,
                PostCode = receivingLocation.PostCode,
                Fascia = receivingLocation.Fascia == "Courts" ? "C" : "N",
                RequestedDate = stockTransfer.CreatedDate,
                Type = BookingTypes.Transfer,
                OrderedOn = stockTransfer.CreatedDate,
                Reference = stockTransfer.Id.ToString(),
                CreatedBy = stockTransfer.CreatedById,
                ReceivingLocation = Convert.ToInt16(receivingLocation.SalesId)
            };

            var location = locationRepository.Get(stockTransfer.ViaLocationId.Value);
            short salesLocationId;
            if (!short.TryParse(location.SalesId, out salesLocationId))
            {
                throw new InvalidOperationException(
                    string.Format("The location with sales Id '{0}' should be a number between 1 and {1}", location.SalesId, short.MaxValue));
            }
            message.SalesBranch = message.StockBranch = message.DeliveryBranch = salesLocationId;

            foreach (var product in stockTransfer.Products)
            {
                message.Id = product.BookingId;
                message.SKU = product.Sku;
                message.ItemId = product.ProductId;
                message.ProductDescription = product.Description;
                message.Quantity = (short)product.Quantity;
                message.UnitPrice = product.AverageWeightedCost.Value;
                message.ItemUPC = product.CorporateUPC;
                message.ProductBrand = product.Brand ?? string.Empty;
                message.ProductModel = product.Model;
                message.ProductCategory = product.Category;
                message.Comment = product.Comments;
                message.ReceivingLocation = Convert.ToInt16(receivingLocation.SalesId);
                message.ReceivingLocationSpecified = true;

                publisher.Publish<Context, BookingSubmit>("Warehouse.Booking.Submit", message);
            }
        }
    }
}