using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanDoGomSu.Application.Port.Out;
using QuanLyBanDoGomSu.Domain.Entities;

namespace QuanLyBanDoGomSu.Infrastructure.Persistence.Repositories
{
    public class DanhMucHangHoaRepository : IDanhMucHangHoaRepository
    {
        private readonly QuanLyBanDoGomSuDbContext _context;

        public DanhMucHangHoaRepository(QuanLyBanDoGomSuDbContext context)
        {
            _context = context;
        }

        public async Task<DanhMucHangHoa> GetByIdAsync(string maHangHoa)
        {
            return await _context.DanhMucHangHoas
                .Include(h => h.Loai)
                .Include(h => h.KichThuoc)
                .Include(h => h.CongDung)
                .Include(h => h.LoaiMen)
                .Include(h => h.HinhKhoi)
                .Include(h => h.MauSac)
                .Include(h => h.NuocSanXuat)
                .Include(h => h.HoaVan)
                .FirstOrDefaultAsync(h => h.MaHangHoa == maHangHoa);
        }

        public async Task<(List<DanhMucHangHoa> Items, int TotalCount)> GetPagedAsync(
            string keyword, string maLoai, string maKichThuoc, string maLoaiMen, string maMauSac, 
            decimal? minPrice, decimal? maxPrice, int pageIndex, int pageSize)
        {
            var query = _context.DanhMucHangHoas
                .Include(h => h.Loai)
                .Include(h => h.KichThuoc)
                .Include(h => h.CongDung)
                .Include(h => h.LoaiMen)
                .Include(h => h.HinhKhoi)
                .Include(h => h.MauSac)
                .Include(h => h.NuocSanXuat)
                .Include(h => h.HoaVan)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(h => h.TenHangHoa.Contains(keyword) || h.MaHangHoa.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(maLoai)) query = query.Where(h => h.MaLoai == maLoai);
            if (!string.IsNullOrWhiteSpace(maKichThuoc)) query = query.Where(h => h.MaKichThuoc == maKichThuoc);
            if (!string.IsNullOrWhiteSpace(maLoaiMen)) query = query.Where(h => h.MaLoaiMen == maLoaiMen);
            if (!string.IsNullOrWhiteSpace(maMauSac)) query = query.Where(h => h.MaMauSac == maMauSac);

            if (minPrice.HasValue) query = query.Where(h => h.DonGiaBan >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(h => h.DonGiaBan <= maxPrice.Value);

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(h => h.MaHangHoa)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(DanhMucHangHoa entity)
        {
            await _context.DanhMucHangHoas.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DanhMucHangHoa entity)
        {
            _context.DanhMucHangHoas.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string maHangHoa)
        {
            var entity = await _context.DanhMucHangHoas.FindAsync(maHangHoa);
            if (entity != null)
            {
                _context.DanhMucHangHoas.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string maHangHoa)
        {
            return await _context.DanhMucHangHoas.AnyAsync(h => h.MaHangHoa == maHangHoa);
        }
    }
}
