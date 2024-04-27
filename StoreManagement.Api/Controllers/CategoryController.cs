using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CategoryController : ApiControllersBase
    {
        private readonly ICategoryService _categoryServices;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryServices = categoryService;
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto addCategoryDto)
        {
            var category = await _categoryServices.AddCategory(addCategoryDto);
            return Ok(category);
        }
        [HttpGet, AllowAnonymous]
        public async

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            List<Category> categories = await _categoryServices.GetCategoriesAsync();
            return Ok(categories);
        }


        [HttpDelete, AllowAnonymous]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            
            var isDeleted = await _categoryServices.DeleteCategoryAsync(id);

            if (isDeleted)
            {
                return Ok(new { message = "Category deleted successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to delete category" });
            }
        }


    }
}
