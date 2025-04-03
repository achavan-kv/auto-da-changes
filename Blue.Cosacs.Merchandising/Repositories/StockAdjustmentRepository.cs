namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public interface IStockAdjustmentRepository
    {
        StockAdjustmentViewModel Get(int id);

        StockAdjustmentViewModel Create(StockAdjustmentCreateModel model, int userId);

        StockAdjustmentSearchModel Search(StockAdjustmentSearchQueryModel search, int pageSize, int pageIndex);

        StockAdjustmentViewModel Approve(int id, string comments, int userId);

        StockAdjustmentPrintModel Print(int id);
    }

    public class StockAdjustmentRepository : IStockAdjustmentRepository
    {
        private readonly IEventStore audit;

        private readonly IProductRepository productRepository;

        private readonly ICostRepository costRepository;
        private readonly IStockAdjustmentPublisher publisher;

        public StockAdjustmentRepository(IEventStore audit, IProductRepository productRepository, ICostRepository costRepository, IStockAdjustmentPublisher publisher)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.costRepository = costRepository;
            this.publisher = publisher;
        }

        public StockAdjustmentViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var stockAdjustment = scope.Context.StockAdjustmentView.Single(a => a.Id == id);
                var model = Mapper.Map<StockAdjustmentViewModel>(stockAdjustment);

                var products = scope.Context.StockAdjustmentProductView.Where(p => p.StockAdjustmentId == id).ToList();
                model.Products = products.Select(Mapper.Map<StockAdjustmentProductViewModel>).ToList();

                return model;
            }
        }

        public StockAdjustmentViewModel Create(StockAdjustmentCreateModel model, int userId)
        {
            using (var scope = Context.Write())
            {
                StockAdjustment stockAdjustment;
                if (model.Id == null || model.Id == 0)
                {
                    stockAdjustment = new StockAdjustment();
                    scope.Context.StockAdjustment.Add(stockAdjustment);
                }
                else
                {
                    stockAdjustment = scope.Context.StockAdjustment.Single(s => s.Id == model.Id);
                }

                Mapper.Map(model, stockAdjustment);
                stockAdjustment.CreatedDate = DateTime.UtcNow;
                stockAdjustment.CreatedById = userId;
                scope.Context.SaveChanges();

                // Add child products on create
                if (model.Id == null || model.Id == 0)
                {
                    var products = model.Products.Select(Mapper.Map<StockAdjustmentProduct>).ToList();

                    //CR : closing Quarterly Stock counts with variance = 0

                    var dtStockCountProducts = new System.Data.DataTable();
                    dtStockCountProducts.Columns.Add("Id", typeof(int));
                    dtStockCountProducts.Columns.Add("StockAdjustmentId", typeof(int));
                    dtStockCountProducts.Columns.Add("ProductId", typeof(int));
                    dtStockCountProducts.Columns.Add("Quantity", typeof(int));
                    dtStockCountProducts.Columns.Add("Comments", typeof(string));
                    dtStockCountProducts.Columns.Add("ReferenceNumber", typeof(string));
                    dtStockCountProducts.Columns.Add("AverageWeightedCost", typeof(decimal));
                    products.Each(p =>
                    {
                        p.StockAdjustmentId = stockAdjustment.Id;

                        DataRow rowProduct = dtStockCountProducts.NewRow();
                        rowProduct["Id"] = p.Id;
                        rowProduct["StockAdjustmentId"] = p.StockAdjustmentId;
                        rowProduct["ProductId"] = p.ProductId;
                        rowProduct["Quantity"] = p.Quantity;
                        rowProduct["Comments"] = p.Comments;
                        rowProduct["ReferenceNumber"] = p.ReferenceNumber;
                        rowProduct["AverageWeightedCost"] = p.AverageWeightedCost;
                        dtStockCountProducts.Rows.Add(rowProduct);
                    });

                    scope.Context.CreateStockAdjustmentFromStockCount(dtStockCountProducts);
                    var stockAdjustments = products.Select(p => new StockAdjustmentModel
                    {
                        ProductId = p.ProductId,
                        LocationId = stockAdjustment.LocationId,
                        Quantity = p.Quantity
                    }).ToList();
                    productRepository.AdjustStock(stockAdjustments);
                }

                var viewModel = Get(stockAdjustment.Id);
                publisher.PublishCreated(viewModel);
                audit.LogAsync(stockAdjustment, StockAdjustmentEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return viewModel;
            }
        }

        public StockAdjustmentSearchModel Search(StockAdjustmentSearchQueryModel search, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.StockAdjustmentSearchView.Where(v =>
                    (v.StockAdjustmentId >= search.MinStockAdjustmentId || search.MinStockAdjustmentId == null)
                    && (v.StockAdjustmentId <= search.MaxStockAdjustmentId || search.MaxStockAdjustmentId == null)
                    && (v.PrimaryReasonId == search.PrimaryReasonId || search.PrimaryReasonId == null)
                    && (v.SecondaryReasonId == search.SecondaryReasonId || search.SecondaryReasonId == null)
                    && (v.Approved == search.Approved || search.Approved == null)
                    && (v.LocationId == search.LocationId || search.LocationId == null));

                if (search.MinCreatedDate.HasValue)
                {
                    var fromUTC = search.MinCreatedDate.Value.ToUniversalTime();
                    query = query.Where(x => x.CreatedDate >= fromUTC);
                }

                if (search.MaxCreatedDate.HasValue)
                {
                    var toUTC = search.MaxCreatedDate.Value.ToUniversalTime().AddDays(1);
                    query = query.Where(x => x.CreatedDate < toUTC);
                }

                var total = query.Count();
                var page = query.OrderBy(v => v.Id).Skip(pageSize * pageIndex).Take(pageSize).ToList();
                var results = Mapper.Map<List<StockAdjustmentSearchResultModel>>(page);
                var model = new StockAdjustmentSearchModel { Results = results, TotalResults = total };
                return model;
            }
        }

        public StockAdjustmentViewModel Approve(int id, string comments, int userId)
        {
            using (var scope = Context.Write())
            {
                var stockAdjustment = scope.Context.StockAdjustment.Single(a => a.Id == id);
                if (stockAdjustment.AuthorisedDate != null)
                {
                    throw new InvalidOperationException();
                }
                stockAdjustment.AuthorisedDate = DateTime.UtcNow;
                stockAdjustment.AuthorisedById = userId;
                stockAdjustment.Comments = comments;
                scope.Context.SaveChanges();
                audit.LogAsync(stockAdjustment, StockAdjustmentEvents.Authorise, EventCategories.Merchandising);
                scope.Complete();
                return Get(id);
            }
        }

        public StockAdjustmentPrintModel Print(int id)
        {
            using (var scope = Context.Write())
            {
                var stockAdjustmentPrint = this.Get(id);
                var model = Mapper.Map<StockAdjustmentPrintModel>(stockAdjustmentPrint);

                bool original = false;

                foreach (var prod in model.Products)
                {
                    prod.UnitCost = prod.AverageWeightedCost.ToCurrencyWithSymbol();
                    prod.LineCost = (prod.Quantity * prod.AverageWeightedCost).ToCurrencyWithSymbol();
                }

                if (stockAdjustmentPrint.OriginalPrint == null)
                {
                    var stockAdjustment = scope.Context.StockAdjustment.Single(a => a.Id == stockAdjustmentPrint.Id);
                    original = true;
                    stockAdjustment.OriginalPrint = DateTime.UtcNow;
                    scope.Context.SaveChanges();
                }

                audit.LogAsync(new { OriginalPrint = original, GoodsReceipt = this.Get(id) }, StockAdjustmentEvents.Print, EventCategories.Merchandising);
                scope.Complete();
                return model;
            }
        }
    }
}