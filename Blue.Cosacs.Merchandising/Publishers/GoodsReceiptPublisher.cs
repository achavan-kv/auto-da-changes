namespace Blue.Cosacs.Merchandising.Publishers
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Messages.Merchandising.GoodsReceipt;
    using Blue.Hub.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IGoodsReceiptPublisher
    {
        void PublishCreated(GoodsReceiptViewModel goodsReceipt);
    }

    public class GoodsReceiptPublisher : IGoodsReceiptPublisher
    {
        private readonly IPublisher publisher;

        private readonly IProductRepository productRepository;

        public GoodsReceiptPublisher(IPublisher publisher, IProductRepository productRepository)
        {
            this.publisher = publisher;
            this.productRepository = productRepository;
        }

        public void PublishCreated(GoodsReceiptViewModel goodsReceipt)
        {
            var message = Mapper.Map<GoodsReceiptMessage>(goodsReceipt);

            var productIds =
                goodsReceipt.PurchaseOrders.SelectMany(p => p.Products)
                    .Where(p => p.QuantityReceived > 0)
                    .Select(p => p.ProductId)
                    .Distinct()
                    .ToList();

            var productMessages = productRepository.GetProductMessages(productIds);

            if (!productMessages.Any() && productIds.Any())
            {
                throw new Exception(
                    "No product messages found for ids " + string.Join(",", productIds.Select(x => x.ToString())));
            }

            message.Products = Mapper.Map<List<GoodsReceiptMessageProduct>>(productMessages).ToArray();

            foreach (var product in message.Products)
            {
                var productList =
                    goodsReceipt.PurchaseOrders.SelectMany(p => p.Products)
                        .Where(p2 => p2.ProductId == product.Id)
                        .ToList();
                product.Cost = productList.Sum(p => p.LastLandedCost * p.QuantityReceived);
                product.Units = productList.Sum(u => u.QuantityReceived);
            }
            publisher.Publish<Context, GoodsReceiptMessage>("Merchandising.GoodsReceiptCreated", message);
        }
    }
}
