using ComplaintManagementSystem.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse?> GetByIdAsync(int id);
        Task<CategoryResponse?> CreateAsync(CategoryRequest request);
        Task<bool> UpdateAsync(int id, CategoryRequest request);
        Task<bool> DeleteAsync(int id);
    }
}