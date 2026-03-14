using Microsoft.AspNetCore.Mvc;
using ComplaintManagementSystem.API.Repositories;
using ComplaintManagementSystem.Shared.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ComplaintManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = (await _userRepository.GetAllAsync()).ToList();
            var result = users.Select(u => new UserResponse
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId
            }).ToList();

            return Ok(result);
        }
    }
}
