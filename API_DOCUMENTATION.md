# 📜 Tài Liệu Danh Mục RESTful Web API - Quản Lý Bán Đồ Gốm Sứ

![API Version](https://img.shields.io/badge/API-v1-blue)
![Redis Endpoint](https://img.shields.io/badge/Redis%20Cache-localhost%3A6379-DC382D?logo=redis)
![Format](https://img.shields.io/badge/Format-JSON%20ApiResponse%3CT%3E-orange)

Tài liệu này quy định danh sách các Web API Endpoints thuộc ứng dụng Backend REST API (`d:\web_gomsu_nhom5`). Tất cả các phản hồi HTTP đều tuân thủ cấu trúc đóng gói chuẩn `ApiResponse<T>` và quy tắc **Essential Fields Only** (chỉ trả về các trường cần thiết cho UI để tối ưu băng thông).

---

## 📌 Base URL & Redis Configuration

- **Development Base URL**: `http://localhost:5001/api/v1` (hoặc `https://localhost:7001/api/v1`)
- **Redis Cache Connection**: `localhost:6379` (cấu hình qua `RedisConfig:ConnectionString` trong `appsettings.json`)
- **Redis Mandatory Key Prefix**: `GomSuNhom5:` (Tất cả Redis Keys BẮT BUỘC có tiền tố `GomSuNhom5:` ví dụ: `GomSuNhom5:Product:HH001` để tránh xung đột dữ liệu với dự án khác trên local).
- **Sequential Database Primary Keys (Chống phân mảnh Clustered Index SQL Server)**:
  - Khóa chính CSDL bắt buộc có yếu tố **tự tăng theo thứ tự thời gian/số đếm (Sequential ID)** để bảo vệ B-Tree Clustered Index, chống rách trang (Page Split), giảm chi phí I/O và chống phình bộ nhớ RAM.
  - Định dạng sinh khóa:
    - Hàng hóa: `HH` + `00000001`, `HH` + `00000002`... (hoặc số thứ tự tự tăng)
    - Hóa đơn bán: `HDB` + `YYYYMMDD` + `0001` (Tiền tố + Ngày + Thứ tự trong ngày)
    - Hóa đơn nhập: `HDN` + `YYYYMMDD` + `0001`
    - Khách hàng / Nhân viên: `KH00000001`, `NV00000001`...


---

## 📋 Danh Mục Tổng Quan Tất Cả API Endpoints

| Phân Khu | Method | Endpoint | Mô Tả Nghiệp Vụ | Parameters / Payload |
| **Tài Khoản** | `POST` | `/auth/login` | Đăng nhập tài khoản & nhận JWT token | JSON Body: `LoginRequest` |
| **Tài Khoản** | `POST` | `/auth/register` | Đăng ký tài khoản khách hàng mới | JSON Body: `RegisterRequest` |
| **Tài Khoản** | `GET` | `/auth/profile` | Lấy chi tiết hồ sơ cá nhân & avatar người dùng | Query: `username` |
| **Tài Khoản** | `PUT` | `/auth/profile` | Cập nhật thông tin cá nhân & đổi ảnh đại diện | JSON Body: `UpdateProfileRequest` |
| **Tài Khoản** | `POST` | `/auth/change-password` | Đổi mật khẩu tài khoản bảo mật SHA-256 | JSON Body: `ChangePasswordRequest` |
| **Sản Phẩm** | `GET` | `/hang-hoa` | Full-Text Search theo tên & Lọc đa tiêu chí | `keyword`, `maLoai`, `maKichThuoc`, `maLoaiMen`, `maMauSac`, `minPrice`, `maxPrice`, `pageIndex`, `pageSize` |
| **Sản Phẩm** | `GET` | `/hang-hoa/{id}` | Lấy chi tiết thông tin 1 sản phẩm gốm sứ | Path: `id` |
| **Sản Phẩm (Admin)** | `POST` | `/hang-hoa` | Thêm mới 1 sản phẩm đồ gốm sứ | JSON Body: `DanhMucHangHoaDto` (Essential Fields) |
| **Sản Phẩm (Admin)** | `PUT` | `/hang-hoa/{id}` | Cập nhật thông tin sản phẩm gốm sứ | Path: `id`, JSON Body: `DanhMucHangHoaDto` |
| **Sản Phẩm (Admin)** | `DELETE` | `/hang-hoa/{id}` | Xóa sản phẩm khỏi danh mục | Path: `id` |
| **Giỏ Hàng & Đơn Hàng** | `POST` | `/don-hang` | Đặt hàng mua đồ gốm sứ | JSON Body: `CreateOrderRequest` |
| **Dashboard (Admin)** | `GET` | `/thong-ke/doanh-so` | Thống kê doanh số & báo cáo tổng quan | Query: `fromDate`, `toDate` |
| **Báo Cáo (Admin)** | `GET` | `/thong-ke/xuat-excel` | Xuất báo cáo doanh số Excel đa sheet theo ngày/tháng | Query: `startDate`, `endDate` (ClosedXML .xlsx) |
| **Chat Tư Vấn** | `GET` | `/chat/messages` | Lấy lịch sử tin nhắn chat tư vấn | Query: `sessionId` |
| **Chat Tư Vấn** | `GET` | `/chat/messages/cursor` | Lấy lịch sử tin nhắn phân trang Cursor (trượt lên) | Query: `sessionId`, `cursorId`, `limit` |
| **Chat Tư Vấn** | `POST` | `/chat/send` | Gửi tin nhắn chat tư vấn đồ gốm sứ (Append-Only) | JSON Body: `SendMessageRequest` |
| **Chat WebSocket** | `WS` | `/hubs/chat` | SignalR WebSocket Chat Hub thời gian thực | Methods: `JoinSession`, `LeaveSession`, `SendMessage` |

---

## 🔍 Chi Tiết Chi Tiết Các Endpoints

### 1. GET `/api/v1/hang-hoa` - Tìm Kiếm Full-Text Search & Lọc Đa Tiêu Chí

- **HTTP Method**: `GET`
- **Query Parameters**:
  - `keyword` *(string, optional)*: Từ khóa tìm kiếm Full-Text Search theo tên hàng hóa đồ gốm
  - `maLoai` *(string, optional)*: Mã loại sản phẩm (`L01` - Bình hoa, `L02` - Ấm chén...)
  - `maKichThuoc` *(string, optional)*: Mã kích thước (`KT01`, `KT02`)
  - `maLoaiMen` *(string, optional)*: Mã loại men (`LM01` - Men rạn, `LM02` - Men ngọc)
  - `maMauSac` *(string, optional)*: Mã màu sắc
  - `minPrice` / `maxPrice` *(decimal, optional)*: Khoảng giá lọc
  - `pageIndex` / `pageSize` *(int, default: 1 / 10)*: Phân trang

- **Response Mẫu (Essential Fields Only)**:
```json
{
  "status": 200,
  "message": "Lấy danh sách hàng hóa gốm sứ thành công",
  "data": {
    "items": [
      {
        "maHangHoa": "HH001",
        "tenHangHoa": "Bình Hoa Gốm Men Rạn Bát Tràng H25cm",
        "tenLoai": "Bình hoa",
        "tenLoaiMen": "Men rạn cổ",
        "donGiaBan": 1350000.00,
        "soLuong": 20,
        "anhBase64": null
      }
    ],
    "totalCount": 1,
    "pageIndex": 1,
    "pageSize": 10
  },
  "error": null,
  "timestamp": "2026-10-04T10:00:00Z"
}
```

---

### 2. POST `/api/v1/don-hang` - Đặt Hàng Mua Đồ Gốm Sứ

- **HTTP Method**: `POST`
- **Request Body (Essential Fields)**:
```json
{
  "maKhachHang": "KH001",
  "hoTen": "Nguyễn Văn A",
  "soDienThoai": "0987654321",
  "diaChiGiaoHang": "123 Đường Bát Tràng, Hà Nội",
  "ghiChu": "Giao giờ hành chính, đóng gói xốp bong bóng chống va đập",
  "chiTiet": [
    {
      "maHangHoa": "HH001",
      "soLuong": 2,
      "donGiaBan": 1350000.00
    }
  ]
}
```

- **Response Thành Công (200 OK)**:
```json
{
  "status": 200,
  "message": "Tạo đơn hàng mua đồ gốm sứ thành công",
  "data": {
    "soHDB": "HDB20261004001",
    "ngayBan": "2026-10-04T11:00:00Z",
    "tongTien": 2700000.00,
    "trangThai": "ChoXacNhan"
  },
  "error": null,
  "timestamp": "2026-10-04T11:00:00Z"
}
```

---

### 2. POST `/api/v1/don-hang` - Đặt Hàng Mua Sản Phẩm Gốm Sứ

- **HTTP Method**: `POST`
- **Request Body**:
```json
{
  "maKhach": "KH00000001",
  "maNV": "NV001",
  "items": [
    {
      "maHangHoa": "HH00000001",
      "soLuong": 2,
      "giamGia": 5.0
    },
    {
      "maHangHoa": "HH00000002",
      "soLuong": 1,
      "giamGia": 0
    }
  ]
}
```

- **Response Mẫu (`201 Created`)**:
```json
{
  "status": 201,
  "message": "Đặt hàng đồ gốm sứ thành công (Mã HĐ: HDB202610040001)",
  "data": {
    "soHDB": "HDB202610040001",
    "maNV": "NV001",
    "maKhach": "KH00000001",
    "ngayBan": "2026-10-04T11:48:00",
    "tongTien": 4865000.00,
    "items": [
      {
        "maHangHoa": "HH00000001",
        "tenHangHoa": "Bình Hoa Gốm Sứ Men Rạn Cổ Sơn Thủy",
        "soLuong": 2,
        "donGiaBan": 1850000.00,
        "giamGia": 5.0,
        "thanhTien": 3515000.00
      },
      {
        "maHangHoa": "HH00000002",
        "tenHangHoa": "Bộ Ấm Chén Gốm Tử Sa Bát Tràng",
        "soLuong": 1,
        "donGiaBan": 1350000.00,
        "giamGia": 0,
        "thanhTien": 1350000.00
      }
    ]
  },
  "error": null,
  "timestamp": "2026-10-04T11:48:00Z"
}
```

---

### 3. GET `/api/v1/don-hang/lich-su` - Tra Cứu Lịch Sử Đơn Hàng

- **HTTP Method**: `GET`
- **Query Parameters**: `maKhach=KH00000001&pageIndex=1&pageSize=10`
- **Response Mẫu (`200 OK`)**:
```json
{
  "status": 200,
  "message": "Lấy lịch sử đơn hàng của khách hàng 'KH00000001' thành công",
  "data": {
    "items": [
      {
        "soHDB": "HDB202610040001",
        "maNV": "NV001",
        "tenNV": "Nguyễn Văn Admin",
        "maKhach": "KH00000001",
        "tenKhach": "Trần Thị Khách",
        "ngayBan": "2026-10-04T11:48:00",
        "tongTien": 4865000.00,
        "items": []
      }
    ],
    "totalCount": 1,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "error": null,
  "timestamp": "2026-10-04T11:48:00Z"
}
```

---

### 4. GET `/api/v1/thong-ke/doanh-so` - Thống Kê Doanh Số Dashboard Admin

- **HTTP Method**: `GET`
- **Query Parameters**: `startDate=2026-09-01&endDate=2026-10-04`

- **Response Mẫu (`200 OK`)**:
```json
{
  "status": 200,
  "message": "Lấy dữ liệu thống kê doanh số Dashboard thành công",
  "data": {
    "tongDoanhThu": 145800000.00,
    "tongDonHang": 48,
    "tongSanPhamDaBan": 126,
    "tongKhachHang": 32,
    "doanhThuTheoNgay": [
      { "ngay": "2026-10-01", "doanhThu": 28500000.00, "soDonHang": 8 },
      { "ngay": "2026-10-02", "doanhThu": 45200000.00, "soDonHang": 14 }
    ],
    "topSanPhamBanChay": [
      {
        "maHangHoa": "HH00000001",
        "tenHangHoa": "Bình Hoa Gốm Sứ Men Rạn Cổ Sơn Thủy",
        "soLuongDaBan": 42,
        "tongTien": 77700000.00
      }
    ]
  },
  "error": null,
  "timestamp": "2026-10-04T11:48:00Z"
}
```

---

### 5. GET `/api/v1/chat/messages/cursor` - Lịch Sử Tin Nhắn Chat (Phân Trang Con Trỏ Cursor)

- **HTTP Method**: `GET`
- **Query Parameters**: `sessionId=CHAT_SESS_88392&cursorId=105&limit=15`
- **Ghi chú**: Trượt lên đỉnh khung chat sẽ truyền `cursorId` (Mã tin nhắn nhỏ nhất hiện tại) để lấy các tin nhắn cũ hơn. Giúp bảo vệ hiệu năng index B-Tree SQL Server $O(1)$.
- **Response Mẫu (`200 OK`)**:
```json
{
  "status": 200,
  "message": "Lấy danh sách tin nhắn phân trang con trỏ thành công",
  "data": {
    "items": [
      {
        "id": "101",
        "sessionId": "CHAT_SESS_88392",
        "senderId": "KH00000001",
        "senderName": "Trần Thị Khách",
        "content": "Cho tôi hỏi bình hoa men rạn HH00000001 có cắm được hoa tươi không?",
        "isNhanVien": false,
        "timestamp": "2026-10-04T11:48:00"
      },
      {
        "id": "102",
        "sessionId": "CHAT_SESS_88392",
        "senderId": "NV001",
        "senderName": "Tư Vấn Viên Bát Tràng",
        "content": "Chào bạn, bình hoa men rạn được đun ở nhiệt độ 1250°C chống thấm nước 100%, cắm hoa tươi rất tốt nhé!",
        "isNhanVien": true,
        "timestamp": "2026-10-04T11:49:15"
      }
    ],
    "nextCursor": "101",
    "hasMore": true
  },
  "error": null,
  "timestamp": "2026-10-04T11:50:00Z"
}
```

---

### 6. POST `/api/v1/chat/send` - Gửi Tin Nhắn Chat Tư Vấn Gốm Sứ (Append-Only)

- **HTTP Method**: `POST`
- **Quy tắc Append-Only**: Chỉ hỗ trợ Tạo tin nhắn (Send) và Đọc tin nhắn (Read), không cho phép sửa hay xóa tin nhắn.
- **Request Body**:
```json
{
  "sessionId": "CHAT_SESS_88392",
  "senderId": "KH00000001",
  "senderName": "Trần Thị Khách",
  "content": "Cho tôi hỏi bình hoa men rạn HH00000001 có cắm được hoa tươi không?",
  "isNhanVien": false
}
```

- **Response Mẫu (`200 OK`)**:
```json
{
  "status": 200,
  "message": "Gửi tin nhắn tư vấn thành công",
  "data": {
    "id": "103",
    "sessionId": "CHAT_SESS_88392",
    "senderId": "KH00000001",
    "senderName": "Trần Thị Khách",
    "content": "Cho tôi hỏi bình hoa men rạn HH00000001 có cắm được hoa tươi không?",
    "isNhanVien": false,
    "timestamp": "2026-10-04T11:48:00"
  },
  "error": null,
  "timestamp": "2026-10-04T11:48:00Z"
}
```

---

### 7. 🔌 SignalR WebSocket Real-time Chat Hub (`/hubs/chat`)

- **Protocol**: WebSocket / WSS
- **Hub Endpoint**: `http://localhost:5000/hubs/chat`
- **Methods Client Gửi tới Hub**:
  - `JoinSession(sessionId)`: Tham gia nhóm tin nhắn của 1 phiên tư vấn.
  - `LeaveSession(sessionId)`: Rời khỏi nhóm tin nhắn.
  - `SendMessage(payload)`: Gửi tin nhắn mới thời gian thực.
- **Event Server Phát tới Client**:
  - `ReceiveMessage(ChatMessageDto)`: Nhận tin nhắn mới tức thì không cần Polling.

---

### 8. 👤 Authentication & Quản Lý Hồ Sơ Cá Nhân (User Profile, Avatar & Security)

Hệ thống cung cấp nhóm API xác thực và quản lý tài khoản hỗ trợ bảo mật mật khẩu chuẩn SHA-256:

#### 8.1 POST `/api/v1/auth/login` - Đăng Nhập Hệ Thống
- **HTTP Method**: `POST`
- **Request Body**:
```json
{
  "tenDangNhap": "admin",
  "matKhau": "admin123"
}
```
- **Response Mẫu (`200 OK`)**:
```json
{
  "status": 200,
  "message": "Đăng nhập thành công",
  "data": {
    "token": "bearer-mock-token-admin",
    "user": {
      "tenDangNhap": "admin",
      "quyen": "Admin",
      "hoTen": "Quản Trị Viên Bát Tràng",
      "maDinhDanh": "NVADMIN",
      "soDienThoai": "0987654321",
      "diaChi": "Làng gốm Bát Tràng, Gia Lâm, Hà Nội",
      "avatar": "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=150"
    }
  },
  "error": null,
  "timestamp": "2026-10-11T12:00:00Z"
}
```

#### 8.2 GET `/api/v1/auth/profile` - Lấy Thông Tin Hồ Sơ & Avatar
- **HTTP Method**: `GET`
- **Headers**: `Authorization: Bearer <token>`
- **Response Mẫu (`200 OK`)**:
```json
{
  "status": 200,
  "message": "Lấy thông tin người dùng thành công",
  "data": {
    "tenDangNhap": "admin",
    "quyen": "Admin",
    "hoTen": "Quản Trị Viên Bát Tràng",
    "maDinhDanh": "NVADMIN",
    "soDienThoai": "0987654321",
    "diaChi": "Làng gốm Bát Tràng, Gia Lâm, Hà Nội",
    "avatar": "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=150"
  },
  "error": null,
  "timestamp": "2026-10-11T12:00:00Z"
}
```

#### 8.3 PUT `/api/v1/auth/profile` - Cập Nhật Thông Tin Cá Nhân & Avatar
- **HTTP Method**: `PUT`
- **Headers**: `Authorization: Bearer <token>`
- **Request Body**:
```json
{
  "hoTen": "Nghệ Nhân Bát Tràng",
  "soDienThoai": "0912345678",
  "diaChi": "Xóm 1, Bát Tràng, Gia Lâm, Hà Nội",
  "avatar": "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150"
}
```
- **Response Mẫu (`200 OK`)**:
```json
{
  "status": 200,
  "message": "Cập nhật thông tin thành công",
  "data": {
    "tenDangNhap": "admin",
    "quyen": "Admin",
    "hoTen": "Nghệ Nhân Bát Tràng",
    "maDinhDanh": "NVADMIN",
    "soDienThoai": "0912345678",
    "diaChi": "Xóm 1, Bát Tràng, Gia Lâm, Hà Nội",
    "avatar": "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150"
  },
  "error": null,
  "timestamp": "2026-10-11T12:00:00Z"
}
```

#### 8.4 POST `/api/v1/auth/change-password` - Đổi Mật Khẩu
- **HTTP Method**: `POST`
- **Headers**: `Authorization: Bearer <token>`
- **Mã Hóa**: Sử dụng SHA-256 so sánh mật khẩu an toàn.
- **Request Body**:
```json
{
  "matKhauHienTai": "admin123",
  "matKhauMoi": "Admin@2026NewPass"
}
```
- **Response Mẫu (`200 OK`)**:
```json
{
  "status": 200,
  "message": "Đổi mật khẩu thành công. Vui lòng đăng nhập lại với mật khẩu mới.",
  "data": true,
---

### 9. GET `/api/v1/thong-ke/xuat-excel` - Xuất Báo Cáo Doanh Số Excel Đa Sheet (.xlsx)

- **HTTP Method**: `GET`
- **Thư Viện Triển Khai**: `ClosedXML v0.105.1` (Tạo workbook `.xlsx` trong bộ nhớ MemoryStream tốc độ cao, chuẩn font UTF-8 tiếng Việt, hỗ trợ công thức Excel `SUM`).
- **Query Parameters**:
  - `startDate` *(string ISO-8601, optional)*: Thời điểm bắt đầu lọc (ví dụ: `2026-10-01T00:00:00Z`). Nếu bỏ trống, mặc định lấy ngày 1 của tháng hiện tại.
  - `endDate` *(string ISO-8601, optional)*: Thời điểm kết thúc lọc (ví dụ: `2026-10-31T23:59:59Z`). Nếu bỏ trống, mặc định lấy thời điểm hiện tại.
- **Content-Type Trả Về**: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- **File Tải Về**: `BaoCao_DoanhSo_GomSu_YYYYMMDD_YYYYMMDD.xlsx`
- **Cấu Trúc Các Sheet**:
  1. **Sheet 1: "Tổng Quan Doanh Thu"**:
     - Header thương hiệu Bát Tràng Atelier (màu nền men đỏ huyết dụ `#6D2417`, chữ vàng ngà `#FFF9C4`).
     - 4 thẻ KPI tổng kết: Tổng Doanh Thu, Tổng Đơn Hàng, Sản Phẩm Đã Bán, Giá Trị Đơn Trung Bình.
     - Bảng chi tiết doanh thu theo từng ngày (Cột: Ngày, Số Đơn Hàng, Doanh Thu) kèm dòng **Tổng Cộng** sử dụng công thức Excel tự động `=SUM(...)`.
  2. **Sheet 2: "Chi Tiết Đơn Hàng"**:
     - Danh sách đơn hàng trong kỳ lọc: Mã hóa đơn, Ngày bán, Tên khách hàng, Số điện thoại, Nhân viên phụ trách, Tổng tiền (định dạng `#,##0 "₫"`).
     - Dòng tổng kết công thức Excel `=SUM(...)`.
  3. **Sheet 3: "Top Sản Phẩm Bán Chạy"**:
     - Bảng xếp hạng sản phẩm gốm sứ có lượng tiêu thụ và doanh thu cao nhất.
- **Phản Hồi Mẫu**: File nhị phân dạng Blob/Stream (Download trực tiếp).

---

## ⚡ Redis Cache Seam Architecture (`localhost:6379`)

Hệ thống đã thiết kế giao diện cache `ICacheService` để lưu trữ tạm các dữ liệu truy vấn nhiều:
- **Redis Server Endpoint**: `localhost:6379`
- **Quy tắc đặt tên Key Redis (Key Prefix Mandatory)**: Tất cả các Key Redis **bắt buộc** phải bắt đầu bằng tiền tố `GomSuNhom5:` để phân biệt tuyệt đối với các dự án cũ trên cùng Server local.
- **Ví dụ Redis Keys**:
  - `GomSuNhom5:Product:HH001` (Cache chi tiết sản phẩm)
  - `GomSuNhom5:Categories:List` (Cache danh mục loại gốm)
  - `GomSuNhom5:Glazes:List` (Cache danh mục loại men)
  - `GomSuNhom5:Search:MenRan:Min100` (Cache kết quả tìm kiếm/lọc sản phẩm)

```csharp
public interface ICacheService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}
```
*Hiện tại chạy qua `InMemoryCacheService`. Khi khởi chạy Redis Server tại `localhost:6379`, chỉ cần chuyển đổi DI container trong `Program.cs` và bật `RedisConfig:Enabled = true` trong [`appsettings.json`](file:///d:/web_gomsu_nhom5/appsettings.json).*

