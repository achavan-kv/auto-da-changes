namespace Blue.Cosacs.Merchandising.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Blue.Cosacs.Merchandising.Models;

    public interface ITransactionTypeRepository
    {
        List<TransactionTypeViewModel> Get();

        void Update(List<TransactionTypeUpdateModel> model);
    }

    public class TransactionTypeRepository : ITransactionTypeRepository
    {
        public List<TransactionTypeViewModel> Get()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.TransactionType
                    .Project()
                    .To<TransactionTypeViewModel>()
                    .ToList();
            }
        }

        public void Update(List<TransactionTypeUpdateModel> model)
        {
            using (var scope = Context.Write())
            {
                var transactionTypes = scope.Context.TransactionType.ToList();

                model.ForEach(mtt => Mapper.Map(mtt, transactionTypes.Single(tt => tt.Id == mtt.Id)));

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
    }
}