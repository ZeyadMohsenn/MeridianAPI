using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Mapping;

public class ProductMapping : MappingProfileBase
{
    public ProductMapping()
    {
        CreateMap<AddProductDto, Product>();
        CreateMap<Product, GetProductDto>();
        CreateMap<Product, GetProductsDto>();

    }
}
