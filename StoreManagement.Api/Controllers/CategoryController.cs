using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
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

        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            ServiceResponse<Category> category = await _categoryServices.GetCategory(id);
            return Ok(category);
        }


        [HttpGet("GetAll"), AllowAnonymous]
        public async Task<IActionResult> GetCategories([FromQuery] GetAllCategoriesFilter categoriesFitler)
        {
            ServiceResponse<PaginationResponse<Category>> response = await _categoryServices.GetCategoriesAsync(categoriesFitler);

            if (response.Success)
            {
                return Ok(response); 
            }
            else
            {
                return BadRequest(response); 
            }
        }


        [HttpPut, AllowAnonymous]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto categoryDto, Guid id)
        {
            


            var updatedCategory = await _categoryServices.UpdateCategory(categoryDto, id);

            if (updatedCategory == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(updatedCategory);
        }



        [HttpDelete, AllowAnonymous]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            ServiceResponse<bool> deleteResponse = await _categoryServices.DeleteCategoryAsync(id);

            if (deleteResponse.Success)
            {
                return Ok(new { message = "Category deleted successfully" });
            }
            else
            {
                return BadRequest(new { message = deleteResponse.Message });
            }
        }



    }
}
