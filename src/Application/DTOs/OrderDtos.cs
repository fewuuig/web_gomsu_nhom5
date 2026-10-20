using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanDoGomSu.Application.DTOs
{
    public class OrderItemRequest
    {
        [Required]
        public string MaHangHoa { get; set; }

        [Required]
        [Range(1, 1000)]
        public int SoLuong { get; set; }

        public decimal GiamGia { get; set; } = 0;
    }

    public class CreateOrderRequest
    {
        public string? MaKhach { get; set; }
        public string? TenKhach { get; set; }
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? GhiChu { get; set; }
        public bool LuuVaoSoDiaChi { get; set; } = false;

        public string MaNV { get; set; } = "NV001"; // Mặc định Nhân viên bán hàng web

        [Required(ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm gốm sứ")]
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemDto
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaBan { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string? HinhAnh { get; set; }
    }

    public class OrderDto
    {
        public string SoHDB { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public DateTime NgayBan { get; set; }
        public string MaKhach { get; set; }
        public string TenKhach { get; set; }
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? GhiChu { get; set; }
        public decimal TongTien { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
