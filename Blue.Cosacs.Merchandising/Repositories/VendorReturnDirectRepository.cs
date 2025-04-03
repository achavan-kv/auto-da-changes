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
    using System.Linq;
    using Context = Blue.Cosacs.Merchandising.Context;

    public interface IVendorReturnDirectRepository
    {
        VendorReturnDirectViewModel Get(int id);

        VendorReturnDirectNewModel New(int goodsReceiptId);

        VendorReturnDirectViewModel Create(VendorReturnCreateModel model, UserSession user);

        void Approve(int id, int userId, string referenceNumber, string comments);

        VendorReturnDirectPrintModel Print(int id);
    }

    public class VendorReturnDirectRepository : IVendorReturnDirectRepository
    {
        private readonly IEventStore audit;

        private readonly IProductRepository productRepository;

        private readonly IVendorReturnMapper mapper;
        private readonly IVendorReturnDirectPublisher publisher;

        public VendorReturnDirectRepository(IEventStore audit, IProductRepository productRepository, IVendorReturnMapper mapper, IVendorReturnDirectPublisher publisher)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.publisher = publisher;
        }

        public VendorReturnDirectViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var viewModel = Mapper.Map<VendorReturnDirectViewModel>(scope.Context.VendorReturnView.Single(x => x.Id == id));

                var products = scope.Context.VendorReturnDirectProductView
                    .Where(x => x.VendorReturnId == id && x.QuantityReturned > 0)
                    .ToList();

                return mapper.MapVendorReturnViewModel(viewModel, products);
            }
        }

        public VendorReturnDirectNewModel New(int goodsReceiptId)
        {
            using (var scope = Context.Read())
            {
                var newModel = Mapper.Map<VendorReturnDirectNewModel>(scope.Context.GoodsReceiptDirect.Single(x => x.Id == goodsReceiptId));
                var vendorReturnProducts = scope.Context.VendorReturnDirectNewView
                    .Where(x => x.GoodsReceiptId == goodsReceiptId)
                    .ToList();

                return mapper.MapVendorReturnDirectNewModel(newModel, vendorReturnProducts);
            }
        }

        public VendorReturnDirectViewModel Create(VendorReturnCreateModel model, UserSession user)
        {
            using (var scope = Context.Write())
            {
                var vendorReturn = Mapper.Map<VendorReturn>(model);

                // Save the vendor return
                vendorReturn.CreatedDate = DateTime.UtcNow;
                vendorReturn.CreatedById = user.Id;
                vendorReturn.CreatedBy = user.FullName;
                vendorReturn.ReceiptType = GoodsReceiptType.Direct;
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
                    scope.Context.VendorReturnDirectProductView.Where(v => v.VendorReturnId == vendorReturn.Id).ToList();
                productRepository.ReturnStock(vendorProdViewItems);

                var viewModel = Get(vendorReturn.Id);
                publisher.PublishCreated(viewModel);
                
                audit.LogAsync(viewModel, VendorReturnEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return viewModel;
            }
        }

        public void Approve(int id, int userId, string referenceNumber, string comments)
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
                vendorReturn.ApprovedDate = DateTime.Now;
                vendorReturn.Comments = comments;
                vendorReturn.ReferenceNumber = referenceNumber;
                scope.Context.SaveChanges();
               
                audit.LogAsync(new { id, userId, comments }, VendorReturnEvents.Approve, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public VendorReturnDirectPrintModel Print(int id)
        {
            var vendorReturn = Get(id);
            var model = Mapper.Map<VendorReturnDirectViewModel, VendorReturnDirectPrintModel>(vendorReturn);
            model.FormattedTotalCost = model.Products.Sum(p => p.LastLandedCost * p.QuantityReturned ?? 0).ToCurrencyWithSymbol();

            using (var scope = Context.Read())
            {
                var goodsReceipt = scope.Context.GoodsReceiptDirect.Single(gr => gr.Id == vendorReturn.GoodsReceiptId);
                model.GoodsReceipt = Mapper.Map<GoodsReceiptDirectViewModel>(goodsReceipt);
            }

            foreach (var prod in model.Products)
            {
                prod.UnitLandedCost = (prod.LastLandedCost ?? 0).ToCurrencyWithSymbol();
                prod.LineLandedCost = (prod.QuantityReturned * (prod.LastLandedCost ?? 0)).ToCurrencyWithSymbol();
            }

            audit.LogAsync(new { GoodsReceipt = model }, VendorReturnEvents.Print, EventCategories.Merchandising);
            return model;
        }
    }
}