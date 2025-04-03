namespace Blue.Cosacs.Financial.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Financial.Enums;
    using Messages.Merchandising.VendorReturn;

    public interface IVendorReturnMessageRepository
    {
        void Create(int messageId, VendorReturnMessage message);

        bool Exists(int messageId);

        List<TransactionMappingMerchandisingView> GetDepartments();
    }

    public class VendorReturnMessageRepository : IVendorReturnMessageRepository
    {
        private readonly List<string> transactionDescriptions = new List<string>
        {
            TransactionTypes.VendorReturnForeign,
            TransactionTypes.VendorReturnLocal,
            TransactionTypes.VendorReturnParts
        };

        public void Create(int messageId, VendorReturnMessage message)
        {
            using (var scope = Context.Write())
            {
                var vendorReturnMessage = Mapper.Map<Financial.VendorReturnMessage>(message);
                vendorReturnMessage.MessageId = messageId;
                scope.Context.VendorReturnMessage.Add(vendorReturnMessage);
                scope.Context.SaveChanges();

                var products = Mapper.Map<List<VendorReturnProductMessage>>(message.Products);
                products.Each(p => p.VendorReturnMessageId = vendorReturnMessage.Id);
                scope.Context.VendorReturnProductMessage.AddRange(products);
                scope.Context.SaveChanges();

                scope.Complete();
            }
        }

        public bool Exists(int messageId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.VendorReturnMessage.Any(m => m.MessageId == messageId);
            }
        }
        
        public List<TransactionMappingMerchandisingView> GetDepartments()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.TransactionMappingMerchandisingView
                    .Where(t => transactionDescriptions.Contains(t.TransactionDescription))
                    .ToList();
            }
        }
    }
}
