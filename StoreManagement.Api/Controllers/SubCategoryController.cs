﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Application.Services;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;

namespace StoreManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class SubCategoryController : ApiControllersBase
    {
        private readonly ISubCategoryService _subCategoryServices;

        public SubCategoryController(ISubCategoryService subCategoryService)
        {
            _subCategoryServices = subCategoryService;
        }
        [HttpPost]
        //[Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddSubCategory([FromBody] AddSubCategoryDto addSubCategoryDto)
        {
            var subCategory = await _subCategoryServices.AddSubCategory(addSubCategoryDto);
            return Ok(subCategory);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategory(Guid id)
        {
            ServiceResponse<GetSubCategoryDto> subCategory = await _subCategoryServices.GetSubCategory(id);
            return Ok(subCategory);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetSubCategories([FromQuery] GetAllSubCategoriesFilter subCategoriesFitler)
        {
            ServiceResponse<PaginationResponse<GetAllSubCategoriesDto>> response = await _subCategoryServices.GetSubCategoriesAsync(subCategoriesFitler);

            if (response.Success)
                return Ok(response);

            else
                return BadRequest(response);
        }
        [HttpGet("GetAllDropDown")]

        public async Task<IActionResult> GetSubCategoriesDropDownList()
        {
            ServiceResponse<List<DropDownSubCategoriesDto>> subcategories = await _subCategoryServices.GetSubCategoriesDropDownList();
            if (subcategories.Success)
                return Ok(subcategories);

            else
                return NotFound();
        }
        [HttpPut]
        //[Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateSubCategory(UpdateSubCategoryDto subCategoryDto, Guid id)
        {

            var updatedSubCategory = await _subCategoryServices.UpdateSubCategory(subCategoryDto, id);

            if (updatedSubCategory == null)
                return NotFound("Category not found.");

            return Ok(updatedSubCategory);
        }
        [HttpDelete]
        //[Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteSubCategory(Guid id)
        {
            ServiceResponse<bool> deleteResponse = await _subCategoryServices.DeleteSubCategoryAsync(id);

            if (deleteResponse.Success)
                return Ok(new { message = "SubCategory deleted successfully" });

            else
                return BadRequest(new { message = deleteResponse.Message });
        }
        [HttpPost("UploadImage")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadImage(Guid subCategoryId, IFormFile image)
        {
            var result = await _subCategoryServices.UploadSubCategoryImage(subCategoryId, image);
            return Ok(result);
        }

    }
}
