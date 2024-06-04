using Microsoft.AspNetCore.Http;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;

namespace StoreManagement.Application.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResponse<bool>> AddProduct(AddProductDto addProductDto);
        Task<ServiceResponse<GetProductDto>> GetProduct(Guid id);
        Task<ServiceResponse<PaginationResponse<GetProductsDto>>> GetProductsAsync(GetAllProductsFilter productFitler);
        Task<ServiceResponse<bool>> UpdateProduct(UpdateProductDto productDto, Guid id);
        Task<ServiceResponse<bool>> DeleteProductAsync(Guid id);
        Task<ServiceResponse<bool>> SwitchActivationProduct(Guid id);
        Task<ServiceResponse<bool>> UploadImages(Guid productId,List<IFormFile> images);

    }
}
