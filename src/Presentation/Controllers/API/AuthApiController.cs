using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;

namespace QuanLyBanDoGomSu.Presentation.Controllers.API
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthApiController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 1. Đăng ký tài khoản mới và gửi mã xác nhận OTP qua Gmail SMTP
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(new { status = 200, success = true, message = "Mã OTP xác nhận đăng ký đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư!" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = 400, success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 2. Xác thực mã OTP để kích hoạt tài khoản đăng ký
        /// </summary>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyRegisterOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                var response = await _authService.VerifyRegisterOtpAsync(request);
                return Ok(new { status = 200, success = true, data = response, message = "Xác nhận đăng ký tài khoản thành công!" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = 400, success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 3. Đăng nhập hệ thống
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(new { status = 200, success = true, data = response, message = "Đăng nhập thành công!" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = 400, success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 4. Yêu cầu mã OTP Quên Mật Khẩu qua Gmail SMTP
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var result = await _authService.RequestForgotPasswordOtpAsync(request);
                return Ok(new { status = 200, success = true, message = "Mã OTP đặt lại mật khẩu đã được gửi đến email của bạn." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = 400, success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 5. Đặt lại mật khẩu mới với mã OTP
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(request);
                return Ok(new { status = 200, success = true, message = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập bằng mật khẩu mới." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = 400, success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 6. Lấy thông tin hồ sơ người dùng
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string username)
        {
            try
            {
                var profile = await _authService.GetProfileAsync(username);
                return Ok(new { status = 200, success = true, data = profile, message = "Lấy thông tin người dùng thành công!" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = 400, success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 7. Cập nhật hồ sơ người dùng & avatar
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var profile = await _authService.UpdateProfileAsync(request);
                return Ok(new { status = 200, success = true, data = profile, message = "Cập nhật hồ sơ thành công!" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = 400, success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 8. Đổi mật khẩu tài khoản
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var result = await _authService.ChangePasswordAsync(request);
                return Ok(new { status = 200, success = true, message = "Đổi mật khẩu thành công! Vui lòng ghi nhớ mật khẩu mới." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { status = 400, success = false, message = ex.Message });
            }
        }
    }
}
