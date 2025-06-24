using blogpost_website_api.DTO;
using blogpost_website_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace blogpost_website_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        // POST: api/comment/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateComment([FromBody] CommentDTO commentDto)
        {
            var result = await _commentService.CreateComment(commentDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET: api/comment/post/{postId}
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            var result = await _commentService.GetCommentsByPostId(postId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET: api/comment/{commentId}
        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetCommentById(string commentId)
        {
            var result = await _commentService.GetCommentById(commentId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // DELETE: api/comment/{commentId}
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            var result = await _commentService.DeleteComment(commentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
