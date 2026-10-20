using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanDoGomSu.Domain.Entities
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        [StringLength(10)]
        [Column("MaNV", TypeName = "VARCHAR(10)")]
        public string MaNV { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenNV", TypeName = "NVARCHAR(100)")]
        public string TenNV { get; set; }

        [StringLength(10)]
        [Column("GioiTinh", TypeName = "NVARCHAR(10)")]
        public string GioiTinh { get; set; }

        [Column("NgaySinh", TypeName = "DATE")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(20)]
        [Column("DienThoai", TypeName = "VARCHAR(20)")]
        public string DienThoai { get; set; }

        [StringLength(200)]
        [Column("DiaChi", TypeName = "NVARCHAR(200)")]
        public string DiaChi { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaCV", TypeName = "VARCHAR(10)")]
        public string MaCV { get; set; }

        [ForeignKey("MaCV")]
        public virtual CongViec CongViec { get; set; }
    }

    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        [StringLength(10)]
        [Column("MaKhach", TypeName = "VARCHAR(10)")]
        public string MaKhach { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenKhach", TypeName = "NVARCHAR(100)")]
        public string TenKhach { get; set; }

        [StringLength(200)]
        [Column("DiaChi", TypeName = "NVARCHAR(200)")]
        public string DiaChi { get; set; }

        [StringLength(20)]
        [Column("DienThoai", TypeName = "VARCHAR(20)")]
        public string DienThoai { get; set; }
    }

    [Table("NhaCungCap")]
    public class NhaCungCap
    {
        [Key]
        [StringLength(10)]
        [Column("MaNCC", TypeName = "VARCHAR(10)")]
        public string MaNCC { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenNCC", TypeName = "NVARCHAR(100)")]
        public string TenNCC { get; set; }

        [StringLength(200)]
        [Column("DiaChi", TypeName = "NVARCHAR(200)")]
        public string DiaChi { get; set; }

        [StringLength(20)]
        [Column("DienThoai", TypeName = "VARCHAR(20)")]
        public string DienThoai { get; set; }
    }

    [Table("Quyen")]
    public class Quyen
    {
        [Key]
        [StringLength(10)]
        [Column("MaQuyen", TypeName = "VARCHAR(10)")]
        public string MaQuyen { get; set; }

        [Required]
        [StringLength(50)]
        [Column("TenQuyen", TypeName = "NVARCHAR(50)")]
        public string TenQuyen { get; set; }
    }

    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [StringLength(10)]
        [Column("MaTK", TypeName = "VARCHAR(10)")]
        public string MaTK { get; set; }

        [Required]
        [StringLength(50)]
        [Column("TenDangNhap", TypeName = "VARCHAR(50)")]
        public string TenDangNhap { get; set; }

        [Required]
        [Column("MatKhau", TypeName = "VARBINARY(256)")]
        public byte[] MatKhau { get; set; }

        [StringLength(10)]
        [Column("MaNV", TypeName = "VARCHAR(10)")]
        public string? MaNV { get; set; }

        [StringLength(10)]
        [Column("MaKhach", TypeName = "VARCHAR(10)")]
        public string? MaKhach { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Email", TypeName = "VARCHAR(50)")]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaQuyen", TypeName = "VARCHAR(10)")]
        public string MaQuyen { get; set; }

        [Column("TrangThai")]
        public bool TrangThai { get; set; } = true;

        [Column("LanDangNhapCuoi")]
        public DateTime? LanDangNhapCuoi { get; set; }

        [Column("Avatar", TypeName = "NVARCHAR(MAX)")]
        public string? Avatar { get; set; }

        [ForeignKey("MaNV")]
        public virtual NhanVien? NhanVien { get; set; }

        [ForeignKey("MaKhach")]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey("MaQuyen")]
        public virtual Quyen Quyen { get; set; }
    }

    [Table("HoaDonBan")]
    public class HoaDonBan
    {
        [Key]
        [StringLength(20)]
        [Column("SoHDB", TypeName = "VARCHAR(20)")]
        public string SoHDB { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaNV", TypeName = "VARCHAR(10)")]
        public string MaNV { get; set; }

        [Column("NgayBan", TypeName = "DATE")]
        public DateTime NgayBan { get; set; } = DateTime.Now;

        [Required]
        [StringLength(10)]
        [Column("MaKhach", TypeName = "VARCHAR(10)")]
        public string MaKhach { get; set; }

        [Column("TongTien", TypeName = "DECIMAL(18,2)")]
        public decimal TongTien { get; set; } = 0;

        [StringLength(255)]
        [Column("DiaChiGiaoHang", TypeName = "NVARCHAR(255)")]
        public string? DiaChiGiaoHang { get; set; }

        [StringLength(20)]
        [Column("SoDienThoaiNhan", TypeName = "VARCHAR(20)")]
        public string? SoDienThoaiNhan { get; set; }

        [StringLength(100)]
        [Column("TenNguoiNhan", TypeName = "NVARCHAR(100)")]
        public string? TenNguoiNhan { get; set; }

        [StringLength(500)]
        [Column("GhiChu", TypeName = "NVARCHAR(500)")]
        public string? GhiChu { get; set; }

        [ForeignKey("MaNV")]
        public virtual NhanVien NhanVien { get; set; }

        [ForeignKey("MaKhach")]
        public virtual KhachHang KhachHang { get; set; }

        public virtual ICollection<ChiTietHoaDonBan> ChiTietHoaDonBans { get; set; } = new List<ChiTietHoaDonBan>();
    }

    [Table("DiaChiNhanHang")]
    public class DiaChiNhanHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaKhach", TypeName = "VARCHAR(10)")]
        public string MaKhach { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenNguoiNhan", TypeName = "NVARCHAR(100)")]
        public string TenNguoiNhan { get; set; }

        [Required]
        [StringLength(20)]
        [Column("SoDienThoai", TypeName = "VARCHAR(20)")]
        public string SoDienThoai { get; set; }

        [Required]
        [StringLength(255)]
        [Column("DiaChiChiTiet", TypeName = "NVARCHAR(255)")]
        public string DiaChiChiTiet { get; set; }

        [Column("LaMacDinh")]
        public bool LaMacDinh { get; set; } = false;

        [Column("NgayTao", TypeName = "DATETIME")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [ForeignKey("MaKhach")]
        public virtual KhachHang? KhachHang { get; set; }
    }

    [Table("ChiTietHoaDonBan")]
    public class ChiTietHoaDonBan
    {
        [StringLength(20)]
        [Column("SoHDB", TypeName = "VARCHAR(20)")]
        public string SoHDB { get; set; }

        [StringLength(10)]
        [Column("MaHangHoa", TypeName = "VARCHAR(10)")]
        public string MaHangHoa { get; set; }

        [Column("SoLuong")]
        public int SoLuong { get; set; }

        [Column("GiamGia", TypeName = "DECIMAL(5,2)")]
        public decimal GiamGia { get; set; } = 0;

        [Column("ThanhTien", TypeName = "DECIMAL(18,2)")]
        public decimal ThanhTien { get; set; } = 0;

        [ForeignKey("SoHDB")]
        public virtual HoaDonBan HoaDonBan { get; set; }

        [ForeignKey("MaHangHoa")]
        public virtual DanhMucHangHoa HangHoa { get; set; }
    }

    [Table("HoaDonNhap")]
    public class HoaDonNhap
    {
        [Key]
        [StringLength(10)]
        [Column("SoHDN", TypeName = "VARCHAR(10)")]
        public string SoHDN { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaNV", TypeName = "VARCHAR(10)")]
        public string MaNV { get; set; }

        [Column("NgayNhap", TypeName = "DATE")]
        public DateTime NgayNhap { get; set; } = DateTime.Now;

        [Required]
        [StringLength(10)]
        [Column("MaNCC", TypeName = "VARCHAR(10)")]
        public string MaNCC { get; set; }

        [Column("TongTien", TypeName = "DECIMAL(18,2)")]
        public decimal TongTien { get; set; } = 0;

        [ForeignKey("MaNV")]
        public virtual NhanVien NhanVien { get; set; }

        [ForeignKey("MaNCC")]
        public virtual NhaCungCap NhaCungCap { get; set; }
    }

    [Table("ChiTietHoaDonNhap")]
    public class ChiTietHoaDonNhap
    {
        [StringLength(10)]
        [Column("SoHDN", TypeName = "VARCHAR(10)")]
        public string SoHDN { get; set; }

        [StringLength(10)]
        [Column("MaHangHoa", TypeName = "VARCHAR(10)")]
        public string MaHangHoa { get; set; }

        [Column("SoLuong")]
        public int SoLuong { get; set; }

        [Column("DonGia", TypeName = "DECIMAL(18,2)")]
        public decimal DonGia { get; set; } = 0;

        [Column("ThanhTien", TypeName = "DECIMAL(18,2)")]
        public decimal ThanhTien { get; set; } = 0;

        [ForeignKey("SoHDN")]
        public virtual HoaDonNhap HoaDonNhap { get; set; }

        [ForeignKey("MaHangHoa")]
        public virtual DanhMucHangHoa HangHoa { get; set; }
    }
}
