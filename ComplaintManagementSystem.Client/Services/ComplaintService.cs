using System.Net.Http;
using System.Net.Http.Json;
using ComplaintManagementSystem.Shared.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ComplaintManagementSystem.Client.Services
{
    public class ComplaintService
    {
        private readonly HttpClient _http;

        public ComplaintService(HttpClient http)
        {
            _http = http;
        }

        public async Task CreateComplaintAsync(CreateComplaintRequest request)
        {
            await _http.PostAsJsonAsync("api/complaints", request);
        }

        public async Task<List<ComplaintListResponse>> GetComplaintsAsync(int? statusId = null, int? categoryId = null, string? priority = null, string? search = null)
        {
            var query = new List<string>();
            if (statusId.HasValue) query.Add($"statusId={statusId.Value}");
            if (categoryId.HasValue) query.Add($"categoryId={categoryId.Value}");
            if (!string.IsNullOrWhiteSpace(priority)) query.Add($"priority={Uri.EscapeDataString(priority)}");
            if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");

            var url = "api/complaints" + (query.Count > 0 ? "?" + string.Join("&", query) : string.Empty);

            var result = await _http.GetFromJsonAsync<List<ComplaintListResponse>>(url);
            return result ?? new List<ComplaintListResponse>();
        }

        public async Task<ComplaintDetailsResponse?> GetComplaintByIdAsync(int complaintId)
        {
            return await _http.GetFromJsonAsync<ComplaintDetailsResponse>($"api/complaints/{complaintId}");
        }

        public async Task UpdateComplaintStatusAsync(int complaintId, UpdateComplaintStatusRequest request)
        {
            await _http.PutAsJsonAsync($"api/complaints/{complaintId}/status", request);
        }

        public async Task<bool> AssignComplaintAsync(int complaintId, AssignComplaintRequest request)
        {
            var response = await _http.PostAsJsonAsync($"api/complaints/{complaintId}/assign", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<DashboardResponse> GetDashboardAsync()
        {
            return await _http.GetFromJsonAsync<DashboardResponse>("api/dashboard");
        }
    }
}
