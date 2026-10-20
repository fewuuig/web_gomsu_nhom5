/* ================================================================
   FLYWAY MIGRATION: V3__Add_Avatar_And_UserProfile.sql
   MÔ TẢ: Bổ sung trường Avatar cho bảng TaiKhoan và tối ưu thông tin người dùng
   HỆ CSDL: SQL Server 2022 (WebQuanLyBanDoGomSu)
   ================================================================ */

-- 1. Bổ sung cột Avatar cho bảng TaiKhoan nếu chưa tồn tại
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.TaiKhoan') AND name = 'Avatar'
)
BEGIN
    ALTER TABLE dbo.TaiKhoan ADD Avatar NVARCHAR(MAX) NULL;
    PRINT N'Đã thêm cột Avatar vào bảng TaiKhoan thành công.';
END
ELSE
BEGIN
    PRINT N'Cột Avatar đã tồn tại trong bảng TaiKhoan.';
END
GO

-- 2. Cập nhật Avatar mặc định cho tài khoản quản trị và khách hàng mẫu
UPDATE dbo.TaiKhoan
SET Avatar = 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&auto=format&fit=crop&q=80'
WHERE TenDangNhap = 'admin' AND (Avatar IS NULL OR Avatar = '');

UPDATE dbo.TaiKhoan
SET Avatar = 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150&auto=format&fit=crop&q=80'
WHERE TenDangNhap = 'user' AND (Avatar IS NULL OR Avatar = '');
GO

PRINT N'Hoàn tất thực thi Migration V3__Add_Avatar_And_UserProfile.sql';
GO
