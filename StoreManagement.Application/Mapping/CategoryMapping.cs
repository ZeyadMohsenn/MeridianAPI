using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Mapping;

public class CategoryMapping : MappingProfileBase
{
    public CategoryMapping()
    {
        CreateMap<AddCategoryDto, Category>();

        CreateMap<Category, GetCategoryDto>();

        CreateMap<Category, GetAllCategoriesDto>();

        CreateMap<Category, DropDownCategoriesDto>();

    }
}
