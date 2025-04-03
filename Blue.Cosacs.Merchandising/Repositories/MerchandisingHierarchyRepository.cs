namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using EntityFramework.BulkInsert.Extensions;
    using System.Collections.Generic;
    using System.Linq;
    using Model = Blue.Cosacs.Merchandising.Models;


    public interface IMerchandisingHierarchyRepository
    {
        Level Save(Level level);

        void DeleteLevel(int id);

        Tag Save(Tag tag, string code = null);

        void Save(List<Model.HierarchyTagImport> tags);

        void DeleteTag(int id);

        List<LevelModel> Get(int? levelId = null);

        IEnumerable<Level> GetAllLevels();

        List<Tag> GetAllTags();

        IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetSortedList();

        string CanDeleteTag(int id);

        string GetSelectionString(Dictionary<string, string> hierarchySelections);

        Dictionary<int, decimal> ProductFirstYearWarrantyProvision(List<int> productFilter);
        Dictionary<string, int> GetHierarchyIds(int productId);
        Dictionary<string, string> GetHierarchy(int productId);
    }

    public class MerchandisingHierarchyRepository : IMerchandisingHierarchyRepository
    {
        private readonly IEventStore audit;

        public MerchandisingHierarchyRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public Level Save(Level level)
        {
            using (var scope = Context.Write())
            {
                if (level != null && !string.IsNullOrWhiteSpace(level.Name) && level.Name.Length > 100)
                {
                    level.Name = level.Name.Substring(0, 100);
                }

                if (level.Id == 0)
                {
                    var newLevel = scope.Context.HierarchyLevel.Add(new HierarchyLevel { Name = level.Name });
                    scope.Context.SaveChanges();

                    level.Id = newLevel.Id;
                    audit.LogAsync(new { Name = level.Name, Id = level.Id }, EventType.CreateLevel, EventCategories.Merchandising);

                }
                else
                {
                    var dbLevel = scope.Context.HierarchyLevel.Find(level.Id);
                    var oldName = dbLevel.Name;
                    dbLevel.Name = level.Name;
                    scope.Context.SaveChanges();

                    audit.LogAsync(new { NewName = level.Name, Id = level.Id, OldName = oldName }, EventType.EditLevel, EventCategories.Merchandising);
                }

                scope.Complete();
            }

            return level;
        }

        public void DeleteLevel(int id)
        {
            using (var scope = Context.Write())
            {
                var dbLevel = scope.Context.HierarchyLevel.Find(id);
                var levelName = dbLevel.Name;

                scope.Context.HierarchyLevel.Remove(dbLevel);
                scope.Context.SaveChanges();


                audit.LogAsync(new { Id = id, Name = levelName }, EventType.DeleteLevel, EventCategories.Merchandising);
                scope.Complete();

            }
        }

        public Tag Save(Tag tag, string code = null)
        {
            using (var scope = Context.Write())
            {
                if (tag != null && !string.IsNullOrWhiteSpace(tag.Name) && tag.Name.Length > 100)
                {
                    tag.Name = tag.Name.Substring(0, 100);
                }

                HierarchyTag existingTagWithCode = scope.Context.HierarchyTag.FirstOrDefault(t => t.Code == code);

                if (tag.Id == 0 && existingTagWithCode == null)
                {
                    var newTag =
                        scope.Context.HierarchyTag.Add(new HierarchyTag { Name = tag.Name, LevelId = tag.Level.Id, Code = string.IsNullOrEmpty(code) ? string.Empty : code });
                    scope.Context.SaveChanges();
                    tag.Id = newTag.Id;

                    audit.LogAsync(
                        new { Name = tag.Name, Id = tag.Id, LevelId = tag.Level.Id, LevelName = tag.Level.Name, Code = code },
                        EventType.CreateTag,
                        EventCategories.Merchandising);
                }
                else
                {
                    var dbTag = scope.Context.HierarchyTag.Find(tag.Id);

                    if (existingTagWithCode != null)
                        dbTag = existingTagWithCode;
                    var oldName = dbTag.Name;
                    dbTag.Name = tag.Name;
                    scope.Context.SaveChanges();

                    audit.LogAsync(
                        new { Id = tag.Id, LevelId = tag.Level.Id, LevelName = tag.Level.Name, NewName = tag.Name, OldName = oldName },
                        EventType.EditTag,
                        EventCategories.Merchandising);
                }

                scope.Complete();
            }

            return tag;
        }

        public void Save(List<Model.HierarchyTagImport> tags)
        {
            using (var scope = Context.Write())
            {

                var levels = scope.Context.HierarchyLevel.ToDictionary(d => d.Name, d => d.Id);
                var existingTags = scope.Context.HierarchyTag.ToList();

                var newtags = tags.Where(t => !string.IsNullOrWhiteSpace(t.Name)).Select(t =>
                {
                    t.Name = t.Name.Length > 100 ? t.Name.Substring(0, 100) : t.Name;
                    t.LevelId = levels[t.Level];
                    return t;
                }).ToList().GroupBy(c => c.Code).Select(t => new HierarchyTag { Name = t.Max(x => x.Name), LevelId = t.Max(x => x.LevelId), Code = t.Key }).ToList();
                // Lets max just incase duplicates

                // Set tag name if exists.
                existingTags.ForEach(f =>
                {
                    f.Name = newtags.Where(n => n.Code == f.Code).Select(s => s.Name).FirstOrDefault() ?? f.Name;
                });

                // Add new tag if not exists.
                existingTags.AddRange(newtags.Where(t => !existingTags.Any(x => t.Code == x.Code)));
                scope.Context.SaveChanges();

                // Insert product hierarchy.
                scope.Context.BulkInsert((from e in existingTags
                                          join t in tags on e.Code equals t.Code
                                          select new ProductHierarchyStaging
                                          {
                                              ProductId = t.ProductId,
                                              HierarchyLevelId = t.LevelId,
                                              HierarchyTagId = e.Id
                                          }).ToList());
                scope.Context.SaveChanges();
                scope.Context.BulkHierarchySave();
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void DeleteTag(int id)
        {
            using (var scope = Context.Write())
            {
                var dbTag = scope.Context.HierarchyTag.Find(id);
                var tagName = dbTag.Name;
                var level = dbTag.LevelId;

                scope.Context.HierarchyTag.Remove(dbTag);
                scope.Context.SaveChanges();
                audit.LogAsync(new { Id = id, Name = tagName, LevelId = level }, EventType.DeleteTag, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public List<LevelModel> Get(int? levelId = null)
        {
            using (var scope = Context.Read())
            {
                var values = scope.Context.TagView
                    .Select(p => p);

                if (levelId.HasValue)
                {
                    values = values
                        .Where(p => p.LevelId == levelId.Value);
                }

                return values
                    .OrderBy(t => t.LevelId)
                    .ThenBy(t => t.Id)
                    .ToList()
                    .GroupBy(t => t.LevelId)
                    .Select(g =>
                    {
                        var level = Mapper.Map<LevelModel>(g.First());
                        level.Tags = g.Select(Mapper.Map<TagModel>).ToList();
                        return level;
                    })
                    .ToList();
            }
        }

        public string GetSelectionString(Dictionary<string, string> hierarchySelections)
        {
            using (var scope = Context.Read())
            {
                var levels = GetAllLevels();
                if (hierarchySelections == null)
                {
                    return string.Join(", ", levels.Select(l => string.Format("{0}: {1}", l.Name, "Any")));
                }

                var tagIds = hierarchySelections.Select(h => int.Parse(h.Value)).ToList();
                var tags = scope.Context.TagView.Where(t => tagIds.Contains(t.Id)).ToList();
                return string.Join(
                    ", ",
                    levels.Select(
                        l =>
                        {
                            var sel = hierarchySelections != null
                                          ? tags.FirstOrDefault(hs => hs.Id == l.Id)
                                          : null;

                            return string.Format("{0}: {1}", l.Name, sel == null ? "Any" : sel.TagName);
                        }));
            }
        }

        public IEnumerable<Level> GetAllLevels()
        {
            using (var scope = Context.Read())
            {
                return (from l in scope.Context.HierarchyLevel
                        select new Level
                        {
                            Id = l.Id,
                            Name = l.Name
                        })
                       .ToList()
                       .OrderBy(l => l.Id)
                       .ToList();
            }
        }

        public List<Tag> GetAllTags()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.TagView
                    .OrderBy(t => t.Id)
                    .Select(Mapper.Map<Tag>)
                    .ToList();
            }
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetSortedList()
        {
            return GetAllTags()
                .GroupBy(l => new { l.Level.Id, l.Level.Name })
                .Select(g => new KeyValuePair<string, IEnumerable<string>>(
                    g.Key.Name.ToLower(),
                    g.Select(t => t.Name)));
        }

        public string CanDeleteTag(int id)
        {
            string message = null;
            using (var scope = Context.Read())
            {
                if (scope.Context.ProductHierarchy.Any(p => p.HierarchyTagId == id))
                {
                    message = "Products";
                }
                if (scope.Context.PromotionHierarchy.Any(p => p.TagId == id))
                {
                    message = string.IsNullOrEmpty(message) ? "Promotions" : message + " and Promotions";
                }
                if (scope.Context.StockCountHierarchyView.Any(p => p.HierarchyTagId == id))
                {
                    message = string.IsNullOrEmpty(message) ? "Stock Counts" : message + " and Stock Counts";
                }
            }
            return message;
        }

        public Dictionary<int, decimal> ProductFirstYearWarrantyProvision(List<int> productFilter)
        {
            using (var scope = Context.Read())
            {
                return (from l in scope.Context.HierarchyLevel
                        join ph in scope.Context.ProductHierarchy
                            on l.Id equals ph.HierarchyLevelId
                        join t in scope.Context.HierarchyTag
                            on ph.HierarchyTagId equals t.Id
                        where l.Name == "Division" && productFilter.Contains(ph.ProductId)
                        select new
                        {
                            ProductId = ph.ProductId,
                            Percentage = t.FirstYearWarrantyProvision == null ? 1M : t.FirstYearWarrantyProvision
                        })
                        .ToList()
                        .ToDictionary(k => k.ProductId, v => v.Percentage.Value);
            }
        }

        public Dictionary<string, int> GetHierarchyIds(int productId)
        {
            //Dictionary<int, int> is not supported for serialization/deserialization of a dictionary, keys must be strings or objects.
            //So have used Dictionary<string, int>
            using (var scope = Context.Read())
            {
                var data = from ph in scope.Context.ProductHierarchy
                           join hl in scope.Context.HierarchyLevel
                                on ph.HierarchyLevelId equals hl.Id
                           join ht in scope.Context.HierarchyTag
                           on ph.HierarchyTagId equals ht.Id
                           where ph.ProductId == productId
                           select new
                           {
                               Key = hl.Id.ToString(),
                               Value = ht.Id
                           };
                var seeme = data.ToDictionary(x => x.Key, x => x.Value);
                return seeme;
            }
        }

        public Dictionary<string, string> GetHierarchy(int productId)
        {
            using (var scope = Context.Read())
            {
                var data = from ph in scope.Context.ProductHierarchy
                           join hl in scope.Context.HierarchyLevel
                           on ph.HierarchyLevelId equals hl.Id
                           join ht in scope.Context.HierarchyTag
                           on ph.HierarchyTagId equals ht.Id
                           where ph.ProductId == productId
                           select new
                           {
                               Key = hl.Name.ToLower(),
                               Value = ht.Name
                           };
                return data.ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private static class EventType
        {
            public const string CreateLevel = "CreateLevel";
            public const string EditLevel = "EditLevel";
            public const string DeleteLevel = "DeleteLevel";
            public const string CreateTag = "CreateTag";
            public const string EditTag = "EditTag";
            public const string DeleteTag = "DeleteTag";
        }
    }
}
