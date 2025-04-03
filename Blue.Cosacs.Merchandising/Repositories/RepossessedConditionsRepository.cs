namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Events;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRepossessedConditionsRepository
    {
        IEnumerable<RepossessedCondition> Get();

        RepossessedCondition Get(int id);

        Models.EditRepossessedCondition Create(Models.RepossessedCondition model);

        Models.EditRepossessedCondition Update(Models.EditRepossessedCondition model);

        bool CanCreate(Models.RepossessedCondition condition);

        bool Exists(Models.EditRepossessedCondition condition);

        bool HasHierarchyTags(int id);

        bool HasProducts(int id);

        bool Delete(int id);
    }

    public class RepossessedConditionsRepository : IRepossessedConditionsRepository
    {
        private readonly IEventStore audit;

        public RepossessedConditionsRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public IEnumerable<RepossessedCondition> Get()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.RepossessedCondition.ToList();
            }
        }

        public RepossessedCondition Get(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.RepossessedCondition.Single(c => c.Id == id);
            }
        }

        public Models.EditRepossessedCondition Create(Models.RepossessedCondition model)
        {
            using (var scope = Context.Write())
            {
                var condition = new Merchandising.RepossessedCondition();
                scope.Context.RepossessedCondition.Add(condition);

                Mapper.Map(model, condition);
                scope.Context.SaveChanges();
                audit.LogAsync(condition, RepossessedConditionEvents.CreateRepossessedCondition, EventCategories.Merchandising);
                scope.Complete();
                return Mapper.Map<Models.EditRepossessedCondition>(condition);
            }
        }

        public Models.EditRepossessedCondition Update(Models.EditRepossessedCondition model)
        {
            using (var scope = Context.Write())
            {
                var condition = scope.Context.RepossessedCondition.Find(model.Id);

                Mapper.Map(model, condition);
                scope.Context.SaveChanges();
                audit.LogAsync(condition, RepossessedConditionEvents.EditRepossessedCondition, EventCategories.Merchandising);
                scope.Complete();
                return Mapper.Map<Models.EditRepossessedCondition>(condition);
            }
        }

        public bool CanCreate(Models.RepossessedCondition condition)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.RepossessedCondition.Any(c => c.Name == condition.Name || c.SKUSuffix == condition.SKUSuffix);
            }
        }

        public bool Exists(Models.EditRepossessedCondition condition)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.RepossessedCondition.Any(c => (c.Id != condition.Id) && (c.Name == condition.Name || c.SKUSuffix == condition.SKUSuffix));
            }
        }

        public bool HasHierarchyTags(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.HierarchyTagCondition.Any(c => c.RepossessedConditionId == id);
            }
        }

        public bool HasProducts(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.RepossessedProduct.Any(p => p.RepossessedConditionId == id);
            }
        }

        public bool Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var condition = scope.Context.RepossessedCondition.First(c => c.Id == id);

                scope.Context.RepossessedCondition.Remove(condition);
                var deleted = scope.Context.SaveChanges();
                audit.LogAsync(condition, RepossessedConditionEvents.DeleteRepossessedCondition, EventCategories.Merchandising);
                scope.Complete();
                return deleted == 1;
            }
        }
    }
}