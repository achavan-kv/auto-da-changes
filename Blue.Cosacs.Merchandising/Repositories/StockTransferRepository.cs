namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Blue.Cosacs.Merchandising.Solr;
    using Blue.Cosacs.Messages.Merchandising.BookingMessage;
    using Blue.Events;
    using Blue.Hub.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IStockTransferRepository
    {
        StockTransferViewModel Get(int id);

        StockTransferViewModel Create(StockTransferCreateModel model, int userId, Func<int> getNextBookingId);

        void Cancel(BookingMessage message);

        void Receive(BookingMessage message);

        PagedSearchResult<StockTransferSearchResultModel> Search(StockTransferQueryModel model, int pageSize, int pageIndex);

        StockTransferPrintModel PrintBase(int id);
    }

    public class StockTransferRepository : IStockTransferRepository
    {
        private readonly IEventStore audit;

        private readonly IProductRepository productRepository;
        private readonly ICostRepository costRepository;
        private readonly IStockTransferPublisher publisher;
        private readonly IStockSolrIndexer stockSolrIndexer;
        private readonly ILocationRepository locationRepository;

        public StockTransferRepository(IEventStore audit, IProductRepository productRepository, ICostRepository costRepository, IStockTransferPublisher publisher, IStockSolrIndexer stockSolrIndexer, ILocationRepository locationRepository)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.costRepository = costRepository;
            this.publisher = publisher;
            this.stockSolrIndexer = stockSolrIndexer;
            this.locationRepository = locationRepository;
        }

        public StockTransferViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var stockTransfer = scope.Context.StockTransferView.Single(a => a.Id == id);
                var model = Mapper.Map<StockTransferViewModel>(stockTransfer);

                var locations = locationRepository.Get(new List<int>() 
                { 
                    stockTransfer.ViaLocationId.HasValue ? stockTransfer.ViaLocationId.Value : 0, 
                    stockTransfer.ReceivingLocationId, 
                     stockTransfer.SendingLocationId
                }).ToDictionary(d => d.Id);

                var products = scope.Context.StockTransferProductView.Where(p => p.StockTransferId == id).ToList();
                model.Products = products.Select(Mapper.Map<StockTransferProductViewModel>).ToList();

                model.ReceivingLocation = locations[model.ReceivingLocationId].Name;
                model.ReceivingLocationSalesId = locations[model.ReceivingLocationId].SalesId;

                model.SendingLocation = locations[model.SendingLocationId].Name;
                model.SendingLocationSalesId = locations[model.SendingLocationId].SalesId;
                if (model.ViaLocationId.HasValue)
                {
                    model.ViaLocation = locations[model.ViaLocationId.Value].Name;
                    model.ViaLocationSalesId = locations[model.ViaLocationId.Value].SalesId;
                }

                return model;
            }
        }

        public StockTransferProductViewModel GetByBooking(int bookingId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.StockTransferProductView
                    .Project()
                    .To<StockTransferProductViewModel>()
                    .Single(p => p.BookingId == bookingId);
            }
        }

        public StockTransferViewModel Create(StockTransferCreateModel model, int userId, Func<int> getNextBookingId)
        {
            using (var scope = Context.Write())
            {
                var stockTransfer = new StockTransfer();
                scope.Context.StockTransfer.Add(stockTransfer);

                Mapper.Map(model, stockTransfer);
                stockTransfer.ViaLocationId = stockTransfer.ViaLocationId == 0 ? null : stockTransfer.ViaLocationId;
                stockTransfer.CreatedDate = DateTime.UtcNow;
                stockTransfer.CreatedById = userId;
                scope.Context.SaveChanges();

                var products = model.Products.Select(Mapper.Map<StockTransferProduct>).ToList();
                var productIds = products.Select(p => p.ProductId).ToList();
                var costs = costRepository.GetCurrentByProducts(productIds);
                products.Each(p =>
                {
                    p.BookingId = getNextBookingId();
                    p.StockTransferId = stockTransfer.Id;
                    p.AverageWeightedCost = costs.FirstOrDefault(c => c.ProductId == p.ProductId).AverageWeightedCost;
                });
                scope.Context.StockTransferProduct.AddRange(products);
                scope.Context.SaveChanges();

                // direct transfers update to/from locations immediately
                this.MoveStock(stockTransfer, products);

                // Update stock in solr
                this.stockSolrIndexer.Index(productIds);

                var viewModel = Get(stockTransfer.Id);
                if (stockTransfer.ViaLocationId.HasValue)
                {
                    publisher.PublishCreated(viewModel);
                }
                else
                {
                    products.Each(product =>
                    {
                        scope.Context.StockTransferMovement.Add(new StockTransferMovement
                        {
                            BookingId = product.BookingId,
                            ProductId = product.ProductId,
                            ReceivingLocationId = stockTransfer.ReceivingLocationId,
                            SendingLocationId = stockTransfer.SendingLocationId,
                            DateProcessed = DateTime.UtcNow,
                            Quantity = product.Quantity,
                            Type = "DirectTransfer",
                            AverageWeightedCost = product.AverageWeightedCost
                        });
                    });
                }
                publisher.PublishFinancials(viewModel, products.Select(p => new KeyValuePair<int, int>(p.ProductId, p.Quantity)));

                scope.Context.SaveChanges();
                audit.LogAsync(stockTransfer, StockTransferEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return viewModel;
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

                var product = scope.Context.StockTransferProduct.Single(p => p.BookingId == message.BookingId);
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

                var product = scope.Context.StockTransferProduct.Single(p => p.BookingId == message.BookingId);
                product.QuantityReceived += message.Quantity;

                if (product.QuantityPending < 0)
                {
                    throw new MessageValidationException("Quantity to receive is greater than the pending quantity", null);
                }

                if (product.QuantityPending == 0)
                {
                    product.CompletedOn = DateTime.UtcNow;
                }

                var stockTransfer = scope.Context.StockTransfer.Single(s => s.Id == product.StockTransferId);

                if (!stockTransfer.ViaLocationId.HasValue)
                {
                    throw new Exception("Stock Transfer can only be received if it has a Via Location");
                }

                var movement = new StockMovementModel
                {
                    ProductId = product.ProductId,
                    Quantity = message.Quantity,
                    SendingLocationId = stockTransfer.ViaLocationId.Value,
                    ReceivingLocationId = stockTransfer.ReceivingLocationId,
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
                    Type = "ViaTransfer",
                    AverageWeightedCost = product.AverageWeightedCost
                });

                scope.Context.SaveChanges();

                var stockTransferModel = Get(stockTransfer.Id);

                publisher.PublishProductFinancials(stockTransferModel, GetByBooking(message.BookingId), message.Quantity, message.CurrentBookingId);
                scope.Complete();
            }
        }

        public PagedSearchResult<StockTransferSearchResultModel> Search(StockTransferQueryModel model, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var locations = scope.Context.Location.Select(l => new
                {
                    Id = l.Id,
                    Name = l.Name
                }).ToDictionary(d => d.Id, d => d.Name);

                var query = scope.Context.StockTransferView.Where(x => (model.Id == null || model.Id == x.Id));

                if (model.SendingLocationId.HasValue)
                {
                    query = query.Where(q => model.SendingLocationId.Value == q.SendingLocationId);
                }

                if (model.ReceivingLocationId.HasValue)
                {
                    query = query.Where(q => model.ReceivingLocationId.Value == q.ReceivingLocationId);
                }

                if (model.ViaLocationId.HasValue)
                {
                    query = query.Where(q => model.ViaLocationId.Value == q.ViaLocationId);
                }

                if (!string.IsNullOrEmpty(model.ReferenceNumber))
                {
                    query = query.Where(q => model.ReferenceNumber == q.ReferenceNumber);
                }

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

                if (!string.IsNullOrEmpty(model.Type))
                {
                    query = query.Where(q => (model.Type == "Direct" && !q.ViaLocationId.HasValue) || (model.Type == "Via" && q.ViaLocationId.HasValue));
                }

                if (!string.IsNullOrEmpty(model.Sku))
                {
                    query = (from st in query
                             join stp in scope.Context.StockTransferProduct on st.Id equals stp.StockTransferId
                             join p in scope.Context.Product on stp.ProductId equals p.Id
                             where p.SKU == model.Sku
                             select st).Distinct();
                }

                var count = query.Count();
                var page = query.OrderBy(v => v.Id)
                    .Skip(pageSize * pageIndex).Take(pageSize).ToList()
                    .Select(s => new StockTransferSearchResultModel
                        {
                            Comments = s.Comments,
                            CreatedBy = s.CreatedBy,
                            CreatedById = s.CreatedById,
                            CreatedDate = s.CreatedDate,
                            Id = s.Id,
                            ReceivingLocation = locations[s.ReceivingLocationId],
                            ReceivingLocationId = s.ReceivingLocationId,
                            ReferenceNumber = s.ReferenceNumber,
                            SendingLocation = locations[s.SendingLocationId],
                            SendingLocationId = s.SendingLocationId,
                            Total = s.Total,
                            ViaLocation = s.ViaLocationId.HasValue ? locations[s.ViaLocationId.Value] : string.Empty,
                            ViaLocationId = s.ViaLocationId
                        })
                    .ToList();
                return new PagedSearchResult<StockTransferSearchResultModel> { Count = count, Page = page };
            }
        }

        public StockTransferPrintModel PrintBase(int id)
        {
            using (var scope = Context.Read())
            {
                var st = scope.Context.StockTransferView.Find(id);
                var locs = new List<int?> { st.ViaLocationId, st.SendingLocationId, st.ReceivingLocationId };
                var locations = scope.Context.Location.Where(l => locs.Contains(l.Id)).ToDictionary(d => d.Id, d => d.Name);

                var model = new StockTransferPrintModel
                {
                    Comments = st.Comments,
                    CreatedBy = st.CreatedBy,
                    CreatedDate = st.CreatedDate.ToLocalDateTime().ToString("yyyy/MM/dd"),
                    Id = st.Id,
                    OriginalPrint = st.OriginalPrint,
                    ReceivingLocation = locations[st.ReceivingLocationId],
                    ReferenceNumber = st.ReferenceNumber,
                    SendingLocation = locations[st.SendingLocationId],
                    ViaLocation = st.ViaLocationId.HasValue ? locations[st.ViaLocationId.Value] : string.Empty
                };

                this.SetOriginalPrint(st);

                model.Products = scope.Context.StockTransferProductView
                    .Where(p => p.StockTransferId == id)
                    .Project().To<StockTransferProductViewModel>()
                    .ToList();

                return model;
            }
        }

        private void MoveStock(StockTransfer stockTransfer, List<StockTransferProduct> products)
        {
            var stockMovements = products.Select(p =>
                new StockMovementModel
                {
                    ProductId = p.ProductId,
                    SendingLocationId = stockTransfer.SendingLocationId,
                    ReceivingLocationId = stockTransfer.ViaLocationId ?? stockTransfer.ReceivingLocationId,
                    Quantity = p.Quantity,
                }).ToList();

            productRepository.AdjustStock(stockMovements);
        }

        private void SetOriginalPrint(StockTransferView stockTransferView)
        {
            if (stockTransferView.OriginalPrint == null)
            {
                using (var scope = Context.Write())
                {
                    var stockTransfer = scope.Context.StockTransfer.Single(a => a.Id == stockTransferView.Id);
                    stockTransfer.OriginalPrint = DateTime.UtcNow;
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
            }
        }
    }
}