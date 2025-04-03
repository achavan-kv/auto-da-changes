using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Merchandising.Helpers
{
    public static class EnumerableExtensions 
    {
        public static bool AllDistinct<T>(this IEnumerable<T> input) 
        {
            var count = input.Count();
            return count == input.Distinct().Count();
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> inputItems, Func<T, T, bool> equalityFunc)  
        {
            var result = new List<T>();
            foreach (var item in inputItems)
            {
                if (result.All(i => !equalityFunc(i, item)))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static T? MinOrDefault<T>(this IEnumerable<T> sequence) where T: struct
        {
            return sequence.Select(v => (T?) v).DefaultIfEmpty().Min();
        }

        public static T? MaxOrDefault<T>(this IEnumerable<T> sequence) where T : struct
        {
            return sequence.Select(v => (T?)v).DefaultIfEmpty().Max();
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            if (chunksize > 0 && source != null)
            {
                while (source.Any())
                {
                    yield return source.Take(chunksize);
                    source = source.Skip(chunksize);
                }
            }
        }
    }
}
