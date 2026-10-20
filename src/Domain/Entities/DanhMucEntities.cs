using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanDoGomSu.Domain.Entities
{
    [Table("KichThuoc")]
    public class KichThuoc
    {
        [Key]
        [StringLength(10)]
        [Column("MaKichThuoc", TypeName = "VARCHAR(10)")]
        public string MaKichThuoc { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenKichThuoc", TypeName = "NVARCHAR(100)")]
        public string TenKichThuoc { get; set; }
    }

    [Table("Loai")]
    public class Loai
    {
        [Key]
        [StringLength(10)]
        [Column("MaLoai", TypeName = "VARCHAR(10)")]
        public string MaLoai { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenLoai", TypeName = "NVARCHAR(100)")]
        public string TenLoai { get; set; }
    }

    [Table("CongDung")]
    public class CongDung
    {
        [Key]
        [StringLength(10)]
        [Column("MaCongDung", TypeName = "VARCHAR(10)")]
        public string MaCongDung { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenCongDung", TypeName = "NVARCHAR(100)")]
        public string TenCongDung { get; set; }
    }

    [Table("LoaiMen")]
    public class LoaiMen
    {
        [Key]
        [StringLength(10)]
        [Column("MaLoaiMen", TypeName = "VARCHAR(10)")]
        public string MaLoaiMen { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenLoaiMen", TypeName = "NVARCHAR(100)")]
        public string TenLoaiMen { get; set; }
    }

    [Table("NuocSanXuat")]
    public class NuocSanXuat
    {
        [Key]
        [StringLength(10)]
        [Column("MaNuocSX", TypeName = "VARCHAR(10)")]
        public string MaNuocSX { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenNuocSX", TypeName = "NVARCHAR(100)")]
        public string TenNuocSX { get; set; }
    }

    [Table("MauSac")]
    public class MauSac
    {
        [Key]
        [StringLength(10)]
        [Column("MaMauSac", TypeName = "VARCHAR(10)")]
        public string MaMauSac { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenMauSac", TypeName = "NVARCHAR(100)")]
        public string TenMauSac { get; set; }
    }

    [Table("HinhKhoi")]
    public class HinhKhoi
    {
        [Key]
        [StringLength(10)]
        [Column("MaHinhKhoi", TypeName = "VARCHAR(10)")]
        public string MaHinhKhoi { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenHinhKhoi", TypeName = "NVARCHAR(100)")]
        public string TenHinhKhoi { get; set; }
    }

    [Table("HoaVan")]
    public class HoaVan
    {
        [Key]
        [StringLength(10)]
        [Column("MaHoaVan", TypeName = "VARCHAR(10)")]
        public string MaHoaVan { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenHoaVan", TypeName = "NVARCHAR(100)")]
        public string TenHoaVan { get; set; }
    }

    [Table("CongViec")]
    public class CongViec
    {
        [Key]
        [StringLength(10)]
        [Column("MaCV", TypeName = "VARCHAR(10)")]
        public string MaCV { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenCV", TypeName = "NVARCHAR(100)")]
        public string TenCV { get; set; }

        [Column("MucLuong", TypeName = "DECIMAL(18,2)")]
        public decimal? MucLuong { get; set; }
    }
}
