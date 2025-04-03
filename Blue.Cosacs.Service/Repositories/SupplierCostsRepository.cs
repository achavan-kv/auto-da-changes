using System;
using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Service.Models;

namespace Blue.Cosacs.Service.Repositories
{
    public class SupplierCostsRepository
    {
        public SupplierCostsRepository(IClock clock)
        {
            this.clock = clock;
        }

        private readonly IClock clock;

        public void Save(SupplierCosts supplierCosts)
        {
            using (var scope = Context.Write())
            {
                if (supplierCosts.costs != null)                    //#14883
                { 
                    if (supplierCosts.costs.Count() != 0)
                    {
                        //Delete all records then save
                        var oldCosts = scope.Context.SupplierCost.Where(c => c.Supplier == supplierCosts.supplier).ToList();
                        oldCosts.ForEach(c =>
                        {
                            scope.Context.SupplierCost.Remove(c);
                        });
                        scope.Context.SaveChanges();
                        
                        supplierCosts.costs.ToList().ForEach(c =>
                        {
                            string[] month = Convert.ToString(c.month).Split('-');

                            var supplierCost = new SupplierCost
                            {
                                Supplier = supplierCosts.supplier,
                                Product = c.product,
                                Year = Convert.ToInt16(Convert.ToInt16(month[1]) / 12),
                                PartType = c.partType,
                                PartPercent = c.partPcent,
                                PartLimit = c.partVal,
                                LabourPercent = c.labourPcent,
                                LabourLimit = c.labourVal,
                                AdditionalPercent = c.additionalPcent,
                                AdditionalLimit = c.additionalVal
                            };
                            scope.Context.SupplierCost.Add(supplierCost);
                        });
                    }
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public SupplierCosts GetSupplierCosts(string supplier, string product = null)
        {
            if (string.IsNullOrWhiteSpace(supplier) && string.IsNullOrWhiteSpace(product))
                return null;

            using (var scope = Context.Read())
            {
                var costs = (from s in scope.Context.SupplierCost
                             where s.Supplier == supplier
                                && (product == null || s.Product == product)
                             select new Cost
                             {
                                 product = s.Product,
                                 year = s.Year,
                                 partType = s.PartType,
                                 partPcent = s.PartPercent,
                                 partVal = s.PartLimit,
                                 labourPcent = s.LabourPercent,
                                 labourVal = s.LabourLimit,
                                 additionalPcent = s.AdditionalPercent,
                                 additionalVal = s.AdditionalLimit
                             }).ToArray();

                foreach (var cost in costs)
                    cost.month = GetSupplierCostMonths(cost.year);

                return new SupplierCosts { supplier = supplier, costs = costs };
            }
        }

        public SupplierCosts GetSupplierCostsWithExchangeRate(string supplier, string product = null)
        {
            var dollarRate = (decimal)GetExchangeRate();
            if (dollarRate <= 0)
                throw new ApplicationException("Cannot load US Dollars exchange rate.");

            var costs = GetSupplierCosts(supplier, product);
            if (costs != null)
            {
                foreach (var cost in costs.costs)
                {
                    cost.partVal *= dollarRate;
                    cost.labourVal *= dollarRate;
                    cost.additionalVal *= dollarRate;
                }
            
            }

            return costs;
        }

        public bool AreUnique(SupplierCosts supplierCosts)
        {
            var toReturn = true;

            if (supplierCosts.costs != null)
            {
                if (supplierCosts.costs.Count() != 0)
                {
                    var duplicated = (from c in supplierCosts.costs.ToList()
                                      group c by new { c.product, c.month, c.partType } into grp
                                      where grp.Count() > 1
                                      select grp.Key);
                    
                    if (duplicated.Count() > 0)
                        toReturn = false;
                }
            }

            return toReturn;
        }

        public IEnumerable<string> GetProducts(string supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier))
                return null;

            using (var scope = Context.Read())
                return scope.Context.SupplierCost.Where(s => s.Supplier.Trim() == supplier.Trim()).Select(s => s.Product).Distinct().ToList();
        }

        //public IEnumerable<string> GetPartType(string supplier, string product)
        //{
        //    using (var scope = Context.Read())
        //        return scope.Context.SupplierCost.Where(s => s.Supplier.Trim() == supplier.Trim() && s.Product.Trim() == product).Select(s => s.PartType).OrderBy(s => s).ToList();
        //}

        //Return the Months rather than years to select from the Month picklist
        private string GetSupplierCostMonths(short year)
        {
            var months = string.Empty;

            switch (year)
            {
                case 1:
                    months = "1 - 12";
                    break;
                case 2:
                    months = "13 - 24";
                    break;
                case 3:
                    months = "25 - 36";
                    break;
                case 4:
                    months = "37 - 48";
                    break;
                case 5:
                    months = "49 - 60";
                    break;
            }

            return months;
        }

        private double GetExchangeRate(string currencySearchCriteria = "US")
        {
            using (var scope = Context.Read())
            {
                return (from rate in scope.Context.ExchangeRateView
                        where rate.CurrencyName.Contains(currencySearchCriteria)
                        orderby rate.Currency ascending
                        select rate.Rate).FirstOrDefault();
            }
        }

    }
}
