using ComplaintManagementSystem.API.Repositories;
using ComplaintManagementSystem.Shared.DTOs;
using ComplaintManagementSystem.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ComplaintManagementSystem.API.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _complaintRepository;

        public ComplaintService(IComplaintRepository complaintRepository)
        {
            _complaintRepository = complaintRepository;
        }

        public async Task<int> CreateComplaintAsync(CreateComplaintRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var complaint = new Complaint
            {
                UserId = request.UserId,
                CategoryId = request?.CategoryId ?? 0,
                StatusId = (int)Shared.Enums.ComplaintStatusType.Pending,
                Title = request.Title,
                Description = request.Description,
                LocationAddress = request.LocationAddress,
                Priority = request.Priority,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CreatedAt = DateTime.UtcNow
            };

            var id = await _complaintRepository.CreateAsync(complaint);

            return id;
        }

        public Task<IEnumerable<ComplaintListResponse>> GetAllAsync(int? statusId = null, int? categoryId = null, string? priority = null, string? search = null, int? userId = null)
        {
            return _complaintRepository.GetAllAsync(statusId, categoryId, priority, search, userId);
        }

        public async Task<ComplaintDetailsResponse?> GetByIdAsync(int id)
        {
            var complaint = await _complaintRepository.GetByIdAsync(id);
            return complaint;
        }

        public async Task UpdateStatusAsync(UpdateComplaintStatusRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var existing = await _complaintRepository.GetByIdAsync(request.ComplaintId);
            if (existing == null) throw new InvalidOperationException("Complaint not found.");

            await _complaintRepository.UpdateStatusAsync(request.ComplaintId, request.StatusId, request.UpdatedByUserId, request.Note);
        }

        public async Task AssignAsync(AssignComplaintRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var existing = await _complaintRepository.GetByIdAsync(request.ComplaintId);
            if (existing == null) throw new InvalidOperationException("Complaint not found.");

            var assignment = new ComplaintAssignment
            {
                ComplaintId = request.ComplaintId,
                AssignedToUserId = request.AssignedToUserId,
                AssignedByUserId = request.AssignedByUserId,
                AssignedAt = DateTime.UtcNow
            };

            await _complaintRepository.AssignAsync(assignment);
        }
    }
}