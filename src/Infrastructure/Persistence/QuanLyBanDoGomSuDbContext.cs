using Microsoft.EntityFrameworkCore;
using QuanLyBanDoGomSu.Domain.Entities;

namespace QuanLyBanDoGomSu.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core DbContext đại diện cho CSDL QuanLyBanDoGomSu.
    /// Cấu hình ddl-auto=none: Không gọi Database.EnsureCreated() hay Database.Migrate() tự động.
    /// Schema được quản lý 100% bằng script Flyway/DbUp SQL.
    /// </summary>
    public class QuanLyBanDoGomSuDbContext : DbContext
    {
        public QuanLyBanDoGomSuDbContext(DbContextOptions<QuanLyBanDoGomSuDbContext> options)
            : base(options)
        {
        }

        public DbSet<KichThuoc> KichThuocs { get; set; }
        public DbSet<Loai> Loais { get; set; }
        public DbSet<CongDung> CongDungs { get; set; }
        public DbSet<LoaiMen> LoaiMens { get; set; }
        public DbSet<NuocSanXuat> NuocSanXuats { get; set; }
        public DbSet<MauSac> MauSacs { get; set; }
        public DbSet<HinhKhoi> HinhKhois { get; set; }
        public DbSet<HoaVan> HoaVans { get; set; }
        public DbSet<CongViec> CongViecs { get; set; }
        public DbSet<DanhMucHangHoa> DanhMucHangHoas { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<HoaDonNhap> HoaDonNhaps { get; set; }
        public DbSet<ChiTietHoaDonNhap> ChiTietHoaDonNhaps { get; set; }
        public DbSet<HoaDonBan> HoaDonBans { get; set; }
        public DbSet<ChiTietHoaDonBan> ChiTietHoaDonBans { get; set; }
        public DbSet<Quyen> Quyens { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<TinNhanChat> TinNhanChats { get; set; }
        public DbSet<MaGiamGia> MaGiamGias { get; set; }
        public DbSet<KhachHang_MaGiamGia> KhachHang_MaGiamGias { get; set; }
        public DbSet<DiaChiNhanHang> DiaChiNhanHangs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Khóa chính phức hợp cho ChiTietHoaDonBan (SoHDB, MaHangHoa)
            modelBuilder.Entity<ChiTietHoaDonBan>()
                .HasKey(c => new { c.SoHDB, c.MaHangHoa });

            // Cấu hình Khóa chính phức hợp cho ChiTietHoaDonNhap (SoHDN, MaHangHoa)
            modelBuilder.Entity<ChiTietHoaDonNhap>()
                .HasKey(c => new { c.SoHDN, c.MaHangHoa });

            // Cấu hình Khóa chính phức hợp cho KhachHang_MaGiamGia (MaKhach, MaGiamGia)
            modelBuilder.Entity<KhachHang_MaGiamGia>()
                .HasKey(k => new { k.MaKhach, k.MaGiamGia });

            // Cấu hình Quan hệ TaiKhoan - NhanVien / KhachHang
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.NhanVien)
                .WithMany()
                .HasForeignKey(t => t.MaNV)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.KhachHang)
                .WithMany()
                .HasForeignKey(t => t.MaKhach)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TinNhanChat>()
                .HasIndex(t => new { t.SessionId, t.Id });
        }
    }
}
