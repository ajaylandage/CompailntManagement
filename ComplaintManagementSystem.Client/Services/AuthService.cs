using System.Net.Http;
using System.Net.Http.Json;
using ComplaintManagementSystem.Shared.DTOs;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace ComplaintManagementSystem.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private string? _token;

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_token);

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/login", request);
            if (!resp.IsSuccessStatusCode)
            {
                // try to read error message from body
                try
                {
                    var err = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (err != null && err.TryGetValue("message", out var m) && !string.IsNullOrWhiteSpace(m))
                        throw new System.Exception(m);
                }
                catch { }

                resp.EnsureSuccessStatusCode(); // will throw default
            }

            var login = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            if (!string.IsNullOrEmpty(login?.Token))
            {
                _token = login.Token;
                await _js.InvokeVoidAsync("localStorage.setItem", "authToken", login.Token);
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login.Token);
            }
            return login!;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/register", request);
            if (!resp.IsSuccessStatusCode)
            {
                try
                {
                    var err = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (err != null && err.TryGetValue("message", out var m) && !string.IsNullOrWhiteSpace(m))
                        throw new System.Exception(m);
                }
                catch { }

                resp.EnsureSuccessStatusCode();
            }
        }

        public async Task InitializeAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                _token = token;
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task LogoutAsync()
        {
            _token = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            _http.DefaultRequestHeaders.Authorization = null;
        }
    }
}
