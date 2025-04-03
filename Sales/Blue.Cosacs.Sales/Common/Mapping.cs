using AutoMapper;
using Blue.Cosacs.Sales.Models;
using Blue.Cosacs.Sales.Repositories;

namespace Blue.Cosacs.Sales.Common
{
    public class Mapping
    {
        public static void MapEntites()
        {
            Mapper.CreateMap<Order, OrderDto>()
                .ForMember(dto => dto.Customer, m => m.MapFrom(s => s.OrderCustomer));

            Mapper.CreateMap<OrderItem, ItemDto>()
              .ForSourceMember(x => x.Order, y => y.Ignore())
              .ForSourceMember(x => x.OrderItems, y => y.Ignore())
              .ForSourceMember(x => x.ParentItem, y => y.Ignore());

            Mapper.CreateMap<OrderCustomer, CustomerDto>()
                .ForMember(dto => dto.TownOrCity, m => m.MapFrom(s => s.AddressLine3))
                .ForMember(dto => dto.HomePhoneNumber, m => m.MapFrom(s => s.HomePhone))
                .ForMember(dto => dto.MobileNumber, m => m.MapFrom(s => s.MobilePhone))
                .ForSourceMember(x => x.Order, y => y.Ignore());

            Mapper.CreateMap<OrderPayment, PaymentDto>()
                .ForSourceMember(x => x.Order, y => y.Ignore());

            Mapper.CreateMap<Order, OrderExtendedDto>()
                .ForMember(dto => dto.Customer, m => m.MapFrom(s => s.OrderCustomer));

            // the other way around
            Mapper.CreateMap<OrderDto, Order>()
                .ForMember(dto => dto.OrderCustomer, m => m.MapFrom(s => s.Customer));

            Mapper.CreateMap<ItemDto, OrderItem>()
                            .ForSourceMember(x => x.Id, y => y.Ignore())
                            .ForSourceMember(x => x.OrderId, y => y.Ignore())
                            .ForSourceMember(x => x.ParentId, y => y.Ignore());

            Mapper.CreateMap<CustomerDto, OrderCustomer>()
                .ForMember(dto => dto.AddressLine3, m => m.MapFrom(s => s.TownOrCity));

            Mapper.CreateMap<PaymentDto, OrderPayment>();

            Mapper.CreateMap<OrderCustomer, Messages.Customer>();
            Mapper.CreateMap<OrderItem,Messages.Item>();
            Mapper.CreateMap<OrderPayment, Messages.Payment>()
                .ForMember(dto => dto.MethodId, m => m.MapFrom(s => s.PaymentMethodId));
        }
    }
}
