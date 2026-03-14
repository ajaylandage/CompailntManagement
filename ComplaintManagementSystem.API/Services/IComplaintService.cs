using ComplaintManagementSystem.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Services
{
    public interface IComplaintService
    {
        Task<int> CreateComplaintAsync(CreateComplaintRequest request);
        Task<IEnumerable<ComplaintListResponse>> GetAllAsync(int? statusId = null, int? categoryId = null, string? priority = null, string? search = null, int? userId = null);
        Task<ComplaintDetailsResponse?> GetByIdAsync(int id);
        Task UpdateStatusAsync(UpdateComplaintStatusRequest request);
        Task AssignAsync(AssignComplaintRequest request);
    }
}