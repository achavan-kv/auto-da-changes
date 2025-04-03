using System.Linq;

namespace Blue.Cosacs.Test.Helpers
{
    using System.Collections.Generic;

    public static class ObjectExtensions
    {
        public static IEnumerable<string> GetNullPropertyNames<T>(this T obj, IEnumerable<string> excluded = null)
        {
            if (excluded == null)
            {
                return obj.GetType().GetProperties().Where(p => p.GetValue(obj, null) == null).Select(p => p.Name);
            }

            return obj.GetType().GetProperties().Where(p => !excluded.Contains(p.Name) && p.GetValue(obj, null) == null).Select(p => p.Name);
        }

        public static IEnumerable<string> AndExclude(this List<string> lst, IEnumerable<string> excluded)
        {
            lst.AddRange(excluded);
            return lst;
        }

        public static IEnumerable<string> AndExclude(this List<string> lst, string excluded)
        {
            lst.Add(excluded);
            return lst;
        }
    }
}
