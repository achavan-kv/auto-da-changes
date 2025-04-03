namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IGoodsOnLoanRepository
    {
        GoodsOnLoanViewModel Get(int id);

        GoodsOnLoanViewModel Create(GoodsOnLoanCreateModel model, int userId, Func<int> getNextBookingId);

        GoodsOnLoanViewModel Update(GoodsOnLoanUpdateModel model);

        PagedSearchResult<GoodsOnLoanSearchResultModel> Search(GoodsOnLoanQueryModel model, int pageSize, int pageIndex);

        GoodsOnLoanViewModel Collect(int id, int userId, Func<int> getNextBookingId);

        GoodsOnLoanPrintModel Print(int id, GoodsOnLoanPrintType type);

        GoodsOnLoanStockInfoModel GetStockInfo(int productId, int warehouseLocationId);
    }

    public class GoodsOnLoanRepository : IGoodsOnLoanRepository
    {
        private readonly IEventStore audit;

        private readonly IUserRepository userRepository;

        private readonly IStockTransferRepository stockTransferRepository;

        private readonly Settings merchandiseSettings;

        private readonly ICostRepository costRepository;

        public GoodsOnLoanRepository(IEventStore audit, ICostRepository costRepository, IUserRepository userRepository, IStockTransferRepository stockTransferRepository, Blue.Cosacs.Merchandising.Settings merchandiseSettings)
        {
            this.audit = audit;
            this.costRepository = costRepository;

            this.userRepository = userRepository;
            this.stockTransferRepository = stockTransferRepository;
            this.merchandiseSettings = merchandiseSettings;
        }

        public GoodsOnLoanViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var goodsOnLoan = scope.Context.GoodsOnLoan.Single(a => a.Id == id);
                var model = Mapper.Map<GoodsOnLoanViewModel>(goodsOnLoan);

                model.CreatedBy = userRepository.Get(model.CreatedById).FullName;
                model.StockLocation = scope.Context.Location.Single(l => l.Id == goodsOnLoan.StockLocationId).Name;

                var products = scope.Context.GoodsOnLoanProductView.Where(p => p.GoodsOnLoanId == id).ToList();
                model.Products = products.Select(Mapper.Map<GoodsOnLoanProductViewModel>).ToList();

                return model;
            }
        }

        public GoodsOnLoanViewModel Create(GoodsOnLoanCreateModel model, int userId, Func<int> getNextBookingId)
        {
            using (var scope = Context.Write())
            {
                var goodsOnLoan = new GoodsOnLoan();
                scope.Context.GoodsOnLoan.Add(goodsOnLoan);

                Mapper.Map(model, goodsOnLoan);
                goodsOnLoan.CreatedOn = DateTime.UtcNow;
                goodsOnLoan.CreatedById = userId;
                scope.Context.SaveChanges();

                var products = model.Products.Select(Mapper.Map<GoodsOnLoanProduct>).ToList();
                var costs = costRepository.GetCurrentByProducts(products.Select(p => p.ProductId).ToList());
                products.Each(
                    p =>
                    {
                        p.GoodsOnLoanId = goodsOnLoan.Id;
                        p.AverageWeightedCost = costs.First(c => c.ProductId == p.ProductId).AverageWeightedCost;
                    });
                scope.Context.GoodsOnLoanProduct.AddRange(products);
                scope.Context.SaveChanges();

                if (string.IsNullOrWhiteSpace(merchandiseSettings.GoodsOnLoanWarehouse))
                {
                    throw new Exception("Goods on loan warehouse must be configured.");
                }

                var goodsOnLoanWarehouse = scope.Context.Location.Single(l => l.Name == merchandiseSettings.GoodsOnLoanWarehouse);

                var transfer = stockTransferRepository.Create(
                    new StockTransferCreateModel
                        {
                            SendingLocationId = model.StockLocationId,
                            ReceivingLocationId = goodsOnLoanWarehouse.Id,
                            Comments = "Automatic stock transfer created to Goods on Loan Warehouse for Goods on Loan GOL#" + goodsOnLoan.Id,
                            Products = Mapper.Map<List<StockTransferProductCreateModel>>(model.Products)
                        },
                    userId,
                    getNextBookingId);

                goodsOnLoan.StockTransferId = transfer.Id;

                scope.Context.SaveChanges();

                var viewModel = Get(goodsOnLoan.Id);
              
                audit.LogAsync(goodsOnLoan, GoodsOnLoanEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return viewModel;
            }
        }

        public GoodsOnLoanViewModel Update(GoodsOnLoanUpdateModel model)
        {
            using (var scope = Context.Write())
            {
                var goodsOnLoan = scope.Context.GoodsOnLoan.Single(g => g.Id == model.Id);

                goodsOnLoan.ExpectedCollectionDate = model.ExpectedCollectionDate;
                goodsOnLoan.Comments = model.Comments;

                scope.Context.SaveChanges();

                var viewModel = Get(goodsOnLoan.Id);
              
                audit.LogAsync(goodsOnLoan, GoodsOnLoanEvents.Update, EventCategories.Merchandising);
                scope.Complete();
                return viewModel;
            }
        }

        public GoodsOnLoanStockInfoModel GetStockInfo(int productId, int warehouseLocationId)
        {
            var cost = costRepository.GetLatestByProduct(productId);

            return new GoodsOnLoanStockInfoModel
            {
                LocationId = warehouseLocationId,
                ProductId = productId,
                AverageWeightedCost = cost.AverageWeightedCost
            };
        }

        public GoodsOnLoanViewModel Collect(int id, int userId, Func<int> getNextBookingId)
        {
            using (var scope = Context.Write())
            {
                var goodsOnLoan = scope.Context.GoodsOnLoan.Single(g => g.Id == id);

                if (goodsOnLoan.CollectedDate.HasValue)
                {
                    throw new Exception("This goods on loan has already been collected");
                }

                var products = scope.Context.GoodsOnLoanProduct.Where(p => p.GoodsOnLoanId == id).ToList();

                if (string.IsNullOrWhiteSpace(merchandiseSettings.GoodsOnLoanWarehouse))
                {
                    throw new Exception("Goods on loan warehouse must be configured.");
                }

                var goodsOnLoanWarehouse = scope.Context.Location.Single(l => l.Name == merchandiseSettings.GoodsOnLoanWarehouse);

                var transfer = stockTransferRepository.Create(
                    new StockTransferCreateModel
                    {
                        SendingLocationId = goodsOnLoanWarehouse.Id,
                        ReceivingLocationId = goodsOnLoan.StockLocationId,
                        Comments = "Automatic stock transfer created from Goods on Loan Warehouse for Goods on Loan GOL#" + goodsOnLoan.Id,
                        Products = Mapper.Map<List<StockTransferProductCreateModel>>(products)
                    },
                    userId,
                    getNextBookingId);

                goodsOnLoan.ReturnStockTransferId = transfer.Id;
                goodsOnLoan.CollectedDate = DateTime.UtcNow;

                scope.Context.SaveChanges();

                var viewModel = Get(goodsOnLoan.Id);
            
                audit.LogAsync(goodsOnLoan, GoodsOnLoanEvents.Collect, EventCategories.Merchandising);
                scope.Complete();
                return viewModel;
            }
        }

        public PagedSearchResult<GoodsOnLoanSearchResultModel> Search(GoodsOnLoanQueryModel model, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var query =
                    scope.Context.GoodsOnLoanSearchView.Where(x =>
                        (model.Id == null || x.Id == model.Id) &&
                        (model.StockLocationId == null || x.StockLocationId == model.StockLocationId)
                        && (model.ExpectedCollectionDate == null || (x.ExpectedCollectionDate <= model.ExpectedCollectionDate && x.Status != "Completed"))
                        && (model.Sku == null || x.SKU == model.Sku)
                        && (model.Status == null || x.Status == model.Status) && (model.ReferenceNumber == null || x.ReferenceNumber == model.ReferenceNumber)
                        && (model.Type == null || x.Type == model.Type));

                if (model.CreatedFrom.HasValue)
                {
                    var fromUTC = model.CreatedFrom.Value.ToUniversalTime();
                    query = query.Where(x => x.CreatedOn >= fromUTC);
                }

                if (model.CreatedTo.HasValue)
                {
                    var toUTC = model.CreatedTo.Value.ToUniversalTime().AddDays(1);
                    query = query.Where(x => x.CreatedOn < toUTC);
                }

                query = query.DistinctBy(x => x.GoodsOnLoanId);

                var count = query.Count();
                var page = query.OrderBy(v => v.Id)
                    .Skip(pageSize * pageIndex).Take(pageSize)
                    .Project().To<GoodsOnLoanSearchResultModel>()
                    .ToList();
                return new PagedSearchResult<GoodsOnLoanSearchResultModel> { Count = count, Page = page };
            }
        }

        public GoodsOnLoanPrintModel Print(int id, GoodsOnLoanPrintType type)
        {
            using (var scope = Context.Write())
            {
                var goodsOnLoan = scope.Context.GoodsOnLoan.Single(a => a.Id == id);

                var originalPrint = false;

                var model = Mapper.Map<GoodsOnLoanPrintModel>(goodsOnLoan);

                model.CreatedBy = userRepository.Get(model.CreatedById.Value).FullName;
                model.StockLocation = scope.Context.Location.Single(l => l.Id == goodsOnLoan.StockLocationId).Name;

                var products = scope.Context.GoodsOnLoanProductView.Where(p => p.GoodsOnLoanId == id).ToList();
                model.Products = products.Select(Mapper.Map<GoodsOnLoanProductPrintModel>).ToList();

                if (type == GoodsOnLoanPrintType.Collection && !goodsOnLoan.CollectionPrintedDate.HasValue)
                {
                    goodsOnLoan.CollectionPrintedDate = DateTime.UtcNow;
                    originalPrint = true;
                }

                if (type == GoodsOnLoanPrintType.Delivery && !goodsOnLoan.DeliveryPrintedDate.HasValue)
                {
                    goodsOnLoan.DeliveryPrintedDate = DateTime.UtcNow;
                    originalPrint = true;
                }

                scope.Context.SaveChanges();
               
                audit.LogAsync(new { OriginalPrint = originalPrint, GodosOnLoan = this.Get(id) }, type == GoodsOnLoanPrintType.Collection ? GoodsOnLoanEvents.PrintCollectionNote : GoodsOnLoanEvents.PrintDeliveryNote, EventCategories.Merchandising);
                scope.Complete();
                return model;
            }
        }
    }

    public enum GoodsOnLoanPrintType
    {
        Delivery,

        Collection
    }
}
