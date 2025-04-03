namespace Blue.Cosacs.Financial.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Financial.Enums;
    using Messages.Merchandising.CintOrderReceipt;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICintOrderReceiptMessageRepository
    {
        void Create(int messageId, CintOrderReceiptMessage message);

        bool Exists(int messageId);

        List<TransactionMappingMerchandisingView> GetDeliveredAccounts();

        List<TransactionMappingMerchandisingView> GetCostOfSaleAccounts();
    }

    public class CintOrderReceiptMessageRepository : ICintOrderReceiptMessageRepository
    {
        private readonly List<string> transactionDescriptions = new List<string>
        {
            TransactionTypes.CostOfSale,
            TransactionTypes.FywProvisioning
        };

        public void Create(int messageId, CintOrderReceiptMessage message)
        {
            using (var scope = Context.Write())
            {
                var cintOrderReceiptMessage = Mapper.Map<Financial.CintOrderReceiptMessage>(message);
                cintOrderReceiptMessage.MessageId = messageId;
                scope.Context.CintOrderReceiptMessage.Add(cintOrderReceiptMessage);
                scope.Context.SaveChanges();

                scope.Complete();
            }
        }

        public bool Exists(int messageId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.CintOrderReceiptMessage.Any(m => m.MessageId == messageId);
            }
        }

        public List<TransactionMappingMerchandisingView> GetDeliveredAccounts()
        {
            using (var scope = Context.Read())
            {
                var accounts = scope.Context.TransactionMappingMerchandisingView
                    .Where(t => transactionDescriptions.Contains(t.TransactionDescription))
                    .ToList();

                if (accounts.Count() != transactionDescriptions.Count * 2)
                {
                    throw new Exception("Credit and debit accounts have not been configured correctly.");
                }

                return accounts;
            }
        }

        public List<TransactionMappingMerchandisingView> GetCostOfSaleAccounts()
        {
            using (var scope = Context.Read())
            {
                var accounts = scope.Context.TransactionMappingMerchandisingView
                    .Where(t => TransactionTypes.CostOfSale == t.TransactionDescription)
                    .ToList();

                if (accounts.Count() != 2)
                {
                    throw new Exception("Credit and debit accounts have not been configured correctly.");
                }

                return accounts;
            }
        }
    }
}
