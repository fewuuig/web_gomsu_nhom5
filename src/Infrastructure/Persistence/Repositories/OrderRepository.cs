using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanDoGomSu.Application.Port.Out;
using QuanLyBanDoGomSu.Domain.Entities;

namespace QuanLyBanDoGomSu.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly QuanLyBanDoGomSuDbContext _context;

        public OrderRepository(QuanLyBanDoGomSuDbContext context)
        {
            _context = context;
        }

        public async Task<HoaDonBan> GetByIdAsync(string soHDB)
        {
            return await _context.HoaDonBans
                .Include(h => h.NhanVien)
                .Include(h => h.KhachHang)
                .Include(h => h.ChiTietHoaDonBans)
                    .ThenInclude(c => c.HangHoa)
                .FirstOrDefaultAsync(h => h.SoHDB == soHDB);
        }

        public async Task<(List<HoaDonBan> Items, int TotalCount)> GetOrderHistoryAsync(string maKhach, int pageIndex, int pageSize)
        {
            var query = _context.HoaDonBans
                .Include(h => h.KhachHang)
                .Include(h => h.NhanVien)
                .Include(h => h.ChiTietHoaDonBans)
                    .ThenInclude(c => c.HangHoa)
                .AsNoTracking()
                .OrderByDescending(h => h.SoHDB)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(maKhach) && !maKhach.Trim().Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                string search = maKhach.Trim();
                string cleanSearch = search.Replace(" ", "").Replace(".", "").Replace("-", "");
                query = query.Where(h =>
                    h.MaKhach == search ||
                    h.SoHDB == search ||
                    (h.SoDienThoaiNhan != null && (h.SoDienThoaiNhan == search || h.SoDienThoaiNhan.Contains(cleanSearch))) ||
                    (h.KhachHang != null && h.KhachHang.DienThoai != null && (h.KhachHang.DienThoai == search || h.KhachHang.DienThoai.Contains(cleanSearch))));
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(HoaDonBan order, List<ChiTietHoaDonBan> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.HoaDonBans.AddAsync(order);
                await _context.ChiTietHoaDonBans.AddRangeAsync(items);

                // Cập nhật giảm số lượng tồn kho sản phẩm gốm sứ
                foreach (var item in items)
                {
                    var product = await _context.DanhMucHangHoas.FindAsync(item.MaHangHoa);
                    if (product != null)
                    {
                        product.SoLuong = Math.Max(0, product.SoLuong - item.SoLuong);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Sinh Mã Hóa Đơn Bán Tăng Dần (Sequential Primary Key): HDB + YYYYMMDD + 0001
        /// Giúp bảo vệ B-Tree Clustered Index, chống phân mảnh trang và tối ưu bộ nhớ RAM trong SQL Server 2022.
        /// </summary>
        public async Task<string> GenerateSequentialOrderIdAsync()
        {
            string datePrefix = $"HDB{DateTime.Now:yyyyMMdd}";
            var lastOrder = await _context.HoaDonBans
                .Where(h => h.SoHDB.StartsWith(datePrefix))
                .OrderByDescending(h => h.SoHDB)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
            {
                return $"{datePrefix}0001";
            }

            string suffixStr = lastOrder.SoHDB.Substring(datePrefix.Length);
            if (int.TryParse(suffixStr, out int currentNumber))
            {
                return $"{datePrefix}{(currentNumber + 1):D4}";
            }

            return $"{datePrefix}{Guid.NewGuid().ToString("N").Substring(0, 4)}";
        }
    }
}
