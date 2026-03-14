using Microsoft.AspNetCore.Mvc;
using ComplaintManagementSystem.API.Services;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _categoryService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Shared.DTOs.CategoryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CategoryName)) return BadRequest();
            var result = await _categoryService.CreateAsync(request);
            if (result == null) return StatusCode(500, "Failed to create category");
            return CreatedAtAction(nameof(GetById), new { id = result.CategoryID }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Shared.DTOs.CategoryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CategoryName)) return BadRequest();
            var success = await _categoryService.UpdateAsync(id, request);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}