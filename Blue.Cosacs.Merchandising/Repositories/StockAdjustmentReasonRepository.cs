namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Mappers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public interface IStockAdjustmentReasonRepository
    {
        List<StockAdjustmentReasonViewModel> Get();

        StockAdjustmentReasonViewModel Get(int id);

        bool PrimaryReasonIsUnqiue(string reason);

        StockAdjustmentReasonViewModel Create(StockAdjustmentReasonCreateModel model);

        bool PrimaryReasonContainsDefaultSecondaryReason(int id);

        void DeletePrimaryReason(int id);

        bool SecondaryReasonIsUnqiue(int primaryReasonId, string reason);

        StockAdjustmentSecondaryReasonViewModel AddSecondaryReason(int id, StockAdjustmentSecondaryReasonViewModel model);

        StockAdjustmentSecondaryReasonViewModel UpdateSecondaryReasonViewModel(StockAdjustmentSecondaryReasonViewModel model);

        void RemoveSecondaryReason(int stockAdjustmentSecondaryReasonId);

        void SetDefaultSecondaryReason(int stockAdjustmentSecondaryReasonId);
    }

    public class StockAdjustmentReasonRepository : IStockAdjustmentReasonRepository
    {
        private readonly IEventStore audit;
        private readonly IStockAdjustmentReasonMapper reasonMapper;

        public StockAdjustmentReasonRepository(IEventStore audit, IStockAdjustmentReasonMapper reasonMapper)
        {
            this.audit = audit;
            this.reasonMapper = reasonMapper;
        }

        public List<StockAdjustmentReasonViewModel> Get()
        {
            using (var scope = Context.Read())
            {
                var stockAdjustmentReasons = scope.Context.StockAdjustmentReasonView.ToList();
                return reasonMapper.MapViewModel(stockAdjustmentReasons);
            }
        }

        public StockAdjustmentReasonViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var stockAdjustmentReasons = scope.Context.StockAdjustmentReasonView.Where(r => r.Id == id).ToList();
                return reasonMapper.MapViewModel(stockAdjustmentReasons).Single();
            }
        }

        public bool PrimaryReasonIsUnqiue(string reason)
        {
            using (var scope = Context.Read())
            {
                return !scope.Context.StockAdjustmentReasonView.Any(r => r.PrimaryReason == reason);
            }
        }

        public StockAdjustmentReasonViewModel Create(StockAdjustmentReasonCreateModel model)
        {
            if (!PrimaryReasonIsUnqiue(model.PrimaryReason))
            {
                throw new DuplicateNameException(model.PrimaryReason);
            }

            using (var scope = Context.Write())
            {
                var stockAdjustmentReason = Mapper.Map<StockAdjustmentPrimaryReason>(model);
                scope.Context.StockAdjustmentPrimaryReason.Add(stockAdjustmentReason);
                scope.Context.SaveChanges();
                audit.LogAsync(stockAdjustmentReason, StockAdjustmentReasonEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return Mapper.Map<StockAdjustmentReasonViewModel>(stockAdjustmentReason);
            }
        }

        public bool PrimaryReasonContainsDefaultSecondaryReason(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context
                    .StockAdjustmentSecondaryReason
                    .Any(r => r.DateDeleted != null && r.DefaultForCountAdjustment && r.PrimaryReasonId == id);
            }
        }

        public void DeletePrimaryReason(int id)
        {
            using (var scope = Context.Write())
            {
                var stockAdjustmentReason = scope.Context.StockAdjustmentPrimaryReason.Single(r => r.Id == id);
                stockAdjustmentReason.DateDeleted = DateTime.UtcNow;
                scope.Context.SaveChanges();
                audit.LogAsync(new { stockAdjustmentReason.Id, stockAdjustmentReason.Name }, StockAdjustmentReasonEvents.Delete, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public bool SecondaryReasonIsUnqiue(int primaryReasonId, string reason)
        {
            using (var scope = Context.Read())
            {
                return !scope.Context.StockAdjustmentReasonView.Any(r => r.PrimaryReasonId == primaryReasonId && r.SecondaryReason == reason);
            }
        }

        public StockAdjustmentSecondaryReasonViewModel AddSecondaryReason(int id, StockAdjustmentSecondaryReasonViewModel model)
        {
            if (!SecondaryReasonIsUnqiue(id, model.SecondaryReason))
            {
                throw new DuplicateNameException(model.SecondaryReason);
            }
            using (var scope = Context.Write())
            {
                var stockAdjustmentSecondaryReason = Mapper.Map<StockAdjustmentSecondaryReason>(model);
                stockAdjustmentSecondaryReason.PrimaryReasonId = id;

                var defaultExists = scope.Context.StockAdjustmentSecondaryReason.Any(r => r.DefaultForCountAdjustment);
                if (!defaultExists)
                {
                    stockAdjustmentSecondaryReason.DefaultForCountAdjustment = true;
                }

                scope.Context.StockAdjustmentSecondaryReason.Add(stockAdjustmentSecondaryReason);
                scope.Context.SaveChanges();
                audit.LogAsync(stockAdjustmentSecondaryReason, StockAdjustmentReasonEvents.AddSecondaryReason, EventCategories.Merchandising);
                scope.Complete();
                return Mapper.Map<StockAdjustmentSecondaryReasonViewModel>(stockAdjustmentSecondaryReason);
            }
        }

        public StockAdjustmentSecondaryReasonViewModel UpdateSecondaryReasonViewModel(StockAdjustmentSecondaryReasonViewModel model)
        {
            using (var scope = Context.Write())
            {
                var stockAdjustmentSecondaryReason = scope.Context.StockAdjustmentSecondaryReason.Single(r => r.Id == model.Id);
                Mapper.Map(model, stockAdjustmentSecondaryReason);
                scope.Context.SaveChanges();
                audit.LogAsync(stockAdjustmentSecondaryReason, StockAdjustmentReasonEvents.UpdateSecondaryReason, EventCategories.Merchandising);
                scope.Complete();
                return Mapper.Map<StockAdjustmentSecondaryReasonViewModel>(stockAdjustmentSecondaryReason);
            }
        }

        public void RemoveSecondaryReason(int stockAdjustmentSecondaryReasonId)
        {
            using (var scope = Context.Write())
            {
                var stockAdjustmentReason = scope.Context.StockAdjustmentSecondaryReason.Single(r => r.Id == stockAdjustmentSecondaryReasonId);

                if (stockAdjustmentReason.DefaultForCountAdjustment)
                {
                    throw new Exception("Cannot remove the default secondary reason");
                }

                stockAdjustmentReason.DateDeleted = DateTime.UtcNow;
                scope.Context.SaveChanges();
                audit.LogAsync(new { Id = stockAdjustmentReason.Id, Name = stockAdjustmentReason.SecondaryReason }, StockAdjustmentReasonEvents.RemoveSecondaryReason, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void SetDefaultSecondaryReason(int stockAdjustmentSecondaryReasonId)
        {
            using (var scope = Context.Write())
            {
                var defaultReason = scope.Context.StockAdjustmentSecondaryReason.Where(r => r.DefaultForCountAdjustment);
                defaultReason.Each(r => r.DefaultForCountAdjustment = false);
                var stockAdjustmentReason = scope.Context.StockAdjustmentSecondaryReason.Single(r => r.Id == stockAdjustmentSecondaryReasonId);
                stockAdjustmentReason.DefaultForCountAdjustment = true;
                scope.Context.SaveChanges();
                audit.LogAsync(new { stockAdjustmentSecondaryReasonId }, StockAdjustmentReasonEvents.ChangeDefault, EventCategories.Merchandising);
                scope.Complete();
            }
        }
    }
}
