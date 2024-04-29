using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Services
{
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

                var categoryTempp = _categoryRepo.FindByID(addSubCategoryDto.CategoryId);
                if (categoryTempp == null)
                    return new ServiceResponse<bool>() { Success = false, Message = "Category not found" };


                if (addSubCategoryDto.Description == null)

                    addSubCategoryDto.Description = string.Empty;

                addSubCategoryDto.Description = addSubCategoryDto.Description.Trim();


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

 
        public async Task<ServiceResponse<SubCategory>> GetSubCategory(Guid id)
        {
            try
            {
                SubCategory subCategory = _subCategoryRepo.FindByID(id);
                if (subCategory == null)
                    return new ServiceResponse<SubCategory>() { Success = false, Message = "Category not found" };
                return new ServiceResponse<SubCategory>() { Data = subCategory, Success = true, Message = "Retrieved Successfully" };
            }
            catch (Exception)
            {
                return new ServiceResponse<SubCategory>() { Data = null, Success = false, Message = "An error occurred while getting the SubCategory" };
            }
        }
        public async Task<ServiceResponse<PaginationResponse<SubCategory>>> GetSubCategoriesAsync(PagingModel pagingModel)
        {
            try
            {
                var query = await _subCategoryRepo.GetAllQueryableAsync(filterPredicate: a => true);

                if (!query.Any())
                    return new ServiceResponse<PaginationResponse<SubCategory>> { Success = true, Message = "SubCategories not found" };

                var count = await query.CountAsync();

                var subCategories = await query.Skip((pagingModel.PageNumber - 1) * pagingModel.PageSize)
                                            .Take(pagingModel.PageSize)
                                            .ToListAsync();

                var paginationResponse = new PaginationResponse<SubCategory>
                {
                    Length = count,
                    Collection = subCategories
                };

                return new ServiceResponse<PaginationResponse<SubCategory>>
                {
                    Data = paginationResponse,
                    Message = "SubCategories retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaginationResponse<SubCategory>> { Data = null, Success = false, Message = "An error occurred while retrieving SubCategories: " + ex.Message };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateSubCategory(UpdateSubCategoryDto subCategoryDto, Guid id)
        {
            if (id == Guid.Empty)
            {
                return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

            }
            try
            {
                subCategoryDto.Name = subCategoryDto.Name.Trim();
                var temp = await _categoryRepo.FindAsync(c => c.Name == subCategoryDto.Name);
                if (temp is not null)
                
                    return new ServiceResponse<bool>() { Success = false, Message = $"{subCategoryDto.Name} already exisitng" };
                
                if (subCategoryDto.Description == null)
                
                    subCategoryDto.Description = string.Empty;
                
                subCategoryDto.Description = subCategoryDto.Description.Trim();

                SubCategory dbSubCategory = _subCategoryRepo.FindByID(id);

                if (dbSubCategory == null)
                {
                    return new ServiceResponse<bool>()
                    {
                        Success = false,
                        Message = "SubCategory not found"
                    };

                }
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
            {
                return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

            }
            try
            {
                SubCategory subCategory = _subCategoryRepo.FindByID(id);
                if (subCategory == null)
                {
                    return new ServiceResponse<bool> { Success = false, Message = "SubCategory not found" };
                }

                _subCategoryRepo.Remove(subCategory);
                var affectedRows = await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = affectedRows > 0, Message = "SubCategory deleted successfully" };
            }
            catch (Exception ex)
            {


                return new ServiceResponse<bool> { Success = false, Message = "An error occurred while deleting the SubCategory" };
            }
        }
    }
}
