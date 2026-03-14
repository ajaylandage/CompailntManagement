using ComplaintManagementSystem.Shared.Entities;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<int> CreateAsync(User user);
    }
}