using Blue.Cosacs.Merchandising.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models.RP3Export;
    using System.Data.Entity;
    using Context = Blue.Cosacs.Merchandising.Context;

    public interface IRP3ExportRepository
    {
        List<LocationExportModel> GetLocations();

        List<ProductExportModel> GetProducts();

        List<PurchaseOrderProductExportModel> GetPurchaseOrderDetail(DateTime transactionsFromDate);

        List<PurchaseOrderExportModel> GetPurchaseOrders(DateTime transactionsFromDate);

        List<VendorReturnExportModel> GetVendorReturns(DateTime transactionsFromDate, DateTime rp3GoLiveDate);

        List<VendorReturnProductExportModel> GetVendorReturnProducts(DateTime transactionsFromDate, DateTime rp3GoLiveDate);

        List<GoodsReceiptExportModel> GetGoodsReceipts(DateTime transactionsFromDate, DateTime rp3GoLiveDate);

        List<GoodsReceiptProductExportModel> GetGoodsReceiptProducts(DateTime transactionsFromDate, DateTime rp3GoLiveDate);

        List<StockAdjustmentExportModel> GetStockAdjustments(DateTime transactionsFromDate, DateTime rp3GoLiveDate);

        List<StockAdjustmentProductExportModel> GetStockAdjustmentProducts(DateTime transactionsFromDate, DateTime rp3GoLiveDate);

        List<StockTransferExportModel> GetStockTransfers(DateTime transactionsFromDate, DateTime rp3GoLiveDate);

        List<WeeklyMerchandisingReportExportModel> GetWeeklyMerchandisingReport();

        List<StockTransferProductExportModel> GetStockTransferProducts(DateTime transactionsFromDate, DateTime rp3GoLiveDate);
    }

    public class RP3ExportRepository : IRP3ExportRepository
    {
        private readonly Blue.Config.Repositories.Settings settingsRepo;

        private readonly ILocationRepository locationRepository;

        public RP3ExportRepository(Blue.Config.Repositories.Settings settingsRepo, ILocationRepository locationRepository)
        {
            this.settingsRepo = settingsRepo;
            this.locationRepository = locationRepository;
        }

        public List<LocationExportModel> GetLocations()
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var locations = locationRepository.Get().Select(Mapper.Map<LocationExportModel>).ToList();
                locations.ForEach(l => l.Company = this.GetCompany());

                return locations;
            }
        }

        public List<ProductExportModel> GetProducts()
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var products = Mapper.Map<List<ProductModel>>(scope.Context.RP3ProductExportView.AsNoTracking().ToList()
                    .Select(s =>
                    {
                        s.CreationDate = s.ExternalCreationDate.HasValue ? s.ExternalCreationDate.Value : s.CreationDate.ToLocalDateTime();
                        s.LastTransactionDate = s.LastTransactionDate.ToLocalDateTime();
                        return s;
                    }).ToList());
                var exportProducts = Mapper.Map<List<ProductExportModel>>(products);
                return exportProducts.Select(l => { l.Company = this.GetCompany(); return l; }).ToList();
            }
        }

        public List<PurchaseOrderProductExportModel> GetPurchaseOrderDetail(DateTime transactionsFromDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var purchaseOrderProducts = scope.Context.RP3PurchaseOrderProductView.AsNoTracking().ToList()
                                            .Select(p => { p.CreatedDate = p.CreatedDate.ToLocalDateTime(); return p; }).ToList();

                var exportPurchaseOrderProducts = Mapper.Map<List<PurchaseOrderProductExportModel>>(purchaseOrderProducts);
                exportPurchaseOrderProducts.ForEach(l => l.Company = this.GetCompany());

                return exportPurchaseOrderProducts;
            }
        }

        public List<PurchaseOrderExportModel> GetPurchaseOrders(DateTime transactionsFromDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK

                var purchaseOrders = scope.Context.RP3PurchaseOrderView.AsNoTracking().ToList()
                                     .Select(p => { p.TransactionDate.ToLocalDateTime(); return p; }).ToList();

                var exportPurchaseOrders = Mapper.Map<List<PurchaseOrderExportModel>>(purchaseOrders);
                exportPurchaseOrders.ForEach(l => l.Company = this.GetCompany());

                return exportPurchaseOrders;
            }
        }

        public List<VendorReturnExportModel> GetVendorReturns(DateTime transactionsFromDate, DateTime rp3GoLiveDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var fromDate = transactionsFromDate.ToUniversalTime();
                var vendorReturns = scope.Context.RP3VendorReturnView.Where(vr => vr.TransactionDate >= fromDate && vr.TransactionDate > rp3GoLiveDate).AsNoTracking().ToList()
                                    .Select(v => { v.TransactionDate = v.TransactionDate.ToLocalDateTime(); return v; }).ToList();
                var exportVendorReturns = Mapper.Map<List<VendorReturnExportModel>>(vendorReturns);
                exportVendorReturns.ForEach(l => l.Company = this.GetCompany());

                return exportVendorReturns;
            }
        }

        public List<VendorReturnProductExportModel> GetVendorReturnProducts(DateTime transactionsFromDate, DateTime rp3GoLiveDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var fromDate = transactionsFromDate.ToUniversalTime();
                var vendorReturnProducts = scope.Context.RP3VendorReturnProductView.Where(vr => vr.TransactionDate >= fromDate && vr.TransactionDate > rp3GoLiveDate).AsNoTracking().ToList();

                var exportVendorReturnProducts = Mapper.Map<List<VendorReturnProductExportModel>>(vendorReturnProducts);
                exportVendorReturnProducts.ForEach(l => l.Company = this.GetCompany());

                return exportVendorReturnProducts;
            }
        }

        public List<GoodsReceiptExportModel> GetGoodsReceipts(DateTime transactionsFromDate, DateTime rp3GoLiveDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var fromDate = transactionsFromDate.ToUniversalTime();
                var goodsReceipts = scope.Context.RP3GoodsReceiptView.Where(gr => gr.TransactionDate >= fromDate && gr.TransactionDate > rp3GoLiveDate).AsNoTracking().ToList();

                var exportGoodsReceipts = Mapper.Map<List<GoodsReceiptExportModel>>(goodsReceipts);
                exportGoodsReceipts.ForEach(l => l.Company = this.GetCompany());

                return exportGoodsReceipts;
            }
        }

        public List<GoodsReceiptProductExportModel> GetGoodsReceiptProducts(DateTime transactionsFromDate, DateTime rp3GoLiveDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var fromDate = transactionsFromDate.ToUniversalTime();
                var goodsReceiptProducts = scope.Context.RP3GoodsReceiptProductView.Where(gr => gr.CreatedDate >= fromDate && gr.CreatedDate > rp3GoLiveDate && gr.UnitsReceived > 0)
                                           .AsNoTracking().ToList()
                                           .Select(v => { v.CreatedDate = v.CreatedDate.ToLocalDateTime(); return v; }).ToList();

                var exportGoodsReceiptProducts = Mapper.Map<List<GoodsReceiptProductExportModel>>(goodsReceiptProducts);
                exportGoodsReceiptProducts.ForEach(l => l.Company = this.GetCompany());

                return exportGoodsReceiptProducts;
            }
        }

        public List<StockAdjustmentExportModel> GetStockAdjustments(DateTime transactionsFromDate, DateTime rp3GoLiveDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var fromDate = transactionsFromDate.ToUniversalTime();
                var stockAdjustments = scope.Context.RP3StockAdjustmentView.Where(s => s.TransactionDate >= fromDate && s.TransactionDate > rp3GoLiveDate).AsNoTracking().ToList()
                                             .Select(v => { v.TransactionDate = v.TransactionDate.ToLocalDateTime(); return v; }).ToList();

                var exportStockAdjustments = Mapper.Map<List<StockAdjustmentExportModel>>(stockAdjustments);
                exportStockAdjustments.ForEach(l => l.Company = this.GetCompany());

                return exportStockAdjustments;
            }
        }

        public List<StockAdjustmentProductExportModel> GetStockAdjustmentProducts(DateTime transactionsFromDate, DateTime rp3GoLiveDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var fromDate = transactionsFromDate.ToUniversalTime();
                var stockAdjustmentProducts = scope.Context.RP3StockAdjustmentProductView.Where(s => s.CreatedDate >= fromDate && s.CreatedDate > rp3GoLiveDate).AsNoTracking().ToList()
                                            .Select(v => { v.CreatedDate = v.CreatedDate.ToLocalDateTime(); return v; }).ToList();

                var exportStockAdjustmentProducts = Mapper.Map<List<StockAdjustmentProductExportModel>>(stockAdjustmentProducts);
                exportStockAdjustmentProducts.ForEach(l => l.Company = this.GetCompany());

                return exportStockAdjustmentProducts;
            }
        }

        public List<StockTransferExportModel> GetStockTransfers(DateTime transactionsFromDate, DateTime rp3GoLiveDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var fromDate = transactionsFromDate.ToUniversalTime();
                var stockTransfers = scope.Context.RP3StockTransferView.Where(s => s.TransactionDate >= fromDate && s.TransactionDate > rp3GoLiveDate).AsNoTracking().ToList()
                                        .Select(v => { v.TransactionDate = v.TransactionDate.ToLocalDateTime(); return v; }).ToList();

                var exportStockTransfers = Mapper.Map<List<StockTransferExportModel>>(stockTransfers);
                exportStockTransfers.ForEach(l => l.Company = this.GetCompany());

                return exportStockTransfers;
            }
        }

        public List<StockTransferProductExportModel> GetStockTransferProducts(DateTime transactionsFromDate, DateTime rp3GoLiveDate)
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK
                var fromDate = transactionsFromDate.ToUniversalTime();
                var stockTranferProducts = scope.Context.RP3StockTransferProductView.Where(s => s.DateProcessed >= fromDate && s.DateProcessed > rp3GoLiveDate).AsNoTracking().ToList()
                                              .Select(v => { v.DateProcessed = v.DateProcessed.ToLocalDateTime(); return v; }).ToList();

                var exportStockTransferProducts = Mapper.Map<List<StockTransferProductExportModel>>(stockTranferProducts);
                exportStockTransferProducts.ForEach(l => l.Company = this.GetCompany());

                return exportStockTransferProducts;
            }
        }

        public List<WeeklyMerchandisingReportExportModel> GetWeeklyMerchandisingReport()
        {
            //Go live date parameter in views as the way these were done
            using (var scope = Context.Read())
            {
                scope.Context.Database.CommandTimeout = 7200; // HACK

                var previous = scope.Context.RP3PreviousWeeklyMerchandisingView
                    .AsNoTracking()
                    .ToList()
                    .Select(p => new WeeklyMerchandisingReportExportModel
                    {
                        Country = p.Country,
                        ACYear = p.ACYear.ToString(),
                        ACMonth = p.ACMonth.ToString(),
                        Store = p.Store,
                        ProdCat = p.DivisionCode,
                        CatDesc = p.DivisionName,
                        DeptCode = p.DepartmentCode,
                        DeptDesc = p.DepartmentName,
                        BrandCode = p.BrandCode,
                        BrandDesc = p.BrandName,
                        VendorCode = p.SupplierCode,
                        VendorName = p.SupplierName,
                        Style = p.VendorStyleLong,
                        Product = p.Sku,
                        ProdDesc = p.LongDescription,
                        InitSUnits = p.InitialStockUnits.HasValue ? p.InitialStockUnits.Value : 0,
                        InitSValue = p.InitialStockValue.HasValue ? p.InitialStockValue.Value : 0,
                        InitSSales = p.InitialStockCost.HasValue ? p.InitialStockCost.Value : 0,
                        PurchUnits = p.PurchUnits.HasValue ? p.PurchUnits.Value : 0,
                        PurchValue = p.PurchValue.HasValue ? p.PurchValue.Value : 0,
                        PurchSales = p.PurchSales.HasValue ? p.PurchSales.Value : 0,
                        UnitSales = p.UnitSales.HasValue ? p.UnitSales.Value : 0,
                        CostSales = p.CostSales.HasValue ? p.CostSales.Value : 0,
                        RetailSales = p.RetailSales.HasValue ? p.RetailSales.Value : 0,
                        UnitAdj = p.UnitAdj.HasValue ? p.UnitAdj.Value : 0,
                        CostAdj = p.CostAdj.HasValue ? p.CostAdj.Value : 0,
                        SalesAdj = p.SalesAdj.HasValue ? p.SalesAdj.Value : 0,
                        UnitTrans = p.UnitTrans.HasValue ? p.UnitTrans.Value : 0,
                        CostTrans = p.CostTrans.HasValue ? p.CostTrans.Value : 0,
                        SalesTrans = p.SalesTrans.HasValue ? p.SalesTrans.Value : 0,
                        FinalSUnits = p.FinalStockUnits.HasValue ? p.FinalStockUnits.Value : 0,
                        FinalSSales = p.FinalStockCost.HasValue ? p.FinalStockCost.Value : 0,
                        FinalSValue = p.FinalStockValue.HasValue ? p.FinalStockValue.Value : 0,
                        MarkUp = 0,
                        MarkDown = p.Discount.HasValue ? p.Discount.Value : 0,
                        LastPDate = p.LastPDate.ToLocalDateTime().ToString("yyyy/MM/dd"),
                        LastSDate = p.LastSDate.ToLocalDateTime().ToString("yyyy/MM/dd"),
                        LastTDate = p.LastTDate.ToLocalDateTime().ToString("yyyy/MM/dd"),
                        FirstRDate = p.FirstRDate.ToLocalDateTime().ToString("yyyy/MM/dd")
                    });

                var current = scope.Context.RP3CurrentWeeklyMerchandisingView
                    .AsNoTracking()
                    .ToList()
                    .Select(p => new WeeklyMerchandisingReportExportModel
                    {
                        Country = p.Country,
                        ACYear = p.ACYear.ToString(),
                        ACMonth = p.ACMonth.ToString(),
                        Store = p.Store,
                        ProdCat = p.DivisionCode,
                        CatDesc = p.DivisionName,
                        DeptCode = p.DepartmentCode,
                        DeptDesc = p.DepartmentName,
                        BrandCode = p.BrandCode,
                        BrandDesc = p.BrandName,
                        VendorCode = p.SupplierCode,
                        VendorName = p.SupplierName,
                        Style = p.VendorStyleLong,
                        Product = p.Sku,
                        ProdDesc = p.LongDescription,
                        InitSUnits = p.InitialStockUnits.HasValue ? p.InitialStockUnits.Value : 0,
                        InitSValue = p.InitialStockValue.HasValue ? p.InitialStockValue.Value : 0,
                        InitSSales = p.InitialStockCost.HasValue ? p.InitialStockCost.Value : 0,
                        PurchUnits = p.PurchUnits.HasValue ? p.PurchUnits.Value : 0,
                        PurchValue = p.PurchValue.HasValue ? p.PurchValue.Value : 0,
                        PurchSales = p.PurchSales.HasValue ? p.PurchSales.Value : 0,
                        UnitSales = p.UnitSales.HasValue ? p.UnitSales.Value : 0,
                        CostSales = p.CostSales.HasValue ? p.CostSales.Value : 0,
                        RetailSales = p.RetailSales.HasValue ? p.RetailSales.Value : 0,
                        UnitAdj = p.UnitAdj.HasValue ? p.UnitAdj.Value : 0,
                        CostAdj = p.CostAdj.HasValue ? p.CostAdj.Value : 0,
                        SalesAdj = p.SalesAdj.HasValue ? p.SalesAdj.Value : 0,
                        UnitTrans = p.UnitTrans.HasValue ? p.UnitTrans.Value : 0,
                        CostTrans = p.CostTrans.HasValue ? p.CostTrans.Value : 0,
                        SalesTrans = p.SalesTrans.HasValue ? p.SalesTrans.Value : 0,
                        FinalSUnits = p.FinalStockUnits.HasValue ? p.FinalStockUnits.Value : 0,
                        FinalSSales = p.FinalStockCost.HasValue ? p.FinalStockCost.Value : 0,
                        FinalSValue = p.FinalStockValue.HasValue ? p.FinalStockValue.Value : 0,
                        MarkUp = 0,
                        MarkDown = p.Discount.HasValue ? p.Discount.Value : 0,
                        LastPDate = p.LastPDate.ToLocalDateTime().ToString("yyyy/MM/dd"),
                        LastSDate = p.LastSDate.ToLocalDateTime().ToString("yyyy/MM/dd"),
                        LastTDate = p.LastTDate.ToLocalDateTime().ToString("yyyy/MM/dd"),
                        FirstRDate = p.FirstRDate.ToLocalDateTime().ToString("yyyy/MM/dd")
                    });

                return previous
                    .Union(current)
                    .ToList();
            }
        }

        private string GetCompany()
        {
            return string.Format("{0}_UNICOMER", this.settingsRepo.Get("ISOCountryCode"));
        }
    }
}
