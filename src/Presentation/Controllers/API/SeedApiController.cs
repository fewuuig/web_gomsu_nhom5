using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanDoGomSu.Domain.Entities;
using QuanLyBanDoGomSu.Infrastructure.Persistence;

namespace QuanLyBanDoGomSu.Presentation.Controllers.API
{
    [ApiController]
    [Route("api/v1/seed")]
    public class SeedApiController : ControllerBase
    {
        private readonly QuanLyBanDoGomSuDbContext _context;

        public SeedApiController(QuanLyBanDoGomSuDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SeedDatabase()
        {
            try
            {
                // Remove existing dependent and master rows
                _context.ChiTietHoaDonBans.RemoveRange(_context.ChiTietHoaDonBans);
                _context.ChiTietHoaDonNhaps.RemoveRange(_context.ChiTietHoaDonNhaps);
                _context.DanhMucHangHoas.RemoveRange(_context.DanhMucHangHoas);
                _context.Loais.RemoveRange(_context.Loais);
                _context.LoaiMens.RemoveRange(_context.LoaiMens);
                _context.KichThuocs.RemoveRange(_context.KichThuocs);
                _context.MauSacs.RemoveRange(_context.MauSacs);
                _context.HinhKhois.RemoveRange(_context.HinhKhois);
                _context.NuocSanXuats.RemoveRange(_context.NuocSanXuats);
                _context.HoaVans.RemoveRange(_context.HoaVans);
                _context.CongDungs.RemoveRange(_context.CongDungs);

                await _context.SaveChangesAsync();

                // 1. Loai (Categories)
                var loais = new List<Loai>
                {
                    new Loai { MaLoai = "L01", TenLoai = "Ấm Trà & Bộ Trà" },
                    new Loai { MaLoai = "L02", TenLoai = "Bát Đĩa & Thố Cơm" },
                    new Loai { MaLoai = "L03", TenLoai = "Bình Hoa & Lọ Hoa" },
                    new Loai { MaLoai = "L04", TenLoai = "Đồ Thờ Cúng" },
                    new Loai { MaLoai = "L05", TenLoai = "Gốm Sứ Nghệ Thuật" },
                    new Loai { MaLoai = "L06", TenLoai = "Gốm Phong Thủy" }
                };
                _context.Loais.AddRange(loais);

                // 2. LoaiMen (Glazes)
                var loaiMens = new List<LoaiMen>
                {
                    new LoaiMen { MaLoaiMen = "LM01", TenLoaiMen = "Men Lam Cổ" },
                    new LoaiMen { MaLoaiMen = "LM02", TenLoaiMen = "Men Rạn Cổ" },
                    new LoaiMen { MaLoaiMen = "LM03", TenLoaiMen = "Men Ngọc Bích" },
                    new LoaiMen { MaLoaiMen = "LM04", TenLoaiMen = "Men Hỏa Biến" },
                    new LoaiMen { MaLoaiMen = "LM05", TenLoaiMen = "Men Trắng Ngà" }
                };
                _context.LoaiMens.AddRange(loaiMens);

                // 3. KichThuoc
                var kichThuocs = new List<KichThuoc>
                {
                    new KichThuoc { MaKichThuoc = "KT01", TenKichThuoc = "Nhỏ (S)" },
                    new KichThuoc { MaKichThuoc = "KT02", TenKichThuoc = "Vừa (M)" },
                    new KichThuoc { MaKichThuoc = "KT03", TenKichThuoc = "Lớn (L)" },
                    new KichThuoc { MaKichThuoc = "KT04", TenKichThuoc = "Bộ Tiêu Chuẩn" }
                };
                _context.KichThuocs.AddRange(kichThuocs);

                // 4. MauSac
                var mauSacs = new List<MauSac>
                {
                    new MauSac { MaMauSac = "MS01", TenMauSac = "Xanh Men Lam" },
                    new MauSac { MaMauSac = "MS02", TenMauSac = "Nâu Đất Rạn" },
                    new MauSac { MaMauSac = "MS03", TenMauSac = "Xanh Ngọc Bích" },
                    new MauSac { MaMauSac = "MS04", TenMauSac = "Hỏa Biến Đa Sắc" }
                };
                _context.MauSacs.AddRange(mauSacs);

                // 5. HinhKhoi
                var hinhKhois = new List<HinhKhoi>
                {
                    new HinhKhoi { MaHinhKhoi = "HK01", TenHinhKhoi = "Hình Trụ Bầu" },
                    new HinhKhoi { MaHinhKhoi = "HK02", TenHinhKhoi = "Hình Tròn Tráp" },
                    new HinhKhoi { MaHinhKhoi = "HK03", TenHinhKhoi = "Dáng Giọt Nước" },
                    new HinhKhoi { MaHinhKhoi = "HK04", TenHinhKhoi = "Bộ 6-10 Món" }
                };
                _context.HinhKhois.AddRange(hinhKhois);

                // 6. NuocSanXuat
                var nuocSXs = new List<NuocSanXuat>
                {
                    new NuocSanXuat { MaNuocSX = "VN", TenNuocSX = "Việt Nam (Bát Tràng)" }
                };
                _context.NuocSanXuats.AddRange(nuocSXs);

                // 7. HoaVan
                var hoaVans = new List<HoaVan>
                {
                    new HoaVan { MaHoaVan = "HV01", TenHoaVan = "Rồng Phượng Dát Vàng" },
                    new HoaVan { MaHoaVan = "HV02", TenHoaVan = "Hoa Sen Bách Nhiên" },
                    new HoaVan { MaHoaVan = "HV03", TenHoaVan = "Tùng Cúc Trúc Mai" },
                    new HoaVan { MaHoaVan = "HV04", TenHoaVan = "Sơn Thủy Hữu Tình" }
                };
                _context.HoaVans.AddRange(hoaVans);

                // 8. CongDung
                var congDungs = new List<CongDung>
                {
                    new CongDung { MaCongDung = "CD01", TenCongDung = "Thưởng Trà" },
                    new CongDung { MaCongDung = "CD02", TenCongDung = "Khang Gia Đạo" },
                    new CongDung { MaCongDung = "CD03", TenCongDung = "Trang Trí Nội Thất" },
                    new CongDung { MaCongDung = "CD04", TenCongDung = "Thờ Cúng Tâm Linh" }
                };
                _context.CongDungs.AddRange(congDungs);

                await _context.SaveChangesAsync();

                // 9. DanhMucHangHoa (20 Products)
                var products = new List<DanhMucHangHoa>
                {
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH001",
                        TenHangHoa = "Bộ Ấm Trà Tử Sa Bát Tràng Dát Vàng Họa Tiết Hoa Sen",
                        MaLoai = "L01",
                        MaLoaiMen = "LM01",
                        MaKichThuoc = "KT04",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK04",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV02",
                        MaCongDung = "CD01",
                        SoLuong = 45,
                        DonGiaNhap = 1800000,
                        DonGiaBan = 2720000,
                        HinhAnh = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Bộ ấm trà tử sa thủ công 6 chén 1 ấm nung lò 1.300 độ C cao cấp."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH002",
                        TenHangHoa = "Bộ Ấm Trà Tử Sa Men Lam Rồng Phượng Bát Tràng",
                        MaLoai = "L01",
                        MaLoaiMen = "LM01",
                        MaKichThuoc = "KT04",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK04",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV01",
                        MaCongDung = "CD01",
                        SoLuong = 30,
                        DonGiaNhap = 2100000,
                        DonGiaBan = 3150000,
                        HinhAnh = "https://images.unsplash.com/photo-1594631252845-29fc4cc8cde9?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Chất gốm tử sa giữ nhiệt hoàn hảo, hoa văn rồng phượng tinh xảo."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH003",
                        TenHangHoa = "Lộc Bình Gốm Sứ Men Rạn Dát Vàng Thuận Buồm Xuôi Gió",
                        MaLoai = "L06",
                        MaLoaiMen = "LM02",
                        MaKichThuoc = "KT03",
                        MaMauSac = "MS02",
                        MaHinhKhoi = "HK01",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV04",
                        MaCongDung = "CD02",
                        SoLuong = 12,
                        DonGiaNhap = 8500000,
                        DonGiaBan = 12800000,
                        HinhAnh = "https://images.unsplash.com/photo-1610701596007-11502861dcfa?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Cặp lộc bình cao 1m6 phong thủy chiêu tài lộc gia đạo."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH004",
                        TenHangHoa = "Bình Hoa Gốm Sứ Men Hỏa Biến Dáng Giọt Nước",
                        MaLoai = "L03",
                        MaLoaiMen = "LM04",
                        MaKichThuoc = "KT02",
                        MaMauSac = "MS04",
                        MaHinhKhoi = "HK03",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV03",
                        MaCongDung = "CD03",
                        SoLuong = 50,
                        DonGiaNhap = 650000,
                        DonGiaBan = 980000,
                        HinhAnh = "https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Men hỏa biến tự nhiên mỗi sản phẩm là một bản thể độc bản."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH005",
                        TenHangHoa = "Bộ Bát Đĩa Gốm Sứ Men Ngọc 22 Món Gia Đình",
                        MaLoai = "L02",
                        MaLoaiMen = "LM03",
                        MaKichThuoc = "KT04",
                        MaMauSac = "MS03",
                        MaHinhKhoi = "HK04",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV02",
                        MaCongDung = "CD02",
                        SoLuong = 25,
                        DonGiaNhap = 1900000,
                        DonGiaBan = 2850000,
                        HinhAnh = "https://images.unsplash.com/photo-1610701596007-11502861dcfa?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Bộ bàn ăn cao cấp chống xước, an toàn với lò vi sóng."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH006",
                        TenHangHoa = "Bát Đồ Thờ Cúng Men Lam Cổ Họa Tiết Rồng Chầu Mặt Nguyệt",
                        MaLoai = "L04",
                        MaLoaiMen = "LM01",
                        MaKichThuoc = "KT02",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK02",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV01",
                        MaCongDung = "CD04",
                        SoLuong = 60,
                        DonGiaNhap = 450000,
                        DonGiaBan = 680000,
                        HinhAnh = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Đồ thờ Bát Tràng tâm linh chuẩn phong thủy truyền thống."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH007",
                        TenHangHoa = "Tượng Gốm Sứ Di Lặc Ngồi Bao Tiền Men Rạn Cổ",
                        MaLoai = "L05",
                        MaLoaiMen = "LM02",
                        MaKichThuoc = "KT03",
                        MaMauSac = "MS02",
                        MaHinhKhoi = "HK01",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV01",
                        MaCongDung = "CD02",
                        SoLuong = 15,
                        DonGiaNhap = 3200000,
                        DonGiaBan = 4800000,
                        HinhAnh = "https://images.unsplash.com/photo-1594631252845-29fc4cc8cde9?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Tượng Phật Di Lặc mang lại an lạc thịnh vượng cho gia chủ."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH008",
                        TenHangHoa = "Bộ Trà Men Trắng Ngà Vẽ Tay Tùng Cúc Trúc Mai",
                        MaLoai = "L01",
                        MaLoaiMen = "LM05",
                        MaKichThuoc = "KT04",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK04",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV03",
                        MaCongDung = "CD01",
                        SoLuong = 40,
                        DonGiaNhap = 1200000,
                        DonGiaBan = 1850000,
                        HinhAnh = "https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Họa tiết Tứ Quý vẽ tay thanh nhã, chất sứ hoàng gia."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH009",
                        TenHangHoa = "Lọ Hoa Men Ngọc Bích Khắc Nổi Sen Hồng Bát Tràng",
                        MaLoai = "L03",
                        MaLoaiMen = "LM03",
                        MaKichThuoc = "KT02",
                        MaMauSac = "MS03",
                        MaHinhKhoi = "HK03",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV02",
                        MaCongDung = "CD03",
                        SoLuong = 35,
                        DonGiaNhap = 800000,
                        DonGiaBan = 1250000,
                        HinhAnh = "https://images.unsplash.com/photo-1610701596007-11502861dcfa?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Lọ hoa cắm hoa phòng khách sang trọng, đường nét mướt mịn."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH010",
                        TenHangHoa = "Đèn Thờ Gốm Sứ Men Lam Vẽ Tay Họa Tiết Mây Rồng",
                        MaLoai = "L04",
                        MaLoaiMen = "LM01",
                        MaKichThuoc = "KT02",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK01",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV01",
                        MaCongDung = "CD04",
                        SoLuong = 28,
                        DonGiaNhap = 950000,
                        DonGiaBan = 1450000,
                        HinhAnh = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Đèn dầu thờ cúng trang nghiêm cho gian thờ gia tiên."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH011",
                        TenHangHoa = "Mai Bình Tích Lộc Men Hỏa Biến Đa Sắc Bát Tràng",
                        MaLoai = "L06",
                        MaLoaiMen = "LM04",
                        MaKichThuoc = "KT03",
                        MaMauSac = "MS04",
                        MaHinhKhoi = "HK01",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV04",
                        MaCongDung = "CD02",
                        SoLuong = 18,
                        DonGiaNhap = 4200000,
                        DonGiaBan = 6300000,
                        HinhAnh = "https://images.unsplash.com/photo-1594631252845-29fc4cc8cde9?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Mai bình tích lộc chiêu tài đón vượng khí cho doanh nghiệp."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH012",
                        TenHangHoa = "Bộ Bát Cơm Gốm Sứ Men Rạn Cổ 10 Bát Chuẩn Bát Tràng",
                        MaLoai = "L02",
                        MaLoaiMen = "LM02",
                        MaKichThuoc = "KT01",
                        MaMauSac = "MS02",
                        MaHinhKhoi = "HK02",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV02",
                        MaCongDung = "CD02",
                        SoLuong = 70,
                        DonGiaNhap = 500000,
                        DonGiaBan = 750000,
                        HinhAnh = "https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Bát ăn cơm đầm tay, hoa văn hoa sen thanh thoát."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH013",
                        TenHangHoa = "Tranh Gốm Sứ Bát Tràng Họa Tiết Sơn Thủy Hữu Tình Dát Vàng",
                        MaLoai = "L05",
                        MaLoaiMen = "LM01",
                        MaKichThuoc = "KT03",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK01",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV04",
                        MaCongDung = "CD03",
                        SoLuong = 10,
                        DonGiaNhap = 6800000,
                        DonGiaBan = 9900000,
                        HinhAnh = "https://images.unsplash.com/photo-1610701596007-11502861dcfa?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Tranh treo tường nghệ thuật phong thủy đắp nổi thủ công."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH014",
                        TenHangHoa = "Bộ Ấm Trà Men Hỏa Biến Dáng Tây Hồ Cao Cấp",
                        MaLoai = "L01",
                        MaLoaiMen = "LM04",
                        MaKichThuoc = "KT04",
                        MaMauSac = "MS04",
                        MaHinhKhoi = "HK04",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV04",
                        MaCongDung = "CD01",
                        SoLuong = 32,
                        DonGiaNhap = 1600000,
                        DonGiaBan = 2400000,
                        HinhAnh = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Thiết kế cách tân hiện đại, sắc men chuyển hóa kỳ ảo."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH015",
                        TenHangHoa = "Thố Cơm Gốm Sứ Men Lam Cổ Có Nắp Đậy Bát Tràng",
                        MaLoai = "L02",
                        MaLoaiMen = "LM01",
                        MaKichThuoc = "KT02",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK02",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV02",
                        MaCongDung = "CD02",
                        SoLuong = 40,
                        DonGiaNhap = 380000,
                        DonGiaBan = 580000,
                        HinhAnh = "https://images.unsplash.com/photo-1594631252845-29fc4cc8cde9?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Thố đựng cơm giữ nhiệt cực tốt, hoa văn trang nhã."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH016",
                        TenHangHoa = "Mâm Bồng Thờ Cúng Gốm Sứ Men Rạn Họa Tiết Lưỡng Long",
                        MaLoai = "L04",
                        MaLoaiMen = "LM02",
                        MaKichThuoc = "KT02",
                        MaMauSac = "MS02",
                        MaHinhKhoi = "HK02",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV01",
                        MaCongDung = "CD04",
                        SoLuong = 35,
                        DonGiaNhap = 600000,
                        DonGiaBan = 890000,
                        HinhAnh = "https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Mâm bồng bày ngũ quả dâng kính gia tiên."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH017",
                        TenHangHoa = "Bình Hút Tài Lộc Men Ngà Vẽ Vàng Tùng Hạc Diên Niên",
                        MaLoai = "L06",
                        MaLoaiMen = "LM05",
                        MaKichThuoc = "KT03",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK01",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV03",
                        MaCongDung = "CD02",
                        SoLuong = 14,
                        DonGiaNhap = 5200000,
                        DonGiaBan = 7800000,
                        HinhAnh = "https://images.unsplash.com/photo-1610701596007-11502861dcfa?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Bình phong thủy miệng rộng thân phình giữ tài lộc lâu bền."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH018",
                        TenHangHoa = "Bộ Đĩa Hoa Mai Men Ngọc Bích 6 Cánh Cao Cấp",
                        MaLoai = "L02",
                        MaLoaiMen = "LM03",
                        MaKichThuoc = "KT04",
                        MaMauSac = "MS03",
                        MaHinhKhoi = "HK04",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV02",
                        MaCongDung = "CD02",
                        SoLuong = 22,
                        DonGiaNhap = 1400000,
                        DonGiaBan = 2100000,
                        HinhAnh = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Bộ đĩa tạo hình bông hoa mai độc đáo cho mâm cỗ Tết."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH019",
                        TenHangHoa = "Tượng Cá Chép Vượt Vũ Môn Men Hỏa Biến Bát Tràng",
                        MaLoai = "L05",
                        MaLoaiMen = "LM04",
                        MaKichThuoc = "KT03",
                        MaMauSac = "MS04",
                        MaHinhKhoi = "HK01",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV04",
                        MaCongDung = "CD03",
                        SoLuong = 16,
                        DonGiaNhap = 2800000,
                        DonGiaBan = 4200000,
                        HinhAnh = "https://images.unsplash.com/photo-1594631252845-29fc4cc8cde9?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Biểu tượng thăng tiến công danh sự nghiệp thịnh vượng."
                    },
                    new DanhMucHangHoa
                    {
                        MaHangHoa = "HH020",
                        TenHangHoa = "Bộ Ấm Trà Quai Đồng Gốm Sứ Men Lam Cổ",
                        MaLoai = "L01",
                        MaLoaiMen = "LM01",
                        MaKichThuoc = "KT04",
                        MaMauSac = "MS01",
                        MaHinhKhoi = "HK04",
                        MaNuocSX = "VN",
                        MaHoaVan = "HV03",
                        MaCongDung = "CD01",
                        SoLuong = 50,
                        DonGiaNhap = 980000,
                        DonGiaBan = 1490000,
                        HinhAnh = "https://images.unsplash.com/photo-1578749556568-bc2c40e68b61?auto=format&fit=crop&w=800&q=80",
                        GhiChu = "Quai đồng sang trọng phong cách trà đạo Bát Tràng."
                    }
                };

                _context.DanhMucHangHoas.AddRange(products);
                await _context.SaveChangesAsync();

                // Seed default test accounts (Admin & User)
                await SeedDefaultAccountsInternal();

                return Ok(new
                {
                    Status = 200,
                    Message = "Khởi tạo 20 sản phẩm và tài khoản Test (Admin: admin/admin123, User: user/user123) thành công!",
                    TotalProducts = products.Count,
                    TotalCategories = loais.Count
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Status = 500, Message = ex.Message, Detail = ex.InnerException?.Message });
            }
        }

        [HttpPost("accounts")]
        public async Task<IActionResult> SeedAccounts()
        {
            try
            {
                await SeedDefaultAccountsInternal();
                return Ok(new
                {
                    Status = 200,
                    Message = "Khởi tạo tài khoản Test thành công! Admin (admin / admin123), User (user / user123)"
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Status = 500, Message = ex.Message });
            }
        }

        private async Task SeedDefaultAccountsInternal()
        {
            // 1. Ensure Roles (Quyen) exist
            var roles = new List<Quyen>
            {
                new Quyen { MaQuyen = "ADMIN", TenQuyen = "Quản Trị Viên" },
                new Quyen { MaQuyen = "KH", TenQuyen = "Khách Hàng" },
                new Quyen { MaQuyen = "NV", TenQuyen = "Nhân Viên" },
                new Quyen { MaQuyen = "QUANLY", TenQuyen = "Quản Lý" }
            };

            foreach (var role in roles)
            {
                if (!await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_context.Quyens, q => q.MaQuyen == role.MaQuyen))
                {
                    _context.Quyens.Add(role);
                }
            }
            await _context.SaveChangesAsync();

            // 2. Ensure Job (CongViec) for Admin NhanVien
            var congViec = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.CongViecs, c => c.MaCV == "CV01");
            if (congViec == null)
            {
                congViec = new CongViec { MaCV = "CV01", TenCV = "Quản Lý Hệ Thống", MucLuong = 20000000 };
                _context.CongViecs.Add(congViec);
                await _context.SaveChangesAsync();
            }

            // 3. Ensure Admin Employee (NhanVien)
            var nhanVienAdmin = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.NhanViens, n => n.MaNV == "NVADMIN");
            if (nhanVienAdmin == null)
            {
                nhanVienAdmin = new NhanVien
                {
                    MaNV = "NVADMIN",
                    TenNV = "Quản Trị Viên Bát Tràng",
                    GioiTinh = "Nam",
                    DienThoai = "0988888888",
                    DiaChi = "Làng Gốm Bát Tràng, Gia Lâm, Hà Nội",
                    MaCV = "CV01"
                };
                _context.NhanViens.Add(nhanVienAdmin);
                await _context.SaveChangesAsync();
            }

            // 4. Ensure Customer (KhachHang) for User Account
            var khachHangUser = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.KhachHangs, k => k.MaKhach == "KHUSER");
            if (khachHangUser == null)
            {
                khachHangUser = new KhachHang
                {
                    MaKhach = "KHUSER",
                    TenKhach = "Nguyễn Văn An (Khách Hàng VIP)",
                    DienThoai = "0977777777",
                    DiaChi = "Số 88 Phố Cổ, Hoàn Kiếm, Hà Nội"
                };
                _context.KhachHangs.Add(khachHangUser);
                await _context.SaveChangesAsync();
            }

            // 5. Seed ADMIN Account (admin / admin123)
            var adminAccount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.TaiKhoans, t => t.TenDangNhap == "admin");
            byte[] adminPasswordHash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("admin123"));
            if (adminAccount == null)
            {
                adminAccount = new TaiKhoan
                {
                    MaTK = "TKADMIN",
                    TenDangNhap = "admin",
                    MatKhau = adminPasswordHash,
                    Email = "admin@battrangatelier.vn",
                    MaNV = "NVADMIN",
                    MaQuyen = "ADMIN",
                    TrangThai = true,
                    LanDangNhapCuoi = DateTime.Now
                };
                _context.TaiKhoans.Add(adminAccount);
            }
            else
            {
                adminAccount.MatKhau = adminPasswordHash;
                adminAccount.TrangThai = true;
                adminAccount.MaQuyen = "ADMIN";
            }

            // 6. Seed USER Account (user / user123)
            var userAccount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.TaiKhoans, t => t.TenDangNhap == "user");
            byte[] userPasswordHash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("user123"));
            if (userAccount == null)
            {
                userAccount = new TaiKhoan
                {
                    MaTK = "TKUSER",
                    TenDangNhap = "user",
                    MatKhau = userPasswordHash,
                    Email = "user@battrangatelier.vn",
                    MaKhach = "KHUSER",
                    MaQuyen = "KH",
                    TrangThai = true,
                    LanDangNhapCuoi = DateTime.Now
                };
                _context.TaiKhoans.Add(userAccount);
            }
            else
            {
                userAccount.MatKhau = userPasswordHash;
                userAccount.TrangThai = true;
                userAccount.MaQuyen = "KH";
            }

            await _context.SaveChangesAsync();
        }
    }
}
