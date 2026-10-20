# 🏺 Backend RESTful Web API - Hệ Thống Quản Lý Bán Đồ Gốm Sứ

![.NET Version](https://img.shields.io/badge/.NET-8.0%2F9.0-512BD4?logo=dotnet)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture%20%2F%20Ports%20%26%20Adapters-blue)
![Database](https://img.shields.io/badge/Database-SQL%20Server%202022-CC292B?logo=microsoftsqlserver)
![Redis Cache](https://img.shields.io/badge/Redis%20Seam-localhost%3A6379-DC382D?logo=redis)
![API Standard](https://img.shields.io/badge/API-ApiResponse%3CT%3E%20Standard-orange)

Dự án Backend RESTful Web API chuyên trách xử lý toàn bộ logic nghiệp vụ, quản lý cơ sở dữ liệu gốm sứ và cung cấp Endpoints JSON tiêu chuẩn cho ứng dụng Frontend ReactJS SPA ([`d:\Fe_gomsu_nhom5`](file:///d:/Fe_gomsu_nhom5)).

---

## 🚀 1. Lộ Trình Phát Triển Tính Năng (3 Giai Đoạn)

### 🟢 Giai Đoạn 1: Core E-Commerce & Management (Hoàn Thành)
- [x] **Full-Text Search & Filter API**: Tìm kiếm sản phẩm theo tên (Full-text match) & Lọc đa tiêu chí (Loại, Men, Kích thước, Màu sắc, Giá).
- [x] **Essential Fields DTOs**: Tối ưu Payload API chỉ trả về các trường dữ liệu thực sự cần thiết cho UI.
- [x] **Product Catalog & Detail**: API lấy danh sách và chi tiết mặt hàng gốm sứ.
- [x] **Admin Product CRUD**: Thêm mới (`POST`), Cập nhật (`PUT`), Xóa (`DELETE`) sản phẩm đồ gốm sứ.

### 🟡 Giai Đoạn 2: Giỏ Hàng, Đơn Hàng & Chat Tư Vấn (Hoàn Thành)
- [x] **Giỏ hàng (Cart)**: Quản lý giỏ hàng `GioHang` (DTOs & Service API).
- [x] **Đặt hàng & Lịch sử đơn hàng**: Xử lý tạo đơn `HoaDonBan` (Sequential Auto ID `HDB...`) và tra cứu trạng thái đơn hàng đã đặt.
- [x] **Khung Chat Tư Vấn Trực Tuyến**: Customer Floating Chat Widget & Admin Live Support Chat Panel (`/api/v1/chat`).

### 🔵 Giai Đoạn 3: Dashboard Doanh Số & Redis Cache Seam (Hoàn Thành)
- [x] **Dashboard Thống Kê**: Báo cáo doanh số theo ngày/tháng, top sản phẩm bán chạy (`GET /api/v1/thong-ke/doanh-so`).
- [x] **Redis Caching Seam**: Cổng `ICacheService` kết nối Redis tại `localhost:6379` với tiền tố bắt buộc `GomSuNhom5:` (In-Memory fallback hiện tại).

---

## ⚡ 2. Cấu Hình Redis Cache Seam (`localhost:6379`) & Key Prefix

Hệ thống đã thiết kế cổng trừu tượng **Cache Seam** (`ICacheService`) ở tầng `Application`:
- **Redis Connection Endpoint**: `localhost:6379`
- **Tên Tiền Tố Key (Key Prefix)**: `GomSuNhom5:` (Ví dụ: `GomSuNhom5:Product:HH001`, `GomSuNhom5:Categories`) nhằm đảm bảo **100% không bị trùng lặp key** với các dự án khác trên cùng Redis Server.
- **Chế độ hiện tại**: Tự động dùng `InMemoryCacheService` (.NET `IMemoryCache`) để phát triển không phụ thuộc môi trường.
- **Khi kích hoạt Redis**: Đổi `RedisConfig:Enabled = true` trong [`appsettings.json`](file:///d:/web_gomsu_nhom5/appsettings.json), hệ thống sẽ chuyển sang `RedisCacheService` với Key Prefix `GomSuNhom5:` mà không thay đổi bất kỳ code nghiệp vụ nào!

---

## 📐 3. Kiến Trúc Hệ Thống (Clean Architecture / Ports & Adapters)

```text
d:\web_gomsu_nhom5\
├── QuanLyBanDoGomSu.sql            # Script DDL SQL Server 2022 (21 Bảng CSDL)
├── appsettings.json                # ConnectionString & RedisConfig (localhost:6379)
├── README.md                       # Tài liệu tổng quan Backend & Roadmap
├── API_DOCUMENTATION.md            # Tài liệu danh mục API Endpoints & Payload mẫu
└── src/
    ├── Domain/                     # Core Entities (DanhMucHangHoa, LoaiMen, HoaDonBan...)
    ├── Application/                # Use Cases, DTOs Essential Fields & Port Interfaces
    │   ├── DTOs/                   # (DanhMucHangHoaDto, ProductFilterRequest, OrderDto...)
    │   ├── Port/In/                # (IDanhMucHangHoaService, IOrderService, IChatService...)
    │   └── Port/Out/               # (IDanhMucHangHoaRepository, ICacheService...)
    ├── Exception/                  # Custom Exceptions & Chuẩn ApiResponse<T>
    ├── Infrastructure/             # EF Core DbContext & Redis/InMemory Cache Adapter
    └── Presentation/               # Web API Controllers (HangHoa, DonHang, Chat, ThongKe)
```

---

## 🗄️ 4. Danh Sách 21 Bảng Cơ Sở Dữ Liệu SQL Server (`WebQuanLyBanDoGomSu`)

| STT | Tên Bảng SQL | Mô Tả Nghiệp Vụ Quản Lý |
| :-: | :--- | :--- |
| 1 | `DanhMucHangHoa` | Thông tin sản phẩm gốm sứ (mã, tên, loại men, kích thước, đơn giá...) |
| 2 | `Loai` | Phân loại sản phẩm (Bình hoa, Ấm chén, Lục bình, Đĩa trang trí...) |
| 3 | `KichThuoc` | Kích thước sản phẩm (Chiều cao, đường kính) |
| 4 | `CongDung` | Công dụng đồ gốm (Trang trí, Đồ thờ, Dùng hàng ngày...) |
| 5 | `LoaiMen` | Loại men gốm (Men rạn, Men lam, Men ngọc, Men hoả biến...) |
| 6 | `HinhKhoi` | Hình khối sản phẩm (Tròn, Vuông, Hình trụ, Hình cầu...) |
| 7 | `MauSac` | Màu sắc chủ đạo sản phẩm gốm sứ |
| 8 | `NuocSanXuat` | Nguồn gốc sản xuất (Việt Nam - Bát Tràng, Chu Đậu, Giang Tây...) |
| 9 | `HoaVan` | Họa tiết khắc hoa văn (Sơn thủy, Tùng cúc trúc mai, Rồng phụng...) |
| 10 | `TaiKhoan` | Tài khoản đăng nhập hệ thống (BCrypt Password, Role, Status) |
| 11 | `NhanVien` | Thông tin nhân viên quản lý & bán hàng |
| 12 | `KhachHang` | Thông tin khách hàng mua đồ gốm sứ |
| 13 | `NhaCungCap` | Nhà cung cấp nguồn gốm sứ |
| 14 | `HoaDonBan` | Hóa đơn đơn hàng bán cho khách hàng |
| 15 | `ChiTietHoaDonBan` | Chi tiết mặt hàng & số lượng trong hóa đơn bán |
| 16 | `HoaDonNhap` | Hóa đơn nhập hàng từ nhà cung cấp |
| 17 | `ChiTietHoaDonNhap` | Chi tiết mặt hàng nhập trong hóa đơn nhập |
| 18 | `CongViec` | Chức vụ và danh mục công việc nhân viên |
| 19 | `PhanQuyen` | Danh mục quyền hạn quản trị hệ thống |
| 20 | `GioHang` | Thông tin giỏ hàng lưu trữ người dùng |
| 21 | `ChiTietGioHang` | Chi tiết sản phẩm gốm sứ có trong giỏ hàng |

> ⚠️ **Quy tắc ORM**: CSDL được quản lý `100%` bằng script SQL DDL ([`QuanLyBanDoGomSu.sql`](file:///d:/web_gomsu_nhom5/QuanLyBanDoGomSu.sql)). Cấu hình `OrmConfig:DdlAuto = "none"`, không tự động sinh schema.

---

## 🛠️ 5. Hướng Dẫn Khởi Chạy

1. **Khởi Tạo CSDL**: Chạy script [`QuanLyBanDoGomSu.sql`](file:///d:/web_gomsu_nhom5/QuanLyBanDoGomSu.sql) trong SQL Server 2022.
2. **Cấu Hình Chuỗi Kết Nối & Redis**: Kiểm tra ConnectionString & Redis Endpoint (`localhost:6379`) trong [`appsettings.json`](file:///d:/web_gomsu_nhom5/appsettings.json).
3. **Khởi Chạy API**:
   ```bash
   cd d:\web_gomsu_nhom5
   dotnet run
   ```

---

## 📖 6. Tài Liệu REST API Chi Tiết

- [📖 API_DOCUMENTATION.md](file:///d:/web_gomsu_nhom5/API_DOCUMENTATION.md)
