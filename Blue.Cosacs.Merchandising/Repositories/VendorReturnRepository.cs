namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Admin;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Mappers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context = Blue.Cosacs.Merchandising.Context;

    public interface IVendorReturnRepository
    {
        VendorReturnViewModel Get(int id);

        VendorReturnNewModel New(int goodsReceiptId);

        VendorReturnViewModel Create(VendorReturnCreateModel model, UserSession user);

        VendorReturnSearchModel Search(VendorReturnSearchQueryModel search, int pageSize, int pageIndex);

        void Approve(int id, int userId, string referenceNumber, string comments, DateTime approveDate);

        VendorReturnPrintModel Print(int id);
    }

    public class VendorReturnRepository : IVendorReturnRepository
    {
        private readonly IEventStore audit;
        private readonly IProductRepository productRepository;
        private readonly IGoodsReceiptRepository receiptRepository;
        private readonly IVendorReturnPublisher publisher;
        private readonly IVendorReturnMapper mapper;

        public VendorReturnRepository(IEventStore audit, IProductRepository productRepository, IVendorReturnMapper mapper, IGoodsReceiptRepository receiptRepository, IVendorReturnPublisher publisher)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.receiptRepository = receiptRepository;
            this.publisher = publisher;
        }

        public VendorReturnViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var viewModel = Mapper.Map<VendorReturnViewModel>(scope.Context.VendorReturnView.Single(x => x.Id == id));

                var products = scope.Context.VendorReturnProductView
                    .Where(x => x.VendorReturnId == id && x.QuantityReturned > 0)
                    .ToList();

                return mapper.MapVendorReturnViewModel(viewModel, products);
            }
        }

        public VendorReturnNewModel New(int goodsReceiptId)
        {
            using (var scope = Context.Read())
            {
                var newModel = Mapper.Map<VendorReturnNewModel>(scope.Context.GoodsReceipt.Single(x => x.Id == goodsReceiptId));
                var vendorReturnProducts = scope.Context.VendorReturnNewView
                    .Where(x => x.GoodsReceiptId == goodsReceiptId)
                    .ToList();

                return mapper.MapVendorReturnNewModel(newModel, vendorReturnProducts);
            }
        }

        public VendorReturnViewModel Create(VendorReturnCreateModel model, UserSession user)
        {
            using (var scope = Context.Write())
            {
                var vendorReturn = Mapper.Map<VendorReturn>(model);

                // Save the vendor return
                vendorReturn.CreatedDate = DateTime.UtcNow;
                vendorReturn.CreatedById = user.Id;
                vendorReturn.CreatedBy = user.FullName;
                vendorReturn.ReceiptType = GoodsReceiptType.Standard;
                scope.Context.VendorReturn.Add(vendorReturn);
                scope.Context.SaveChanges();

                // Save the goods vendor return
                var vendorReturnProducts = model.VendorReturnProducts.Select(vrpm =>
                {
                    var vrp = Mapper.Map<VendorReturnProduct>(vrpm);
                    vrp.VendorReturnId = vendorReturn.Id;
                    return vrp;
                }).ToList();

                scope.Context.VendorReturnProduct.AddRange(vendorReturnProducts);
                scope.Context.SaveChanges();

                // Update stock and average weighted costs if costing ok
                var vendorProdViewItems =
                    scope.Context.VendorReturnProductView.Where(v => v.VendorReturnId == vendorReturn.Id).ToList();
                productRepository.ReturnStock(vendorProdViewItems);

                var viewModel = Get(vendorReturn.Id);
                viewModel.CreatedDate = viewModel.CreatedDate.ToLocalDateTime(); // Local date as we are converting to date.
                publisher.PublishCreated(viewModel);
                scope.Complete();

                audit.LogAsync(viewModel, VendorReturnEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return viewModel;
            }
        }

        public VendorReturnSearchModel Search(VendorReturnSearchQueryModel search, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.VendorReturnSearchView.AsQueryable();

                if (search.MaxCreatedDate.HasValue)
                {
                    var maxdate = search.MaxCreatedDate.Value.Date.AddDays(1);
                    query = query.Where(v => v.CreatedDate < maxdate);
                }

                if (search.MinCreatedDate.HasValue)
                {
                    query = query.Where(v => v.CreatedDate >= search.MinCreatedDate.Value);
                }

                if (search.VendorId.HasValue)
                {
                    query = query.Where(v => v.VendorId == search.VendorId.Value);
                }

                if (search.LocationId.HasValue)
                {
                    query = query.Where(v => v.LocationId == search.LocationId.Value);
                }

                if (search.Approved.HasValue)
                {
                    query = query.Where(v => v.Approved == search.Approved.Value);
                }

                if (!string.IsNullOrEmpty(search.Type))
                {
                    query = query.Where(v => v.GoodsReceiptType == search.Type);
                }

                if (search.LocationId.HasValue)
                {
                    query = query.Where(v => v.LocationId == search.LocationId.Value);
                }

                if (search.MaxVendorReturnId.HasValue)
                {
                    query = query.Where(v => v.VendorReturnId <= search.MaxVendorReturnId.Value);
                }

                if (search.MinVendorReturnId.HasValue)
                {
                    query = query.Where(v => v.VendorReturnId >= search.MinVendorReturnId.Value);
                }

                var total = query.Count();
                var page = query.OrderBy(v => v.Id).Skip(pageSize * pageIndex).Take(pageSize).ToList();
                var results = Mapper.Map<List<VendorReturnSearchResultModel>>(page);
                var model = new VendorReturnSearchModel { Results = results, TotalResults = total };

                return model;
            }
        }

        public void Approve(int id, int userId, string referenceNumber, string comments, DateTime approveDate)
        {
            using (var scope = Context.Write())
            {
                var vendorReturn = scope.Context.VendorReturn.Single(x => x.Id == id);
                var user = scope.Context.UserPermissionsView.First(u => u.UserId == userId).FullName;

                if (vendorReturn.ApprovedDate.HasValue)
                {
                    throw new ArgumentException(string.Format("The vendor return with id {0} has already been approved", id));
                }
                vendorReturn.ApprovedBy = user;
                vendorReturn.ApprovedById = userId;
                vendorReturn.ApprovedDate = approveDate;
                vendorReturn.Comments = comments;
                vendorReturn.ReferenceNumber = referenceNumber;

                scope.Context.SaveChanges();
                audit.LogAsync(new { id, userId, comments }, VendorReturnEvents.Approve, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public VendorReturnPrintModel Print(int id)
        {
            var vendorReturn = Get(id);
            var model = Mapper.Map<VendorReturnViewModel, VendorReturnPrintModel>(vendorReturn);
            model.GoodsReceipt = receiptRepository.Get(vendorReturn.GoodsReceiptId);
            model.FormattedTotalCost = model.PurchaseOrders.Sum(o => o.Products.Sum(p => p.LastLandedCost * p.QuantityReturned ?? 0)).ToCurrencyWithSymbol();

            foreach (var po in model.PurchaseOrders)
            {
                foreach (var prod in po.Products)
                {
                    prod.UnitLandedCost = (prod.LastLandedCost ?? 0).ToCurrencyWithSymbol();
                    prod.LineLandedCost = (prod.QuantityReturned * (prod.LastLandedCost ?? 0)).ToCurrencyWithSymbol();
                }
            }

            audit.LogAsync(new { GoodsReceipt = model }, VendorReturnEvents.Print, EventCategories.Merchandising);
            return model;
        }
    }
}