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
    public class OrderService : IOrderService
    {
        private const string DEFAULT_PRODUCT_IMAGE = "https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?auto=format&fit=crop&w=800&q=80";

        private readonly IOrderRepository _orderRepository;
        private readonly IDanhMucHangHoaRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IAddressService _addressService;

        public OrderService(
            IOrderRepository orderRepository,
            IDanhMucHangHoaRepository productRepository,
            ICacheService cacheService,
            IAddressService addressService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _cacheService = cacheService;
            _addressService = addressService;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                throw new ValidationCustomException("Đơn hàng phải có ít nhất 1 sản phẩm gốm sứ");
            }

            string soHDB = await _orderRepository.GenerateSequentialOrderIdAsync();
            decimal tongTien = 0;
            var chiTietList = new List<ChiTietHoaDonBan>();
            var itemDtos = new List<OrderItemDto>();

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.MaHangHoa);
                if (product == null)
                {
                    throw new NotFoundException($"Không tìm thấy sản phẩm gốm với mã '{item.MaHangHoa}'");
                }

                if (product.SoLuong < item.SoLuong)
                {
                    throw new ValidationCustomException($"Sản phẩm '{product.TenHangHoa}' chỉ còn tồn {product.SoLuong} cái, không đủ để bán {item.SoLuong} cái");
                }

                decimal giamGiaTien = product.DonGiaBan * (item.GiamGia / 100m);
                decimal thanhTien = (product.DonGiaBan - giamGiaTien) * item.SoLuong;
                tongTien += thanhTien;

                chiTietList.Add(new ChiTietHoaDonBan
                {
                    SoHDB = soHDB,
                    MaHangHoa = item.MaHangHoa,
                    SoLuong = item.SoLuong,
                    GiamGia = item.GiamGia,
                    ThanhTien = thanhTien
                });

                itemDtos.Add(new OrderItemDto
                {
                    MaHangHoa = product.MaHangHoa,
                    TenHangHoa = product.TenHangHoa,
                    SoLuong = item.SoLuong,
                    DonGiaBan = product.DonGiaBan,
                    GiamGia = item.GiamGia,
                    ThanhTien = thanhTien,
                    HinhAnh = !string.IsNullOrEmpty(product.HinhAnh)
                        ? product.HinhAnh
                        : (product.Anh != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(product.Anh)}" : DEFAULT_PRODUCT_IMAGE)
                });
            }

            string maKhach = string.IsNullOrWhiteSpace(request.MaKhach) ? "KH001" : request.MaKhach;

            var hoaDon = new HoaDonBan
            {
                SoHDB = soHDB,
                MaNV = string.IsNullOrWhiteSpace(request.MaNV) ? "NV001" : request.MaNV,
                MaKhach = maKhach,
                NgayBan = DateTime.Now,
                TongTien = tongTien,
                TenNguoiNhan = !string.IsNullOrWhiteSpace(request.TenKhach) ? request.TenKhach.Trim() : null,
                SoDienThoaiNhan = !string.IsNullOrWhiteSpace(request.DienThoai) ? request.DienThoai.Trim() : null,
                DiaChiGiaoHang = !string.IsNullOrWhiteSpace(request.DiaChi) ? request.DiaChi.Trim() : null,
                GhiChu = !string.IsNullOrWhiteSpace(request.GhiChu) ? request.GhiChu.Trim() : null
            };

            await _orderRepository.AddAsync(hoaDon, chiTietList);

            // Tùy chọn lưu địa chỉ mới vào Sổ địa chỉ người dùng nếu được tích chọn
            if (request.LuuVaoSoDiaChi && !string.IsNullOrWhiteSpace(request.MaKhach) && !string.IsNullOrWhiteSpace(request.DiaChi))
            {
                try
                {
                    var existingAddresses = await _addressService.GetAddressesByCustomerAsync(request.MaKhach);
                    bool alreadyExists = existingAddresses.Any(a =>
                        a.DiaChiChiTiet.Trim().Equals(request.DiaChi.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (!alreadyExists)
                    {
                        await _addressService.CreateAddressAsync(request.MaKhach, new CreateAddressRequest
                        {
                            MaKhach = request.MaKhach,
                            TenNguoiNhan = !string.IsNullOrWhiteSpace(request.TenKhach) ? request.TenKhach.Trim() : "Khách hàng",
                            SoDienThoai = !string.IsNullOrWhiteSpace(request.DienThoai) ? request.DienThoai.Trim() : "",
                            DiaChiChiTiet = request.DiaChi.Trim(),
                            LaMacDinh = existingAddresses.Count == 0
                        });
                    }
                }
                catch
                {
                    // Không ngắt luồng nếu lưu sổ địa chỉ phụ gặp lỗi
                }
            }

            var resultDto = new OrderDto
            {
                SoHDB = soHDB,
                MaNV = hoaDon.MaNV,
                MaKhach = hoaDon.MaKhach,
                TenKhach = request.TenKhach,
                DienThoai = request.DienThoai,
                DiaChi = request.DiaChi,
                GhiChu = request.GhiChu,
                NgayBan = hoaDon.NgayBan,
                TongTien = tongTien,
                Items = itemDtos
            };

            // Cache chi tiết đơn hàng vừa tạo (TTL = 30 phút)
            string cacheKeyDetails = $"OrderDetails:{soHDB}";
            await _cacheService.SetAsync(cacheKeyDetails, resultDto, TimeSpan.FromMinutes(30));

            // Invalidate Cache lịch sử đơn hàng của khách hàng này để lần tra cứu sau cập nhật đơn mới
            if (!string.IsNullOrEmpty(request.MaKhach))
            {
                await _cacheService.RemoveByPrefixAsync($"OrderHistory:{request.MaKhach}");
            }
            await _cacheService.RemoveByPrefixAsync("OrderHistory:ALL");

            return resultDto;
        }

        public async Task<OrderDto> GetOrderByIdAsync(string soHDB)
        {
            if (string.IsNullOrWhiteSpace(soHDB))
            {
                throw new ArgumentException("Mã hóa đơn không được để trống");
            }

            // 1. Kiểm tra trong Cache/Redis (Tiền tố GomSuNhom5:OrderDetails:{soHDB})
            string cacheKey = $"OrderDetails:{soHDB.Trim()}";
            var cachedOrder = await _cacheService.GetAsync<OrderDto>(cacheKey);
            if (cachedOrder != null)
            {
                return cachedOrder;
            }

            // 2. Nếu chưa có trong Cache thì truy vấn SQL Database
            var entity = await _orderRepository.GetByIdAsync(soHDB);
            if (entity == null)
            {
                throw new NotFoundException($"Không tìm thấy hóa đơn bán mã: '{soHDB}'");
            }

            var dto = new OrderDto
            {
                SoHDB = entity.SoHDB,
                MaNV = entity.MaNV,
                TenNV = entity.NhanVien?.TenNV,
                MaKhach = entity.MaKhach,
                TenKhach = !string.IsNullOrEmpty(entity.TenNguoiNhan) ? entity.TenNguoiNhan : entity.KhachHang?.TenKhach,
                DienThoai = !string.IsNullOrEmpty(entity.SoDienThoaiNhan) ? entity.SoDienThoaiNhan : entity.KhachHang?.DienThoai,
                DiaChi = !string.IsNullOrEmpty(entity.DiaChiGiaoHang) ? entity.DiaChiGiaoHang : entity.KhachHang?.DiaChi,
                GhiChu = entity.GhiChu,
                NgayBan = entity.NgayBan,
                TongTien = entity.TongTien,
                Items = entity.ChiTietHoaDonBans?.Select(c => new OrderItemDto
                {
                    MaHangHoa = c.MaHangHoa,
                    TenHangHoa = c.HangHoa?.TenHangHoa ?? c.MaHangHoa,
                    SoLuong = c.SoLuong,
                    DonGiaBan = c.HangHoa?.DonGiaBan ?? (c.SoLuong > 0 ? c.ThanhTien / c.SoLuong : 0),
                    GiamGia = c.GiamGia,
                    ThanhTien = c.ThanhTien,
                    HinhAnh = !string.IsNullOrEmpty(c.HangHoa?.HinhAnh)
                        ? c.HangHoa.HinhAnh
                        : (c.HangHoa?.Anh != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(c.HangHoa.Anh)}" : DEFAULT_PRODUCT_IMAGE)
                }).ToList() ?? new List<OrderItemDto>()
            };

            // 3. Lưu vào Cache với TTL 30 phút
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30));

            return dto;
        }

        public async Task<PagedResultDto<OrderDto>> GetOrderHistoryAsync(string maKhach, int pageIndex = 1, int pageSize = 10)
        {
            string khachKey = string.IsNullOrWhiteSpace(maKhach) ? "ALL" : maKhach.Trim();
            string cacheKey = $"OrderHistory:{khachKey}:P{pageIndex}_S{pageSize}";

            // 1. Kiểm tra Cache trước
            var cachedHistory = await _cacheService.GetAsync<PagedResultDto<OrderDto>>(cacheKey);
            if (cachedHistory != null)
            {
                return cachedHistory;
            }

            // 2. Truy vấn SQL Database
            var (items, totalCount) = await _orderRepository.GetOrderHistoryAsync(maKhach, pageIndex, pageSize);

            var dtos = items.Select(entity => new OrderDto
            {
                SoHDB = entity.SoHDB,
                MaNV = entity.MaNV,
                TenNV = entity.NhanVien?.TenNV,
                MaKhach = entity.KhachHang?.MaKhach ?? entity.MaKhach,
                TenKhach = !string.IsNullOrEmpty(entity.TenNguoiNhan) ? entity.TenNguoiNhan : entity.KhachHang?.TenKhach,
                DienThoai = !string.IsNullOrEmpty(entity.SoDienThoaiNhan) ? entity.SoDienThoaiNhan : entity.KhachHang?.DienThoai,
                DiaChi = !string.IsNullOrEmpty(entity.DiaChiGiaoHang) ? entity.DiaChiGiaoHang : entity.KhachHang?.DiaChi,
                GhiChu = entity.GhiChu,
                NgayBan = entity.NgayBan,
                TongTien = entity.TongTien,
                Items = entity.ChiTietHoaDonBans?.Select(c => new OrderItemDto
                {
                    MaHangHoa = c.MaHangHoa,
                    TenHangHoa = c.HangHoa?.TenHangHoa ?? c.MaHangHoa,
                    SoLuong = c.SoLuong,
                    DonGiaBan = c.HangHoa?.DonGiaBan ?? (c.SoLuong > 0 ? c.ThanhTien / c.SoLuong : 0),
                    GiamGia = c.GiamGia,
                    ThanhTien = c.ThanhTien,
                    HinhAnh = !string.IsNullOrEmpty(c.HangHoa?.HinhAnh)
                        ? c.HangHoa.HinhAnh
                        : (c.HangHoa?.Anh != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(c.HangHoa.Anh)}" : DEFAULT_PRODUCT_IMAGE)
                }).ToList() ?? new List<OrderItemDto>()
            }).ToList();

            var result = new PagedResultDto<OrderDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            // 3. Lưu vào Cache 10 phút
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }
    }
}
