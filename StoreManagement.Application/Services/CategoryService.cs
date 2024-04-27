using AutoMapper;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Category> _categoryRepo;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper) :base()
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryRepo = unitOfWork.GetRepository<Category>();

        }

        public async Task<bool> AddCategory(AddCategoryDto addCategoryDto)
        {
           
                Category dbCategory = new Category()
                {
                    Name = addCategoryDto.Name,
                    Description = addCategoryDto.Description
                };

                await _categoryRepo.AddAsync(dbCategory);
                await _unitOfWork.CommitAsync();

                return true; 
        }


     
        public Category GetCategory(Guid id)
        {
            Category category = _categoryRepo.FindByID(id);
            return category;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = await _categoryRepo.GetAllAsync(a => true);
            return categories;
        }



        public Category UpdateCategory(Category category, Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            Category category = _categoryRepo.FindByID(id);
            try
            {
                _categoryRepo.Remove(category);
                var affectedRows = await _unitOfWork.CommitAsync();

                return affectedRows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
