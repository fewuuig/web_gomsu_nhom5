using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanDoGomSu.Domain.Entities
{
    [Table("DanhMucHangHoa")]
    public class DanhMucHangHoa
    {
        [Key]
        [StringLength(10)]
        [Column("MaHangHoa", TypeName = "VARCHAR(10)")]
        public string MaHangHoa { get; set; }

        [Required]
        [StringLength(200)]
        [Column("TenHangHoa", TypeName = "NVARCHAR(200)")]
        public string TenHangHoa { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaLoai", TypeName = "VARCHAR(10)")]
        public string MaLoai { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaKichThuoc", TypeName = "VARCHAR(10)")]
        public string MaKichThuoc { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaCongDung", TypeName = "VARCHAR(10)")]
        public string MaCongDung { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaLoaiMen", TypeName = "VARCHAR(10)")]
        public string MaLoaiMen { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaHinhKhoi", TypeName = "VARCHAR(10)")]
        public string MaHinhKhoi { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaMauSac", TypeName = "VARCHAR(10)")]
        public string MaMauSac { get; set; }

        [Required]
        [StringLength(10)]
        [Column("MaNuocSX", TypeName = "VARCHAR(10)")]
        public string MaNuocSX { get; set; }

        [StringLength(10)]
        [Column("MaHoaVan", TypeName = "VARCHAR(10)")]
        public string? MaHoaVan { get; set; }

        [Column("SoLuong")]
        public int SoLuong { get; set; } = 0;

        [Column("DonGiaNhap", TypeName = "DECIMAL(18,2)")]
        public decimal DonGiaNhap { get; set; } = 0;

        [Column("DonGiaBan", TypeName = "DECIMAL(18,2)")]
        public decimal DonGiaBan { get; set; } = 0;

        [Column("ThoiGianBaoHanh")]
        public int? ThoiGianBaoHanh { get; set; }

        [Column("Anh", TypeName = "VARBINARY(MAX)")]
        public byte[]? Anh { get; set; }

        [StringLength(500)]
        [Column("HinhAnh", TypeName = "NVARCHAR(500)")]
        public string? HinhAnh { get; set; }

        [StringLength(500)]
        [Column("GhiChu", TypeName = "NVARCHAR(500)")]
        public string? GhiChu { get; set; }

        // Navigation Properties
        [ForeignKey("MaLoai")]
        public virtual Loai Loai { get; set; }

        [ForeignKey("MaKichThuoc")]
        public virtual KichThuoc KichThuoc { get; set; }

        [ForeignKey("MaCongDung")]
        public virtual CongDung CongDung { get; set; }

        [ForeignKey("MaLoaiMen")]
        public virtual LoaiMen LoaiMen { get; set; }

        [ForeignKey("MaHinhKhoi")]
        public virtual HinhKhoi HinhKhoi { get; set; }

        [ForeignKey("MaMauSac")]
        public virtual MauSac MauSac { get; set; }

        [ForeignKey("MaNuocSX")]
        public virtual NuocSanXuat NuocSanXuat { get; set; }

        [ForeignKey("MaHoaVan")]
        public virtual HoaVan HoaVan { get; set; }
    }
}
