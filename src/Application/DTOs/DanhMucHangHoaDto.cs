using System.ComponentModel.DataAnnotations;

namespace QuanLyBanDoGomSu.Application.DTOs
{
    public class DanhMucHangHoaDto
    {
        [Required(ErrorMessage = "Mã hàng hóa không được để trống")]
        [StringLength(10, ErrorMessage = "Mã hàng hóa tối đa 10 ký tự")]
        public string MaHangHoa { get; set; }

        [Required(ErrorMessage = "Tên hàng hóa không được để trống")]
        [StringLength(200, ErrorMessage = "Tên hàng hóa tối đa 200 ký tự")]
        public string TenHangHoa { get; set; }

        public string? MaLoai { get; set; }
        public string? TenLoai { get; set; }

        public string? MaKichThuoc { get; set; }
        public string? TenKichThuoc { get; set; }

        public string? MaCongDung { get; set; }
        public string? TenCongDung { get; set; }

        public string? MaLoaiMen { get; set; }
        public string? TenLoaiMen { get; set; }

        public string? MaHinhKhoi { get; set; }
        public string? TenHinhKhoi { get; set; }

        public string? MaMauSac { get; set; }
        public string? TenMauSac { get; set; }

        public string? MaNuocSX { get; set; }
        public string? TenNuocSX { get; set; }

        public string? MaHoaVan { get; set; }
        public string? TenHoaVan { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int SoLuong { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá nhập phải lớn hơn hoặc bằng 0")]
        public decimal DonGiaNhap { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá bán phải lớn hơn hoặc bằng 0")]
        public decimal DonGiaBan { get; set; }

        public int? ThoiGianBaoHanh { get; set; }
        public string? GhiChu { get; set; }
        public string? HinhAnh { get; set; }
        public string? AnhBase64 { get; set; }
    }
}
