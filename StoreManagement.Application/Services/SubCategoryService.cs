using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;
using System.Linq.Expressions;

namespace StoreManagement.Application.Services;

public class SubCategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ServiceBase(), ISubCategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IBaseRepository<SubCategory> _subCategoryRepo = unitOfWork.GetRepository<SubCategory>();
    private readonly IBaseRepository<Category> _categoryRepo = unitOfWork.GetRepository<Category>();


    public async Task<ServiceResponse<bool>> AddSubCategory(AddSubCategoryDto addSubCategoryDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(addSubCategoryDto.Name))
                return new ServiceResponse<bool>() { Success = false, Message = "Empty Name" };

            addSubCategoryDto.Name = addSubCategoryDto.Name.Trim();

            var temp = await _subCategoryRepo.FindAsync(c => c.Name == addSubCategoryDto.Name);

            if (temp != null)
                return new ServiceResponse<bool>() { Success = false, Message = $"{addSubCategoryDto.Name} is already exisitng" };

            if (addSubCategoryDto.CategoryId == Guid.Empty)
                return new ServiceResponse<bool>() { Success = false, Message = "Subcategory Creation Failed: Please provide the proper Category Id " };

            var categoryDb = _categoryRepo.FindByID(addSubCategoryDto.CategoryId);

            if (categoryDb == null)
                return new ServiceResponse<bool>() { Success = false, Message = "Category not found" };


            //if (addSubCategoryDto.Description == null)
            //    addSubCategoryDto.Description = string.Empty;

            addSubCategoryDto.Description = addSubCategoryDto.Description?.Trim();


            SubCategory dbSubCategory = new()
            {
                Name = addSubCategoryDto.Name,
                Description = addSubCategoryDto.Description,
                Category_Id = addSubCategoryDto.CategoryId
            };

            await _subCategoryRepo.AddAsync(dbSubCategory);

            await _unitOfWork.CommitAsync();

            return new ServiceResponse<bool>()
            {
                Success = true,
                Message = "SubCategory Created Successfully",
                Data = true,
            };
        }
        catch (Exception ex)
        {
            return await LogError(ex, false);
        }
    }


    public async Task<ServiceResponse<GetSubCategoryDto>> GetSubCategory(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return new ServiceResponse<GetSubCategoryDto>() { Message = "Empty Date" };

            Expression<Func<SubCategory, bool>> filterPredicate = p => p.Id == id;

            SubCategory subCategory = await _subCategoryRepo.FindAsync(filterPredicate, Include: q => q.Include(p => p.Category));

            if (subCategory == null)
                return new ServiceResponse<GetSubCategoryDto>() { Success = false, Message = "SubCategory not found" };

            var getSubCategoryDto = new GetSubCategoryDto
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                Description = subCategory.Description,
                Photo = subCategory.Photo,
                CategoryId = subCategory.Category_Id,
                CategoryName = subCategory.Category?.Name
            };

            return new ServiceResponse<GetSubCategoryDto>() { Data = getSubCategoryDto, Success = true, Message = "Retrieved Successfully" };
        }

       
        catch (Exception)
        {
            return new ServiceResponse<GetSubCategoryDto>() { Data = null, Success = false, Message = "An error occurred while getting Subcategory" };
        }
    }
    public async Task<ServiceResponse<PaginationResponse<GetAllSubCategoriesDto>>> GetSubCategoriesAsync(GetAllSubCategoriesFilter subCategoriesFilter)
    {
        try
        {
            //var query = await _subCategoryRepo.GetAllQueryableAsync(filterPredicate: a => true);
            var query = _subCategoryRepo.GetAllQueryableAsync();

            if (subCategoriesFilter.CategoryId != Guid.Empty)
                query = query.Where(s => s.Category_Id == subCategoriesFilter.CategoryId);

            if (!string.IsNullOrEmpty(subCategoriesFilter.SubCategoryName))
                query = query.Where(subCategory => subCategory.Name.Contains(subCategoriesFilter.SubCategoryName));

            if (subCategoriesFilter.Is_Deleted.HasValue)
                query = query.Where(subCategory => subCategory.Is_Deleted == subCategoriesFilter.Is_Deleted);

            var count = await query.CountAsync();

            var subCategories = await query.Select(s => new GetAllSubCategoriesDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Photo = s.Photo,
                CategoryId = s.Category_Id,
                IsDeleted = s.Is_Deleted,
            }).Skip((subCategoriesFilter.PageNumber - 1) * subCategoriesFilter.PageSize)
              .Take(subCategoriesFilter.PageSize)
              .ToListAsync();

            //var subcategoriesDtoList = subCategories.Select(subcategory => new GetAllSubCategoriesDto
            //{
            //    Name = subcategory.Name,
            //    Description = subcategory.Description,
            //    Photo = subcategory.Photo,
            //    Id = subcategory.Id,
            //    CategoryId = subcategory.Category_Id,
            //    IsDeleted = subcategory.Is_Deleted,

            //}).ToList();

            var paginationResponse = new PaginationResponse<GetAllSubCategoriesDto>
            {
                Length = count,
                Collection = subCategories,
            };

            return new ServiceResponse<PaginationResponse<GetAllSubCategoriesDto>>
            {
                Data = paginationResponse,
                Message = "SubCategories retrieved successfully",
                Success = true
            };
        }
        catch (Exception)
        {
            return new ServiceResponse<PaginationResponse<GetAllSubCategoriesDto>> { Data = null, Success = false, Message = "An error occurred while retrieving SubCategories " };
        }
    }

    public async Task<ServiceResponse<bool>> UpdateSubCategory(UpdateSubCategoryDto subCategoryDto, Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

            subCategoryDto.Name = subCategoryDto.Name.Trim();

            subCategoryDto.Description = subCategoryDto.Description?.Trim();

            SubCategory dbSubCategory = await _subCategoryRepo.FindByIdAsync(id);

            var temp = await _categoryRepo.FindAsync(c => c.Name == subCategoryDto.Name);

            if (temp != null && temp.Name != dbSubCategory.Name)
                return new ServiceResponse<bool>() { Success = false, Message = $"{subCategoryDto.Name} already exisitng" };

            if (dbSubCategory == null)
            {
                return new ServiceResponse<bool>()
                {
                    Success = false,
                    Message = "SubCategory not found"
                };

            }

            if (dbSubCategory.Category_Id != subCategoryDto.CategoryId)
            {
                Category? category = _categoryRepo.FindByID(subCategoryDto.CategoryId);
                if (category == null)
                    return new ServiceResponse<bool>() { Success = false, Message = "Category not found" };
            }

            dbSubCategory.Category_Id = subCategoryDto.CategoryId;

            dbSubCategory.Name = subCategoryDto.Name;

            dbSubCategory.Description = subCategoryDto.Description;

            _subCategoryRepo.Update(dbSubCategory);

            await _unitOfWork.CommitAsync();

            return new ServiceResponse<bool>()
            {
                Success = true,
                Message = "SubCategory Updated Successfully"
            };
        }
        catch (Exception ex)
        {
            return await LogError(ex, false);
        }
    }
    public async Task<ServiceResponse<bool>> DeleteSubCategoryAsync(Guid id)
    {
        if (id == Guid.Empty)
            return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

        try
        {
            SubCategory subCategory = await _subCategoryRepo.FindByIdAsync(id);

            if (subCategory == null)
                return new ServiceResponse<bool> { Success = false, Message = "SubCategory not found" };

            _subCategoryRepo.Delete(subCategory);

            var affectedRows = await _unitOfWork.CommitAsync();

            return new ServiceResponse<bool> { Success = affectedRows > 0, Message = "SubCategory deleted successfully" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<bool> { Success = false, Message = "An error occurred while deleting the SubCategory" };
        }
    }

    public async Task<ServiceResponse<List<DropDownSubCategoriesDto>>> GetSubCategoriesDropDownList()
    {
        try
        {
            var subcategoriesQueryable = _subCategoryRepo.GetAllQueryableAsync();

            var subcategoriesDtoList = await subcategoriesQueryable
                .Select(subcategory => new DropDownSubCategoriesDto
                {
                    Id = subcategory.Id,
                    Name = subcategory.Name,
                })
                .ToListAsync();

            return new ServiceResponse<List<DropDownSubCategoriesDto>>
            {
                Data = subcategoriesDtoList,
                Message = "Categories retrieved successfully",
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<DropDownSubCategoriesDto>>
            {
                Data = null,
                Success = false,
                Message = "An error occurred while retrieving categories "
            };
        }
    }
}
