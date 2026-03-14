using Microsoft.AspNetCore.Mvc;
using ComplaintManagementSystem.API.Services;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ComplaintManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get the current user's ID and role from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub") ?? User.FindFirst("UserId");
            var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role") ?? User.FindFirst("RoleId");
            
            int? userId = null;
            
            // If the user is not an admin (RoleId != 1) or Engineer (RoleId != 3), filter by their userId
            if (roleClaim?.Value != "1" && roleClaim?.Value?.ToLower() != "admin" &&
                roleClaim?.Value != "3" && roleClaim?.Value?.ToLower() != "engineer")
            {
                if (int.TryParse(userIdClaim?.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }
            }
            
            var data = await _dashboardService.GetDashboardAsync(userId);
            return Ok(data);
        }
    }
}
