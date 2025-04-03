using Blue.Cosacs.Merchandising.Models;

namespace Blue.Cosacs.Merchandising.Mappers
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising;
    using Blue.Cosacs.Merchandising.DataWarehousing.Models;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Logic;
    using Blue.Cosacs.Merchandising.Models.RP3Export;
    using Blue.Cosacs.Messages.Merchandising.GoodsReceipt;
    using Blue.Cosacs.Messages.Merchandising.StockAdjustment;
    using Blue.Cosacs.Messages.Merchandising.Transfer;
    using Blue.Cosacs.Messages.Merchandising.VendorReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HierarchyTagCondition = Blue.Cosacs.Merchandising.Models.HierarchyTagCondition;
    using Product = Product;
    using ProductExportModel = ProductExportModel;
    using Promotion = Blue.Cosacs.Merchandising.Models.Promotion;
    using PromotionDetail = Blue.Cosacs.Merchandising.Models.PromotionDetail;
    using PurchaseOrderExportModel = PurchaseOrderExportModel;
    using RepossessedCondition = Blue.Cosacs.Merchandising.Models.RepossessedCondition;

    public class MerchandisingAutomapperProfile : Profile
    {
        public override string ProfileName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        protected override void Configure()
        {
            CreateJsonCollectionMap<string>();
            CreateJsonCollectionMap<StringKeyValue>();
            CreateJsonCollectionMap<FieldSchema>();
            CreateJsonCollectionMap<PurchaseOrderAttribute>();

            MapCostPrice();
            this.MapDataWarehouse();
            MapGoodsReceipt();
            MapExport();
            MapHeirarchy();
            MapLocation();
            this.MapRP3Export();
            this.MapGoodsOnLoan();
            MapPermissions();
            this.MapPreviousStockCount();
            MapProduct();
            MapPromotion();
            MapPurchaseOrder();
            MapReports();
            MapRepossessedCondition();
            MapRetailPrice();
            MapSolr();
            this.MapStockAdjustment();
            this.MapStockAdjustmentReason();
            this.MapStockAllocation();
            this.MapStockCount();
            this.MapStockMovement();
            this.MapStockRequisition();
            this.MapStockTransfer();
            MapTag();
            MapTaxRate();
            MapTicketing();
            this.MapTransactionType();
            MapVendor();
            MapVendorReturn();
            MapTradingExport();

            base.Configure();
        }

        private void MapTransactionType()
        {
            Mapper.CreateMap<TransactionType, TransactionTypeViewModel>();
            Mapper.CreateMap<TransactionTypeUpdateModel, TransactionType>().ForMember(x => x.Name, o => o.Ignore());
        }

        private void MapRP3Export()
        {
            Mapper.CreateMap<LocationModel, LocationExportModel>()
                .ForMember(x => x.Company, s => s.Ignore())
                .ForMember(x => x.FasciaCode, s => s.Ignore())
                .ForMember(x => x.FasciaName, s => s.MapFrom(o => o.Fascia))
                .ForMember(x => x.LocationCode, s => s.MapFrom(o => o.SalesId))
                .ForMember(x => x.LocationName, s => s.MapFrom(o => o.Name))
                .ForMember(x => x.WarehouseFlag, s => s.MapFrom(o => o.Warehouse ? "Y" : "N"))
                .ForMember(x => x.SquareMeters, s => s.Ignore())
                .ForMember(x => x.ActiveFlag, s => s.MapFrom(o => o.Active ? "Y" : "N"))
                .ForMember(x => x.StoreType, s => s.MapFrom(o => o.StoreType))
                .ForMember(x => x.AddressLine1, s => s.MapFrom(o => o.AddressLine1))
                .ForMember(x => x.City, s => s.MapFrom(o => o.City));

            Mapper.CreateMap<RP3ProductExportView, ProductModel>();

            Mapper.CreateMap<ProductModel, Models.RP3Export.ProductExportModel>()
                .ForMember(x => x.Company, o => o.Ignore())
                .ForMember(x => x.ExchangeRate, o => o.Ignore()) // Not sent
                .ForMember(x => x.HighestPrice, o => o.Ignore()) // Not sent
                .ForMember(x => x.VendorStorage, o => o.Ignore()) // Not sent
                .ForMember(x => x.Consignment, o => o.Ignore()) // Not sent
                .ForMember(x => x.HandlesSerial, o => o.Ignore()) // Not sent
                .ForMember(x => x.ExemptSales, o => o.Ignore()) // Not sent
                .ForMember(x => x.ExemptPurchase, o => o.Ignore()) // Not sent
                .ForMember(x => x.WarrantySelling, o => o.Ignore()) // Not sent
                .ForMember(x => x.LandTime, o => o.Ignore()) // Not sent
                .ForMember(x => x.MimimumDisplay, o => o.Ignore()) // Not sent
                .ForMember(x => x.MasterPack, o => o.Ignore()) // Not sent
                .ForMember(x => x.PackSize, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "PackSize")))
                .ForMember(x => x.Voltage, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "Voltage")))
                .ForMember(x => x.Hertz, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "Hertz")))
                .ForMember(x => x.CubicFeet, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "CubicFeet")))
                .ForMember(x => x.Attr20Ft, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "20Ft")))
                .ForMember(x => x.Attr40Std, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "40STD")))
                .ForMember(x => x.Attr40Hq, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "40HQ")))
                .ForMember(x => x.Attr45Ft, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "45Ft")))
                .ForMember(x => x.AttrLCL, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "LCL")))
                .ForMember(x => x.AttrRt53, o => o.MapFrom(s => this.GetAttributeValue(s.Attributes, "RT53")))
                .ForMember(x => x.NeverOut, o => o.MapFrom(s => s.Tags.Contains("Never Out") ? "Yes" : "Not"))
                .ForMember(x => x.ProductStrategy, o => o.MapFrom(s => string.Join("|", s.Tags)))
                .ForMember(x => x.MOQ, o => o.Ignore()) // Not sent
                .ForMember(x => x.HarmonizeTariffCode, o => o.Ignore()) // Not sent
                .ForMember(x => x.Percentage, o => o.Ignore()) // Not sent
                .ForMember(x => x.CreationDate, o => o.MapFrom(s => s.CreationDate.ToString("yyyy/MM/dd")))
                .ForMember(x => x.LastTransactionDate, o => o.MapFrom(s => s.LastTransactionDate.ToString("yyyy/MM/dd")))
                .ForMember(x => x.LastReceptionDate, o => o.MapFrom(s => s.LastReceptionDate.ToString("yyyy/MM/dd")))
                .ForMember(x => x.LastSalesDate, o => o.MapFrom(s => s.LastSalesDate.ToString("yyyy/MM/dd")))
                .ForMember(x => x.Company, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ProductAction, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.User, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.SKUStatusCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.SkuType, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ProductCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.CorporateUPC, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.DivisionCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.DivisionName, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.DepartmentCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.DepartmentName, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ClassCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ClassName, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.VendorCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.VendorName, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.BrandCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.BrandName, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.SupplierModel, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.Description, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.CurrencyType, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.LastTransactionDate, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.LastReceptionDate, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.LastSalesDate, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ProductStatus, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.NeverOut, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ProductStrategy, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ProductType, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ReplacingTo, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.Incoterm, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.CountryOfDispatch, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.LeadTime, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.MOQ, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.SubjectTax, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.TaxPercentage, o => o.NullSubstitute(string.Empty));

            Mapper.CreateMap<RP3PurchaseOrderProductView, PurchaseOrderProductExportModel>()
                .ForMember(x => x.Company, o => o.Ignore())
                .ForMember(x => x.UnitsFreeOfCharge, o => o.Ignore())
                .ForMember(x => x.DiscountPercentage, o => o.Ignore())
                .ForMember(x => x.TaxValue, o => o.Ignore())
                .ForMember(x => x.PONumber, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ProductCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.SupplierCurrency, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.Total, o => o.MapFrom(s => s.SubTotal));

            Mapper.CreateMap<RP3PurchaseOrderView, Models.RP3Export.PurchaseOrderExportModel>()
                .ForMember(x => x.Company, o => o.Ignore())
                .ForMember(x => x.DiscountPercentage, o => o.Ignore())
                .ForMember(x => x.DiscountValue, o => o.Ignore())
                .ForMember(x => x.TransactionDate, o => o.MapFrom(s => s.TransactionDate.ToString("yyyy/MM/dd")))
                .ForMember(x => x.ExpectedDeliveryDate, o => o.MapFrom(s => s.ExpectedDeliveryDate.ToString("yyyy/MM/dd")))
                .ForMember(x => x.Total, o => o.MapFrom(s => s.SubTotal))
                .ForMember(
                    x => x.SupplierInvoiceNumber,
                    o => o.ResolveUsing(
                        s =>
                            {
                                if (s.ReferenceNumbers == null)
                                {
                                    return string.Empty;
                                }

                                var referenceNumbers = JsonConvertHelper.DeserializeObjectOrDefault<List<StringKeyValue>>(s.ReferenceNumbers);

                                var supplierInvoice = referenceNumbers.SingleOrDefault(r => r.Key == "SupplierInvoiceNumber");
                                return supplierInvoice == null ? string.Empty : supplierInvoice.Value;
                            })).ForMember(
                                x => x.ReferenceNumber,
                                o => o.ResolveUsing(
                                    s =>
                                        {
                                            if (s.ReferenceNumbers == null)
                                            {
                                                return string.Empty;
                                            }

                                            var referenceNumber = JsonConvertHelper.DeserializeObjectOrDefault<List<StringKeyValue>>(s.ReferenceNumbers).FirstOrDefault();
                                            return referenceNumber == null ? string.Empty : referenceNumber.Value;
                                        }))
                .ForMember(x => x.VendorCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.VendorName, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ReceivingLocationCode, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.ReceivingLocationName, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.POStatus, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.POType, o => o.NullSubstitute(string.Empty))
                .ForMember(x => x.CorporatePONumber, o => o.NullSubstitute(string.Empty));

            Mapper.CreateMap<RP3VendorReturnView, VendorReturnExportModel>()
                .ForMember(x => x.Company, o => o.Ignore())
                .ForMember(x => x.TransactionDate, o => o.MapFrom(s => s.TransactionDate.ToString("yyyy/MM/dd")))
                .ForMember(x => x.RTSStatus, o => o.Ignore());
            Mapper.CreateMap<RP3VendorReturnProductView, VendorReturnProductExportModel>().ForMember(x => x.Company, o => o.Ignore());

            Mapper.CreateMap<RP3GoodsReceiptView, GoodsReceiptExportModel>()
                .ForMember(x => x.Company, o => o.Ignore())
                .ForMember(x => x.TransactionDate, o => o.MapFrom(s => s.TransactionDate.ToString("yyyy/MM/dd")));
            Mapper.CreateMap<RP3GoodsReceiptProductView, GoodsReceiptProductExportModel>().ForMember(x => x.Company, o => o.Ignore());

            Mapper.CreateMap<RP3StockAdjustmentView, StockAdjustmentExportModel>()
                .ForMember(x => x.Company, o => o.Ignore())
                .ForMember(x => x.TransactionDate, o => o.MapFrom(s => s.TransactionDate.ToString("yyyy/MM/dd")));
            Mapper.CreateMap<RP3StockAdjustmentProductView, StockAdjustmentProductExportModel>().ForMember(x => x.Company, o => o.Ignore());

            Mapper.CreateMap<RP3StockTransferView, StockTransferExportModel>()
                .ForMember(x => x.Company, o => o.Ignore())
                .ForMember(x => x.TransactionDate, o => o.MapFrom(s => s.TransactionDate.ToString("yyyy/MM/dd")))
                .ForMember(x => x.TransferStatus, o => o.UseValue("R"));

            Mapper.CreateMap<RP3StockTransferProductView, StockTransferProductExportModel>().ForMember(x => x.Company, o => o.Ignore());
        }

        private string GetAttributeValue(List<FieldSchema> attributes, string attributeName)
        {
            var attr = attributes.SingleOrDefault(a => a.DisplayName == attributeName);

            return attr != null ? attr.Value : string.Empty;
        }

        private void MapDataWarehouse()
        {
            Mapper.CreateMap<ReportModel, SalesStatistics>();
            Mapper.CreateMap<RowModel, SalesStatistics>();
            Mapper.CreateMap<InventoryModel, InventoryStatistics>();
        }

        private void MapTradingExport()
        {
            Mapper.CreateMap<WTRWarrantyView, TradingExportGenericModel>()
                .ForMember(x => x.Department, o => o.Ignore())
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTRInstallationsView, TradingExportGenericModel>()
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTRAffinityView, TradingExportGenericModel>()
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTRIGenericServicesView, TradingExportGenericModel>()
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTRLoansView, TradingExportGenericModel>()
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTRServiceChargeView, TradingExportGenericModel>()
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTRRebatesView, TradingExportGenericModel>()
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTRDiscountsView, TradingExportGenericModel>()
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTRSalesAView, TradingExportGenericModel>()
                .ForMember(x => x.LocationName, o => o.MapFrom(s => s.Fascia))
                .ForMember(x => x.Fascia, o => o.Ignore());

            Mapper.CreateMap<WTRSalesBView, TradingExportGenericModel>()
                .ForMember(x => x.Fascia, o => o.Ignore());

            Mapper.CreateMap<WTRSalesFView, TradingExportGenericModel>()
                .ForMember(x => x.Fascia, o => o.Ignore())
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Department, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore());

            Mapper.CreateMap<WTRPenaltyView, TradingExportGenericModel>()
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore())
                .ForMember(x => x.Department, o => o.Ignore())
                .ForMember(x => x.DepartmentCode, o => o.Ignore());

            Mapper.CreateMap<WTRCreditCashDifferenceView, TradingExportGenericModel>();

            Mapper.CreateMap<WTRCurrentStockItemsValueBranchView, TradingExportGenericModel>()
                .ForMember(x => x.DateKey, o => o.Ignore())
                .ForMember(x => x.Department, o => o.Ignore())
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.Fascia, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore());

            Mapper.CreateMap<WTREndOfMonthStockItemsValueBranchView, TradingExportGenericModel>()
                .ForMember(x => x.Department, o => o.Ignore())
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.ClassCode, o => o.Ignore())
                .ForMember(x => x.Fascia, o => o.Ignore());

            Mapper.CreateMap<WTRCurrentStockItemsValueView, TradingExportGenericModel>()
                .ForMember(x => x.DateKey, o => o.Ignore())
                .ForMember(x => x.Fascia, o => o.Ignore());

            Mapper.CreateMap<WTREndOfMonthStockItemsValueView, TradingExportGenericModel>()
                .ForMember(x => x.Fascia, o => o.Ignore());
        }

        private void MapStockCount()
        {
            Mapper.CreateMap<StockCountView, StockCountViewModel>().ForMember(x => x.Hierarchy, o => o.Ignore());
            Mapper.CreateMap<StockCountProductView, StockCountProductViewModel>().ForMember(x => x.Currency, o => o.Ignore()).ForMember(x => x.Cost, o => o.Ignore());
            Mapper.CreateMap<StockCountProductView, StockCountProductExportModel>().ForMember(x => x.Variance, o => o.MapFrom(z => z.Count - z.StartStockOnHand + z.SystemAdjustment));
            Mapper.CreateMap<StockCountCreateModel, StockCount>()
                .ForMember(x => x.CancelledDate, o => o.Ignore())
                .ForMember(x => x.CancelledById, o => o.Ignore())
                .ForMember(x => x.StartedById, o => o.Ignore())
                .ForMember(x => x.ClosedById, o => o.Ignore())
                .ForMember(x => x.ClosedDate, o => o.Ignore())
                .ForMember(x => x.StartedDate, o => o.Ignore())
                .ForMember(x => x.StockAdjustmentId, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore());
            Mapper.CreateMap<StockCountProductUpdateModel, StockCountProduct>()
                .ForMember(x => x.StartStockOnHand, o => o.Ignore())
                .ForMember(x => x.Hierarchy, o => o.Ignore())
                .ForMember(x => x.StockCountId, o => o.Ignore())
                .ForMember(x => x.ProductId, o => o.Ignore())
                .ForMember(x => x.VerifiedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore());
            Mapper.CreateMap<StockCountView, StockCountSearchResultModel>();
            Mapper.CreateMap<StockCountView, StockCountPrintModel>().ForMember(x => x.Hierarchys, o => o.Ignore());
            Mapper.CreateMap<StockCountView, SimpleStockCountViewModel>().ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<StockCountProductView, SimpleStockCountProductViewModel>().ForMember(d => d.Barcode, o => o.MapFrom(s => s.CorporateUPC));
            Mapper.CreateMap<SimpleStockCountProductViewModel, StockCountProduct>()
                .ForMember(x => x.StartStockOnHand, o => o.Ignore())
                .ForMember(x => x.Hierarchy, o => o.Ignore())
                .ForMember(x => x.SystemAdjustment, o => o.Ignore())
                .ForMember(x => x.Comments, o => o.Ignore())
                .ForMember(x => x.ProductId, o => o.Ignore())
                .ForMember(x => x.VerifiedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore());

        }

        private void MapStockMovement()
        {
            Mapper.CreateMap<StockMovementReportView, StockMovementReportModel>()
                .ForMember(x => x.Quantity, o => o.MapFrom(s => s.Quantity.HasValue ? s.Quantity.Value : 0))
                .ForMember(x => x.StockLevel, o => o.Ignore())
                .ForMember(x => x.Tags, o => o.Ignore());
            Mapper.CreateMap<StockMovementReportModel, StockMovementReportModel>(); // for cloning
        }

        private void MapStockAdjustment()
        {
            Mapper.CreateMap<StockAdjustmentView, StockAdjustmentViewModel>().ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentViewModel, StockAdjustment>();
            Mapper.CreateMap<StockAdjustmentCreateModel, StockAdjustment>()
                .ForMember(x => x.OriginalPrint, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.AuthorisedById, o => o.Ignore())
                .ForMember(x => x.AuthorisedDate, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentProductView, StockAdjustmentProductViewModel>()
                .ForMember(d => d.Description, o => o.MapFrom(s => s.LongDescription))
                .ForMember(x => x.DepartmentCode, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentProductViewModel, StockAdjustmentProduct>().ForMember(x => x.StockAdjustmentId, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentProductCreateModel, StockAdjustmentProduct>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.StockAdjustmentId, o => o.Ignore())
                .ForMember(x => x.AverageWeightedCost, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentSearchView, StockAdjustmentSearchResultModel>();
            Mapper.CreateMap<StockAdjustmentViewModel, StockAdjustmentPrintModel>()
                .ForMember(x => x.CreatedDate, o => o.MapFrom(s => s.CreatedDate.ToServerTime()))
                .ForMember(x => x.AuthorisedDate, o => o.MapFrom(s => s.AuthorisedDate.ToServerTime()));
            Mapper.CreateMap<StockAdjustmentProductViewModel, StockAdjustmentProductPrintModel>()
                .ForMember(x => x.UnitCost, o => o.Ignore())
                .ForMember(x => x.LineCost, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentPrintModel, StockAdjustmentViewModel>()
                .ForMember(x => x.LocationId, o => o.Ignore())
                .ForMember(x => x.SalesLocationId, o => o.Ignore())
                .ForMember(x => x.PrimaryReasonId, o => o.Ignore())
                .ForMember(x => x.SecondaryReasonId, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentProductPrintModel, StockAdjustmentProductViewModel>().ForMember(x => x.DepartmentCode, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentViewModel, StockAdjustmentMessage>()
                .ForMember(x => x.AdjustmentId, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.SecondaryReason, o => o.MapFrom(y => y.SecondaryReasonId))
                .ForMember(x => x.CreatedDate, o => o.MapFrom(y => y.CreatedDate.ToLocalDateTime()))
                .ForMember(x => x.AWC, o => o.MapFrom(y => y.Products.Sum(p => p.AverageWeightedCost * p.Quantity)))
                .ForMember(x => x.Description, o => o.MapFrom(y => string.Format("ADJ#{0}", y.Id)));
            Mapper.CreateMap<ProductMessageView, StockAdjustmentMessageProduct>()
                .ForMember(x => x.Cost, o => o.Ignore())
                .ForMember(x => x.Units, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentProductViewModel, StockAdjustmentMessageProduct>()
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Cost, o => o.Ignore())
                .ForMember(x => x.Units, o => o.Ignore());
        }

        private void MapGoodsOnLoan()
        {
            Mapper.CreateMap<GoodsOnLoan, GoodsOnLoanViewModel>()
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.StockLocation, o => o.Ignore())
                .ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<GoodsOnLoanProduct, GoodsOnLoanProductViewModel>().ForMember(x => x.Sku, o => o.Ignore()).ForMember(x => x.Description, o => o.Ignore());
            Mapper.CreateMap<GoodsOnLoanProductView, GoodsOnLoanProductViewModel>().ForMember(x => x.Description, o => o.MapFrom(s => s.LongDescription));
            Mapper.CreateMap<GoodsOnLoanCreateModel, GoodsOnLoan>()
                .ForMember(x => x.CollectedDate, o => o.Ignore())
                .ForMember(x => x.DeliveryPrintedDate, o => o.Ignore())
                .ForMember(x => x.CollectionPrintedDate, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedOn, o => o.Ignore())
                .ForMember(x => x.ReturnStockTransferId, o => o.Ignore())
                .ForMember(x => x.StockTransferId, o => o.Ignore());

            Mapper.CreateMap<GoodsOnLoanProductCreateModel, GoodsOnLoanProduct>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.GoodsOnLoanId, o => o.Ignore())
                .ForMember(x => x.AverageWeightedCost, o => o.Ignore());
            Mapper.CreateMap<GoodsOnLoanProductCreateModel, StockTransferProductCreateModel>();
            Mapper.CreateMap<GoodsOnLoanProduct, StockTransferProductCreateModel>();
            Mapper.CreateMap<GoodsOnLoanSearchView, GoodsOnLoanSearchResultModel>()
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.Id, o => o.MapFrom(s => s.GoodsOnLoanId));
            Mapper.CreateMap<GoodsOnLoan, GoodsOnLoanPrintModel>()
                .ForMember(x => x.DeliveryPrinted, o => o.MapFrom(s => s.DeliveryPrintedDate.HasValue))
                .ForMember(x => x.CollectionPrinted, o => o.MapFrom(s => s.CollectionPrintedDate.HasValue))
                .ForMember(x => x.CollectedDate, o => o.MapFrom(s => s.CollectedDate.ToPrintDateTimeString()))
                .ForMember(x => x.CreatedOn, o => o.MapFrom(s => s.CreatedOn.ToPrintDateTimeString()))
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.StockLocation, o => o.Ignore())
                .ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<GoodsOnLoanProductView, GoodsOnLoanProductPrintModel>().ForMember(x => x.Description, o => o.MapFrom(s => s.LongDescription));
        }

        private void MapStockAllocation()
        {
            Mapper.CreateMap<StockAllocationProductCreateModel, StockAllocationProduct>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.BookingId, o => o.Ignore())
                .ForMember(x => x.QuantityCancelled, o => o.Ignore())
                .ForMember(x => x.QuantityReceived, o => o.Ignore())
                .ForMember(x => x.CompletedOn, o => o.Ignore())
                .ForMember(x => x.WarehouseLocationId, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.ReferenceNumber, o => o.Ignore())
                .ForMember(x => x.Comments, o => o.Ignore())
                .ForMember(x => x.AverageWeightedCost, o => o.Ignore());
            Mapper.CreateMap<StockAllocationProductView, StockAllocationProductViewModel>()
                .ForMember(d => d.Description, o => o.MapFrom(s => s.LongDescription))
                .ForMember(x => x.AverageWeightedCost, o => o.MapFrom(y => y.AverageWeightedCost));
            Mapper.CreateMap<StockAllocationProductView, StockAllocationSearchResultModel>()
                .ForMember(x => x.AverageWeightedCost, o => o.MapFrom(y => y.AverageWeightedCost));
        }

        private void MapStockTransfer()
        {
            Mapper.CreateMap<StockTransferProductCreateModel, StockTransferProduct>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.StockTransferId, o => o.Ignore())
                .ForMember(x => x.BookingId, o => o.Ignore())
                .ForMember(x => x.QuantityCancelled, o => o.Ignore())
                .ForMember(x => x.QuantityReceived, o => o.Ignore())
                .ForMember(x => x.CompletedOn, o => o.Ignore())
                .ForMember(x => x.AverageWeightedCost, o => o.Ignore());
            Mapper.CreateMap<StockTransferProductView, StockTransferProductViewModel>().ForMember(x => x.Description, o => o.MapFrom(y => y.LongDescription));
            Mapper.CreateMap<ProductMessageView, Messages.Merchandising.Transfer.TransferMessageProduct>()
                .ForMember(x => x.Cost, o => o.Ignore())
                .ForMember(x => x.Units, o => o.Ignore());
            Mapper.CreateMap<StockTransferViewModel, TransferMessage>()
                .ForMember(x => x.AWC, o => o.MapFrom(y => y.Products.Sum(p => p.AverageWeightedCost * p.Quantity)))
                .ForMember(x => x.WarehouseLocationId, o => o.Ignore())
                .ForMember(x => x.WarehouseLocationSalesId, o => o.Ignore())
                .ForMember(x => x.ReceivingLocationSalesId, o => o.Ignore())
                .ForMember(x => x.Products, o => o.Ignore())
                .ForMember(x => x.Description, o => o.MapFrom(y => string.Format("ST#{0}", y.Id)));
            Mapper.CreateMap<StockTransferCreateModel, StockTransfer>()
                .ForMember(x => x.OriginalPrint, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore());
            Mapper.CreateMap<StockTransfer, StockTransferCreateModel>().ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<StockTransferViewModel, StockTransferView>().ForMember(x => x.OriginalPrint, o => o.Ignore()).ForMember(x => x.Total, o => o.Ignore());
            Mapper.CreateMap<StockTransferView, StockTransferViewModel>().ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<StockTransferView, StockTransferSearchResultModel>();
            //Mapper.CreateMap<StockTransferSearchResultModel, StockTransferView>()
            //    .ForMember(x => x.OriginalPrint, o => o.Ignore())
            //    .ForMember(x => x.SendingLocationSalesId, o => o.Ignore())
            //    .ForMember(x => x.ViaLocationSalesId, o => o.Ignore())
            //    .ForMember(x => x.ReceivingLocationSalesId, o => o.Ignore());
            Mapper.CreateMap<StockTransferView, StockTransferPrintModel>()
                .ForMember(x => x.CreatedDate, o => o.MapFrom(y => y.CreatedDate.ToLocalTime()))
                .ForMember(x => x.Products, o => o.Ignore());
        }

        private void MapStockRequisition()
        {
            Mapper.CreateMap<StockRequisitionProductCreateModel, StockRequisitionProduct>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.BookingId, o => o.Ignore())
                .ForMember(x => x.QuantityCancelled, o => o.Ignore())
                .ForMember(x => x.QuantityReceived, o => o.Ignore())
                .ForMember(x => x.CompletedOn, o => o.Ignore())
                .ForMember(x => x.WarehouseLocationId, o => o.Ignore())
                .ForMember(x => x.ReceivingLocationId, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.AverageWeightedCost, o => o.Ignore());
            Mapper.CreateMap<StockRequisitionProductView, StockRequisitionProductViewModel>()
                .ForMember(x => x.Description, o => o.MapFrom(y => y.LongDescription))
                .ForMember(x => x.AverageWeightedCost, o => o.MapFrom(y => y.AverageWeightedCost));
            Mapper.CreateMap<StockRequisitionProductView, StockRequisitionSearchResultModel>()
                .ForMember(x => x.AverageWeightedCost, o => o.MapFrom(y => y.AverageWeightedCost));
        }

        private void MapPreviousStockCount()
        {
            Mapper.CreateMap<StockCountProductView, StockCountPreviousItemViewModel>();
        }

        private void MapStockAdjustmentReason()
        {
            Mapper.CreateMap<StockAdjustmentReasonView, StockAdjustmentSecondaryReasonViewModel>().ForMember(d => d.Id, o => o.MapFrom(s => s.SecondaryReasonId));
            Mapper.CreateMap<StockAdjustmentReasonView, StockAdjustmentReasonViewModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.PrimaryReasonId))
                .ForMember(x => x.SecondaryReasons, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentReasonCreateModel, StockAdjustmentPrimaryReason>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.PrimaryReason))
                .ForMember(x => x.DateDeleted, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentSecondaryReasonViewModel, StockAdjustmentSecondaryReason>().ForMember(x => x.DateDeleted, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentPrimaryReason, StockAdjustmentReasonViewModel>()
                .ForMember(d => d.PrimaryReason, o => o.MapFrom(s => s.Name))
                .ForMember(x => x.SecondaryReasons, o => o.Ignore());
            Mapper.CreateMap<StockAdjustmentSecondaryReason, StockAdjustmentSecondaryReasonViewModel>();
        }

        private static void MapPermissions()
        {
            Mapper.CreateMap<UserPermissionsView, UserPermissionsViewModel>().ForMember(d => d.Id, o => o.MapFrom(s => s.UserId));
            Mapper.CreateMap<UserPermissionsView, UserViewModel>().ForMember(d => d.Id, o => o.MapFrom(s => s.UserId)).ForMember(x => x.PermissionIds, o => o.Ignore());
        }

        private static void MapTicketing()
        {
            Mapper.CreateMap<TicketingModel, TicketExtractView>();
            Mapper.CreateMap<TicketExtractView, TicketingModel>().ForMember(x => x.Components, o => o.Ignore());
        }

        private static void MapTaxRate()
        {
            Mapper.CreateMap<TaxRateModel, TaxRate>().ForMember(d => d.EffectiveDate, o => o.MapFrom(s => s.EffectiveDate));
            Mapper.CreateMap<TaxRate, TaxRateModel>();
        }

        private static void MapHeirarchy()
        {
            Mapper.CreateMap<Merchandising.HierarchyTagCondition, HierarchyTagCondition>().ForMember(x => x.ConditionName, o => o.Ignore());
            Mapper.CreateMap<HierarchyTagCondition, Merchandising.HierarchyTagCondition>();

            Mapper.CreateMap<TagView, LevelModel>()
                .ForMember(l => l.Id, o => o.MapFrom(x => x.LevelId))
                .ForMember(l => l.Name, o => o.MapFrom(x => x.LevelName))
                .ForMember(x => x.Tags, o => o.Ignore());
            Mapper.CreateMap<TagView, TagModel>().ForMember(l => l.Id, o => o.MapFrom(x => x.Id)).ForMember(l => l.Name, o => o.MapFrom(x => x.TagName));
        }

        private static void MapVendor()
        {
            Mapper.CreateMap<Supplier, SupplierModel>();
            Mapper.CreateMap<SupplierModel, Supplier>().ForMember(s => s.Country, o => o.MapFrom(x => x.Country.ToUpper()));
            Mapper.CreateMap<SupplierImportModel, Supplier>()
                .ForMember(s => s.Country, o => o.MapFrom(x => x.Country.ToUpper()))
                .ForMember(x => x.OrderEmail, o => o.Ignore())
                .ForMember(x => x.Tags, o => o.Ignore())
                .ForMember(x => x.Currency, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore());
        }

        private static void MapRepossessedCondition()
        {
            Mapper.CreateMap<Merchandising.RepossessedCondition, EditRepossessedCondition>();
            Mapper.CreateMap<EditRepossessedCondition, Merchandising.RepossessedCondition>();
            Mapper.CreateMap<RepossessedCondition, Merchandising.RepossessedCondition>().ForMember(x => x.Id, o => o.Ignore());
            Mapper.CreateMap<Merchandising.RepossessedCondition, RepossessedCondition>();
        }

        private static void MapLocation()
        {
            Mapper.CreateMap<LocationModel, Location>();
            Mapper.CreateMap<Location, LocationModel>();
        }

        private static void MapTag()
        {
            Mapper.CreateMap<TagView, Tag>()
                .ForMember(d => d.Level, o => o.MapFrom(s => new Level { Name = s.LevelName, Id = s.LevelId }))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.TagName))
                .ForMember(x => x.RepossessedConditions, o => o.Ignore());

            Mapper.CreateMap<Tag, TagView>()
                .ForMember(d => d.LevelName, o => o.MapFrom(s => s.Level.Name))
                .ForMember(d => d.LevelId, o => o.MapFrom(s => s.Level.Id))
                .ForMember(d => d.TagName, o => o.MapFrom(s => s.Name));

            Mapper.CreateMap<Tag, HierarchyTag>().ForMember(d => d.LevelId, o => o.MapFrom(s => s.Level.Id));

            Mapper.CreateMap<HierarchyTag, Tag>()
                .ForMember(d => d.Level, o => o.MapFrom(s => new Level { Id = s.LevelId }))
                .ForMember(x => x.RepossessedConditions, o => o.Ignore());
        }

        private static void MapProduct()
        {
            // Products
            Mapper.CreateMap<Product, Product>().ForMember(x => x.CreatedById, o => o.Ignore());
            Mapper.CreateMap<Product, ProductDescriptor>();
            Mapper.CreateMap<Models.Product, Product>()
                .ForMember(d => d.OnlineDateAdded, o => o.Ignore())
                .ForMember(x => x.LastStatusChangeDate, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore());
            Mapper.CreateMap<Product, Models.Product>()
                .ForMember(x => x.StockLevel, o => o.Ignore())
                .ForMember(x => x.Sales, o => o.Ignore())
                .ForMember(x => x.CurrentRetailPrice, o => o.Ignore())
                .ForMember(x => x.CurrentTaxRate, o => o.Ignore())
                .ForMember(x => x.CostPrice, o => o.Ignore())
                .ForMember(x => x.Suppliers, o => o.Ignore())
                .ForMember(x => x.Hierarchy, o => o.Ignore())
                .ForMember(x => x.TaxRates, o => o.Ignore())
                .ForMember(x => x.RetailPrices, o => o.Ignore())
                .ForMember(x => x.Incoterms, o => o.Ignore())
                .ForMember(x => x.Promotions, o => o.Ignore());
            Mapper.CreateMap<Brand, Brand>().ForMember(d => d.Id, o => o.Ignore());

            // Combos
            Mapper.CreateMap<ComboView, ComboModel>().ForMember(x => x.ComboPrices, o => o.Ignore()).ForMember(x => x.Components, o => o.Ignore());
            Mapper.CreateMap<ComboModel, Combo>();
            Mapper.CreateMap<ComboModel, Product>()
                .ForMember(d => d.POSDescription, o => o.MapFrom(s => s.LongDescription))
                .ForMember(d => d.Tags, o => o.Ignore())
                .ForMember(x => x.ProductType, o => o.Ignore())
                .ForMember(x => x.StoreTypes, o => o.Ignore())
                .ForMember(x => x.Attributes, o => o.Ignore())
                .ForMember(x => x.SKUStatus, o => o.Ignore())
                .ForMember(x => x.CorporateUPC, o => o.Ignore())
                .ForMember(x => x.VendorUPC, o => o.Ignore())
                .ForMember(x => x.VendorStyleLong, o => o.Ignore())
                .ForMember(x => x.CountryOfOrigin, o => o.Ignore())
                .ForMember(x => x.VendorWarranty, o => o.Ignore())
                .ForMember(x => x.ReplacingTo, o => o.Ignore())
                .ForMember(x => x.Features, o => o.Ignore())
                .ForMember(x => x.BrandId, o => o.Ignore())
                .ForMember(x => x.PrimaryVendorId, o => o.Ignore())
                .ForMember(x => x.OnlineDateAdded, o => o.Ignore())
                .ForMember(x => x.LastStatusChangeDate, o => o.Ignore())
                .ForMember(x => x.LabelRequired, o => o.Ignore())
                .ForMember(x => x.BoxCount, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.ExternalCreationDate, o => o.Ignore())
                .ForMember(x => x.ProductAction, o => o.Ignore())
                .ForMember(x => x.MagentoExportType, o => o.Ignore());
            Mapper.CreateMap<Models.Product, ComboModel>()
                .ForMember(d => d.LongDescription, o => o.MapFrom(s => s.POSDescription))
                .ForMember(d => d.Tags, o => o.Ignore())
                .ForMember(x => x.StartDate, o => o.Ignore())
                .ForMember(x => x.EndDate, o => o.Ignore())
                .ForMember(x => x.ComboPrices, o => o.Ignore())
                .ForMember(x => x.Components, o => o.Ignore());
            Mapper.CreateMap<ComboProductView, ComboProduct>();

            // Sets
            Mapper.CreateMap<SetModel, SetProduct>()
                .ForMember(x => x.ProductId, o => o.Ignore())
                .ForMember(x => x.SetId, o => o.Ignore())
                .ForMember(x => x.Quantity, o => o.Ignore());
            Mapper.CreateMap<SetModel, Product>()
                .ForMember(d => d.POSDescription, o => o.MapFrom(s => s.LongDescription))
                .ForMember(d => d.Tags, o => o.Ignore())
                .ForMember(x => x.ProductType, o => o.Ignore())
                .ForMember(x => x.StoreTypes, o => o.Ignore())
                .ForMember(x => x.Attributes, o => o.Ignore())
                .ForMember(x => x.SKUStatus, o => o.Ignore())
                .ForMember(x => x.CorporateUPC, o => o.Ignore())
                .ForMember(x => x.VendorUPC, o => o.Ignore())
                .ForMember(x => x.VendorStyleLong, o => o.Ignore())
                .ForMember(x => x.CountryOfOrigin, o => o.Ignore())
                .ForMember(x => x.VendorWarranty, o => o.Ignore())
                .ForMember(x => x.ReplacingTo, o => o.Ignore())
                .ForMember(x => x.Features, o => o.Ignore())
                .ForMember(x => x.BrandId, o => o.Ignore())
                .ForMember(x => x.PrimaryVendorId, o => o.Ignore())
                .ForMember(x => x.OnlineDateAdded, o => o.Ignore())
                .ForMember(x => x.LastStatusChangeDate, o => o.Ignore())
                .ForMember(x => x.LabelRequired, o => o.Ignore())
                .ForMember(x => x.BoxCount, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.ExternalCreationDate, o => o.Ignore())
                .ForMember(x => x.ProductAction, o => o.Ignore())
                .ForMember(x => x.MagentoExportType, o => o.Ignore());

            Mapper.CreateMap<Product, SetModel>()
                .ForMember(d => d.LongDescription, o => o.MapFrom(s => s.POSDescription))
                .ForMember(x => x.Components, o => o.Ignore())
                .ForMember(x => x.Locations, o => o.Ignore());
            Mapper.CreateMap<Models.Product, SetModel>()
                .ForMember(d => d.LongDescription, o => o.MapFrom(s => s.POSDescription))
                .ForMember(x => x.Components, o => o.Ignore())
                .ForMember(x => x.Locations, o => o.Ignore());

            Mapper.CreateMap<SetProductView, SetProduct>();
        }

        private static void MapCostPrice()
        {
            Mapper.CreateMap<CostPriceCreateModel, CostPrice>()
                .ForMember(x => x.AverageWeightedCost, o => o.Ignore())
                .ForMember(x => x.AverageWeightedCostUpdated, o => o.Ignore())
                .ForMember(x => x.LastLandedCostUpdated, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore());
            Mapper.CreateMap<CostPriceModel, CostPrice>();
            Mapper.CreateMap<CostPrice, CostPriceModel>();
            Mapper.CreateMap<CostPriceModel, CostPriceView>();
            Mapper.CreateMap<CostPriceView, CostPriceModel>();
            Mapper.CreateMap<CostPriceModel, CurrentCostPriceView>();
            Mapper.CreateMap<CurrentCostPriceView, CostPriceModel>();
            Mapper.CreateMap<CurrentCostPriceView, CostPrice>();
        }

        private static void MapRetailPrice()
        {
            Mapper.CreateMap<RetailPriceViewModel, RetailPrice>().ForMember(x => x.LastUpdated, o => o.Ignore());
            Mapper.CreateMap<RetailPrice, RetailPriceViewModel>().ForMember(x => x.Location, o => o.Ignore()).ForMember(x => x.TaxRate, o => o.Ignore());
            Mapper.CreateMap<RetailPriceView, RetailPriceViewModel>().ForMember(v => v.Location, o => o.MapFrom(s => s.Name));
            Mapper.CreateMap<RepossessedPriceView, RetailPriceViewModel>().ForMember(v => v.Location, o => o.MapFrom(s => s.Name));
            Mapper.CreateMap<RetailPriceViewModel, RetailPriceAuditModel>()
                .ForMember(v => v.EffectiveDate, o => o.MapFrom(s => s.EffectiveDate.ToAuditDateString()))
                .ForMember(v => v.ExclusiveRegularPrice, o => o.MapFrom(s => s.RegularPrice.ToCurrency()))
                .ForMember(v => v.ExclusiveCashPrice, o => o.MapFrom(s => s.CashPrice.ToCurrency()))
                .ForMember(v => v.ExclusiveDutyFreePrice, o => o.MapFrom(s => s.DutyFreePrice.ToCurrency()))
                .ForMember(v => v.InclusiveRegularPrice, o => o.MapFrom(s => TaxCalcuator.AddTax(s.RegularPrice, s.TaxRate).ToCurrency()))
                .ForMember(v => v.InclusiveCashPrice, o => o.MapFrom(s => TaxCalcuator.AddTax(s.CashPrice, s.TaxRate).ToCurrency()))
                .ForMember(v => v.InclusiveDutyFreePrice, o => o.MapFrom(s => TaxCalcuator.AddTax(s.DutyFreePrice, s.TaxRate).ToCurrency()));
            Mapper.CreateMap<CurrentRetailPriceView, RetailPriceView>();
            Mapper.CreateMap<CurrentRetailPriceView, RepossessedPriceView>();
            Mapper.CreateMap<RepossessedPriceView, CurrentRetailPriceView>();
            Mapper.CreateMap<CurrentRetailPriceView, SetLocationView>()
                .ForMember(v => v.SetId, o => o.MapFrom(x => x.ProductId));
            Mapper.CreateMap<SetLocationView, CurrentRetailPriceView>()
                .ForMember(v => v.TaxRate, o => o.Ignore())
                .ForMember(v => v.ProductId, o => o.MapFrom(s => s.SetId));
        }

        private static void MapSolr()
        {
            //for cloning
            Mapper.CreateMap<ForceMerchandiseStockLevelsView, ForceMerchandiseStockLevelsView>();
        }

        private static void MapPromotion()
        {
            Mapper.CreateMap<Promotion, Merchandising.Promotion>().ForMember(d => d.PromotionType, o => o.MapFrom(s => ((PromotionType)s.Type.Value).ToString()));
            Mapper.CreateMap<Merchandising.Promotion, Promotion>()
                .ForMember(d => d.Type, o => o.MapFrom(s => (int)Enum.Parse(typeof(PromotionType), s.PromotionType)))
                .ForMember(x => x.Location, o => o.Ignore())
                .ForMember(x => x.StartDate, o => o.MapFrom(s => s.StartDate))
                .ForMember(x => x.EndDate, o => o.MapFrom(s => s.EndDate))
                .ForMember(x => x.Details, o => o.Ignore());
            Mapper.CreateMap<Merchandising.PromotionDetail, PromotionDetail>()
                .ForMember(d => d.SetPrice, o => o.MapFrom(s => s.Price))
                .ForMember(d => d.PriceTypeName, o => o.MapFrom(s => s.PriceType))
                .ForMember(d => d.PriceType, o => o.MapFrom(s => (int)Enum.Parse(typeof(PriceType), s.PriceType)))
                .ForMember(x => x.Sku, o => o.Ignore())
                .ForMember(x => x.OriginalPrice, o => o.Ignore())
                .ForMember(x => x.AverageWeightedCost, o => o.Ignore())
                .ForMember(x => x.Name, o => o.Ignore())
                .ForMember(x => x.TaxRate, o => o.Ignore())
                .ForMember(x => x.Hierarchies, o => o.Ignore());
            Mapper.CreateMap<PromotionDetail, Merchandising.PromotionDetail>()
                .ForMember(d => d.Price, o => o.MapFrom(s => s.SetPrice))
                .ForMember(d => d.PriceType, o => o.MapFrom(s => (PriceType)s.PriceType))
                .ForMember(x => x.PromotionId, o => o.Ignore());
        }

        private static void MapPurchaseOrder()
        {
            Mapper.CreateMap<PurchaseOrderCreateModel, PurchaseOrder>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Vendor, o => o.Ignore())
                .ForMember(x => x.ReceivingLocation, o => o.Ignore())
                .ForMember(x => x.Status, o => o.Ignore())
                .ForMember(x => x.OriginalPrint, o => o.Ignore())
                .ForMember(x => x.ExpiredDate, o => o.Ignore());
            Mapper.CreateMap<PurchaseOrder, PurchaseOrderViewModel>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(x => x.FormattedTotalCost, o => o.Ignore())
                .ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<EditPurchaseOrderViewModel, PurchaseOrder>()
                .ForMember(x => x.VendorId, o => o.Ignore())
                .ForMember(x => x.Vendor, o => o.Ignore())
                .ForMember(x => x.RequestedDeliveryDate, o => o.Ignore())
                .ForMember(x => x.ReceivingLocationId, o => o.Ignore())
                .ForMember(x => x.ReceivingLocation, o => o.Ignore())
                .ForMember(x => x.Currency, o => o.Ignore())
                .ForMember(x => x.Status, o => o.Ignore())
                .ForMember(x => x.OriginalPrint, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.PaymentTerms, o => o.Ignore())
                .ForMember(x => x.OriginSystem, o => o.Ignore())
                .ForMember(x => x.CorporatePoNumber, o => o.Ignore())
                .ForMember(x => x.ShipDate, o => o.Ignore())
                .ForMember(x => x.ShipVia, o => o.Ignore())
                .ForMember(x => x.PortOfLoading, o => o.Ignore())
                .ForMember(x => x.Attributes, o => o.Ignore())
                .ForMember(x => x.CommissionChargeFlag, o => o.Ignore())
                .ForMember(x => x.CommissionPercentage, o => o.Ignore())
                .ForMember(x => x.ExpiredDate, o => o.Ignore());
            Mapper.CreateMap<PurchaseOrderProductImportModel, PurchaseOrderProduct>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Sku, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.UnitCost, o => o.Ignore())
                .ForMember(x => x.QuantityCancelled, o => o.Ignore())
                .ForMember(x => x.EstimatedDeliveryDate, o => o.Ignore())
                .ForMember(x => x.PurchaseOrderId, o => o.Ignore());
            Mapper.CreateMap<PurchaseOrderProductViewModel, PurchaseOrderProduct>()
                .ForMember(x => x.PurchaseOrderId, o => o.Ignore())
                .ForMember(x => x.PreLandedExtendedCost, o => o.Ignore())
                .ForMember(x => x.QuantityCancelled, o => o.Ignore())
                .ForMember(x => x.PreLandedUnitCost, o => o.Ignore());
            Mapper.CreateMap<PurchaseOrderProduct, PurchaseOrderProductViewModel>()
                .ForMember(d => d.FormattedUnitCost, o => o.MapFrom(s => s.UnitCost.ToCurrency()))
                .ForMember(d => d.FormattedLineCost, o => o.MapFrom(s => (s.UnitCost * s.QuantityOrdered).ToCurrency()))
                .ForMember(d => d.FormattedPendingCost, o => o.Ignore())
                .ForMember(x => x.QuantityPending, o => o.Ignore())
                .ForMember(x => x.QuantityCancelled, o => o.MapFrom(s => s.QuantityCancelled.HasValue ? s.QuantityCancelled.Value : 0))
                .ForMember(x => x.TotalQuantityReceived, o => o.Ignore());
            Mapper.CreateMap<PurchaseOrderViewModel, PurchaseOrder>()
                .ForMember(x => x.CommissionChargeFlag, o => o.Ignore())
                .ForMember(x => x.CommissionPercentage, o => o.Ignore())
                .ForMember(x => x.ExpiredDate, o => o.Ignore());
            Mapper.CreateMap<PurchaseOrder, PurchaseOrderViewModel>();
            Mapper.CreateMap<PurchaseOrderProduct, PurchaseOrderProductViewModel>().ForMember(x => x.PrintQuantity, o => o.Ignore()).ForMember(x => x.BoxCount, o => o.Ignore());
            Mapper.CreateMap<PurchaseOrderViewModel, PurchaseOrderPrintModel>().ForMember(x => x.LocationDetails, o => o.Ignore()).ForMember(x => x.VendorDetails, o => o.Ignore());
            Mapper.CreateMap<PurchaseOrderPrintModel, PurchaseOrderViewModel>()
                .ForMember(x => x.OriginSystem, o => o.Ignore())
                .ForMember(x => x.CorporatePoNumber, o => o.Ignore())
                .ForMember(x => x.ShipDate, o => o.Ignore())
                .ForMember(x => x.ShipVia, o => o.Ignore())
                .ForMember(x => x.PortOfLoading, o => o.Ignore())
                .ForMember(x => x.Attributes, o => o.Ignore());
        }

        private static void MapGoodsReceipt()
        {
            Mapper.CreateMap<GoodsReceipt, GoodsReceiptViewModel>()
                .ForMember(x => x.SalesLocationId, o => o.Ignore())
                .ForMember(x => x.VendorType, o => o.Ignore())
                .ForMember(x => x.EnableBackOrderCancel, o => o.Ignore())
                .ForMember(x => x.PurchaseOrders, o => o.Ignore());
            Mapper.CreateMap<GoodsReceipt, GoodsReceiptPrintModel>()
                .ForMember(d => d.DateApproved, o => o.MapFrom(s => s.DateApproved.ToLocalPrintDateString()))
                .ForMember(d => d.DateReceived, o => o.MapFrom(s => s.DateReceived.ToPrintDateString()))
                .ForMember(x => x.ProcessedById, o => o.Ignore())
                .ForMember(x => x.ProcessedBy, o => o.Ignore())
                .ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptCreateModel, GoodsReceipt>()
                .ForMember(x => x.ReceivedBy, o => o.Ignore())
                .ForMember(x => x.Location, o => o.Ignore())
                .ForMember(x => x.OriginalPrint, o => o.Ignore())
                .ForMember(x => x.CostConfirmed, o => o.Ignore())
                .ForMember(x => x.CostConfirmedById, o => o.Ignore())
                .ForMember(x => x.CostConfirmedBy, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.DateApproved, o => o.Ignore())
                .ForMember(x => x.ApprovedById, o => o.Ignore())
                .ForMember(x => x.ApprovedBy, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptProductView, GoodsReceiptProductViewModel>().ForMember(x => x.UnitCost, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptProductView, GoodsReceiptPurchaseOrderViewModel>().ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptProductCreateModel, GoodsReceiptProduct>()
                .ForMember(d => d.PurchaseOrderProductId, o => o.MapFrom(s => s.Id))
                .ForMember(x => x.LastLandedCost, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(x => x.GoodsReceiptId, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptProduct, GoodsReceiptProductCreateModel>().ForMember(d => d.Id, o => o.MapFrom(s => s.PurchaseOrderProductId));
            Mapper.CreateMap<GoodsReceipt, GoodsReceiptCostsModel>()
                .ForMember(x => x.ProcessedBy, o => o.Ignore())
                .ForMember(x => x.ProcessedById, o => o.Ignore())
                .ForMember(x => x.PurchaseOrders, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptViewModel, GoodsReceiptAuditModel>()
                .ForMember(d => d.DateApproved, o => o.MapFrom(s => s.DateApproved.ToAuditDateString()))
                .ForMember(d => d.DateReceived, o => o.MapFrom(s => s.DateApproved.ToAuditDateString()))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.DateApproved.ToAuditDateTimeString()))
                .ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptPurchaseOrderViewModel, GoodsReceiptPurchaseOrderAuditModel>();
            Mapper.CreateMap<GoodsReceiptProductViewModel, GoodsReceiptProductAuditModel>()
                .ForMember(d => d.EstimatedDeliveryDate, o => o.MapFrom(s => s.EstimatedDeliveryDate.ToAuditDateString()));
            Mapper.CreateMap<GoodsReceiptViewModel, GoodsReceiptMessage>()
                .ForMember(x => x.ReceiptId, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.VendorId, o => o.MapFrom(y => y.PurchaseOrders.First().VendorId))
                .ForMember(x => x.TotalLandedCost, o => o.MapFrom(y => y.PurchaseOrders.SelectMany(p => p.Products).Sum(p => p.LastLandedCost * p.QuantityReceived)))
                .ForMember(x => x.ReceiptType, o => o.UseValue(GoodsReceiptType.Standard))
                .ForMember(x => x.Products, o => o.Ignore())
                .ForMember(x => x.Description, o => o.MapFrom(y => string.Format("GRN#{0}", y.Id)));

            Mapper.CreateMap<ProductMessageView, GoodsReceiptMessageProduct>()
                .ForMember(x => x.Cost, o => o.Ignore())
                .ForMember(x => x.Units, o => o.Ignore());

            // Direct Receipts
            Mapper.CreateMap<GoodsReceiptDirect, GoodsReceiptDirectViewModel>().ForMember(x => x.SalesLocationId, o => o.Ignore()).ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptDirect, GoodsReceiptDirectPrintModel>()
                .ForMember(x => x.Products, o => o.Ignore())
                .ForMember(x => x.ProcessedBy, o => o.Ignore())
                .ForMember(x => x.ProcessedById, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptDirectCreateModel, GoodsReceiptDirect>()
                .ForMember(x => x.ReceivedBy, o => o.Ignore())
                .ForMember(x => x.Vendor, o => o.Ignore())
                .ForMember(x => x.Currency, o => o.Ignore())
                .ForMember(x => x.OriginalPrint, o => o.Ignore())
                .ForMember(x => x.Location, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.DateApproved, o => o.Ignore())
                .ForMember(x => x.ApprovedById, o => o.Ignore())
                .ForMember(x => x.ApprovedBy, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptDirectProductCreateModel, GoodsReceiptDirectProduct>()
                .ForMember(x => x.Sku, o => o.Ignore())
                .ForMember(x => x.Description, o => o.Ignore())
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.GoodsReceiptDirectId, o => o.Ignore())
                .ForMember(x => x.UnitLandedCost, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptDirectProduct, GoodsReceiptDirectProductPrintModel>().ForMember(x => x.Currency, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptDirectViewModel, GoodsReceiptMessage>()
                .ForMember(x => x.ReceiptId, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.VendorId, o => o.MapFrom(y => y.VendorId))
                .ForMember(x => x.TotalLandedCost, o => o.MapFrom(y => y.Products.Sum(p => p.UnitLandedCost * p.QuantityReceived)))
                .ForMember(x => x.ReceiptType, o => o.UseValue(GoodsReceiptType.Direct))
                .ForMember(x => x.Description, o => o.MapFrom(y => string.Format("GRD#{0}", y.Id)));
            Mapper.CreateMap<GoodsReceiptDirectProduct, GoodsReceiptMessageProduct>()
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Cost, o => o.Ignore())
                .ForMember(x => x.Units, o => o.Ignore());
        }

        private static void MapVendorReturn()
        {
            Mapper.CreateMap<VendorReturnProductView, VendorReturnProductViewModel>();

            Mapper.CreateMap<VendorReturn, VendorReturnNewModel>()
                .ForMember(x => x.Location, o => o.Ignore())
                .ForMember(x => x.LocationId, o => o.Ignore())
                .ForMember(x => x.DateReceived, o => o.Ignore())
                .ForMember(x => x.VendorDeliveryNumber, o => o.Ignore())
                .ForMember(x => x.VendorInvoiceNumber, o => o.Ignore())
                .ForMember(x => x.ReceivedBy, o => o.Ignore())
                .ForMember(x => x.PurchaseOrders, o => o.Ignore());
            Mapper.CreateMap<VendorReturnNewView, VendorReturnProductNewModel>()
                .ForMember(d => d.QuantityRemaining, o => o.MapFrom(s => s.QuantityReceived - s.QuantityPreviouslyReturned));

            Mapper.CreateMap<VendorReturnCreateModel, VendorReturn>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.CreatedById, o => o.Ignore())
                .ForMember(x => x.ApprovedById, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.ApprovedBy, o => o.Ignore())
                .ForMember(x => x.CreatedDate, o => o.Ignore())
                .ForMember(x => x.ApprovedDate, o => o.Ignore())
                .ForMember(x => x.ReceiptType, o => o.Ignore());
            Mapper.CreateMap<VendorReturnProductCreateModel, VendorReturnProduct>().ForMember(x => x.Id, o => o.Ignore()).ForMember(x => x.VendorReturnId, o => o.Ignore());

            Mapper.CreateMap<VendorReturnViewModel, VendorReturnPrintModel>().ForMember(x => x.GoodsReceipt, o => o.Ignore()).ForMember(x => x.FormattedTotalCost, o => o.Ignore());
            Mapper.CreateMap<VendorReturnPurchaseOrderViewModel, VendorReturnPurchaseOrderPrintModel>();
            Mapper.CreateMap<VendorReturnProductViewModel, VendorReturnProductPrintModel>()
                .ForMember(x => x.UnitLandedCost, o => o.Ignore())
                .ForMember(x => x.LineLandedCost, o => o.Ignore());

            Mapper.CreateMap<VendorReturnSearchView, VendorReturnSearchResultModel>();

            Mapper.CreateMap<VendorReturnView, VendorReturnViewModel>().ForMember(x => x.PurchaseOrders, o => o.Ignore());
            Mapper.CreateMap<GoodsReceipt, VendorReturnViewModel>()
                .ForMember(x => x.ApprovedDate, o => o.Ignore())
                .ForMember(x => x.GoodsReceiptId, o => o.Ignore())
                .ForMember(x => x.ReceiptType, o => o.Ignore())
                .ForMember(x => x.SalesLocationId, o => o.Ignore())
                .ForMember(x => x.PurchaseOrders, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptDirect, VendorReturnNewModel>()
                .ForMember(x => x.GoodsReceiptId, o => o.Ignore())
                .ForMember(x => x.ReceiptType, o => o.Ignore())
                .ForMember(x => x.PurchaseOrders, o => o.Ignore());
            Mapper.CreateMap<GoodsReceipt, VendorReturnNewModel>()
                .ForMember(r => r.GoodsReceiptId, o => o.MapFrom(x => x.Id))
                .ForMember(x => x.ReceiptType, o => o.Ignore())
                .ForMember(x => x.PurchaseOrders, o => o.Ignore());
            Mapper.CreateMap<GoodsReceiptDirect, VendorReturnDirectNewModel>()
                .ForMember(r => r.GoodsReceiptId, o => o.MapFrom(x => x.Id))
                .ForMember(x => x.ReceiptType, o => o.Ignore())
                .ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<VendorReturnDirectNewView, VendorReturnProductNewModel>()
                .ForMember(d => d.QuantityRemaining, o => o.MapFrom(s => s.QuantityReceived - s.QuantityPreviouslyReturned))
                .ForMember(x => x.LastLandedCost, o => o.MapFrom(s => s.UnitLandedCost));
            Mapper.CreateMap<VendorReturnView, VendorReturnDirectViewModel>().ForMember(x => x.ReceiptId, o => o.Ignore()).ForMember(x => x.Products, o => o.Ignore());
            Mapper.CreateMap<VendorReturnDirectProductView, VendorReturnProductViewModel>();
            Mapper.CreateMap<VendorReturnDirectViewModel, VendorReturnDirectPrintModel>()
                .ForMember(x => x.GoodsReceipt, o => o.Ignore())
                .ForMember(x => x.FormattedTotalCost, o => o.Ignore());
            Mapper.CreateMap<VendorReturnViewModel, VendorReturnMessage>()
                .ForMember(x => x.VendorId, o => o.MapFrom(y => y.PurchaseOrders.First().VendorId))
                .ForMember(x => x.VendorReturnId, o => o.MapFrom(y => y.Id))
                .ForMember(
                    x => x.TotalLandedCost,
                    o => o.MapFrom(y => y.PurchaseOrders.SelectMany(p => p.Products).Where(p => p.LastLandedCost != null).Sum(p => p.LastLandedCost * p.QuantityReturned)))
                .ForMember(x => x.VendorType, o => o.Ignore())
                .ForMember(x => x.Products, o => o.Ignore())
                .ForMember(x => x.Description, o => o.MapFrom(y => string.Format("VRN#{0}", y.Id)));
            Mapper.CreateMap<VendorReturnDirectViewModel, VendorReturnMessage>()
                .ForMember(x => x.VendorReturnId, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.TotalLandedCost, o => o.MapFrom(y => y.Products.Where(p => p.LastLandedCost != null).Sum(p => p.LastLandedCost * p.QuantityReturned)))
                .ForMember(x => x.Description, o => o.MapFrom(y => string.Format("VRD#{0}", y.Id)));
            Mapper.CreateMap<VendorReturnProductViewModel, Messages.Merchandising.VendorReturn.Product>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.ProductId))
                .ForMember(x => x.DepartmentCode, o => o.Ignore())
                .ForMember(x => x.Cost, o => o.Ignore())
                .ForMember(x => x.Units, o => o.Ignore());
            Mapper.CreateMap<ProductMessageView, Messages.Merchandising.VendorReturn.Product>()
                .ForMember(x => x.Cost, o => o.Ignore())
                .ForMember(x => x.Units, o => o.Ignore());
        }

        private static void MapExport()
        {
            Mapper.CreateMap<OrderExportView, PurchaseOrderExportModel>()
                .ForMember(d => d.WarehouseNo, o => o.MapFrom(s => s.WarehouseNo.PadRight(3)))
                .ForMember(d => d.DeliveryDate, o => o.MapFrom(s => s.DeliveryDate.HasValue ? s.DeliveryDate.Value.ToString("ddMMyyyy") : DateTime.Now.ToString("ddMMyyyy")))
                .ForMember(d => d.ItemNo, o => o.MapFrom(s => s.ItemNo.PadRight(8)))
                .ForMember(d => d.Supplier, o => o.MapFrom(s => s.Supplier.PadRight(10)))
                .ForMember(d => d.OrderNo, o => o.MapFrom(s => s.OrderNo.ToString().PadRight(10)))
                .ForMember(d => d.OrderQuantity, o => o.MapFrom(s => s.OrderQuantity.FormatForExport(7, 2)));

            Mapper.CreateMap<SetExportView, SetExportModel>()
                .ForMember(d => d.Parent, o => o.MapFrom(s => s.Parent.PadRight(8)))
                .ForMember(d => d.Child, o => o.MapFrom(s => s.Child.ToString().PadRight(8)))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity.FormatForExport(2, 2)));

            Mapper.CreateMap<StockExportView, StockLevelExportModel>()
                .ForMember(d => d.WarehouseNo, o => o.MapFrom(s => s.WarehouseNo.PadRight(3)))
                .ForMember(d => d.ItemNo, o => o.MapFrom(s => s.ItemNo.ToString().PadRight(8)))
                .ForMember(d => d.StockFactAvailable, o => o.MapFrom(s => s.StockFactAvailable.FormatForExport(7, 2)))
                .ForMember(d => d.StockActual, o => o.MapFrom(s => s.StockActual.FormatForExport(7, 2)))
                .ForMember(d => d.StockOnOrder, o => o.MapFrom(s => s.StockOnOrder.FormatForExport(7, 2)));

            Mapper.CreateMap<ProductExportView, ProductExportViewModel>();

            Mapper.CreateMap<ProductExportViewModel, ProductExportModel>()
                .ForMember(d => d.WarehouseNo, o => o.MapFrom(s => s.WarehouseNo.PadRight(3)))
                .ForMember(d => d.ItemNo, o => o.MapFrom(s => s.ItemNo.PadRight(8)))
                .ForMember(d => d.SupplierCode, o => o.MapFrom(s => s.SupplierCode.PadRight(18)))
                .ForMember(d => d.ItemDescr1, o => o.MapFrom(s => s.ItemDescr1.PadRight(30)))
                .ForMember(d => d.ItemDescr2, o => o.MapFrom(s => s.ItemDescr2.PadRight(45)))
                .ForMember(d => d.UnitPriceHP, o => o.MapFrom(s => s.UnitPriceHP.FormatForExport(8, 2)))
                .ForMember(d => d.UnitPriceCash, o => o.MapFrom(s => s.UnitPriceCash.FormatForExport(8, 2)))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.PadRight(4)))
                .ForMember(d => d.Supplier, o => o.MapFrom(s => s.Supplier.PadRight(12)))
                .ForMember(d => d.ProdStatus, o => o.MapFrom(s => s.ProdStatus.PadRight(2)))
                .ForMember(d => d.Warrantable, o => o.MapFrom(s => s.Warrantable.PadRight(2)))
                .ForMember(d => d.ProdType, o => o.MapFrom(s => s.ProdType.PadRight(2)))
                .ForMember(d => d.DutyFreePrice, o => o.MapFrom(s => s.DutyFreePrice.FormatForExport(8, 2)))
                .ForMember(d => d.RefCode, o => o.MapFrom(s => s.RefCode.PadRight(3)))
                .ForMember(d => d.BarCode, o => o.MapFrom(s => s.BarCode.PadRight(16)))
                .ForMember(d => d.LeadTime, o => o.MapFrom(s => s.LeadTime.PadRight(3)))
                .ForMember(d => d.WarrantyRenewalFlag, o => o.MapFrom(s => s.WarrantyRenewalFlag.PadRight(1)))
                .ForMember(d => d.AssemblyRequired, o => o.MapFrom(s => s.AssemblyRequired.PadRight(1)))
                .ForMember(d => d.Deleted, o => o.MapFrom(s => s.Deleted.PadRight(1)))
                .ForMember(d => d.CostPrice, o => o.MapFrom(s => s.CostPrice.FormatForExport(8, 2)))
                .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.SupplierName.PadRight(32)));
            //Change for ZEN/UNC/CRF/CR2018-011 Pricing Promotion - Happy Hour
            Mapper.CreateMap<PromotionProductModel, PromoExportViewModel>()
                .ForMember(d => d.ProductCode, o => o.MapFrom(s => s.Sku.PadRight(8)))
                .ForMember(d => d.WarehouseNumber, o => o.MapFrom(s => s.SalesId.PadRight(3)))
                .ForMember(d => d.HPDateFrom1, o => o.MapFrom(s => s.StartDate.ToString("ddMMyyHHmmss").PadRight(6)))
                .ForMember(d => d.HPDateFrom2, o => o.MapFrom(s => string.Empty.PadRight(6, '0')))
                .ForMember(d => d.HPDateFrom3, o => o.MapFrom(s => string.Empty.PadRight(6, '0')))
                .ForMember(d => d.HPDateTo1, o => o.MapFrom(s => s.EndDate.ToString("ddMMyyHHmmss").PadRight(6)))
                .ForMember(d => d.HPDateTo2, o => o.MapFrom(s => string.Empty.PadRight(6, '0')))
                .ForMember(d => d.HPDateTo3, o => o.MapFrom(s => string.Empty.PadRight(6, '0')))
                .ForMember(d => d.CashDateFrom1, o => o.MapFrom(s => s.StartDate.ToString("ddMMyyHHmmss").PadRight(6)))
                .ForMember(d => d.CashDateFrom2, o => o.MapFrom(s => string.Empty.PadRight(6, '0')))
                .ForMember(d => d.CashDateFrom3, o => o.MapFrom(s => string.Empty.PadRight(6, '0')))
                .ForMember(d => d.CashDateTo1, o => o.MapFrom(s => s.EndDate.ToString("ddMMyyHHmmss").PadRight(6)))
                .ForMember(d => d.CashDateTo2, o => o.MapFrom(s => string.Empty.PadRight(6, '0')))
                .ForMember(d => d.CashDateTo3, o => o.MapFrom(s => string.Empty.PadRight(6, '0')))
                .ForMember(d => d.HPPrice1, o => o.MapFrom(s => s.RegularPrice.FormatForExport(8, 2)))
                .ForMember(d => d.HPPrice2, o => o.MapFrom(s => 0.FormatForExport(8, 2)))
                .ForMember(d => d.HPPrice3, o => o.MapFrom(s => 0.FormatForExport(8, 2)))
                .ForMember(d => d.CashPrice1, o => o.MapFrom(s => s.CashPrice.FormatForExport(8, 2)))
                .ForMember(d => d.CashPrice2, o => o.MapFrom(s => 0.FormatForExport(8, 2)))
                .ForMember(d => d.CashPrice3, o => o.MapFrom(s => 0.FormatForExport(8, 2)));
        }

        private static void MapReports()
        {
            // Top SKU Report
            Mapper.CreateMap<TopSkuReportView, TopSkuProductViewModel>()
                .ForMember(x => x.Cost, o => o.Ignore())
                .ForMember(x => x.CumulativeValueDelivered, o => o.Ignore())
                .ForMember(x => x.CumulativeNetValueDelivered, o => o.Ignore())
                .ForMember(x => x.HierarchyTags, o => o.Ignore())
                .ForMember(x => x.Margin, o => o.Ignore())
                .ForMember(x => x.Contribution, o => o.Ignore());

            // Stock Valuation Report
            Mapper.CreateMap<StockValuationSummaryView, StockValuationSnapshot>().ForMember(x => x.Id, o => o.Ignore()).ForMember(x => x.SnapshotDateId, o => o.Ignore());

            // Sales Comparison Report
            Mapper.CreateMap<SalesComparisonReportView, SalesComparisonViewModel>()
                .ForMember(x => x.HierarchyTags, o => o.MapFrom(s => s.Tags))
                .ForMember(x => x.PromotionalRegularPrice, o => o.Ignore())
                .ForMember(x => x.PromotionalCashPrice, o => o.Ignore())
                .ForMember(x => x.ThisYear, o => o.Ignore())
                .ForMember(x => x.LastYear, o => o.Ignore());
            Mapper.CreateMap<SalesComparisonReportView, SalesComparisonViewModel>();

            // Promotion Report
            Mapper.CreateMap<PromotionReportView, PromotionReportViewModel>()
                .ForMember(x => x.CashPrice, o => o.Ignore())
                .ForMember(x => x.CashTotal, o => o.Ignore())
                .ForMember(x => x.CashMargin, o => o.Ignore());

            // Negative Stock Report
            Mapper.CreateMap<NegativeStockSnapshotView, NegativeStockReportModel>()
                .ForMember(x => x.Division, o => o.Ignore())
                .ForMember(x => x.Department, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore())
                .ForMember(x => x.LastTransactionType, o => o.Ignore())
                .ForMember(x => x.LastTransactionId, o => o.Ignore())
                   .ForMember(x => x.LastTransactionNarration, o => o.Ignore())
                .ForMember(x => x.LastTransactionDate, o => o.Ignore());
            Mapper.CreateMap<NegativeStockReportModel, NegativeStockExportItem>();

            //Stock Received Report
            Mapper.CreateMap<StockReceivedReportView, StockReceivedReportModel>()
                .ForMember(x => x.ExtendedLandedCost, o => o.MapFrom(y => y.ExtendedLandedCost ?? 0))
                .ForMember(x => x.LastLandedCost, o => o.MapFrom(y => y.LastLandedCost ?? 0))
                .ForMember(x => x.Quantity, o => o.MapFrom(y => y.Quantity ?? 0))
                .ForMember(x => x.ReferenceNumberCsl, o => o.MapFrom(y => new ReferenceLink { Label = y.Quantity > 0 ? "GR" : "VR", Id = y.ReferenceNumberCsl }))
                .ForMember(x => x.ReferenceNumberExport, o => o.MapFrom(y => string.Format("{0}{1}", (y.Quantity > 0 ? "GR" : "VR"), y.ReferenceNumberCsl)));

            // SUCR
            Mapper.CreateMap<SummaryUpdateControlReportView, SummaryUpdateControlReportExportModel>()
                .ForMember(x => x.Units, o => o.MapFrom(y => y.Units.HasValue ? y.Units.ToString() : string.Empty))
                .ForMember(x => x.Value, o => o.MapFrom(y => y.Value.HasValue ? y.Value.ToString() : string.Empty))
                .ForMember(x => x.RunNumber, o => o.MapFrom(y => y.RunNumber.HasValue ? y.RunNumber.ToString() : string.Empty));

            //Buyer Sales History
            Mapper.CreateMap<BuyerSalesHistoryProductViewModel, BuyerSalesHistoryExportModel>()
                .ForMember(x => x.AverageWeightedCost, o => o.MapFrom(y => y.AverageWeightedCost.HasValue ? y.AverageWeightedCost.Value : 0))
                .ForMember(x => x.CashPrice, o => o.MapFrom(y => y.CashPrice.HasValue ? y.CashPrice.Value : 0));

            //Warehouse Oversupply
            Mapper.CreateMap<WarehouseOversupplyProductViewModel, WarehouseOversupplyExportModel>()
                .ForMember(x => x.DateLastReceived, o => o.MapFrom(y => y.DateLastReceived.HasValue ? y.DateLastReceived.Value : DateTime.MinValue))
                .ForMember(x => x.Department, o => o.Ignore())
                .ForMember(x => x.Division, o => o.Ignore())
                .ForMember(x => x.Class, o => o.Ignore());

        }

        private static void CreateJsonCollectionMap<T>()
        {
            Mapper.CreateMap<string, List<T>>().ConvertUsing(JsonConvertHelper.DeserializeObjectOrDefault<List<T>>);
            Mapper.CreateMap<List<T>, string>().ConvertUsing(JsonConvertHelper.Serialize);
        }
    }
}