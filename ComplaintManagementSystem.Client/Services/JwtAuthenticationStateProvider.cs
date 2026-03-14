using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace ComplaintManagementSystem.Client.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public JwtAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthenticationState(_anonymous);
                }

                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void MarkUserAsLoggedOut()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        // Convenience: synchronously check if the cached principal is admin
        public static bool IsAdminPrincipal(ClaimsPrincipal user)
        {
            var roleId = user.FindFirst("RoleId")?.Value;
            var role   = user.FindFirst(ClaimTypes.Role)?.Value
                         ?? user.FindFirst("role")?.Value;
            return roleId == "1" || string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        // Convenience: synchronously check if the cached principal is admin or engineer
        public static bool IsAdminOrEngineerPrincipal(ClaimsPrincipal user)
        {
            var roleId = user.FindFirst("RoleId")?.Value;
            var role   = user.FindFirst(ClaimTypes.Role)?.Value
                         ?? user.FindFirst("role")?.Value;
            return roleId == "1" || roleId == "3" || 
                   string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) || 
                   string.Equals(role, "Engineer", StringComparison.OrdinalIgnoreCase);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3) return Array.Empty<Claim>();
            var payload = parts[1];
            // base64 padding
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }
            var bytes = Convert.FromBase64String(payload);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            var keyValues = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            var claims = new List<Claim>();
            if (keyValues == null) return claims;
            foreach (var kv in keyValues)
            {
                if (kv.Value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in element.EnumerateArray())
                        {
                            claims.Add(new Claim(kv.Key, item.ToString()));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(kv.Key, element.ToString()));
                    }
                }
                else
                {
                    claims.Add(new Claim(kv.Key, kv.Value?.ToString() ?? string.Empty));
                }
            }
            // map standard name claim if present
            // If 'given_name' exists, add Name claim
            var given = claims.FirstOrDefault(c => c.Type == "given_name" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            var family = claims.FirstOrDefault(c => c.Type == "family_name" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
            if (given != null || family != null)
            {
                var name = ((given != null ? given.Value : string.Empty) + " " + (family != null ? family.Value : string.Empty)).Trim();
                claims.Add(new Claim(ClaimTypes.Name, name));
            }

            return claims;
        }
    }
}
