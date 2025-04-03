using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class WarrantySaleRepository
    {
        public Model.WarrantySaleOrder Save(Model.WarrantySaleOrder saleOrder)
        {
            var maxgroupidMsg = saleOrder.Warranty.Select(w => w.WarrantyGroupId).Max() ?? 0;
            var maxgroupidWarSale = 0;
            var associatedWarranties = GetAssociatedWarranties(saleOrder);
            var warrantyIds = new List<int>();

            using (var scope = Context.Write())
            {
                maxgroupidWarSale = scope.Context.WarrantySale.Where(w => w.CustomerAccount== saleOrder.CustomerAccount && w.InvoiceNumber == saleOrder.InvoiceNumber 
                                    && w.ItemId == saleOrder.ItemId && w.StockLocation == saleOrder.ItemStockLocation).Select(w => w.WarrantyGroupId).Max() ?? 0;
                var maxgroupid = maxgroupidMsg > maxgroupidWarSale ? maxgroupidMsg : maxgroupidWarSale;

                if (saleOrder.Warranty.Any())
                {
                    SavePurchasedWarranties(saleOrder, associatedWarranties, scope, warrantyIds);

                    //If there are items with no warranties send message for each item
                    if (saleOrder.ItemQuantity.Value - maxgroupid > 0)
                    {
                        SaveItemsWithoutWarranties(saleOrder, maxgroupid, associatedWarranties, scope, warrantyIds);
                    }
                }
                else
                {
                    // No warranties at all
                    SaveSalesOrdersWithNoWarranties(saleOrder, associatedWarranties, scope, warrantyIds);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
            
            return saleOrder;
        }

        private static void SaveItemsWithoutWarranties(Model.WarrantySaleOrder saleOrder, int maxgroupid, ILookup<string, Warranty> associatedWarranties, Transactions.WriteScope<Context> scope, List<int> warrantyIds)
        {
            var counter = Convert.ToInt32(saleOrder.ItemQuantity - maxgroupid);

            for (var i = 1; i <= counter; i++)
            {
                var dbItem = scope.Context.WarrantySale.Add(GetWarrantyEntity(saleOrder, null, maxgroupid + i, associatedWarranties));

                scope.Context.SaveChanges();
                //scope.Complete();

                AddContacts(saleOrder, scope, dbItem);

                warrantyIds.Add(dbItem.Id);
            }
        }

        private static void SavePurchasedWarranties(Model.WarrantySaleOrder saleOrder, ILookup<string, Warranty> associatedWarranties, Transactions.WriteScope<Context> scope, List<int> warrantyIds)
        {
            var previousGroupId = 0;        
            var effectiveDate = DateTime.Today;
            var warrantyLenth = 0;

            var checkFreeExists = saleOrder.Warranty.Where(t=> t.WarrantyType == WarrantyType.Free).Count() == 0 ? false: true;

            foreach (var w in saleOrder.Warranty)
            {

                if (w.WarrantyEffectiveDate == DateTime.MinValue)       // #17313
                {
                    if (checkFreeExists == false && w.WarrantyType != WarrantyType.Free)   //Warranty added after original 
                    {
                        var warrantySale = GetWarrantySale(saleOrder, w.WarrantyGroupId.Value, w.WarrantyType);

                        effectiveDate = warrantySale != null && saleOrder.DeliveredOn.HasValue ? saleOrder.DeliveredOn.Value.AddMonths(warrantySale.WarrantyLength.Value) : DateTime.Today;
                    }
                    else
                    {
                        effectiveDate = previousGroupId == w.WarrantyGroupId.Value && w.WarrantyType != WarrantyType.Free &&
                        saleOrder.DeliveredOn.HasValue ? saleOrder.DeliveredOn.Value.AddMonths(warrantyLenth) : DateTime.Today;  //#16715
                    }

                }
                else
                {
                    effectiveDate = w.WarrantyEffectiveDate.Value;          // #17313
                }

                RedeemExistingWarranties(scope, warrantyIds, w, saleOrder);        // #17678

                var dbItem = scope.Context.WarrantySale.Add(GetWarrantyEntity(saleOrder, w, null, associatedWarranties, effectiveDate)); //#16715

                CancelExistingWarranties(scope, warrantyIds, w);

                //#18389
                if (w.WarrantyType == WarrantyType.Free)
                {
                    if (w.ReLinkContractNo != string.Empty)
                    {
                        ReLinkExistingWarranty(scope, warrantyIds, saleOrder,w.ReLinkContractNo);

                        UpdateLineItemWarrantyGroupId(scope, warrantyIds, saleOrder,w.ReLinkContractNo, w.WarrantyGroupId.Value);
                    }
        
                    //UpdateEffectiveDateForOverlap(scope, warrantyIds, w, saleOrder, effectiveDate);  ?? May be required later
                }

                scope.Context.SaveChanges();
                //scope.Complete();

                AddContacts(saleOrder, scope, dbItem);

                scope.Context.SaveChanges();
                //scope.Complete();

                warrantyIds.Add(dbItem.Id);

                previousGroupId = w.WarrantyGroupId.HasValue ? w.WarrantyGroupId.Value : 0;     //#16715
                warrantyLenth = w.WarrantyLength.HasValue ? w.WarrantyLength.Value : 0;         //#16715
            }
        }

        private static void AddContacts(Model.WarrantySaleOrder saleOrder, Transactions.WriteScope<Context> scope, WarrantySale dbItem)
        {
            if (saleOrder.Contacts != null)
            {
                foreach (var contact in saleOrder.Contacts)
                {
                    scope.Context.WarrantyContact.Add(new WarrantyContact
                    {
                        WarrantySaleId = dbItem.Id,
                        Type = contact.Type,
                        Value = contact.Value
                    });
                }
            }
        }

        private static ILookup<string, Warranty> GetAssociatedWarranties(Model.WarrantySaleOrder saleOrder)
        {
            ILookup<string, Warranty> associatedWarranties = null;
            using (var scope = Context.Read())
            {
                var salesOrderWarranty = (from sow in saleOrder.Warranty
                                          select sow.WarrantyNumber)
                                         .ToList();

                associatedWarranties = (from w in scope.Context.Warranty
                                        where salesOrderWarranty.Contains(w.Number)
                                        select w).ToLookup(e => e.Number);
            }
            return associatedWarranties;
        }

        private static void CancelExistingWarranties(Transactions.WriteScope<Context> scope, List<int> warrantyIds, Model.WarrantySaleOrder.ItemWarranty w)
        {
            var existingWarranty = scope.Context.WarrantySale.Where(e => e.WarrantyContractNo == w.WarrantyContractNo
                                        && e.WarrantyGroupId != w.WarrantyGroupId).FirstOrDefault();

            if (existingWarranty != null)
            {
                existingWarranty.Status = WarrantyStatus.Cancelled;
                warrantyIds.Add(existingWarranty.Id);
            }
        }

        private static void RedeemExistingWarranties(Transactions.WriteScope<Context> scope, List<int> warrantyIds, Model.WarrantySaleOrder.ItemWarranty w, Model.WarrantySaleOrder saleOrder)
        {
            var existingWarranty = (from e in scope.Context.WarrantySale
                                    where e.CustomerAccount == saleOrder.CustomerAccount && e.InvoiceNumber == saleOrder.InvoiceNumber
                                            && DateTime.Today >= e.EffectiveDate && DateTime.Today >= e.WarrantyDeliveredOn 
                                            //&& e.WarrantyGroupId == w.WarrantyGroupId //#17677
                                            && e.WarrantyContractNo == w.RedeemContractNo select e).ToList();

            foreach (var warranty in existingWarranty)
            {
                if (DateTime.Today >= warranty.EffectiveDate && DateTime.Today <= warranty.EffectiveDate.Value.AddMonths(warranty.WarrantyLength.Value))
                {
                    warranty.Status = WarrantyStatus.Redeemed;
                    warrantyIds.Add(warranty.Id);
                    break;
                }
            }
        }

        //#18371 - Update EffectiveDate of existing warranty to start after the new free warranty issued if an overlap exists between the two.
        private static void UpdateEffectiveDateForOverlap(Transactions.WriteScope<Context> scope, List<int> warrantyIds, Model.WarrantySaleOrder.ItemWarranty w, Model.WarrantySaleOrder saleOrder, DateTime? warrantyEffectiveDate = null)
        {
            var existingWarranty = (from e in scope.Context.WarrantySale
                                    where e.CustomerAccount == saleOrder.CustomerAccount && e.InvoiceNumber == saleOrder.InvoiceNumber
                                    && e.WarrantyGroupId == w.WarrantyGroupId
                                    && e.ItemId == saleOrder.ItemId
                                    && e.StockLocation == saleOrder.ItemStockLocation
                                    && e.WarrantyType != w.WarrantyType
                                    && e.Status == WarrantyStatus.Active
                                    select e).FirstOrDefault();

            if (existingWarranty != null)
            {
                if (warrantyEffectiveDate.Value.AddMonths(w.WarrantyLength.Value) > existingWarranty.EffectiveDate.Value)
                {
                    existingWarranty.EffectiveDate = warrantyEffectiveDate.Value.AddMonths(w.WarrantyLength.Value).AddDays(1);
                    warrantyIds.Add(existingWarranty.Id);
                }
  
            }
        }

        //18389 - Relink existing warranty to new item during replacement if warranty was carried over to the new item.
        private static void ReLinkExistingWarranty(Transactions.WriteScope<Context> scope, List<int> warrantyIds, Model.WarrantySaleOrder saleOrder, string reLinkContractNo)
        {
            var warrantyToReLink = (from r in scope.Context.WarrantySale
                                    where r.CustomerAccount == saleOrder.CustomerAccount && r.InvoiceNumber == saleOrder.InvoiceNumber
                                    && r.WarrantyContractNo == reLinkContractNo select r).FirstOrDefault();

            if (warrantyToReLink != null)
            {
                warrantyToReLink.ItemId = saleOrder.ItemId;
                warrantyToReLink.ItemNumber = saleOrder.ItemNumber;
                warrantyToReLink.ItemUPC = saleOrder.ItemUPC;
                warrantyToReLink.ItemPrice = saleOrder.ItemPrice;
                warrantyToReLink.ItemDescription = saleOrder.ItemDescription;
                warrantyToReLink.ItemBrand = saleOrder.ItemBrand;
                warrantyToReLink.ItemModel = saleOrder.ItemModel;
                warrantyToReLink.ItemSupplier = saleOrder.ItemSupplier;
                warrantyToReLink.ItemCostPrice = saleOrder.ItemCostPrice;
                warrantyToReLink.ItemDeliveredOn = saleOrder.DeliveredOn;
                warrantyToReLink.ItemQuantity = saleOrder.ItemQuantity;
                warrantyIds.Add(warrantyToReLink.Id);
            }

            scope.Context.SaveChanges();
        }

        //#18389 - Update Lineitem.WarrantyGroupId for a specific warranty to a new warrantyGroupdId (where link of warranty changed from one item to another)
        private static void UpdateLineItemWarrantyGroupId(Transactions.WriteScope<Context> scope, List<int> warrantyIds, Model.WarrantySaleOrder saleOrder, string contractNo, int warrantyGroupId)
        {
            var existingWarranty = (from e in scope.Context.WarrantySale
                                where e.CustomerAccount == saleOrder.CustomerAccount
                                && e.InvoiceNumber == saleOrder.InvoiceNumber
                                && e.WarrantyContractNo == contractNo
                                && e.WarrantyGroupId != warrantyGroupId
                                select e).FirstOrDefault();

            if (existingWarranty != null)
            {
                existingWarranty.WarrantyGroupId = warrantyGroupId;
                warrantyIds.Add(existingWarranty.Id);
            }

            scope.Context.SaveChanges();
        }

        private void SaveSalesOrdersWithNoWarranties(Model.WarrantySaleOrder saleOrder, ILookup<string, Warranty> associatedWarranties, Transactions.WriteScope<Context> scope, List<int> warrantyIds)
        {
            var counter = Convert.ToInt32(saleOrder.ItemQuantity);
            var existingGroupId = (from ws in scope.Context.WarrantySale
                                   where
                                     ws.InvoiceNumber == saleOrder.InvoiceNumber &&
                                     ws.ItemNumber == saleOrder.ItemNumber &&
                                     ws.ItemQuantity > 0
                                   group ws by ws.ItemNumber into g
                                   select g.Max(ws => ws.WarrantyGroupId)).FirstOrDefault();
            var maxGroupId = existingGroupId.HasValue ? existingGroupId.Value : 0;

            for (var i = 1; i <= counter; i++)
            {
                // If an order is partially delivered, then when the remaining order is delivered the group id will be back to 1.
                // So check for existing items and increase the group id.
                if (!CheckWarrantySaleExists(saleOrder, maxGroupId + i))
                {
                    var dbItem = scope.Context.WarrantySale.Add(GetWarrantyEntity(saleOrder, null, maxGroupId + i, associatedWarranties));

                    scope.Context.SaveChanges();
                    //scope.Complete();

                    AddContacts(saleOrder, scope, dbItem);

                    warrantyIds.Add(dbItem.Id);
                }
            }
        }

        private bool CheckWarrantySaleExists(Model.WarrantySaleOrder saleOrder, int warrrantyGroupId)
        {
            using (var scope = Context.Read())
            {
                // If an Identical replacement was processed, the original item should be Cancelled
                // So only check for entries with Active rows
                var countGroups = scope.Context.WarrantySale.Where(w => w.CustomerAccount == saleOrder.CustomerAccount
                                                                   && w.ItemId == saleOrder.ItemId
                                                                   && w.StockLocation == saleOrder.ItemStockLocation
                                                                   && w.ItemQuantity > 0
                                                                   ).GroupBy(w1 => w1.WarrantyGroupId)
                                                                   .Count();

                if (warrrantyGroupId > countGroups)
                {
                    return false;
                }
            }
            return true;
        }

        private static WarrantySale GetWarrantySale(Model.WarrantySaleOrder saleOrder, int warrantyGroupId, string warrantyType)
        {
            using (var scope = Context.Read())
            {
                var record = scope.Context.WarrantySale.Where(w => w.CustomerAccount == saleOrder.CustomerAccount
                                                                   && w.ItemId == saleOrder.ItemId
                                                                   && w.StockLocation == saleOrder.ItemStockLocation
                                                                   && w.ItemQuantity > 0
                                                                   && w.WarrantyGroupId == warrantyGroupId
                                                                   && w.WarrantyType != warrantyType
                                                                   ).FirstOrDefault();
                return record;
              
            }
        }

        public void SetSerialNumber(string warrantyContractNumber, string serialNumber)
        {
            using (var scope = Context.Write())
            {
                var entity = (from ws in scope.Context.WarrantySale
                              where ws.WarrantyContractNo == warrantyContractNumber
                              select ws).FirstOrDefault();
                
                if (entity != null)
                {
                    entity.ItemSerialNumber = serialNumber;
                    scope.Context.SaveChanges();
                }

                scope.Complete();
            }
        }

        private static WarrantySale GetWarrantyEntity(Model.WarrantySaleOrder saleOrder, Model.WarrantySaleOrder.ItemWarranty warranty, int? itemGroupId = null,
                                                      ILookup<string, Warranty> warrantyLookup = null, DateTime? warrantyEffectiveDate = null) 
        {

            if (warranty != null && !warranty.WarrantyDeliveredOn.HasValue)
            {
                throw new ArgumentNullException("WarrantyDeliveredOn does not have a value");
            }

            return new WarrantySale
            {
                InvoiceNumber = saleOrder.InvoiceNumber,
                SaleBranch = saleOrder.SaleBranch,
                //CHECK THIS CODE
                SoldBy = saleOrder.SoldBy.Value,
                SoldById = saleOrder.SoldBy.SoldById,
                //CHECK THIS CODE
                SoldOn = saleOrder.SoldOn,
                ItemDeliveredOn = saleOrder.DeliveredOn,
                CustomerAccount = saleOrder.CustomerAccount,
                CustomerId = saleOrder.CustomerId,
                CustomerTitle = saleOrder.CustomerTitle,
                CustomerFirstName = saleOrder.CustomerFirstName,
                CustomerLastName = saleOrder.CustomerLastName,
                CustomerAddressLine1 = saleOrder.CustomerAddressLine1,
                CustomerAddressLine2 = saleOrder.CustomerAddressLine2,
                CustomerAddressLine3 = saleOrder.CustomerAddressLine3,
                CustomerPostcode = saleOrder.CustomerPostcode,
                CustomerNotes = saleOrder.CustomerNotes,
                ItemBrand = saleOrder.ItemBrand,
                ItemDescription = saleOrder.ItemDescription,
                ItemId = saleOrder.ItemId,
                ItemModel = saleOrder.ItemModel,
                ItemNumber = saleOrder.ItemNumber,
                ItemPrice = saleOrder.ItemPrice,
                ItemCostPrice = saleOrder.ItemCostPrice,
                ItemSupplier = saleOrder.ItemSupplier,
                ItemUPC = saleOrder.ItemUPC,
                StockLocation = saleOrder.ItemStockLocation,

                WarrantyContractNo = warranty == null ? string.Empty : warranty.WarrantyContractNo,
                WarrantyCostPrice = warranty == null ? null : warranty.WarrantyCostPrice,
                WarrantyId = warranty == null ? null : TryGetLookupValue(warrantyLookup, warranty.WarrantyNumber),
                WarrantyType = warranty == null ? null : warranty.WarrantyType,
                WarrantyLength = warranty == null ? null : warranty.WarrantyLength,
                WarrantyNumber = warranty == null ? string.Empty : warranty.WarrantyNumber,
                WarrantyRetailPrice = warranty == null ? null : warranty.WarrantyRetailPrice,
                WarrantySalePrice = warranty == null ? null : warranty.WarrantySalePrice,
                WarrantyTaxRate = warranty == null ? null : warranty.WarrantyTaxRate,
                WarrantyGroupId = itemGroupId != null ? itemGroupId : warranty == null ? null : warranty.WarrantyGroupId,
                Status = warranty == null ? null : warranty.WarrantyStatus,
                ItemQuantity = saleOrder.ItemQuantity,
                EffectiveDate = warrantyEffectiveDate,
                WarrantyDeliveredOn = warranty == null ? null : warranty.WarrantyDeliveredOn,
                AgreementNumber = int.Parse(saleOrder.InvoiceNumber.Substring(saleOrder.InvoiceNumber.IndexOf(" "), saleOrder.InvoiceNumber.Length - saleOrder.InvoiceNumber.IndexOf(" "))) //#18394   
            };
        }

        private static int? TryGetLookupValue(ILookup<string, Warranty> warrantyLookup, string warrantyNumber)
        {
            if (warrantyLookup != null && warrantyNumber != null && warrantyLookup.Contains(warrantyNumber))
            {
                var warranty = warrantyLookup[warrantyNumber].FirstOrDefault();
                if (warranty == null)
                {
                    return null;
                }

                return warranty.Id;
            }
            else
            {
                return null;
            }
        }

        public int? UpdateStatus(Blue.Cosacs.Messages.Warranty.SalesOrderCancel saleMessage, string status)
        {
            using (var scope = Context.Write())
            {
                var warranty = scope.Context.WarrantySale.Where(w => w.WarrantyContractNo == saleMessage.ContractNumber &&
                                                                w.StockLocation == saleMessage.SaleBranch).FirstOrDefault();
                if (warranty != null)
                {
                    warranty.Status = status;
                    scope.Context.SaveChanges();
                    scope.Complete();
                    return warranty.Id;
                }
            }
            return null;
        }

        //#16309 - Set ItemQuantity to 0 to signify item as cancelled.
        public void WarrantySaleCancelItem(Blue.Cosacs.Messages.Warranty.SalesOrderCancelItem saleMessage)
        {
            var warrantyIds = new List<int>();
            var GroupCount = 0;
            var GroupId = 0;

            using (var scope = Context.Write())
            {
                var warranty = (from w in scope.Context.WarrantySale
                                where w.CustomerAccount == saleMessage.AccountNumber &&
                                     w.InvoiceNumber == saleMessage.InvoiceNumber &&
                                     w.ItemId == saleMessage.ItemId &&
                                     w.StockLocation == saleMessage.SaleBranch &&
                                     w.ItemQuantity != 0
                                select w).OrderBy(w => w.WarrantyGroupId).ToList();

                if (warranty.Count > 0)
                {
                    foreach (var item in warranty)
                    {
                        if (GroupId != Convert.ToInt32(item.WarrantyGroupId))
                        {
                            GroupCount++;
                            GroupId = Convert.ToInt32(item.WarrantyGroupId);
                        }

                        //Do for Quantity cancelled
                        if (GroupCount <= Math.Abs(saleMessage.Quantity))  //#15926
                        {
                            item.ItemQuantity = 0;
                            warrantyIds.Add(item.Id);
                        }
                    }

                    scope.Context.SaveChanges();
                    scope.Complete();
                }
            }
            //Solr.SolrIndex.IndexWarrantySale(warrantyIds.ToArray());
        }

        public class WarrantySaleInfo
        {
            public int? LineItemIdentifier;
            public DateTime? ItemSoldOn;
            public int? StockLocation;
        }

        public IList<WarrantySale> GetLineItemIdentifier(string accountNo, int itemId)
        {
            using (var scope = Context.Read())
            {
                return (from ws in scope.Context.WarrantySale
                        where ws.CustomerAccount == accountNo && ws.ItemId == itemId
                        select ws).ToList();
            }
        }

        public int GetWarrantyPotentialSaleQuantity(Blue.Cosacs.Messages.Warranty.SalesOrder saleMessage)
        {
            using (var scope = Context.Read())
            {

                var currentWarrantySalesCount = (from ws in scope.Context.WarrantySale
                                                    where ws.CustomerAccount == saleMessage.Customer.AccountNumber
                                                    && ws.ItemId == saleMessage.Item.Id
                                                    && ws.StockLocation == saleMessage.Item.StockLocation
                                                    && ws.WarrantyType != WarrantyType.Free
                                                    && ws.Status != WarrantyStatus.Cancelled
                                                    && ws.Status != null
                                                    select ws).Count();

               var potentialSaleQuantity = saleMessage.Item.TotalQuantity == null? 0 : saleMessage.Item.TotalQuantity - currentWarrantySalesCount;

            return potentialSaleQuantity.Value;

            }

        }

        public void UpdateWarrantyPotentialQuantity(Blue.Cosacs.Messages.Warranty.SalesOrderCancel cancel)
        {
            using (var scope = Context.Write())
            {
               

                var warrantySaleRow = (from ws in scope.Context.WarrantySale
                                       where ws.WarrantyContractNo == cancel.ContractNumber
                                        select ws).FirstOrDefault();


                if (warrantySaleRow != null && warrantySaleRow.WarrantyType != WarrantyType.Free)
                {
                    var currentWarrantySalesCount = (from ws in scope.Context.WarrantySale
                                                     where ws.CustomerAccount == warrantySaleRow.CustomerAccount
                                                     && ws.ItemId == warrantySaleRow.ItemId
                                                     && ws.StockLocation == warrantySaleRow.StockLocation
                                                     && ws.WarrantyType != WarrantyType.Free
                                                     && ws.Status != WarrantyStatus.Cancelled
                                                     && ws.Status != null
                                                     select ws).Count();

                    var warrantyPotential = (from wp in scope.Context.WarrantyPotentialSale
                                                where wp.CustomerAccount == warrantySaleRow.CustomerAccount &&
                                                wp.AgreementNumber == warrantySaleRow.AgreementNumber &&
                                                wp.ItemId == warrantySaleRow.ItemId
                                                select wp).ToList();

                    if (warrantyPotential != null)
                    {
                        foreach (var item in warrantyPotential)
                        {
                            if (item.WarrantyType != WarrantyType.Free)
                            {
                                item.Quantity = cancel.ItemQuantity.Value - currentWarrantySalesCount;
                            }
                        }
                    }
     
                    scope.Context.SaveChanges();
                    scope.Complete();
                }

            }

        }


        public void UpdateWarrantyPotentialQuantity(Blue.Cosacs.Messages.Warranty.SalesOrderCancelItem cancelItem)
        {
            using (var scope = Context.Write())
            {

                    var currentWarrantySalesCount = (from ws in scope.Context.WarrantySale
                                                     where ws.CustomerAccount == cancelItem.AccountNumber
                                                     && ws.ItemId == cancelItem.ItemId
                                                     && ws.WarrantyType != WarrantyType.Free
                                                     && ws.Status != WarrantyStatus.Cancelled
                                                     && ws.Status != null
                                                     select ws).Count();

                    var warrantyPotential = (from wp in scope.Context.WarrantyPotentialSale
                                             where wp.CustomerAccount == cancelItem.AccountNumber &&
                                             wp.ItemId == cancelItem.ItemId
                                             select wp).ToList();

                    if (warrantyPotential != null)
                    {
                        foreach (var item in warrantyPotential)
                        {
                            if (item.WarrantyType != WarrantyType.Free)
                            {
                                item.Quantity = cancelItem.TotalQuantity.Value - currentWarrantySalesCount;
                            }
                        }
                    }
                
                    scope.Context.SaveChanges();
                    scope.Complete();  

            }

        }
    }
}
