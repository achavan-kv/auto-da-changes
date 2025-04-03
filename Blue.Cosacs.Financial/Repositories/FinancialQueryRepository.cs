namespace Blue.Cosacs.Financial.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using Blue.Cosacs.Financial.Models;

    public interface IFinancialQueryRepository
    {
        PagedSearchResult<FinancialQueryViewModel> Search(FinanacialQueryQueryModel query, int pageSize, int pageIndex);

        List<FinancialQueryViewModel> Search(FinanacialQueryQueryModel query);
    }

    public class FinancialQueryRepository : IFinancialQueryRepository
    {
        public PagedSearchResult<FinancialQueryViewModel> Search(FinanacialQueryQueryModel query, int pageSize, int pageIndex)
        {
            var q = SearchQuery(query);

            var results = q
                .OrderBy(x => x.Id).Skip(pageSize * pageIndex).Take(pageSize)
                .Project().To<FinancialQueryViewModel>()
                .ToList();

            return new PagedSearchResult<FinancialQueryViewModel> { Page = results, Count = q.Count() };
        }

        public List<FinancialQueryViewModel> Search(FinanacialQueryQueryModel query)
        {
            return SearchQuery(query)
                .Project().To<FinancialQueryViewModel>()
                .ToList();
        }

        private IQueryable<TransactionView> SearchQuery(FinanacialQueryQueryModel query)
        {
            using (var scope = Context.Read())
            {
                var trans = scope.Context.TransactionView.AsNoTracking().AsQueryable();
                if (!string.IsNullOrEmpty(query.TransactionCode))
                {
                    trans = trans.Where(x => x.Type == query.TransactionCode);
                }
                if (!string.IsNullOrEmpty(query.AccountNumber))
                {
                    trans = trans.Where(x => x.Account == query.AccountNumber);
                }
                if (query.LocationId.HasValue)
                {
                    trans = trans.Where(x => x.LocationId == query.LocationId);
                }
                if (query.RunNumber.HasValue)
                {
                    trans = trans.Where(x => x.RunNo == query.RunNumber);
                }
                if (query.FromDate.HasValue)
                {
                    trans = trans.Where(x => x.Date >= query.FromDate.Value);
                }
                if (query.ToDate.HasValue)
                {
                   var dateTo = query.ToDate.Value.AddDays(1);
                   trans = trans.Where(x => x.Date < dateTo);
                }
                return trans;
            }
        }
    }
}
