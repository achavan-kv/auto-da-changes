using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using AutoMapper;
using Blue.Cosacs.Sales.Models;
using Blue.Networking;
using System.Net;
using System.Text;
using Blue.Transactions;
using Blue.Hub.Client;
using Blue.Cosacs.Sales.Common;
using Newtonsoft.Json;

namespace Blue.Cosacs.Sales.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IClock clock;
        private readonly IPublisher publisher;
        private readonly IHttpClientJson httpClientJson;
        private readonly IHttpClient httpClient;

        public OrderRepository(IClock clock, IPublisher publisher, IHttpClientJson httpClientJson, IHttpClient httpClient)
        {
            this.clock = clock;
            this.publisher = publisher;
            this.httpClientJson = httpClientJson;
            this.httpClient = httpClient;
        }

        #region Web Method

        public OrderSaveReturn Save(OrderDto saleToSave, int userId)
        {
            var ret = new OrderSaveReturn();

            try
            {
                var failedPayment = ValidatePayments(saleToSave);

                if (string.IsNullOrEmpty(failedPayment.ErrorMessage))
                {
                    using (var scope = Context.Write())
                    {
                        var newSales = new Order();

                        if (IsOrderHasCustomer(saleToSave))
                        {
                            FillSaleOrderCustomer(saleToSave, newSales);
                        }

                        if (saleToSave.OriginalOrderId > 0)
                        {
                            newSales.OriginalOrderId = saleToSave.OriginalOrderId;
                        }
                        newSales.SoldBy = saleToSave.SoldBy;
                        newSales.Comments = saleToSave.Comments;
                        newSales.BranchNo = saleToSave.BranchNo;
                        newSales.CreatedBy = userId;
                        newSales.CreatedOn = clock.Now;
                        newSales.TotalAmount = saleToSave.TotalAmount;
                        newSales.TotalDiscount = saleToSave.TotalDiscount;
                        newSales.TotalTaxAmount = saleToSave.TotalTaxAmount;
                        newSales.IsTaxFreeSale = saleToSave.IsTaxFreeSale;
                        newSales.IsDutyFreeSale = saleToSave.IsDutyFreeSale;

                        newSales.Payments = GetSaleOrderPayments(saleToSave).ToList();
                        newSales.Items = GetSaleOrderItems(saleToSave).ToList();

                        //CR Function Suvidha
                        string agr_inv_num = GenerateInvoiceNumber(saleToSave.BranchNo); //GenerateInvoiceNumber
                        newSales.AgreementInvoiceNumber = agr_inv_num;

                        if (saleToSave.ReturnedItems != null && saleToSave.ReturnedItems.Count > 0)
                        {
                            SetReturnedSubItems(newSales, saleToSave);
                            UpdateOriginalSubItems(scope, saleToSave);
                        }

                        scope.Context.Order.Add(newSales);
                        scope.Context.SaveChanges();

                        if (saleToSave.OriginalOrderId > 0)
                        {
                            UpdateOriginalOrder(scope, saleToSave);
                        }
                        SendHubMessage(newSales, saleToSave, scope);

                        //scope.Context.Order

                        scope.Complete();
                        string agrinvnum = newSales.AgreementInvoiceNumber.Insert(3, "-");

                        ret = new OrderSaveReturn
                        {
                            InvoiceNo = agrinvnum,                            
                            Valid = true,
                            Errors = new string[] { }
                        };
                    }
                }
                else
                {
                    ret = new OrderSaveReturn
                    {
                        InvoiceNo = "",
                        Valid = false,
                        Errors = new string[] { },
                        FailedPayments = failedPayment
                    };
                }
            }
            catch (DbEntityValidationException e)
            {
                var errMsgs = new StringBuilder();

                foreach (var eve in e.EntityValidationErrors)
                {
                    errMsgs.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errMsgs.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }

                throw new Exception(errMsgs.ToString());
            }
            catch (Exception ex)
            {
                ret = new OrderSaveReturn
                {
                    InvoiceNo = "",
                    Valid = false,
                    Errors = new[] { ex.Message }
                };
            }

            return ret;
        }

        public OrderDto Get(string id)
        {
            OrderDto orderDto = null;

            var parentProductsTypeIds = new List<int>
            {
            (int)ItemTypeEnum.Product, 
            (int)ItemTypeEnum.Kit
            };

            using (var scope = Context.Read())
            {                
                if (id.Length == 14)
                {
                    //saleOrder = scope.Context.Order
                    //.FirstOrDefault(p => p.AgreementInvoiceNumber == id);

                    //var saleorder_id = (from o in scope.Context.Order
                    //                 where o.AgreementInvoiceNumber == id
                    //                 select o.Id);

                    var saleorder_id = scope.Context.Order
                    .FirstOrDefault(p => p.AgreementInvoiceNumber == id).Id;

                    id = Convert.ToString(saleorder_id);
                }
                //else
                //{
                    //saleOrder = scope.Context.Order
                    //.FirstOrDefault(p => p.Id == Convert.ToInt32(id));                    
                //}
                
                int order_id = Convert.ToInt32(id);

                var saleOrder = scope.Context.Order
                .FirstOrDefault(p => p.Id == order_id);

                if (saleOrder == null)
                {
                    return orderDto;
                }

                orderDto = Mapper.Map<Order, OrderDto>(saleOrder);

                var allItems = scope.Context.OrderItem
                    .Where(p => p.OrderId == order_id).ToList();

                var products = allItems.Where(p => parentProductsTypeIds.Contains(p.ItemTypeId)).ToList();

                //If only warranty is return then this is needed
                if (products.Any())
                {
                    orderDto.Items.Clear();
                }

                foreach (var product in products)
                {
                    var itemDto = Mapper.Map<OrderItem, ItemDto>(product);

                    if (product.ParentId.HasValue)
                    {
                        var parentId = product.ParentId.Value;
                        var productItemId = allItems.Where(p => parentProductsTypeIds.Contains(p.ItemTypeId) && p.Id == parentId)
                            .Select(i => i.ProductItemId).SingleOrDefault();

                        var productItemNo = allItems.Where(p => parentProductsTypeIds.Contains(p.ItemTypeId) && p.Id == parentId)
                            .Select(i => i.ItemNo).SingleOrDefault();

                        itemDto.ParentId = productItemId;
                        itemDto.ParentItemNo = productItemNo;
                    }

                    var warranties =
                        allItems.Where(p => p.ParentId == product.Id && p.ItemTypeId == (int)ItemTypeEnum.Warranty)
                            .ToList();

                    itemDto.Warranties = Mapper.Map<List<OrderItem>, List<ItemDto>>(warranties);

                    var installations = allItems.Where(p => p.ParentId == product.Id && p.ItemTypeId == (int)ItemTypeEnum.Installation)
                        .ToList();

                    itemDto.Installations = Mapper.Map<List<OrderItem>, List<ItemDto>>(installations);

                    var discounts = allItems.Where(p => p.ParentId == product.Id && p.ItemTypeId == (int)ItemTypeEnum.Discount).ToList();

                    itemDto.Discounts = new List<DiscountDto>();
                    foreach (var discount in discounts)
                    {
                        itemDto.Discounts.Add(new DiscountDto
                        {
                            Id = discount.Id,
                            Code = discount.ProductItemId ?? 0,
                            ItemNo = discount.ItemNo,
                            Description = discount.Description,
                            PosDescription = string.IsNullOrEmpty(discount.PosDescription) ? discount.Description : discount.PosDescription,
                            Amount = discount.ManualDiscount,
                            Percentage = discount.ManualDiscountPercentage,
                            Returned = discount.Returned,
                            ReturnQuantity = discount.ReturnQuantity,
                            ParentItemNo = discount.ParentItemNo,
                            TaxRate = discount.TaxRate,
                            IsFixedDiscount = discount.IsFixedDiscount,
                            TaxAmount = discount.TaxAmount
                        });
                    }

                    orderDto.Items.Add(itemDto);
                }
            }
            
            return orderDto;
        }

        public ReceiptReprintResponse SearchOrdersForRePrint(int branchNo, System.DateTime dateFrom, System.DateTime dateTo, string invoiceNoMin, string invoiceNoMax, int start, int rows)
        {
            Int64 agrInvNumMin = 0, agrInvNumMax = 0;
            int oldInvNoMin = 0, oldInvNoMax = 0;            
            ReceiptReprintResponse response = new ReceiptReprintResponse();
            dateFrom = dateFrom.Date;
            dateTo = dateTo.Date.AddDays(1).AddTicks(-1);
            using (var scope = Context.Read())
            {
                if (invoiceNoMin.Length >= 14 && invoiceNoMax.Length >= 14)
                {
                    agrInvNumMin = Convert.ToInt64(invoiceNoMin.Replace("-", ""));
                    agrInvNumMax = Convert.ToInt64(invoiceNoMax.Replace("-", ""));
                    var query = from o in scope.Context.Order
                                join oi in scope.Context.OrderItem
                                on o.Id equals oi.OrderId
                                join b in scope.Context.Order on o.OriginalOrderId equals b.Id
                                into rd from rt in rd.DefaultIfEmpty()
                                where ((branchNo == 0 || o.BranchNo == branchNo) && o.CreatedOn >= dateFrom && o.CreatedOn <= dateTo)
                                select new ReceiptReprintDto
                                {
                                    InvoiceNo = o.Id,
                                    AgreementInvoiceNumber =  o.AgreementInvoiceNumber,
                                    CreatedOn = o.CreatedOn,
                                    ItemId = oi.Id,
                                    ParentId = oi.ParentId,
                                    ItemNo = oi.ItemNo,
                                    ItemDescription = oi.Description,
                                    PosDescription = string.IsNullOrEmpty(oi.PosDescription) ? oi.Description : oi.PosDescription,
                                    Branch = o.BranchNo,
                                    Quantity = oi.Quantity,
                                    Price = oi.Price,
                                    TaxAmount = oi.TaxAmount,
                                    Discount = oi.ManualDiscount,
                                    WarrantyContractNo = oi.WarrantyContractNo,
                                    WarrantyLength = oi.WarrantyLengthMonths,
                                    OriginalOrderId = o.OriginalOrderId.HasValue ? o.OriginalOrderId.Value.ToString() : (oi.Price < 0 ? "Manual" : null),
                                    OriginalAgreementInvoiceNumber = o.OriginalOrderId.HasValue ? rt.AgreementInvoiceNumber : (oi.Price < 0 ? "Manual" : null),
                                    Count = 0,
                                    IsFreeWarranty = oi.ItemTypeId == (byte)ItemTypeEnum.Warranty && oi.WarrantyTypeCode == "F"
                                };

                    IEnumerable<ReceiptReprintDto> query_enum = query;
                    var query2 = from i in query_enum
                            where (agrInvNumMin == 0 || Convert.ToInt64(i.AgreementInvoiceNumber) >= agrInvNumMin)
                            && (agrInvNumMax == 0 || Convert.ToInt64(i.AgreementInvoiceNumber) <= agrInvNumMax)
                            select new ReceiptReprintDto
                            {
                                InvoiceNo = i.InvoiceNo,
                                AgreementInvoiceNumber = i.AgreementInvoiceNumber,
                                OriginalAgreementInvoiceNumber = i.OriginalAgreementInvoiceNumber,
                                CreatedOn = i.CreatedOn,
                                ItemId = i.ItemId,
                                ParentId = i.ParentId,
                                ItemNo = i.ItemNo,
                                ItemDescription = i.ItemDescription,
                                PosDescription = i.PosDescription,
                                Branch = i.Branch,
                                Quantity = i.Quantity,
                                Price = i.Price,
                                TaxAmount = i.TaxAmount,
                                Discount = i.Discount,
                                WarrantyContractNo = i.WarrantyContractNo,
                                WarrantyLength = i.WarrantyLength,
                                OriginalOrderId = i.OriginalOrderId,
                                Count = 0,
                                IsFreeWarranty = i.IsFreeWarranty
                            };                    

                    response.Count = query2.Count();
                    var groupQuery = query2.OrderByDescending(x => x.CreatedOn)
                                    .Skip(start)
                                    .Take(rows)
                                    .GroupBy(x => x.InvoiceNo);

                    foreach (var items in groupQuery)
                    {
                        bool isManual = false;
                        if (items.Any(x => "Manual".Equals(x.OriginalOrderId)))
                        {
                            isManual = true;
                        }

                        for (int i = 0; i < items.Count(); i++)
                        {
                            if (i == 0)
                            {
                                items.ElementAt(i).Count = items.Count();
                            }
                            items.ElementAt(i).OriginalOrderId = isManual ? "Manual" : items.ElementAt(i).OriginalOrderId;
                            items.ElementAt(i).AgreementInvoiceNumber = items.ElementAt(i).AgreementInvoiceNumber == null ? "" : items.ElementAt(i).AgreementInvoiceNumber.Insert(3, "-");
                            items.ElementAt(i).OriginalAgreementInvoiceNumber = items.ElementAt(i).OriginalAgreementInvoiceNumber == null ? "" : items.ElementAt(i).OriginalAgreementInvoiceNumber.Insert(3, "-");
                            response.OrdersList.Add(items.ElementAt(i));
                        }
                    }
                    response.OrdersList = response.OrdersList.OrderByDescending(x => x.InvoiceNo).ToList();

                }
                else
                {
                    oldInvNoMin = Convert.ToInt32(invoiceNoMin);
                    oldInvNoMax = Convert.ToInt32(invoiceNoMax);
                    var query = from o in scope.Context.Order
                                join oi in scope.Context.OrderItem
                                on o.Id equals oi.OrderId
                                join b in scope.Context.Order on o.OriginalOrderId equals b.Id
                                into rd from rt in rd.DefaultIfEmpty()
                                where ((branchNo == 0 || o.BranchNo == branchNo) && o.CreatedOn >= dateFrom && o.CreatedOn <= dateTo)
                                && ((oldInvNoMin == 0 || o.Id >= oldInvNoMin) && (oldInvNoMax == 0 || o.Id <= oldInvNoMax))
                                select new ReceiptReprintDto
                                {
                                    InvoiceNo = o.Id,
                                    AgreementInvoiceNumber = o.AgreementInvoiceNumber,
                                    CreatedOn = o.CreatedOn,
                                    ItemId = oi.Id,
                                    ParentId = oi.ParentId,
                                    ItemNo = oi.ItemNo,
                                    ItemDescription = oi.Description,
                                    PosDescription = string.IsNullOrEmpty(oi.PosDescription) ? oi.Description : oi.PosDescription,
                                    Branch = o.BranchNo,
                                    Quantity = oi.Quantity,
                                    Price = oi.Price,
                                    TaxAmount = oi.TaxAmount,
                                    Discount = oi.ManualDiscount,
                                    WarrantyContractNo = oi.WarrantyContractNo,
                                    WarrantyLength = oi.WarrantyLengthMonths,
                                    OriginalOrderId = o.OriginalOrderId.HasValue ? o.OriginalOrderId.Value.ToString() : (oi.Price < 0 ? "Manual" : null),
                                    OriginalAgreementInvoiceNumber = o.OriginalOrderId.HasValue ? rt.AgreementInvoiceNumber : (oi.Price < 0 ? "Manual" : null),
                                    Count = 0,
                                    IsFreeWarranty = oi.ItemTypeId == (byte)ItemTypeEnum.Warranty && oi.WarrantyTypeCode == "F"
                                };

                    response.Count = query.Count();

                    var groupQuery = query.OrderByDescending(x => x.CreatedOn)
                                    .Skip(start)
                                    .Take(rows)
                                    .GroupBy(x => x.InvoiceNo);

                    foreach (var items in groupQuery)
                    {
                        bool isManual = false;
                        if (items.Any(x => "Manual".Equals(x.OriginalOrderId)))
                        {
                            isManual = true;
                        }

                        for (int i = 0; i < items.Count(); i++)
                        {
                            if (i == 0)
                            {
                                items.ElementAt(i).Count = items.Count();
                            }
                            items.ElementAt(i).OriginalOrderId = isManual ? "Manual" : items.ElementAt(i).OriginalOrderId;
                            items.ElementAt(i).AgreementInvoiceNumber = items.ElementAt(i).AgreementInvoiceNumber == null ? "" : items.ElementAt(i).AgreementInvoiceNumber.Insert(3, "-");
                            items.ElementAt(i).OriginalAgreementInvoiceNumber = items.ElementAt(i).OriginalAgreementInvoiceNumber == null ? "" : items.ElementAt(i).OriginalAgreementInvoiceNumber.Insert(3, "-");
                            response.OrdersList.Add(items.ElementAt(i));
                        }
                    }
                    response.OrdersList = response.OrdersList.OrderByDescending(x => x.InvoiceNo).ToList();
                }
                ArrangeLinkedProducts(response.OrdersList);
                return response;
            }
        }
        
        public OrderExtendedDto GetOrderForRePrint(string orderId, string currentUser, string receiptType)
        {
            using (var scope = Context.Read())
            {
                //CR 2018-13
                if (orderId != string.Empty && orderId.Length < 14)
                {
                    int saleOrderID = Convert.ToInt32(orderId);
                    var query = (from o in scope.Context.Order
                                 where o.Id == saleOrderID
                                 select o
                                ).ToList();
                    return EnrichDataToPrint(query, currentUser, receiptType).FirstOrDefault();
                }
                else
                {
                    orderId = orderId.Replace("-", "");
                    var query = (from o in scope.Context.Order
                                 where o.AgreementInvoiceNumber == orderId
                                 select o
                                ).ToList();
                    return EnrichDataToPrint(query, currentUser, receiptType).FirstOrDefault();
                }                
            }
        }

        public IList<OrderExtendedDto> GetOrdersForRePrintAll(int branchNo, DateTime dateFrom, DateTime dateTo, int invoiceNoMin, int invoiceNoMax, string currentUser)
        {
            dateFrom = dateFrom.Date;
            dateTo = dateTo.Date.AddDays(1).AddTicks(-1);
            using (var scope = Context.Read())
            {
                var orderDetails = (from o in scope.Context.Order
                                    where ((branchNo == 0 || o.BranchNo == branchNo) && o.CreatedOn >= dateFrom && o.CreatedOn <= dateTo)
                                             && ((invoiceNoMin == 0 || o.Id >= invoiceNoMin) && (invoiceNoMax == 0 || o.Id <= invoiceNoMax))
                                    select o)
                                    .Take(50)
                                    .ToList();

                return EnrichDataToPrint(orderDetails, currentUser, "Reprint");
            }
        }

        #endregion

        public void IndexNewCustomer(string firstName, string lastName, int userId)
        {
            var url = string.Format("/Customer/api/Reindex/IndexSalesCustomer?firstName={0}&lastName={1}", firstName, lastName);
            var jsonClient = new HttpClientJsonAuth(httpClient, clock, userId.ToString());
            var request = RequestJson<dynamic>.Create(url, System.Net.WebRequestMethods.Http.Post);
            var data = jsonClient.Do<dynamic, dynamic>(request).Body;
        }

        #region FillSaleOrder

        private void FillSaleOrderCustomer(OrderDto orderDto, Order order)
        {
            order.OrderCustomer = new OrderCustomer();
            order.OrderCustomer.Title = orderDto.Customer.Title;
            order.OrderCustomer.FirstName = orderDto.Customer.FirstName;
            order.OrderCustomer.LastName = orderDto.Customer.LastName;
            order.OrderCustomer.AddressLine1 = orderDto.Customer.AddressLine1;
            order.OrderCustomer.AddressLine2 = orderDto.Customer.AddressLine2;
            order.OrderCustomer.AddressLine3 = orderDto.Customer.TownOrCity;
            order.OrderCustomer.PostCode = orderDto.Customer.PostCode;
            order.OrderCustomer.CustomerId = string.IsNullOrEmpty(orderDto.Customer.CustomerId) ? null : orderDto.Customer.CustomerId;
            order.OrderCustomer.Email = string.IsNullOrEmpty(orderDto.Customer.Email) ? null : orderDto.Customer.Email;
            order.OrderCustomer.HomePhone = string.IsNullOrEmpty(orderDto.Customer.HomePhoneNumber) ? null : orderDto.Customer.HomePhoneNumber;
            order.OrderCustomer.MobilePhone = string.IsNullOrEmpty(orderDto.Customer.MobileNumber) ? null : orderDto.Customer.MobileNumber;
            order.OrderCustomer.IsSalesCustomer = orderDto.Customer.IsSalesCustomer;
        }

        private IEnumerable<OrderPayment> GetSaleOrderPayments(OrderDto orderDto)
        {
            var retLst = from p in orderDto.Payments
                         select new OrderPayment
                         {
                             Amount = p.Amount,
                             PaymentMethodId = p.PaymentMethodId,
                             Bank = p.Bank,
                             BankAccountNo = p.BankAccountNo,
                             CardType = p.CardType,
                             CardNo = p.CardNo,
                             StoreCardNo = p.StoreCardNo,
                             VoucherNo = p.VoucherNo,
                             ChequeNo = p.ChequeNo,
                             CurrencyCode = p.CurrencyCode,
                             CurrencyRate = p.CurrencyRate,
                             VoucherIssuer = p.VoucherIssuer,
                             VoucherIssuerCode = p.VoucherIssuerCode,
                             CurrencyAmount = p.CurrencyAmount
                         };

            return retLst;
        }

        private IEnumerable<OrderItem> GetSaleOrderItems(OrderDto orderDto)
        {
            var itemList = new List<OrderItem>();
            var settings = new Settings();

            var products = orderDto.Items.Where(p => p.ItemTypeId == (int)ItemTypeEnum.Product || p.ItemTypeId == (int)ItemTypeEnum.Kit);

            foreach (var item in products)
            {
                var productItemId = item.ProductItemId;
                var isReplacement = item.IsReplacement ?? false;

                var newOrderItem = new OrderItem
                {
                    Description = item.Description,
                    PosDescription = string.IsNullOrEmpty(item.PosDescription) ? item.Description : item.PosDescription,
                    ItemNo = item.ItemNo,
                    ProductItemId = item.ProductItemId,
                    ItemTypeId = item.ItemTypeId,// (int)ItemTypeEnum.Product,
                    Price = item.Price,
                    Quantity = item.Returned == true ? item.ReturnQuantity.Value : item.Quantity,
                    TaxRate = item.TaxRate,
                    TaxAmount = item.TaxAmount,
                    IsClaimed = item.IsClaimed,
                    Returned = item.Returned,
                    ReturnQuantity = item.ReturnQuantity,
                    ReturnReason = item.ReturnReason,
                    ItemUPC = item.ItemUPC,
                    ItemSupplier = item.ItemSupplier,
                    Department = item.Returned == true ? item.Department :
                    ProductDepartment.Data.ContainsKey(item.Department.ToUpper()) ? ProductDepartment.Data[item.Department.ToUpper()] : "PCO",
                    Category = item.Category,
                    Class = item.Class,
                    CostPrice = item.CostPrice,
                    RetailPrice = item.RetailPrice,
                    ParentItemNo = item.ParentItemNo,
                    IsReplacement = item.IsReplacement,
                    SalePrice = item.SalePrice,
                    SaleTaxAmount = item.SaleTaxAmount,
                    OrderItems = new List<OrderItem>()
                };

                // Add Item's Kit items

                if (item.KitItems.Any())
                {
                    AddItemKitItems(newOrderItem, item.KitItems);
                }

                // Add Item's Discount
                if (item.Discounts.Any())
                {
                    AddItemDiscounts(newOrderItem, item);
                }

                // Add Items's Installations
                if (item.Installations.Any())
                {
                    AddItemInstallations(newOrderItem, item);
                }

                // Add Items's Warranties
                if (item.Warranties.Any())
                {
                    var freeWarranty = item.Warranties.FirstOrDefault(w => w.WarrantyTypeCode == "F");
                    var nonFreeWarranties = item.Warranties.Where(w => w.WarrantyTypeCode != "F" && !(w.Returned ?? false)).ToList();

                    if (freeWarranty != null && nonFreeWarranties.Any())
                    {
                        foreach (var nonFreeWarranty in nonFreeWarranties)
                        {
                            if (isReplacement && nonFreeWarranty.WarrantyTypeCode == "I" && !settings.DelayNewIRW)
                            {
                                nonFreeWarranty.WarrantyEffectiveDate = clock.Now;
                            }
                            else
                            {
                                nonFreeWarranty.WarrantyEffectiveDate = clock.Now.AddMonths(Convert.ToInt32(freeWarranty.WarrantyLengthMonths));
                            }
                        }
                    }
                    AddItemWarranties(newOrderItem, item);
                }

                itemList.Add(newOrderItem);
            }

            return itemList;
        }

        private void AddItemKitItems(OrderItem orderItem, List<ItemDto> itemDtos)
        {
            foreach (var item in itemDtos)
            {
                var newOrderItem = new OrderItem
                {
                    Description = item.Description,
                    PosDescription = string.IsNullOrEmpty(item.PosDescription) ? item.Description : item.PosDescription,
                    ItemNo = item.ItemNo,
                    ProductItemId = item.ProductItemId,
                    ItemTypeId = (int)ItemTypeEnum.Product,
                    Price = item.Price,
                    Quantity = item.Returned == true ? item.ReturnQuantity.Value : item.Quantity,
                    TaxRate = item.TaxRate,
                    TaxAmount = item.TaxAmount,
                    Returned = item.Returned,
                    ReturnQuantity = item.ReturnQuantity,
                    ReturnReason = item.ReturnReason,
                    ItemUPC = item.ItemUPC,
                    ItemSupplier = item.ItemSupplier,
                    Department = item.Department, // item.Returned == true ? item.Department : ProductDepartment.Data[item.Department],
                    Category = item.Category,
                    Class = item.Class,
                    CostPrice = item.CostPrice,
                    RetailPrice = item.RetailPrice,
                    ParentItemNo = orderItem.ItemNo,
                    IsReplacement = item.IsReplacement,
                    SalePrice = item.SalePrice,
                    SaleTaxAmount = item.SaleTaxAmount,
                    OrderItems = new List<OrderItem>()
                };

                orderItem.OrderItems.Add(newOrderItem);
            }
        }

        private void AddItemDiscounts(OrderItem orderItem, ItemDto itemDto)
        {
            foreach (var discount in itemDto.Discounts)
            {
                orderItem.OrderItems.Add(new OrderItem
                {
                    Description = discount.Description,
                    PosDescription = string.IsNullOrEmpty(discount.PosDescription) ? discount.Description : discount.PosDescription,
                    ItemNo = discount.ItemNo,
                    ProductItemId = discount.Code,
                    ItemTypeId = (int)ItemTypeEnum.Discount,
                    ManualDiscount = discount.Amount,
                    ManualDiscountPercentage = discount.Percentage,
                    Quantity = orderItem.Quantity,
                    Returned = discount.Returned,
                    ReturnQuantity = discount.ReturnQuantity,
                    ParentItemNo = discount.ParentItemNo,
                    TaxRate = discount.TaxRate,
                    IsFixedDiscount = discount.IsFixedDiscount,
                    TaxAmount = discount.TaxAmount
                });
            }
        }

        private void AddItemInstallations(OrderItem orderItem, ItemDto itemDto)
        {
            foreach (var installation in itemDto.Installations)
            {
                installation.Quantity = installation.Returned == true ? installation.ReturnQuantity.Value : installation.Quantity;
                var newItem = Mapper.Map<ItemDto, OrderItem>(installation);
                orderItem.OrderItems.Add(newItem);
            }
        }

        private void AddItemWarranties(OrderItem orderItem, ItemDto itemDto)
        {
            foreach (var warranty in itemDto.Warranties)
            {
                var newItem = Mapper.Map<ItemDto, OrderItem>(warranty);
                orderItem.OrderItems.Add(newItem);
            }
        }

        private bool IsOrderHasCustomer(OrderDto salesOrder)
        {
            var ret = (salesOrder.Customer != null) &&
                (!string.IsNullOrEmpty(salesOrder.Customer.FirstName)) &&
                (!string.IsNullOrEmpty(salesOrder.Customer.LastName));

            return ret;
        }

        private void SetReturnedSubItems(Order newSales, OrderDto saleToSave)
        {
            foreach (var item in saleToSave.ReturnedItems)
            {
                item.Quantity = item.ReturnQuantity.Value;

                if (item.Discounts != null && item.Discounts.Any())
                {
                    foreach (var discount in item.Discounts)
                    {
                        discount.ReturnQuantity = item.ReturnQuantity;
                    }
                }
                var newOrderItem = Mapper.Map<ItemDto, OrderItem>(item);
                newOrderItem.ParentId = null;

                newSales.Items.Add(newOrderItem);
            }
        }

        private void UpdateOriginalOrder(WriteScope<Context> scope, OrderDto orderDto)
        {
            var products = orderDto.Items.Where(p => (p.ItemTypeId == (int)ItemTypeEnum.Product || p.ItemTypeId == (int)ItemTypeEnum.Kit) && (p.Returned == true));

            foreach (var product in products)
            {
                var item = scope.Context.OrderItem.SingleOrDefault(i => i.Id == product.OriginalId);

                if (item == null)
                {
                    continue;
                }
                var returnQuantity = item.ReturnQuantity.HasValue ? item.ReturnQuantity.Value : 0;

                item.ReturnQuantity = (short?)(returnQuantity + product.ReturnQuantity);

                item.Returned = item.ReturnQuantity >= item.Quantity;

                if (!product.Warranties.Any())
                {
                    continue;
                }

                foreach (var warranty in product.Warranties)
                {
                    var subItem = scope.Context.OrderItem.SingleOrDefault(i => i.Id == warranty.OriginalId);

                    if (subItem == null)
                    {
                        continue;
                    }

                    subItem.ReturnQuantity = warranty.ReturnQuantity;

                    subItem.Returned = subItem.ReturnQuantity >= subItem.Quantity;
                    subItem.IsClaimed = warranty.IsClaimed;
                }

                foreach (var installation in product.Installations)
                {
                    var subItem = scope.Context.OrderItem.SingleOrDefault(i => i.Id == installation.OriginalId);

                    if (subItem == null)
                    {
                        continue;
                    }

                    subItem.ReturnQuantity = installation.ReturnQuantity;

                    subItem.Returned = subItem.ReturnQuantity >= subItem.Quantity;
                }

                foreach (var discount in product.Discounts)
                {
                    var subItem = scope.Context.OrderItem.SingleOrDefault(i => i.Id == discount.OriginalId);

                    if (subItem == null)
                    {
                        continue;
                    }

                    //subItem.ReturnQuantity = item.ReturnQuantity >= item.Quantity ? (short)1 : (short)0;

                    subItem.ReturnQuantity = item.ReturnQuantity;
                    subItem.Returned = item.ReturnQuantity >= item.Quantity;
                }
            }

            scope.Context.SaveChanges();
        }

        private void UpdateOriginalSubItems(WriteScope<Context> scope, OrderDto orderDto)
        {
            foreach (var product in orderDto.ReturnedItems)
            {
                var item = scope.Context.OrderItem.SingleOrDefault(i => i.Id == product.OriginalId);

                if (item == null)
                {
                    continue;
                }
                var returnQuantity = item.ReturnQuantity.HasValue ? item.ReturnQuantity.Value : 0;

                item.ReturnQuantity = (short?)(returnQuantity + product.ReturnQuantity);

                item.Returned = item.ReturnQuantity >= item.Quantity;
            }
            scope.Context.SaveChanges();
        }

        //TODO: Refactor
        private ValidatePaymentsReturn ValidatePayments(OrderDto orderDto)
        {
            var response = new ValidatePaymentsReturn
            {
                TempPaymentId = 0,
                TenderedAmount = 0,
                ErrorMessage = string.Empty,
            };

            var storeCardPayments = orderDto.Payments.Where(x => x.PaymentMethodId.Equals(Convert.ToByte(PaymentTypeEnum.StoreCard)));
            if (storeCardPayments.Any())
            {
                response = ValidateStoreCardPayments(storeCardPayments, orderDto.BranchNo, response);
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return response;
                }
            }

            var giftVoucherPayments = orderDto.Payments.Where(x => x.PaymentMethodId.Equals(Convert.ToByte(PaymentTypeEnum.GiftVoucher)));
            if (giftVoucherPayments.Any())
            {
                response = ValidateGiftVoucherPayments(giftVoucherPayments, response);
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return response;
                }
            }
            return response;
        }

        //TODO: Refactor
        private ValidatePaymentsReturn ValidateStoreCardPayments(IEnumerable<PaymentDto> storeCardPayments, short branchNo, ValidatePaymentsReturn response)
        {
            foreach (var storeCardPayment in storeCardPayments)
            {
                var uriString = string.Format(
                                                "/Courts.NET.WS/Sales/GetStoreCardAvailableBalance?storeCardNo={0}",
                                                storeCardPayment.StoreCardNo.ToString());

                var storeCardDetails = httpClientJson.Do<byte[], dynamic>(RequestJson<byte[]>.Create(uriString, WebRequestMethods.Http.Get)).Body;

                if (!string.IsNullOrEmpty(Convert.ToString(storeCardDetails.errorMessage)))
                {
                    response = new ValidatePaymentsReturn
                    {
                        TempPaymentId = storeCardPayment.TempPaymentId,
                        TenderedAmount = storeCardPayment.Amount,
                        ErrorMessage = Convert.ToString(storeCardDetails.errorMessage)
                    };

                    return response;
                }
                else if (Convert.ToDecimal(storeCardDetails.availableBalance) < storeCardPayment.Amount)
                {
                    response = new ValidatePaymentsReturn
                    {
                        TempPaymentId = storeCardPayment.TempPaymentId,
                        TenderedAmount = storeCardPayment.Amount,
                        ErrorMessage = string.Format(
                                        "Store Card : {0} is having available balance as : {1} which is less than the tendered amount {2}. Please Re-Adjust.",
                                        storeCardPayment.StoreCardNo,
                                        storeCardDetails.availableBalance,
                                        storeCardPayment.Amount)
                    };

                    return response;
                }
            }

            return response;
        }

        //TODO: Refactor this
        private ValidatePaymentsReturn ValidateGiftVoucherPayments(IEnumerable<PaymentDto> giftVoucherPayments, ValidatePaymentsReturn response)
        {
            foreach (var giftVoucherPayment in giftVoucherPayments)
            {
                var uriString = string.Format(
                                            "/Courts.NET.WS/Sales/GetGiftVoucherDetails?giftVoucherIssuer={0}&otherCompanyNo={1}&giftVoucherNo={2}",
                                            giftVoucherPayment.VoucherIssuer,
                                            giftVoucherPayment.VoucherIssuerCode,
                                            giftVoucherPayment.VoucherNo);

                var giftVoucherDetails = httpClientJson.Do<byte[], dynamic>(RequestJson<byte[]>.Create(uriString, WebRequestMethods.Http.Get)).Body;

                if (!string.IsNullOrEmpty(Convert.ToString(giftVoucherDetails.errorMessage)))
                {
                    response = new ValidatePaymentsReturn
                    {
                        TempPaymentId = giftVoucherPayment.TempPaymentId,
                        TenderedAmount = giftVoucherPayment.Amount,
                        ErrorMessage = Convert.ToString(giftVoucherDetails.errorMessage)
                    };
                    return response;
                }
                else if (Convert.ToDecimal(giftVoucherDetails.value) < giftVoucherPayment.Amount)
                {
                    response = new ValidatePaymentsReturn
                    {
                        TempPaymentId = giftVoucherPayment.TempPaymentId,
                        TenderedAmount = giftVoucherPayment.Amount,
                        ErrorMessage = string.Format(
                                        "Gift Voucher: {0} is having available balance as : {1} which is less than the tendered amount {2}. Please Re-Adjust.",
                                        giftVoucherPayment.VoucherNo,
                                        giftVoucherDetails.value,
                                        giftVoucherPayment.Amount)
                    };
                    return response;
                }
            }
            return response;
        }

        #endregion

        #region Enrich Data to Print

        private IList<OrderExtendedDto> EnrichDataToPrint(IList<Order> orderDetails, string currentUser, string receiptType)
        {
            var retLst = new List<OrderExtendedDto>();

            if (!orderDetails.Any())
            {
                return retLst;
            }

            var uriString = string.Format("/Courts.NET.WS/DBOInfo/Branches");
            var branchDetails = httpClientJson.Do<byte[], List<BranchInfo>>(RequestJson<byte[]>.Create(uriString, WebRequestMethods.Http.Get)).Body;

            var convertedOrderDetails = Mapper.Map<IList<Order>, List<OrderExtendedDto>>(orderDetails);
            var setting = new Settings();
            var countryName = setting.CountryName.ToUpper() == "COUNTRYNAME" ? "UNICOMER GUYANA INC." : setting.CountryName;

            retLst = (from o in convertedOrderDetails
                      join b in branchDetails
                          on o.BranchNo equals b.BranchNumber
                      select new OrderExtendedDto
                      {
                          Id = o.Id,                          
                          CurrentUser = currentUser,
                          TotalAmount = o.TotalAmount,
                          TotalTaxAmount = o.TotalTaxAmount,
                          TotalDiscount = o.TotalDiscount,
                          CreatedBy = o.CreatedBy,
                          CreatedOn = o.CreatedOn,
                          Customer = o.Customer,
                          Items = o.Items,
                          Payments = o.Payments,
                          BranchNo = b.BranchNumber,
                          BranchName = b.BranchName,
                          BranchAddress1 = b.BranchAddress1,
                          BranchAddress2 = b.BranchAddress2,
                          BranchAddress3 = b.BranchAddress3,
                          OriginalOrderId = o.OriginalOrderId,
                          ReceiptType = receiptType,
                          TaxName = setting.TaxName,
                          CountryName = countryName,
                          CompanyTaxNumber = setting.CompanyTaxNumber,
                          TaxRate = setting.TaxRate,
                          IsTaxFreeSale = o.IsTaxFreeSale,
                          IsDutyFreeSale = o.IsDutyFreeSale,
                          SoldBy = o.SoldBy,
                          SalesPerson = httpClientJson.Do<byte[], dynamic>(RequestJson<byte[]>.Create(string.Format("/api/LoginUser/GetUserName/{0}", o.SoldBy), WebRequestMethods.Http.Get)).Body,
                          AgreementInvoiceNumber = o.AgreementInvoiceNumber
                      }).ToList();

            foreach (var order in retLst)
            {
                foreach (var payment in order.Payments)
                {
                    payment.PaymentMethod = ((PaymentTypeEnum)payment.PaymentMethodId).ToString();
                    order.PositiveAmountSum += payment.Amount > 0 ? payment.Amount : 0;
                    order.NegativeAmountSum += payment.Amount < 0 ? payment.Amount : 0;
                }

                if (order.PositiveAmountSum > 0 && order.NegativeAmountSum < 0)
                {
                    order.ChangeGiven = true;
                }
                ArrangeLinkedProducts(order.Items);
            }

            return retLst;
        }

        private void ArrangeLinkedProducts(List<ReceiptReprintDto> retLst)
        {
            var items = new List<ReceiptReprintDto>(retLst);
            retLst.Clear();
            foreach (var item in items)
            {
                if (!item.ParentId.HasValue)
                {
                    retLst.Add(item);
                    retLst.AddRange(items.Where(x => x.ParentId == item.ItemId));
                }
                else
                {
                    if (!retLst.Any(x => x.ItemId == item.ItemId))
                    {
                        retLst.Add(item);
                    }
                }
            }
        }

        private void ArrangeLinkedProducts(List<ItemDto> orderItems)
        {
            var items = new List<ItemDto>(orderItems);

            orderItems.Clear();
            foreach (var item in items)
            {
                if (((item.ItemTypeId == (int)ItemTypeEnum.Product || item.ItemTypeId == (int)ItemTypeEnum.Kit) || item.Returned == true) &&
                    item.ParentId == null)
                {
                    orderItems.Add(item);
                    orderItems.AddRange(items.Where(x => x.ParentId == item.Id));
                }
            }
        }

        #endregion

        #region Hub Message

        private void SendHubMessage(Order newSales, OrderDto saleToSave, WriteScope<Context> scope)
        {
            var hubOrder = GenerateMessageForHub(newSales, saleToSave, scope);
            var hubMessageId = publisher.Publish("Sales.Order", hubOrder);
        }

        private Messages.Order GenerateMessageForHub(Order newSales, OrderDto saleToSave, WriteScope<Context> scope)
        {
            short originalBranchNo = newSales.OriginalOrderId.GetValueOrDefault() > 0 ?
                GetOriginalBranchNo(newSales.OriginalOrderId.Value, scope) : (short)0;
            var tempBranch = originalBranchNo > 0 ? originalBranchNo : saleToSave.BranchNo;
            var accountNo = GetCashAndGoAccountNo(tempBranch);
            var hubOrder = new Messages.Order
            {
                Id = newSales.Id,
                BranchNo = newSales.BranchNo,
                SoldBy = newSales.SoldBy,
                CreatedBy = newSales.CreatedBy,
                CreatedOn = newSales.CreatedOn,
                TotalAmount = newSales.TotalAmount,
                TotalDiscount = newSales.TotalDiscount,
                TotalTaxAmount = newSales.TotalTaxAmount,
                IsTaxFreeSale = newSales.IsTaxFreeSale,
                IsDutyFreeSale = newSales.IsDutyFreeSale,
                Comments = newSales.Comments,
                Customer = Mapper.Map<OrderCustomer, Messages.Customer>(newSales.OrderCustomer),
                Items = GenerateItemsForHub(newSales, saleToSave.Items, scope),
                OriginalOrderIdSpecified = newSales.OriginalOrderId.GetValueOrDefault() > 0,
                OriginalOrderId = newSales.OriginalOrderId ?? 0,
                OriginalBranchNoSpecified = originalBranchNo > 0,
                OriginalBranchNo = originalBranchNo,
                Payments = GeneratePaymentsForHub(newSales.Payments),
                PotentialWarranties = GeneratePotentialWarrantiesForHub(saleToSave),
                CashAndGoAccountNo = accountNo
            };
            return hubOrder;
        }

        private short GetOriginalBranchNo(int originalOrderId, WriteScope<Context> scope)
        {
            var originalOrder = scope.Context.Order.Where(o => o.Id == originalOrderId).SingleOrDefault();

            return originalOrder != null ? originalOrder.BranchNo : (short)0;
        }

        private Messages.PotentialWarranty[] GeneratePotentialWarrantiesForHub(OrderDto saleToSave)
        {
            var itemList = new List<Messages.PotentialWarranty>();
            foreach (var item in saleToSave.Items)
            {
                if (item.PotentialWarranties.Any())
                {
                    foreach (var potentialWarranty in item.PotentialWarranties)
                    {
                        itemList.Add(
                            new Messages.PotentialWarranty
                            {
                                ItemId = item.ProductItemId.Value,
                                ItemNo = item.ItemNo,
                                ItemPrice = item.Price,
                                WarrantyId = potentialWarranty.ProductItemId.Value,
                                WarrantyNumber = potentialWarranty.ItemNo,
                                WarrantyLength = potentialWarranty.WarrantyLengthMonths.Value,
                                WarrantyTaxRate = potentialWarranty.TaxRate,
                                WarrantyCostPrice = potentialWarranty.CostPrice.Value,
                                WarrantyRetailPrice = potentialWarranty.RetailPrice.Value,
                                WarrantySalePrice = potentialWarranty.Price,
                                ItemSerialNo = null,
                                ItemCostPrice = item.CostPrice ?? 0,
                                IsReturned = false,
                                SecondEffort = false,
                                WarrantyTypeCode = potentialWarranty.WarrantyTypeCode,
                                Quantity = potentialWarranty.Quantity,
                            });
                    }
                }
            }
            return itemList.ToArray();
        }

        private string GetCashAndGoAccountNo(int branchNo)
        {
            var uriString = string.Format(
                                          "/Courts.NET.WS/Sales/GetCashAndGoAccountNo?branchNo={0}",
                                          branchNo);

            return httpClientJson.Do<byte[], List<string>>(RequestJson<byte[]>.Create(uriString, WebRequestMethods.Http.Get)).Body.ElementAt(0);
        }

        private Messages.Item[] GenerateItemsForHub(Order newSales, List<ItemDto> itemsToSave, WriteScope<Context> scope)
        {
            var itemList = new List<Messages.Item>();
            foreach (var item in newSales.Items)
            {
                decimal priceDifference = 0;
                if (item.Returned == true && item.ItemTypeId == (int)ItemTypeEnum.Warranty && !(item.IsClaimed ?? false))
                {
                    priceDifference = Math.Abs(item.Price + item.TaxAmount.Value - (item.SalePrice + item.SaleTaxAmount));
                    if (item.ParentItem != null && priceDifference != 0)
                    {
                        item.Department = item.ParentItem.Department;
                    }
                }

                itemList.Add(
                    new Messages.Item
                    {
                        Id = item.Id,
                        ParentIdSpecified = item.ParentId.HasValue,
                        ParentId = item.ParentId ?? Convert.ToByte(0),
                        Description = item.Description,
                        ItemNo = item.ItemNo,
                        ItemTypeId = item.ItemTypeId,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Returned = item.Returned.HasValue && item.Returned.Value,
                        ReturnQuantitySpecified = item.ReturnQuantity.HasValue,
                        ReturnQuantity = item.ReturnQuantity ?? Convert.ToInt16(0),
                        ReturnReason = item.ReturnReason,
                        TaxAmountSpecified = item.TaxAmount.HasValue,
                        TaxAmount = item.TaxAmount ?? 0,
                        TaxRate = item.TaxRate,
                        WarrantyEffectiveDateSpecified = item.WarrantyEffectiveDate.HasValue,
                        WarrantyEffectiveDate = item.WarrantyEffectiveDate ?? new DateTime(),
                        WarrantyLengthMonthsSpecified = item.WarrantyLengthMonths.HasValue,
                        WarrantyLengthMonths = item.WarrantyLengthMonths ?? Convert.ToByte(0),
                        WarrantyLinkIdSpecified = item.WarrantyLinkId.HasValue,
                        WarrantyLinkId = item.WarrantyLinkId ?? 0,
                        WarrantyTypeCode = item.WarrantyTypeCode,
                        ProductItemIdSpecified = item.ProductItemId.HasValue,
                        ProductItemId = item.ProductItemId ?? 0,
                        WarrantyContractNo = item.WarrantyContractNo,
                        ItemUPC = item.ItemUPC,
                        ItemSupplier = item.ItemSupplier,
                        Department = item.Department,
                        CategorySpecified = item.Category.HasValue,
                        Category = item.Category ?? Convert.ToByte(0),
                        Class = item.Class,
                        CostPriceSpecified = item.CostPrice.HasValue,
                        CostPrice = item.CostPrice ?? Convert.ToByte(0),
                        RetailPriceSpecified = item.RetailPrice.HasValue,
                        RetailPrice = item.RetailPrice ?? Convert.ToByte(0),
                        ManualDiscountSpecified = item.ItemTypeId == (int)ItemTypeEnum.Discount,
                        ManualDiscount = item.ManualDiscount ?? 0,
                        ParentItemNo = item.ParentItemNo,
                        PriceDifferenceSpecified = priceDifference != 0,
                        PriceDifference = priceDifference != 0 ? priceDifference : 0,
                        IsClaimed = item.IsClaimed ?? false,
                        SalePrice = item.SalePrice
                    });
            }
            return itemList.ToArray();
        }

        private Messages.Payment[] GeneratePaymentsForHub(ICollection<OrderPayment> payments)
        {
            //return Mapper.Map<ICollection<OrderPayment>, List<Messages.Payment>>(payments).ToArray();
            var paymentsList = new List<Messages.Payment>();
            foreach (var payment in payments)
            {
                paymentsList.Add(
                    new Messages.Payment
                    {
                        MethodId = payment.PaymentMethodId,
                        Amount = payment.Amount,
                        Bank = payment.Bank,
                        CardType = payment.CardType,
                        CardNoSpecified = payment.StoreCardNo.HasValue,
                        CardNo = payment.CardNo ?? Convert.ToInt16(0),
                        ChequeNo = payment.ChequeNo,
                        BankAccountNo = payment.BankAccountNo,
                        CurrencyRateSpecified = payment.CurrencyRate.HasValue,
                        CurrencyRate = payment.CurrencyRate ?? 0,
                        CurrencyAmountSpecified = payment.CurrencyAmount.HasValue,
                        CurrencyAmount = payment.CurrencyAmount ?? 0,
                        StoreCardNoSpecified = payment.StoreCardNo.HasValue,
                        StoreCardNo = payment.StoreCardNo ?? 0,
                        VoucherNo = payment.VoucherNo,
                        CurrencyCode = string.IsNullOrEmpty(payment.CurrencyCode) ? null : payment.CurrencyCode,
                        VoucherIssuer = payment.VoucherIssuer,
                        VoucherIssuerCode = payment.VoucherIssuerCode
                    });
            }
            return paymentsList.ToArray();
        }

        private string GenerateInvoiceNumber(int branchNo)
        {
            var uriString = string.Format("/Courts.NET.WS/Sales/GenerateInvoiceNumber?branchNo={0}", branchNo);

            return httpClientJson.Do<byte[], List<string>>(RequestJson<byte[]>.Create(uriString, WebRequestMethods.Http.Get)).Body.ElementAt(0);
            //return httpClientJson.Do<byte[], string>(RequestJson<byte[]>.Create(uriString, WebRequestMethods.Http.Get)).Body.ElementAt(0);
        }

        #endregion

        //Suvidha
        public int GetInvNum(string agreementinvoicenumber)
        {
            using (var scope = Context.Read())
            {
                var query = (from o in scope.Context.Order
                             where o.AgreementInvoiceNumber == agreementinvoicenumber
                             select o.Id
                            );

                return Convert.ToInt32(query.ToString());
            }
        }

        #region Enums

        public enum PaymentTypeEnum
        {
            Cash = 1,
            ForeignCash,
            StoreCard,
            GiftVoucher,
            Cheque,
            DebitCard,
            StandingOrder,
            TravellersCheque,
            DirectDebit,
            CreditCard
        }

        #endregion
    }
}