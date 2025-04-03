using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blue.Cosacs.Warranty.Repositories
{
    class WarrantyPotentialSaleRepository
    {
        public void Save(List<Model.WarrantyPotentialSaleModel> WarrantyPotential)
        {
            if (WarrantyPotential == null || WarrantyPotential.Count <= 0)
            {
                return;
            }

            using (var scope = Context.Write())
            {

                Parallel.ForEach(WarrantyPotential, current =>
                    {
                        var existingPotentialSale = (from p in scope.Context.WarrantyPotentialSale
                                                     where p.CustomerAccount == current.AccountNumber &&
                                                           p.ItemId == current.ItemId &&
                                                           p.WarrantyId == current.Warranty.WarrantyId &&
                                                           p.WarrantyType == current.Warranty.WarrantyType
                                                     select p).FirstOrDefault();

                        //Only insert if it does not exist
                        if (existingPotentialSale == null)
                        {
                            var newPotentialSale = scope.Context.WarrantyPotentialSale.Add(GetPotentialSaleEntity(current));

                        }
                        else
                        {
                            existingPotentialSale.Quantity = current.Quantity;
                        }
                    });

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void Save(Model.WarrantyPotentialSaleModel WarrantyPotential)
        {
            Save(new List<Model.WarrantyPotentialSaleModel>
            {
                WarrantyPotential
            });
        }

        public void Add(List<Model.WarrantyPotentialSaleModel> warrantyPotentialSales)
        {
            using (var scope = Context.Write())
            {
                foreach (var potentialSale in warrantyPotentialSales)
                {
                    scope.Context.WarrantyPotentialSale.Add(GetPotentialSaleEntity(potentialSale));
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private static WarrantyPotentialSale GetPotentialSaleEntity(Model.WarrantyPotentialSaleModel WarrantyPotential)
        {
            return new WarrantyPotentialSale
            {
                InvoiceNumber = WarrantyPotential.InvoiceNumber,
                CustomerAccount = WarrantyPotential.AccountNumber,
                SaleBranch = WarrantyPotential.SaleBranch,
                SoldById = WarrantyPotential.SoldBy.SoldById,
                SoldOn = WarrantyPotential.SoldOn,
                ItemDeliveredOn = WarrantyPotential.DeliveredOn,
                CustomerId = WarrantyPotential.CustomerId,
                ItemId = WarrantyPotential.ItemId,
                ItemNumber = WarrantyPotential.ItemNumber,
                ItemPrice = WarrantyPotential.ItemPrice,
                ItemCostPrice = WarrantyPotential.ItemCostPrice,
                WarrantyCostPrice = WarrantyPotential.Warranty.WarrantyCostPrice,
                WarrantyId = WarrantyPotential.Warranty.WarrantyId,
                WarrantyType = WarrantyPotential.Warranty.WarrantyType,
                WarrantyLength = WarrantyPotential.Warranty.WarrantyLength,
                WarrantyNumber = WarrantyPotential.Warranty.WarrantyNumber,
                WarrantyRetailPrice = WarrantyPotential.Warranty.WarrantyRetailPrice,
                WarrantySalePrice = WarrantyPotential.Warranty.WarrantySalePrice,
                WarrantyTaxRate = WarrantyPotential.Warranty.WarrantyTaxRate,
                IsItemReturned = WarrantyPotential.IsItemReturned,
                SecondEffort = WarrantyPotential.SecondEffort,                        //#17609
                AgreementNumber = int.Parse(WarrantyPotential.InvoiceNumber.Substring(WarrantyPotential.InvoiceNumber.IndexOf(" "), WarrantyPotential.InvoiceNumber.Length - WarrantyPotential.InvoiceNumber.IndexOf(" "))), //#18394  
                Quantity = WarrantyPotential.Quantity
            };
        }

        public void Delete(int warrantyPotentialSaleId)
        {
            using (var scope = Context.Write())
            {
                var WarrantyPotentialSale = scope.Context.WarrantyPotentialSale.Find(warrantyPotentialSaleId);
                if (WarrantyPotentialSale != null)
                {
                    scope.Context.WarrantyPotentialSale.Remove(WarrantyPotentialSale);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IEnumerable<Model.WarrantyPotentialSaleModel> GetPotentialSales()
        {
            using (var scope = Context.Read())
            {
                var query = from p in scope.Context.WarrantyPotentialSale
                            select new Model.WarrantyPotentialSaleModel
                            {
                                InvoiceNumber = p.InvoiceNumber,
                                SaleBranch = p.SaleBranch,
                                SoldBy = new Model.WarrantyPotentialSaleModel.SoldByUser
                                {
                                    SoldById = p.SoldById
                                },
                                SoldOn = p.SoldOn,
                                DeliveredOn = p.ItemDeliveredOn,
                                CustomerId = p.CustomerId,
                                ItemId = p.ItemId,
                                ItemNumber = p.ItemNumber,
                                ItemPrice = p.ItemPrice.HasValue ? p.ItemPrice : 0,
                                ItemCostPrice = p.ItemCostPrice.HasValue ? p.ItemCostPrice : 0,
                                Warranty = new Model.WarrantyPotentialSaleModel.ItemWarranty
                                {
                                    WarrantyCostPrice = p.WarrantyCostPrice.HasValue ? p.WarrantyCostPrice.Value : 0,
                                    WarrantyId = p.WarrantyId,
                                    WarrantyType = p.WarrantyType,
                                    WarrantyLength = p.WarrantyLength,
                                    WarrantyNumber = p.WarrantyNumber,
                                    WarrantyRetailPrice = p.WarrantyRetailPrice.HasValue ? p.WarrantyRetailPrice.Value : 0,
                                    WarrantySalePrice = p.WarrantySalePrice.HasValue ? p.WarrantySalePrice.Value : 0,
                                    WarrantyTaxRate = p.WarrantyTaxRate,
                                },
                                IsItemReturned = p.IsItemReturned
                            };

                return query.ToList();
            }
        }

        public IEnumerable<Model.WarrantyPotentialSaleModel> GetPotentialSalesBySalesPersonId(int SalePersonId)
        {
            using (var scope = Context.Read())
            {
                var query = from p in scope.Context.WarrantyPotentialSale
                            where p.SoldById == SalePersonId
                            select new Model.WarrantyPotentialSaleModel
                            {
                                InvoiceNumber = p.InvoiceNumber,
                                SaleBranch = p.SaleBranch,
                                SoldBy = new Model.WarrantyPotentialSaleModel.SoldByUser
                                {
                                    SoldById = p.SoldById
                                },
                                SoldOn = p.SoldOn,
                                DeliveredOn = p.ItemDeliveredOn,
                                CustomerId = p.CustomerId,
                                ItemId = p.ItemId,
                                ItemNumber = p.ItemNumber,
                                ItemPrice = p.ItemPrice.HasValue ? p.ItemPrice : 0,
                                ItemCostPrice = p.ItemCostPrice.HasValue ? p.ItemCostPrice : 0,
                                Warranty = new Model.WarrantyPotentialSaleModel.ItemWarranty
                                {
                                    WarrantyCostPrice = p.WarrantyCostPrice.HasValue ? (int)p.WarrantyCostPrice : 0,
                                    WarrantyId = p.WarrantyId,
                                    WarrantyType = p.WarrantyType,
                                    WarrantyLength = p.WarrantyLength,
                                    WarrantyNumber = p.WarrantyNumber,
                                    WarrantyRetailPrice = p.WarrantyRetailPrice.HasValue ? (int)p.WarrantyRetailPrice : 0,
                                    WarrantyTaxRate = p.WarrantyTaxRate,
                                },
                                IsItemReturned = p.IsItemReturned
                            };

                return query.ToList();
            }
        }

        public void MarkItemAsReturned(Blue.Cosacs.Messages.Warranty.SalesOrderCancelItem cancelItemMessage)
        {
            using (var scope = Context.Write())
            {
                var warranty = scope.Context.WarrantyPotentialSale.Where(w => w.InvoiceNumber == cancelItemMessage.InvoiceNumber && w.ItemId == cancelItemMessage.ItemId).ToList();
                foreach (WarrantyPotentialSale potSale in warranty)
                {
                    potSale.IsItemReturned = true;
                    scope.Context.SaveChanges();
                }
                scope.Complete();
            }
        }
    }
}
