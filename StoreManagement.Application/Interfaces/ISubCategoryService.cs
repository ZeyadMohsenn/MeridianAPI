using StoreManagement.Bases;
using StoreManagement.Bases.Domain;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Application.Interfaces
{
    public interface ISubCategoryService
    {
        Task<ServiceResponse<bool>> AddSubCategory(AddSubCategoryDto addSubCategoryDto);
        Task<ServiceResponse<SubCategory>> GetSubCategory(Guid id);
        Task<ServiceResponse<PaginationResponse<SubCategory>>> GetSubCategoriesAsync(PagingModel pagingModel);
        Task<ServiceResponse<bool>> UpdateSubCategory(UpdateSubCategoryDto subCategoryDto, Guid id);
        Task<ServiceResponse<bool>> DeleteSubCategoryAsync(Guid id);




    }
}
