# 🏺 HƯỚNG DẪN CÀI ĐẶT & KHỞI CHẠY BACKEND (.NET 8 & SQL SERVER)
> **Dự án**: Hệ Thống Quản Lý Bán Đồ Gốm Sứ Bát Tràng (Nhóm 5)  
> **Backend RESTful Web API**: ASP.NET Core 8.0 LTS + Entity Framework Core 8 + SQL Server

---

## 📌 TÓM TẮT NHANH CÁCH CHẠY (DÀNH CHO NGƯỜI ĐÃ CÓ MÔI TRƯỜNG)
1. Cấu hình chuỗi kết nối SQL Server của bạn trong [`appsettings.json`](file:///d:/web_gomsu_nhom5/appsettings.json).
2. Mở Terminal tại thư mục này và chạy lệnh:
   ```powershell
   dotnet run --urls "http://localhost:5000"
   ```
👉 **Cơ sở dữ liệu (Database + 21 Bảng + Dữ liệu mẫu + Tài khoản test) sẽ TỰ ĐỘNG SINH 100%**.  
👉 Kiểm tra API tại: **[http://localhost:5000/swagger](http://localhost:5000/swagger)**

---

## 🛠️ HƯỚNG DẪN CÀI ĐẶT CHI TIẾT TỪNG BƯỚC TỪ A - Z (CHO MÁY MỚI)

### BƯỚC 1: Cài Đặt Các Phần Mềm Môi Trường Bắt Buộc

#### 1. Cài đặt .NET 8.0 SDK (Bắt buộc)
- **Tải từ trang chủ Microsoft**: [https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Chọn bản **.NET SDK x64** cho Windows (hoặc macOS/Linux tương ứng).
- Tải về và cài đặt theo hướng dẫn mặc định của bộ cài.
- **Kiểm tra sau khi cài đặt**: Mở PowerShell hoặc CMD gõ lệnh:
  ```powershell
  dotnet --version
  ```
  *(Nếu màn hình hiển thị phiên bản bắt đầu bằng `8.0.xxx` hoặc `9.0.xxx` là thành công!)*

#### 2. Cài đặt Microsoft SQL Server (Bắt buộc)
- **Tải SQL Server (Bản Developer hoặc Express miễn phí)**: [https://www.microsoft.com/en-us/sql-server/sql-server-downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Tiến hành cài đặt chọn kiểu **Basic** hoặc **Custom**.
- **Tải thêm công cụ quản lý trực quan SSMS (SQL Server Management Studio)**: [https://aka.ms/ssmsfullsetup](https://aka.ms/ssmsfullsetup) để dễ dàng xem bảng và dữ liệu.

---

### BƯỚC 2: Cấu Hình Chuỗi Kết Nối Cơ Sở Dữ Liệu (`appsettings.json`)

1. Mở file [`appsettings.json`](file:///d:/web_gomsu_nhom5/appsettings.json) trong thư mục dự án bằng VS Code, Visual Studio hoặc Notepad.
2. Tìm đến đoạn `"ConnectionStrings"`:

#### 👉 Trường hợp phổ biến nhất: Kết nối bằng Windows Authentication (Không cần mật khẩu)
Nếu máy bạn cài SQL Server mặc định:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=QuanLyBanDoGomSu;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
*(Nếu máy bạn đặt tên Instance là `SQLEXPRESS`, hãy đổi `Server=localhost\\SQLEXPRESS` hoặc `Server=.\\SQLEXPRESS`)*.

#### 👉 Trường hợp 2: Kết nối bằng tài khoản `sa` và mật khẩu
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=QuanLyBanDoGomSu;User Id=sa;Password=MậtKhẩuCủaBạn;TrustServerCertificate=True;"
  }
}
```

#### 👉 Trường hợp 3: Kết nối bằng SQL Server LocalDB (Có sẵn nếu bạn đã cài Visual Studio)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuanLyBanDoGomSu;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

### BƯỚC 3: Khởi Chạy Ứng Dụng (CSDL Tự Sinh 100%)

> ⚠️ **ĐẶC BIỆT LƯU Ý**: Bạn **KHÔNG CẦN** tự tạo Database hay chạy câu lệnh SQL nào trước!  
> Ứng dụng đã tích hợp sẵn cơ chế **Auto-Migration & Data Seeder** (`DatabaseInitializer.cs`).

1. Mở cửa sổ **PowerShell** hoặc **Command Prompt** (CMD).
2. Di chuyển đến thư mục dự án Backend:
   ```powershell
   cd d:\web_gomsu_nhom5
   ```
3. Chạy lệnh phục hồi các gói thư viện:
   ```powershell
   dotnet restore
   ```
4. Chạy lệnh biên dịch mã nguồn:
   ```powershell
   dotnet build
   ```
   *(Đảm bảo kết quả trả về `0 Warning(s), 0 Error(s)`)*.
5. Chạy ứng dụng Backend Web API:
   ```powershell
   dotnet run --urls "http://localhost:5000"
   ```

Khi màn hình xuất hiện các thông báo sau là hệ thống đã hoạt động hoàn tất:
```text
[DatabaseInitializer] Co so du lieu da san sang voi 20 mat hang mau va 2 tai khoan test mac dinh!
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Những việc hệ thống đã tự động làm cho bạn trong tích tắc:**
1. Tự tạo Cơ sở dữ liệu tên là `QuanLyBanDoGomSu` trên SQL Server của bạn.
2. Tự sinh toàn bộ **21 Bảng dữ liệu** với đầy đủ khóa chính, khóa ngoại, quan hệ 1-N, N-N.
3. Tự nạp dữ liệu danh mục: 6 Loại hàng, 5 Loại men Bát Tràng (Men lam, Men rạn...), 4 Kích thước, Màu sắc, Hoa văn, Hình khối, Công dụng.
4. Tự nạp **20 sản phẩm gốm sứ Bát Tràng mẫu** với giá bán, hình ảnh, mô tả chi tiết, số lượng tồn kho.
5. Tự nạp sẵn **2 Tài khoản kiểm thử** (đã mã hóa BCrypt).

---

### BƯỚC 4: Kiểm Tra Hoạt Động Của API (Swagger UI)

Mở trình duyệt web bất kỳ (Chrome, Edge, Cốc Cốc) và truy cập:
👉 **[http://localhost:5000/swagger](http://localhost:5000/swagger)**

Tại đây bạn sẽ thấy giao diện Swagger UI liệt kê đầy đủ toàn bộ API của hệ thống:
- `GET /api/v1/hang-hoa`: Lấy danh sách gốm sứ (tìm kiếm, lọc giá, loại men...).
- `GET /api/v1/hang-hoa/{id}`: Xem chi tiết 1 sản phẩm.
- `POST /api/v1/auth/login`: Đăng nhập hệ thống.
- `POST /api/v1/don-hang`: Tạo đơn đặt hàng.
- `GET /api/v1/don-hang/tra-cuu?soDienThoai=...`: Tra cứu đơn hàng theo SĐT.
- `GET /api/v1/thong-ke/doanh-so`: Báo cáo doanh số và top sản phẩm bán chạy.

---

## 🔑 TÀI KHOẢN ĐĂNG NHẬP KIỂM THỬ (ĐÃ NẠP SẴN TRONG DB)

| Loại Tài Khoản | Tên Đăng Nhập | Mật Khẩu | Quyền Hạn (Role) | Chức Năng |
| :--- | :--- | :--- | :---: | :--- |
| **Quản Trị Viên (Admin)** | `admin` | `admin123` | `ADMIN` | Quản trị toàn bộ hệ thống: Xem dashboard doanh thu, Thêm/Sửa/Xóa gốm sứ, duyệt trạng thái đơn hàng, chat trực tiếp với khách. |
| **Khách Hàng (Customer)** | `user` | `user123` | `KH` | Mua sắm gốm sứ, thêm giỏ hàng, đặt hàng thanh toán COD/QR, xem lịch sử mua cá nhân. |

---

## 📂 PHƯƠNG ÁN PHỤ: CHẠY SCRIPT SQL THỦ CÔNG (NẾU GIẢNG VIÊN YÊU CẦU)
Nếu người chấm bài muốn thẩm định file script SQL DDL thủ công:
1. Mở **SQL Server Management Studio (SSMS)** kết nối vào SQL Server.
2. Mở file script có sẵn trong dự án: [`QuanLyBanDoGomSu.sql`](file:///d:/web_gomsu_nhom5/QuanLyBanDoGomSu.sql).
3. Bấm nút **Execute** (hoặc phím `F5`) để chạy script tạo DB và bảng.
4. Mở Terminal chạy `dotnet run --urls "http://localhost:5000"`, hệ thống sẽ tự động nhận diện và nạp tiếp dữ liệu sản phẩm mẫu nếu bảng đang trống.

---

## ❓ CÁC LỖI THƯỜNG GẶP VÀ CÁCH KHẮC PHỤC NHANH

1. **Lỗi `Cannot open database "QuanLyBanDoGomSu"` hoặc lỗi kết nối SQL Server**:
   - **Cách sửa**: Kiểm tra xem dịch vụ SQL Server có đang chạy không: Nhấn phím `Windows + R`, gõ `services.msc`, tìm dịch vụ `SQL Server (MSSQLSERVER)` hoặc `SQL Server (SQLEXPRESS)` và bấm chuột phải chọn **Start** (Khởi động).
2. **Lỗi `The process cannot access the file ... QuanLyBanDoGomSu.exe` khi build**:
   - **Cách sửa**: Do ứng dụng đang chạy nền ở một cửa sổ khác. Hãy tắt cửa sổ đó hoặc chạy lệnh sau trong PowerShell để tắt tiến trình:
     ```powershell
     Stop-Process -Name "QuanLyBanDoGomSu" -Force -ErrorAction SilentlyContinue
     ```
3. **Cổng 5000 bị ứng dụng khác chiếm**:
   - **Cách sửa**: Chạy trên cổng khác, ví dụ cổng 5001:
     ```powershell
     dotnet run --urls "http://localhost:5001"
     ```
     *(Sau đó nhớ chỉnh lại cổng Backend tương ứng trong Frontend)*.

---


