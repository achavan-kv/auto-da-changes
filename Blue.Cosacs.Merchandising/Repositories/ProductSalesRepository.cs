namespace Blue.Cosacs.Merchandising.Repositories
{
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context = Blue.Cosacs.Merchandising.Context;
    using Blue.Transactions;

    public interface IProductSalesRepository
    {
        ProductSalesViewModel Get(int productId, bool isSOLRIndex, int? locationId = null);
        List<ProductSalesViewModel> Get(IEnumerable<int> productIds, bool isSOLRIndex);
        List<ProductSalesViewModel> Get(IEnumerable<int> productIds, ReadScope<Context> scope, bool isSOLRIndex);
    }

    public class ProductSalesRepository : IProductSalesRepository
    {
        private readonly ILocationRepository locationRepository;

        public ProductSalesRepository(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }

        public ProductSalesViewModel Get(int productId, bool isSOLRIndex, int? locationId = null)
        {
            using (var scope = Context.Read())
            {
                return GetData(new List<int> { productId }, scope, isSOLRIndex, locationId).FirstOrDefault();
            }
        }

        public List<ProductSalesViewModel> Get(IEnumerable<int> productIds, bool isSOLRIndex)
        {
            using (var scope = Context.Read())
            {
                return this.GetData(productIds, scope, isSOLRIndex);
            }
        }

        public List<ProductSalesViewModel> Get(IEnumerable<int> productIds, ReadScope<Context> scope, bool isSOLRIndex)
        {
            return this.GetData(productIds, scope, isSOLRIndex);
        }

        private List<ProductSalesViewModel> GetData(IEnumerable<int> productIds, ReadScope<Context> scope, bool isSOLRIndex, int? locationId = null)
        {
            // Remember all these end dates are not inclusive.
            var now = DateTime.Today;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            var thisMonthEnd = now.AddDays(1);

            var lastMonth = now.AddMonths(-1);
            var lastMonthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var lastMonthEnd = lastMonthStart.AddMonths(1);

            var thisYtdStart = new DateTime(now.Month >= 4 ? now.Year : now.Year - 1, 4, 1);
            var lastYtdStart = thisYtdStart.AddYears(-1);
            var lastYtdEnd = now.AddYears(-1).AddDays(1);

            List<string> skus;

            var prod = scope.Context.Product.AsNoTracking();


            var salesQuery = scope.Context.SalesView.AsNoTracking().AsQueryable();
            var cint = scope.Context.CintOrder.AsNoTracking().AsQueryable();

            if (productIds != null)
            {
                skus = prod.Where(s => productIds.Contains(s.Id)).Select(s => s.SKU).ToList();
                salesQuery = salesQuery.Where(s => productIds.Contains(s.ProductId));
                cint = cint.Where(c => skus.Contains(c.Sku));
            }


            if (locationId.HasValue)
            {
                salesQuery = salesQuery
                    .Where(o => o.StockLocationId == locationId);
            }

            var sales = salesQuery
                .Select(s => new ProductSalesViewModel
                {
                    ProductId = s.ProductId,
                    Sku = s.Sku,
                    Location = s.StockLocation,
                    LocationId = s.StockLocationId ?? 0
                })
                .ToList();

            //Product Maintenance Page - Deliveries do not affect the Allocations Count
            //Quantity Allocated = RegularOrder + CacelOrder
            //Quantity Sold = Delivery - Return
            var cintOrders = cint.Where(o =>
                    o.TransactionDate >= lastYtdStart
                    && (o.Type == CintOrderType.RegularOrder || o.Type == CintOrderType.CancelOrder || o.Type == CintOrderType.Delivery || o.Type == CintOrderType.Return)
                    && o.Quantity != 0)
                .Select(p => new CintOrdersGroup
                {
                    Sku = p.Sku,
                    TransactionDate = p.TransactionDate,
                    Quantity = p.Quantity,
                    CintOrderType = (byte)(p.Type == CintOrderType.RegularOrder || p.Type == CintOrderType.CancelOrder ? 0 : 1)
                }).ToList();

            //For the SOLR index(Product Enquiry Page) subtract Deliveries from Allocations
            //Quantity Allocated = RegularOrder + CacelOrder - Delivery
            //Quantity Sold = Delivery - Return
            if (isSOLRIndex)
            {
                //Add deliveries with a negative quantity to the Allocated Value
                cintOrders.AddRange(cint.Where(o =>
                                               o.TransactionDate >= lastYtdStart
                                               && o.Type == CintOrderType.Delivery
                                               && o.Quantity != 0)
                                        .Select(p => new CintOrdersGroup
                                        {
                                            Sku = p.Sku,
                                            TransactionDate = p.TransactionDate,
                                            Quantity = -p.Quantity,
                                            CintOrderType = (byte)0
                                        }).ToList()
                    );
            }

            Func<List<CintOrdersGroup>, DateTime, DateTime, IDictionary<Tuple<string, byte>, int>> SumOrders = (orders, start, end) =>
            {
                return orders.Where(o => o.TransactionDate >= start &&
                                         o.TransactionDate < end).GroupBy(g => new Tuple<string, byte>(g.Sku, g.CintOrderType))
                                         .ToDictionary(d => d.Key, d => d.Sum(s => s.Quantity));
            };

            var thisPeriod = SumOrders(cintOrders, thisMonthStart, thisMonthEnd);
            var lastPeriod = SumOrders(cintOrders, lastMonthStart, lastMonthEnd);
            var thisYTD = SumOrders(cintOrders, thisYtdStart, thisMonthEnd);
            var lastYTD = SumOrders(cintOrders, lastYtdStart, lastYtdEnd);

            Func<IDictionary<Tuple<string, byte>, int>, string, byte, int> getValue = (orders, sku, type) =>
            {
                return orders.ContainsKey(new Tuple<string, byte>(sku, type)) ? orders[new Tuple<string, byte>(sku, type)] : 0;
            };

            sales.ForEach(s =>
            {
                s.Sales.Add(new ProductSalesModel
                {
                    Type = "Allocated (Booked)",
                    ThisPeriod = getValue(thisPeriod, s.Sku, 0),
                    LastPeriod = getValue(lastPeriod, s.Sku, 0),
                    ThisYTD = getValue(thisYTD, s.Sku, 0),
                    LastYTD = getValue(lastYTD, s.Sku, 0),
                });

                s.Sales.Add(new ProductSalesModel
                {
                    Type = "Sales (Delivered)",
                    ThisPeriod = getValue(thisPeriod, s.Sku, 1),
                    LastPeriod = getValue(lastPeriod, s.Sku, 1),
                    ThisYTD = getValue(thisYTD, s.Sku, 1),
                    LastYTD = getValue(lastYTD, s.Sku, 1),
                });

            });
            return sales;
        }

        private class CintOrdersGroup
        {
            public string Sku { get; set; }
            public DateTime TransactionDate { get; set; }
            public int Quantity { get; set; }
            public byte CintOrderType { get; set; }
        }
    }
}