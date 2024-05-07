using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
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
                return new ServiceResponse<bool>() { Success = false, Message = $"{addCategoryDto.Name} already exisitng" };

            addCategoryDto.Description = addCategoryDto.Description?.Trim();


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

    public async Task<ServiceResponse<GetCategoryDto>> GetCategory(Guid id)
    {
        try
        {
            Category? category = await _categoryRepo.FindByIdAsync(id);

            if (category is null)
                return new ServiceResponse<GetCategoryDto>() { Success = false, Message = "Category not found" };


            GetCategoryDto categoryDto = new()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Photo = category.Photo,
                IsDeleted = category.Is_Deleted
            };

            return new ServiceResponse<GetCategoryDto>()
            {
                Data = categoryDto,
                Success = true,
                Message = "Retrieved Successfully"
            };

        }
        catch (Exception)
        {
            return new ServiceResponse<GetCategoryDto>() { Data = null, Success = false, Message = "An error occurred while getting category" };
        }
    }

    public async Task<ServiceResponse<PaginationResponse<GetAllCategoriesDto>>> GetCategoriesAsync(GetAllCategoriesFilter categoriesFitler)
    {
        try
        {
            IQueryable<Category> query = _categoryRepo.GetAllQueryableAsync();

            if (!string.IsNullOrEmpty(categoriesFitler.CategoryName))
                query = query.Where(category => category.Name.Contains(categoriesFitler.CategoryName));

            if (categoriesFitler.Is_Deleted.HasValue)
                query = query.Where(category => category.Is_Deleted == categoriesFitler.Is_Deleted);

            var count = await query.CountAsync();

            var categories = await query.Select(category => new GetAllCategoriesDto
                                        {
                                            Name = category.Name,
                                            Description = category.Description,
                                            Photo = category.Photo,
                                            Id = category.Id,
                                            IsDeleted = category.Is_Deleted,
                                        })
                                        .Skip((categoriesFitler.PageNumber - 1) * categoriesFitler.PageSize)
                                        .Take(categoriesFitler.PageSize)
                                        .ToListAsync();

            var paginationResponse = new PaginationResponse<GetAllCategoriesDto>
            {
                Length = count,
                Collection = categories
            };

            return new ServiceResponse<PaginationResponse<GetAllCategoriesDto>>
            {
                Data = paginationResponse,
                Message = "Categories retrieved successfully",
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<PaginationResponse<GetAllCategoriesDto>> { Data = null, Success = false, Message = "An error occurred while retrieving categories: " + ex.Message };
        }
    }

    public async Task<ServiceResponse<bool>> UpdateCategory(UpdateCategoryDto categoryDto, Guid id)
    {
        if (id == Guid.Empty)
            return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };
        try
        {
            categoryDto.Name = categoryDto.Name.Trim();

            categoryDto.Description = categoryDto.Description?.Trim();

            Category? dbCategory = _categoryRepo.FindByID(id);

            if(dbCategory is null)
                return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

            var temp = await _categoryRepo.FindAsync(c => c.Name == categoryDto.Name && c.Id != dbCategory.Id);

            if (temp != null)
                return new ServiceResponse<bool>() { Success = false, Message = $"{categoryDto.Name} already exisitng" };

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
        catch (Exception ex)
        {
            return await LogError(ex, false);
        }
    }

    public async Task<ServiceResponse<bool>> DeleteCategoryAsync(Guid id)
    {
        if (id == Guid.Empty)
            return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };
        try
        {
            Category category = _categoryRepo.FindByID(id);

            if (category == null)
                return new ServiceResponse<bool> { Success = false, Message = "Category not found" };

            _categoryRepo.Delete(category);

            var affectedRows = await _unitOfWork.CommitAsync();

            return new ServiceResponse<bool> { Success = affectedRows > 0, Message = "Category deleted successfully" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<bool> { Success = false, Message = "An error occurred while deleting the category" };
        }
    }

    public async Task<ServiceResponse<List<DropDownCategoriesDto>>> GetCategoriesDropDownList()
    {
        try
        {
            var queryableCategories = await _categoryRepo.GetAllQueryableAsync(filterPredicate: a => true);

            var categoriesDtoList = await queryableCategories
                .Select(category => new DropDownCategoriesDto
                {
                    Name = category.Name,
                    Id = category.Id,
                })
                .ToListAsync();


            return new ServiceResponse<List<DropDownCategoriesDto>>
            {
                Data = categoriesDtoList,
                Message = "Categories retrieved successfully",
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<DropDownCategoriesDto>>
            {
                Data = null,
                Success = false,
                Message = "An error occurred while retrieving categories "
            };
        }
    }

}
