# 📘 HƯỚNG DẪN CÀI ĐẶT & CHẠY TOÀN BỘ HỆ THỐNG GỐM SỨ (BACKEND + FRONTEND)
> **Dự án**: Website Thương Mại Điện Tử & Quản Lý Bán Đồ Gốm Sứ Bát Tràng (Nhóm 5)  
> **Kiến trúc**: Backend RESTful Web API (.NET 8 + SQL Server) & Frontend SPA (React 19 + TypeScript + Vite)

Tài liệu này hướng dẫn chi tiết từng bước cho người mới hoặc giảng viên chấm bài có thể cài đặt và khởi chạy toàn bộ hệ thống từ đầu trên máy tính cá nhân.

---

## ⚡ TÓM TẮT CÁC BƯỚC KHỞI CHẠY NHANH
```text
[BƯỚC 1: KHỞI CHẠY BACKEND (.NET 8 + SQL SERVER)]
Mở PowerShell 1:
cd d:\web_gomsu_nhom5
dotnet run --urls "http://localhost:5000"
(CSDL, 21 bảng, 20 sản phẩm mẫu và tài khoản test sẽ TỰ ĐỘNG SINH 100%)

---------------------------------------------------------
[BƯỚC 2: KHỞI CHẠY FRONTEND (REACT 19 + VITE)]
Mở PowerShell 2:
cd d:\Fe_gomsu_nhom5
npm install
npm run dev

---------------------------------------------------------
[BƯỚC 3: MỞ TRÌNH DUYỆT TRẢI NGHIỆM]
👉 Truy cập: http://localhost:3000
Tài khoản Admin: admin / admin123  (Vào quản trị tại /admin)
Tài khoản User : user  / user123   (Mua sắm, đặt hàng)
```

---

## 💻 YÊU CẦU MÔI TRƯỜNG MÁY TÍNH CẦN CÀI ĐẶT TRƯỚC

| Thành phần | Phiên bản yêu cầu | Link tải chính thức | Lệnh kiểm tra |
| :--- | :--- | :--- | :--- |
| **.NET SDK** | **.NET 8.0 SDK** | [dotnet.microsoft.com](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) | `dotnet --version` |
| **SQL Server** | 2019 hoặc 2022 (Developer / Express) | [microsoft.com/sql-server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) | Dịch vụ SQL Server đang chạy |
| **Node.js & npm** | Node.js v18.x hoặc v20.x LTS+ | [nodejs.org](https://nodejs.org/) | `node -v` và `npm -v` |
| **Trình duyệt** | Google Chrome, Edge hoặc Firefox | Khuyên dùng bản mới nhất | Mở web `http://localhost:3000` |

---

## 🚀 HƯỚNG DẪN CHI TIẾT TỪNG BƯỚC

### PHẦN 1: CÀI ĐẶT & CHẠY BACKEND (.NET 8)

#### Bước 1.1: Cấu hình chuỗi kết nối SQL Server
Mở file [`d:\web_gomsu_nhom5\appsettings.json`](file:///d:/web_gomsu_nhom5/appsettings.json), kiểm tra mục `ConnectionStrings:DefaultConnection`.
- Nếu dùng tài khoản Windows (Mặc định):
  ```json
  "DefaultConnection": "Server=localhost;Database=QuanLyBanDoGomSu;Trusted_Connection=True;TrustServerCertificate=True;"
  ```
- Nếu dùng tài khoản `sa`:
  ```json
  "DefaultConnection": "Server=localhost;Database=QuanLyBanDoGomSu;User Id=sa;Password=MậtKhẩuCủaBạn;TrustServerCertificate=True;"
  ```

#### Bước 1.2: Khởi chạy Backend Web API
Mở cửa sổ **PowerShell thứ nhất**, gõ các lệnh sau:
```powershell
cd d:\web_gomsu_nhom5
dotnet restore
dotnet build
dotnet run --urls "http://localhost:5000"
```

> 🌟 **LƯU Ý QUAN TRỌNG**: Bạn **KHÔNG CẦN** tạo trước database hay chạy câu lệnh SQL nào cả!  
> Ngay khi chạy lệnh `dotnet run`, chương trình sẽ tự động:
> 1. Tạo Database `QuanLyBanDoGomSu`.
> 2. Dựng toàn bộ **21 Bảng dữ liệu**.
> 3. Nạp danh mục 8 bảng chuẩn (Men, Kích thước, Màu sắc, Hình khối...).
> 4. Nạp **20 sản phẩm gốm sứ Bát Tràng mẫu**.
> 5. Nạp **2 Tài khoản kiểm thử** mặc định (`admin` và `user`).

Màn hình hiển thị:
```text
[DatabaseInitializer] Co so du lieu da san sang voi 20 mat hang mau va 2 tai khoan test mac dinh!
Now listening on: http://localhost:5000
```
👉 Bạn có thể kiểm tra danh sách API tại: **[http://localhost:5000/swagger](http://localhost:5000/swagger)**.

---

### PHẦN 2: CÀI ĐẶT & CHẠY FRONTEND (REACT 19 + VITE)

Mở cửa sổ **PowerShell thứ hai** (giữ nguyên cửa sổ Backend đang chạy), gõ các lệnh sau:

```powershell
# 1. Di chuyển vào thư mục Frontend
cd d:\Fe_gomsu_nhom5

# 2. Cài đặt các thư viện phụ thuộc
npm install

# 3. Khởi chạy máy chủ giao diện người dùng
npm run dev
```

Màn hình xuất hiện thông báo:
```text
  VITE v8.3.0  ready in 320 ms
  ➜  Local:   http://localhost:3000/
```

---

### PHẦN 3: TRẢI NGHIỆM VÀ KIỂM THỬ CÁC CHỨC NĂNG

Mở trình duyệt truy cập: 👉 **[http://localhost:3000](http://localhost:3000)**

#### 1. Kiểm thử phía Khách hàng (Customer):
- **Xem & Lọc sản phẩm**: Tìm kiếm theo từ khóa ("Ấm chén", "Bình hoa"), lọc theo loại men truyền thống (**Men rạn**, **Men lam**, **Men ngọc**...) hoặc khoảng giá.
- **Xem chi tiết gốm sứ**: Nhấp vào sản phẩm để xem kích thước, hình khối, công dụng, bảo hành, nghệ nhân.
- **Giỏ hàng & Đăng nhập thông minh**: Nhấn "Thêm vào giỏ", hệ thống sẽ bật Popup Đăng nhập nếu chưa đăng nhập. Nhập tài khoản `user` / `user123` để đăng nhập và tự động đưa món hàng vào giỏ.
- **Đặt hàng**: Điền thông tin giao hàng, chọn COD hoặc Quét mã VietQR chuyển khoản, bấm Đặt hàng để xem hiệu ứng pháo hoa chúc mừng.
- **Tra cứu đơn hàng**: Nhập Số Điện Thoại trong giỏ hàng (tab "Đơn Hàng Của Bạn") để xem toàn bộ lịch sử đơn hàng và tình trạng giao hàng.
- **Chat trực tuyến**: Bấm nút Chat ở góc dưới bên phải màn hình để nhắn tin tư vấn.

#### 2. Kiểm thử phía Quản trị viên (Admin):
- Đăng nhập với tài khoản: `admin` / `admin123`.
- Bấm vào nút **"Quản Trị"** trên Header (hoặc vào `http://localhost:3000/admin`):
  - **Thống kê doanh số**: Biểu đồ doanh thu Recharts, top 5 sản phẩm bán chạy.
  - **Quản lý sản phẩm**: Thêm mới gốm sứ, chỉnh sửa thông tin, xóa sản phẩm.
  - **Quản lý đơn hàng**: Xem chi tiết hóa đơn và chuyển trạng thái đơn hàng (`Chờ xác nhận` ➔ `Đang giao` ➔ `Đã giao`...).
  - **Trung tâm hỗ trợ**: Trả lời tin nhắn tư vấn của khách hàng trong thời gian thực.

---

## 🛠️ XỬ LÝ SỰ CỐ NHANH (TROUBLESHOOTING)

1. **Lỗi kết nối SQL Server**: Mở `services.msc`, kiểm tra dịch vụ `SQL Server (MSSQLSERVER)` đã được bật (`Running`) chưa.
2. **Lỗi Frontend không tải được sản phẩm**: Đảm bảo cửa sổ Backend đang chạy tại cổng `http://localhost:5000`.
3. **Cổng 3000 hoặc 5000 bị chiếm**:
   - Backend: Thêm tham số `--urls "http://localhost:5001"`.
   - Frontend: Vite sẽ tự động hỏi chuyển sang cổng `3001` (chọn `y`).
