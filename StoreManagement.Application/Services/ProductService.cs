using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : ServiceBase(), IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IBaseRepository<Product> _productRepo = unitOfWork.GetRepository<Product>();
        private readonly IBaseRepository<SubCategory> _subCategoryRepo = unitOfWork.GetRepository<SubCategory>();


        public async Task<ServiceResponse<bool>> AddProduct(AddProductDto addProductDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addProductDto.Name))
                    return new ServiceResponse<bool>() { Success = false, Message = "Empty Name" };
                addProductDto.Name = addProductDto.Name.Trim();

                var temp = await _productRepo.FindAsync(c => c.Name == addProductDto.Name);
                if (temp != null)

                    return new ServiceResponse<bool>() { Success = false, Message = $"{addProductDto.Name} is already exisitng" };

                if (addProductDto.SubCategory_Id == Guid.Empty)

                    return new ServiceResponse<bool>() { Success = false, Message = "Product Creation Failed: Please provide the proper SubCategory Id " };

                var categoryTempp = _subCategoryRepo.FindByID(addProductDto.SubCategory_Id);
                if (categoryTempp == null)
                    return new ServiceResponse<bool>() { Success = false, Message = "SubCategory not found" };


                if (addProductDto.Description == null)

                    addProductDto.Description = string.Empty;

                addProductDto.Description = addProductDto.Description.Trim();


                Product dbProduct = new()
                {
                    Name = addProductDto.Name,
                    Description = addProductDto.Description,
                    SubCategory_Id = addProductDto.SubCategory_Id,
                    Price = addProductDto.Price,
                    StockQuantity = addProductDto.StockQuantity,

                };

                await _productRepo.AddAsync(dbProduct);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>()
                {
                    Success = true,
                    Message = "Product Created Successfully",
                    Data = true,

                };
            }
            catch (Exception ex)
            {
                return await LogError(ex, false);
            }
        }

        public async Task<ServiceResponse<Product>> GetProduct(Guid id)
        {
            try
            {
                Product product = _productRepo.FindByID(id);
                if (product == null)
                    return new ServiceResponse<Product>() { Success = false, Message = "Product not found" };
                return new ServiceResponse<Product>() { Data = product, Success = true, Message = "Retrieved Successfully" };
            }
            catch (Exception)
            {
                return new ServiceResponse<Product>() { Data = null, Success = false, Message = "An error occurred while getting the Product" };
            }
        }
        public async Task<ServiceResponse<bool>> DeleteProductAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

            }
            try
            {
                Product Product = _productRepo.FindByID(id);
                if (Product == null)
                {
                    return new ServiceResponse<bool> { Success = false, Message = "Product not found" };
                }

                _productRepo.Remove(Product);
                var affectedRows = await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = affectedRows > 0, Message = "Product deleted successfully" };
            }
            catch (Exception ex)
            {


                return new ServiceResponse<bool> { Success = false, Message = "An error occurred while deleting the Product" };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<Product>>> GetProductsAsync(GetAllProductsFilter productFilter)
        {
            try
            {
                var query = _productRepo.GetAllQueryableAsync();

                if (productFilter.ProductId != Guid.Empty)
                {
                    query = query.Where(Product => Product.Id == productFilter.ProductId);
                }

                if (!string.IsNullOrEmpty(productFilter.ProductName))
                {
                    query = query.Where(p => p.Name.Contains(productFilter.ProductName));
                }

                if (productFilter.Is_Deleted)
                {
                    query = query.Where(p => p.Is_Deleted == productFilter.Is_Deleted);
                }




                if (!query.Any())
                    return new ServiceResponse<PaginationResponse<Product>> { Success = true, Message = "Products not found" };

                var count = await query.CountAsync();

                var products = await query.Skip((productFilter.PageNumber - 1) * productFilter.PageSize)
                                            .Take(productFilter.PageSize)
                                            .ToListAsync();

                var paginationResponse = new PaginationResponse<Product>
                {
                    Length = count,
                    Collection = products
                };

                return new ServiceResponse<PaginationResponse<Product>>
                {
                    Data = paginationResponse,
                    Message = "Products retrieved successfully",
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<PaginationResponse<Product>> { Data = null, Success = false, Message = "An error occurred while retrieving Products: " };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProduct(UpdateProductDto productDto, Guid id)
        {
            if (id == Guid.Empty)
            {
                return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

            }
            try
            {
                productDto.Name = productDto.Name.Trim();
                var temp = await _productRepo.FindAsync(c => c.Name == productDto.Name);
                if (temp is not null)

                    return new ServiceResponse<bool>() { Success = false, Message = $"{productDto.Name} already exisitng" };

                if (productDto.Description == null)

                    productDto.Description = string.Empty;

                productDto.Description = productDto.Description.Trim();

                Product dbProduct = _productRepo.FindByID(id);

                if (dbProduct == null)
                {
                    return new ServiceResponse<bool>()
                    {
                        Success = false,
                        Message = "Product not found"
                    };

                }
                dbProduct.Name = productDto.Name;
                dbProduct.Description = productDto.Description;
                dbProduct.Price = productDto.Price;
                dbProduct.StockQuantity = productDto.Quantity;
                _productRepo.Update(dbProduct);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>()
                {
                    Success = true,
                    Message = "Product Updated Successfully"
                };
            }
            catch (Exception ex)
            {
                return await LogError(ex, false);
            }
        }
    }
}


