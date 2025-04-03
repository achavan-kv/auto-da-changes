namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Blue.Cosacs.Messages.Merchandising.BookingMessage;
    using Blue.Events;
    using Blue.Hub.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IStockRequisitionRepository
    {
        StockRequisitionProductViewModel Get(int id);

        StockRequisitionProductViewModel GetByBooking(int bookingId);

        List<StockRequisitionProductViewModel> Get(IEnumerable<int> ids);

        List<StockRequisitionProductViewModel> Create(StockRequisitionCreateModel model, int userId, Func<int> getNextBookingId);

        void Cancel(BookingMessage message);

        void Receive(BookingMessage message);

        StockRequisitionOnOrderViewModel GetOnOrder(int productId, int locationId);

        StockRequisitionOnOrderViewModel GetPendingPurchaseQuantity(int productId, int locationId);

        StockRequisitionStockInfoModel GetStockInfo(int productId, int receivingLocationId, int warehouseLocationId);

        PagedSearchResult<StockRequisitionSearchResultModel> Search(StockRequisitionQueryModel model, int pageSize, int pageIndex);
    }

    public class StockRequisitionRepository : IStockRequisitionRepository
    {
        private readonly IEventStore audit;
        private readonly ICostRepository costRepository;
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IStockRequisitionPublisher publisher;
        private readonly IProductRepository productRepository;

        public StockRequisitionRepository(IEventStore audit, ICostRepository costRepository, IPurchaseRepository purchaseRepository, IStockRequisitionPublisher publisher, IProductRepository productRepository)
        {
            this.audit = audit;
            this.costRepository = costRepository;
            this.publisher = publisher;
            this.productRepository = productRepository;
            this.purchaseRepository = purchaseRepository;
        }

        public StockRequisitionProductViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.StockRequisitionProductView.AsNoTracking()
                    .Project().To<StockRequisitionProductViewModel>()
                    .Single(x => x.Id == id);
            }
        }

        public StockRequisitionProductViewModel GetByBooking(int bookingId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.StockRequisitionProductView.AsNoTracking()
                    .Project().To<StockRequisitionProductViewModel>()
                    .Single(x => x.BookingId == bookingId);
            }
        }

        public List<StockRequisitionProductViewModel> Get(IEnumerable<int> ids)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.StockRequisitionProductView.AsNoTracking()
                    .Where(x => ids.Contains(x.Id))
                    .Project().To<StockRequisitionProductViewModel>()
                    .ToList();
            }
        }

        public List<StockRequisitionProductViewModel> Create(StockRequisitionCreateModel model, int userId, Func<int> getNextBookingId)
        {
            var now = DateTime.UtcNow;

            using (var scope = Context.Write())
            {
                var costs = costRepository.GetCurrentByProducts(model.Products.Select(p => p.ProductId.Value).ToList());

                var products = model.Products.Select(p =>
                {
                    var product = Mapper.Map<StockRequisitionProduct>(p);
                    product.BookingId = getNextBookingId();
                    product.CreatedDate = now;
                    product.CreatedById = userId;
                    product.WarehouseLocationId = model.WarehouseLocationId;
                    product.ReceivingLocationId = model.ReceivingLocationId;
                    product.AverageWeightedCost = costs.FirstOrDefault(c => c.ProductId == p.ProductId).AverageWeightedCost;
                    return product;
                }).ToList();
                scope.Context.StockRequisitionProduct.AddRange(products);
                scope.Context.SaveChanges();

                var viewModels = Get(products.Select(p => p.Id));
                publisher.PublishCreated(viewModels);
                audit.LogAsync(viewModels, StockRequisitionEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return viewModels;
            }
        }

        public void Cancel(BookingMessage message)
        {
            using (var scope = Context.Write())
            {
                if (message.Quantity < 0)
                {
                    throw new MessageValidationException("Cannot cancel a negative quantity", null);
                }

                var product = scope.Context.StockRequisitionProduct.Single(p => p.BookingId == message.BookingId);
                product.QuantityCancelled = message.Quantity;

                if (product.QuantityPending < 0)
                {
                    throw new MessageValidationException("Quantity to cancel is greater than the pending quantity", null);
                }

                if (product.QuantityPending == 0)
                {
                    product.CompletedOn = DateTime.UtcNow;
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void Receive(BookingMessage message)
        {
            using (var scope = Context.Write())
            {
                if (message.Quantity < 0)
                {
                    throw new MessageValidationException("Cannot receive a negative quantity", null);
                }

                var product = scope.Context.StockRequisitionProduct.Single(p => p.BookingId == message.BookingId);
                product.QuantityReceived += message.Quantity;

                if (product.QuantityPending < 0)
                {
                    throw new MessageValidationException("Quantity to receive is greater than the pending quantity", null);
                }

                if (product.QuantityPending == 0)
                {
                    product.CompletedOn = DateTime.UtcNow;
                }

                var movement = new StockMovementModel
                {
                    ProductId = product.ProductId,
                    Quantity = message.Quantity,
                    SendingLocationId = product.WarehouseLocationId,
                    ReceivingLocationId = product.ReceivingLocationId,
                };

                productRepository.AdjustStock(new List<StockMovementModel> { movement });

                scope.Context.StockTransferMovement.Add(new StockTransferMovement
                {
                    BookingId = product.BookingId,
                    ProductId = product.ProductId,
                    ReceivingLocationId = movement.ReceivingLocationId,
                    SendingLocationId = movement.SendingLocationId,
                    DateProcessed = DateTime.UtcNow,
                    Quantity = message.Quantity,
                    Type = "Requisition",
                    AverageWeightedCost = product.AverageWeightedCost
                });

                scope.Context.SaveChanges();

                publisher.PublishReceived(GetByBooking(message.BookingId), message.Quantity, message.CurrentBookingId);
                scope.Complete();
            }
        }
                
        public StockRequisitionOnOrderViewModel GetPendingPurchaseQuantity(int productId, int locationId)
        {
            using (var scope = Context.Read())
            {
                var prod = scope.Context.Product.Single(p => p.Id == productId);
                var loc = scope.Context.Location.Single(l => l.Id == locationId);
                var purchaseOrders = scope.Context.PendingStockOnOrderView
                                        .Where(p => p.LocationId == locationId
                                                && p.ProductId == productId);

                var model = new StockRequisitionOnOrderViewModel
                {
                    Id = productId,
                    Sku = prod.SKU,
                    Description = prod.LongDescription,
                    LocationId = locationId,
                    Location = loc.Name
                };

                purchaseOrders.Each(po => 
                    model.StockItems.Add(new StockRequisitionOnOrderProductViewModel
                    {
                        Id = po.PurchaseOrderId,
                        Date = po.DeliveryDate,
                        Movement = po.PendingStock.Value,
                        Total = po.PendingStock.Value
                    }));

                return model;
            }
        }

        public StockRequisitionOnOrderViewModel GetOnOrder(int productId, int locationId)
        {
            using (var scope = Context.Read())
            {
                var prod = scope.Context.Product.Single(p => p.Id == productId);
                var loc = scope.Context.Location.Single(l => l.Id == locationId);
                var purchaseOrders = purchaseRepository.GetNotReceived()
                        .Where(p => p.Products.Any(o => o.ProductId == productId) && p.ReceivingLocationId == locationId);

                var movementTotal = 0;
                var model = new StockRequisitionOnOrderViewModel
                {
                    Id = productId,
                    Sku = prod.SKU,
                    Description = prod.LongDescription,
                    LocationId = locationId,
                    Location = loc.Name
                };

                purchaseOrders.Each(po =>
                {
                    po.Products.Where(p => p.ProductId == productId).Each(pop =>
                    {
                        var movement = pop.QuantityPending;
                        movementTotal += movement;
                        model.StockItems.Add(new StockRequisitionOnOrderProductViewModel
                        {
                            Id = po.Id,
                            Date = pop.EstimatedDeliveryDate ?? pop.RequestedDeliveryDate,
                            Movement = movement,
                            Total = movementTotal
                        });
                    });
                });

                return model;
            }
        }

        public StockRequisitionStockInfoModel GetStockInfo(int productId, int receivingLocationId, int warehouseLocationId)
        {
            using (var scope = Context.Read())
            {
                var receivingLocationStockLevel =
                    scope.Context.ProductStockLevel.FirstOrDefault(
                        x => x.ProductId == productId && x.LocationId == receivingLocationId) ?? new ProductStockLevel();

                var warehouseLocationStockLevel =
                    scope.Context.ProductStockLevel.FirstOrDefault(
                        x => x.ProductId == productId && x.LocationId == warehouseLocationId) ?? new ProductStockLevel();

                var now = DateTime.Now.Date;
                var deliveryDate =
                    scope.Context.StockDeliveryDateView.Where(
                        x => x.ProductId == productId && x.ReceivingLocationId == warehouseLocationId)
                        .OrderByDescending(o => o.EstimatedDeliveryDate)
                        .FirstOrDefault();

                var past = now.AddDays(-14);
                var sales = scope.Context.CintOrderCostView
                    .Where(x => x.StockLocationId == receivingLocationId && x.ProductId == productId)
                    .Where(x => x.TransactionDate <= now && x.TransactionDate >= past)
                    .Where(x => x.Type == CintOrderType.Delivery);

                var requisitionOustanding =
                    scope.Context.StockRequisitionProductView.Where(sr => sr.ReceivingLocationId == receivingLocationId && sr.ProductId == productId).ToList();
                var purchasingStockAllocation =
                    scope.Context.StockAllocationProduct.Where(sap => sap.ReceivingLocationId == receivingLocationId && sap.ProductId == productId).ToList();

                var product = scope.Context.Product.Single(p => p.Id == productId);

                return new StockRequisitionStockInfoModel()
                {
                    ProductId = productId,
                    Sku = product.SKU,
                    StockOnHand = receivingLocationStockLevel.StockOnHand,
                    AvailableStock = receivingLocationStockLevel.StockAvailable,
                    WarehouseStockOnHand = warehouseLocationStockLevel.StockOnHand,
                    WarehouseAvailableStock = warehouseLocationStockLevel.StockAvailable,
                    WarehouseStockOnOrder = warehouseLocationStockLevel.StockOnOrder,
                    ExpectedDeliveryDate = deliveryDate != null ? deliveryDate.EstimatedDeliveryDate : null,
                    StockOnOrder = receivingLocationStockLevel.StockOnOrder,
                    LastSales = sales.Any() ? sales.Sum(x => x.Quantity) : 0,
                    DistributionsOutstanding = requisitionOustanding.Sum(r => r.Quantity - r.QuantityCancelled - r.QuantityReceived) + purchasingStockAllocation.Sum(p => p.Quantity - p.QuantityCancelled - p.QuantityReceived)
                };
            }
        }

        public PagedSearchResult<StockRequisitionSearchResultModel> Search(StockRequisitionQueryModel model, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.StockRequisitionProductView.Where(x =>
                       (model.Id == null || model.Id == x.Id)
                    && (model.BookingId == null || x.BookingId == model.BookingId)
                    && (model.WarehouseLocationId == null || x.WarehouseLocationId == model.WarehouseLocationId)
                    && (model.ReceivingLocationId == null || x.ReceivingLocationId == model.ReceivingLocationId)
                    && (model.ReferenceNumber == null || x.ReferenceNumber == model.ReferenceNumber)
                    && (model.Sku == null || x.Sku == model.Sku)
                    && (model.Status == null || model.Status == x.Status));

                if (model.CreatedFrom.HasValue)
                {
                    var fromUTC = model.CreatedFrom.Value.ToUniversalTime();
                    query = query.Where(x => x.CreatedDate >= fromUTC);
                }

                if (model.CreatedTo.HasValue)
                {
                    var toUTC = model.CreatedTo.Value.ToUniversalTime().AddDays(1);
                    query = query.Where(x => x.CreatedDate < toUTC);
                }

                var count = query.Count();
                var page = query.OrderBy(v => v.Id)
                    .Skip(pageSize * pageIndex).Take(pageSize)
                    .Project().To<StockRequisitionSearchResultModel>()
                    .ToList();

                return new PagedSearchResult<StockRequisitionSearchResultModel> { Count = count, Page = page };
            }
        }
    }
}