using System;
using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Warranty.Model;
using Blue.Events;
using Blue.Caching;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class WarrantyHierachyRepository
    {
        private IEventStore audit;
        private ICacheClient cache;
        private IWarrantyPriceRepository warrantyPriceRepositoy;

        private const string AuditEventCategory = "WarrantyHierarchy";

        public WarrantyHierachyRepository(IEventStore audit, ICacheClient cache, IWarrantyPriceRepository warrantyPriceRepositoy)
        {
            this.audit = audit;
            this.cache = cache;
            this.warrantyPriceRepositoy = warrantyPriceRepositoy;
        }

        public WarrantyLevel Save(WarrantyLevel level)
        {
            if (level != null && !string.IsNullOrWhiteSpace(level.Name) && level.Name.Length > 100)
            {
                level.Name = level.Name.Substring(0, 100);
            }

            if (level.Id == 0)
            {
                using (var scope = Context.Write())
                {
                    var newLevel = scope.Context.Level.Add(new Level { Name = level.Name });
                    scope.Context.SaveChanges();
                    scope.Complete();
                    level.Id = newLevel.Id;
                    audit.LogAsync(new { Name = level.Name, Id = level.Id }, EventType.CreateLevel, AuditEventCategory);
                }
            }
            else
            {
                using (var scope = Context.Write())
                {
                    var dbLevel = scope.Context.Level.Find(level.Id);
                    var oldName = dbLevel.Name;
                    dbLevel.Name = level.Name;
                    scope.Context.SaveChanges();
                    scope.Complete();
                    audit.LogAsync(new { NewName = level.Name, Id = level.Id, OldName = oldName }, EventType.EditLevel, AuditEventCategory);
                }
            }

            return level;
        }

        public List<int> DeleteLevel(int id)
        {
            using (var scope = Context.Write())
            {
                var dbLevel = scope.Context.Level.Find(id);
                var levelName = dbLevel.Name;

                var affectedWarranties = (from w in scope.Context.Warranty
                                          join wl in scope.Context.WarrantyTags on w.Id equals wl.WarrantyId
                                          where wl.LevelId == id
                                          select w.Id).ToList();

                scope.Context.WarrantyTags.Where(wt => wt.LevelId == id).ToList().ForEach(wt => scope.Context.WarrantyTags.Remove(wt));
                scope.Context.SaveChanges();

                scope.Context.Level.Remove(dbLevel);
                scope.Context.SaveChanges();
                scope.Complete();

                audit.LogAsync(new { Id = id, Name = levelName }, EventType.DeleteLevel, AuditEventCategory);
                warrantyPriceRepositoy.DeletePriceCalcViewCache();
                return affectedWarranties;
            }
        }

        public WarrantyTag Save(WarrantyTag tag)
        {
            if (tag != null && !string.IsNullOrWhiteSpace(tag.Name) && tag.Name.Length > 100)
            {
                tag.Name = tag.Name.Substring(0, 100);
            }

            if (tag.Id == 0)
            {
                using (var scope = Context.Write())
                {
                    var newTag = scope.Context.Tag.Add(new Tag { Name = tag.Name, LevelId = tag.Level.Id });
                    scope.Context.SaveChanges();
                    scope.Complete();
                    tag.Id = newTag.Id;

                    audit.LogAsync(new { Name = tag.Name, Id = tag.Id, LevelId = tag.Level.Id, LevelName = tag.Level.Name }, EventType.CreateTag, AuditEventCategory);
                }
            }
            else
            {
                using (var scope = Context.Write())
                {
                    var dbTag = scope.Context.Tag.Find(tag.Id);
                    var oldName = dbTag.Name;
                    dbTag.Name = tag.Name;
                    scope.Context.SaveChanges();
                    scope.Complete();

                    audit.LogAsync(new { Id = tag.Id, LevelId = tag.Level.Id, LevelName = tag.Level.Name, NewName = tag.Name, OldName = oldName }, EventType.EditTag, AuditEventCategory);
                }
            }

            return tag;
        }

        public List<int> DeleteTag(int id)
        {
            this.ValidateDeleteTag(id);
            using (var scope = Context.Write())
            {
                var dbTag = scope.Context.Tag.Find(id);
                var tagName = dbTag.Name;
                var level = dbTag.LevelId;
                var affectedWarranties = (from w in scope.Context.Warranty
                                          join wl in scope.Context.WarrantyTags on w.Id equals wl.WarrantyId
                                          where wl.TagId == id
                                          select w.Id).ToList();

                scope.Context.Tag.Remove(dbTag);
                scope.Context.SaveChanges();
                scope.Complete();

                audit.LogAsync(new { Id = id, Name = tagName, LevelId = level }, EventType.DeleteTag, AuditEventCategory);
                return affectedWarranties;
            }
        }

        private void ValidateDeleteTag(int id)
        {
            using (var scope = Context.Read())
            {
                if (scope.Context.WarrantyReturnFilter.Count(p => p.TagId == id) > 0)
                {
                    throw new ApplicationException("Cannot delete this Tag because it's linked to existing Filters on Warranty Return.");
                }

                if (scope.Context.WarrantyTags.Count(p => p.TagId == id) > 0)
                {
                    throw new ApplicationException("Cannot delete this Tag because it's linked Warranty.");
                }
            }
        }

        public List<WarrantyLevel> GetAllLevels()
        {
            List<WarrantyLevel> levels = new List<WarrantyLevel>();

            using (var scope = Context.Read())
            {
                levels = (from l in scope.Context.Level
                          select new WarrantyLevel
                              {
                                  Id = l.Id,
                                  Name = l.Name
                              }
                        ).OrderBy(l => l.Id).ToList();
            }

            return levels;
        }

        public List<WarrantyTag> GetAllTags()
        {
            List<WarrantyTag> tags = new List<WarrantyTag>();

            using (var scope = Context.Read())
            {
                tags = (from t in scope.Context.Tag
                        join l in scope.Context.Level on t.LevelId equals l.Id
                        select new WarrantyTag
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Level = new WarrantyLevel
                            {
                                Id = l.Id,
                                Name = l.Name
                            }
                        }).ToList();
            }

            return tags;
        }

        public List<WarrantySelection> GetTagSelection()
        {
            using (var scope = Context.Read())
            {
                var ws = (from l in scope.Context.Level
                          select new WarrantySelection
                          {
                              LevelId = l.Id,
                              LevelName = l.Name
                          }).OrderBy(l => l.LevelId).ToList();

                ws.ForEach(w =>
                {
                    w.Tags = (from t in scope.Context.Tag
                              where t.LevelId == w.LevelId
                              select t).ToList();
                });
                return ws;
            }
        }

        private class EventType
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
