using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Application.Port.Out;
using QuanLyBanDoGomSu.Domain.Entities;
using QuanLyBanDoGomSu.Exception;

namespace QuanLyBanDoGomSu.Application.Service
{
    public class DanhMucHangHoaService : IDanhMucHangHoaService
    {
        private readonly IDanhMucHangHoaRepository _repository;
        private readonly ICacheService _cacheService;

        public DanhMucHangHoaService(IDanhMucHangHoaRepository repository, ICacheService cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task<DanhMucHangHoaDto> GetByIdAsync(string maHangHoa)
        {
            if (string.IsNullOrWhiteSpace(maHangHoa))
            {
                throw new ArgumentException("Mã hàng hóa không được để trống");
            }

            // 1. Check Cache first (GomSuNhom5:Product:{maHangHoa})
            string cacheKey = $"Product:{maHangHoa.Trim()}";
            var cachedProduct = await _cacheService.GetAsync<DanhMucHangHoaDto>(cacheKey);
            if (cachedProduct != null)
            {
                return cachedProduct;
            }

            // 2. Query DB
            var entity = await _repository.GetByIdAsync(maHangHoa);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy hàng hóa đồ gốm với mã: '{maHangHoa}'");
            }

            var dto = MapToDto(entity);

            // 3. Cache product info for 60 minutes (hiển thị thông tin hàng hóa ít thay đổi)
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(60));

            return dto;
        }

        public async Task<PagedResultDto<DanhMucHangHoaDto>> FilterAsync(ProductFilterRequest request)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(
                request.Keyword, request.MaLoai, request.MaKichThuoc, request.MaLoaiMen, request.MaMauSac,
                request.MinPrice, request.MaxPrice, request.PageIndex, request.PageSize);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResultDto<DanhMucHangHoaDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public async Task<DanhMucHangHoaDto> CreateAsync(DanhMucHangHoaDto dto)
        {
            if (await _repository.ExistsAsync(dto.MaHangHoa))
            {
                throw new ValidationCustomException($"Mã hàng hóa '{dto.MaHangHoa}' đã tồn tại trong hệ thống");
            }

            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);

            var resultDto = MapToDto(entity);

            // Cache new product
            string cacheKey = $"Product:{dto.MaHangHoa.Trim()}";
            await _cacheService.SetAsync(cacheKey, resultDto, TimeSpan.FromMinutes(60));

            return resultDto;
        }

        public async Task<DanhMucHangHoaDto> UpdateAsync(string maHangHoa, DanhMucHangHoaDto dto)
        {
            var existing = await _repository.GetByIdAsync(maHangHoa);
            if (existing == null)
            {
                throw new NotFoundException($"Không tìm thấy hàng hóa đồ gốm với mã: '{maHangHoa}' để cập nhật");
            }

            existing.TenHangHoa = dto.TenHangHoa;
            existing.MaLoai = dto.MaLoai;
            existing.MaKichThuoc = dto.MaKichThuoc;
            existing.MaCongDung = dto.MaCongDung;
            existing.MaLoaiMen = dto.MaLoaiMen;
            existing.MaHinhKhoi = dto.MaHinhKhoi;
            existing.MaMauSac = dto.MaMauSac;
            existing.MaNuocSX = dto.MaNuocSX;
            existing.MaHoaVan = dto.MaHoaVan;
            existing.SoLuong = dto.SoLuong;
            existing.DonGiaNhap = dto.DonGiaNhap;
            existing.DonGiaBan = dto.DonGiaBan;
            existing.ThoiGianBaoHanh = dto.ThoiGianBaoHanh;
            existing.GhiChu = dto.GhiChu;
            existing.HinhAnh = dto.HinhAnh;

            if (!string.IsNullOrEmpty(dto.AnhBase64))
            {
                existing.Anh = Convert.FromBase64String(dto.AnhBase64);
            }

            await _repository.UpdateAsync(existing);
            var updatedDto = MapToDto(existing);

            // Invalidate/Update product cache
            string cacheKey = $"Product:{maHangHoa.Trim()}";
            await _cacheService.RemoveAsync(cacheKey);
            await _cacheService.SetAsync(cacheKey, updatedDto, TimeSpan.FromMinutes(60));

            return updatedDto;
        }

        public async Task DeleteAsync(string maHangHoa)
        {
            if (!await _repository.ExistsAsync(maHangHoa))
            {
                throw new NotFoundException($"Không tìm thấy hàng hóa với mã '{maHangHoa}' để xóa");
            }

            await _repository.DeleteAsync(maHangHoa);

            // Invalidate cache
            string cacheKey = $"Product:{maHangHoa.Trim()}";
            await _cacheService.RemoveAsync(cacheKey);
        }

        private static DanhMucHangHoaDto MapToDto(DanhMucHangHoa entity)
        {
            return new DanhMucHangHoaDto
            {
                MaHangHoa = entity.MaHangHoa,
                TenHangHoa = entity.TenHangHoa,
                MaLoai = entity.MaLoai,
                TenLoai = entity.Loai?.TenLoai,
                MaKichThuoc = entity.MaKichThuoc,
                TenKichThuoc = entity.KichThuoc?.TenKichThuoc,
                MaCongDung = entity.MaCongDung,
                TenCongDung = entity.CongDung?.TenCongDung,
                MaLoaiMen = entity.MaLoaiMen,
                TenLoaiMen = entity.LoaiMen?.TenLoaiMen,
                MaHinhKhoi = entity.MaHinhKhoi,
                TenHinhKhoi = entity.HinhKhoi?.TenHinhKhoi,
                MaMauSac = entity.MaMauSac,
                TenMauSac = entity.MauSac?.TenMauSac,
                MaNuocSX = entity.MaNuocSX,
                TenNuocSX = entity.NuocSanXuat?.TenNuocSX,
                MaHoaVan = entity.MaHoaVan,
                TenHoaVan = entity.HoaVan?.TenHoaVan,
                SoLuong = entity.SoLuong,
                DonGiaNhap = entity.DonGiaNhap,
                DonGiaBan = entity.DonGiaBan,
                ThoiGianBaoHanh = entity.ThoiGianBaoHanh,
                GhiChu = entity.GhiChu,
                HinhAnh = entity.HinhAnh,
                AnhBase64 = entity.Anh != null ? Convert.ToBase64String(entity.Anh) : null
            };
        }

        private static DanhMucHangHoa MapToEntity(DanhMucHangHoaDto dto)
        {
            return new DanhMucHangHoa
            {
                MaHangHoa = dto.MaHangHoa,
                TenHangHoa = dto.TenHangHoa,
                MaLoai = dto.MaLoai,
                MaKichThuoc = dto.MaKichThuoc,
                MaCongDung = dto.MaCongDung,
                MaLoaiMen = dto.MaLoaiMen,
                MaHinhKhoi = dto.MaHinhKhoi,
                MaMauSac = dto.MaMauSac,
                MaNuocSX = dto.MaNuocSX,
                MaHoaVan = dto.MaHoaVan,
                SoLuong = dto.SoLuong,
                DonGiaNhap = dto.DonGiaNhap,
                DonGiaBan = dto.DonGiaBan,
                ThoiGianBaoHanh = dto.ThoiGianBaoHanh,
                GhiChu = dto.GhiChu,
                HinhAnh = dto.HinhAnh,
                Anh = !string.IsNullOrEmpty(dto.AnhBase64) ? Convert.FromBase64String(dto.AnhBase64) : null
            };
        }
    }
}
