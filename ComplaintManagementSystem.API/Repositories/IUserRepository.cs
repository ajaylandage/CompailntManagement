using ComplaintManagementSystem.Shared.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ComplaintManagementSystem.API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<int> CreateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
    }
}