using ComplaintManagementSystem.Shared.Entities;
using ComplaintManagementSystem.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Repositories
{
    public interface IComplaintRepository
    {
        Task<int> CreateAsync(Complaint complaint);
        Task<IEnumerable<ComplaintListResponse>> GetAllAsync(int? statusId = null, int? categoryId = null, string? priority = null, string? search = null, int? userId = null);
        Task<ComplaintDetailsResponse?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int complaintId, int statusId, int updatedByUserId, string? note);
        Task AssignAsync(ComplaintAssignment assignment);
        Task<DashboardResponse> GetDashboardDataAsync(int? userId = null);
    }
}