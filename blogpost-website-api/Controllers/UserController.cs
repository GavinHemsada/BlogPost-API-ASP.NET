using blogpost_website_api.DTO;
using blogpost_website_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace blogpost_website_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _userService.Register(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _userService.Login(dto);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("profile/create")]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileDTO dto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _userService.CreateUserProfile(dto, token);
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
        public async Task<IActionResult> EditUserProfile([FromBody] UserProfileDTO dto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _userService.EditUserProfileDetails(token, dto);
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
