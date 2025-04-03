using Blue.Cosacs.Merchandising.Models;

namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Events;
    using System.Collections.Generic;
    using System.Data.Entity.Core;
    using System.Linq;

    public interface ITagValuesRepository
    {
        IEnumerable<Tag> Get();

        Tag SaveRepossessedConditions(Tag model);

        Tag Save(Tag model);
    }

    public class TagValuesRepository : ITagValuesRepository
    {
        private readonly IEventStore audit;
        private readonly IRepossessedConditionsRepository conditionsRepository;

        public TagValuesRepository(IEventStore audit, IRepossessedConditionsRepository conditionsRepository)
        {
            this.audit = audit;
            this.conditionsRepository = conditionsRepository;
        }

        public IEnumerable<Tag> Get()
        {
            using (var scope = Context.Read())
            {
                var tags = scope.Context.TagView.Project().To<Tag>().ToList();
                var tagConditions = scope.Context.HierarchyTagCondition.ToList();
                var conditions = scope.Context.RepossessedCondition.ToList();
                foreach (var t in tags)
                {
                    // insert conditions from database where condition exists in settings
                    var t1 = t;

                    t.RepossessedConditions =
                        tagConditions.Where(tc => tc.HierarchyTagId == t1.Id)
                            .Select(
                                c =>
                                new Blue.Cosacs.Merchandising.Models.HierarchyTagCondition
                                    {
                                        HierarchyTagId = c.HierarchyTagId,
                                        Id = c.Id,
                                        Percentage = c.Percentage,
                                        RepossessedConditionId = c.RepossessedConditionId,
                                        ConditionName = conditions.First(rc => rc.Id == c.RepossessedConditionId).Name
                                    })
                            .ToList();

                    // insert conditions from settings where not already in collection
                    t.RepossessedConditions.AddRange(
                        conditions.Where(c => t.RepossessedConditions.All(r => r.RepossessedConditionId != c.Id))
                            .Select(
                                c =>
                                new Blue.Cosacs.Merchandising.Models.HierarchyTagCondition
                                    {
                                        RepossessedConditionId = c.Id,
                                        HierarchyTagId = t.Id,
                                        ConditionName = conditions.First(rc => rc.Id == c.Id).Name
                                    }));
                }
                return tags;
            }
        }

        public Tag SaveRepossessedConditions(Tag model)
        {
            using (var scope = Context.Write())
            {
                scope.Context.HierarchyTagCondition.RemoveRange(scope.Context.HierarchyTagCondition.Where(w => w.HierarchyTagId == model.Id));
                scope.Context.SaveChanges();

                // insert new conditions
                if (model.RepossessedConditions != null && model.RepossessedConditions.Any(a => a.Percentage.HasValue))
                {
                    var conditionSettings = conditionsRepository.Get().ToList();
                    var newConditions = model.RepossessedConditions
                        .Where(c => conditionSettings.Select(cs => cs.Id).Contains(c.RepossessedConditionId))
                        .Where(c => c.Percentage != null);

                    scope.Context.HierarchyTagCondition.AddRange(newConditions.Select(Mapper.Map<HierarchyTagCondition>));
                    scope.Context.SaveChanges();
                }
              
                this.audit.LogAsync(model, WarrantyHierarchyEvents.EditTagRepossessConditions, EventCategories.Merchandising);
                scope.Complete();
                return model;
            }
        }

        public Tag Save(Tag model)
        {
            using (var scope = Context.Write())
            {
                var tag = scope.Context.HierarchyTag.Find(model.Id);
                if (tag == null)
                    throw new ObjectNotFoundException();

                tag.FirstYearWarrantyProvision = model.FirstYearWarrantyProvision;
                tag.AgedAfter = model.AgedAfter;

                scope.Context.SaveChanges();
                this.audit.LogAsync(model, WarrantyHierarchyEvents.EditTagFirstYearWarranty, EventCategories.Merchandising);
                scope.Complete();
                return model;
            }
        }
    }
}
