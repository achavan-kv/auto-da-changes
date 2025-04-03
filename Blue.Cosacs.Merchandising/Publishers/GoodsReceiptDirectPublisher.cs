namespace Blue.Cosacs.Merchandising.Publishers
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Messages.Merchandising.GoodsReceipt;
    using Blue.Hub.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IGoodsReceiptDirectPublisher
    {
        void PublishCreated(GoodsReceiptDirectViewModel goodsReceipt);
    }

    public class GoodsReceiptDirectPublisher : IGoodsReceiptDirectPublisher
    {
        private readonly IPublisher publisher;

        private readonly IProductRepository productRepository;

        public GoodsReceiptDirectPublisher(IPublisher publisher, IProductRepository productRepository)
        {
            this.publisher = publisher;
            this.productRepository = productRepository;
        }

        public void PublishCreated(GoodsReceiptDirectViewModel goodsReceipt)
        {
            var message = Mapper.Map<GoodsReceiptMessage>(goodsReceipt);
            var productIds = goodsReceipt.Products.Where(p => p.QuantityReceived > 0).Select(p => p.ProductId).Distinct().ToList();
            var products = productRepository.GetProductMessages(productIds);

            if (!products.Any())
            {
                throw new Exception(
                    "No product messages found for ids " + string.Join(",", productIds.Select(x => x.ToString())));
            }

            message.Products = Mapper.Map<List<GoodsReceiptMessageProduct>>(products).ToArray();

            foreach (var p in message.Products)
            {
                var productList = goodsReceipt.Products.Where(p2 => p2.ProductId == p.Id).ToList();
                p.Cost = productList.Sum(u => u.UnitLandedCost * u.QuantityReceived);
                p.Units = productList.Sum(u => u.QuantityReceived);
            }

            publisher.Publish<Context, GoodsReceiptMessage>("Merchandising.GoodsReceiptCreated", message);
        }
    }
}
