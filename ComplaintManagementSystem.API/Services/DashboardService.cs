using ComplaintManagementSystem.API.Repositories;
using ComplaintManagementSystem.Shared.DTOs;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IComplaintRepository _complaintRepository;

        public DashboardService(IComplaintRepository complaintRepository)
        {
            _complaintRepository = complaintRepository;
        }

        public Task<DashboardResponse> GetDashboardAsync(int? userId = null)
        {
            return _complaintRepository.GetDashboardDataAsync(userId);
        }
    }
}
