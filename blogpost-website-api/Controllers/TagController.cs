using blogpost_website_api.DTO;
using blogpost_website_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace blogpost_website_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        // POST: api/tag
        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagDTO tagDTO)
        {
            var result = await _tagService.CreateTag(tagDTO);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET: api/tag
        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            var result = await _tagService.GetAllTags();
            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET: api/tag/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(string id)
        {
            var result = await _tagService.GetTagById(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET: api/tag/post/{postId}
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetTagsForPost(string postId)
        {
            var result = await _tagService.GetTagsForPost(postId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // PUT: api/tag/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(string id, [FromBody] TagDTO tagDTO)
        {
            var result = await _tagService.UpdateTag(id, tagDTO);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // DELETE: api/tag/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(string id)
        {
            var result = await _tagService.DeleteTag(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET: api/tag/search?text=somequery
        [HttpGet("search")]
        public async Task<IActionResult> SearchTags([FromQuery] string text)
        {
            var result = await _tagService.SearchTags(text);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
