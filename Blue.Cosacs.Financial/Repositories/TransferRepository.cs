namespace Blue.Cosacs.Financial.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Financial.Enums;
    using Messages.Merchandising.Transfer;

    public interface ITransferMessageRepository
    {
        void Create(int messageId, TransferMessage message);

        bool Exists(int messageId);

        List<TransactionMappingMerchandisingView> GetAccounts();
    }

    public class TransferMessageRepository : ITransferMessageRepository
    {
        public void Create(int messageId, TransferMessage message)
        {
            using (var scope = Context.Write())
            {
                var transferMessage = Mapper.Map<Financial.TransferMessage>(message);
                transferMessage.MessageId = messageId;
                scope.Context.TransferMessage.Add(transferMessage);
                scope.Context.SaveChanges();

                var products = Mapper.Map<List<TransferProductMessage>>(message.Products);
                products.Each(p => p.TransferMessageId = transferMessage.Id);
                scope.Context.TransferProductMessage.AddRange(products);
                scope.Context.SaveChanges();

                scope.Complete();
            }
        }

        public bool Exists(int messageId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.TransferMessage.Any(m => m.MessageId == messageId);
            }
        }

        public List<TransactionMappingMerchandisingView> GetAccounts()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.TransactionMappingMerchandisingView
                    .Where(t => t.TransactionDescription == TransactionTypes.StockTransfer)
                    .ToList();
            }
        }
    }
}
