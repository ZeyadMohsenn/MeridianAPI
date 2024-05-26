using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Mapping;

public class SubCategoryMapping : MappingProfileBase
{
    public SubCategoryMapping()
    {
        CreateMap<AddSubCategoryDto, SubCategory>();
        CreateMap<SubCategory, GetSubCategoryDto>();
        CreateMap<SubCategory, GetAllSubCategoriesDto>()
            .ForMember(des => des.CategoryName, src => src.MapFrom(subCategory => subCategory.Category.Name));
        CreateMap<SubCategory, DropDownSubCategoriesDto>();

    }


}
