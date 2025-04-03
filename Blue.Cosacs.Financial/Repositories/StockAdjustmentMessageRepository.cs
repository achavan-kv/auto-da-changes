namespace Blue.Cosacs.Financial.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Financial.Enums;
    using Messages.Merchandising.StockAdjustment;

    public interface IStockAdjustmentMessageRepository
    {
        void Create(int messageId, StockAdjustmentMessage message);

        bool Exists(int messageId);

        IEnumerable<TransactionMappingMerchandisingView> GetAccounts();
    }

    public class StockAdjustmentMessageRepository : IStockAdjustmentMessageRepository
    {
        public void Create(int messageId, StockAdjustmentMessage message)
        {
            using (var scope = Context.Write())
            {
                var stockAdjustmentMessage = Mapper.Map<Financial.StockAdjustmentMessage>(message);
                stockAdjustmentMessage.MessageId = messageId;
                scope.Context.StockAdjustmentMessage.Add(stockAdjustmentMessage);
                scope.Context.SaveChanges();

                var products = Mapper.Map<List<StockAdjustmentProductMessage>>(message.Products);
                products.Each(p => p.StockAdjustmentMessageId = stockAdjustmentMessage.Id);
                scope.Context.StockAdjustmentProductMessage.AddRange(products);
                scope.Context.SaveChanges();

                scope.Complete();
            }
        }

        public bool Exists(int messageId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.StockAdjustmentMessage.Any(m => m.MessageId == messageId);
            }
        }

        public IEnumerable<TransactionMappingMerchandisingView> GetAccounts()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.TransactionMappingMerchandisingView
                    .Where(t => t.AdjustmentType != null && t.TransactionDescription == TransactionTypes.StockAdjustment);
            }
        }
    }
}
