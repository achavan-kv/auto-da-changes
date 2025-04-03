namespace Blue.Cosacs.Financial.Repositories
{
    using System.Collections.Generic;

    public interface ITransactionRepository
    {
        void Create(List<Transaction> transaction);
    }

    public class TransactionRepository : ITransactionRepository
    {
        public void Create(List<Transaction> transaction)
        {
            using (var scope = Context.Write())
            {
                scope.Context.Transaction.AddRange(transaction);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
    }
}
