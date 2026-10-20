using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Domain.Entities;
using QuanLyBanDoGomSu.Exception;
using QuanLyBanDoGomSu.Infrastructure.Persistence;

namespace QuanLyBanDoGomSu.Application.Service
{
    public class AddressService : IAddressService
    {
        private readonly QuanLyBanDoGomSuDbContext _context;

        public AddressService(QuanLyBanDoGomSuDbContext context)
        {
            _context = context;
        }

        public async Task<List<AddressDto>> GetAddressesByCustomerAsync(string maKhach)
        {
            if (string.IsNullOrWhiteSpace(maKhach))
            {
                return new List<AddressDto>();
            }

            var addresses = await _context.DiaChiNhanHangs
                .AsNoTracking()
                .Where(a => a.MaKhach == maKhach.Trim())
                .OrderByDescending(a => a.LaMacDinh)
                .ThenByDescending(a => a.NgayTao)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    MaKhach = a.MaKhach,
                    TenNguoiNhan = a.TenNguoiNhan,
                    SoDienThoai = a.SoDienThoai,
                    DiaChiChiTiet = a.DiaChiChiTiet,
                    LaMacDinh = a.LaMacDinh,
                    NgayTao = a.NgayTao
                })
                .ToListAsync();

            return addresses;
        }

        public async Task<AddressDto> CreateAddressAsync(string maKhach, CreateAddressRequest request)
        {
            string targetMaKhach = !string.IsNullOrWhiteSpace(maKhach) ? maKhach.Trim() : (request.MaKhach?.Trim() ?? string.Empty);
            if (string.IsNullOrWhiteSpace(targetMaKhach))
            {
                throw new ValidationCustomException("Mã khách hàng không được để trống khi lưu địa chỉ nhận hàng.");
            }

            // Kiểm tra xem khách hàng đã có địa chỉ nào chưa
            int currentCount = await _context.DiaChiNhanHangs.CountAsync(a => a.MaKhach == targetMaKhach);
            bool isDefault = request.LaMacDinh || currentCount == 0;

            if (isDefault && currentCount > 0)
            {
                // Bỏ mặc định của các địa chỉ cũ
                var oldDefaults = await _context.DiaChiNhanHangs
                    .Where(a => a.MaKhach == targetMaKhach && a.LaMacDinh)
                    .ToListAsync();

                foreach (var old in oldDefaults)
                {
                    old.LaMacDinh = false;
                }
            }

            var entity = new DiaChiNhanHang
            {
                MaKhach = targetMaKhach,
                TenNguoiNhan = request.TenNguoiNhan.Trim(),
                SoDienThoai = request.SoDienThoai.Trim(),
                DiaChiChiTiet = request.DiaChiChiTiet.Trim(),
                LaMacDinh = isDefault,
                NgayTao = DateTime.Now
            };

            await _context.DiaChiNhanHangs.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new AddressDto
            {
                Id = entity.Id,
                MaKhach = entity.MaKhach,
                TenNguoiNhan = entity.TenNguoiNhan,
                SoDienThoai = entity.SoDienThoai,
                DiaChiChiTiet = entity.DiaChiChiTiet,
                LaMacDinh = entity.LaMacDinh,
                NgayTao = entity.NgayTao
            };
        }

        public async Task<AddressDto> UpdateAddressAsync(int id, string maKhach, UpdateAddressRequest request)
        {
            if (string.IsNullOrWhiteSpace(maKhach))
            {
                throw new ValidationCustomException("Mã khách hàng không hợp lệ.");
            }

            var entity = await _context.DiaChiNhanHangs
                .FirstOrDefaultAsync(a => a.Id == id && a.MaKhach == maKhach.Trim());

            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy địa chỉ nhận hàng với Id={id}");
            }

            if (request.LaMacDinh && !entity.LaMacDinh)
            {
                var oldDefaults = await _context.DiaChiNhanHangs
                    .Where(a => a.MaKhach == maKhach.Trim() && a.LaMacDinh && a.Id != id)
                    .ToListAsync();

                foreach (var old in oldDefaults)
                {
                    old.LaMacDinh = false;
                }
            }

            entity.TenNguoiNhan = request.TenNguoiNhan.Trim();
            entity.SoDienThoai = request.SoDienThoai.Trim();
            entity.DiaChiChiTiet = request.DiaChiChiTiet.Trim();
            entity.LaMacDinh = request.LaMacDinh;

            await _context.SaveChangesAsync();

            return new AddressDto
            {
                Id = entity.Id,
                MaKhach = entity.MaKhach,
                TenNguoiNhan = entity.TenNguoiNhan,
                SoDienThoai = entity.SoDienThoai,
                DiaChiChiTiet = entity.DiaChiChiTiet,
                LaMacDinh = entity.LaMacDinh,
                NgayTao = entity.NgayTao
            };
        }

        public async Task<bool> DeleteAddressAsync(int id, string maKhach)
        {
            if (string.IsNullOrWhiteSpace(maKhach))
            {
                throw new ValidationCustomException("Mã khách hàng không hợp lệ.");
            }

            var entity = await _context.DiaChiNhanHangs
                .FirstOrDefaultAsync(a => a.Id == id && a.MaKhach == maKhach.Trim());

            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy địa chỉ nhận hàng với Id={id}");
            }

            bool wasDefault = entity.LaMacDinh;
            _context.DiaChiNhanHangs.Remove(entity);
            await _context.SaveChangesAsync();

            // Nếu xóa địa chỉ mặc định, tự động gán địa chỉ mới nhất còn lại làm mặc định
            if (wasDefault)
            {
                var remaining = await _context.DiaChiNhanHangs
                    .Where(a => a.MaKhach == maKhach.Trim())
                    .OrderByDescending(a => a.NgayTao)
                    .FirstOrDefaultAsync();

                if (remaining != null)
                {
                    remaining.LaMacDinh = true;
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<AddressDto> SetDefaultAddressAsync(int id, string maKhach)
        {
            if (string.IsNullOrWhiteSpace(maKhach))
            {
                throw new ValidationCustomException("Mã khách hàng không hợp lệ.");
            }

            var entity = await _context.DiaChiNhanHangs
                .FirstOrDefaultAsync(a => a.Id == id && a.MaKhach == maKhach.Trim());

            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy địa chỉ nhận hàng với Id={id}");
            }

            var allAddresses = await _context.DiaChiNhanHangs
                .Where(a => a.MaKhach == maKhach.Trim())
                .ToListAsync();

            foreach (var a in allAddresses)
            {
                a.LaMacDinh = (a.Id == id);
            }

            await _context.SaveChangesAsync();

            return new AddressDto
            {
                Id = entity.Id,
                MaKhach = entity.MaKhach,
                TenNguoiNhan = entity.TenNguoiNhan,
                SoDienThoai = entity.SoDienThoai,
                DiaChiChiTiet = entity.DiaChiChiTiet,
                LaMacDinh = true,
                NgayTao = entity.NgayTao
            };
        }
    }
}
