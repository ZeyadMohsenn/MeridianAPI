using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;
using System.Linq.Expressions;

namespace StoreManagement.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService) : ServiceBase(), IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IBaseRepository<Product> _productRepo = unitOfWork.GetRepository<Product>();
        private readonly IBaseRepository<SubCategory> _subCategoryRepo = unitOfWork.GetRepository<SubCategory>();
        private readonly IBaseRepository<Domain.Entities.Image> _imageRepo = unitOfWork.GetRepository<Domain.Entities.Image>();
        private readonly IImageService _imageService = imageService;




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

                var categoryDb = _subCategoryRepo.FindByID(addProductDto.SubCategory_Id);

                if (categoryDb == null)
                    return new ServiceResponse<bool>() { Success = false, Message = "SubCategory not found" };

                addProductDto.Description = addProductDto.Description?.Trim();


                Product dbProduct = _mapper.Map<Product>(addProductDto);

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

        public async Task<ServiceResponse<GetProductDto>> GetProduct(Guid id)
        {
            try
            {
                Expression<Func<Product, bool>> filterPredicate = p => p.Id == id;

                Product product = await _productRepo.FindAsync(filterPredicate, Include: q => q.Include(p => p.SubCategory).Include(p => p.Images), asNoTracking: true);

                if (product == null)
                    return new ServiceResponse<GetProductDto>() { Success = false, Message = "Product not found" };

                var getProductDto = _mapper.Map<GetProductDto>(product);

                foreach (var image in getProductDto.Images)
                {
                    if (image.ImageUrl != null)
                        image.ImageUrl = _imageService.GetImageUrl(image.ImageUrl);
                }

                return new ServiceResponse<GetProductDto>() { Data = getProductDto, Success = true, Message = "Retrieved Successfully" };
            }
            catch (Exception)
            {
                return new ServiceResponse<GetProductDto>() { Data = null, Success = false, Message = "An error occurred while getting the Product" };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<GetProductsDto>>> GetProductsAsync(GetAllProductsFilter productFilter)
        {
            try
            {
                IQueryable<Product> query = _productRepo.GetAllQueryableAsync();

                if (productFilter.SubCategoryId != Guid.Empty)
                    query = query.Where(p => p.SubCategory_Id == productFilter.SubCategoryId);

                if (!string.IsNullOrEmpty(productFilter.ProductName))
                    query = query.Where(p => p.Name.Contains(productFilter.ProductName));

                if (productFilter.Is_Deleted)
                    query = query.Where(p => p.Is_Deleted == productFilter.Is_Deleted);

                var count = await query.CountAsync();

                query = query.Include(p => p.SubCategory).Include(p => p.Images);

                var products = await query.Skip((productFilter.PageNumber - 1) * productFilter.PageSize)
                                          .Take(productFilter.PageSize)
                                          .ToListAsync();

                var mappedProducts = products.Select(p =>
                {
                    var getProductDto = _mapper.Map<GetProductsDto>(p);
                    foreach (var image in getProductDto.Images)
                    {
                        if (image.ImageUrl != null)
                            image.ImageUrl = _imageService.GetImageUrl(image.ImageUrl);
                    }
                    return getProductDto;
                }).ToList();

                var paginationResponse = new PaginationResponse<GetProductsDto>
                {
                    Length = count,
                    Collection = mappedProducts
                };

                return new ServiceResponse<PaginationResponse<GetProductsDto>>
                {
                    Data = paginationResponse,
                    Message = "Products retrieved successfully",
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<PaginationResponse<GetProductsDto>> { Data = null, Success = false, Message = "An error occurred while retrieving Products" };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProduct(UpdateProductDto productDto, Guid id)
        {

            try
            {
                if (id == Guid.Empty)
                    return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

                productDto.Name = productDto.Name.Trim();

                if (productDto.Description == null)
                    productDto.Description = string.Empty;

                productDto.Description = productDto.Description.Trim();

                Product dbProduct = await _productRepo.FindByIdAsync(id);

                var temp = await _productRepo.FindAsync(c => c.Name == productDto.Name);
                if (temp != null && temp.Name != dbProduct.Name)
                    return new ServiceResponse<bool>() { Success = false, Message = $"{productDto.Name} already exisitng" };

                if (dbProduct == null)
                {
                    return new ServiceResponse<bool>()
                    {
                        Success = false,
                        Message = "Product not found"
                    };

                }
                if (dbProduct.SubCategory_Id != productDto.SubCategoryId)
                {
                    SubCategory? subCategory = _subCategoryRepo.FindByID(productDto.SubCategoryId);
                    if (subCategory == null)
                        return new ServiceResponse<bool>() { Success = false, Message = "SubCategory not found" };
                }

                dbProduct.SubCategory_Id = productDto.SubCategoryId;
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
        public async Task<ServiceResponse<bool>> DeleteProductAsync(Guid id)
        {
            if (id == Guid.Empty)
                return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };
            try
            {
                var product = _productRepo.FindByID(id);

                if (product == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Product not found" };

                _productRepo.Delete(product);

                var affectedRows = await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = affectedRows > 0, Message = "Product deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = "An error occurred while deleting the Product" };
            }
        }

        public async Task<ServiceResponse<bool>> SwitchActivationProduct(Guid id)
        {
            if (id == Guid.Empty)
                return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

            try
            {
                Product product = _productRepo.FindByID(id);

                if (product == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Product not found" };

                if (product.StockQuantity == 0 & product.isActive == false)
                    return new ServiceResponse<bool> { Success = false, Message = "Product out of stock and can't be activate" };

                product.isActive = !product.isActive;
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = true };
            }
            catch (Exception)
            {
                return new ServiceResponse<bool> { Success = false, Message = "An error occurred " };
            }
        }

        public async Task<ServiceResponse<bool>> UploadImages(Guid productId, List<IFormFile> images)
        {
            try
            {
                if (productId == Guid.Empty)
                    return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

                if (images.Count == 0)
                    return new ServiceResponse<bool>() { Success = false, Message = " Please attach an image" };

                Expression<Func<Product, bool>> filterPredicate = p => p.Id == productId;

                Product? dbProduct = await _productRepo.FindAsync(filterPredicate, Include: q => q.Include(c => c.Images));

                if (dbProduct == null)
                    return new ServiceResponse<bool>() { Success = false, Message = " Product is not found" };

                foreach (var image in images)
                {
                    string storedFileName = await _imageService.UploadImage(nameof(Product), image);
                    dbProduct.Images.Add(new Domain.Entities.Image { StoredFileName = storedFileName });
                }

                _productRepo.Update(dbProduct);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>() { Success = true, Message = "Images uploaded successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Success = false, Message = $"An error occurred: {ex.Message}" };
            }

        }
    }
}



