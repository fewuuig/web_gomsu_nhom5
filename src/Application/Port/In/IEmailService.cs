using System.Threading.Tasks;

namespace QuanLyBanDoGomSu.Application.Port.In
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string bodyHtml);
        Task SendVerificationOtpAsync(string toEmail, string fullName, string otpCode);
        Task SendForgotPasswordOtpAsync(string toEmail, string username, string otpCode);
    }
}
