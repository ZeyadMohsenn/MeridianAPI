using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Mapping;

public class ProductMapping : MappingProfileBase
{
    public ProductMapping()
    {
        CreateMap<AddProductDto, Product>();
        CreateMap<Product, GetProductDto>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(image => new GetImagesDto { ImageUrl = image.StoredFileName})));
        CreateMap<Product, GetProductsDto>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(image => new GetImagesDto { ImageUrl = image.StoredFileName})));
    }
}
