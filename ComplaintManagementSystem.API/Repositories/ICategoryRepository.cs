using ComplaintManagementSystem.Shared.Entities;
using ComplaintManagementSystem.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse?> GetByIdAsync(int categoryId);
        Task<int> CreateAsync(CategoryRequest request);
        Task<bool> UpdateAsync(int categoryId, CategoryRequest request);
        Task<bool> DeleteAsync(int categoryId);
    }
}