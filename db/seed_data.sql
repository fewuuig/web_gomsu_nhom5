/* ================================================================
   SCRIPT KHỞI TẠO VÀ SEED DỮ LIỆU SẢN PHẨM GỐM SỨ BÁT TRÀNG CHUẨN
   Database: WebQuanLyBanDoGomSu
   ================================================================ */

USE WebQuanLyBanDoGomSu;
GO

-- 1. Thêm cột HinhAnh (NVARCHAR(500)) nếu chưa có trong bảng DanhMucHangHoa
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'DanhMucHangHoa' AND COLUMN_NAME = 'HinhAnh'
)
BEGIN
    ALTER TABLE DanhMucHangHoa ADD HinhAnh NVARCHAR(500) NULL;
END
GO

-- 2. Seed Bảng Danh Mục Cơ Bản
IF NOT EXISTS (SELECT 1 FROM Loai)
BEGIN
    INSERT INTO Loai (MaLoai, TenLoai) VALUES 
    ('L01', N'Gốm Sứ Mỹ Nghệ'),
    ('L02', N'Bộ Trà & Ấm Trà'),
    ('L03', N'Bộ Bát Đĩa & Thờ Cúng'),
    ('L04', N'Bình Hoa Trang Trí'),
    ('L05', N'Lộc Bình Phong Thủy'),
    ('L06', N'Tượng Gốm Nghệ Thuật');
END

IF NOT EXISTS (SELECT 1 FROM LoaiMen)
BEGIN
    INSERT INTO LoaiMen (MaLoaiMen, TenLoaiMen) VALUES 
    ('LM01', N'Men Hỏa Biến'),
    ('LM02', N'Men Lam Bát Tràng'),
    ('LM03', N'Men Rạn Cổ'),
    ('LM04', N'Men Ngọc Hoàng Gia'),
    ('LM05', N'Men Trắng Tuyết');
END

IF NOT EXISTS (SELECT 1 FROM KichThuoc)
BEGIN
    INSERT INTO KichThuoc (MaKichThuoc, TenKichThuoc) VALUES 
    ('KT01', N'Nhỏ (Cao 15-20cm)'),
    ('KT02', N'Vừa (Cao 30-40cm)'),
    ('KT03', N'Lớn (Cao 50-60cm)'),
    ('KT04', N'Đại (Cao 1m2-1m6)');
END

IF NOT EXISTS (SELECT 1 FROM CongDung)
BEGIN
    INSERT INTO CongDung (MaCongDung, TenCongDung) VALUES 
    ('CD01', N'Trang Trí Phòng Khách'),
    ('CD02', N'Thưởng Trà & Tiếp Khách'),
    ('CD03', N'Thờ Cúng Gia Tiên'),
    ('CD04', N'Chiêu Tài Hút Lộc Phong Thủy');
END

IF NOT EXISTS (SELECT 1 FROM MauSac)
BEGIN
    INSERT INTO MauSac (MaMauSac, TenMauSac) VALUES 
    ('MS01', N'Xanh Lam Truyền Thống'),
    ('MS02', N'Hỏa Biến Hổ Phách'),
    ('MS03', N'Trắng Ngọc Ngà'),
    ('MS04', N'Vàng Hoàng Gia');
END

IF NOT EXISTS (SELECT 1 FROM HinhKhoi)
BEGIN
    INSERT INTO HinhKhoi (MaHinhKhoi, TenHinhKhoi) VALUES 
    ('HK01', N'Bình Tròn Hút Lộc'),
    ('HK02', N'Lộc Bình Thắt Thân'),
    ('HK03', N'Dáng Ấm Trà Tử Sa'),
    ('HK04', N'Dáng Đĩa Tròn Nghệ Thuật');
END

IF NOT EXISTS (SELECT 1 FROM HoaVan)
BEGIN
    INSERT INTO HoaVan (MaHoaVan, TenHoaVan) VALUES 
    ('HV01', N'Rồng Phượng Dát Vàng'),
    ('HV02', N'Hoa Sen Thanh Cao'),
    ('HV03', N'Sơn Thủy Hữu Tình'),
    ('HV04', N'Thuận Buồm Xuôi Gió');
END

IF NOT EXISTS (SELECT 1 FROM NuocSanXuat)
BEGIN
    INSERT INTO NuocSanXuat (MaNuocSX, TenNuocSX) VALUES 
    ('VN', N'Việt Nam (Làng Nghề Bát Tràng)');
END

IF NOT EXISTS (SELECT 1 FROM CongViec)
BEGIN
    INSERT INTO CongViec (MaCV, TenCV, MucLuong) VALUES 
    ('CV01', N'Quản Lý Bán Hàng Online', 15000000);
END

IF NOT EXISTS (SELECT 1 FROM NhanVien)
BEGIN
    INSERT INTO NhanVien (MaNV, TenNV, GioiTinh, NgaySinh, DienThoai, DiaChi, MaCV) VALUES 
    ('NV001', N'Nguyễn Bát Tràng', N'Nam', '1990-01-01', '0349348526', N'Làng gốm Bát Tràng, Gia Lâm, Hà Nội', 'CV01');
END

IF NOT EXISTS (SELECT 1 FROM KhachHang)
BEGIN
    INSERT INTO KhachHang (MaKhach, TenKhach, DiaChi, DienThoai) VALUES 
    ('KH001', N'Khách Hàng Vãng Lai (Web Guest)', N'Hà Nội', '0349348526'),
    ('KH002', N'Trần Thị Hoa', N'Hồ Chí Minh', '0912345678');
END

-- 3. Seed Danh Mục Hàng Hóa Gốm Sứ Bát Tràng Đầy Đủ Ảnh Online
MERGE INTO DanhMucHangHoa AS Target
USING (VALUES
    ('HH001', N'Bình Hoa Gốm Sứ Hỏa Biến Bát Tràng Dáng Hút Lộc', 'L04', 'KT02', 'CD01', 'LM01', 'HK01', 'MS02', 'VN', 'HV03', 25, 650000, 1250000, 12, N'Tác phẩm gốm hỏa biến Bát Tràng dáng hút tài lộc, được nung ở 1.300°C với lớp men chuyển màu độc bản.', 'https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?auto=format&fit=crop&w=800&q=80'),
    ('HH002', N'Bộ Ấm Trà Tử Sa Bát Tràng Men Lam Rồng Phượng', 'L02', 'KT01', 'CD02', 'LM02', 'HK03', 'MS01', 'VN', 'HV01', 40, 450000, 890000, 24, N'Bộ ấm trà Tử Sa giữ nhiệt cực tốt, men lam truyền thống vẽ tay tỉ mỉ rồng phượng mang may mắn.', 'https://images.unsplash.com/photo-1576092768241-dec231879fc3?auto=format&fit=crop&w=800&q=80'),
    ('HH003', N'Lộc Bình Gốm Sứ Men Rạn Dát Vàng Thuận Buồm Xuôi Gió', 'L05', 'KT04', 'CD04', 'LM03', 'HK02', 'MS04', 'VN', 'HV04', 8, 8500000, 15800000, 36, N'Lộc bình gốm sứ Bát Tràng men rạn cổ dát vàng 24K, biểu tượng chiêu tài đón lộc cho gia chủ.', 'https://images.unsplash.com/photo-1610701596007-11502861dcfa?auto=format&fit=crop&w=800&q=80'),
    ('HH004', N'Bộ Bát Đĩa Thờ Cúng Men Ngọc Vẽ Hoa Sen Bát Tràng', 'L03', 'KT02', 'CD03', 'LM04', 'HK04', 'MS03', 'VN', 'HV02', 15, 1800000, 3200000, 12, N'Bộ bát đĩa thờ cúng 22 món men ngọc xanh ngọc bích vẽ hoa sen đài các sang trọng.', 'https://images.unsplash.com/photo-1565183997392-2f6f122e5912?auto=format&fit=crop&w=800&q=80'),
    ('HH005', N'Tượng Phật Di Lặc Gốm Sứ Men Hỏa Biến Phong Thủy', 'L06', 'KT03', 'CD04', 'LM01', 'HK01', 'MS02', 'VN', 'HV03', 12, 2200000, 4500000, 24, N'Tượng Phật Di Lặc nụ cười hoan hỷ, nung nhiệt độ cao sắc hỏa biến lung linh tài lộc.', 'https://images.unsplash.com/photo-1544816155-12df9643f363?auto=format&fit=crop&w=800&q=80'),
    ('HH006', N'Bình Hút Tài Lộc Men Lam Cổ Sơn Thủy Hữu Tình', 'L04', 'KT02', 'CD01', 'LM02', 'HK01', 'MS01', 'VN', 'HV03', 30, 1200000, 2450000, 12, N'Bình hút tài lộc vẽ cảnh sơn thủy hữu tình mang phong thái thư thái, gia tăng vượng khí.', 'https://images.unsplash.com/photo-1581783342308-f792dbdd27c5?auto=format&fit=crop&w=800&q=80'),
    ('HH007', N'Bộ Tách Trà Gốm Sứ Men Ngọc Bát Tràng Đẳng Cấp', 'L02', 'KT01', 'CD02', 'LM04', 'HK03', 'MS03', 'VN', 'HV02', 50, 680000, 1350000, 12, N'Bộ trà men ngọc bóng bẩy cao cấp dành cho phòng khách biệt thự và quà tặng ngoại giao.', 'https://images.unsplash.com/photo-1594631252845-29fc4cc8cde9?auto=format&fit=crop&w=800&q=80'),
    ('HH008', N'Bình Dáng Tỏi Men Rạn Hoa Văn Tố Nữ Bát Tràng', 'L04', 'KT02', 'CD01', 'LM03', 'HK01', 'MS01', 'VN', 'HV03', 18, 950000, 1950000, 18, N'Bình dáng tỏi phong thủy men rạn giả cổ hoa văn tố nữ đậm đà văn hóa dân gian Việt Nam.', 'https://images.unsplash.com/photo-1615529182904-14819c35db37?auto=format&fit=crop&w=800&q=80')
) AS Source (MaHangHoa, TenHangHoa, MaLoai, MaKichThuoc, MaCongDung, MaLoaiMen, MaHinhKhoi, MaMauSac, MaNuocSX, MaHoaVan, SoLuong, DonGiaNhap, DonGiaBan, ThoiGianBaoHanh, GhiChu, HinhAnh)
ON Target.MaHangHoa = Source.MaHangHoa
WHEN MATCHED THEN
    UPDATE SET 
        TenHangHoa = Source.TenHangHoa,
        MaLoai = Source.MaLoai,
        MaKichThuoc = Source.MaKichThuoc,
        MaCongDung = Source.MaCongDung,
        MaLoaiMen = Source.MaLoaiMen,
        MaHinhKhoi = Source.MaHinhKhoi,
        MaMauSac = Source.MaMauSac,
        MaNuocSX = Source.MaNuocSX,
        MaHoaVan = Source.MaHoaVan,
        SoLuong = Source.SoLuong,
        DonGiaNhap = Source.DonGiaNhap,
        DonGiaBan = Source.DonGiaBan,
        ThoiGianBaoHanh = Source.ThoiGianBaoHanh,
        GhiChu = Source.GhiChu,
        HinhAnh = Source.HinhAnh
WHEN NOT MATCHED THEN
    INSERT (MaHangHoa, TenHangHoa, MaLoai, MaKichThuoc, MaCongDung, MaLoaiMen, MaHinhKhoi, MaMauSac, MaNuocSX, MaHoaVan, SoLuong, DonGiaNhap, DonGiaBan, ThoiGianBaoHanh, GhiChu, HinhAnh)
    VALUES (Source.MaHangHoa, Source.TenHangHoa, Source.MaLoai, Source.MaKichThuoc, Source.MaCongDung, Source.MaLoaiMen, Source.MaHinhKhoi, Source.MaMauSac, Source.MaNuocSX, Source.MaHoaVan, Source.SoLuong, Source.DonGiaNhap, Source.DonGiaBan, Source.ThoiGianBaoHanh, Source.GhiChu, Source.HinhAnh);
GO

PRINT N'DA SEED THANH CONG DU LIEU GON SU BAT TRANG VA IMAGE URLS VAC DB!';
