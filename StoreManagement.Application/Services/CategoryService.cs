using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ServiceBase(), ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IBaseRepository<Category> _categoryRepo = unitOfWork.GetRepository<Category>();

    public async Task<ServiceResponse<bool>> AddCategory(AddCategoryDto addCategoryDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(addCategoryDto.Name))
                return new ServiceResponse<bool>() { Success = false, Message = "Empty Name" };
            addCategoryDto.Name = addCategoryDto.Name.Trim();

            var temp = await _categoryRepo.FindAsync(c => c.Name == addCategoryDto.Name);
            if (temp != null)
            {
                return new ServiceResponse<bool>() { Success = false, Message = $"{addCategoryDto.Name} already exisitng" };
            }
            if (addCategoryDto.Description == null)
            {
                addCategoryDto.Description = string.Empty;
            }
            addCategoryDto.Description = addCategoryDto.Description.Trim();


            Category dbCategory = new()
            {
                Name = addCategoryDto.Name,
                Description = addCategoryDto.Description
            };

            await _categoryRepo.AddAsync(dbCategory);
            await _unitOfWork.CommitAsync();

            return new ServiceResponse<bool>()
            {
                Success = true,
                Message = "Data Created Successfully"
            };
        }
        catch (Exception ex)
        {
            return await LogError(ex, false);
        }
    }




    public async Task<ServiceResponse<Category>> GetCategory(Guid id)
    {
        try
        {
            Category category = _categoryRepo.FindByID(id);
            if (category != null)
            {
                return new ServiceResponse<Category>() { Data = category, Success = true, Message = "Retrieved Successfully" };
            }
            else
            {
                return new ServiceResponse<Category>() { Success = false, Message = "Category not found" };
            }
        }
        catch (Exception)
        {
            return new ServiceResponse<Category>() {Data = null, Success = false, Message = "An error occurred while getting category" };
        }

    }

    public async Task<ServiceResponse<PaginationResponse<Category>>> GetCategoriesAsync(PagingModel pagingModel)
    {
        try
        {
            var query = await _categoryRepo.GetAllQueryableAsync(filterPredicate: a => true);

            if (!query.Any())
                return new ServiceResponse<PaginationResponse<Category>> { Success = true, Message = "Categories not found" };

            var count = await query.CountAsync();

            var categories = await query.Skip((pagingModel.PageNumber - 1) * pagingModel.PageSize)
                                        .Take(pagingModel.PageSize)
                                        .ToListAsync();

            var paginationResponse = new PaginationResponse<Category>
            {
                Length = count,
                Collection = categories
            };

            return new ServiceResponse<PaginationResponse<Category>>
            {
                Data = paginationResponse,
                Message = "Categories retrieved successfully",
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<PaginationResponse<Category>> { Data = null, Success = false, Message = "An error occurred while retrieving categories: " + ex.Message };
        }
    }

    public async Task<ServiceResponse<bool>> UpdateCategory(UpdateCategoryDto categoryDto, Guid id)
    {
        if (id == Guid.Empty)
        {
            return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

        }
        try
        {
            categoryDto.Name = categoryDto.Name.Trim();
            var temp = await _categoryRepo.FindAsync(c => c.Name == categoryDto.Name);
            if (temp != null)
            {
                return new ServiceResponse<bool>() { Success = false, Message = $"{categoryDto.Name} already exisitng" };
            }
            if (categoryDto.Description == null)
            {
                categoryDto.Description = string.Empty;
            }
            categoryDto.Description = categoryDto.Description.Trim();

            Category dbCategory = _categoryRepo.FindByID(id);

            if (dbCategory != null)
            {
                dbCategory.Name = categoryDto.Name;
                dbCategory.Description = categoryDto.Description;
                _categoryRepo.Update(dbCategory);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>()
                {
                    Success = true,
                    Message = "Category Updated Successfully"
                };
            }
            else
            {
                return new ServiceResponse<bool>()
                {
                    Success = false,
                    Message = "Category not found"
                };
            }
        }
        catch (Exception ex)
        {
            return await LogError(ex, false);
        }
    }



    public async Task<ServiceResponse<bool>> DeleteCategoryAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

        }
        try
        {
            Category category = _categoryRepo.FindByID(id);
            if (category == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Category not found" };
            }

            _categoryRepo.Remove(category);
            var affectedRows = await _unitOfWork.CommitAsync();

            return new ServiceResponse<bool> { Success = affectedRows > 0, Message = "Category deleted successfully" };
        }
        catch (Exception ex)
        {


            return new ServiceResponse<bool> { Success = false, Message = "An error occurred while deleting the category" };
        }
    }




}
