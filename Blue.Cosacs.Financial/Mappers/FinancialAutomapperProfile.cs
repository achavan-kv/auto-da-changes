namespace Blue.Cosacs.Financial.Mappers
{
    using AutoMapper;
    using Blue.Cosacs.Financial.Models;
    using Messages.Merchandising.GoodsReceipt;
    using Messages.Merchandising.Transfer;
    using Blue.Cosacs.Financial;

    public class FinancialAutomapperProfile : Profile
    {
        protected override void Configure()
        {
            MapMessages();
            base.Configure();
        }

        private void MapMessages()
        {
            Mapper.CreateMap<Messages.Merchandising.CintOrderReceipt.CintOrderReceiptMessage, CintOrderReceiptMessage>()
                .ForMember(m => m.SalesLocationId, o => o.MapFrom(v => v.SaleLocationId))
                .ForMember(m => m.SalesType, o => o.MapFrom(v => v.SaleType));

            Mapper.CreateMap<Messages.Merchandising.GoodsReceipt.GoodsReceiptMessage, Financial.GoodsReceiptMessage>();
            Mapper.CreateMap<GoodsReceiptMessageProduct, GoodsReceiptProductMessage>()
                .ForMember(m => m.ProductId, o => o.MapFrom(v => v.Id));

            Mapper.CreateMap<Messages.Merchandising.StockAdjustment.StockAdjustmentMessage, StockAdjustmentMessage>();
            Mapper.CreateMap<Messages.Merchandising.StockAdjustment.StockAdjustmentMessageProduct, StockAdjustmentProductMessage>()
                .ForMember(m => m.ProductId, o => o.MapFrom(v => v.Id));

            Mapper.CreateMap<Blue.Cosacs.Messages.Merchandising.Transfer.TransferMessage, Financial.TransferMessage>()
                .ForMember(m => m.LocationId, o => o.MapFrom(t => t.WarehouseLocationId))
                .ForMember(m => m.SalesLocationId, o => o.MapFrom(t => t.WarehouseLocationSalesId));
            Mapper.CreateMap<TransferMessageProduct, TransferProductMessage>()
                .ForMember(m => m.ProductId, o => o.MapFrom(v => v.Id));

            Mapper.CreateMap<Messages.Merchandising.VendorReturn.VendorReturnMessage, VendorReturnMessage>();
            Mapper.CreateMap<Messages.Merchandising.VendorReturn.Product, VendorReturnProductMessage>()
                .ForMember(m => m.ProductId, o => o.MapFrom(v => v.Id));

            Mapper.CreateMap<TransactionView, FinancialQueryViewModel>()
                .ForMember(m => m.TransactionId, o => o.MapFrom(x => x.Id))
                .ForMember(m => m.RunNumber, o => o.MapFrom(x => x.RunNo))
                .ForMember(m => m.AccountNumber, o => o.MapFrom(x => x.Account))
                .ForMember(m => m.TransactionCode, o => o.MapFrom(x => x.Type))
                .ForMember(m => m.TransactionValue, o => o.MapFrom(x => x.Amount))
                .ForMember(m => m.TransactionDate, o => o.MapFrom(x => x.Date))
                .ForMember(m => m.OriginalTransactionId, o => o.MapFrom(x => x.Description));
        }
    }
}
