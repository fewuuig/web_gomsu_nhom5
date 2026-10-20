using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.DTOs;

namespace QuanLyBanDoGomSu.Application.Port.In
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<AuthResponseDto> VerifyRegisterOtpAsync(VerifyOtpRequest request);
        Task<AuthResponseDto> LoginAsync(LoginRequest request);
        Task<bool> RequestForgotPasswordOtpAsync(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
        Task<UserProfileDto> GetProfileAsync(string tenDangNhap);
        Task<UserProfileDto> UpdateProfileAsync(UpdateProfileRequest request);
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
    }
}
