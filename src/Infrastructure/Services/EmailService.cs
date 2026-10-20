using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuanLyBanDoGomSu.Application.Port.In;

namespace QuanLyBanDoGomSu.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            var server = _configuration["SmtpSettings:Server"] ?? "smtp.gmail.com";
            var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
            var senderEmail = _configuration["SmtpSettings:SenderEmail"] ?? "loog5277@gmail.com";
            var senderName = _configuration["SmtpSettings:SenderName"] ?? "Gốm Sứ Bát Tràng Nhóm 5";
            var username = _configuration["SmtpSettings:Username"] ?? "loog5277@gmail.com";
            var password = _configuration["SmtpSettings:Password"] ?? "vqabvcgsgomlvxom";
            var enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"] ?? "true");

            using var message = new MailMessage();
            message.From = new MailAddress(senderEmail, senderName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = bodyHtml;
            message.IsBodyHtml = true;

            using var smtp = new SmtpClient(server, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            try
            {
                await smtp.SendMailAsync(message);
                _logger.LogInformation("Gửi email thành công tới {ToEmail} với tiêu đề: {Subject}", toEmail, subject);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email SMTP tới {ToEmail}", toEmail);
                throw;
            }
        }

        public async Task SendVerificationOtpAsync(string toEmail, string fullName, string otpCode)
        {
            string subject = "Xác nhận Đăng ký Tài khoản - Gốm Sứ Bát Tràng";
            string bodyHtml = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #8B5CF6; padding: 20px; text-align: center; color: white;'>
                        <h2 style='margin: 0;'>🎉 Chào mừng đến với Gốm Sứ Bát Tràng</h2>
                    </div>
                    <div style='padding: 24px; color: #333333;'>
                        <p>Xin chào <strong>{WebUtility.HtmlEncode(fullName)}</strong>,</p>
                        <p>Cảm ơn bạn đã đăng ký tài khoản tại Cửa Hàng Gốm Sứ Bát Tràng (Nhóm 5).</p>
                        <p>Mã xác nhận OTP kích hoạt tài khoản của bạn là:</p>
                        <div style='text-align: center; margin: 24px 0;'>
                            <span style='font-size: 32px; font-weight: bold; letter-spacing: 6px; color: #8B5CF6; background-color: #F3E8FF; padding: 12px 24px; border-radius: 8px; border: 2px dashed #8B5CF6;'>{otpCode}</span>
                        </div>
                        <p style='color: #666666; font-size: 14px;'>Mã xác nhận này có hiệu lực trong vòng <strong>15 phút</strong>. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
                        <hr style='border: none; border-top: 1px solid #eeeeee; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #888888; text-align: center;'>Trân trọng,<br/>Đội ngũ Hỗ trợ Gốm Sứ Bát Tràng Nhóm 5</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, subject, bodyHtml);
        }

        public async Task SendForgotPasswordOtpAsync(string toEmail, string username, string otpCode)
        {
            string subject = "Yêu cầu Đặt lại Mật khẩu - Gốm Sứ Bát Tràng";
            string bodyHtml = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #EF4444; padding: 20px; text-align: center; color: white;'>
                        <h2 style='margin: 0;'>🔒 Đặt lại Mật khẩu Tài khoản</h2>
                    </div>
                    <div style='padding: 24px; color: #333333;'>
                        <p>Xin chào <strong>{WebUtility.HtmlEncode(username)}</strong>,</p>
                        <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản đăng ký với email <strong>{WebUtility.HtmlEncode(toEmail)}</strong>.</p>
                        <p>Mã xác nhận OTP đặt lại mật khẩu của bạn là:</p>
                        <div style='text-align: center; margin: 24px 0;'>
                            <span style='font-size: 32px; font-weight: bold; letter-spacing: 6px; color: #EF4444; background-color: #FEE2E2; padding: 12px 24px; border-radius: 8px; border: 2px dashed #EF4444;'>{otpCode}</span>
                        </div>
                        <p style='color: #666666; font-size: 14px;'>Mã OTP này có hiệu lực trong vòng <strong>15 phút</strong>. Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                        <hr style='border: none; border-top: 1px solid #eeeeee; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #888888; text-align: center;'>Trân trọng,<br/>Đội ngũ Hỗ trợ Gốm Sứ Bát Tràng Nhóm 5</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, subject, bodyHtml);
        }
    }
}
