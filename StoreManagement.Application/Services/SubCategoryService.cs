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

namespace StoreManagement.Application.Services;

public class SubCategoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IImageService imageService) : ServiceBase(), ISubCategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IBaseRepository<SubCategory> _subCategoryRepo = unitOfWork.GetRepository<SubCategory>();
    private readonly IBaseRepository<Category> _categoryRepo = unitOfWork.GetRepository<Category>();
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IImageService _imageService = imageService;



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


            addSubCategoryDto.Description = addSubCategoryDto.Description?.Trim();


            SubCategory dbSubCategory = _mapper.Map<SubCategory>(addSubCategoryDto);

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

            var getSubCategoryDto = _mapper.Map<GetSubCategoryDto>(subCategory);

            if(subCategory.StoredFileName != null)
                 getSubCategoryDto.ImageUrl =  _imageService.GetCategoryImageUrl(subCategory.StoredFileName);


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

            query = query.Include(subCategory => subCategory.Category);

            var count = await query.CountAsync();

            var subCategories = await query/*.Select(sub => _mapper.Map<GetAllSubCategoriesDto>(sub))*/
                                           .Skip((subCategoriesFilter.PageNumber - 1) * subCategoriesFilter.PageSize)
                                           .Take(subCategoriesFilter.PageSize)
                                           .ToListAsync();

            var subCategoriesDto = new List<GetAllSubCategoriesDto>();
            foreach (var subCategory in subCategories)
            {
                var subCategoryDto = _mapper.Map<GetAllSubCategoriesDto>(subCategory);

                if(subCategory.StoredFileName != null)
                     subCategoryDto.ImageUrl = _imageService.GetCategoryImageUrl(subCategory.StoredFileName);
                subCategoriesDto.Add(subCategoryDto);
            }

            var paginationResponse = new PaginationResponse<GetAllSubCategoriesDto>
            {
                Length = count,
                Collection = subCategoriesDto,
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
                .Select(subcategory => _mapper.Map<DropDownSubCategoriesDto>(subcategory))
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
    public async Task<ServiceResponse<string>> UploadSubCategoryImage(Guid subCategoryId, IFormFile image)
    {

        try
        {
            if (subCategoryId == Guid.Empty)
                return new ServiceResponse<string>() { Success = false, Message = "Please Enter the id" };

            if (image == null)
                return new ServiceResponse<string>() { Success = false, Message = " Please attach an image" };


            SubCategory? subCategory = _subCategoryRepo.FindByID(subCategoryId);

            if (subCategory is null)
                return new ServiceResponse<string>() { Success = false, Message = "SubCategory not found" };

           
            subCategory.StoredFileName = await _imageService.UploadImage(nameof(SubCategory), image);

            _subCategoryRepo.Update(subCategory);
            await _unitOfWork.CommitAsync();
            return new ServiceResponse<string> { Success = true, Message = "Image Uploaded Successfuly" , Data = subCategory.StoredFileName };

        }
        catch
        {
            return new ServiceResponse<string> { Success = false, Message = "An error occurred while uploading the image" };
        }
    }
}
