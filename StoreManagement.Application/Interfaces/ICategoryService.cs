using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Application.Interfaces
{
    public interface ICategoryService
    {
         Task<bool> AddCategory(AddCategoryDto addCategoryDto);
        Category GetCategory(Guid id);
        Task<List<Category>> GetCategoriesAsync();
        Category UpdateCategory(Category category, Guid id);

        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
