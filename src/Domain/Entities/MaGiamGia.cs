using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanDoGomSu.Domain.Entities
{
    [Table("MaGiamGia")]
    public class MaGiamGia
    {
        [Key]
        [StringLength(10)]
        [Column("MaGiamGia", TypeName = "VARCHAR(10)")]
        public string MaCode { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TenGiamGia", TypeName = "NVARCHAR(100)")]
        public string TenGiamGia { get; set; }

        [Column("PhanTramGiam", TypeName = "DECIMAL(5,2)")]
        public decimal PhanTramGiam { get; set; }

        [Column("NgayBatDau", TypeName = "DATE")]
        public DateTime NgayBatDau { get; set; }

        [Column("NgayKetThuc", TypeName = "DATE")]
        public DateTime NgayKetThuc { get; set; }

        [Column("TrangThai")]
        public bool TrangThai { get; set; } = true;
    }

    [Table("KhachHang_MaGiamGia")]
    public class KhachHang_MaGiamGia
    {
        [StringLength(10)]
        [Column("MaKhach", TypeName = "VARCHAR(10)")]
        public string MaKhach { get; set; }

        [StringLength(10)]
        [Column("MaGiamGia", TypeName = "VARCHAR(10)")]
        public string MaGiamGia { get; set; }

        [Column("NgayNhan", TypeName = "DATE")]
        public DateTime NgayNhan { get; set; } = DateTime.Now;

        [Column("DaSuDung")]
        public bool DaSuDung { get; set; } = false;

        [ForeignKey("MaKhach")]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey("MaGiamGia")]
        public virtual MaGiamGia Voucher { get; set; }
    }
}
