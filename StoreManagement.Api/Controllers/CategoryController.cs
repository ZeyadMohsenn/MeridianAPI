using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Login_Token;

namespace StoreManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class CategoryController : ApiControllersBase
    {
        private readonly ICategoryService _categoryServices;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryServices = categoryService;
        }
        [HttpPost]
        //[Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto addCategoryDto)
        {
            var category = await _categoryServices.AddCategory(addCategoryDto);
            return Ok(category);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            ServiceResponse<GetCategoryDto> category = await _categoryServices.GetCategory(id);
            return Ok(category);
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetCategories([FromQuery] GetAllCategoriesFilter categoriesFitler)
        {
            ServiceResponse<PaginationResponse<GetAllCategoriesDto>> response = await _categoryServices.GetCategoriesAsync(categoriesFitler);

            if (response.Success)
                return Ok(response);

            else
                return BadRequest(response);
        }
        [HttpGet("GetAllDropDown")]
        public async Task<IActionResult> GetCategoriesDropDownList()
        {
            ServiceResponse<List<DropDownCategoriesDto>> categories = await _categoryServices.GetCategoriesDropDownList();
            if (categories.Success)
                return Ok(categories);

            else
                return NotFound();
        }


        [HttpPut]
        //[Authorize(nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto categoryDto, Guid id)
        {
            var updatedCategory = await _categoryServices.UpdateCategory(categoryDto, id);

            if (updatedCategory == null)
                return NotFound("Category not found.");

            return Ok(updatedCategory);
        }



        [HttpDelete]
        //[Authorize(nameof(UserRole.Admin))]

        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            ServiceResponse<bool> deleteResponse = await _categoryServices.DeleteCategoryAsync(id);

            if (deleteResponse.Success)
                return Ok(new { message = "Category deleted successfully" });

            else
                return BadRequest(new { message = deleteResponse.Message });
        }
        [HttpPost("UploadImage")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadImage(Guid categoryId, IFormFile image)
        {
            var result = await  _categoryServices.UploadCategoryImage(categoryId, image);
            return Ok(result);  
        }




    }
}
