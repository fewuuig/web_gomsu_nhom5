/* ================================================================
   CƠ SỞ DỮ LIỆU: QUẢN LÝ BÁN ĐỒ GỐM SỨ
   SQL Server 2022 - Khởi tạo các bảng
   ================================================================ */

IF DB_ID('WebQuanLyBanDoGomSu') IS NOT NULL
BEGIN
    ALTER DATABASE WebQuanLyBanDoGomSu SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE WebQuanLyBanDoGomSu;
END
GO

CREATE DATABASE WebQuanLyBanDoGomSu;
GO

USE WebQuanLyBanDoGomSu;
GO

/* ---------------- 1. CÁC BẢNG DANH MỤC ---------------- */

CREATE TABLE KichThuoc (
    MaKichThuoc     VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenKichThuoc    NVARCHAR(100)   NOT NULL
);
GO

CREATE TABLE Loai (
    MaLoai          VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenLoai         NVARCHAR(100)   NOT NULL
);
GO

CREATE TABLE CongDung (
    MaCongDung      VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenCongDung     NVARCHAR(100)   NOT NULL
);
GO

CREATE TABLE LoaiMen (
    MaLoaiMen       VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenLoaiMen      NVARCHAR(100)   NOT NULL
);
GO

CREATE TABLE NuocSanXuat (
    MaNuocSX        VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenNuocSX       NVARCHAR(100)   NOT NULL
);
GO

CREATE TABLE MauSac (
    MaMauSac        VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenMauSac       NVARCHAR(100)   NOT NULL
);
GO

CREATE TABLE HinhKhoi (
    MaHinhKhoi      VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenHinhKhoi     NVARCHAR(100)   NOT NULL
);
GO

CREATE TABLE HoaVan (
    MaHoaVan        VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenHoaVan       NVARCHAR(100)   NOT NULL
);
GO

CREATE TABLE CongViec (
    MaCV            VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenCV           NVARCHAR(100)   NOT NULL,
    MucLuong        DECIMAL(18,2)   NULL
);
GO

/* ---------------- 2. DANH MỤC HÀNG HÓA ---------------- */

CREATE TABLE DanhMucHangHoa (
    MaHangHoa       VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenHangHoa      NVARCHAR(200)   NOT NULL,
    MaLoai          VARCHAR(10)     NOT NULL,
    MaKichThuoc     VARCHAR(10)     NOT NULL,
    MaCongDung      VARCHAR(10)     NOT NULL,
    MaLoaiMen       VARCHAR(10)     NOT NULL,
    MaHinhKhoi      VARCHAR(10)     NOT NULL,
    MaMauSac        VARCHAR(10)     NOT NULL,
    MaNuocSX        VARCHAR(10)     NOT NULL,
    MaHoaVan        VARCHAR(10)     NULL,
    SoLuong         INT             NOT NULL DEFAULT 0,
    DonGiaNhap      DECIMAL(18,2)   NOT NULL DEFAULT 0,
    DonGiaBan       DECIMAL(18,2)   NOT NULL DEFAULT 0,
    ThoiGianBaoHanh INT             NULL,
    Anh             VARBINARY(MAX)  NULL,
    GhiChu          NVARCHAR(500)   NULL,

    CONSTRAINT FK_DMHH_Loai      FOREIGN KEY (MaLoai)      REFERENCES Loai(MaLoai),
    CONSTRAINT FK_DMHH_KichThuoc FOREIGN KEY (MaKichThuoc) REFERENCES KichThuoc(MaKichThuoc),
    CONSTRAINT FK_DMHH_CongDung  FOREIGN KEY (MaCongDung)  REFERENCES CongDung(MaCongDung),
    CONSTRAINT FK_DMHH_LoaiMen   FOREIGN KEY (MaLoaiMen)   REFERENCES LoaiMen(MaLoaiMen),
    CONSTRAINT FK_DMHH_HinhKhoi  FOREIGN KEY (MaHinhKhoi)  REFERENCES HinhKhoi(MaHinhKhoi),
    CONSTRAINT FK_DMHH_MauSac    FOREIGN KEY (MaMauSac)    REFERENCES MauSac(MaMauSac),
    CONSTRAINT FK_DMHH_NuocSX    FOREIGN KEY (MaNuocSX)    REFERENCES NuocSanXuat(MaNuocSX),
    CONSTRAINT FK_DMHH_HoaVan    FOREIGN KEY (MaHoaVan)    REFERENCES HoaVan(MaHoaVan)
);
GO

/* ---------------- 3. NHÂN VIÊN ---------------- */

CREATE TABLE NhanVien (
    MaNV        VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenNV       NVARCHAR(100)   NOT NULL,
    GioiTinh    NVARCHAR(10)    NULL,
    NgaySinh    DATE            NULL,
    DienThoai   VARCHAR(20)     NULL,
    DiaChi      NVARCHAR(200)   NULL,
    MaCV        VARCHAR(10)     NOT NULL,

    CONSTRAINT FK_NV_CV FOREIGN KEY (MaCV) REFERENCES CongViec(MaCV)
);
GO

/* ---------------- 4. KHÁCH HÀNG / NHÀ CUNG CẤP ---------------- */

CREATE TABLE KhachHang (
    MaKhach     VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenKhach    NVARCHAR(100)   NOT NULL,
    DiaChi      NVARCHAR(200)   NULL,
    DienThoai   VARCHAR(20)     NULL
);
GO

CREATE TABLE NhaCungCap (
    MaNCC       VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenNCC      NVARCHAR(100)   NOT NULL,
    DiaChi      NVARCHAR(200)   NULL,
    DienThoai   VARCHAR(20)     NULL
);
GO

/* ---------------- 5. HÓA ĐƠN NHẬP ---------------- */

CREATE TABLE HoaDonNhap (
    SoHDN       VARCHAR(10)     NOT NULL PRIMARY KEY,
    MaNV        VARCHAR(10)     NOT NULL,
    NgayNhap    DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    MaNCC       VARCHAR(10)     NOT NULL,
    TongTien    DECIMAL(18,2)   NOT NULL DEFAULT 0,

    CONSTRAINT FK_HDN_NV  FOREIGN KEY (MaNV)  REFERENCES NhanVien(MaNV),
    CONSTRAINT FK_HDN_NCC FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC)
);
GO

CREATE TABLE ChiTietHoaDonNhap (
    SoHDN       VARCHAR(10)     NOT NULL,
    MaHangHoa   VARCHAR(10)     NOT NULL,
    SoLuong     INT             NOT NULL,
    DonGia      DECIMAL(18,2)   NOT NULL,
    ThanhTien   AS (SoLuong * DonGia) PERSISTED,

    CONSTRAINT PK_CTHDN        PRIMARY KEY (SoHDN, MaHangHoa),
    CONSTRAINT FK_CTHDN_HDN     FOREIGN KEY (SoHDN)     REFERENCES HoaDonNhap(SoHDN),
    CONSTRAINT FK_CTHDN_HangHoa FOREIGN KEY (MaHangHoa) REFERENCES DanhMucHangHoa(MaHangHoa)
);
GO

/* ---------------- 6. HÓA ĐƠN BÁN ---------------- */

CREATE TABLE HoaDonBan (
    SoHDB       VARCHAR(10)     NOT NULL PRIMARY KEY,
    MaNV        VARCHAR(10)     NOT NULL,
    NgayBan     DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    MaKhach     VARCHAR(10)     NOT NULL,
    TongTien    DECIMAL(18,2)   NOT NULL DEFAULT 0,

    CONSTRAINT FK_HDB_NV    FOREIGN KEY (MaNV)    REFERENCES NhanVien(MaNV),
    CONSTRAINT FK_HDB_Khach FOREIGN KEY (MaKhach) REFERENCES KhachHang(MaKhach)
);
GO

CREATE TABLE ChiTietHoaDonBan (
    SoHDB       VARCHAR(10)     NOT NULL,
    MaHangHoa   VARCHAR(10)     NOT NULL,
    SoLuong     INT             NOT NULL,
    GiamGia     DECIMAL(5,2)    NOT NULL DEFAULT 0,
    ThanhTien   DECIMAL(18,2)   NOT NULL DEFAULT 0,

    CONSTRAINT PK_CTHDB        PRIMARY KEY (SoHDB, MaHangHoa),
    CONSTRAINT FK_CTHDB_HDB     FOREIGN KEY (SoHDB)     REFERENCES HoaDonBan(SoHDB),
    CONSTRAINT FK_CTHDB_HangHoa FOREIGN KEY (MaHangHoa) REFERENCES DanhMucHangHoa(MaHangHoa)
);
GO

/* ---------------- 7. PHÂN QUYỀN VÀ TÀI KHOẢN ---------------- */

CREATE TABLE Quyen (
    MaQuyen     VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenQuyen    NVARCHAR(50)    NOT NULL   -- VD: Admin, Bán hàng, Kho, Kế toán...
);
GO

CREATE TABLE TaiKhoan (
    MaTK            VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenDangNhap     VARCHAR(50)     NOT NULL UNIQUE,
    MatKhau         VARBINARY(256)  NOT NULL,   -- lưu hash, không lưu plain text
    MaNV            VARCHAR(10)     NULL,
    MaKhach         VARCHAR(10)     NULL, 
    Email           VARCHAR(50)     NOT NULL,
    MaQuyen         VARCHAR(10)     NOT NULL,
    TrangThai       BIT             NOT NULL DEFAULT 1,  -- 1: đang hoạt động, 0: khóa
    LanDangNhapCuoi DATETIME        NULL,

    CONSTRAINT FK_TK_NV    FOREIGN KEY (MaNV)    REFERENCES NhanVien(MaNV),
    CONSTRAINT FK_TK_KH    FOREIGN KEY (MaKhach) REFERENCES KhachHang(MaKhach),
    CONSTRAINT FK_TK_Quyen FOREIGN KEY (MaQuyen) REFERENCES Quyen(MaQuyen),
    CONSTRAINT CK_TK_LoaiTK CHECK (
        (MaNV IS NOT NULL AND MaKhach IS NULL) OR
        (MaNV IS NULL AND MaKhach IS NOT NULL)
    )
);
GO

/* ---------------- 8. MÃ GIẢM GIÁ ---------------- */

CREATE TABLE MaGiamGia (
    MaGiamGia       VARCHAR(10)     NOT NULL PRIMARY KEY,
    TenGiamGia      NVARCHAR(100)   NOT NULL,
    PhanTramGiam    DECIMAL(5,2)    NOT NULL,   -- % giảm, VD: 10.00 = 10%
    NgayBatDau      DATE            NOT NULL,
    NgayKetThuc     DATE            NOT NULL,
    TrangThai       BIT             NOT NULL DEFAULT 1,  -- 1: còn hiệu lực, 0: hết hạn/khóa

    CONSTRAINT CK_MGG_PhanTram CHECK (PhanTramGiam BETWEEN 0 AND 100),
    CONSTRAINT CK_MGG_NgayHopLe CHECK (NgayKetThuc >= NgayBatDau)
);
GO

CREATE TABLE KhachHang_MaGiamGia (
    MaKhach         VARCHAR(10)     NOT NULL,
    MaGiamGia       VARCHAR(10)     NOT NULL,
    NgayNhan        DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    DaSuDung        BIT             NOT NULL DEFAULT 0,

    CONSTRAINT PK_KH_MGG       PRIMARY KEY (MaKhach, MaGiamGia),
    CONSTRAINT FK_KHMGG_Khach  FOREIGN KEY (MaKhach)    REFERENCES KhachHang(MaKhach),
    CONSTRAINT FK_KHMGG_MaGG   FOREIGN KEY (MaGiamGia)  REFERENCES MaGiamGia(MaGiamGia)
);
GO

-- 23. Bảng lưu trữ tin nhắn chat tư vấn trực tuyến (Bảo vệ clustered index B-Tree với BIGINT IDENTITY tự tăng)
IF OBJECT_ID('TinNhanChat', 'U') IS NULL
BEGIN
    CREATE TABLE TinNhanChat (
        Id          BIGINT IDENTITY(1,1)    NOT NULL,
        SessionId   VARCHAR(50)             NOT NULL,
        SenderId    NVARCHAR(50)            NOT NULL,
        SenderName  NVARCHAR(100)           NOT NULL,
        Content     NVARCHAR(MAX)           NOT NULL,
        IsNhanVien  BIT                     NOT NULL DEFAULT 0,
        ThoiGian    DATETIME2               NOT NULL DEFAULT GETDATE(),

        CONSTRAINT PK_TinNhanChat PRIMARY KEY CLUSTERED (Id)
    );

    CREATE NONCLUSTERED INDEX IX_TinNhanChat_SessionId_Id
    ON TinNhanChat (SessionId, Id DESC);
END
GO
