using blogpost_website_api.DTO;
using blogpost_website_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace blogpost_website_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] PostDTO postDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _postService.CreatePost(postDto, token);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await _postService.AllPost();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPosts(string userId)
        {
            var result = await _postService.GetUserPosts(userId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetPostsByCategory(string categoryId)
        {
            var result = await _postService.GetPostsByCategory(categoryId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("update/{postId}")]
        public async Task<IActionResult> UpdatePost(string postId, [FromBody] PostDTO postDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _postService.UpdatePost(postId, postDto, token);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{postId}")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _postService.DeletePost(postId, token);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("like/{postId}")]
        public async Task<IActionResult> AddLike(string postId)
        {
            var result = await _postService.AddLike(postId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("view/{postId}")]
        public async Task<IActionResult> AddView(string postId)
        {
            var result = await _postService.AddView(postId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
