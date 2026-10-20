using System;

namespace QuanLyBanDoGomSu.Application.DTOs
{
    public class RegisterRequest
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
        public string TenKhach { get; set; }
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }

    public class LoginRequest
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string MaTK { get; set; }
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string MaQuyen { get; set; }
        public string TenHienThi { get; set; }
        public string? MaKhach { get; set; }
        public string? MaNV { get; set; }
        public string? Avatar { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string MatKhauMoi { get; set; }
    }

    public class UserProfileDto
    {
        public string MaTK { get; set; }
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string MaQuyen { get; set; }
        public string TenQuyen { get; set; }
        public string TenHienThi { get; set; }
        public string? Avatar { get; set; }
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? MaKhach { get; set; }
        public string? MaNV { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string TenDangNhap { get; set; }
        public string? TenHienThi { get; set; }
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? Avatar { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string TenDangNhap { get; set; }
        public string MatKhauCu { get; set; }
        public string MatKhauMoi { get; set; }
    }
}
