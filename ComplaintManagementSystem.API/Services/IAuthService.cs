using ComplaintManagementSystem.Shared.DTOs;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Services
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}