using blogpost_website_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace blogpost_website_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OTPController : ControllerBase
    {
        private readonly OTPService _otpService;

        public OTPController(OTPService otpService)
        {
            _otpService = otpService;
        }

        // POST: api/otp/send
        [HttpPost("send")]
        public async Task<IActionResult> SendOTP([FromQuery] string email)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _otpService.SendOTP(token, email);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // POST: api/otp/verify
        [HttpPost("verify")]
        public IActionResult VerifyOTP([FromQuery] string otp)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = _otpService.CheckOTP(otp, token);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET: api/otp/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOTPs()
        {
            var result = await _otpService.AllOTP();
            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET: api/otp/user/{userid}
        [HttpGet("user/{userid}")]
        public async Task<IActionResult> GetUserOTP(string userid)
        {
            var result = await _otpService.ReadUserOTP(userid);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
