using blogpost_website_api.DTO;
using blogpost_website_api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blogpost_website_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("profile/create")]
        public async Task<IActionResult> CreateUserProfile([FromForm] UserProfileDTO dto, IFormFile profileImage)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _userService.CreateUserProfile(dto, token,profileImage);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _userService.UserProfile(token);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditUser([FromBody] UserDTO dto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _userService.EditUserDetails(token, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("profile/edit")]
        public async Task<IActionResult> EditUserProfile([FromForm] UserProfileDTO dto, IFormFile profileImage)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _userService.EditUserProfileDetails(token, dto, profileImage);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _userService.DeleteUser(token);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
