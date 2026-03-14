using System.Net.Http;
using System.Net.Http.Json;
using ComplaintManagementSystem.Shared.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ComplaintManagementSystem.Client.Services
{
    public class UserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UserResponse>> GetUsersAsync()
        {
            var result = await _http.GetFromJsonAsync<List<UserResponse>>("api/users");
            return result ?? new List<UserResponse>();
        }
    }
}
