using Microsoft.AspNetCore.Mvc;
using RestaurantApp.DTOs;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;

        public AuthController(IUserService userService, IAuthService authService, IOtpService otpService)
        {
            _userService = userService;
            _authService = authService;
            _otpService = otpService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.RegisterUser(dto);

                // Generate and send OTP for email verification
                var otpCode = await _otpService.GenerateOtpCode();
                await _otpService.SaveOtpCode(user.Email, otpCode, "EmailVerification");
                await _otpService.SendOtp(user.Email, otpCode);

                return Ok(new
                {
                    message = "Registration successful. Please verify your email with the OTP sent.",
                    userId = user.Id,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.ValidateUser(dto);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                var token = await _authService.GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = user.Id,
                        name = user.Name,
                        email = user.Email,
                        role = user.Role?.Name ?? "Customer",
                        isVerified = user.IsVerified
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isValid = await _otpService.VerifyOtpCode(dto.Email, dto.OtpCode);
            if (!isValid)
            {
                return BadRequest(new { message = "Invalid or expired OTP." });
            }

            return Ok(new { message = "Email verified successfully." });
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserByEmail(dto.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var otpCode = await _otpService.GenerateOtpCode();
            await _otpService.SaveOtpCode(dto.Email, otpCode, "EmailVerification");
            await _otpService.SendOtp(dto.Email, otpCode);

            return Ok(new { message = "OTP resent successfully." });
        }
    }

    // Small DTOs specific to auth endpoints
    public class OtpVerifyDto
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }

    public class ResendOtpDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
