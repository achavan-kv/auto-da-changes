namespace Blue.Cosacs.Merchandising.Publishers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Messages.Merchandising.VendorReturn;
    using Blue.Hub.Client;
    using Product = Blue.Cosacs.Messages.Merchandising.VendorReturn.Product;

    public interface IVendorReturnDirectPublisher
    {
        void PublishCreated(VendorReturnDirectViewModel vendorReturn);
    }

    public class VendorReturnDirectPublisher : IVendorReturnDirectPublisher
    {
        private readonly IPublisher publisher;
        private readonly IProductRepository productRepository;
        private readonly ISupplierRepository vendorRepository;

        public VendorReturnDirectPublisher(IPublisher publisher, IProductRepository productRepository, ISupplierRepository vendorRepository)
        {
            this.publisher = publisher;
            this.productRepository = productRepository;
            this.vendorRepository = vendorRepository;
        }

        public void PublishCreated(VendorReturnDirectViewModel vendorReturn)
        {
            var message = Mapper.Map<VendorReturnMessage>(vendorReturn);

            var productIds = vendorReturn.Products.Where(p => p.QuantityReturned > 0).Select(p => p.ProductId).Distinct().ToList();
            var products = productRepository.GetProductMessages(productIds);
            message.Products = Mapper.Map<List<Product>>(products).ToArray();
            message.VendorType = vendorRepository.Get(message.VendorId).Type;

            foreach (var product in message.Products)
            {
                var productModel = vendorReturn.Products.Single(p2 => p2.ProductId == product.Id);
                product.Cost = (float)(productModel.LastLandedCost ?? 0) * productModel.QuantityReturned;
                product.Units = productModel.QuantityReturned;
            }

            publisher.Publish<Context, VendorReturnMessage>("Merchandising.VendorReturnCreated", message);
        }
    }
}