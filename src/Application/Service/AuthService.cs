using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Application.Port.Out;
using QuanLyBanDoGomSu.Domain.Entities;
using QuanLyBanDoGomSu.Infrastructure.Cache;
using QuanLyBanDoGomSu.Infrastructure.Persistence;

namespace QuanLyBanDoGomSu.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly QuanLyBanDoGomSuDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly IEmailService _emailService;

        public AuthService(QuanLyBanDoGomSuDbContext context, ICacheService cacheService, IEmailService emailService)
        {
            _context = context;
            _cacheService = cacheService;
            _emailService = emailService;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TenDangNhap) || string.IsNullOrWhiteSpace(request.MatKhau) || string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Tên đăng nhập, mật khẩu và email không được để trống.");
            }

            // Kiểm tra tên đăng nhập hoặc email đã tồn tại chưa
            bool existsUser = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == request.TenDangNhap);
            if (existsUser)
            {
                throw new InvalidOperationException("Tên đăng nhập đã được sử dụng.");
            }

            bool existsEmail = await _context.TaiKhoans.AnyAsync(t => t.Email == request.Email);
            if (existsEmail)
            {
                throw new InvalidOperationException("Email này đã được đăng ký tài khoản.");
            }

            // Tạo mã OTP 6 chữ số
            string otpCode = Random.Shared.Next(100000, 999999).ToString();

            // Lưu thông tin đăng ký tạm thời & OTP vào Cache/Redis với TTL 15 phút (900 giây)
            // Khóa tiền tố Redis: GomSuNhom5:
            string cacheKeyOtp = $"VerifyOtp:{request.Email.ToLower().Trim()}";
            string cacheKeyInfo = $"PendingRegister:{request.Email.ToLower().Trim()}";

            await _cacheService.SetAsync(cacheKeyOtp, otpCode, TimeSpan.FromMinutes(15));
            await _cacheService.SetAsync(cacheKeyInfo, request, TimeSpan.FromMinutes(15));

            // Gửi Email SMTP
            await _emailService.SendVerificationOtpAsync(request.Email, request.TenKhach ?? request.TenDangNhap, otpCode);

            return true;
        }

        public async Task<AuthResponseDto> VerifyRegisterOtpAsync(VerifyOtpRequest request)
        {
            string emailKey = request.Email.ToLower().Trim();
            string cacheKeyOtp = $"VerifyOtp:{emailKey}";
            string cacheKeyInfo = $"PendingRegister:{emailKey}";

            string? cachedOtp = await _cacheService.GetAsync<string>(cacheKeyOtp);
            if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != request.OtpCode.Trim())
            {
                throw new InvalidOperationException("Mã OTP không chính xác hoặc đã hết hạn.");
            }

            var pendingReg = await _cacheService.GetAsync<RegisterRequest>(cacheKeyInfo);
            if (pendingReg == null)
            {
                throw new InvalidOperationException("Thông tin đăng ký đã hết hạn. Vui lòng đăng ký lại.");
            }

            // Đảm bảo có Quyền "KH" (Khách hàng)
            var quyenKhach = await _context.Quyens.FirstOrDefaultAsync(q => q.MaQuyen == "KH" || q.MaQuyen == "KHACH");
            if (quyenKhach == null)
            {
                quyenKhach = new Quyen { MaQuyen = "KH", TenQuyen = "Khách Hàng" };
                _context.Quyens.Add(quyenKhach);
                await _context.SaveChangesAsync();
            }

            // Tạo Khách Hàng mới
            string maKhach = "KH" + Random.Shared.Next(100000, 999999).ToString();
            var khachHang = new KhachHang
            {
                MaKhach = maKhach,
                TenKhach = string.IsNullOrWhiteSpace(pendingReg.TenKhach) ? pendingReg.TenDangNhap : pendingReg.TenKhach,
                DienThoai = pendingReg.DienThoai,
                DiaChi = pendingReg.DiaChi
            };
            _context.KhachHangs.Add(khachHang);

            // Hash Mật Khẩu (SHA256)
            byte[] passwordHash = HashPassword(pendingReg.MatKhau);

            // Tạo Tài Khoản mới
            string maTK = "TK" + Random.Shared.Next(100000, 999999).ToString();
            var taiKhoan = new TaiKhoan
            {
                MaTK = maTK,
                TenDangNhap = pendingReg.TenDangNhap,
                MatKhau = passwordHash,
                Email = pendingReg.Email,
                MaKhach = maKhach,
                MaQuyen = quyenKhach.MaQuyen,
                TrangThai = true,
                LanDangNhapCuoi = DateTime.Now
            };
            _context.TaiKhoans.Add(taiKhoan);

            await _context.SaveChangesAsync();

            // Xóa cache OTP sau khi xác thực thành công
            await _cacheService.RemoveAsync(cacheKeyOtp);
            await _cacheService.RemoveAsync(cacheKeyInfo);

            return new AuthResponseDto
            {
                Token = $"mock-jwt-token-{Guid.NewGuid():N}",
                MaTK = taiKhoan.MaTK,
                TenDangNhap = taiKhoan.TenDangNhap,
                Email = taiKhoan.Email,
                MaQuyen = taiKhoan.MaQuyen,
                TenHienThi = khachHang.TenKhach,
                MaKhach = khachHang.MaKhach,
                Avatar = taiKhoan.Avatar
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TenDangNhap) || string.IsNullOrWhiteSpace(request.MatKhau))
            {
                throw new ArgumentException("Tên đăng nhập và mật khẩu không được để trống.");
            }

            string searchKey = request.TenDangNhap.Trim();
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.KhachHang)
                .Include(t => t.NhanVien)
                .Include(t => t.Quyen)
                .FirstOrDefaultAsync(t => t.TenDangNhap.Trim() == searchKey || t.Email.Trim() == searchKey);

            if (taiKhoan == null)
            {
                throw new InvalidOperationException("Tài khoản không tồn tại trong hệ thống.");
            }

            if (!taiKhoan.TrangThai)
            {
                throw new InvalidOperationException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
            }

            // Verify Password
            byte[] inputHash = HashPassword(request.MatKhau.Trim());
            if (!SlowEquals(taiKhoan.MatKhau, inputHash))
            {
                throw new InvalidOperationException("Mật khẩu không chính xác.");
            }

            // Cập nhật lần đăng nhập cuối
            taiKhoan.LanDangNhapCuoi = DateTime.Now;
            await _context.SaveChangesAsync();

            string tenHienThi = taiKhoan.KhachHang?.TenKhach ?? taiKhoan.NhanVien?.TenNV ?? taiKhoan.TenDangNhap;

            return new AuthResponseDto
            {
                Token = $"mock-jwt-token-{Guid.NewGuid():N}",
                MaTK = taiKhoan.MaTK,
                TenDangNhap = taiKhoan.TenDangNhap,
                Email = taiKhoan.Email,
                MaQuyen = taiKhoan.MaQuyen,
                TenHienThi = tenHienThi,
                MaKhach = taiKhoan.MaKhach,
                MaNV = taiKhoan.MaNV,
                Avatar = taiKhoan.Avatar
            };
        }

        public async Task<bool> RequestForgotPasswordOtpAsync(ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Email không được để trống.");
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == request.Email);
            if (taiKhoan == null)
            {
                throw new InvalidOperationException("Không tìm thấy tài khoản liên kết với email này.");
            }

            // Tạo mã OTP 6 chữ số
            string otpCode = Random.Shared.Next(100000, 999999).ToString();

            // Lưu OTP Quên Mật Khẩu vào Cache/Redis 15 phút
            string cacheKeyResetOtp = $"ResetOtp:{request.Email.ToLower().Trim()}";
            await _cacheService.SetAsync(cacheKeyResetOtp, otpCode, TimeSpan.FromMinutes(15));

            // Gửi Email SMTP
            await _emailService.SendForgotPasswordOtpAsync(request.Email, taiKhoan.TenDangNhap, otpCode);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.OtpCode) || string.IsNullOrWhiteSpace(request.MatKhauMoi))
            {
                throw new ArgumentException("Vui lòng điền đầy đủ email, mã OTP và mật khẩu mới.");
            }

            string emailKey = request.Email.ToLower().Trim();
            string cacheKeyResetOtp = $"ResetOtp:{emailKey}";

            string? cachedOtp = await _cacheService.GetAsync<string>(cacheKeyResetOtp);
            if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != request.OtpCode.Trim())
            {
                throw new InvalidOperationException("Mã OTP không chính xác hoặc đã hết hạn.");
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == request.Email);
            if (taiKhoan == null)
            {
                throw new InvalidOperationException("Không tìm thấy tài khoản.");
            }

            // Cập nhật mật khẩu mới
            taiKhoan.MatKhau = HashPassword(request.MatKhauMoi);
            await _context.SaveChangesAsync();

            // Xóa cache OTP
            await _cacheService.RemoveAsync(cacheKeyResetOtp);

            return true;
        }

        public async Task<UserProfileDto> GetProfileAsync(string tenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                throw new ArgumentException("Tên đăng nhập không được để trống.");
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.KhachHang)
                .Include(t => t.NhanVien)
                .Include(t => t.Quyen)
                .FirstOrDefaultAsync(t => t.TenDangNhap.Trim() == tenDangNhap.Trim());

            if (taiKhoan == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin tài khoản.");
            }

            string tenHienThi = taiKhoan.KhachHang?.TenKhach ?? taiKhoan.NhanVien?.TenNV ?? taiKhoan.TenDangNhap;
            string? dienThoai = taiKhoan.KhachHang?.DienThoai ?? taiKhoan.NhanVien?.DienThoai;
            string? diaChi = taiKhoan.KhachHang?.DiaChi ?? taiKhoan.NhanVien?.DiaChi;
            string? gioiTinh = taiKhoan.NhanVien?.GioiTinh;
            DateTime? ngaySinh = taiKhoan.NhanVien?.NgaySinh;

            return new UserProfileDto
            {
                MaTK = taiKhoan.MaTK,
                TenDangNhap = taiKhoan.TenDangNhap,
                Email = taiKhoan.Email,
                MaQuyen = taiKhoan.MaQuyen,
                TenQuyen = taiKhoan.Quyen?.TenQuyen ?? taiKhoan.MaQuyen,
                TenHienThi = tenHienThi,
                Avatar = taiKhoan.Avatar,
                DienThoai = dienThoai,
                DiaChi = diaChi,
                GioiTinh = gioiTinh,
                NgaySinh = ngaySinh,
                MaKhach = taiKhoan.MaKhach,
                MaNV = taiKhoan.MaNV,
                LanDangNhapCuoi = taiKhoan.LanDangNhapCuoi
            };
        }

        public async Task<UserProfileDto> UpdateProfileAsync(UpdateProfileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TenDangNhap))
            {
                throw new ArgumentException("Tên đăng nhập không được để trống.");
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.KhachHang)
                .Include(t => t.NhanVien)
                .Include(t => t.Quyen)
                .FirstOrDefaultAsync(t => t.TenDangNhap.Trim() == request.TenDangNhap.Trim());

            if (taiKhoan == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin tài khoản.");
            }

            // Cập nhật Avatar nếu được cung cấp
            if (request.Avatar != null)
            {
                taiKhoan.Avatar = request.Avatar;
            }

            // Cập nhật thông tin KhachHang hoặc NhanVien
            if (taiKhoan.KhachHang != null)
            {
                if (!string.IsNullOrWhiteSpace(request.TenHienThi))
                    taiKhoan.KhachHang.TenKhach = request.TenHienThi.Trim();
                if (request.DienThoai != null)
                    taiKhoan.KhachHang.DienThoai = request.DienThoai.Trim();
                if (request.DiaChi != null)
                    taiKhoan.KhachHang.DiaChi = request.DiaChi.Trim();
            }
            else if (taiKhoan.NhanVien != null)
            {
                if (!string.IsNullOrWhiteSpace(request.TenHienThi))
                    taiKhoan.NhanVien.TenNV = request.TenHienThi.Trim();
                if (request.DienThoai != null)
                    taiKhoan.NhanVien.DienThoai = request.DienThoai.Trim();
                if (request.DiaChi != null)
                    taiKhoan.NhanVien.DiaChi = request.DiaChi.Trim();
                if (request.GioiTinh != null)
                    taiKhoan.NhanVien.GioiTinh = request.GioiTinh.Trim();
                if (request.NgaySinh.HasValue)
                    taiKhoan.NhanVien.NgaySinh = request.NgaySinh.Value;
            }

            await _context.SaveChangesAsync();

            return await GetProfileAsync(taiKhoan.TenDangNhap);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TenDangNhap))
            {
                throw new ArgumentException("Tên đăng nhập không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(request.MatKhauCu) || string.IsNullOrWhiteSpace(request.MatKhauMoi))
            {
                throw new ArgumentException("Mật khẩu hiện tại và mật khẩu mới không được để trống.");
            }

            if (request.MatKhauMoi.Length < 6)
            {
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap.Trim() == request.TenDangNhap.Trim());
            if (taiKhoan == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin tài khoản.");
            }

            // Kiểm tra mật khẩu cũ
            byte[] oldHash = HashPassword(request.MatKhauCu.Trim());
            if (!SlowEquals(taiKhoan.MatKhau, oldHash))
            {
                throw new InvalidOperationException("Mật khẩu hiện tại không chính xác.");
            }

            // Cập nhật mật khẩu mới
            taiKhoan.MatKhau = HashPassword(request.MatKhauMoi.Trim());
            await _context.SaveChangesAsync();

            return true;
        }

        private static byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
