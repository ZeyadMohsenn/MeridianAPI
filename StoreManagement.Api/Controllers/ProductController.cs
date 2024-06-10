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
    public class ProductController : ApiControllersBase
    {
        private readonly IProductService _productServices;

        public ProductController(IProductService productService)
        {
            _productServices = productService;
        }
        [HttpPost]
        //[Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddProduct([FromBody] AddProductDto addProductDto)
        {
            var product = await _productServices.AddProduct(addProductDto);
            return Ok(product);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            ServiceResponse<GetProductDto> Product = await _productServices.GetProduct(id);
            return Ok(Product);
        }
        [HttpDelete]
        //[Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            ServiceResponse<bool> deleteResponse = await _productServices.DeleteProductAsync(id);

            if (deleteResponse.Success)
                return Ok(new { message = "Product deleted successfully" });

            else
                return BadRequest(new { message = deleteResponse.Message });
        }
        [HttpGet("GetAll"), Authorize(Roles = nameof(UserRole.Cashier))]

        public async Task<IActionResult> GetProducts([FromQuery] GetAllProductsFilter prodctsFitler)
        {
            ServiceResponse<PaginationResponse<GetProductsDto>> response = await _productServices.GetProductsAsync(prodctsFitler);

            if (response.Success)
                return Ok(response);

            else
                return BadRequest(response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto productDto, Guid id)
        {

            var updatedSubCategory = await _productServices.UpdateProduct(productDto, id);

            if (updatedSubCategory == null)
                return NotFound("Product not found.");

            return Ok(updatedSubCategory);
        }
        [HttpPut("Deactivate")]
        public async Task<IActionResult> SwitchProductActivation(Guid id)
        {
            ServiceResponse<bool> product = await _productServices.SwitchActivationProduct(id);

            if (product.Success)
                return Ok(product);

            return BadRequest(product);
        }

        [HttpPost("UploadImages")]
        public async Task<IActionResult> UploadImages(Guid productId,List<IFormFile> images)
        {
            var result = await _productServices.UploadImages(productId, images);
            return Ok(result);  
        }
    }
}
