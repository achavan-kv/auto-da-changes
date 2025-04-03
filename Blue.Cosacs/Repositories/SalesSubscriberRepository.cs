using Blue.Cosacs.Messages;
using Blue.Cosacs.Shared;
using STL.Common;
using STL.Common.Constants.AuditSource;
using STL.Common.Constants.Delivery;
using STL.Common.Constants.FTransaction;
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Blue.Cosacs.Repositories
{
    public class SalesSubscriberRepository : CommonObject
    {
        private enum ItemOp
        {
            New,
            Return,
            Exchange
        }

        private const string FtNotesWarrantyReturn = "DNWA";
        private const string FtNotesWarrantyPurchase = "DNWG";
        private const string FtNotes = "DNCG";
        private const string FtNotesPayment = "dnet";
        private const string TaxItemNo = "STAX";
        private readonly bool isTaxExclusive;
        private readonly bool isAgreementTaxExclusive;
        private string accountNo;
        private int taxItemId;
        private Dictionary<short, short> paymentLookUp;

        private Order order;
        private int userId;

        private List<Item> items = new List<Item>();
        private List<Item> warranties = new List<Item>();
        private List<Item> nonWarrantiesItems = new List<Item>();
        private List<Item> stockItems = new List<Item>();
        private List<Item> newItems = new List<Item>();
        private List<Item> returnedItems = new List<Item>();
        private List<Payment> payments = new List<Payment>();
        private bool isReturnOrder;

        public SalesSubscriberRepository()
        {
            isTaxExclusive = (string)Country[CountryParameterNames.TaxType] == "E";
            isAgreementTaxExclusive = (string)Country[CountryParameterNames.AgreementTaxType] == "E";
        }

        #region Web Methods

        public void SaveSalesOrders(Order pOrder)
        {
            Setup(pOrder);

            ManageNewWarranty();

            using (var connection = new SqlConnection(Connections.Default))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    using (var ctx = Context.Create(connection, transaction))
                    {
                        var branchDetails = ctx.Branch.Single(b => b.branchno == order.BranchNo);
                        var countryCode = ctx.Country.FirstOrDefault().countrycode;

                        paymentLookUp = ctx.PaymentMethodLookUp
                            .Where(x => x.CountryCode == countryCode)
                            .ToDictionary(x => x.PosPayMethodId, x => x.WinCosacsPayMethodId);

                        taxItemId = ctx.StockInfo.FirstOrDefault(i => TaxItemNo.Equals(i.itemno)).Id;

                        if (!isReturnOrder)
                        {
                            ProcessItems(ctx, connection, transaction, branchDetails);
                        }
                        else
                        {
                            branchDetails = ctx.Branch.Single(b => b.branchno == order.OriginalBranchNo);

                            ProcessReturnItems(ctx, connection, transaction, branchDetails);
                        }

                        UpdateStockQty(ctx, order.BranchNo);

                        ctx.SubmitChanges();

                        PostWarrantyHubMessages(connection, transaction);
                        ctx.SubmitChanges();

                        transaction.Commit();
                    }
                }
            }
        }

        #endregion

        #region Utility Functions

        private void Setup(Order pOrder)
        {
            order = pOrder;

            items = new List<Item>(order.Items);

            var totalComponentPrice = 0M;

            using (var ctx = Context.Create())
            {
                var itemNos = items.Select(i => i.ItemNo).Distinct().ToList();
                var stoItems = ctx.StockInfo.Where(s => itemNos.Contains(s.SKU)).Distinct()
                    .GroupBy(s => s.SKU, (key, xs) => xs.OrderByDescending(s => s.Id).First()).ToList(); //ctx.StockInfo.Where(s => itemNos.Contains(s.SKU)).Distinct().ToList();


                // TODO: this need to hapen when indexing and not here
                foreach (var item in items)
                {
                    var product = stoItems.FirstOrDefault(s => s.SKU == item.ItemNo);
                    var productTaxRate = (product == null ? 0M : (decimal)product.taxrate);

                    item.ProductItemId = product == null ? item.ProductItemId : product.Id;
                    item.TaxRate = item.TaxRate == 0 ? productTaxRate : item.TaxRate;

                    var quantity = item.Quantity > 0 ? item.Quantity : 1;
                    var addedTax = isAgreementTaxExclusive ? 0 : item.TaxAmount;

                    item.TaxAmount = CountryRound(item.TaxAmount);
                    item.Price = CountryRound(item.Price + addedTax);

                    if (string.IsNullOrEmpty(item.ParentItemNo) || item.ParentItemNo == "0") continue;

                    var parentProduct = stoItems.FirstOrDefault(s => s.SKU == item.ParentItemNo);
                    item.ParentId = parentProduct == null ? item.ProductItemId : parentProduct.Id;

                    if (item.ItemTypeId != (int)ItemTypeEnum.Discount)
                    {
                        totalComponentPrice += item.Price;
                    }

                    if (item.ManualDiscountSpecified && item.ItemTypeId == (int)ItemTypeEnum.Discount)
                    {
                        if (!isAgreementTaxExclusive)
                        {
                            item.ManualDiscount = item.ManualDiscount + item.TaxAmount;// * item.Quantity);
                        }
                        else
                        {
                            item.ManualDiscount = item.ManualDiscount;// * item.Quantity);
                        }

                        //item.Quantity = 1;
                    }

                    if (item.ItemTypeId == (int)ItemTypeEnum.Warranty &&
                        (item.Department.ToUpper() != "PCE" || item.Department.ToUpper() != "PCF"))
                    {
                        item.Department = (item.Department.ToUpper() == "FURNITURE") ? "PCF" : "PCE";
                        item.Category = (short)((item.Department.ToUpper() == "PCF") ? 82 : 12);
                    }

                }
            }

            // Set Kit's Prices
            var kit = items.FirstOrDefault(i => i.ItemTypeId == (int)ItemTypeEnum.Kit);

            if (kit != null)
            {
                kit.Price = CountryRound(totalComponentPrice);
            }

            warranties = items.Where(x => x.ItemTypeId == (int)ItemTypeEnum.Warranty).OrderBy(x => x.WarrantyContractNo).ToList();
            nonWarrantiesItems = items.Where(x => x.ItemTypeId != (int)ItemTypeEnum.Warranty).ToList();
            stockItems = items
                .Where(x => (x.ItemTypeId == (int)ItemTypeEnum.Product || x.ItemTypeId == (int)ItemTypeEnum.Kit)).ToList();
            newItems = items.Where(i => !i.Returned).ToList();
            returnedItems = items.Where(i => i.Returned).ToList();

            payments = new List<Payment>(order.Payments);

            isReturnOrder = order.OriginalOrderIdSpecified;
            accountNo = order.CashAndGoAccountNo;
            userId = order.CreatedBy;
        }

        private void ProcessItems(Context ctx, SqlConnection connection, SqlTransaction transaction, Branch branchDetails)
        {
            var totalTaxCalc = GetTotalTax(newItems);

            //For Agreement Table
            var defaultAgreement = GetDefaultAgreement(order.TotalAmount, order.TotalDiscount);
            ctx.Agreement.InsertOnSubmit(defaultAgreement);

            //For LineItem
            PopulateLineTable(ctx, connection, transaction, totalTaxCalc);

            //Facttrans insert for cintorder
            PopulateFactTransTable(ctx);

            //For Delivery Table
            PopulateDeliveryTable(ctx, connection, transaction, branchDetails, false, totalTaxCalc);

            //For FinTrans Table
            PopulateFinTrans(ctx, connection, transaction, branchDetails, false);

        }

        private void ProcessReturnItems(Context ctx, SqlConnection connection, SqlTransaction transaction, Branch branchDetails)
        {
            var returnTotalPrice = GetTotalPrice(returnedItems);
            var returnTotalTax = GetTotalTax(returnedItems);
            var returnTotalDiscount = GetTotalDiscount(returnedItems);
            var returnTotalAmount = CountryRound(returnTotalPrice + returnTotalTax + returnTotalDiscount);

            //agreement table
            var originalAgreementData = ctx.Agreement.Single(a => a.agrmtno == order.OriginalOrderId);

            originalAgreementData.cashprice = CountryRound(originalAgreementData.cashprice + returnTotalPrice);
            originalAgreementData.discount = CountryRound(originalAgreementData.discount + returnTotalDiscount);
            originalAgreementData.agrmttotal = CountryRound(originalAgreementData.agrmttotal + returnTotalPrice);

            //update line item
            foreach (var r in returnedItems)
            {
                LineItem lineItem;
                switch (r.ItemTypeId)
                {
                    case (int)ItemTypeEnum.Product:
                    case (int)ItemTypeEnum.Kit:
                        lineItem = ctx.LineItem.Single(l => l.agrmtno == order.OriginalOrderId && l.itemno.Equals(r.ItemNo));
                        break;
                    case (int)ItemTypeEnum.Warranty:
                        lineItem = ctx.LineItem.Single(l => l.agrmtno == order.OriginalOrderId && l.contractno == r.WarrantyContractNo);
                        break;
                    default:
                        lineItem = ctx.LineItem.Single(l => l.agrmtno == order.OriginalOrderId && l.itemno.Equals(r.ItemNo) &&
                            l.parentitemno != null && r.ParentItemNo != null && l.parentitemno.Equals(r.ParentItemNo));
                        break;
                }

                var isDiscount = r.ItemTypeId == (int)ItemTypeEnum.Discount;

                if (isDiscount)
                {
                    lineItem.quantity = lineItem.quantity - r.ReturnQuantity;
                    lineItem.ordval = CountryRound(r.ManualDiscount * (decimal)lineItem.quantity * -1);
                }
                else
                {
                    lineItem.quantity = lineItem.quantity - r.ReturnQuantity;
                    lineItem.ordval = lineItem.price * Convert.ToDecimal(lineItem.quantity);
                }

                lineItem.taxamt = (double)(r.TaxAmount * -1);

                PopulateLineitemAuditTable(ctx, connection, transaction, lineItem, ItemOp.Return, isDiscount, r.ReturnQuantity);

                lineItem.taxamt = (double)(r.TaxAmount * (decimal)lineItem.quantity * -1);
            }

            if (isAgreementTaxExclusive)
            {
                var defaultLineItemTax = ctx.LineItem.SingleOrDefault(l => l.agrmtno == order.OriginalOrderId && l.itemno.Equals(TaxItemNo));

                if (defaultLineItemTax != null)
                {
                    defaultLineItemTax.quantity = 0;
                    defaultLineItemTax.ordval = returnTotalTax;
                }
            }

            //For Delivary Table
            PopulateDeliveryTable(ctx, connection, transaction, branchDetails, true, returnTotalTax, true);

            if (newItems.Any())
            {
                var newTotalPrice = GetTotalPrice(newItems);
                var newTotalTax = GetTotalTax(newItems);
                var newTotalDiscount = GetTotalDiscount(newItems);
                var newTotalAmount = newTotalPrice + newTotalDiscount;

                //Handle payments for original order
                var defaultCashPayment = new Payment();

                //new Sale

                //For Agreement Table
                ctx.Agreement.InsertOnSubmit(GetDefaultAgreement(newTotalAmount, newTotalDiscount));

                //For LineItem
                PopulateLineTable(ctx, connection, transaction, newTotalTax);

                if (order.TotalAmount < 0)
                {
                    defaultCashPayment.Amount = CountryRound(returnTotalAmount - order.TotalAmount);
                    defaultCashPayment.MethodId = 1;
                    DN_FinTransWriteSP finTransEntry = FinTransWriteSproc(connection, transaction, order, defaultCashPayment,
                        ++branchDetails.hirefno, false, true, null);
                    finTransEntry.ExecuteNonQuery();

                    defaultCashPayment.Amount = CountryRound(newTotalAmount);
                    defaultCashPayment.MethodId = 1;
                    finTransEntry = FinTransWriteSproc(connection, transaction, order, defaultCashPayment, ++branchDetails.hirefno,
                        false, false, null);
                    finTransEntry.ExecuteNonQuery();

                    //now remaining return
                    PopulateFinTrans(ctx, connection, transaction, branchDetails, true);
                }
                else
                {
                    defaultCashPayment.Amount = CountryRound(returnTotalPrice);
                    defaultCashPayment.MethodId = 1;
                    DN_FinTransWriteSP finTransEntry = FinTransWriteSproc(connection, transaction, order, defaultCashPayment,
                        ++branchDetails.hirefno, false, true, null);
                    finTransEntry.ExecuteNonQuery();

                    defaultCashPayment.Amount = CountryRound(newTotalAmount - order.TotalAmount);
                    defaultCashPayment.MethodId = 1;
                    finTransEntry = FinTransWriteSproc(connection, transaction, order, defaultCashPayment, ++branchDetails.hirefno,
                        false, false, null);
                    finTransEntry.ExecuteNonQuery();

                    PopulateFinTrans(ctx, connection, transaction, branchDetails, false);
                }

                //Facttrans insert for cintorder
                PopulateFactTransTable(ctx);

                //For Delivery Table
                PopulateDeliveryTable(ctx, connection, transaction, branchDetails, false, CountryRound(newTotalTax));
            }
            else
            {
                PopulateFinTrans(ctx, connection, transaction, branchDetails, true);
            }
        }

        private decimal GetTotalDiscount(IEnumerable<Item> itemsToConsider)
        {
            return CountryRound(itemsToConsider.Sum(r => r.ManualDiscount));
        }

        private decimal GetTotalTax(IEnumerable<Item> itemsToConsider)
        {
            var total = itemsToConsider.ToList()
                .Aggregate<Item, decimal>(0, (current, i) => current + (i.TaxAmount * i.Quantity));

            return CountryRound(total);
        }

        private decimal GetTotalPrice(IEnumerable<Item> itemsToConsider)
        {
            var total = itemsToConsider.ToList()
                .Aggregate<Item, decimal>(0, (current, i) => current + (i.Price * i.Quantity));

            return CountryRound(total);
        }

        #endregion

        #region In GiftVoucherRedeemed Table

        private GiftVoucherRedeemed AddInGiftVoucherRedeemedTable(string reference, string acctNo, int transRefNo)
        {
            return new GiftVoucherRedeemed
            {
                reference = reference,
                acctnoredeemed = acctNo,
                transrefnoredeemed = transRefNo
            };
        }

        #endregion

        #region In FinXfr Table

        private Finxfr TransfrStoreCardPayment(Order order, Payment payment, int transRefNo, string storeCardAcctNo, string storeCardAcctName,
            bool transfer, bool isReturn)
        {
            return new Finxfr
            {
                origbr = 0,
                acctno = transfer ? storeCardAcctNo : accountNo,
                transrefno = transRefNo,
                datetrans = DateTime.Now,
                acctnoxfr = transfer ? accountNo : storeCardAcctNo,
                acctname = storeCardAcctName,
                agrmtno = isReturn ? order.OriginalOrderId : order.Id,
                storecardno = payment.StoreCardNo,
                OrigTransRefNo = 0
            };
        }

        #endregion

        #region In StockInfo, StockPrice, StockQuantity Table

        private StockInfo AddInStockInfoTable(Item item, int id)
        {
            // This for v9.0 and it shouldn't go beyond that
            var cat = (item.Department.ToLower() == "furniture") ? 82 : 12;

            return new StockInfo
            {
                itemno = item.ItemNo,
                itemdescr1 = item.Description,
                itemdescr2 = string.Format("Warranty Length - {0}", item.WarrantyLengthMonths),
                category = (short?)cat,  //As mentioned
                itemtype = "N",
                warrantyrenewalflag = "N",
                taxrate = Convert.ToDouble(item.TaxRate),
                IUPC = item.ItemNo,
                WarrantyLength = item.WarrantyLengthMonths,
                Id = id,
                WarrantyType = item.WarrantyTypeCode,
                SKU = item.ItemNo
            };
        }

        private StockQuantity AddInStockQuantityTable(Item warranty, int nextId, Order order)
        {
            return new StockQuantity
            {
                itemno = warranty.ItemNo,
                stocklocn = order.BranchNo,
                qtyAvailable = 0,
                stock = 0,
                stockonorder = 0,
                stockdamage = 0,
                leadtime = 0,
                dateupdated = order.CreatedOn,
                deleted = "N",
                LastOperationSource = string.Empty,
                ID = nextId
            };
        }

        private StockPrice AddInStockPriceTable(Item warranty, int nextId, short branchNumber)
        {
            return new StockPrice
            {
                itemno = warranty.ItemNo,
                branchno = branchNumber,
                CreditPrice = warranty.Price,
                CashPrice = warranty.Price,
                //DutyFreePrice = 
                CostPrice = warranty.CostPrice,
                //Refcode =
                //DateActivated = 
                ID = nextId
            };
        }

        #endregion

        #region Agreement Default

        private Agreement GetDefaultAgreement(decimal totalAmount, decimal totalDiscount)
        {
            var agreement = new Agreement
            {
                origbr = 0,
                acctno = accountNo,
                agrmtno = order.Id,
                dateagrmt = order.CreatedOn,
                empeenosale = order.SoldBy,
                datedepchqclr = order.CreatedOn,
                holdmerch = "Y",
                holdprop = "Y",
                datedel = order.CreatedOn,
                datenextdue = order.CreatedOn,
                oldagrmtbal = 0,
                cashprice = CountryRound(totalAmount),
                discount = CountryRound(totalDiscount),
                pxallowed = 0,
                servicechg = 0,
                sdrychgtot = 0,
                agrmttotal = CountryRound(totalAmount),
                deposit = 0,
                codflag = "N",
                soa = null,
                paymethod = null,
                unpaidflag = null,
                deliveryflag = null,
                fulldelflag = null,
                PaymentMethod = string.Empty,
                empeenoauth = null,
                dateauth = null,
                empeenochange = order.CreatedBy,
                datechange = null,
                AdminFee = 0,
                InsCharge = 0,
                datefullydelivered = null,
                createdby = order.CreatedBy,
                paymentcardline = 0,
                paymentholidays = 0,
                AgreementPrinted = "Y",
                TaxInvoicePrinted = "Y",
                WarrantyPrinted = "Y",
                source = "Cosacs",
                TaxFree = order.IsTaxFreeSale == true ? true : false
            };

            return agreement;
        }

        #endregion

        #region Line item Audit

        private void PopulateLineitemAuditTable(Context ctx, SqlConnection connection, SqlTransaction transaction, LineItem item,
            ItemOp itemOp, bool isDiscount = false, short retQuantity = 0)
        {
            var l = new DLineItem();
            var source = AS.NewAccount;
            var quantityBefore = 0d;
            var quantityAfter = 0d;
            var valueBefore = 0m;
            var valueAfter = 0m;
            var taxamtBefore = 0d;
            var taxamtAfter = 0d;
            //item.quantity = item.quantity > 0 ? item.quantity : 1;

            switch (itemOp)
            {
                case ItemOp.Return:
                case ItemOp.Exchange:
                    source = itemOp == ItemOp.Return ? AS.GRTCancel : AS.GRTExchange;
                    quantityBefore = item.quantity + retQuantity;
                    quantityAfter = item.quantity;
                    quantityBefore = quantityBefore > 0 ? quantityBefore : 1;
                    valueAfter = CountryRound(Convert.ToDecimal(quantityAfter) * item.price);
                    valueBefore = CountryRound(Convert.ToDecimal(quantityBefore) * item.price);
                    taxamtAfter = (double)CountryRound(item.taxamt * quantityAfter);// (double)((item.taxamt / quantityBefore) * quantityAfter);
                    taxamtBefore = (double)(item.taxamt * quantityBefore);

                    break;
                default:
                    quantityAfter = item.quantity;
                    valueAfter = isDiscount == true ? CountryRound(Convert.ToDecimal(item.quantity) * item.price) :
                        Convert.ToDecimal(item.quantity) * item.price;
                    taxamtAfter = item.taxamt;// * item.quantity;
                    break;
            }

            l.AccountNumber = item.acctno;
            l.AgreementNumber = item.agrmtno;
            l.User = userId;
            l.ItemNumber = item.itemno;
            l.ItemID = item.ItemID;
            l.StockLocation = item.stocklocn;
            l.ContractNo = item.contractno;
            l.AuditSource = source;
            l.ParentItemNumber = item.parentitemno;
            l.ParentStockLocation = item.parentlocation;
            l.ParentItemID = item.ParentItemID;

            l.UpdateLineItemAudit(connection, transaction, quantityBefore, quantityAfter, valueBefore, valueAfter, taxamtBefore, taxamtAfter);
        }

        #endregion

        #region LineItem Table

        private void PopulateLineTable(Context ctx, SqlConnection connection, SqlTransaction transaction, decimal totalTaxAmount)
        {
            foreach (var item in nonWarrantiesItems)
            {
                if (item.Returned) continue;

                var parentItem = items.FirstOrDefault(x => (x.ItemTypeId == (int)ItemTypeEnum.Product || x.ItemTypeId == (int)ItemTypeEnum.Kit) &&
                    x.ProductItemId == item.ParentId);
                var defaultLineItem = GetDefaultLineItem(order, item, parentItem, null);

                ctx.LineItem.InsertOnSubmit(defaultLineItem);

                var isDiscount = item.ItemTypeId == (int)ItemTypeEnum.Discount;

                defaultLineItem.price = isDiscount ? item.ManualDiscount : item.Price;

                PopulateLineitemAuditTable(ctx, connection, transaction, defaultLineItem, ItemOp.New, isDiscount);

                var groupId = new List<WarrantyGroup>();
                var itemWarranties = warranties.Where(w => w.ParentId == item.ProductItemId).ToList();

                foreach (var warranty in itemWarranties)
                {
                    if (warranty.Returned)
                    {
                        continue;
                    }
                    //var parentItem = items
                    //    .FirstOrDefault(x => x.ItemTypeId == (int)ItemTypeEnum.Product && x.ProductItemId == warranty.ParentId);
                    int? warrantyGroupId = 1;

                    if (warranty.Returned)
                    {
                        warrantyGroupId = (from c in ctx.LineItem
                                           where c.contractno == warranty.WarrantyContractNo && c.itemno.Equals(warranty.ItemNo) && c.stocklocn == order.BranchNo
                                           select c.WarrantyGroupId).FirstOrDefault();
                    }
                    else
                    {
                        var currentWarrantyGroup = groupId.FirstOrDefault(x => x.ParentId == warranty.ParentId &&
                            warranty.WarrantyTypeCode.Equals(x.WarrantyType));

                        if (currentWarrantyGroup == null)
                        {
                            groupId.Add(new WarrantyGroup
                            {
                                ParentId = item.ProductItemId,
                                WarrantyType = warranty.WarrantyTypeCode,
                                Count = 1
                            });
                        }
                        else
                        {
                            currentWarrantyGroup.Count++;
                        }

                        warrantyGroupId = groupId.Single(x => x.ParentId == warranty.ParentId &&
                            warranty.WarrantyTypeCode.Equals(x.WarrantyType)).Count;
                    }

                    var defaultWarLineItem = GetDefaultLineItem(order, warranty, item, warrantyGroupId);

                    ctx.LineItem.InsertOnSubmit(defaultWarLineItem);

                    PopulateLineitemAuditTable(ctx, connection, transaction, defaultWarLineItem, ItemOp.New);
                }
            }

            if (!isAgreementTaxExclusive) return;

            var defaultLineItemTax = GetDefaultLineItemTax(order, totalTaxAmount);

            ctx.LineItem.InsertOnSubmit(defaultLineItemTax);
        }

        private LineItem GetDefaultLineItem(Order order, Item item, Item parentItem, int? warrantyGroupId)
        {
            var totalTaxAmount = item.TaxAmount * item.Quantity;

            var ret = new LineItem
            {
                origbr = 0,
                acctno = accountNo,
                agrmtno = order.Id,
                itemno = item.ItemNo,
                itemsupptext = string.Empty,
                quantity = item.Returned ? 0 : item.Quantity,
                delqty = item.Quantity,
                stocklocn = order.BranchNo,
                price = CountryRound(item.ItemTypeId == (int)ItemTypeEnum.Discount ? item.ManualDiscount : item.Price),
                ordval = item.ItemTypeId == (int)ItemTypeEnum.Discount ?
                    CountryRound(item.ManualDiscount * item.Quantity) : (item.Returned ? 0 : (CountryRound(item.Price) * item.Quantity)),
                datereqdel = order.CreatedOn,
                //timereqdel = 
                dateplandel = order.CreatedOn,
                delnotebranch = order.BranchNo,
                qtydiff = string.Empty,
                itemtype = item.ItemTypeId == (int)ItemTypeEnum.Product ? "S" : "N",
                notes = string.Empty,
                taxamt = Convert.ToDouble(totalTaxAmount),
                isKit = item.ItemTypeId == (int)ItemTypeEnum.Kit ? Convert.ToInt16(1) : Convert.ToInt16(0),
                deliveryaddress = "H",
                parentitemno = parentItem != null ? parentItem.ItemNo : string.Empty,
                parentlocation = parentItem != null ? order.BranchNo : Convert.ToInt16(0),
                contractno = item.WarrantyContractNo ?? string.Empty,
                expectedreturndate = null,
                deliveryprocess = item.ItemTypeId == (int)ItemTypeEnum.Product ? "S" : string.Empty,
                deliveryarea = string.Empty,
                DeliveryPrinted = "Y",
                assemblyrequired = "N",
                damaged = "N",
                OrderNo = null,
                Orderlineno = null,
                PrintOrder = null,
                taxrate = Convert.ToDouble(CountryRound(item.TaxRate)),
                ItemID = item.ProductItemId,
                ParentItemID = parentItem != null ? parentItem.ProductItemId : 0,
                SalesBrnNo = order.BranchNo,
                //Express
                WarrantyGroupId = warrantyGroupId
            };

            return ret;
        }

        private LineItem GetDefaultLineItemTax(Order order, decimal totalTaxAmount)
        {
            var isManualReturn = !order.OriginalOrderIdSpecified && (order.Items.Any(x => x.Returned) && !order.Items.Any(x => !x.Returned));

            return new LineItem
            {
                origbr = 0,
                acctno = accountNo,
                agrmtno = order.Id,
                itemno = TaxItemNo,
                itemsupptext = string.Empty,
                quantity = isManualReturn ? 0 : 1,
                delqty = isManualReturn ? 0 : 1,
                stocklocn = order.BranchNo,
                price = CountryRound(totalTaxAmount),
                ordval = CountryRound(isManualReturn ? 0 : totalTaxAmount),
                datereqdel = order.CreatedOn,
                //timereqdel = 
                dateplandel = order.CreatedOn,
                delnotebranch = order.BranchNo,
                qtydiff = string.Empty,
                itemtype = "N",
                notes = string.Empty,
                taxamt = Convert.ToDouble(0),
                isKit = Convert.ToInt16(0),
                deliveryaddress = "H",
                parentitemno = string.Empty,
                parentlocation = Convert.ToInt16(0),
                contractno = string.Empty,
                expectedreturndate = null,
                deliveryprocess = string.Empty,
                deliveryarea = string.Empty,
                DeliveryPrinted = "Y",
                assemblyrequired = string.Empty,
                damaged = string.Empty,
                OrderNo = null,
                Orderlineno = null,
                PrintOrder = null,
                taxrate = Convert.ToDouble(0),
                ItemID = taxItemId,
                ParentItemID = 0,
                SalesBrnNo = null
                //Express
                //WarrantyGroupId
            };
        }

        #endregion

        #region Facttrans Table

        private void PopulateFactTransTable(Context ctx)
        {
            foreach (var item in stockItems.Where(s => s.Returned == false))
            {
                if (item.ItemTypeId != (int)ItemTypeEnum.Kit)
                {
                    ctx.FactTrans.InsertOnSubmit(AddItemsToFactTransTable(order, item));
                }
            }
        }

        private FactTrans AddItemsToFactTransTable(Messages.Order order, Messages.Item item)
        {
            var stocklocn = item.Returned ? order.OriginalBranchNo : order.BranchNo;
            var ret = new FactTrans
            {
                origbr = 0,
                acctno = accountNo,
                agrmtno = order.Id,
                itemno = item.ItemNo,
                quantity = item.Quantity,
                stocklocn = stocklocn,
                buffno = order.Id,
                ItemID = item.ProductItemId,
                price = Convert.ToDouble(CountryRound(item.Price)),
                trandate = order.CreatedOn,
                value = Convert.ToDouble(CountryRound(item.Price * item.Quantity)),
                trantype = "01",
                taxamt = Convert.ToDouble(CountryRound(item.TaxAmount)),
                tccode = "61"
            };

            return ret;
        }

        #endregion

        #region Delivery Table

        private void PopulateDeliveryTable(Context ctx, SqlConnection connection, SqlTransaction transaction, Branch branchDetails,
            bool isOriginal, decimal totalTaxAmount, bool prossesWarranties = false)
        {
            if (nonWarrantiesItems.Any())
            {
                branchDetails.hibuffno++;
                branchDetails.hirefno++;
            }

            var items = nonWarrantiesItems.Where(i => i.Returned == isOriginal);

            foreach (var item in items)
            {
                if (item.ItemTypeId == (int)ItemTypeEnum.Kit) continue;

                var parentItem = items.FirstOrDefault(x => (x.ItemTypeId == (int)ItemTypeEnum.Product || x.ItemTypeId == (int)ItemTypeEnum.Kit) &&
                    x.ProductItemId == item.ParentId);
                var defaultDelivery = GetDefaultDelivery(order, item, parentItem, branchDetails.hibuffno, branchDetails.hirefno, isOriginal);

                ctx.Delivery.InsertOnSubmit(defaultDelivery);
            }

            if (isAgreementTaxExclusive)
            {
                var defaultDeliveryTax = GetDefaultDeliveryTax(order, branchDetails.hibuffno, branchDetails.hirefno, isOriginal, totalTaxAmount);

                ctx.Delivery.InsertOnSubmit(defaultDeliveryTax);
            }

            foreach (var warranty in warranties)
            {
                if (warranty.Returned && isOriginal)
                {
                    var parentItem = items
                        .FirstOrDefault(x => (x.ItemTypeId == (int)ItemTypeEnum.Product || x.ItemTypeId == (int)ItemTypeEnum.Kit) && x.ProductItemId == warranty.ParentId);

                    ctx.Delivery.InsertOnSubmit(GetDefaultDeliveryWarranty(order, warranty, parentItem, ++branchDetails.hibuffno,
                        ++branchDetails.hirefno, warranty.Returned));
                }

                if (warranty.PriceDifferenceSpecified)
                {
                    DN_FinTransWriteSP warrantyPriceChange = AddPaymentsInFinTransTable(connection, transaction, order, warranty.Department,
                        warranty.PriceDifference, ++branchDetails.hirefno, warranty.Returned);

                    warrantyPriceChange.ExecuteNonQuery();
                }
            }
        }

        private Delivery GetDefaultDelivery(Order order, Item item, Item parentItem, int hibuffno, int hirefno, bool isOriginal)
        {
            var stocklocn = item.Returned ? order.OriginalBranchNo : order.BranchNo;
            var transvalue = item.ItemTypeId == (int)ItemTypeEnum.Discount ?
                CountryRound(item.ManualDiscount * item.Quantity) : item.Price * item.Quantity;

            var retval = item.ItemTypeId == (int)ItemTypeEnum.Discount ?
                CountryRound(item.ManualDiscount * item.ReturnQuantity) : item.Price * item.ReturnQuantity;

            var ret = new Delivery
            {
                origbr = 0,
                acctno = accountNo,
                agrmtno = isOriginal ? order.OriginalOrderId : order.Id,
                datedel = order.CreatedOn,
                delorcoll = item.Returned ? DelType.Collection : DelType.Normal,
                itemno = item.ItemNo,
                stocklocn = stocklocn,
                quantity = item.Returned ? item.ReturnQuantity * -1 : item.Quantity,
                retitemno = item.Returned ? item.ItemNo : string.Empty,
                retstocklocn = item.Returned ? order.BranchNo : Convert.ToInt16(0),
                retval = (double)(item.Returned ? retval : 0),
                buffno = item.Returned ? hibuffno : order.Id,
                buffbranchno = stocklocn, // TODO: check this
                datetrans = DateTime.Now,
                branchno = stocklocn,
                transrefno = hirefno,
                transvalue = transvalue,
                runno = 0,
                contractno = string.Empty,
                ReplacementMarker = null,
                NotifiedBy = order.CreatedBy,
                ftnotes = FtNotes,
                InvoiceLineNo = null,
                ExtInvoice = null,
                ParentItemNo = parentItem != null ? parentItem.ItemNo : string.Empty,
                ItemID = item.ProductItemId,
                ParentItemID = parentItem != null ? parentItem.ProductItemId : 0,
                RetItemID = item.Returned ? item.ProductItemId : 0,
                BrokerExRunNo = 0
            };

            return ret;
        }

        private Delivery GetDefaultDeliveryTax(Order order, int hibuffno, int hirefno, bool isOriginal, decimal totalTaxAmount)
        {
            var isManualReturn = !order.OriginalOrderIdSpecified && (items.Any(x => x.Returned) && items.All(x => x.Returned));
            var stocklocn = isOriginal ? order.OriginalBranchNo : order.BranchNo;

            return new Delivery
            {
                origbr = 0,
                acctno = accountNo,
                agrmtno = isOriginal ? order.OriginalOrderId : order.Id,
                datedel = order.CreatedOn,
                delorcoll = isOriginal ? DelType.Collection : (isManualReturn ? DelType.Collection : DelType.Normal),
                itemno = TaxItemNo,
                stocklocn = stocklocn,
                quantity = isOriginal ? -1 : (isManualReturn ? -1 : 1),
                retitemno = isOriginal ? TaxItemNo : (isManualReturn ? TaxItemNo : string.Empty),
                retstocklocn = isOriginal ? order.BranchNo : (isManualReturn ? order.BranchNo : Convert.ToInt16(0)),
                retval = isOriginal ? Convert.ToDouble(totalTaxAmount) : (isManualReturn ? Convert.ToDouble(totalTaxAmount) : 0),
                buffno = isOriginal ? hibuffno : order.Id,
                buffbranchno = stocklocn,
                datetrans = DateTime.Now,
                branchno = stocklocn,
                transrefno = hirefno,
                transvalue = CountryRound(totalTaxAmount),
                runno = 0,
                contractno = string.Empty,
                ReplacementMarker = null,
                NotifiedBy = order.CreatedBy,
                ftnotes = FtNotes,
                InvoiceLineNo = null,
                ExtInvoice = null,
                ParentItemNo = string.Empty,
                ItemID = taxItemId,
                ParentItemID = 0,
                RetItemID = isOriginal ? taxItemId : 0,
                BrokerExRunNo = 0
            };
        }

        private Delivery GetDefaultDeliveryWarranty(Order order, Item warranty, Item parentItem, int hibuffno, int hirefno, bool isOriginal)
        {
            var stocklocn = warranty.Returned ? order.OriginalBranchNo : order.BranchNo;

            var ret = new Delivery
            {
                origbr = 0,
                acctno = accountNo,
                agrmtno = isOriginal ? order.OriginalOrderId : order.Id,
                datedel = order.CreatedOn,
                delorcoll = warranty.Returned ? DelType.Collection : DelType.Normal,
                itemno = warranty.ItemNo,
                stocklocn = stocklocn,
                quantity = warranty.Returned ? warranty.Quantity * -1 : warranty.Quantity,
                retitemno = warranty.Returned ? warranty.ItemNo : string.Empty,
                retstocklocn = warranty.Returned ? order.BranchNo : Convert.ToInt16(0),
                retval = warranty.Returned ? Convert.ToDouble(warranty.SalePrice) : 0,
                buffno = hibuffno,
                buffbranchno = stocklocn,
                datetrans = DateTime.Now,
                branchno = stocklocn,
                transrefno = hirefno,
                transvalue = CountryRound(warranty.Price),
                runno = 0,
                contractno = warranty.WarrantyContractNo,
                ReplacementMarker = null,
                NotifiedBy = order.CreatedBy,
                ftnotes = warranty.Returned ? FtNotesWarrantyReturn : FtNotesWarrantyPurchase,
                InvoiceLineNo = null,
                ExtInvoice = null,
                ParentItemNo = parentItem != null ? parentItem.ItemNo : string.Empty,
                ItemID = warranty.ProductItemId,
                ParentItemID = parentItem != null ? parentItem.ProductItemId : 0,
                RetItemID = warranty.Returned ? warranty.ProductItemId : 0,
                BrokerExRunNo = 0
            };

            return ret;
        }

        #endregion

        #region In FinTrans Table

        private void PopulateFinTrans(Context ctx, SqlConnection connection, SqlTransaction transaction, Branch branchDetails, bool isReturn)
        {
            foreach (var payment in payments)
            {
                if (payment.MethodId == (int)PaymentTypeEnum.StoreCard)
                {
                    var storeCardDetails = (from s in ctx.StoreCard
                                            join c in ctx.Customer on s.CustID equals c.custid
                                            where s.CardNumber == payment.StoreCardNo
                                            select new
                                            {
                                                acctNo = s.AcctNo,
                                                acctName = c.name
                                            }).Single();

                    DN_FinTransWriteSP storeCardPayment = FinTransWriteSproc(connection, transaction, order, payment,
                        ++branchDetails.hirefno, true, isReturn, storeCardDetails.acctNo);
                    storeCardPayment.ExecuteNonQuery();

                    var transfrStoreCardPayment = TransfrStoreCardPayment(order, payment, branchDetails.hirefno, storeCardDetails.acctNo,
                        storeCardDetails.acctName, true, isReturn);
                    ctx.Finxfr.InsertOnSubmit(transfrStoreCardPayment);

                    DN_FinTransWriteSP storeCardTransfer = FinTransWriteSproc(connection, transaction, order, payment,
                        ++branchDetails.hirefno, false, isReturn, storeCardDetails.acctNo);
                    storeCardTransfer.ExecuteNonQuery();

                    transfrStoreCardPayment = TransfrStoreCardPayment(order, payment, branchDetails.hirefno, storeCardDetails.acctNo,
                        storeCardDetails.acctName, false, isReturn);
                    ctx.Finxfr.InsertOnSubmit(transfrStoreCardPayment);

                    var customer = (from sc in ctx.StoreCard
                                    join c in ctx.Customer
                                        on sc.CustID equals c.custid
                                    where sc.CardNumber.Equals(payment.StoreCardNo)
                                    select c).Single();

                    customer.StoreCardAvailable = customer.StoreCardAvailable - payment.Amount;
                    customer.AvailableSpend = customer.AvailableSpend - payment.Amount;

                    var accountDetails = (from a in ctx.Acct
                                          where a.acctno.Equals(storeCardDetails.acctNo)
                                          select a)
                                          .Single();

                    accountDetails.outstbal = accountDetails.outstbal + payment.Amount;
                }
                else
                {
                    string paymentMethodId = null;
                    DateTime dateNow = DateTime.Now;
                    if (payment.MethodId == (int)PaymentTypeEnum.ForeignCash || payment.MethodId == (int)PaymentTypeEnum.TravellersCheque)
                    {
                        try
                        {
                            var suffix = payment.MethodId == (int)PaymentTypeEnum.TravellersCheque ? " Cheque" : "";
                            var codedescript = (payment.CurrencyCode + suffix).ToLower();

                            paymentMethodId = ctx.Code.Single(x => x.category == "FPM" && x.codedescript.ToLower() == codedescript).code;
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("Foreign Currency {0} used on POS is not present in Win Cosacs Payment method. Please enter it in dbo.code table as payment method having category as FPM", payment.CurrencyCode));
                        }
                    }

                    DN_FinTransWriteSP finTransEntry = FinTransWriteSproc(connection, transaction, order, payment,
                        ++branchDetails.hirefno, false, isReturn, null, paymentMethodId, dateNow);
                    finTransEntry.ExecuteNonQuery();

                    if (payment.MethodId == (int)PaymentTypeEnum.ForeignCash || payment.MethodId == (int)PaymentTypeEnum.TravellersCheque)
                    {
                        var finExchangeEntry = TransExchangeSproc(connection, transaction, order, payment,
                            Convert.ToInt16(paymentMethodId), branchDetails.hirefno, dateNow);
                        finExchangeEntry.ExecuteNonQuery();
                    }

                    if (payment.MethodId == (int)PaymentTypeEnum.Cheque)
                    {
                        var chequeDetail = new DChequeDetail();

                        chequeDetail.Write(connection, transaction, accountNo, payment.Bank, payment.BankAccountNo,
                            payment.ChequeNo, (double)payment.Amount, branchDetails.hirefno);
                    }

                    if (payment.MethodId == (int)PaymentTypeEnum.GiftVoucher)
                    {
                        if ("C".Equals(payment.VoucherIssuer))
                        {
                            var giftVoucher = (from g in ctx.GiftVoucherCourts
                                               where g.reference.Equals(payment.VoucherNo)
                                               select g).Single();

                            giftVoucher.dateredeemed = order.CreatedOn;
                            giftVoucher.empeenoredeemed = order.CreatedBy;
                            giftVoucher.acctnoredeemed = accountNo;
                            giftVoucher.transrefnoredeemed = branchDetails.hirefno;

                            ctx.GiftVoucherRedeemed.InsertOnSubmit(AddInGiftVoucherRedeemedTable(giftVoucher.reference, giftVoucher.acctnoredeemed, giftVoucher.transrefnoredeemed.Value));
                        }
                        else
                        {
                            var giftVoucher = (from g in ctx.GiftVoucherOther
                                               where g.reference.Equals(payment.VoucherNo) && g.acctnocompany.Equals(payment.VoucherIssuerCode)
                                               select g).Single();

                            giftVoucher.dateredeemed = order.CreatedOn;
                            giftVoucher.acctnoredeemed = accountNo;
                            giftVoucher.transrefnoredeemed = branchDetails.hirefno;

                            ctx.GiftVoucherRedeemed.InsertOnSubmit(AddInGiftVoucherRedeemedTable(giftVoucher.reference, giftVoucher.acctnoredeemed, giftVoucher.transrefnoredeemed.Value));
                        }
                    }
                }
            }
        }

        private DN_ExchangeTransSaveSP TransExchangeSproc(SqlConnection conn, SqlTransaction t, Order order,
            Payment payment, short paymentMethodId, int transRefNo, DateTime? dateNow = null)
        {
            return new DN_ExchangeTransSaveSP(conn, t)
            {
                piAcctNo = accountNo,
                piTransRefNo = transRefNo,
                piDateTrans = dateNow == null ? DateTime.Now : dateNow,
                piPayMethod = paymentMethodId,
                piForeignTender = CountryRound(payment.CurrencyAmount),
                piLocalChange = CountryRound(0),
                piBranchNo = order.BranchNo
            };
        }

        private DN_FinTransWriteSP FinTransWriteSproc(SqlConnection conn, SqlTransaction t, Order order, Payment payment,
            int hirefno, bool transfer, bool isReturn, string storeCardAcctNo, string paymentMethodId = null, DateTime? dateNow = null)
        {
            var isStoreCrad = (payment.MethodId == (int)PaymentTypeEnum.StoreCard);
            var storeCradTransType = isReturn ? TransType.StoreCardRefund : TransType.StoreCardPayment;
            var nonStoreCradTransType = isReturn
                ? TransType.Refund
                : payment.Amount < 0 ? TransType.Refund : TransType.Payment;

            var mappedPyMethod = string.IsNullOrEmpty(paymentMethodId) ? (short)0 : short.Parse(paymentMethodId);

            if (mappedPyMethod > 0 && paymentLookUp.ContainsKey(mappedPyMethod))
            {
                mappedPyMethod = paymentLookUp[mappedPyMethod];
            }

            var ret = new DN_FinTransWriteSP(conn, t)
            {
                origbr = 0,
                branchno = isReturn ? order.OriginalBranchNo : order.BranchNo,
                acctno = transfer ? storeCardAcctNo : accountNo,
                transrefno = hirefno,
                datetrans = dateNow == null ? DateTime.Now : dateNow,
                transtypecode = transfer ? TransType.Transfer : (isStoreCrad ? storeCradTransType : nonStoreCradTransType),
                empeeno = order.CreatedBy,
                transupdated = "Y",
                transprinted = "Y",
                transvalue = CountryRound(transfer ? payment.Amount : payment.Amount * -1),
                bankcode = payment.Bank,
                bankacctno = payment.BankAccountNo,
                chequeno = payment.MethodId == (int)PaymentTypeEnum.StoreCard ?         //strange but they use this column for storecard account no
                                (transfer ? accountNo : storeCardAcctNo) :
                                payment.MethodId == (int)PaymentTypeEnum.Cheque ? payment.ChequeNo : string.Empty,
                ftnotes = FtNotesPayment,
                paymethod = mappedPyMethod > 0 ? mappedPyMethod : paymentLookUp.ContainsKey(payment.MethodId) ? paymentLookUp[payment.MethodId] : payment.MethodId,
                runno = 0,
                source = "COSACS",
                agrmtno = isReturn ? order.OriginalOrderId : order.Id
            };

            return ret;
        }

        //Add price difference due to Warrnaty Rerurn Percentage
        private DN_FinTransWriteSP AddPaymentsInFinTransTable(SqlConnection conn, SqlTransaction t, Order order, string department,
            decimal priceDifference, int hirefno, bool isReturn = false)
        {
            var stocklocn = isReturn ? order.OriginalBranchNo : order.BranchNo;

            var ret = new DN_FinTransWriteSP(conn, t)
            {
                origbr = 0,
                branchno = stocklocn,
                acctno = accountNo,
                transrefno = hirefno,
                datetrans = DateTime.Now,
                transtypecode = "PCF".Equals(department) ? TransType.FurnWarrantyRecovery : TransType.ElecWarrantyRecovery,
                empeeno = order.CreatedBy,
                transupdated = "Y",
                transprinted = "Y",
                transvalue = CountryRound(priceDifference),
                bankcode = string.Empty,
                bankacctno = string.Empty,
                chequeno = string.Empty,
                ftnotes = FtNotesPayment,
                paymethod = 0,
                runno = 0,
                source = "COSACS",
                agrmtno = order.OriginalOrderId
            };

            return ret;
        }

        #endregion

        private void ManageNewWarranty()
        {
            var uniqueWarranties = new List<string>();
            using (var ctx = Context.Create())
            {
                foreach (var warranty in warranties)
                {
                    //In POS, ID for warranty is coming through warranty.warranty table but in wincosacs' delivery table ID from stockinfo is required
                    //If there is no entry for any particular warranty in stockinfo table then have to create it.. stockitem is used in warranty contract so 

                    if (!uniqueWarranties.Contains(warranty.ItemNo))
                    {
                        uniqueWarranties.Add(warranty.ItemNo);
                    }

                    //Had to use FirstOrDefault() as some of the warranties had 2 rows in stockinfo table. Strange but true
                    warranty.ProductItemId =
                        ctx.StockItem.Where(i => warranty.ItemNo.Equals(i.itemno) && i.stocklocn == order.BranchNo)
                            .OrderByDescending(i => i.ID)
                            .Select(i => i.ItemID)
                            .FirstOrDefault();

                    if (warranty.ProductItemId == 0)
                    {
                        var stockInfodata = ctx.StockInfo.FirstOrDefault(i => warranty.ItemNo.Equals(i.itemno));
                        if (stockInfodata != null)
                        {
                            if (!ctx.StockPrice.Any(i => i.ID == stockInfodata.Id && i.branchno == order.BranchNo))
                            {
                                ctx.StockPrice.InsertOnSubmit(AddInStockPriceTable(warranty, stockInfodata.Id, order.BranchNo));
                            }
                            if (!ctx.StockQuantity.Any(i => i.ID == stockInfodata.Id && i.stocklocn == order.BranchNo))
                            {
                                ctx.StockQuantity.InsertOnSubmit(AddInStockQuantityTable(warranty, stockInfodata.Id, order));
                            }
                            warranty.ProductItemId = stockInfodata.Id;
                        }
                        else
                        {
                            var nextId = ctx.StockInfo.Max(x => x.Id) + 1;
                            ctx.StockInfo.InsertOnSubmit(AddInStockInfoTable(warranty, nextId));
                            ctx.StockPrice.InsertOnSubmit(AddInStockPriceTable(warranty, nextId, order.BranchNo));
                            ctx.StockQuantity.InsertOnSubmit(AddInStockQuantityTable(warranty, nextId, order));

                            warranty.ProductItemId = nextId;
                        }
                    }
                }

                ctx.SubmitChanges();
            }
        }

        private void PostWarrantyHubMessages(SqlConnection conn, SqlTransaction tran)
        {
            using (var ctx = Context.Create(conn, tran))
            {
                var warrantyRepos = new WarrantyRepository();

                foreach (var item in nonWarrantiesItems)
                {
                    if (item.Returned)
                    {
                        var itemWarranties = warranties.Where(w => w.ParentId == item.ProductItemId && w.Returned).ToList();
                        foreach (var warranty in itemWarranties)
                        {
                            warrantyRepos.ReturnWarranty(warranty.WarrantyContractNo, order.BranchNo, conn,
                                tran, WarrantyStatus.Cancelled, warranty.ReturnQuantity);
                        }

                    }
                    else
                    {
                        warrantyRepos.DeliverCGItem(conn, tran, accountNo, order.Id,
                            item.ProductItemId, order.BranchNo, item.Quantity, null, order.CreatedBy, order.IsTaxFreeSale);
                    }
                }
                ctx.SubmitChanges();
            }
        }

        //private void UpdateBrokerTransaction(Chub chub)
        //{
        //    using (var conn = new SqlConnection(Connections.Default))
        //    {
        //        chub.Submit(installations, conn, trans);
        //    }

        //}

        private void UpdateStockQty(Context ctx, short branchNo)
        {
            foreach (var item in items.Where(x => x.ItemTypeId == (int)ItemTypeEnum.Product))
            {
                try
                {
                    var databaseItem = ctx.StockQuantity.SingleOrDefault(c => c.itemno == item.ItemNo && c.stocklocn == branchNo) ??
                                                 new StockQuantity
                                                 {
                                                     itemno = item.ItemNo,
                                                     stocklocn = branchNo,
                                                     qtyAvailable = 0,
                                                     stock = 0,
                                                     stockdamage = 0,
                                                     leadtime = 0,
                                                     deleted = "N"
                                                 };

                    var qty = item.Returned ? item.ReturnQuantity * -1 : item.Quantity;
                    databaseItem.qtyAvailable = databaseItem.qtyAvailable - qty;
                    databaseItem.stock = databaseItem.stock - qty;
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Quantity update failed for itemNo = {0}. Chceck StockQuantity table.", item.ItemNo));
                }
            }
        }

        #region Enums

        private enum ItemTypeEnum
        {
            Product = 1, Warranty = 2, Installation = 3, NonStock = 4, Discount = 5, Kit = 8
        }

        private enum PaymentTypeEnum
        {
            Cash = 1,
            ForeignCash,
            StoreCard,
            GiftVoucher,
            Cheque,
            DebitCreditCard,
            StandingOrder,
            TravellersCheque
        }

        #endregion

        #region Removed Code

        #endregion

        private class WarrantyGroup
        {
            public int ParentId { get; set; }
            public string WarrantyType { get; set; }
            public int Count { get; set; }
        }
    }
}
