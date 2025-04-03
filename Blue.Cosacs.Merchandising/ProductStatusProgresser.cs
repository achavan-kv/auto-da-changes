namespace Blue.Cosacs.Merchandising
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Blue.Cosacs.Merchandising.Repositories;

    using Enums;

    using Microsoft.Practices.ObjectBuilder2;

    public interface IProductStatusProgresser
    {
        void AutoProgress(Product product);

        List<int> ElapsedProgress();
    }

    public class ProductStatusProgresser : IProductStatusProgresser
    {
        private readonly Settings settings;

        private readonly ITagValuesRepository tagValuesRepository;

        public ProductStatusProgresser(Settings settings, ITagValuesRepository tagValuesRepository)
        {
            this.settings = settings;
            this.tagValuesRepository = tagValuesRepository;
        }

        public void AutoProgress(Product product)
        {
            using (var scope = Context.Read())
            {
                if (product.Status != (int)ProductStatuses.NonActive || product.Id == 0)
                {
                    return;
                }

                if (product.ProductType != ProductTypes.Combo.ToString() && product.ProductType != ProductTypes.Set.ToString())
                {
                    StockAutoProgress(product);
                }

                if (product.ProductType == ProductTypes.Combo.ToString())
                {
                    ComboAutoProgress(product);
                }

                if (product.ProductType == ProductTypes.Set.ToString())
                {
                    SetAutoProgress(product);
                }
            }
        }

        private void StockAutoProgress(Product product)
        {
            using (var scope = Context.Read())
            {
                if (!scope.Context.CurrentRetailPriceView.Any(p => p.ProductId == product.Id))
                {
                    return;
                }

                if (!scope.Context.CostPrice.Any(p => p.ProductId == product.Id) && product.ProductType != ProductTypes.RepossessedStock.ToString())
                {
                    return;
                }

                var departmentHierarchy = scope.Context.HierarchyLevel.FirstOrDefault(l => l.Name == "Department");

                if (departmentHierarchy != null)
                {
                    if (!scope.Context.ProductHierarchy.Any(h => h.HierarchyLevelId == departmentHierarchy.Id && h.ProductId == product.Id))
                    {
                        return;
                    }
                }
            }
            product.LastStatusChangeDate = DateTime.UtcNow;
            product.Status = (int)ProductStatuses.ActiveNew;
        }

        private void ComboAutoProgress(Product product)
        {
            using (var scope = Context.Read())
            {
                var combo = scope.Context.Combo.Where(p => p.Id == product.Id).FirstOrDefault();
                if (combo == null || (combo == null && (combo.StartDate > DateTime.Now || combo.EndDate.AddDays(1) < DateTime.Now)))
                {
                    return;
                }

                if (!scope.Context.ComboProductPrice.Any(p => p.ComboProductId == product.Id))
                {
                    return;
                }
            }
            product.LastStatusChangeDate = DateTime.UtcNow;
            product.Status = (int)ProductStatuses.ActiveNew;
        }

        private void SetAutoProgress(Product product)
        {
            using (var scope = Context.Read())
            {
                if (!scope.Context.SetProduct.Any(p => p.SetId == product.Id))
                {
                    return;
                }

                var date = DateTime.Now.Date;
                if (!scope.Context.SetLocation.Any(p => p.SetId == product.Id && p.EffectiveDate >= date))
                {
                    return;
                }
            }
            product.LastStatusChangeDate = DateTime.UtcNow;
            product.Status = (int)ProductStatuses.ActiveNew;
        }

        public List<int> ElapsedProgress()
        {
            var productIds = new List<int>();

            using (var scope = Context.Write())
            {
                //Non Active to Active New
                var nonActiveIds = scope.Context.NonActiveStatusProgressView.Select(p => p.Id).ToList();
                var nonActiveProducts = scope.Context.Product.Where(p => nonActiveIds.Contains(p.Id));
                nonActiveProducts.ForEach(
                    p =>
                    {
                        p.Status = (int)ProductStatuses.ActiveNew;
                        p.LastStatusChangeDate = DateTime.UtcNow;
                    });

                //Active New to Active Current
                var activeNewProducts =
                    scope.Context.Product.Where(
                        p => p.Status == (int)ProductStatuses.ActiveNew && DbFunctions.DiffDays(p.LastStatusChangeDate, DateTime.UtcNow) > this.settings.ActiveNewMigrationPeriod);
                activeNewProducts.ForEach(
                    p =>
                    {
                        p.Status = (int)ProductStatuses.ActiveCurrent;
                        p.LastStatusChangeDate = DateTime.UtcNow;
                    });

                productIds.AddRange(activeNewProducts.Select(p => p.Id));

                //To Aged
                var allTags = tagValuesRepository.Get();
                var lastTimeReceived = scope.Context.GoodsReceiptAllProductView
                                            .GroupBy(r => r.ProductId)
                                            .Select(a => a.OrderByDescending(p => p.DateReceived).FirstOrDefault())
                                            .ToList();
                var productHierarchy = scope.Context.ProductHierarchyView
                    .Where(p => p.Status != (int)ProductStatuses.Deleted && p.Status != (int)ProductStatuses.Aged && p.Status != (int)ProductStatuses.NonActive)
                    .Select(p => new
                    {
                        p.TagId,
                        p.ProductId
                    })
                    .GroupBy(
                        k => k.ProductId, 
                        v => v.TagId)
                    .ToList();

                var productIdsToAge = new List<int>();

                var ids = productHierarchy
                    .Select(ph =>
                    {
                        var productTags = new HashSet<int>(ph.Select(p => p));

                        // Only age products that have been received
                        return allTags.Where(t => productTags.Contains(t.Id) && t.AgedAfter.HasValue && lastTimeReceived.Where(r => r.ProductId == ph.Key) 
                                      .Any(r => (DateTime.Now.Date - r.DateReceived).Days >= t.AgedAfter))
                                      .Select(p => ph.Key)
                                      .Distinct()
                                      .ToList();
                     })
                    .SelectMany(p => p.ToList())
                    .ToList();

                productIdsToAge.AddRange(ids);
                productIds.AddRange(ids);

                scope.Context.Product
                    .Where(p => productIdsToAge.Contains(p.Id))
                    .ForEach(
                        p =>
                        {
                            p.Status = (int)ProductStatuses.Aged;
                            p.LastStatusChangeDate = DateTime.UtcNow;
                        });

                scope.Context.SaveChanges();
                scope.Complete();
            }

            return productIds;
        }
    }
}
