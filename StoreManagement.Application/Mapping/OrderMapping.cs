using StoreManagement.Bases;
using StoreManagement.Domain.Dtos.Order;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Mapping;

public class OrderMapping : MappingProfileBase
{
    public OrderMapping()
    {
        CreateMap<AddOrderDto, Order>()
            .ForMember(dest => dest.DateTime, opt => opt.Ignore()) 
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()) 
            .ForMember(dest => dest.RemainedAmount, opt => opt.Ignore()) 
            .ForMember(dest => dest.Status, opt => opt.Ignore()) 
            .ForMember(dest => dest.Client_Id, opt => opt.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts));

        CreateMap<OrderProductDto, OrderProduct>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.OrderId, opt => opt.Ignore()); 


        CreateMap<Order, GetOrderDto>()
            .ForMember(dest => dest.Client_Name, opt => opt.MapFrom(src => src.Client.Name))
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Client_Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.NetPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.NumberOfPieces, opt => opt.MapFrom(src => src.OrderProducts.Sum(op => op.Quantity)))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderProducts))
            .AfterMap((src, dest) =>
            {
                dest.PriceBeforeTax = src.TotalPrice / (1 + src.TaxPercentage);
                dest.PriceBeforeDiscount = dest.PriceBeforeTax + src.Discount;
            });

        CreateMap<OrderProduct, ProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.PieceDiscountAmount, opt => opt.MapFrom(src => src.Product.Price * src.ProductDiscount))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        CreateMap<Order, GetOrdersDto>()
         .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Client_Id))
         .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
         .ForMember(dest => dest.PriceBeforeTax, opt => opt.MapFrom(src => src.TotalPrice / (1 + src.TaxPercentage)))
         .ForMember(dest => dest.PriceBeforeDiscount, opt => opt.MapFrom(src => (src.TotalPrice / (1 + src.TaxPercentage)) + src.Discount))
         .ForMember(dest => dest.NetPrice, opt => opt.MapFrom(src => src.TotalPrice))
         .ForMember(dest => dest.NumberOfPieces, opt => opt.MapFrom(src => src.OrderProducts.Sum(op => op.Quantity)));
    }
}

