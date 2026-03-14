using ComplaintManagementSystem.Shared.DTOs;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Services
{
    public interface IDashboardService
    {
        Task<DashboardResponse> GetDashboardAsync(int? userId = null);
    }
}
