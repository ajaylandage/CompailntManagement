using ComplaintManagementSystem.API.Repositories;
using ComplaintManagementSystem.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            return _categoryRepository.GetAllAsync();
        }

        public Task<CategoryResponse?> GetByIdAsync(int id)
        {
            return _categoryRepository.GetByIdAsync(id);
        }

        public async Task<CategoryResponse?> CreateAsync(CategoryRequest request)
        {
            var id = await _categoryRepository.CreateAsync(request);
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(int id, CategoryRequest request)
        {
            return await _categoryRepository.UpdateAsync(id, request);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }
    }
}