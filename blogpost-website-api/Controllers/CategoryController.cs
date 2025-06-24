using blogpost_website_api.DTO;
using blogpost_website_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace blogpost_website_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            var result = await _categoryService.CreateCategory(categoryDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategories();
            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var result = await _categoryService.GetCategoryById(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryDTO categoryDto)
        {
            var result = await _categoryService.UpdateCategory(id, categoryDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var result = await _categoryService.DeleteCategory(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET: api/category/search?text=...
        [HttpGet("search")]
        public async Task<IActionResult> SearchCategories([FromQuery] string text)
        {
            var result = await _categoryService.SearchCategories(text);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
