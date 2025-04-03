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

    public interface IVendorReturnPublisher
    {
        void PublishCreated(VendorReturnViewModel vendorReturn);
    }

    public class VendorReturnPublisher : IVendorReturnPublisher
    {
        private readonly IPublisher publisher;
        private readonly IProductRepository productRepository;
        private readonly ISupplierRepository vendorRepository;

        public VendorReturnPublisher(IPublisher publisher, IProductRepository productRepository, ISupplierRepository vendorRepository)
        {
            this.publisher = publisher;
            this.productRepository = productRepository;
            this.vendorRepository = vendorRepository;
        }

        public void PublishCreated(VendorReturnViewModel vendorReturn)
        {
            var message = Mapper.Map<VendorReturnMessage>(vendorReturn);

            var productIds = vendorReturn.PurchaseOrders.SelectMany(p => p.Products).Where(p => p.QuantityReturned > 0).Select(p => p.ProductId).Distinct().ToList();
            var products = productRepository.GetProductMessages(productIds);
            message.Products = Mapper.Map<List<Product>>(products).ToArray();
            message.VendorType = vendorRepository.Get(message.VendorId).Type;

            foreach (var product in message.Products)
            {
                var productList = vendorReturn.PurchaseOrders.SelectMany(p => p.Products).Where(p2 => p2.ProductId == product.Id).ToList();
                product.Cost = (float)productList.Sum(p => (p.LastLandedCost ?? 0) * p.QuantityReturned);
                product.Units = productList.Sum(u => u.QuantityReturned);
            }
            publisher.Publish<Context, VendorReturnMessage>("Merchandising.VendorReturnCreated", message);
        }
    }
}