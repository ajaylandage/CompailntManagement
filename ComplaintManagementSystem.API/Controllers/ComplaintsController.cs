using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ComplaintManagementSystem.API.Services;
using ComplaintManagementSystem.Shared.DTOs;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ComplaintManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? statusId, [FromQuery] int? categoryId, [FromQuery] string? priority, [FromQuery] string? search)
        {
            int? userId = null;
            
            var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role") ?? User.FindFirst("RoleId");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub") ?? User.FindFirst("UserId");

            if (roleClaim?.Value != "1" && roleClaim?.Value?.ToLower() != "admin" &&
                roleClaim?.Value != "3" && roleClaim?.Value?.ToLower() != "engineer")
            {
                if (int.TryParse(userIdClaim?.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }
            }

            var list = await _complaintService.GetAllAsync(statusId, categoryId, priority, search, userId);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var complaint = await _complaintService.GetByIdAsync(id);
            if (complaint == null) return NotFound();
            return Ok(complaint);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateComplaintRequest request)
        {
            if (request == null) return BadRequest();

            // Read user id from JWT claims
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (!int.TryParse(sub, out var userId)) return Unauthorized();

            request.UserId = userId;

            var id = await _complaintService.CreateComplaintAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        // Admin-only (and Engineer): update complaint status
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin,Engineer")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateComplaintStatusRequest request)
        {
            if (request == null || request.ComplaintId != id) return BadRequest();

            // Use authenticated user as UpdatedByUserId if not provided
            if (request.UpdatedByUserId == 0)
            {
                var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
                if (int.TryParse(sub, out var userId)) request.UpdatedByUserId = userId;
            }

            await _complaintService.UpdateStatusAsync(request);
            return NoContent();
        }

        // Admin-only (and Engineer): assign complaint to a user
        [HttpPost("{id:int}/assign")]
        [Authorize(Roles = "Admin,Engineer")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignComplaintRequest request)
        {
            if (request == null || request.ComplaintId != id) return BadRequest();

            if (request.AssignedByUserId == 0)
            {
                var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
                if (int.TryParse(sub, out var userId)) request.AssignedByUserId = userId;
            }

            await _complaintService.AssignAsync(request);
            return NoContent();
        }
    }
}