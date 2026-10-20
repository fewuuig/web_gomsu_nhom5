/* ================================================================
   FLYWAY MIGRATION: V4__Add_DiaChiNhanHang_And_OrderDeliveryInfo.sql
   MÔ TẢ: Bổ sung sổ địa chỉ nhận hàng (1:N) & Lưu vết địa chỉ giao hàng cho Hóa đơn bán
   HỆ CSDL: SQL Server 2022 (WebQuanLyBanDoGomSu)
   ================================================================ */

-- 1. Bổ sung các cột thông tin giao hàng cho bảng HoaDonBan nếu chưa có
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.HoaDonBan') AND name = 'DiaChiGiaoHang'
)
BEGIN
    ALTER TABLE dbo.HoaDonBan ADD DiaChiGiaoHang NVARCHAR(255) NULL;
    PRINT N'Đã thêm cột DiaChiGiaoHang vào bảng HoaDonBan.';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.HoaDonBan') AND name = 'SoDienThoaiNhan'
)
BEGIN
    ALTER TABLE dbo.HoaDonBan ADD SoDienThoaiNhan VARCHAR(20) NULL;
    PRINT N'Đã thêm cột SoDienThoaiNhan vào bảng HoaDonBan.';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.HoaDonBan') AND name = 'TenNguoiNhan'
)
BEGIN
    ALTER TABLE dbo.HoaDonBan ADD TenNguoiNhan NVARCHAR(100) NULL;
    PRINT N'Đã thêm cột TenNguoiNhan vào bảng HoaDonBan.';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.HoaDonBan') AND name = 'GhiChu'
)
BEGIN
    ALTER TABLE dbo.HoaDonBan ADD GhiChu NVARCHAR(500) NULL;
    PRINT N'Đã thêm cột GhiChu vào bảng HoaDonBan.';
END
GO

-- 2. Backfill dữ liệu giao hàng cho các đơn hàng cũ chưa có thông tin
UPDATE h
SET h.TenNguoiNhan = ISNULL(h.TenNguoiNhan, k.TenKhach),
    h.SoDienThoaiNhan = ISNULL(h.SoDienThoaiNhan, k.DienThoai),
    h.DiaChiGiaoHang = ISNULL(h.DiaChiGiaoHang, k.DiaChi)
FROM dbo.HoaDonBan h
INNER JOIN dbo.KhachHang k ON h.MaKhach = k.MaKhach
WHERE h.TenNguoiNhan IS NULL OR h.DiaChiGiaoHang IS NULL;
GO

-- 3. Tạo bảng DiaChiNhanHang (Sổ địa chỉ người dùng: 1 Khách hàng có nhiều Địa chỉ nhận hàng)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DiaChiNhanHang')
BEGIN
    CREATE TABLE dbo.DiaChiNhanHang (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MaKhach VARCHAR(10) NOT NULL,
        TenNguoiNhan NVARCHAR(100) NOT NULL,
        SoDienThoai VARCHAR(20) NOT NULL,
        DiaChiChiTiet NVARCHAR(255) NOT NULL,
        LaMacDinh BIT NOT NULL DEFAULT 0,
        NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_DiaChiNhanHang_KhachHang FOREIGN KEY (MaKhach) REFERENCES dbo.KhachHang(MaKhach) ON DELETE CASCADE
    );

    CREATE INDEX IX_DiaChiNhanHang_MaKhach ON dbo.DiaChiNhanHang(MaKhach);
    PRINT N'Đã tạo bảng DiaChiNhanHang thành công.';
END
ELSE
BEGIN
    PRINT N'Bảng DiaChiNhanHang đã tồn tại.';
END
GO

-- 4. Tạo địa chỉ mẫu cho tài khoản khách hàng mặc định (KHUSER - Nguyễn Văn An)
IF EXISTS (SELECT 1 FROM dbo.KhachHang WHERE MaKhach = 'KHUSER')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.DiaChiNhanHang WHERE MaKhach = 'KHUSER')
    BEGIN
        INSERT INTO dbo.DiaChiNhanHang (MaKhach, TenNguoiNhan, SoDienThoai, DiaChiChiTiet, LaMacDinh)
        VALUES 
        ('KHUSER', N'Nguyễn Văn An', '0912345678', N'Số 15, Ngõ Gốm, Làng Cổ Bát Tràng, Gia Lâm, Hà Nội', 1),
        ('KHUSER', N'Nguyễn Văn An (Văn phòng)', '0987654321', N'Tầng 8, Tòa Keangnam Landmark 72, Mễ Trì, Nam Từ Liêm, Hà Nội', 0);
        PRINT N'Đã tạo 2 địa chỉ mẫu cho khách hàng KHUSER.';
    END
END
GO

-- 5. Cập nhật hình ảnh sản phẩm gốm sứ bị thiếu (bao gồm HH9999)
UPDATE dbo.DanhMucHangHoa
SET HinhAnh = 'https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?auto=format&fit=crop&w=800&q=80'
WHERE MaHangHoa = 'HH9999' OR HinhAnh IS NULL OR HinhAnh = '';
GO

PRINT N'Hoàn tất thực thi Migration V4__Add_DiaChiNhanHang_And_OrderDeliveryInfo.sql';
GO
