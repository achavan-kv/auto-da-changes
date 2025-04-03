namespace Blue.Cosacs.Merchandising.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class IQueryableExtensions
    {
        public static IQueryable<TSource> DistinctBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return source.GroupBy(keySelector)
                    .Select(g => g.FirstOrDefault());
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return source.AsQueryable().GroupBy(keySelector)
                    .Select(g => g.FirstOrDefault());
        }
    }
}
