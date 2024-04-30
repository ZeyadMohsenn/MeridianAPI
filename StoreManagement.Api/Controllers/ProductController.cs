﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class ProductController : ApiControllersBase
    {
        private readonly IProductService _productServices;

        public ProductController(IProductService productService)
        {
            _productServices = productService;
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto addProductDto)
        {
            var product = await _productServices.AddProduct(addProductDto);
            return Ok(product);
        }
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            ServiceResponse<Product> Product = await _productServices.GetProduct(id);
            return Ok(Product);
        }
        [HttpDelete, AllowAnonymous]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            ServiceResponse<bool> deleteResponse = await _productServices.DeleteProductAsync(id);

            if (deleteResponse.Success)
            {
                return Ok(new { message = "Product deleted successfully" });
            }
            else
            {
                return BadRequest(new { message = deleteResponse.Message });
            }
        }
        [HttpGet("GetAll"), AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] GetAllProductsFilter prodctsFitler)
        {
            ServiceResponse<PaginationResponse<Product>> response = await _productServices.GetProductsAsync(prodctsFitler);

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
        public async Task<IActionResult> UpdateProduct(UpdateProductDto productDto, Guid id)
        {

            var updatedSubCategory = await _productServices.UpdateProduct(productDto, id);

            if (updatedSubCategory == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(updatedSubCategory);
        }
        [HttpPut("Deactivate"), AllowAnonymous]
        public async Task<IActionResult> DeactivateProduct(Guid id)
        {
            ServiceResponse<bool> product = await _productServices.DeactivateProduct(id);
            if(product.Success)
            {
                return Ok(product);
            }
            return BadRequest(product);
        }
    }
}