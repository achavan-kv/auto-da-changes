namespace Blue.Cosacs.Merchandising.Publishers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Hub.Client;
    using Messages.Merchandising.StockAdjustment;
    using Blue.Cosacs.Merchandising.Helpers;

    public interface IStockAdjustmentPublisher
    {
        void PublishCreated(StockAdjustmentViewModel stockAdjustment);
    }

    public class StockAdjustmentPublisher : IStockAdjustmentPublisher
    {
        private readonly IPublisher publisher;

        private readonly IProductRepository productRepository;

        public StockAdjustmentPublisher(IPublisher publisher, IProductRepository productRepository)
        {
            this.publisher = publisher;
            this.productRepository = productRepository;
        }

        public void PublishCreated(StockAdjustmentViewModel stockAdjustment)
        {
            var message = Mapper.Map<StockAdjustmentMessage>(stockAdjustment);

            var products = productRepository.GetProductMessages(stockAdjustment.Products.Select(p => p.ProductId).Distinct().ToList());
            if (!products.Any())
            {
                throw new Exception(
                    "No product messages found for ids " + string.Join(",", products.Select(x => x.Id.ToString())));
            }
            else if (products.Where(r => r.DepartmentCode == null).Any())
            {
                throw new Exception(
                  "Legacy code is not set for the Product Ids: "
                        + string.Join(",", products.Where(r => r.DepartmentCode == null).Select(x => x.Id)));
            }

            message.Products = Mapper.Map<List<StockAdjustmentMessageProduct>>(products).ToArray();

            var totals = (from p in stockAdjustment.Products
                          group p by p.ProductId into g
                          select new
                          {
                              key = g.Key,
                              cost = g.Sum(c => c.AverageWeightedCost * c.Quantity),
                              units = g.Sum(u => u.Quantity)
                          }).ToDictionary(d => d.key);

            message.Products = message.Products.Select(x =>
            {
                x.Cost = totals[x.Id].cost;
                x.Units = totals[x.Id].units;
                return x;
            }).ToArray();

            publisher.Publish("Merchandising.StockAdjustmentCreated", message);
        }
    }
}
