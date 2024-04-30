using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResponse<bool>> AddProduct(AddProductDto addProductDto);
        Task<ServiceResponse<Product>> GetProduct(Guid id);
        Task<ServiceResponse<PaginationResponse<Product>>> GetProductsAsync(GetAllProductsFilter productFitler);

        Task<ServiceResponse<bool>> UpdateProduct(UpdateProductDto productDto, Guid id);

        Task<ServiceResponse<bool>> DeleteProductAsync(Guid id);
        Task<ServiceResponse<bool>> DeactivateProduct(Guid id);



    }
}
