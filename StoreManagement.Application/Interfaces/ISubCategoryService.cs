using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;

namespace StoreManagement.Application.Interfaces
{
    public interface ISubCategoryService
    {
        Task<ServiceResponse<bool>> AddSubCategory(AddSubCategoryDto addSubCategoryDto);
        Task<ServiceResponse<GetSubCategoryDto>> GetSubCategory(Guid id);
        Task<ServiceResponse<PaginationResponse<GetAllSubCategoriesDto>>> GetSubCategoriesAsync(GetAllSubCategoriesFilter subCategoriesFitler);
        Task<ServiceResponse<bool>> UpdateSubCategory(UpdateSubCategoryDto subCategoryDto, Guid id);
        Task<ServiceResponse<bool>> DeleteSubCategoryAsync(Guid id);
        Task<ServiceResponse<List<DropDownSubCategoriesDto>>> GetSubCategoriesDropDownList();

    }
}
