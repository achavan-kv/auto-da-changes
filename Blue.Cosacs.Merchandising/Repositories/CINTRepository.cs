using Blue.Cosacs.Merchandising.Calculations;
using Blue.Cosacs.Merchandising.Enums;
using Blue.Cosacs.Merchandising.Models;
using Blue.Cosacs.Merchandising.Models.Cint;
using Blue.Cosacs.Merchandising.Publishers;
using Blue.Cosacs.Merchandising.Solr;
using Blue.Hub.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Message = Blue.Cosacs.Messages.Merchandising.Cints;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface ICINTRepository
    {
        void Create(List<Model.CintOrder> cintMessages, int id, int messageId);
        void Create(Message.CintOrderSubmit cintMessage, int messageId);
    }

    public class CINTRepository : ICINTRepository
    {
        private readonly ICintOrderPublisher cintPublisher;
        private readonly IStockSolrIndexer stockSolrIndexer;
        private readonly Merchandising.Settings merchandisingSettings;
        private readonly ICostRepository costRepository;
        private readonly IPublisher publisher;
        private readonly ICintErrorRepository cintError;
        private readonly IClock clock;

        private readonly decimal repoCostPercent;

        public CINTRepository(
                            ICintOrderPublisher cintPublisher,
                            ICostRepository costRepository,
                            IStockSolrIndexer stockSolrIndexer,
                            Merchandising.Settings merchandisingSettings,
                            IPublisher publisher,
                            ICintErrorRepository cintError,
                            IClock clock)
        {
            this.cintPublisher = cintPublisher;
            this.stockSolrIndexer = stockSolrIndexer;
            this.merchandisingSettings = merchandisingSettings;
            this.costRepository = costRepository;
            this.publisher = publisher;
            this.cintError = cintError;
            this.clock = clock;

            this.repoCostPercent = 1 - (merchandisingSettings.RepossessionCost / 100);
        }

        public void Create(List<Model.CintOrder> cintMessages, int runno, int messageId)
        {
            try
            {
                using (var scope = Context.Write())
                {
                    scope.Context.Database.CommandTimeout = 7200;
                    var msgs = Validate(cintMessages);

                    // Process invalid messages.Put invalid messages on queue.
                    var errorMessages = msgs.Where(m => !string.IsNullOrEmpty(m.Error.ToString())).Select(s => s.ToMessage(runno)).ToList();
                    errorMessages.ForEach(e =>
                    {
                        publisher.Publish<Context, Messages.Merchandising.Cints.CintOrderSubmit>("Merchandising.Cint", e, runno.ToString());
                    });

                    //Process valid messages.

                    var validMessages = msgs.Where(m => string.IsNullOrEmpty(m.Error.ToString()));
                    if (validMessages.Any())
                    {
                        Process(validMessages, runno);
                    }
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
                // TODO - Lets hope the scope above rolls back..
            }
            catch (Exception ex)
            {
                // Everything is lost! The main batch processing has failed. Lets dump everything in to the other queue so some things will process.
                cintMessages.Select(s => s.ToMessage(runno)).ToList().ForEach(e =>
                {
                    publisher.Publish<Context, Messages.Merchandising.Cints.CintOrderSubmit>("Merchandising.Cint", e, runno.ToString());
                });
                cintError.SaveBulkError(new CintsError
                {
                    CreatedOn = clock.Now,
                    Runno = runno,
                    MessageId = messageId,
                    Exception = ex.Message
                });
            }
        }

        public void Create(Message.CintOrderSubmit cintMessage, int messageId)
        {
            try
            {
                using (var scope = Context.Write())
                {
                    scope.Context.Database.CommandTimeout = 7200;
                    var msgs = new List<Model.CintOrder>() { new Model.CintOrder(cintMessage.CintOrder) };
                    msgs = Validate(msgs, single: true);
                    Process(msgs, cintMessage.CintOrder.RunNo);
                    cintError.MarkResolved(messageId);
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
                // Update cint error table with latest.
            }
            catch (Exception ex)
            {
                cintError.SaveError(new List<CintError>
                {
                    new CintError
                    {
                        Date = clock.Now,
                        MessageId = messageId,
                        PrimaryReference = cintMessage.CintOrder.PrimaryReference,
                        ProductCode = cintMessage.CintOrder.Sku,
                        ReferenceType = cintMessage.CintOrder.ReferenceType,
                        RunNo = cintMessage.CintOrder.RunNo,
                        SaleLocation = cintMessage.CintOrder.SaleLocation,
                        SecondaryReference = cintMessage.CintOrder.SecondaryReference,
                        StockLocation = cintMessage.CintOrder.StockLocation,
                        Type = cintMessage.CintOrder.Type,
                        ErrorMessage = ex.Message
                    }
                });
                throw;
            }
        }

        private IEnumerable<Model.CintOrder> SaveCintOrders(IEnumerable<Model.CintOrder> cints, Models.ProductAWCInput input, int runno, IDictionary<int, decimal> costs)
        {
            using (var scope = Context.Write())
            {
                var newCintOrders = cints.Select(s => s.ToTVP(
                                        runno,
                                        input.CostPrice.ContainsKey(s.MerchProductId) ? input.CostPrice[s.MerchProductId].AverageWeightedCost : (decimal?)null,
                                        input.ReposessionProductPrice.ContainsKey(s.MerchProductId) ? input.ReposessionProductPrice[s.MerchProductId] : (decimal?)null,
                                        costs.ContainsKey(s.MerchProductId) ? costs[s.MerchProductId] : 0M));

                var tempIds = scope.Context.CintOrderSave(new CintOrderSaveInput { cintOrder = newCintOrders }).Result.ToDictionary(d => d.TempId, d => d.Id);
                cints.ToList()
                     .ForEach(c =>
                        {
                            c.Id = tempIds[c.TempId];
                        });
                scope.Context.SaveChanges();
                scope.Complete();
            }
            return cints;
        }

        private List<Model.CintOrder> Validate(List<Model.CintOrder> orders, bool single = false)
        {
            using (var scope = Context.Read())
            {
                var locations = scope.Context.Location.Select(s => new { s.SalesId, s.Id }).ToDictionary(d => d.SalesId, d => d.Id);
                var products = orders.Select(s => s.Sku.ToLower()).Distinct();
                var matchingProducts = scope.Context.Product.Where(p => products.Contains(p.SKU)).Select(s => new { sku = s.SKU.ToLower(), s.Id }).ToDictionary(d => d.sku, d => d.Id);
                var primaryReferences = orders.Where(o => o.Type == CintOrderType.Delivery).Select(s => s.PrimaryReference);
                var cintOrders = scope.Context.CintOrder.Where(c => primaryReferences.Contains(c.PrimaryReference)).ToList();

                var productIds = matchingProducts.Select(s => s.Value).ToList();
                var costPriceIds = scope.Context.CostPrice.Where(c => productIds.Contains(c.ProductId)).Select(s => s.ProductId).Distinct().ToList();

                var i = 0;
                orders.ForEach(o =>
                {
                    o.TempId = i++;
                    o.StockLocationId = locations[o.StockLocation];
                    o.SaleLocationId = locations[o.SaleLocation];
                    o.MerchProductId = matchingProducts.ContainsKey(o.Sku.ToLower()) ? matchingProducts[o.Sku.ToLower()] : 0; // Filtered out below. Important we match on new productId.
                    o.QuantityAbs = Math.Abs(o.Quantity);
                });

                //// Check if existing order to overwrite.
                //var lst = (from o in orders
                //           join cint in cintOrders on o.PrimaryReference equals cint.PrimaryReference
                //           where o.Sku == cint.Sku
                //           && o.ParentSku == cint.ParentSku
                //           && o.StockLocation == cint.StockLocation
                //           && cint.Type == CintOrderType.RegularOrder
                //           && o.Type == CintOrderType.RegularOrder
                //           select o).ToList();

                //lst.ForEach(o =>
                // {
                //     o.HasExistingOrder = true;
                // });

                //(from o in orders
                // join o2 in orders on o.PrimaryReference equals o2.PrimaryReference
                // where o.Sku == o2.Sku
                // && o.ParentSku == o2.ParentSku
                // && o.StockLocation == o2.StockLocation
                // && o2.Type == CintOrderType.RegularOrder
                // && o.Type == CintOrderType.RegularOrder
                // select o).ToList().ForEach(o =>
                // {
                //     o.HasExistingOrder = true;
                // });

                // Order in db.
                var dbaseOrder = (from o in orders
                                  join cint in cintOrders on o.PrimaryReference equals cint.PrimaryReference
                                  where ((o.ReferenceType == "Service Request" && o.SecondaryReference == cint.SecondaryReference)
                                      || (o.ReferenceType == "Invoice" && o.SecondaryReference == cint.SecondaryReference)
                                      || (o.ReferenceType == "Delivery"))
                                  && o.Sku == cint.Sku
                                  && o.ParentSku == cint.ParentSku
                                  && o.StockLocation == cint.StockLocation
                                  && cint.Type == CintOrderType.RegularOrder
                                  && o.Type != CintOrderType.RegularOrder
                                  select new { o, cint }).ToList()
                      .Select(s =>
                      {
                          s.o.CashPrice = s.cint.CashPrice;
                          s.o.ParentOrder = s.cint;
                          return s.o.TempId;
                      }).ToList();

                // Order in same batch.
                var hasOrder = (from o in orders
                                join o2 in orders on o.PrimaryReference equals o2.PrimaryReference
                                where ((o.ReferenceType == "Service Request" && o.SecondaryReference == o2.SecondaryReference)
                                    || (o.ReferenceType == "Invoice" && o.SecondaryReference == o2.SecondaryReference)
                                    || (o.ReferenceType == "Delivery"))
                                && o.Sku == o2.Sku
                                && o.ParentSku == o2.ParentSku
                                && o.StockLocation == o2.StockLocation
                                && o2.Type == CintOrderType.RegularOrder
                                && o.Type != CintOrderType.RegularOrder
                                select new { o, o2 }).ToList()
                                .Select(s =>
                                {
                                    s.o.CashPrice = s.o2.CashPrice;
                                    return s.o.TempId;
                                }).ToList();

                orders.ForEach(o =>
                {
                    var errors = new StringBuilder();

                    if (!CintOrderType.OrderTypes().Contains(o.Type))
                    {
                        errors.Append("Unknown order type - " + (string.IsNullOrEmpty(o.Type) ? "NULL" : o.Type) + ".\r\n ");
                    }

                    if (!matchingProducts.ContainsKey(o.Sku.ToLower()))
                    {
                        errors.AppendFormat("Product Sku ({0}) does not exist.\r\n ", string.IsNullOrEmpty(o.Sku) ? "NULL" : o.Sku);
                    }

                    if (o.Type == CintOrderType.RegularOrder && o.Quantity <= 0)
                    {
                        //CR - to reduce poisons in queue 200 where quantity for Regular order appears to be negative
                        // errors.Append("Regular Order must specify a quantity greater than 0.\r\n ");
                        o.Quantity = Math.Abs(o.Quantity);
                    }

                    if (o.Type == CintOrderType.Delivery && o.Quantity <= 0)
                    {
                        errors.Append("Delivery must specify a quantity greater than 0.\r\n ");
                    }

                    if (!locations.ContainsKey(o.SaleLocation))
                    {
                        errors.AppendFormat("Invalid sales location ({0}).\r\n ", string.IsNullOrEmpty(o.SaleLocation) ? "NULL" : o.SaleLocation);
                    }

                    if (!locations.ContainsKey(o.StockLocation))
                    {
                        errors.AppendFormat("Invalid stock location ({0}).\r\n ", string.IsNullOrEmpty(o.StockLocation) ? "NULL" : o.StockLocation);
                    }

                    if (!(o.SaleType == "Credit" || o.SaleType == "Cash"))
                    {
                        errors.AppendFormat("Invalid sale type ({0}).\r\n ", string.IsNullOrEmpty(o.SaleType) ? "NULL" : o.SaleType);
                    }

                    if (o.TransactionDate >= DateTime.UtcNow)
                    {
                        errors.AppendFormat("Transaction date cannot be in the future ({0}).\r\n ", o.TransactionDate);
                    }

                    if (o.Type == CintOrderType.Delivery && !(dbaseOrder.Any(c => c == o.TempId) || hasOrder.Any(h => h == o.TempId)))
                    {
                        errors.Append("There is no corresponding order for this delivery.\r\n ");
                    }

                    if (!costPriceIds.Where(w => w == o.MerchProductId).Any())
                    {
                        errors.AppendFormat("There is no cost price information for this item. Please process PO for {0}.\r\n ", string.IsNullOrEmpty(o.Sku) ? "NULL" : o.Sku);
                    }

                    o.Error = errors.ToString();

                    if (single && !string.IsNullOrEmpty(o.Error))
                    {
                        throw new Exception(errors.ToString());
                    }
                });
                return orders;
            }
        }

        internal IEnumerable<ProductsAwc> CalcAWC(IEnumerable<Model.CintOrder> cintOrders, Models.ProductAWCInput input)
        {
            return cintOrders.Where(w => w.Type == CintOrderType.Repossession)
                                   .GroupBy(order => order.MerchProductId)
                                   .Where(w => w.Sum(s => s.QuantityAbs) > 0)
                                   .Select(product =>
               new ProductsAwc
               {
                   ProductId = product.Key,
                   Awc = AWC.CalculateAWC(
                                    input.CostPrice[product.Key].AverageWeightedCost,
                                    input.ReposessionProductPrice[product.Key],
                                    input.StockOnHand[product.Key],
                                    product.Sum(s => s.QuantityAbs),
                                    merchandisingSettings.RepossessionCost), // Negative to increase stock on hand.
                   LastLandedCost = input.ReposessionProductPrice[product.Key] * repoCostPercent,
                   SupplierCost = 0,
                   Currency = merchandisingSettings.LocalCurrency
               }).ToList();
        }

        private Tuple<IEnumerable<UpdateProductStockLevelTVP>, IEnumerable<UpdateCintOrderStatsTVP>> CalculateUpdateStock(IEnumerable<Model.CintOrder> cintOrders, IDictionary<int, int> oldQuantity)
        {
            var productStocks = new List<UpdateProductStockLevelTVP>();
            var orderStats = new List<UpdateCintOrderStatsTVP>();

            // Build up TVP for database call later.
            cintOrders.Where(c => c.Type != CintOrderType.RegularOrder).ToList().ForEach(o =>
            {
                var productStock = new UpdateProductStockLevelTVP
                {
                    LocationId = o.StockLocationId,
                    ProductId = o.MerchProductId
                };

                var orderStat = new UpdateCintOrderStatsTVP
                {
                    ParentSku = o.ParentSku,
                    PrimaryReference = o.PrimaryReference,
                    ReferenceType = o.ReferenceType,
                    SecondaryReference = o.SecondaryReference,
                    Sku = o.Sku,
                    StockLocation = o.StockLocation
                };

                // All types 
                switch (o.Type)
                {
                    case CintOrderType.CancelOrder:
                        {
                            // for a cancel order the qty isnt sent so we need to calculate what is actually left on the order and cancel that
                            orderStat.QtyOrderedInc = -o.QuantityAbs;
                            productStock.StockAvailable = o.QuantityAbs;
                            break;
                        }
                    case CintOrderType.Delivery:
                        {
                            orderStat.QtyDeliveredInc = o.QuantityAbs;
                            productStock.StockOnHand = -o.QuantityAbs;
                            break;
                        }
                    case CintOrderType.Redelivery:
                        {
                            // redelivery is equivalent to an order and a delivery - occurs for repossessed stock being returned to customer
                            // on a new sku
                            orderStat.QtyOrderedInc = o.QuantityAbs;
                            orderStat.QtyDeliveredInc = o.QuantityAbs;
                            productStock.StockOnHand = -o.QuantityAbs;
                            productStock.StockAvailable = -o.QuantityAbs;

                            break;
                        }
                    case CintOrderType.Return:
                        {
                            // quantity is negative to represent taking away from customer
                            orderStat.QtyReturnedInc = o.QuantityAbs;
                            productStock.StockAvailable = o.QuantityAbs;
                            productStock.StockOnHand = o.QuantityAbs;
                            break;
                        }
                    case CintOrderType.Repossession:
                        {
                            // quantity is negative to represent taking away from customer
                            orderStat.QtyRepossessedInc = o.QuantityAbs;
                            productStock.StockAvailable = o.QuantityAbs;
                            productStock.StockOnHand = o.QuantityAbs;
                            break;
                        }
                    default:
                        throw new Exception("Not found type " + o.Type);
                }
                productStocks.Add(productStock);
                orderStats.Add(orderStat);
            });

            // Orders - Find the latest order.
            var productOrders = cintOrders.Where(c => c.Type == CintOrderType.RegularOrder).GroupBy(g =>
                                          new
                                          {
                                              g.PrimaryReference,
                                              g.Sku,
                                              g.ParentSku,
                                              g.StockLocation
                                          }).Select(s => s.OrderByDescending(d => d.TransactionDate).First()).ToList();

            // Add latest or new order to products.
            productStocks.AddRange(
                productOrders.Select(s => new UpdateProductStockLevelTVP
                {
                    LocationId = s.StockLocationId,
                    ProductId = s.MerchProductId,
                    StockAvailable = (oldQuantity.ContainsKey(s.TempId) ? oldQuantity[s.TempId] : 0) - s.Quantity
                }));

            // Add latest order to cint order stats.
            orderStats.AddRange(
            productOrders.Select(s =>
            new UpdateCintOrderStatsTVP
            {
                ParentSku = s.ParentSku,
                PrimaryReference = s.PrimaryReference,
                ReferenceType = s.ReferenceType,
                SecondaryReference = s.SecondaryReference,
                Sku = s.Sku,
                StockLocation = s.StockLocation,
                QtyOrdered = s.Quantity
            }));

            // Aggregate by the key for Merge statement in SQL.
            var totalProductsStocks = productStocks.GroupBy(g => new { g.ProductId, g.LocationId })
                                                   .Select(s =>
                                                   new UpdateProductStockLevelTVP
                                                   {
                                                       ProductId = s.Key.ProductId,
                                                       LocationId = s.Key.LocationId,
                                                       StockAvailable = s.Sum(a => a.StockAvailable),
                                                       StockOnHand = s.Sum(a => a.StockOnHand),
                                                       StockOnOrder = s.Sum(a => a.StockOnOrder)
                                                   });

            var totalOrderStats = orderStats.GroupBy(g => new
            {
                g.ParentSku,
                g.PrimaryReference,
                g.ReferenceType,
                g.SecondaryReference,
                g.Sku,
                g.StockLocation
            }).Select(s => new UpdateCintOrderStatsTVP
            {
                ParentSku = s.Key.ParentSku,
                PrimaryReference = s.Key.PrimaryReference,
                ReferenceType = s.Key.ReferenceType,
                SecondaryReference = s.Key.SecondaryReference,
                Sku = s.Key.Sku,
                StockLocation = s.Key.StockLocation,
                QtyOrdered = s.Sum(a => a.QtyOrdered),
                QtyDeliveredInc = s.Sum(a => a.QtyDeliveredInc),
                QtyOrderedInc = s.Sum(a => a.QtyOrderedInc),
                QtyRepossessedInc = s.Sum(a => a.QtyRepossessedInc),
                QtyReturnedInc = s.Sum(a => a.QtyReturnedInc)
            });

            return new Tuple<IEnumerable<UpdateProductStockLevelTVP>, IEnumerable<UpdateCintOrderStatsTVP>>(totalProductsStocks, totalOrderStats);
        }

        private Models.ProductAWCInput GetAWCInput(IEnumerable<Model.CintOrder> cintOrders)
        {
            using (var scope = Context.Read())
            {
                var costProductIds = cintOrders.Select(s => s.MerchProductId).Distinct();
                var repoProductIds = cintOrders.Where(c => c.Type == CintOrderType.Repossession)
                                               .Select(s => s.MerchProductId).Distinct();

                var costPrice = scope.Context.CurrentCostPriceView.Where(cp => costProductIds.Contains(cp.ProductId))
                                                                  .Select(s => new Models.GeneralCostPrice()
                                                                  {
                                                                      ProductId = s.ProductId,
                                                                      SupplierCost = s.SupplierCost,
                                                                      AverageWeightedCost = s.AverageWeightedCost,
                                                                      LastLandedCost = s.LastLandedCost,
                                                                      Currency = s.SupplierCurrency
                                                                  })
                                                                  .ToDictionary(d => d.ProductId);

                var prices = scope.Context.CurrentRetailPriceView.Where(r => repoProductIds.Contains(r.ProductId) &&
                                                                                              r.LocationId == null &&
                                                                                              r.Fascia == "Courts")
                                                                                .Select(s => new { s.CashPrice, s.ProductId })
                                                                                .ToList();
                if (repoProductIds.Any())
                {
                    var exceptions = repoProductIds.Except(prices.Select(s => s.ProductId));
                    var localPrices = scope.Context.CurrentStockPriceByLocationView.Where(c => exceptions.Contains(c.ProductId)).ToList()
                                                                           .Select(s => new { s.CashPrice, s.ProductId });

                    prices.AddRange(localPrices);
                }
                var stockOnHand = scope.Context.ProductStockLevel
                                               .Where(s => costProductIds.Contains(s.ProductId))
                                               .GroupBy(g => g.ProductId)
                                               .Select(s => new { productId = s.Key, stockOnHand = s.Sum(h => h.StockOnHand) })
                                               .ToDictionary(d => d.productId, d => d.stockOnHand);

                return new Models.ProductAWCInput
                {
                    CostPrice = costPrice,
                    ReposessionProductPrice = prices.ToDictionary(d => d.ProductId, d => d.CashPrice ?? 0),
                    StockOnHand = stockOnHand
                };
            }
        }

        private IDictionary<int, int> GetOrderQuantity(IEnumerable<Model.CintOrder> cintOrder)
        {
            using (var scope = Context.Read())
            {
                var quants = scope.Context.CintOrderStatsGetQuantity(new CintOrderStatsGetQuantityInput
                {
                    stats = cintOrder.Select(s =>
                                new CintOrderStatsTVP
                                {
                                    OrderId = s.TempId,
                                    ParentSku = s.ParentSku,
                                    PrimaryReference = s.PrimaryReference,
                                    SecondaryReference = s.SecondaryReference,
                                    Sku = s.Sku,
                                    StockLocation = s.StockLocation,
                                    ReferenceType = s.ReferenceType
                                })
                });
                return quants.Result.ToDictionary(d => d.TempId, d => d.Quantity);
            }
        }

        private decimal GetAWC(int productId, IDictionary<int, decimal> productAWC, Dictionary<int, GeneralCostPrice> costPrice)
        {
            // Get AWC if recalc 
            if (productAWC.ContainsKey(productId))
            {
                return productAWC[productId];
            }
            // If not recalc get existing.
            if (costPrice.ContainsKey(productId))
            {
                return costPrice[productId].AverageWeightedCost;
            }
            // Not found? Probably should never get here. 
            return 0m;
        }

        private void Process(IEnumerable<Model.CintOrder> cintOrders, int runno)
        {
            using (var scope = Context.Write())
            {
                var productData = GetAWCInput(cintOrders);
                var updatedAWC = CalcAWC(cintOrders, productData);

                cintOrders = SaveCintOrders(cintOrders, productData, runno, updatedAWC.ToDictionary(d => d.ProductId, d => d.LastLandedCost));
                var stockUpdate = CalculateUpdateStock(cintOrders, GetOrderQuantity(cintOrders));

                SaveCostPrice(updatedAWC);
                scope.Context.UpdateCintOrderStats(new UpdateCintOrderStatsInput
                {
                    stats = stockUpdate.Item2
                });

                scope.Context.UpdateProductStockLevel(new UpdateProductStockLevelInput
                {
                    productStock = stockUpdate.Item1
                });

                var productAWC = updatedAWC.ToDictionary(d => d.ProductId, d => d.Awc);

                cintOrders.ToList().ForEach(c =>
                {
                    this.Publish(c, GetAWC(c.MerchProductId, productAWC, productData.CostPrice), runno);
                });
                scope.Context.SaveChanges();
                stockSolrIndexer.Index(cintOrders.Select(c => c.MerchProductId).Distinct());
                scope.Complete();
            }
        }

        private void SaveCostPrice(IEnumerable<ProductsAwc> awcs)
        {
            using (var scope = Context.Write())
            {
                scope.Context.CostPrice.AddRange(awcs.Select(a =>
                    new CostPrice
                    {
                        AverageWeightedCost = a.Awc,
                        AverageWeightedCostUpdated = DateTime.UtcNow,
                        LastLandedCost = a.LastLandedCost,
                        LastLandedCostUpdated = DateTime.UtcNow,
                        ProductId = a.ProductId,
                        SupplierCost = a.SupplierCost,
                        SupplierCurrency = a.Currency
                    }));
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void Publish(Model.CintOrder cintOrder, decimal awc, int runno)
        {
            switch (cintOrder.Type)
            {
                case CintOrderType.Delivery:
                    {
                        this.cintPublisher.PublishDelivered(cintOrder, awc, runno);
                        break;
                    }
                case CintOrderType.Repossession:
                case CintOrderType.Return:
                case CintOrderType.Redelivery:
                    {
                        this.cintPublisher.PublishCostOfSale(cintOrder, awc, runno, cintOrder.ParentOrder);
                        break;
                    }
            }
        }
    }
}