using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.Out;

namespace QuanLyBanDoGomSu.Infrastructure.Persistence.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly QuanLyBanDoGomSuDbContext _context;

        public StatisticsRepository(QuanLyBanDoGomSuDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatisticsDto> GetSalesStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var queryHdb = _context.HoaDonBans
                .AsNoTracking()
                .Where(h => h.NgayBan >= startDate && h.NgayBan <= endDate);

            var tongDoanhThu = await queryHdb.SumAsync(h => h.TongTien);
            var tongDonHang = await queryHdb.CountAsync();
            var tongKhachHang = await queryHdb.Select(h => h.MaKhach).Distinct().CountAsync();

            var orderIds = await queryHdb.Select(h => h.SoHDB).ToListAsync();

            var queryItems = _context.ChiTietHoaDonBans
                .AsNoTracking()
                .Where(c => orderIds.Contains(c.SoHDB));

            var tongSanPhamDaBan = await queryItems.SumAsync(c => c.SoLuong);

            // Thống kê doanh thu theo ngày
            var dailyDataRaw = await queryHdb
                .GroupBy(h => h.NgayBan.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    DoanhThu = g.Sum(x => x.TongTien),
                    SoDonHang = g.Count()
                })
                .ToListAsync();

            var dailyData = dailyDataRaw
                .Select(x => new DailyRevenueDto
                {
                    Ngay = x.Date.ToString("yyyy-MM-dd"),
                    DoanhThu = x.DoanhThu,
                    SoDonHang = x.SoDonHang
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            // Top sản phẩm bán chạy (Top 5)
            var topProducts = await queryItems
                .GroupBy(c => new { c.MaHangHoa, c.HangHoa.TenHangHoa })
                .Select(g => new TopProductDto
                {
                    MaHangHoa = g.Key.MaHangHoa,
                    TenHangHoa = g.Key.TenHangHoa ?? string.Empty,
                    SoLuongDaBan = g.Sum(x => x.SoLuong),
                    TongTien = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.SoLuongDaBan)
                .Take(5)
                .ToListAsync();

            return new DashboardStatisticsDto
            {
                TongDoanhThu = tongDoanhThu,
                TongDonHang = tongDonHang,
                TongSanPhamDaBan = tongSanPhamDaBan,
                TongKhachHang = tongKhachHang,
                DoanhThuTheoNgay = dailyData,
                TopSanPhamBanChay = topProducts
            };
        }

        public async Task<SalesReportDataDto> GetSalesReportDataAsync(DateTime startDate, DateTime endDate)
        {
            var summary = await GetSalesStatisticsAsync(startDate, endDate);

            var orders = await _context.HoaDonBans
                .AsNoTracking()
                .Include(h => h.KhachHang)
                .Include(h => h.NhanVien)
                .Where(h => h.NgayBan >= startDate && h.NgayBan <= endDate)
                .OrderByDescending(h => h.NgayBan)
                .Select(h => new OrderReportItemDto
                {
                    SoHDB = h.SoHDB,
                    NgayBan = h.NgayBan,
                    TenKhachHang = h.KhachHang != null ? h.KhachHang.TenKhach : "Khách vãng lai",
                    DienThoaiKhach = h.KhachHang != null ? (h.KhachHang.DienThoai ?? string.Empty) : string.Empty,
                    TenNhanVien = h.NhanVien != null ? h.NhanVien.TenNV : string.Empty,
                    TongTien = h.TongTien
                })
                .ToListAsync();

            var orderIds = orders.Select(o => o.SoHDB).ToList();

            var allSoldProducts = await _context.ChiTietHoaDonBans
                .AsNoTracking()
                .Where(c => orderIds.Contains(c.SoHDB))
                .GroupBy(c => new { c.MaHangHoa, c.HangHoa.TenHangHoa })
                .Select(g => new TopProductDto
                {
                    MaHangHoa = g.Key.MaHangHoa,
                    TenHangHoa = g.Key.TenHangHoa ?? string.Empty,
                    SoLuongDaBan = g.Sum(x => x.SoLuong),
                    TongTien = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.SoLuongDaBan)
                .Take(100)
                .ToListAsync();

            return new SalesReportDataDto
            {
                StartDate = startDate,
                EndDate = endDate,
                Summary = summary,
                Orders = orders,
                TopProducts = allSoldProducts
            };
        }
    }
}
