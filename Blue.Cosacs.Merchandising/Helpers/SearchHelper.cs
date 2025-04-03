namespace Blue.Cosacs.Merchandising.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    public interface IHierarchyType
    {
        int LevelId { get; set; }
        string Tag { get; set; }
    }

    public interface ITaggedType
    {
        string Tags { get; set; }
    }

    public static class SearchHelper
    {
        public static List<int> ItemsInHierarchy<T>(this IQueryable<T> query, Dictionary<string, string> hierarchy, Func<T, int> idProperty) where T : IHierarchyType
        {
            query = hierarchy.Aggregate(query, (current, level) => current.Where(p => p.LevelId == int.Parse(level.Key) && p.Tag == level.Value));
            return query.Select(idProperty).ToList();
        }

        public static List<int> ItemsInHierarchy<T>(this IQueryable<T> query, Dictionary<int, string> hierarchy, Func<T, int> idProperty) where T : IHierarchyType
        {
            query = hierarchy.Aggregate(query, (current, level) => current.Where(p => p.LevelId == level.Key && p.Tag == level.Value));
            return query.Select(idProperty).ToList();
        }

        public static List<int> ItemsWithTags<T>(this IQueryable<T> query, List<string> tags, Func<T, int> idProperty) where T : ITaggedType
        {
            query = query.Where(o => o.Tags != null &&
                ((List<string>)new JsonSerializer().Deserialize(new StringReader(o.Tags), typeof(List<string>))).Any(t => tags.Any(s => s == t)));
            return query.Select(idProperty).ToList();
        }
    }
}
