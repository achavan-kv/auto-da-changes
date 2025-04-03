using Blue.Cosacs.Warranty.Model;
using Blue.Events;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class SalesRepository
    {
        private readonly IEventStore audit;
        private readonly IClock clock;

        public SalesRepository(IEventStore audit, IClock clock)
        {
            this.audit = audit;
            this.clock = clock;
        }

        public void CreateWarrantySale(Messages.Order order,string currentUserName)
        {
            var items = new List<Messages.Item>(order.Items);
            var warranties = items.Where(x => x.ItemTypeId == (int)ItemTypeEnum.Warranty && !x.Returned && !x.IsClaimed).OrderBy(x=>x.WarrantyContractNo).ToList();
            var groupId = new List<WarrantyGroup>();
            var newProducts = items.Where(x => x.ItemTypeId == (int)ItemTypeEnum.Product && !x.Returned);
            var productsWithoutWarranty = from p in newProducts
                                          where !warranties.Any(w=>w.ParentId == p.Id)
                                          select p;

            if (warranties.Any() || productsWithoutWarranty.Any())
            {
                using (var scope = Context.Write())
                {
                    foreach (var warranty in warranties)
                    {
                        var parentItem = items.Single(x => x.ItemTypeId == (int)ItemTypeEnum.Product && x.Id == warranty.ParentId);
                        var currentWarrantyGroup = groupId.SingleOrDefault(x => x.ParentId == warranty.ParentId && warranty.WarrantyTypeCode.Equals(x.WarrantyType));

                        if (currentWarrantyGroup == null)
                        {
                            groupId.Add(new WarrantyGroup
                            {
                                ParentId = parentItem.Id,
                                WarrantyType = warranty.WarrantyTypeCode,
                                Count = 1
                            });
                        }
                        else
                        {
                            currentWarrantyGroup.Count++;
                        }

                        var warrantySale = new WarrantySale
                        {
                            InvoiceNumber = string.Format("{0} {1}", order.CashAndGoAccountNo, order.Id),
                            SaleBranch = order.BranchNo,
                            SoldOn = order.CreatedOn,
                            SoldBy = currentUserName,
                            CustomerAccount = order.CashAndGoAccountNo,
                            CustomerId = order.Customer !=null ? order.Customer.CustomerId : null,
                            CustomerTitle = order.Customer != null ? order.Customer.Title : null,
                            CustomerFirstName = order.Customer != null ? order.Customer.FirstName : null,
                            CustomerLastName = order.Customer != null ? order.Customer.LastName : null,
                            CustomerAddressLine1 = order.Customer != null ? order.Customer.AddressLine1 : null,
                            CustomerAddressLine2 = order.Customer != null ? order.Customer.AddressLine2 : null,
                            CustomerAddressLine3 = order.Customer != null ? order.Customer.AddressLine3 : null,
                            CustomerPostcode = order.Customer != null ? order.Customer.PostCode : null,
                            ItemId = parentItem.ProductItemId,
                            ItemNumber = parentItem.ItemNo,
                            ItemUPC = parentItem.ItemUPC,
                            ItemPrice = parentItem.Price,
                            ItemDescription =parentItem.Description,
                            //ItemBrand = , Not Required
                            //ItemModel = , Not Required
                            ItemSupplier = parentItem.ItemSupplier,
                            WarrantyContractNo = warranty.WarrantyContractNo,
                            WarrantyId = warranty.WarrantyLinkId,
                            WarrantyNumber = warranty.ItemNo,                            
                            WarrantyLength = warranty.WarrantyLengthMonths,
                            WarrantyTaxRate = warranty.TaxRate,
                            WarrantyRetailPrice =warranty.RetailPrice,
                            WarrantySalePrice = warranty.Price,                            
                            WarrantyCostPrice =warranty.CostPrice,
                            Status = "Active",
                            StockLocation = order.BranchNo,
                            //ItemSerialNumber = , Not Required
                            //CustomerNotes = ,Not Required
                            ItemCostPrice = parentItem.CostPrice,
                            ItemDeliveredOn = order.CreatedOn,
                            WarrantyGroupId = groupId.Where(x=>x.ParentId == warranty.ParentId && warranty.WarrantyTypeCode.Equals(x.WarrantyType)).Select(x=>x.Count).Single(),
                            SoldById = order.CreatedBy,
                            ItemQuantity = warranty.Quantity,
                            EffectiveDate = warranty.WarrantyEffectiveDate,
                            WarrantyDeliveredOn = order.CreatedOn,
                            WarrantyType = warranty.WarrantyTypeCode,
                            AgreementNumber = order.Id,
                        };
                        scope.Context.WarrantySale.Add(warrantySale);
                        scope.Context.SaveChanges();

                        audit.LogAsync(
                           new
                           {
                               WarrantySaleId = warrantySale.Id,
                               Reason = "Warranty sold on POS"
                           },
                           EventType.CreateWarrantySale,
                           EventCategory.WarrantySale);
                    }

                    foreach (var product in productsWithoutWarranty)
                    {
                        var warrantySale = new WarrantySale
                        {
                            InvoiceNumber = string.Format("{0} {1}", order.CashAndGoAccountNo, order.Id),
                            SaleBranch = order.BranchNo,
                            SoldOn = order.CreatedOn,
                            SoldBy = currentUserName,
                            CustomerAccount = order.CashAndGoAccountNo,
                            CustomerId = order.Customer != null ? order.Customer.CustomerId : null,
                            CustomerTitle = order.Customer != null ? order.Customer.Title : null,
                            CustomerFirstName = order.Customer != null ? order.Customer.FirstName : null,
                            CustomerLastName = order.Customer != null ? order.Customer.LastName : null,
                            CustomerAddressLine1 = order.Customer != null ? order.Customer.AddressLine1 : null,
                            CustomerAddressLine2 = order.Customer != null ? order.Customer.AddressLine2 : null,
                            CustomerAddressLine3 = order.Customer != null ? order.Customer.AddressLine3 : null,
                            CustomerPostcode = order.Customer != null ? order.Customer.PostCode : null,
                            ItemId = product.ProductItemId,
                            ItemNumber = product.ItemNo,
                            ItemUPC = product.ItemUPC,
                            ItemPrice = product.Price,
                            ItemDescription = product.Description,
                            ItemSupplier = product.ItemSupplier,
                            Status = "Active",
                            StockLocation = order.BranchNo,
                            ItemCostPrice = product.CostPrice,
                            ItemDeliveredOn = order.CreatedOn,
                            WarrantyGroupId = 1,
                            SoldById = order.CreatedBy,
                            ItemQuantity = product.Quantity,
                            AgreementNumber = order.Id,
                        };
                        scope.Context.WarrantySale.Add(warrantySale);
                        scope.Context.SaveChanges();

                        audit.LogAsync(
                           new
                           {
                               WarrantySaleId = warrantySale.Id,
                               Reason = "Item sold on POS without any Warranty"
                           },
                           EventType.CreateWarrantySale,
                           EventCategory.WarrantySale);
                    }
                    scope.Complete();
                }
            }
        }

        public void AddPotentialWarranties(Messages.Order order, string currentUserName)
        {
            var potentialWarranties = new List<Messages.PotentialWarranty>(order.PotentialWarranties);

            using (var scope = Context.Write())
            {
                foreach (var warranty in potentialWarranties)
                {
                    var warrantyPotentialSale = new WarrantyPotentialSale
                    {
                        InvoiceNumber = string.Format("{0} {1}", order.CashAndGoAccountNo, order.Id),
                        SaleBranch = order.BranchNo,
                        SoldOn = order.CreatedOn,
                        SoldById = order.CreatedBy,
                        CustomerId = order.Customer != null && order.Customer.CustomerId != null ? order.Customer.CustomerId : "PAID & TAKEN",
                        ItemId = warranty.ItemId,
                        ItemNumber = warranty.ItemNo,
                        ItemPrice = warranty.ItemPrice,
                        WarrantyId = warranty.WarrantyId,
                        WarrantyNumber = warranty.WarrantyNumber,
                        WarrantyLength = warranty.WarrantyLength,
                        WarrantyTaxRate = warranty.WarrantyTaxRate,
                        WarrantyCostPrice = warranty.WarrantyCostPrice,
                        WarrantyRetailPrice = warranty.WarrantyRetailPrice,
                        WarrantySalePrice = warranty.WarrantySalePrice,
                        ItemSerialNumber = null,
                        ItemCostPrice = warranty.ItemCostPrice,
                        ItemDeliveredOn = order.CreatedOn,
                        IsItemReturned = warranty.IsReturned,
                        SecondEffort = warranty.SecondEffort,
                        WarrantyType = warranty.WarrantyTypeCode,
                        CustomerAccount = order.CashAndGoAccountNo,
                        AgreementNumber = order.Id,
                        Quantity = warranty.Quantity,
                    };
                    scope.Context.WarrantyPotentialSale.Add(warrantyPotentialSale);
                    scope.Context.SaveChanges();

                    audit.LogAsync(
                          new
                          {
                              PotentialWarrantyId = warrantyPotentialSale.Id,
                              Reason = "Potential Warranty on POS"
                          },
                          EventType.CreatePotentialWarrantySale,
                          EventCategory.PotentialWarrantySale);
                }
                scope.Complete();
            }
        }

        public void CancelWarrantySale(Messages.Order order, string currentUserName)
        {
            var items = new List<Messages.Item>(order.Items);
            var warranties = items.Where(x => x.ItemTypeId == (int)ItemTypeEnum.Warranty && (x.Returned || x.IsClaimed)).ToList();

            if (warranties.Any())
            {
                using (var scope = Context.Write())
                {
                    foreach (var returnedWarranty in warranties)
                    {
                        var existingWarranty = scope.Context.WarrantySale.FirstOrDefault(w => w.WarrantyContractNo == returnedWarranty.WarrantyContractNo);
                        existingWarranty.Status = returnedWarranty.IsClaimed ? WarrantyStatus.Redeemed : WarrantyStatus.Cancelled;
                        existingWarranty.ItemQuantity = 0;

                        var potentialWarranty = scope.Context.WarrantyPotentialSale.Where(w => w.InvoiceNumber == existingWarranty.InvoiceNumber
                            && w.ItemId == existingWarranty.ItemId && !returnedWarranty.IsClaimed).ToList();

                        foreach (var potentialSale in potentialWarranty)
                        {
                            if (items.FirstOrDefault(x => x.ItemNo.Equals(returnedWarranty.ParentItemNo) && x.Returned && x.ItemTypeId == (int)ItemTypeEnum.Product) != null)
                            {
                                potentialSale.IsItemReturned = true;
                            }
                        }

                        scope.Context.SaveChanges();
                        audit.LogAsync(
                         new
                         {
                             WarrantySaleId = existingWarranty.Id,
                             Reason = string.Format("Warranty {0} on POS",returnedWarranty.IsClaimed ? "Claimed" : "Returned")
                         },
                         EventType.CancelWarrantySale,
                         EventCategory.WarrantySale);
                    }
                    scope.Complete();
                }
            }
        }
        
        public enum ItemTypeEnum
        {
            Product = 1, Warranty, Installation, NonStock, Discount
        }
    }
}
