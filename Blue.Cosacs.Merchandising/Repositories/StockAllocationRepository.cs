namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Blue.Cosacs.Messages.Merchandising.BookingMessage;
    using Blue.Events;
    using Blue.Hub.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IStockAllocationRepository
    {
        StockAllocationProductViewModel Get(int id);

        StockAllocationProductViewModel GetByBooking(int bookingId);

        List<StockAllocationProductViewModel> Get(IEnumerable<int> ids);

        void Cancel(BookingMessage message);

        void Receive(BookingMessage message);

        StockAllocationStockInfoModel GetStockInfo(int productId, int warehouseLocationId);

        List<StockAllocationProductViewModel> Create(StockAllocationCreateModel model, int userId, Func<int> getNextBookingId);

        PagedSearchResult<StockAllocationSearchResultModel> Search(StockAllocationQueryModel model, int pageSize, int pageIndex);
    }

    public class StockAllocationRepository : IStockAllocationRepository
    {
        private readonly IEventStore audit;

        private readonly ICostRepository costRepository;
        private readonly IStockAllocationPublisher publisher;
        private readonly IProductRepository productRepository;

        public StockAllocationRepository(IEventStore audit, ICostRepository costRepository, IStockAllocationPublisher publisher, IProductRepository productRepository)
        {
            this.audit = audit;
            this.costRepository = costRepository;
            this.publisher = publisher;
            this.productRepository = productRepository;
        }

        public StockAllocationProductViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.StockAllocationProductView.AsNoTracking()
                    .Project().To<StockAllocationProductViewModel>()
                    .Single(x => x.Id == id);
            }
        }

        public StockAllocationProductViewModel GetByBooking(int bookingId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.StockAllocationProductView.AsNoTracking()
                    .Project().To<StockAllocationProductViewModel>()
                    .Single(x => x.BookingId == bookingId);
            }
        }

        public List<StockAllocationProductViewModel> Get(IEnumerable<int> ids)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.StockAllocationProductView.AsNoTracking()
                    .Where(x => ids.Contains(x.Id))
                    .Project().To<StockAllocationProductViewModel>()
                    .ToList();
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

                var product = scope.Context.StockAllocationProduct.Single(p => p.BookingId == message.BookingId);
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

                var product = scope.Context.StockAllocationProduct.Single(p => p.BookingId == message.BookingId);
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
                    Type = "Allocation",
                    AverageWeightedCost = product.AverageWeightedCost
                });

                scope.Context.SaveChanges();
                
                publisher.PublishReceived(GetByBooking(message.BookingId), message.Quantity, message.CurrentBookingId);
                scope.Complete();
            }
        }

        public StockAllocationStockInfoModel GetStockInfo(int productId, int warehouseLocationId)
        {
            var cost = costRepository.GetLatestByProduct(productId);

            using (var scope = Context.Read())
            {
                var level = scope.Context.ProductStockLevel.FirstOrDefault(s => s.ProductId == productId && s.LocationId == warehouseLocationId);

                return new StockAllocationStockInfoModel
                {
                    LocationId = warehouseLocationId,
                    ProductId = productId,
                    QuantityAvailable = level == null ? 0 : level.StockAvailable,
                    VendorUnitCost = cost.SupplierCost
                };
            }
        }

        public List<StockAllocationProductViewModel> Create(StockAllocationCreateModel model, int userId, Func<int> getNextBookingId)
        {
            var now = DateTime.UtcNow;

            using (var scope = Context.Write())
            {
                var costs = costRepository.GetCurrentByProducts(model.Products.Select(p => p.ProductId.Value).ToList());

                var products = model.Products.Select(p =>
                {
                    var product = Mapper.Map<StockAllocationProduct>(p);
                    product.BookingId = getNextBookingId();
                    product.CreatedDate = now;
                    product.CreatedById = userId;
                    product.WarehouseLocationId = model.WarehouseLocationId;
                    product.Comments = model.Comments;
                    product.AverageWeightedCost = costs.FirstOrDefault(c => c.ProductId == p.ProductId).AverageWeightedCost;
                    return product;
                }).ToList();
                scope.Context.StockAllocationProduct.AddRange(products);
                scope.Context.SaveChanges();

                var viewModels = Get(products.Select(p => p.Id));
                publisher.PublishCreated(viewModels);
              
                audit.LogAsync(viewModels, StockAllocationEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return viewModels;
            }
        }

        public PagedSearchResult<StockAllocationSearchResultModel> Search(StockAllocationQueryModel model, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.StockAllocationProductView.Where(x =>
                       (model.Id == null || x.Id == model.Id)
                    && (model.BookingId == null || x.BookingId == model.BookingId)
                    && (model.WarehouseLocationId == null || x.WarehouseLocationId == model.WarehouseLocationId)
                    && (model.ReceivingLocationId == null || x.ReceivingLocationId == model.ReceivingLocationId)
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

                query = query.DistinctBy(x => x.Id);

                var count = query.Count();
                var page = query.OrderBy(v => v.Id)
                    .Skip(pageSize * pageIndex).Take(pageSize)
                    .Project().To<StockAllocationSearchResultModel>()
                    .ToList();

                return new PagedSearchResult<StockAllocationSearchResultModel> { Count = count, Page = page };
            }
        }
    }
}