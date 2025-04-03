using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Warranty.Model;
using Blue.Cosacs.Warranty.Solr;
using Blue.Events;
using System;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class WarrantyRepository
    {
        private IEventStore audit;
        private IWarrantyPriceRepository warrantyPriceRepository;
        private const string AuditEventCategory = "Warranty";

        public WarrantyRepository(IEventStore audit, IWarrantyPriceRepository warrantyPriceRepository)
        {
            this.audit = audit;
            this.warrantyPriceRepository = warrantyPriceRepository;
        }

        public Model.Warranty Get(int id)
        {
            using (var scope = Context.Read())
            {
                var w = scope.Context.Warranty.Find(id);
                if (w == null)
                    throw new Exception("Warranty not found. Please try reindexing.");
                var war = new Model.Warranty(w);
                war.WarrantyTags = (from wt in scope.Context.WarrantyTags
                                    join t in scope.Context.Tag on wt.TagId equals t.Id
                                    where wt.WarrantyId == id
                                    select new Model.Warranty.Tag
                                    {
                                        LevelId = wt.LevelId,
                                        TagName = t.Name
                                    }).ToList();
                war.RenewalChildren = (from r in scope.Context.Renewal
                                       join wa in scope.Context.Warranty on r.WarrantyId equals wa.Id
                                       where r.RenewalId == id
                                       select new Blue.Cosacs.Warranty.Model.Warranty.WarrantyRenewals
                                       {
                                           id = r.WarrantyId,
                                           text = wa.Number + " : " + wa.Description
                                       }).ToArray();
                war.RenewalParents = (from r in scope.Context.Renewal
                                      join wa in scope.Context.Warranty on r.RenewalId equals wa.Id
                                      where r.WarrantyId == id
                                      select new Blue.Cosacs.Warranty.Model.Warranty.WarrantyRenewals
                                      {
                                          id = r.RenewalId,
                                          text = wa.Number + " : " + wa.Description
                                      }).ToArray();
                return war;
            }
        }

        public bool Update(Model.Warranty warranty)
        {
            using (var scope = Context.Write())
            {
                if (scope.Context.Warranty
                    .Where(w => w.Number.ToLower() == warranty.Number.ToLower() &&
                                w.Id != warranty.Id).Any())
                    return true;

                var war = scope.Context.Warranty.Find(warranty.Id);
                var oldTags = (from wt in scope.Context.WarrantyTags
                               join t in scope.Context.Tag on wt.TagId equals t.Id
                               where wt.WarrantyId == war.Id
                               select new { wt.LevelId, t.Name }).ToList();

                audit.LogAsync(new { OldWarranty = war, OldWarrantyTags = oldTags, UpdatedWarranty = warranty }, EventType.EditWarranty, AuditEventCategory);
                war.TypeCode = warranty.TypeCode;
                war.Deleted = warranty.IsDeleted;
                war.Description = warranty.Description;
                war.Length = warranty.Length;
                war.Number = warranty.Number;
                war.TaxRate = warranty.TaxRate;
                scope.Context.SaveChanges();
                SaveTags(warranty.Id, warranty.WarrantyTags);
                SaveRenewals(warranty.RenewalChildren, war.Id);
                scope.Complete();
            }
            warrantyPriceRepository.DeletePriceCalcViewCache();
            return false;
        }

        private void SaveTags(int warrantyId, List<Model.Warranty.Tag> tags)
        {
            if (tags == null || tags.Count <= 0)
            {
                return;
            }

            using (var scope = Context.Write())
            {
                var oldTags = scope.Context.WarrantyTags.Where(w => w.WarrantyId == warrantyId).ToList();
                oldTags.ForEach(f => { scope.Context.WarrantyTags.Remove(f); });

                tags.ForEach(t =>
                             {
                                 if (t.TagId > 0)
                                 {
                                     var found = scope.Context.Tag.Where(w => w.LevelId == t.LevelId && t.TagName == w.Name).Any();
                                     if (!found)
                                     {
                                         scope.Context.Tag.Add(new Tag
                                             {
                                                 LevelId = t.LevelId,
                                                 Name = t.TagName
                                             });
                                     }
                                 }
                             });
                scope.Context.SaveChanges();

                var tagTable = scope.Context.Tag.ToList();

                var wtags = (from t in tagTable
                             join wt in tags on t.LevelId equals wt.LevelId
                             where wt.TagName == t.Name
                             select new { t, wt }).ToList();

                wtags.ForEach(w =>
                {
                    scope.Context.WarrantyTags.Add(new WarrantyTags
                    {
                        LevelId = w.wt.LevelId,
                        TagId = w.t.Id,
                        WarrantyId = warrantyId
                    });
                });
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public int Create(Model.Warranty warranty)
        {
            int id;
            using (var scope = Context.Write())
            {
                if (scope.Context.Warranty.Where(w => w.Number.ToLower() == warranty.Number.ToLower()).Any())
                    return -1;

                var newWarranty = new Warranty
                    {
                        Deleted = warranty.IsDeleted,
                        Description = warranty.Description,
                        TypeCode = warranty.TypeCode,
                        Id = warranty.Id,
                        Length = warranty.Length,
                        Number = warranty.Number,
                        TaxRate = warranty.TaxRate
                    };
                scope.Context.Warranty.Add(newWarranty);
                scope.Context.SaveChanges();
                id = newWarranty.Id;
                SaveTags(id, warranty.WarrantyTags);
                if (newWarranty.TypeCode == WarrantyType.Extended)
                {
                    SaveRenewals(warranty.RenewalChildren, id);
                }
                scope.Complete();

                audit.LogAsync(new { WarrantyNumber = warranty.Number, Id = newWarranty.Id }, EventType.CreateWarranty, AuditEventCategory);
            }
            warrantyPriceRepository.DeletePriceCalcViewCache();
            return id;
        }

        private void SaveRenewals(IEnumerable<Blue.Cosacs.Warranty.Model.Warranty.WarrantyRenewals> relations, int warrantyId)
        {
            using (var scope = Context.Write())
            {
                var renewals = scope.Context.Renewal.Where(r => r.RenewalId == warrantyId).ToList();
                renewals.ForEach(r =>
                {
                    scope.Context.Renewal.Remove(r);
                });

                if (relations != null)
                {
                    relations.ToList().ForEach(r =>
                    {
                        scope.Context.Renewal.Add(new Renewal
                            {
                                RenewalId = warrantyId,
                                WarrantyId = r.id
                            });
                    });
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IEnumerable<int> ChangeWarrantyTag(int existingTagId, int newTagId)
        {
            var warrantyIds = new List<int>();

            using (var scope = Context.Write())
            {
                var warranties = scope.Context.WarrantyTags.Where(wt => wt.TagId == existingTagId);
                foreach (var warranty in warranties)
                {
                    warranty.TagId = newTagId;
                    warrantyIds.Add(warranty.WarrantyId);
                }

                scope.Context.SaveChanges();
                scope.Complete();

                audit.LogAsync(new { ExistingTagId = existingTagId, NewTagId = newTagId }, EventType.ChangeWarrantyTag, AuditEventCategory);
            }

            return warrantyIds;
        }

        public IEnumerable<int> AssignWarrantyTag(int levelId, int tagId)
        {
            var warrantyIds = new List<int>();
            using (var scope = Context.Write())
            {
                var warranties = scope.Context.WarrantyTags.Where(wt => wt.LevelId == levelId && !wt.TagId.HasValue);
                foreach (var warranty in warranties)
                {
                    warranty.TagId = tagId;
                    warrantyIds.Add(warranty.WarrantyId);
                }

                scope.Context.SaveChanges();
                scope.Complete();

                audit.LogAsync(new { LevelId = levelId, TagId = tagId }, EventType.AssignWarrantyTag, AuditEventCategory);
            }

            return warrantyIds;
        }

        private class EventType
        {
            public const string CreateWarranty = "CreateWarranty";
            public const string EditWarranty = "EditWarranty";
            public const string ChangeWarrantyTag = "ChangeWarrantyTag";
            public const string AssignWarrantyTag = "AssignWarrantyTag";
        }
    }
}