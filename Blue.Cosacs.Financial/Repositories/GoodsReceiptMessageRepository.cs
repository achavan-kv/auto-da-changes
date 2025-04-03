namespace Blue.Cosacs.Financial.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Financial.Enums;
    using Messages.Merchandising.GoodsReceipt;

    public interface IGoodsReceiptMessageRepository
    {
        void Create(int messageId, GoodsReceiptMessage message);

        bool Exists(int messageId);

        IEnumerable<TransactionMappingMerchandisingView> GetAccounts();
    }

    public class GoodsReceiptMessageRepository : IGoodsReceiptMessageRepository
    {
        private readonly List<string> transactionDescriptions = new List<string>
        {
            TransactionTypes.GoodsReceivedForeign,
            TransactionTypes.GoodsReceivedLocal,
            TransactionTypes.GoodsReceivedParts
        };

        public void Create(int messageId, GoodsReceiptMessage message)
        {
            using (var scope = Context.Write())
            {
                var goodsReceiptMessage = Mapper.Map<Financial.GoodsReceiptMessage>(message);
                goodsReceiptMessage.MessageId = messageId;
                scope.Context.GoodsReceiptMessage.Add(goodsReceiptMessage);
                scope.Context.SaveChanges();

                var products = Mapper.Map<List<GoodsReceiptProductMessage>>(message.Products);
                products.Each(p => p.GoodsReceiptMessageId = goodsReceiptMessage.Id);
                scope.Context.GoodsReceiptProductMessage.AddRange(products);
                scope.Context.SaveChanges();

                scope.Complete();
            }
        }

        public bool Exists(int messageId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.GoodsReceiptMessage.Any(m => m.MessageId == messageId);
            }
        }

        public IEnumerable<TransactionMappingMerchandisingView> GetAccounts()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.TransactionMappingMerchandisingView
                    .Where(t => transactionDescriptions.Contains(t.TransactionDescription));
            }
        }
    }
}
