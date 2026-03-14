using System.Net.Http;
using System.Net.Http.Json;
using ComplaintManagementSystem.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Client.Services
{
    public class CategoryService
    {
        private readonly HttpClient _http;

        public CategoryService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CategoryResponse>> GetCategoriesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<CategoryResponse>>("api/categories");
            return result ?? new List<CategoryResponse>();
        }

        public async Task<CategoryResponse?> CreateCategoryAsync(CategoryRequest request)
        {
            var result = await _http.PostAsJsonAsync("api/categories", request);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<CategoryResponse>();
            }
            return null;
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryRequest request)
        {
            var result = await _http.PutAsJsonAsync($"api/categories/{id}", request);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var result = await _http.DeleteAsync($"api/categories/{id}");
            return result.IsSuccessStatusCode;
        }
    }
}
