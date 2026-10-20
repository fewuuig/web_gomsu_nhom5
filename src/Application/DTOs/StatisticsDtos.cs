using System;
using System.Collections.Generic;

namespace QuanLyBanDoGomSu.Application.DTOs
{
    /// <summary>
    /// DTO tổng hợp số liệu thống kê doanh số cho Dashboard
    /// </summary>
    public class DashboardStatisticsDto
    {
        public decimal TongDoanhThu { get; set; }
        public int TongDonHang { get; set; }
        public int TongSanPhamDaBan { get; set; }
        public int TongKhachHang { get; set; }
        public List<DailyRevenueDto> DoanhThuTheoNgay { get; set; } = new List<DailyRevenueDto>();
        public List<TopProductDto> TopSanPhamBanChay { get; set; } = new List<TopProductDto>();
    }

    /// <summary>
    /// DTO biểu đồ doanh thu theo ngày
    /// </summary>
    public class DailyRevenueDto
    {
        public string Ngay { get; set; } = string.Empty; // Định dạng YYYY-MM-DD
        public decimal DoanhThu { get; set; }
        public int SoDonHang { get; set; }
    }

    /// <summary>
    /// DTO top sản phẩm bán chạy
    /// </summary>
    public class TopProductDto
    {
        public string MaHangHoa { get; set; } = string.Empty;
        public string TenHangHoa { get; set; } = string.Empty;
        public int SoLuongDaBan { get; set; }
        public decimal TongTien { get; set; }
    }

    /// <summary>
    /// DTO chi tiết hóa đơn phục vụ xuất báo cáo Excel
    /// </summary>
    public class OrderReportItemDto
    {
        public string SoHDB { get; set; } = string.Empty;
        public DateTime NgayBan { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string DienThoaiKhach { get; set; } = string.Empty;
        public string TenNhanVien { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
    }

    /// <summary>
    /// DTO tổng hợp toàn bộ dữ liệu báo cáo doanh số cho Excel
    /// </summary>
    public class SalesReportDataDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DashboardStatisticsDto Summary { get; set; } = new DashboardStatisticsDto();
        public List<OrderReportItemDto> Orders { get; set; } = new List<OrderReportItemDto>();
        public List<TopProductDto> TopProducts { get; set; } = new List<TopProductDto>();
    }
}
