using Blue.Cosacs.Model;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.Warranty;
using Blue.Cosacs.Warehouse.Common;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.AuditSource;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using Message = Blue.Cosacs.Messages.Warranty;


namespace Blue.Cosacs.Repositories
{
    public class WarrantyRepository
    {

        public void SaveWarranty(XmlNode lineitems)
        {

            var nodes = lineitems.SelectNodes("//Item[@Type='Warranty']"); //#17635
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                foreach (XmlNode node in nodes)
                {
                    if (!ctx.StockInfo.Any(s => s.Id == Convert.ToInt32(node.Attributes[Tags.ItemId].Value)))
                        InsertWarrantyInfo(node, ctx);

                    if (!ctx.StockQuantity.Any(s => s.ID == Convert.ToInt32(node.Attributes[Tags.ItemId].Value) &&
                        s.stocklocn == Convert.ToInt32(node.Attributes[Tags.Location].Value)))
                        InsertWarrantyQuantity(node, ctx);

                    if (!ctx.StockPrice.Any(s => s.ID == Convert.ToInt32(node.Attributes[Tags.ItemId].Value) &&
                        s.branchno == Convert.ToInt32(node.Attributes[Tags.Location].Value)))
                    {
                        InsertWarrantyPrice(node, ctx);
                    }
                    else    //#16338 Update any price changes
                    {
                        UpdateWarrantyPrice(node, ctx);
                    }

                }
            });

        }

        public void CreateWarranty(SaveWarrantyStockinfoRequest warrantyRequest)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var branches = (from b in ctx.Branch
                                select b.branchno).ToList();   // #16237

                foreach (var warranty in warrantyRequest.StockInfo)
                {
                    if (!CheckWarranty(ctx, warranty.Id))                   // #16647
                    {
                        //return;
                        ctx.StockInfo.InsertOnSubmit(new StockInfo
                        {
                            Id = warranty.Id,
                            itemno = warranty.ItemNo,
                            SKU = warranty.ItemNo,
                            IUPC = warranty.ItemNo,
                            itemdescr1 = warranty.Description,
                            itemdescr2 = "Warranty Length - " + warranty.Length.ToString(),
                            itemtype = "N",
                            warrantyrenewalflag = "N",
                            leadtime = 0,
                            taxrate = Convert.ToDouble(warranty.TaxRate),
                            RepossessedItem = false,
                            SparePart = false,
                            WarrantyLength = Convert.ToInt16(warranty.Length),
                            category = 12,
                            WarrantyType = warranty.WarrantyType
                        });
                    }

                    foreach (var branchno in branches)          // #16237 Insert Price & quantity for all branches
                    {
                        if (!ctx.StockQuantity.Any(s => s.ID == warranty.Id &&
                                    s.stocklocn == Convert.ToInt32(branchno)))      // #16647
                        {
                            ctx.StockQuantity.InsertOnSubmit(new StockQuantity
                            {
                                ID = warranty.Id,
                                itemno = warranty.ItemNo,
                                stock = 0,
                                qtyAvailable = 0,
                                stockonorder = 0,
                                //stocklocn = Convert.ToInt16(warranty.Location),
                                stocklocn = Convert.ToInt16(branchno),      // #16237
                                leadtime = 0,
                                stockdamage = 0,
                                LastOperationSource = string.Empty,
                                dateupdated = DateTime.Now,
                                deleted = "N"
                            });
                        }

                        if (!ctx.StockPrice.Any(s => s.ID == warranty.Id &&
                                    s.branchno == Convert.ToInt32(branchno)))            // #16647
                        {
                            ctx.StockPrice.InsertOnSubmit(new StockPrice
                            {
                                ID = warranty.Id,
                                itemno = warranty.ItemNo,
                                //branchno = Convert.ToInt16(warranty.Location),
                                branchno = Convert.ToInt16(branchno),       // #16237
                                CreditPrice = warranty.Price,
                                CashPrice = warranty.Price,
                                CostPrice = warranty.CostPrice,
                                Refcode = string.Empty
                            });
                        }
                    }
                }
                ctx.SubmitChanges();
            });
        }

        private bool CheckWarranty(Context ctx, int warranty)
        {
            return ctx.StockInfo.Where(s => s.Id == warranty).Any();
        }


        private void InsertWarrantyInfo(XmlNode warranty, Context ctx)
        {
            if (!ctx.StockInfo.Any(s => s.Id == Convert.ToInt32(warranty.Attributes[Tags.ItemId].Value)))
            {
                ctx.StockInfo.InsertOnSubmit(new StockInfo
                {
                    Id = Convert.ToInt32(warranty.Attributes[Tags.ItemId].Value),
                    itemno = warranty.Attributes[Tags.Code].Value.ToString(),
                    IUPC = warranty.Attributes[Tags.Code].Value.ToString(),
                    SKU = warranty.Attributes[Tags.Code].Value.ToString(),
                    itemdescr1 = warranty.Attributes[Tags.Description1].Value.ToString(),
                    itemdescr2 = "Warranty Length - " + warranty.Attributes["Length"].Value.ToString(),
                    itemtype = "N",
                    warrantyrenewalflag = "N",
                    leadtime = 0,
                    taxrate = Convert.ToDouble(warranty.Attributes[Tags.TaxRate].Value),
                    RepossessedItem = false,
                    SparePart = false,
                    WarrantyLength = Convert.ToInt16(warranty.Attributes["Length"].Value),
                    category = Convert.ToString(warranty.Attributes["ProductCategory"].Value) == "PCE" ? short.Parse("12") : short.Parse("82"),
                    WarrantyType = Convert.ToString(warranty.Attributes["WarrantyType"].Value)      //#17883         //#15639

                });
                ctx.SubmitChanges();
            }
        }

        private void InsertWarrantyQuantity(XmlNode warranty, Context ctx)
        {
            var branches = (from b in ctx.Branch
                            select b.branchno).ToList();       // #16237

            foreach (var branchno in branches)       // #16237 - insert quantity for all branches 
            {
                if (!ctx.StockQuantity.Any(s => s.ID == Convert.ToInt32(warranty.Attributes[Tags.ItemId].Value) &&
                    //s.stocklocn == Convert.ToInt32(warranty.Attributes[Tags.Location].Value)))
                    s.stocklocn == Convert.ToInt32(branchno)))      // #16237
                {
                    ctx.StockQuantity.InsertOnSubmit(new StockQuantity
                    {
                        ID = Convert.ToInt32(warranty.Attributes[Tags.ItemId].Value),
                        itemno = warranty.Attributes[Tags.Code].Value.ToString(),
                        stock = 0,
                        qtyAvailable = 0,
                        stockonorder = 0,
                        //stocklocn = Convert.ToInt16(warranty.Attributes[Tags.Location].Value),
                        stocklocn = Convert.ToInt16(branchno),      // #16237
                        leadtime = 0,
                        stockdamage = 0,
                        LastOperationSource = string.Empty,
                        dateupdated = DateTime.Now,
                        deleted = "N"
                    });

                    //ctx.SubmitChanges();
                }
            }
            ctx.SubmitChanges();
        }



        private void InsertWarrantyPrice(XmlNode warranty, Context ctx)
        {
            var branches = (from b in ctx.Branch
                            select b.branchno).ToList();

            foreach (var branchno in branches)      // #16237 - insert price for all branches 
            {
                if (!ctx.StockPrice.Any(s => s.ID == Convert.ToInt32(warranty.Attributes[Tags.ItemId].Value) &&
                    //s.branchno == Convert.ToInt32(warranty.Attributes[Tags.Location].Value)))
                    s.branchno == Convert.ToInt32(branchno)))
                {
                    ctx.StockPrice.InsertOnSubmit(new StockPrice
                    {
                        ID = Convert.ToInt32(warranty.Attributes[Tags.ItemId].Value),
                        itemno = warranty.Attributes[Tags.Code].Value.ToString(),
                        //branchno = Convert.ToInt16(warranty.Attributes[Tags.Location].Value),
                        branchno = Convert.ToInt16(branchno),
                        Refcode = string.Empty,
                        CreditPrice = warranty.Attributes[Tags.UnitPrice].Value == string.Empty ? 0 : Convert.ToDecimal(warranty.Attributes[Tags.UnitPrice].Value),
                        CashPrice = warranty.Attributes[Tags.UnitPrice].Value == string.Empty ? 0 : Convert.ToDecimal(warranty.Attributes[Tags.UnitPrice].Value),
                        CostPrice = warranty.Attributes[Tags.UnitPrice].Value == string.Empty ? 0 : Convert.ToDecimal(warranty.Attributes[Tags.CostPrice].Value)

                    });

                    //ctx.SubmitChanges();
                }
            }
            ctx.SubmitChanges();
        }

        //#16338 - Update StockPrice 
        private void UpdateWarrantyPrice(XmlNode warranty, Context ctx)
        {
            var stockPrice = (from p in ctx.StockPrice
                              where p.ID == Convert.ToInt32(warranty.Attributes[Tags.ItemId].Value)
                              && p.branchno == Convert.ToInt32(warranty.Attributes[Tags.Location].Value)
                              && (p.CostPrice != Convert.ToDecimal(warranty.Attributes[Tags.CostPrice].Value) ||
                                      p.CreditPrice != Convert.ToDecimal(warranty.Attributes[Tags.HPPrice].Value) ||
                                      p.CashPrice != Convert.ToDecimal(warranty.Attributes[Tags.CashPrice].Value))
                              select p).FirstOrDefault();

            if (stockPrice != null)
            {
                stockPrice.CashPrice = Convert.ToDecimal(warranty.Attributes[Tags.CashPrice].Value);
                stockPrice.CreditPrice = Convert.ToDecimal(warranty.Attributes[Tags.HPPrice].Value);
                stockPrice.CostPrice = Convert.ToDecimal(warranty.Attributes[Tags.CostPrice].Value);

                ctx.SubmitChanges();
            }
        }

        private static void ReplaceDepartmentWithValidCategory(Context ctx, List<ItemWarranty> warranty)
        {
            var warrDepartment = ctx.Code
                .Where(e => e.codedescript == "Warranties")
                .Where(e => e.code == "12" || e.code == "82")
                .Where(e => e.category == "PCE" || e.category == "PCF")
                .ToList();

            warranty.ForEach(i =>
            {
                var warCode = i.WarrantyDepartment;
                i.WarrantyDepartment = warrDepartment.Where(e => e.code == warCode).Select(e => e.category).FirstOrDefault();
            });

            //throw new Exception("Count " + warrDepartment.Count() + ", ");
        }

        public void DeliverItem(SqlConnection conn, SqlTransaction tran, Blue.Cosacs.Model.LineitemBooking booking, DateTime? dateDelivered = null, int confirmedby = 0, bool isTaxFree = false)      // #17692
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var warranty = (from l in ctx.LineItem
                                join s in ctx.StockInfo on l.ItemID equals s.Id
                                join sp in ctx.StockPrice on s.Id equals sp.ID
                                where (s.category == 12 || s.category == 82) &&
                                sp.branchno == l.stocklocn &&
                                l.acctno == booking.AcctNo &&
                                l.agrmtno == booking.AgreementNo &&
                                l.parentlocation == booking.StockLocation &&
                                l.ParentItemID == booking.ItemId &&
                                (l.contractno != null || l.contractno != string.Empty) &&
                                l.quantity > 0 &&
                                !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.contractno == l.contractno && del.acctno == l.acctno && del.agrmtno == l.agrmtno)   // #17290 && del.ParentItemID == l.ParentItemID && del.acctno == l.acctno && del.agrmtno == l.agrmtno)// Exclude warranties already delivered
                                select new ItemWarranty
                                {
                                    WarrantyContractNo = l.contractno,
                                    WarrantyStockLocn = l.stocklocn,
                                    WarrantyQuantity = Convert.ToInt32(l.quantity),
                                    WarrantyItemId = l.ItemID,
                                    WarrantyItemNo = l.itemno,
                                    WarrantyTaxRate = Convert.ToDecimal(s.taxrate),
                                    WarrantyDepartment = s.category.HasValue ? s.category.Value.ToString() : string.Empty,
                                    WarrantyCostPrice = sp.CostPrice,
                                    WarrantyRetailPrice = sp.CashPrice,
                                    WarrantyPrice = l.price,
                                    WarrantyOrdval = l.ordval,
                                    WarrantyParentItemId = l.ParentItemID,
                                    WarrantyParentItemNo = l.parentitemno,
                                    WarrantyParentLocation = l.parentlocation,
                                    WarrantyAgrmtno = l.agrmtno,
                                    WarrantyAcctno = l.acctno,
                                    WarrantyType = s.WarrantyType,
                                    WarrantyLength = s.WarrantyLength,
                                    WarrantyEffectiveDate = DateTime.MinValue
                                }).ToList();

                ReplaceDepartmentWithValidCategory(ctx, warranty);

                var salesOrderView = (from w in ctx.WarrantySaleGenerateView
                                      where w.CustomerAccount == booking.AcctNo &&
                                      w.AgreementNumber == booking.AgreementNo &&
                                      w.ItemId == booking.ItemId &&
                                      w.StockLocation == booking.StockLocation
                                      select w).ToList();

                List<Message.SalesOrder> salesOrder = new List<Message.SalesOrder>();

                foreach (var s in salesOrderView)
                {
                    var message = new Message.SalesOrder
                    {
                        Customer = new Message.Customer
                        {
                            AccountNumber = s.CustomerAccount,
                            AddressLine1 = s.CustomerAddressLine1,
                            AddressLine2 = s.CustomerAddressLine2,
                            AddressLine3 = s.CustomerAddressLine3,
                            CustomerId = s.customerId,
                            FirstName = s.CustomerFirstName,
                            LastName = s.CustomerLastName,
                            Notes = s.CustomerNotes,
                            PostCode = s.CustomerPostCode,
                            Title = s.customerTitle,
                            HomePhone = s.homePhone,
                            WorkPhone = s.workPhone,
                            MobilePhone = s.mobilePhone,
                            Email = s.Email,
                            AccountType = s.AccountType
                        },
                        DeliveredOn = DateTime.SpecifyKind(dateDelivered.HasValue ? dateDelivered.Value : DateTime.Now.Date, DateTimeKind.Utc),
                        InvoiceNumber = s.invoiceNumber,
                        Item = new Message.Item
                        {
                            Brand = s.itemBrand,
                            CostPrice = s.ItemCostPrice.HasValue ? s.ItemCostPrice.Value : 0,
                            Description = s.Description,
                            Id = s.ItemId,
                            Number = s.ItemNumber,
                            Price = s.ItemPrice,
                            StockLocation = s.StockLocation,
                            //Quantity = Convert.ToInt32(s.Quantity),
                            Quantity = Convert.ToInt32(booking.Quantity),
                            Supplier = s.itemSupplier,
                            UPC = s.ItemUPC,
                            Warranty = warranty.Count > 0 ? ReturnWarrantyMessage(ctx, conn, tran, warranty, s.CustomerAccount,
                                                            s.AgreementNumber, s.ItemId, s.StockLocation, Convert.ToInt32(booking.Quantity), false, isTaxFree) : null,
                            Department = s.Department,
                            TotalQuantity = int.Parse(s.Quantity.ToString())
                        },

                        SaleBranch = s.SaleBranch,
                        SoldBy = new Message.SalesOrderSoldBy
                        {
                            Value = s.SoldBy,
                            SoldById = s.SoldById,
                            SoldByIdSpecified = true
                        },
                        SoldOn = DateTime.SpecifyKind(s.SoldOn.HasValue ? s.SoldOn.Value.Date : DateTime.Now.Date, DateTimeKind.Utc),
                        SecondEffort = false
                    };

                    salesOrder.Add(message);
                };

                var branches = ctx.Branch.ToDictionary(b => b.branchno);

                var GroupId = 0;
                var GroupCount = 0;

                List<ItemWarranty> sortedWarranties = new List<ItemWarranty>();
                sortedWarranties = warranty.OrderBy(w => w.WarrantyGroupId).ToList();

                sortedWarranties.ForEach(w =>
                {
                    var countDel = Convert.ToInt16((from d in ctx.Delivery
                                                    where d.acctno == booking.AcctNo &&
                                                          d.agrmtno == booking.AgreementNo &&
                                                          d.ItemID == w.WarrantyItemId &&
                                                          d.stocklocn == w.WarrantyStockLocn &&
                                                          d.contractno == w.WarrantyContractNo
                                                    select d).Count());

                    if (countDel == 0) //#16309 Warranty not delivered.
                    {
                        if (GroupId != w.WarrantyGroupId)
                        {
                            GroupCount++;
                            GroupId = Convert.ToInt32(w.WarrantyGroupId);
                        }

                        if (GroupCount <= booking.Quantity)
                        {
                            ctx.Delivery.InsertOnSubmit(new Delivery
                            {
                                acctno = w.WarrantyAcctno,
                                agrmtno = Convert.ToInt32(w.WarrantyAgrmtno),
                                branchno = w.WarrantyStockLocn,
                                buffbranchno = booking.DelLocation.HasValue ? booking.DelLocation.Value : Convert.ToInt16(w.WarrantyStockLocn),
                                buffno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno + 1,
                                contractno = w.WarrantyContractNo,
                                transvalue = w.WarrantyOrdval.Value,
                                //datedel = dateDelivered != null ? Convert.ToDateTime(dateDelivered) : DateTime.Today,       // #17506
                                datedel = DateTime.Now.Date,
                                stocklocn = Convert.ToInt16(w.WarrantyStockLocn),
                                delorcoll = "D",
                                ItemID = Convert.ToInt32(w.WarrantyItemId),
                                ParentItemID = booking.ItemId,
                                ParentItemNo = w.WarrantyParentItemNo,
                                transrefno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno + 1,
                                datetrans = DateTime.Now,
                                itemno = w.WarrantyItemNo,
                                quantity = 1,
                                NotifiedBy = confirmedby,
                                ftnotes = "DNWA",
                                BrokerExRunNo = 0,
                                retitemno = string.Empty,
                                runno = 0,
                                RetItemID = 0
                            });

                            var lineitem = (from l in ctx.LineItem
                                            where l.acctno == w.WarrantyAcctno
                                            && l.agrmtno == w.WarrantyAgrmtno
                                            && l.ItemID == w.WarrantyItemId
                                            && l.stocklocn == w.WarrantyStockLocn
                                            && l.contractno == w.WarrantyContractNo
                                            select l).AnsiFirstOrDefault(ctx);

                            lineitem.delqty = Convert.ToDouble(w.WarrantyQuantity);

                            branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno++;
                            branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno++;

                            // #16954 - update outstanding balance for warranty value - Tax value already done in STAX
                            var acct = (from a in ctx.Acct
                                        where a.acctno == w.WarrantyAcctno
                                        select a).AnsiFirstOrDefault(ctx);

                            acct.outstbal += Convert.ToDecimal(w.WarrantyOrdval);
                        }
                    }
                });

                ctx.SubmitChanges();

                var chub = new Chub();
                salesOrder.ForEach(s =>
                {
                    chub.Submit(s, connection, transaction);
                });

            }, conn: conn, trans: tran);
        }

        public void DeliverRenewalItem(SqlConnection conn, SqlTransaction tran, Blue.Cosacs.Model.LineitemBooking booking, int confirmedby = 0)      // #17692
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var warranty = (from l in ctx.LineItem
                                join s in ctx.StockInfo on l.ItemID equals s.Id
                                join sp in ctx.StockPrice on s.Id equals sp.ID
                                where (s.category == 12 || s.category == 82) &&
                                sp.branchno == l.stocklocn &&
                                l.acctno == booking.AcctNo &&
                                l.agrmtno == booking.AgreementNo &&
                                l.parentlocation == booking.StockLocation &&
                                l.ParentItemID == booking.ItemId &&
                                (l.contractno != null || l.contractno != string.Empty) &&
                                !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.contractno == l.contractno && del.ParentItemID == l.ParentItemID) // Exclude warranties already delivered
                                select new ItemWarranty
                                {
                                    WarrantyContractNo = l.contractno,
                                    WarrantyStockLocn = l.stocklocn,
                                    WarrantyQuantity = Convert.ToInt32(l.quantity),
                                    WarrantyItemId = l.ItemID,
                                    WarrantyItemNo = l.itemno,
                                    WarrantyTaxRate = Convert.ToDecimal(s.taxrate),
                                    WarrantyDepartment = s.category.HasValue ? s.category.Value.ToString() : string.Empty,
                                    WarrantyCostPrice = sp.CostPrice,
                                    WarrantyRetailPrice = sp.CashPrice,
                                    WarrantyPrice = l.price,
                                    WarrantyOrdval = l.ordval,
                                    WarrantyParentItemId = l.ParentItemID,
                                    WarrantyParentItemNo = l.parentitemno,
                                    WarrantyParentLocation = l.parentlocation,
                                    WarrantyAgrmtno = l.agrmtno,
                                    WarrantyAcctno = l.acctno,
                                    WarrantyType = s.WarrantyType,                   //#17883
                                    WarrantyLength = s.WarrantyLength,
                                    WarrantyEffectiveDate = l.datereqdel             // #17313
                                }).ToList();

                ReplaceDepartmentWithValidCategory(ctx, warranty);

                var branches = ctx.Branch.ToDictionary(b => b.branchno);

                warranty.ForEach(w =>
                {
                    ctx.Delivery.InsertOnSubmit(new Delivery
                    {
                        acctno = w.WarrantyAcctno,
                        agrmtno = Convert.ToInt32(w.WarrantyAgrmtno),
                        branchno = w.WarrantyStockLocn,
                        buffbranchno = booking.DelLocation.HasValue ? booking.DelLocation.Value : Convert.ToInt16(w.WarrantyStockLocn),
                        buffno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno + 1,
                        contractno = w.WarrantyContractNo,
                        transvalue = w.WarrantyOrdval.Value,
                        datedel = DateTime.Now.Date,                        // #17506
                        stocklocn = Convert.ToInt16(w.WarrantyStockLocn),
                        delorcoll = "D",
                        ItemID = Convert.ToInt32(w.WarrantyItemId),
                        ParentItemID = booking.ItemId,
                        ParentItemNo = w.WarrantyParentItemNo,
                        transrefno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno + 1,
                        datetrans = DateTime.Now,
                        itemno = w.WarrantyItemNo,
                        quantity = 1,
                        NotifiedBy = STL.Common.Static.Credential.UserId,
                        ftnotes = "DNWA",
                        BrokerExRunNo = 0,
                        retitemno = string.Empty,
                        runno = 0,
                        RetItemID = 0
                    });

                    var lineitem = (from l in ctx.LineItem
                                    where l.acctno == w.WarrantyAcctno
                                    && l.agrmtno == w.WarrantyAgrmtno
                                    && l.ItemID == w.WarrantyItemId
                                    && l.stocklocn == w.WarrantyStockLocn
                                    select l).AnsiFirstOrDefault(ctx);

                    lineitem.delqty = Convert.ToDouble(w.WarrantyQuantity);

                    branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno++;
                    branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno++;
                });
                ctx.SubmitChanges();

                var salesOrderView = (from w in ctx.WarrantyRenewalGenerateView
                                      where w.CustomerAccount == booking.AcctNo &&
                                      w.AgreementNumber == booking.AgreementNo &&
                                      w.ItemId == booking.ItemId &&
                                      w.StockLocation == booking.StockLocation
                                      select w).ToList();

                List<Message.SalesOrder> salesOrder = new List<Message.SalesOrder>();

                foreach (var s in salesOrderView)
                {

                    var message = new Message.SalesOrder
                    {
                        Customer = new Message.Customer
                        {
                            AccountNumber = s.CustomerAccount,
                            AddressLine1 = s.CustomerAddressLine1,
                            AddressLine2 = s.CustomerAddressLine2,
                            AddressLine3 = s.CustomerAddressLine3,
                            CustomerId = s.customerId,
                            FirstName = s.CustomerFirstName,
                            LastName = s.CustomerLastName,
                            Notes = s.CustomerNotes,
                            PostCode = s.CustomerPostCode,
                            Title = s.customerTitle,
                            HomePhone = s.homePhone,
                            WorkPhone = s.workPhone,
                            MobilePhone = s.mobilePhone,
                            Email = s.Email,
                            AccountType = s.AccountType
                        },
                        DeliveredOn = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Utc),
                        InvoiceNumber = s.invoiceNumber,
                        Item = new Message.Item
                        {
                            Brand = s.itemBrand,
                            CostPrice = s.ItemCostPrice.HasValue ? s.ItemCostPrice.Value : 0,
                            Description = s.Description,
                            Id = s.ItemId,
                            Number = s.ItemNumber,
                            Price = s.ItemPrice.HasValue ? s.ItemPrice.Value : 0,
                            StockLocation = s.StockLocation,
                            Quantity = Convert.ToInt32(s.Quantity),
                            Supplier = s.itemSupplier,
                            UPC = s.ItemUPC,
                            Warranty = warranty.Count > 0 ? ReturnWarrantyMessage(ctx, conn, tran, warranty, s.CustomerAccount, s.AgreementNumber, s.ItemId, s.StockLocation, Convert.ToInt16(booking.Quantity), true) : null,    //#17200
                            Department = s.Department
                        },

                        SaleBranch = s.SaleBranch,
                        SoldBy = new Message.SalesOrderSoldBy
                        {
                            Value = s.SoldBy,
                            SoldById = s.SoldById,
                            SoldByIdSpecified = true
                        },
                        SoldOn = DateTime.SpecifyKind(s.SoldOn.HasValue ? s.SoldOn.Value.Date : DateTime.Now.Date, DateTimeKind.Utc),
                    };

                    salesOrder.Add(message);
                };


                var chub = new Chub();
                salesOrder.ForEach(s =>
                {
                    chub.Submit(s, connection, transaction);
                });
            }, conn: conn, trans: tran);
        }

        //#15073
        public void CollectItem(SqlConnection conn, SqlTransaction tran, Blue.Cosacs.Model.LineitemBooking booking, int itemQuantity, int confirmedby = 0, int? totalItemQuantity = null)      // #17692
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var warranty = (from l in ctx.LineItem
                                join s in ctx.StockInfo on l.ItemID equals s.Id
                                join sp in ctx.StockPrice on s.Id equals sp.ID
                                join sc in ctx.Schedule on l.acctno equals sc.acctno
                                where (s.category == 12 || s.category == 82) &&                         //#18812
                                sp.branchno == l.stocklocn &&
                                l.acctno == booking.AcctNo &&
                                l.agrmtno == booking.AgreementNo &&
                                l.parentlocation == booking.StockLocation &&
                                l.ParentItemID == booking.ItemId &&
                                (l.contractno != null || l.contractno != string.Empty) &&
                                !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.contractno == l.contractno && del.ParentItemID == l.ParentItemID &&
                                 del.delorcoll == DeliveryOrCollectionType.Collection) &&// Exclude warranties already delivered
                                l.agrmtno == sc.agrmtno && l.ItemID == sc.ItemID && l.stocklocn == sc.stocklocn && l.contractno == sc.contractno

                                select new ItemWarranty
                                {
                                    WarrantyContractNo = l.contractno,
                                    WarrantyStockLocn = l.stocklocn,
                                    WarrantyQuantity = Convert.ToInt32(l.quantity),
                                    WarrantyItemId = l.ItemID,
                                    WarrantyItemNo = l.itemno,
                                    WarrantyTaxRate = Convert.ToDecimal(s.taxrate),
                                    WarrantyCostPrice = sp.CostPrice,
                                    WarrantyRetailPrice = sp.CashPrice,
                                    WarrantyPrice = l.price,
                                    WarrantyOrdval = l.ordval,
                                    WarrantyParentItemId = l.ParentItemID,
                                    WarrantyParentItemNo = l.parentitemno,
                                    WarrantyParentLocation = l.parentlocation,
                                    WarrantyAgrmtno = l.agrmtno,
                                    WarrantyAcctno = l.acctno,
                                    WarrantyType = s.WarrantyType,                          //#17883
                                    WarrantyLength = s.WarrantyLength,
                                    WarrantyGroupId = l.WarrantyGroupId,
                                    WarrantyEffectiveDate = DateTime.MinValue              // #17313
                                }).OrderBy(o => o.WarrantyGroupId).ToList();

                var branches = ctx.Branch.ToDictionary(b => b.branchno);
                var GroupId = 0;
                var GroupCount = 0;

                var AgreementTaxType = (from c in ctx.CountryMaintenance
                                        where c.CodeName == "agrmttaxtype"
                                        select c.Value).FirstOrDefault().ToString();           // #16909

                warranty.ForEach(w =>
                {

                    LineItemAuditWarrantyCollection(connection, transaction, w);

                    //Send message to Cancel warranties on web
                    this.ReturnWarranty(w.WarrantyContractNo, Convert.ToInt16(w.WarrantyStockLocn), connection, transaction, WarrantyStatus.Cancelled, totalItemQuantity);  //#18409

                    //Only select warranties for quantity of items collected.
                    if (GroupId != Convert.ToInt32(w.WarrantyGroupId))
                    {
                        GroupCount++;
                        GroupId = Convert.ToInt32(w.WarrantyGroupId);
                    }

                    if (GroupCount <= (itemQuantity * -1))
                    {
                        ctx.Delivery.InsertOnSubmit(new Delivery
                        {
                            acctno = w.WarrantyAcctno,
                            agrmtno = Convert.ToInt32(w.WarrantyAgrmtno),
                            branchno = w.WarrantyStockLocn,
                            buffbranchno = booking.DelLocation.HasValue ? booking.DelLocation.Value : Convert.ToInt16(w.WarrantyStockLocn),
                            buffno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno + 1,
                            contractno = w.WarrantyContractNo,
                            transvalue = -w.WarrantyOrdval.Value,
                            datedel = DateTime.Today,                    // #17506
                            stocklocn = Convert.ToInt16(w.WarrantyStockLocn),
                            delorcoll = "C",
                            ItemID = Convert.ToInt32(w.WarrantyItemId),
                            ParentItemID = booking.ItemId,
                            ParentItemNo = w.WarrantyParentItemNo,
                            transrefno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno + 1,
                            datetrans = DateTime.Now,
                            itemno = w.WarrantyItemNo,
                            quantity = -1,
                            NotifiedBy = confirmedby,      // #17692
                            ftnotes = "DNWA",
                            BrokerExRunNo = 0,
                            retitemno = string.Empty,
                            runno = 0,
                            RetItemID = 0,
                            retval = Convert.ToDouble(-w.WarrantyOrdval)
                        });

                        //#15179
                        var lineitem = (from l in ctx.LineItem
                                        where l.acctno == w.WarrantyAcctno
                                        && l.agrmtno == w.WarrantyAgrmtno
                                        && l.ItemID == w.WarrantyItemId
                                        && l.stocklocn == w.WarrantyStockLocn
                                        && l.contractno == w.WarrantyContractNo
                                        select l).AnsiFirstOrDefault(ctx);

                        lineitem.delqty = 0;
                        lineitem.quantity = 0;
                        lineitem.ordval = 0;

                        var agreement = (from ag in ctx.Agreement
                                         where ag.acctno == w.WarrantyAcctno
                                         && ag.agrmtno == w.WarrantyAgrmtno
                                         select ag).AnsiFirstOrDefault(ctx);
                        agreement.datechange = DateTime.Now;

                        agreement.agrmttotal += Convert.ToDecimal(-w.WarrantyOrdval);


                        var acct = (from a in ctx.Acct
                                    where a.acctno == w.WarrantyAcctno
                                    select a).AnsiFirstOrDefault(ctx);

                        acct.agrmttotal += Convert.ToDecimal(-w.WarrantyOrdval);
                        acct.outstbal += Convert.ToDecimal(-w.WarrantyOrdval);              //#18554

                        if (AgreementTaxType == "E")   // #16909
                        {
                            agreement.agrmttotal += Convert.ToDecimal(-lineitem.taxamt);
                            acct.agrmttotal += Convert.ToDecimal(-lineitem.taxamt);
                        }

                        lineitem.taxamt = 0;            // #16909

                        branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno++;
                        branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno++;
                    }

                });
                ctx.SubmitChanges();

            }, conn: conn, trans: tran);
        }

        private static void LineItemAuditWarrantyCollection(SqlConnection connection, SqlTransaction transaction, ItemWarranty w)
        {
            DLineItem li = new DLineItem();
            BItem lineItem = new BItem();

            li.GetSingleItem(connection, transaction, int.Parse(w.WarrantyStockLocn.Value.ToString()), w.WarrantyItemId.Value,                  //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and parentItemID rather than ItemNumber and ParentItemNo
                   w.WarrantyAcctno, w.WarrantyAgrmtno.Value, w.WarrantyContractNo, w.WarrantyParentItemId.Value);

            double bfQty = li.Quantity;
            double newQty = li.Quantity - w.WarrantyQuantity.Value;
            decimal bfValue = Convert.ToDecimal(bfQty) * li.Price;
            decimal newValue = Convert.ToDecimal(newQty) * li.Price;
            double bfTaxamt = li.TaxAmount;
            decimal bfPrice = li.Price;
            li.AuditSource = AS.GRTCancel;

            lineItem.AccountNumber = w.WarrantyAcctno;
            lineItem.AgreementNumber = w.WarrantyAgrmtno.Value;
            lineItem.User = STL.Common.Static.Credential.UserId;
            lineItem.ItemNumber = w.WarrantyItemNo;
            lineItem.ItemId = w.WarrantyItemId.Value;
            lineItem.StockLocation = w.WarrantyStockLocn.Value;
            lineItem.ContractNo = w.WarrantyContractNo;
            lineItem.ParentItemId = w.WarrantyParentItemId.Value;
            lineItem.ParentItemNumber = w.WarrantyParentItemNo;
            lineItem.ParentStockLocation = w.WarrantyParentLocation.Value;
            lineItem.AuditSource = AS.GRTCancel;

            lineItem.UpdateLineItemAudit(connection, transaction, bfQty, newQty, bfValue, newValue, bfTaxamt, 0);
        }

        //#16208 - Collecting warranties from Goods Return save
        public void CollectGRTWarranties(SqlConnection conn, SqlTransaction tran, string acctNo, int agrmtNo, DataRow warrantyRow, int confirmedby = 0)      // #17692
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var itemQuantity = (from l in ctx.LineItem
                                    where l.acctno == acctNo &&
                                    l.agrmtno == agrmtNo &&
                                    l.ItemID == int.Parse(warrantyRow["parentitemid"].ToString()) &&
                                    l.stocklocn == short.Parse(warrantyRow["StockLocn"].ToString())
                                    select l.quantity).FirstOrDefault();

                var warranty = (from l in ctx.LineItem
                                join s in ctx.StockInfo on l.ItemID equals s.Id
                                join sp in ctx.StockPrice on s.Id equals sp.ID
                                where (s.category == 12 || s.category == 82) &&                 //#18812
                                sp.branchno == l.stocklocn &&
                                l.acctno == acctNo &&
                                l.agrmtno == agrmtNo &&
                                (l.parentlocation == Convert.ToInt16(warrantyRow["StockLocn"]) &&
                                l.ParentItemID == Convert.ToInt32(warrantyRow["ParentItemId"])
                                 || (l.ParentItemID == 0 && l.parentlocation == 0 && l.WarrantyGroupId == 0)) &&  //Catch redeemed warranties being collected
                                (l.contractno != null || l.contractno != string.Empty) && l.contractno == warrantyRow["contractno"] &&
                                !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.contractno == l.contractno && del.ParentItemID == l.ParentItemID &&
                                 del.delorcoll == DeliveryOrCollectionType.Collection)

                                select new ItemWarranty
                                {
                                    WarrantyContractNo = l.contractno,
                                    WarrantyStockLocn = l.stocklocn,
                                    WarrantyQuantity = Convert.ToInt32(l.quantity),
                                    WarrantyItemId = l.ItemID,
                                    WarrantyItemNo = l.itemno,
                                    WarrantyTaxRate = Convert.ToDecimal(s.taxrate),
                                    WarrantyCostPrice = sp.CostPrice,
                                    WarrantyRetailPrice = sp.CashPrice,
                                    WarrantyPrice = l.price,
                                    WarrantyOrdval = l.ordval,
                                    WarrantyParentItemId = l.ParentItemID,
                                    WarrantyParentItemNo = l.parentitemno,
                                    WarrantyParentLocation = l.parentlocation,
                                    WarrantyAgrmtno = l.agrmtno,
                                    WarrantyAcctno = l.acctno,
                                    WarrantyType = s.WarrantyType,                          //#17883,
                                    WarrantyLength = s.WarrantyLength,
                                    WarrantyGroupId = l.WarrantyGroupId,             //#15993
                                    WarrantyEffectiveDate = DateTime.MinValue             // #17313
                                }).FirstOrDefault();

                var AgreementTaxType = (from c in ctx.CountryMaintenance
                                        where c.CodeName == "agrmttaxtype"
                                        select c.Value).FirstOrDefault().ToString();

                if (warranty != null)
                {
                    var branches = ctx.Branch.ToDictionary(b => b.branchno);

                    //Send message to Cancel warranties on web
                    this.ReturnWarranty(warranty.WarrantyContractNo, Convert.ToInt16(warranty.WarrantyStockLocn), connection, transaction, WarrantyStatus.Cancelled, (int?)itemQuantity); //#18409

                    ctx.Delivery.InsertOnSubmit(new Delivery
                    {
                        acctno = warranty.WarrantyAcctno,
                        agrmtno = Convert.ToInt32(warranty.WarrantyAgrmtno),
                        branchno = warranty.WarrantyStockLocn,
                        buffbranchno = Convert.ToInt16(warranty.WarrantyStockLocn),
                        buffno = branches[Convert.ToInt16(warranty.WarrantyStockLocn)].hibuffno + 1,
                        contractno = warranty.WarrantyContractNo,
                        transvalue = -warranty.WarrantyOrdval.Value,
                        datedel = DateTime.Today,                        // #17506
                        stocklocn = Convert.ToInt16(warranty.WarrantyStockLocn),
                        delorcoll = "C",
                        ItemID = Convert.ToInt32(warranty.WarrantyItemId),
                        ParentItemID = Convert.ToInt32(warranty.WarrantyParentItemId),
                        ParentItemNo = warranty.WarrantyParentItemNo,
                        transrefno = branches[Convert.ToInt16(warranty.WarrantyStockLocn)].hirefno + 1,
                        datetrans = DateTime.Now,
                        itemno = warranty.WarrantyItemNo,
                        quantity = -1,
                        NotifiedBy = confirmedby,     // #17692
                        ftnotes = "DNWA",
                        BrokerExRunNo = 0,
                        retitemno = string.Empty,
                        runno = 0,
                        RetItemID = 0,
                        retval = Convert.ToDouble(-warranty.WarrantyOrdval)
                    });

                    //#15179
                    var lineitem = (from l in ctx.LineItem
                                    where l.acctno == warranty.WarrantyAcctno
                                    && l.agrmtno == warranty.WarrantyAgrmtno
                                    && l.ItemID == warranty.WarrantyItemId
                                    && l.stocklocn == warranty.WarrantyStockLocn
                                    && l.contractno == warranty.WarrantyContractNo
                                    select l).AnsiFirstOrDefault(ctx);

                    lineitem.delqty = 0;
                    lineitem.quantity = 0;
                    lineitem.ordval = 0;

                    var agreement = (from ag in ctx.Agreement
                                     where ag.acctno == warranty.WarrantyAcctno
                                     && ag.agrmtno == warranty.WarrantyAgrmtno
                                     select ag).AnsiFirstOrDefault(ctx);

                    agreement.datechange = DateTime.Now;

                    agreement.agrmttotal += Convert.ToDecimal(-warranty.WarrantyOrdval);

                    var acct = (from a in ctx.Acct
                                where a.acctno == warranty.WarrantyAcctno
                                select a).AnsiFirstOrDefault(ctx);

                    acct.agrmttotal += Convert.ToDecimal(-warranty.WarrantyOrdval);

                    if (AgreementTaxType == "E")
                    {
                        agreement.agrmttotal += Convert.ToDecimal(-lineitem.taxamt);
                        acct.agrmttotal += Convert.ToDecimal(-lineitem.taxamt);
                    }

                    lineitem.taxamt = 0;

                    branches[Convert.ToInt16(warranty.WarrantyStockLocn)].hirefno++;
                    branches[Convert.ToInt16(warranty.WarrantyStockLocn)].hibuffno++;

                    ctx.SubmitChanges();
                }

            }, conn: conn, trans: tran);
        }



        //CR2018-013  - Collecting warranties with Stock item from Goods Return save - Agreement will deduction will not be done here
        public void CollectGRTWarrantiesWithStock(SqlConnection conn, SqlTransaction tran, string acctNo, int agrmtNo, DataRow warrantyRow, int confirmedby = 0)      // #17692
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var itemQuantity = (from l in ctx.LineItem
                                    where l.acctno == acctNo &&
                                    l.agrmtno == agrmtNo &&
                                    l.ItemID == int.Parse(warrantyRow["parentitemid"].ToString()) &&
                                    l.stocklocn == short.Parse(warrantyRow["StockLocn"].ToString())
                                    select l.quantity).FirstOrDefault();

                var warranty = (from l in ctx.LineItem
                                join s in ctx.StockInfo on l.ItemID equals s.Id
                                join sp in ctx.StockPrice on s.Id equals sp.ID
                                where (s.category == 12 || s.category == 82) &&                 //#18812
                                sp.branchno == l.stocklocn &&
                                l.acctno == acctNo &&
                                l.agrmtno == agrmtNo &&
                                (l.parentlocation == Convert.ToInt16(warrantyRow["StockLocn"]) &&
                                l.ParentItemID == Convert.ToInt32(warrantyRow["ParentItemId"])
                                 || (l.ParentItemID == 0 && l.parentlocation == 0 && l.WarrantyGroupId == 0)) &&  //Catch redeemed warranties being collected
                                (l.contractno != null || l.contractno != string.Empty) && l.contractno == warrantyRow["contractno"] &&
                                !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.contractno == l.contractno && del.ParentItemID == l.ParentItemID &&
                                 del.delorcoll == DeliveryOrCollectionType.Collection)

                                select new ItemWarranty
                                {
                                    WarrantyContractNo = l.contractno,
                                    WarrantyStockLocn = l.stocklocn,
                                    WarrantyQuantity = Convert.ToInt32(l.quantity),
                                    WarrantyItemId = l.ItemID,
                                    WarrantyItemNo = l.itemno,
                                    WarrantyTaxRate = Convert.ToDecimal(s.taxrate),
                                    WarrantyCostPrice = sp.CostPrice,
                                    WarrantyRetailPrice = sp.CashPrice,
                                    WarrantyPrice = l.price,
                                    WarrantyOrdval = l.ordval,
                                    WarrantyParentItemId = l.ParentItemID,
                                    WarrantyParentItemNo = l.parentitemno,
                                    WarrantyParentLocation = l.parentlocation,
                                    WarrantyAgrmtno = l.agrmtno,
                                    WarrantyAcctno = l.acctno,
                                    WarrantyType = s.WarrantyType,                          //#17883,
                                    WarrantyLength = s.WarrantyLength,
                                    WarrantyGroupId = l.WarrantyGroupId,             //#15993
                                    WarrantyEffectiveDate = DateTime.MinValue             // #17313
                                }).FirstOrDefault();

                var AgreementTaxType = (from c in ctx.CountryMaintenance
                                        where c.CodeName == "agrmttaxtype"
                                        select c.Value).FirstOrDefault().ToString();

                if (warranty != null)
                {
                    var branches = ctx.Branch.ToDictionary(b => b.branchno);

                    //Send message to Cancel warranties on web
                    this.ReturnWarranty(warranty.WarrantyContractNo, Convert.ToInt16(warranty.WarrantyStockLocn), connection, transaction, WarrantyStatus.Cancelled, (int?)itemQuantity); //#18409

                    ctx.Delivery.InsertOnSubmit(new Delivery
                    {
                        acctno = warranty.WarrantyAcctno,
                        agrmtno = Convert.ToInt32(warranty.WarrantyAgrmtno),
                        branchno = warranty.WarrantyStockLocn,
                        buffbranchno = Convert.ToInt16(warranty.WarrantyStockLocn),
                        buffno = branches[Convert.ToInt16(warranty.WarrantyStockLocn)].hibuffno + 1,
                        contractno = warranty.WarrantyContractNo,
                        transvalue = -warranty.WarrantyOrdval.Value,
                        datedel = DateTime.Today,                        // #17506
                        stocklocn = Convert.ToInt16(warranty.WarrantyStockLocn),
                        delorcoll = "C",
                        ItemID = Convert.ToInt32(warranty.WarrantyItemId),
                        ParentItemID = Convert.ToInt32(warranty.WarrantyParentItemId),
                        ParentItemNo = warranty.WarrantyParentItemNo,
                        transrefno = branches[Convert.ToInt16(warranty.WarrantyStockLocn)].hirefno + 1,
                        datetrans = DateTime.Now,
                        itemno = warranty.WarrantyItemNo,
                        quantity = -1,
                        NotifiedBy = confirmedby,     // #17692
                        ftnotes = "DNWA",
                        BrokerExRunNo = 0,
                        retitemno = string.Empty,
                        runno = 0,
                        RetItemID = 0,
                        retval = Convert.ToDouble(-warranty.WarrantyOrdval)
                    });

                    //#15179
                    var lineitem = (from l in ctx.LineItem
                                    where l.acctno == warranty.WarrantyAcctno
                                    && l.agrmtno == warranty.WarrantyAgrmtno
                                    && l.ItemID == warranty.WarrantyItemId
                                    && l.stocklocn == warranty.WarrantyStockLocn
                                    && l.contractno == warranty.WarrantyContractNo
                                    select l).AnsiFirstOrDefault(ctx);

                    lineitem.delqty = 0;
                    lineitem.quantity = 0;
                    lineitem.ordval = 0;                
                    lineitem.taxamt = 0;

                    branches[Convert.ToInt16(warranty.WarrantyStockLocn)].hirefno++;
                    branches[Convert.ToInt16(warranty.WarrantyStockLocn)].hibuffno++;

                    ctx.SubmitChanges();
                }

            }, conn: conn, trans: tran);
        }

        //#16339 - Collect Cash & Go Warranties
        public void CollectCashAndGoWarranties(SqlConnection conn, SqlTransaction tran, Blue.Cosacs.Model.LineitemBooking booking, int itemQuantity, int confirmedby = 0)      // #17692
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var warranty = (from l in ctx.LineItem
                                join s in ctx.StockInfo on l.ItemID equals s.Id
                                join sp in ctx.StockPrice on s.Id equals sp.ID
                                where (s.category == 12 || s.category == 82) &&                     //#18812
                                sp.branchno == l.stocklocn &&
                                l.acctno == booking.AcctNo &&
                                l.agrmtno == booking.AgreementNo &&
                                l.parentlocation == booking.StockLocation &&
                                l.ParentItemID == booking.ItemId &&
                                l.quantity > 0 &&     //#18490
                                (l.contractno != null || l.contractno != string.Empty) &&
                                !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.contractno == l.contractno && del.ParentItemID == l.ParentItemID &&
                                 del.delorcoll == DeliveryOrCollectionType.Collection) // Exclude warranties already delivered                               

                                select new ItemWarranty
                                {
                                    WarrantyContractNo = l.contractno,
                                    WarrantyStockLocn = l.stocklocn,
                                    WarrantyQuantity = Convert.ToInt32(l.quantity),
                                    WarrantyItemId = l.ItemID,
                                    WarrantyItemNo = l.itemno,
                                    WarrantyTaxRate = Convert.ToDecimal(s.taxrate),
                                    WarrantyCostPrice = sp.CostPrice,
                                    WarrantyRetailPrice = sp.CashPrice,
                                    WarrantyPrice = l.price,
                                    WarrantyOrdval = l.ordval,
                                    WarrantyParentItemId = l.ParentItemID,
                                    WarrantyParentItemNo = l.parentitemno,
                                    WarrantyParentLocation = l.parentlocation,
                                    WarrantyAgrmtno = l.agrmtno,
                                    WarrantyAcctno = l.acctno,
                                    WarrantyType = s.WarrantyType,                          //#17883,
                                    WarrantyLength = s.WarrantyLength,
                                    WarrantyGroupId = l.WarrantyGroupId,             //#15993
                                    WarrantyEffectiveDate = DateTime.MinValue             // #17313
                                }).OrderBy(o => o.WarrantyGroupId).ToList();

                var branches = ctx.Branch.ToDictionary(b => b.branchno);
                var GroupId = 0;
                var GroupCount = 0;

                warranty.ForEach(w =>
                {
                    //Send message to Cancel warranties on web
                    this.ReturnWarranty(w.WarrantyContractNo, Convert.ToInt16(w.WarrantyStockLocn), connection, transaction, WarrantyStatus.Cancelled);  //#18409

                    //Only select warranties for quantity of items collected.
                    if (GroupId != Convert.ToInt32(w.WarrantyGroupId))
                    {
                        GroupCount++;
                        GroupId = Convert.ToInt32(w.WarrantyGroupId);
                    }

                    if (GroupCount <= Math.Abs(itemQuantity))
                    {
                        ctx.Delivery.InsertOnSubmit(new Delivery
                        {
                            acctno = w.WarrantyAcctno,
                            agrmtno = Convert.ToInt32(w.WarrantyAgrmtno),
                            branchno = w.WarrantyStockLocn,
                            buffbranchno = booking.DelLocation.HasValue ? booking.DelLocation.Value : Convert.ToInt16(w.WarrantyStockLocn),
                            buffno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno + 1,
                            contractno = w.WarrantyContractNo,
                            transvalue = -w.WarrantyOrdval.Value,
                            datedel = DateTime.Today,                   // #17506
                            stocklocn = Convert.ToInt16(w.WarrantyStockLocn),
                            delorcoll = "C",
                            ItemID = Convert.ToInt32(w.WarrantyItemId),
                            ParentItemID = booking.ItemId,
                            ParentItemNo = w.WarrantyParentItemNo,
                            transrefno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno + 1,
                            datetrans = DateTime.Now,
                            itemno = w.WarrantyItemNo,
                            quantity = -1,
                            NotifiedBy = confirmedby,     // #17692
                            ftnotes = "DNWA",            // #14889  
                            BrokerExRunNo = 0,
                            retitemno = string.Empty,
                            runno = 0,
                            RetItemID = 0,
                            retval = Convert.ToDouble(-w.WarrantyOrdval)
                        });

                        //#15179
                        var lineitem = (from l in ctx.LineItem
                                        where l.acctno == w.WarrantyAcctno
                                        && l.agrmtno == w.WarrantyAgrmtno
                                        && l.ItemID == w.WarrantyItemId
                                        && l.stocklocn == w.WarrantyStockLocn
                                        && l.contractno == w.WarrantyContractNo
                                        select l).AnsiFirstOrDefault(ctx);

                        lineitem.delqty = 0;
                        lineitem.quantity = 0;
                        lineitem.ordval = 0;

                        var agreement = (from ag in ctx.Agreement
                                         where ag.acctno == w.WarrantyAcctno
                                         && ag.agrmtno == w.WarrantyAgrmtno
                                         select ag).AnsiFirstOrDefault(ctx);
                        agreement.datechange = DateTime.Now;

                        agreement.agrmttotal += Convert.ToDecimal(-w.WarrantyOrdval);

                        var acct = (from a in ctx.Acct
                                    where a.acctno == w.WarrantyAcctno
                                    select a).AnsiFirstOrDefault(ctx);

                        acct.agrmttotal += Convert.ToDecimal(-w.WarrantyOrdval);

                        branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno++;
                        branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno++;
                    }
                });
                ctx.SubmitChanges();

            }, conn: conn, trans: tran);
        }

        //#15639 - Method to populate the Warranty in the SalesOrder Message
        public Message.Warranty[] ReturnWarrantyMessage(Context ctx, SqlConnection conn, SqlTransaction trans,
            List<ItemWarranty> warranties, string acctNo, int agrmtNo, int itemId,
            short stockLocn, int quantity, bool renewal = false, bool isTaxFree = false) //#17200 //#16206
        {
            var delayIRW = GetDelayIRCountryParameterValue(conn, trans);  //#17623

            List<Message.Warranty> warrantyList = new List<Message.Warranty>();

            var sortedWarranties = warranties.OrderBy(w => w.WarrantyType).ToList();                //#17883

            //First check if warranties already delivered for this item, if so get the Max WarrantyGroupId for Free & Extended
            //as we will need to incremenet from this number.
            var warrantyGroupIdFree = GetMaxWarrantyGroupId(conn, trans, acctNo, agrmtNo, itemId, stockLocn, WarrantyType.Free);        // #17287 
            var warrantyGroupIdExt = GetMaxWarrantyGroupId(conn, trans, acctNo, agrmtNo, itemId, stockLocn, WarrantyType.Extended);        // #17287 
            var warrantyGroupIdIR = GetMaxWarrantyGroupId(conn, trans, acctNo, agrmtNo, itemId, stockLocn, WarrantyType.InstantReplacement);        // #17287

            // #17287 
            int indexI = sortedWarranties.FindIndex(f => f.WarrantyType == WarrantyType.InstantReplacement);
            int indexE = sortedWarranties.FindIndex(f => f.WarrantyType == WarrantyType.Extended);
            int indexF = sortedWarranties.FindIndex(f => f.WarrantyType == WarrantyType.Free);   //#18437
            var type = indexI >= 0 ? WarrantyType.InstantReplacement : WarrantyType.Extended;
            type = indexE >= 0 ? WarrantyType.Extended : WarrantyType.Free;
            // set starting groupid depending on warranty type

            var replacementGroupId = (indexI != -1 && indexF != -1) ? (warrantyGroupIdIR > warrantyGroupIdFree) ? warrantyGroupIdIR : warrantyGroupIdFree : -1;

            var warrantyGroupId = 0;
            if (warrantyGroupIdIR != 0 || indexI != -1) //#18437  // #17290 - Instant replacement exists
            {
                warrantyGroupId = replacementGroupId != -1 ? replacementGroupId : type != WarrantyType.InstantReplacement ? warrantyGroupIdFree : warrantyGroupIdIR;  //#18437 // #17287
                ////warrantyGroupId = type != WarrantyType.InstantReplacement ? ((warrantyGroupIdFree < warrantyGroupIdIR) ? warrantyGroupIdIR : warrantyGroupIdFree ) : warrantyGroupIdIR;  // #17287
            }
            else
            {
                warrantyGroupId = warrantyGroupIdFree < (type != WarrantyType.InstantReplacement ? warrantyGroupIdExt : warrantyGroupIdIR) ? warrantyGroupIdFree : (type != WarrantyType.InstantReplacement ? warrantyGroupIdExt : warrantyGroupIdIR);  // #17287
            }

            var origwarrantyGroupId = warrantyGroupId;

            var count = 0;


            sortedWarranties.ForEach(w =>
            {
                if (type != w.WarrantyType && count != 0) //Switching from Extended to Free     //#17883
                {
                    origwarrantyGroupId = replacementGroupId != -1 ? replacementGroupId : warrantyGroupIdFree; //#18437
                    warrantyGroupId = origwarrantyGroupId + 1;
                    count = warrantyGroupId == 0 ? count : warrantyGroupId;
                }
                else
                {
                    if (type == WarrantyType.Extended && warrantyGroupIdFree < warrantyGroupIdExt)    //#17883 #16328
                    {
                        warrantyGroupId = warrantyGroupIdExt;
                        warrantyGroupIdExt++;       //#17287
                    }
                    if (type == WarrantyType.InstantReplacement && warrantyGroupIdFree < warrantyGroupIdIR)    //#17287 
                    {
                        warrantyGroupId = warrantyGroupIdIR;
                        warrantyGroupIdIR++;        //#17287
                    }

                    warrantyGroupId++;

                    count = warrantyGroupId == 0 ? count + 1 : warrantyGroupId;
                }

                if (count == 0)
                {
                    count = warrantyGroupId == 0 ? count + 1 : warrantyGroupId + 1;
                }

                type = w.WarrantyType;             //#17883
                w.WarrantyGroupId = count;

            });

            // #17287
            if (indexI >= 0)       // index>=0 Instant replacement exists therefore sort ascending (F,I) - Free warranty first
            {
                sortedWarranties = warranties.OrderBy(w => w.WarrantyGroupId).ThenBy(w => w.WarrantyType).ToList(); //Sort in GroupId order  //#17883
            }
            else        // No Instant replacement exists therefore sort descending (F,E) - Free warranty first
            {
                sortedWarranties = warranties.OrderBy(w => w.WarrantyGroupId).ThenByDescending(w => w.WarrantyType).ToList(); //Sort in GroupId order  //#17883
            }

            var GroupId = 0;
            var GroupCount = 0;

            //#17678 - Select replacements 
            var replacement = (from e in ctx.WarrantyReplacementView
                               where e.acctno == acctNo
                               && e.agrmtno == agrmtNo && e.stocklocn == stockLocn && (e.ItemID == itemId || e.ReplacementItemId == itemId)           // #17290
                               select e).OrderByDescending(e => e.ExchangeDate).ToList();

            var replacementCount = replacement.Count();
            ////var sortedWarrantiesCount = 0;
            //var replacementWarrantyLength = replacement.Count() != 0 ? replacement[0].ReplaceWarrantyLength.Value : 0;              //#17290

            //#14508 - For Identical Replacement need to update Exchange.ReplacementItemId to indicate replacement processed.
            if (replacementCount > 0 && replacement[0].CollectionType == "R") //Indicates Identical Replacement
            {
                new AccountRepository().UpdateExchangeReplaceItem(conn, trans, acctNo, agrmtNo, Convert.ToInt32(replacement[0].ItemID), stockLocn, itemId);
            }


            sortedWarranties.ForEach(w =>
            {

                //#16206
                if (GroupId != w.WarrantyGroupId)
                {
                    GroupCount++;
                    GroupId = Convert.ToInt32(w.WarrantyGroupId);
                }

                //Only add warranties Free & Extended for number of items delivered.
                //#18437
                if (GroupCount <= quantity || renewal)          //#17200
                {


                    var RedeemContractNumber = (from r in replacement
                                                where !(from wl in warrantyList
                                                        select wl.RedeemContractNumber)
                                                        .Contains(r.contractno)
                                                select r.contractno).FirstOrDefault() ?? string.Empty;

                    var ReLinkContractNumber = (from r in replacement
                                                where !(from wl in warrantyList
                                                        select wl.ReLinkContractNo)
                                                        .Contains(r.ReLinkContractNo)
                                                select r.ReLinkContractNo).FirstOrDefault() ?? string.Empty;


                    warrantyList.Add(
                    new Message.Warranty
                    {
                        ContractNumber = w.WarrantyContractNo,
                        CostPrice = Convert.ToDecimal(w.WarrantyCostPrice),
                        Id = Convert.ToInt32(w.WarrantyItemId),
                        TypeCode = w.WarrantyType,               //#17883
                        //Length = w.WarrantyLength.HasValue ? (short)w.WarrantyLength.Value : (short)0,
                        Length = replacement.Count != 0 && WarrantyType.IsFree(Convert.ToString(w.WarrantyType)) ? (short)replacement[0].ReplaceWarrantyLength : w.WarrantyLength.HasValue ? (short)w.WarrantyLength.Value : (short)0, //#17290   //#17678
                        Number = w.WarrantyItemNo,
                        RetailPrice = Convert.ToDecimal(w.WarrantyRetailPrice),
                        SalePrice = Convert.ToDecimal(w.WarrantyPrice),
                        TaxRate = isTaxFree ? 0 : Convert.ToDecimal(w.WarrantyTaxRate),
                        Department = w.WarrantyDepartment,
                        WarrantyGroupId = Convert.ToInt32(w.WarrantyGroupId),
                        EffectiveDate = delayIRW == false && replacement.Count != 0 && w.WarrantyType == WarrantyType.InstantReplacement ? DateTime.Today : w.WarrantyEffectiveDate.Value,     //#17623
                        RedeemContractNumber = replacement.Count != 0 && (WarrantyType.IsFree(Convert.ToString(w.WarrantyType)) || WarrantyType.IsInstantReplacement(Convert.ToString(w.WarrantyType))) ? RedeemContractNumber : "",    // #18462
                        ReLinkContractNo = replacement.Count != 0 && WarrantyType.IsFree(Convert.ToString(w.WarrantyType)) ? (agrmtNo > 1 ? replacement[0].LinkIrwContractno : ReLinkContractNumber) : "",    //#18389
                        WarrantyDeliveredOn = DateTime.Now.Date,
                        WarrantyDeliveredOnSpecified = true
                    });

                    //#18389 - Update LineItem.WarrantyGroupId if when doing a replacement the link of an IR/Extended warranty has changed to a different item.
                    if (WarrantyType.IsFree(Convert.ToString(w.WarrantyType)) && replacement.Count != 0)
                    {
                        if (replacement[0].ReLinkContractNo != "")
                        {
                            UpdateLineItemWarrantyGroupId(conn, trans, acctNo, agrmtNo, replacement[0].ReLinkContractNo, w.WarrantyGroupId.Value);
                        }
                    }

                }

                ////sortedWarrantiesCount++;
            });

            //Update WarrantyGroupId on the Lineitem table
            UpdateLineItemWarrantyGroupId(conn, trans, acctNo, agrmtNo, warrantyList);

            //#17290 - Update Lineitem.ParentItemId for any Replacement warranties previously linked to a different item
            // on performing a replacement
            if (agrmtNo > 1 && replacement.Count() > 0 && Convert.ToInt32(replacement[0].LinkIrwId) != 0)
            {
                UpdateLineItemForReplacement(conn, trans, acctNo, agrmtNo, Convert.ToInt32(replacement[0].ReplacementItemId),
                                                 Convert.ToInt32(replacement[0].LinkIrwId), Convert.ToString(replacement[0].LinkIrwContractno));
            }

            return warrantyList.ToArray();
        }

        //#15183 - Return the max WarrantyGroupId from the WarrantySales table.
        public int GetMaxWarrantyGroupId(SqlConnection conn, SqlTransaction tran, string account, int agrmtNo, int itemId, short stockLocn, string type)
        {
            var maxWarrantyGroupId = (int?)null;

            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                maxWarrantyGroupId = (from l in ctx.LineItem
                                      join s in ctx.StockInfo on l.ItemID equals s.Id
                                      where l.acctno == account
                                      && l.agrmtno == agrmtNo
                                      && l.ParentItemID == itemId
                                      && l.parentlocation == stockLocn
                                      && l.contractno != string.Empty
                                      && s.WarrantyType == type           // #17287 #17883
                                      select l.WarrantyGroupId).Max();

            }, conn: conn, trans: tran);

            return maxWarrantyGroupId == null ? 0 : Convert.ToInt32(maxWarrantyGroupId);

        }

        //#15183 - Update Lineitem.WarrantyGroupId
        public void UpdateLineItemWarrantyGroupId(SqlConnection conn, SqlTransaction tran, string acctNo, int agrmtNo, List<Message.Warranty> warranties)
        {

            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                warranties.ForEach(w =>
                {
                    var lineitem = (from l in ctx.LineItem
                                    where l.acctno == acctNo
                                    && l.agrmtno == agrmtNo
                                    && l.contractno == w.ContractNumber
                                    select l).FirstOrDefault();

                    lineitem.WarrantyGroupId = w.WarrantyGroupId;

                });

                ctx.SubmitChanges();

            }, conn: conn, trans: tran);

        }

        //#18389 - Update Lineitem.WarrantyGroupId for a specific warranty to a new warrantyGroupdId (where link of warranty changed from one item to another)
        public void UpdateLineItemWarrantyGroupId(SqlConnection conn, SqlTransaction tran, string acctNo, int agrmtNo, string contractNo, int warrantyGroupId)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var lineitem = (from l in ctx.LineItem
                                where l.acctno == acctNo
                                && l.agrmtno == agrmtNo
                                && l.contractno == contractNo
                                && l.WarrantyGroupId != warrantyGroupId
                                select l).FirstOrDefault();

                if (lineitem != null)
                {
                    lineitem.WarrantyGroupId = warrantyGroupId;
                }

                ctx.SubmitChanges();

            }, conn: conn, trans: tran);
        }

        //#17290 - Replacement for Cash & Go update ParentItemId of existing IR warraty to replacement Item if different to original
        public void UpdateLineItemForReplacement(SqlConnection conn, SqlTransaction tran, string acctNo, int agrmtNo, int replacementItemId,
                                                    int linkIrwId, string linkIrwContractno)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var lineitem = (from l in ctx.LineItem
                                where l.acctno == acctNo
                                && l.agrmtno == agrmtNo
                                && l.ItemID == linkIrwId
                                && l.contractno == linkIrwContractno
                                select l).FirstOrDefault();

                lineitem.ParentItemID = replacementItemId;

                ctx.SubmitChanges();

            }, conn: conn, trans: tran);

        }

        //#17290 - Replacement for Cash & Go update ParentItemId of existing IR warraty to replacement Item if different to original
        public bool GetDelayIRCountryParameterValue(SqlConnection conn, SqlTransaction tran)
        {
            var delayIR = "";

            Context.ExecuteTx((ctx, connection, transaction) =>
            {

                delayIR = (from c in ctx.CountryMaintenance
                           where c.CodeName == CountryParameterNames.DelayNewIRW
                           select c.Value).FirstOrDefault();

            }, conn: conn, trans: tran);

            return Convert.ToBoolean(delayIR);
        }

        public void ReturnWarranty(DataSet items, SqlConnection c, SqlTransaction t)
        {
            var chub = new Chub();
            var table = items.Tables[TN.Deliveries];

            foreach (DataRow row in table.Rows)
            {
                var itemType = (string)row[CN.ItemType];
                if (itemType == "N" && ((string)row[CN.ContractNo]) != "")
                {
                    chub.Cancel(new Message.SalesOrderCancel
                    {
                        ContractNumber = (string)row[CN.ContractNo],
                        SaleBranch = Convert.ToInt16(row[CN.StockLocn]),
                        Status = WarrantyStatus.Cancelled,
                        IsRepossessed = row[CN.DelOrColl].ToString() == "R" && int.Parse(row[CN.Quantity].ToString()) < 0,
                        CancelledDate = DateTime.Now.Date

                    }, c, t);
                }
                else if (itemType == "S")
                {
                    chub.Cancel(new Message.SalesOrderCancelItem
                    {
                        AccountNumber = (string)row[CN.AcctNo],
                        InvoiceNumber = string.Format("{0} {1}", row[CN.AcctNo], row[CN.AgrmtNo]),
                        ItemId = Convert.ToInt32(row[CN.ItemId]),
                        Quantity = Math.Abs(Convert.ToInt32(row[CN.Quantity])),
                        SaleBranch = Convert.ToInt16(row[CN.StockLocn])
                    }, c, t);
                }
            }

            // MV: I have no idea what the below code is doing... but it doesn't seem to work...
            // HP: up there doen't seen much better...
            if (items.Tables[TN.NonStockList] != null && items.Tables[TN.NonStockList].Rows.Count > 0)
            {
                foreach (DataRow row in items.Tables[TN.NonStockList].Rows)
                {
                    chub.Cancel(new Message.SalesOrderCancel
                    {
                        ContractNumber = (string)row[CN.ContractNo],
                        // MV: why are we sending the Stock branch for the warranty?
                        SaleBranch = (short)Convert.ToInt32(row[CN.StockLocn]),
                        Status = WarrantyStatus.Cancelled,
                        CancelledDate = DateTime.Now.Date

                    }, c, t);
                }
            }
        }

        public void ReturnWarranty(string contractNo, short stockLocn, SqlConnection c, SqlTransaction t, string status, int? totalItemQuantity = null)
        {
            var chub = new Chub();

            chub.Cancel(new Message.SalesOrderCancel
            {
                ContractNumber = contractNo,
                SaleBranch = stockLocn,
                Status = status,
                ItemQuantity = totalItemQuantity,
                CancelledDate = DateTime.Now.Date
            }, c, t);
        }

        public List<WarrantyDeliveredView> GetRenewableWarranties(string account)
        {
            //#16017
            var WarrantyExpPromptDays = CountryParameterCache.GetCountryParameter<int>(CountryParameterNames.ActivePrompDays);
            var WarrantyPromptDaysAfterExp = CountryParameterCache.GetCountryParameter<int>(CountryParameterNames.WarrantyExpiryMaxPrompt);
            var WarrantyExpSettlementDays = CountryParameterCache.GetCountryParameter<int>(CountryParameterNames.WarrantyExpiryPromptDays);

            return Context.ExecuteTx<List<WarrantyDeliveredView>>((ctx, connection, transaction) =>
            {
                return (from w in ctx.WarrantyDeliveredView
                        where w.acctno == account
                        select w).Where(w => ((w.EffectiveDate.Value.AddMonths(w.warrantyLength ?? 0)).AddDays(-WarrantyExpPromptDays) < DateTime.Today // Days before expiry
                            && ((w.EffectiveDate.Value.AddMonths(w.warrantyLength ?? 0)).AddDays(WarrantyPromptDaysAfterExp) > DateTime.Today)  //Days after expiry
                                && w.currstatus != "S") //Not settled

                            || (w.EffectiveDate.Value.AddMonths(w.warrantyLength ?? 0) >= DateTime.Today && //Warranty expiry
                                    ((w.EffectiveDate.Value.AddMonths(w.warrantyLength ?? 0)).AddDays(-WarrantyExpSettlementDays) < DateTime.Today) //Days prior to settlement
                                        && w.currstatus == "S") //Settled 
                            ).ToList();
            });
        }

        public Tuple<int?, DateTime?, DateTime?, int, string> GetWarrantyElapsedMonthsDel(int warrantyId, string contractno, int stocklocn)          // #17506
        {
            return Context.ExecuteTx<Tuple<int?, DateTime?, DateTime?, int, string>>((ctx, connection, transaction) =>                   // #17506
            {
                var warranty = (from w in ctx.WarrantyDeliveredView
                                where w.ItemId == warrantyId
                                && w.stocklocn == stocklocn
                                && w.contractno == contractno
                                select w).FirstOrDefault();
                if (warranty != null)
                    //return new Tuple<int?, DateTime?>(DateTime.Now.MonthDifference(warranty.datedel.Value), warranty.datedel);
                    return new Tuple<int?, DateTime?, DateTime?, int, string>(DateTime.Now.MonthDifference(warranty.ProductDeliveryDate.Value), warranty.EffectiveDate, warranty.WarrantyDeliveredDate, warranty.FreeWarrantyLength ?? 12, warranty.contractno);     // #17506

                else
                    return null;
            });
        }

        public List<ItemsWithoutWarrantiesView> GetItemsWithoutWarranties(string custid)
        {
            return Context.ExecuteTx<List<ItemsWithoutWarrantiesView>>((ctx, connection, transaction) =>
            {

                return (from i in ctx.ItemsWithoutWarrantiesView
                        where i.custid == custid
                        select i).ToList();
            });
        }

        // #15181 Cash&Go items 
        public void DeliverCGItem(SqlConnection conn, SqlTransaction tran, string accountNo, int agreementNo, int itemId, short stockLocn,
            int quantity, XmlNode replacement, int notifiedBy = -1, bool isTaxFree = false)  //#18409
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var warranty = (from l in ctx.LineItem
                                join s in ctx.StockInfo on l.ItemID equals s.Id
                                join sp in ctx.StockPrice on s.Id equals sp.ID
                                where (s.category == 12 || s.category == 82) &&
                                sp.branchno == l.stocklocn &&
                                l.acctno == accountNo &&
                                l.agrmtno == agreementNo &&
                                l.ParentItemID == itemId &&
                                l.parentlocation == stockLocn &&
                                l.quantity > 0 &&
                                (l.contractno != null || l.contractno != string.Empty) &&
                                !ctx.Delivery.Any(del => del.ItemID == l.ItemID && del.contractno == l.contractno && del.ParentItemID == l.ParentItemID)// Exclude warranties already delivered
                                select new ItemWarranty
                                {
                                    WarrantyContractNo = l.contractno,
                                    WarrantyStockLocn = l.stocklocn,
                                    WarrantyQuantity = Convert.ToInt32(l.quantity),
                                    WarrantyItemId = l.ItemID,
                                    WarrantyItemNo = l.itemno,
                                    WarrantyTaxRate = Convert.ToDecimal(s.taxrate),
                                    WarrantyDepartment = s.category.HasValue ? s.category.Value.ToString() : string.Empty,
                                    WarrantyCostPrice = sp.CostPrice,
                                    WarrantyRetailPrice = sp.CashPrice,
                                    WarrantyPrice = l.price,
                                    WarrantyOrdval = l.ordval,
                                    WarrantyParentItemId = l.ParentItemID,
                                    WarrantyParentItemNo = l.parentitemno,
                                    WarrantyParentLocation = l.parentlocation,
                                    WarrantyAgrmtno = l.agrmtno,
                                    WarrantyAcctno = l.acctno,
                                    WarrantyType = s.WarrantyType,                      //#17883
                                    WarrantyLength = s.WarrantyLength,
                                    WarrantyEffectiveDate = DateTime.MinValue              // #17313
                                }).ToList();

                ReplaceDepartmentWithValidCategory(ctx, warranty);

                var branches = ctx.Branch.ToDictionary(b => b.branchno);

                warranty.ForEach(w =>
                {
                    ctx.Delivery.InsertOnSubmit(new Delivery
                    {
                        origbr = 0,
                        acctno = w.WarrantyAcctno,
                        agrmtno = Convert.ToInt32(w.WarrantyAgrmtno),
                        branchno = w.WarrantyStockLocn,
                        buffbranchno = Convert.ToInt16(w.WarrantyStockLocn),
                        buffno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno + 1,
                        contractno = w.WarrantyContractNo,
                        transvalue = w.WarrantyOrdval.Value,
                        datedel = DateTime.Now.Date,            // #17506
                        stocklocn = Convert.ToInt16(w.WarrantyStockLocn),
                        delorcoll = "D",
                        ItemID = Convert.ToInt32(w.WarrantyItemId),
                        ParentItemID = Convert.ToInt32(w.WarrantyParentItemId),
                        ParentItemNo = w.WarrantyParentItemNo,
                        transrefno = branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno + 1,
                        datetrans = DateTime.Now,
                        itemno = w.WarrantyItemNo,
                        quantity = 1,
                        NotifiedBy = notifiedBy < 0 ? STL.Common.Static.Credential.UserId : notifiedBy,
                        ftnotes = "DNWG",
                        BrokerExRunNo = 0,
                        retitemno = string.Empty,
                        runno = 0,
                        RetItemID = 0
                    });

                    //#18364 Select Paymethod used for Cash & Go sale to insert PAY trans into Fintrans for the warranty
                    var payMethod = (from f in ctx.FinTrans
                                     where f.acctno == w.WarrantyAcctno &&
                                           f.agrmtno == w.WarrantyAgrmtno.Value &&
                                           f.transtypecode == TransType.Payment
                                     select f.paymethod).Take(1).AnsiFirstOrDefault(ctx);

                    //#18251
                    BAccount baccount = new BAccount();
                    var isPaidAndTaken = baccount.IsPaidAndTakenAccount(conn, tran, Convert.ToString(w.WarrantyAcctno));

                    ////// #18488 - removed - payment processed
                    ////if (!WarrantyType.IsFree(w.WarrantyType) && isPaidAndTaken)
                    ////{
                    ////    BTransaction t = new BTransaction(conn, tran, w.WarrantyAcctno, w.WarrantyStockLocn.Value,
                    ////                      branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno + 1, -w.WarrantyOrdval.Value, STL.Common.Static.Credential.UserId, TransType.Payment,
                    ////                       "", "", "", payMethod.Value, CountryParameterCache.GetCountryParameter<string>(CountryParameterNames.CountryCode),
                    ////                       DateTime.Now, "", w.WarrantyAgrmtno.Value);

                    ////}

                    branches[Convert.ToInt16(w.WarrantyStockLocn)].hirefno++;
                    branches[Convert.ToInt16(w.WarrantyStockLocn)].hibuffno++;
                    // #16954 - update outstanding balance for warranty value - Tax value already done in STAX
                    //////if (!isPaidAndTaken)
                    //////{
                    // #18488 - must update balance with warranty value
                    var acct = (from a in ctx.Acct
                                where a.acctno == w.WarrantyAcctno
                                select a).AnsiFirstOrDefault(ctx);

                    acct.outstbal += Convert.ToDecimal(w.WarrantyOrdval);
                    //////}

                });


                ctx.SubmitChanges();

                var salesOrderView = (from w in ctx.WarrantySaleGenerateView
                                      where w.CustomerAccount == accountNo &&
                                      w.AgreementNumber == agreementNo &&
                                      w.ItemId == itemId &&
                                      w.StockLocation == stockLocn
                                      select w).ToList();

                List<Message.SalesOrder> salesOrder = new List<Message.SalesOrder>();

                foreach (var s in salesOrderView)
                {
                    var message = new Message.SalesOrder
                    {
                        Customer = new Message.Customer
                        {
                            AccountNumber = s.CustomerAccount,
                            AddressLine1 = s.CustomerAddressLine1,
                            AddressLine2 = s.CustomerAddressLine2,
                            AddressLine3 = s.CustomerAddressLine3,
                            CustomerId = s.customerId,
                            FirstName = s.CustomerFirstName,
                            LastName = s.CustomerLastName,
                            Notes = s.CustomerNotes,
                            PostCode = s.CustomerPostCode,
                            Title = s.customerTitle,
                            HomePhone = s.homePhone,
                            WorkPhone = s.workPhone,
                            MobilePhone = s.mobilePhone,
                            Email = s.Email,
                            AccountType = s.AccountType
                        },
                        DeliveredOn = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Utc),
                        InvoiceNumber = s.invoiceNumber,
                        Item = new Message.Item
                        {
                            Brand = s.itemBrand,
                            CostPrice = s.ItemCostPrice.HasValue ? s.ItemCostPrice.Value : 0,
                            Description = s.Description,
                            Id = s.ItemId,
                            Number = s.ItemNumber,
                            Price = s.ItemPrice,
                            StockLocation = s.StockLocation,
                            Quantity = Convert.ToInt32(s.Quantity),
                            Supplier = s.itemSupplier,
                            UPC = s.ItemUPC,
                            Warranty = warranty.Count > 0 ? ReturnWarrantyMessage(ctx, conn, tran, warranty, s.CustomerAccount,
                                        agreementNo, s.ItemId, s.StockLocation, quantity, false, isTaxFree) : null,
                            Department = s.Department
                        },

                        SaleBranch = s.SaleBranch,
                        SoldBy = new Message.SalesOrderSoldBy
                        {
                            Value = s.SoldBy,
                            SoldById = s.SoldById,
                            SoldByIdSpecified = true
                        },
                        SoldOn = DateTime.SpecifyKind(s.SoldOn.HasValue ? s.SoldOn.Value.Date : DateTime.Now.Date, DateTimeKind.Utc)
                    };

                    salesOrder.Add(message);
                };

                var chub = new Chub();
                salesOrder.ForEach(s =>
                {
                    chub.Submit(s, connection, transaction);
                });

                //#18409
                if (salesOrder != null && salesOrder.Any() && salesOrder[0].Item.Warranty == null && replacement != null)
                {
                    InstantReplacementDetails replacementDet = InstantReplacementDetails.DeSerialise(replacement);

                    var warrantyToRedeem = (from r in ctx.WarrantyReplacementView
                                            where r.acctno == accountNo &&
                                                  r.agrmtno == agreementNo &&
                                                  r.ItemID == replacementDet.ItemId &&
                                                  r.stocklocn == replacementDet.StockLocn
                                            select r).OrderByDescending(r => r.ExchangeDate).FirstOrDefault();

                    new WarrantyRepository().ReturnWarranty(warrantyToRedeem.contractno, warrantyToRedeem.stocklocn, connection, transaction, WarrantyStatus.Redeemeded);
                }

            }, conn: conn, trans: tran);
        }

        public void ReturnItem(string acctno, int agrmtNo, int itemID, short stockLocn, int qty, SqlConnection c, SqlTransaction t, int? totalQuantity = null)
        {
            var chub = new Chub();

            chub.Cancel(new Message.SalesOrderCancelItem
            {
                AccountNumber = acctno,
                InvoiceNumber = acctno + " " + agrmtNo,
                ItemId = itemID,
                SaleBranch = stockLocn,
                Quantity = qty,
                TotalQuantity = totalQuantity
            }, c, t);
        }


        //#16992 - Send Potential message when Second Effort Solicitation displayed (Payments Screen).
        public void SendWarrantyPotential(SqlConnection conn, SqlTransaction trans, string acctNo, int agreementNo, int itemId, int stockLocn)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {

                var salesOrderView = (from w in ctx.WarrantySaleGenerateView
                                      where w.CustomerAccount == acctNo &&
                                      w.AgreementNumber == agreementNo &&
                                      w.ItemId == itemId &&
                                      w.StockLocation == stockLocn
                                      select w).ToList();

                var dateDelivered = (from d in ctx.Delivery
                                     where d.acctno == acctNo &&
                                     d.agrmtno == agreementNo &&
                                     d.ItemID == itemId &&
                                     d.stocklocn == stockLocn
                                     select d.datedel).FirstOrDefault();

                List<Message.PotentialSales> potentialSale = new List<Message.PotentialSales>();

                var items = new List<Message.Item>();

                foreach (var s in salesOrderView)
                {
                    items.Clear();
                    items.Add(new Message.Item
                    {
                        Brand = s.itemBrand,
                        CostPrice = s.ItemCostPrice.HasValue ? s.ItemCostPrice.Value : 0,
                        Description = s.Description,
                        Id = s.ItemId,
                        Number = s.ItemNumber,
                        Price = s.ItemPrice,
                        StockLocation = s.StockLocation,
                        Quantity = Convert.ToInt32(s.Quantity),
                        Supplier = s.itemSupplier,
                        UPC = s.ItemUPC,
                        Warranty = null,
                        Department = s.Department
                    });

                    var message = new Message.PotentialSales
                    {
                        Customer = new Message.Customer
                        {
                            AccountNumber = s.CustomerAccount,
                            AddressLine1 = s.CustomerAddressLine1,
                            AddressLine2 = s.CustomerAddressLine2,
                            AddressLine3 = s.CustomerAddressLine3,
                            CustomerId = s.customerId,
                            FirstName = s.CustomerFirstName,
                            LastName = s.CustomerLastName,
                            Notes = s.CustomerNotes,
                            PostCode = s.CustomerPostCode,
                            Title = s.customerTitle,
                            HomePhone = s.homePhone,
                            WorkPhone = s.workPhone,
                            MobilePhone = s.mobilePhone,
                            Email = s.Email,
                            AccountType = s.AccountType
                        },
                        DeliveredOn = dateDelivered, // is this supposed to be UTC code unreachable.
                        InvoiceNumber = s.invoiceNumber,
                        AccountNumber = s.CustomerAccount,
                        Items = items.ToArray(),

                        SaleBranch = s.SaleBranch,
                        SoldBy = s.SoldById,
                        SoldOn = s.SoldOn.HasValue ? s.SoldOn.Value.Date : DateTime.Now.Date,
                        SecondEffort = true                                                             //#17609
                    };

                    potentialSale.Add(message);
                };

                var chub = new Chub();
                potentialSale.ForEach(s =>
                {
                    chub.Submit(s, connection, transaction);
                });

            }, conn: conn, trans: trans);
        }

    }
}

