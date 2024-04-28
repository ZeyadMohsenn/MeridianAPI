using StoreManagement.Bases;
using StoreManagement.Bases.Domain;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Interfaces;

public interface ICategoryService
{
    Task<ServiceResponse<bool>> AddCategory(AddCategoryDto addCategoryDto);
    Task<ServiceResponse<Category>> GetCategory(Guid id);
    Task<ServiceResponse<PaginationResponse<Category>>> GetCategoriesAsync(PagingModel pagingModel);
    Task<ServiceResponse<bool>> UpdateCategory(UpdateCategoryDto categoryDto, Guid id);

    Task<ServiceResponse<bool>> DeleteCategoryAsync(Guid id);
}
