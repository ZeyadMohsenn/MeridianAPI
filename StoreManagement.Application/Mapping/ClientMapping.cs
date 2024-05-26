using StoreManagement.Bases;
using StoreManagement.Domain.Dtos.Client;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Entities.StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Mapping
{
    public class ClientMapping : MappingProfileBase
    {
        public ClientMapping()
        {
            CreateMap<AddClientDto, Client>()
                .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));

            CreateMap<PhonesDto, Phone>()
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Phone));

            CreateMap<Client, GetClientDto>()
                           .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                           .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count))
                           .ForMember(dest => dest.TotalNetPrice, opt => opt.MapFrom(src => src.Orders.Sum(o => o.TotalPrice)))
                           .ForMember(dest => dest.TotalPaidAmount, opt => opt.MapFrom(src => src.Orders.Sum(o => o.PaidAmount)))
                           .ForMember(dest => dest.TotalRemainedAmount, opt => opt.MapFrom(src => src.Orders.Sum(o => o.RemainedAmount)))
                           .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones))
                           .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));

            CreateMap<Phone, PhonesDto>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Number));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.NetPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => src.PaidAmount))
                .ForMember(dest => dest.RemainedAmount, opt => opt.MapFrom(src => src.RemainedAmount));

            CreateMap<Client, GetClientsDto>();
        }
    }
}
