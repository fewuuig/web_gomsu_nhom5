using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Application.Port.Out;

namespace QuanLyBanDoGomSu.Application.Service
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _repository;

        // Bảng màu nhận diện thương hiệu Gốm Sứ Bát Tràng
        private static readonly XLColor BrandPrimary = XLColor.FromArgb(109, 36, 23);     // Đỏ men huyết dụ #6D2417
        private static readonly XLColor BrandSecondary = XLColor.FromArgb(141, 50, 31);   // Đỏ nung #8D321F
        private static readonly XLColor CardBackground = XLColor.FromArgb(248, 239, 234); // Kem gốm ấm #F8EFEA
        private static readonly XLColor ZebraLight = XLColor.FromArgb(253, 250, 248);     // Sọc xen kẽ #FDFAF8
        private static readonly XLColor BorderGray = XLColor.FromArgb(215, 204, 200);     // Viền nâu gốm nhạt #D7CCC8

        public StatisticsService(IStatisticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);
            var start = startDate ?? DateTime.Today.AddDays(-30);

            return await _repository.GetSalesStatisticsAsync(start, end);
        }

        public async Task<(byte[] FileBytes, string FileName)> ExportSalesReportExcelAsync(DateTime? startDate, DateTime? endDate)
        {
            // Thiết lập khoảng ngày mặc định nếu không truyền vào
            var end = endDate ?? DateTime.Today.AddDays(1).AddTicks(-1);
            var start = startDate ?? DateTime.Today.AddDays(-30);

            var reportData = await _repository.GetSalesReportDataAsync(start, end);

            using var workbook = new XLWorkbook();

            // ----------------------------------------------------
            // SHEET 1: TỔNG QUAN DOANH THU
            // ----------------------------------------------------
            CreateOverviewSheet(workbook, reportData, start, end);

            // ----------------------------------------------------
            // SHEET 2: CHI TIẾT ĐƠN HÀNG
            // ----------------------------------------------------
            CreateOrdersDetailSheet(workbook, reportData, start, end);

            // ----------------------------------------------------
            // SHEET 3: TOP SẢN PHẨM BÁN CHẠY
            // ----------------------------------------------------
            CreateTopProductsSheet(workbook, reportData, start, end);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"BaoCao_DoanhSo_BatTrang_{start:yyyyMMdd}_{end:yyyyMMdd}.xlsx";
            return (stream.ToArray(), fileName);
        }

        private static void CreateOverviewSheet(XLWorkbook workbook, SalesReportDataDto data, DateTime start, DateTime end)
        {
            var ws = workbook.Worksheets.Add("Tổng Quan Doanh Thu");
            ws.ShowGridLines = true;

            // 1. Tiêu đề thương hiệu
            ws.Cell("B2").Value = "GỐM SỨ BÁT TRÀNG - BÁO CÁO DOANH SỐ KINH DOANH";
            ws.Cell("B2").Style.Font.Bold = true;
            ws.Cell("B2").Style.Font.FontSize = 16;
            ws.Cell("B2").Style.Font.FontColor = BrandPrimary;
            ws.Range("B2:F2").Merge();

            ws.Cell("B3").Value = $"Kỳ báo cáo: Từ ngày {start:dd/MM/yyyy} đến ngày {end:dd/MM/yyyy}  |  Ngày xuất file: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cell("B3").Style.Font.Italic = true;
            ws.Cell("B3").Style.Font.FontSize = 10;
            ws.Cell("B3").Style.Font.FontColor = XLColor.DimGray;
            ws.Range("B3:F3").Merge();

            // 2. Khối thẻ KPI Tổng hợp (Cards)
            int kpiRowHeader = 5;
            int kpiRowValue = 6;

            // Cột B: Doanh thu
            SetupKpiCard(ws, "B", kpiRowHeader, kpiRowValue, "TỔNG DOANH THU", data.Summary.TongDoanhThu, true);
            // Cột C: Đơn hàng
            SetupKpiCard(ws, "C", kpiRowHeader, kpiRowValue, "TỔNG ĐƠN HÀNG", data.Summary.TongDonHang, false);
            // Cột D: Sản phẩm đã bán
            SetupKpiCard(ws, "D", kpiRowHeader, kpiRowValue, "SẢN PHẨM ĐÃ BÁN", data.Summary.TongSanPhamDaBan, false);
            // Cột E: Khách hàng
            SetupKpiCard(ws, "E", kpiRowHeader, kpiRowValue, "KHÁCH HÀNG GIAO DỊCH", data.Summary.TongKhachHang, false);

            // 3. Bảng Doanh số theo ngày
            int tableStartRow = 9;
            ws.Cell(tableStartRow, 2).Value = "THỐNG KÊ DOANH THU THEO TỪNG NGÀY";
            ws.Cell(tableStartRow, 2).Style.Font.Bold = true;
            ws.Cell(tableStartRow, 2).Style.Font.FontSize = 12;
            ws.Cell(tableStartRow, 2).Style.Font.FontColor = BrandSecondary;
            ws.Range(tableStartRow, 2, tableStartRow, 6).Merge();

            int headerRow = 10;
            string[] headers = { "STT", "Ngày Ghi Nhận", "Số Đơn Hàng", "Doanh Thu (VNĐ)", "TB / Đơn Hàng" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = BrandPrimary;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            ws.Row(headerRow).Height = 24;

            int currentRow = headerRow + 1;
            int stt = 1;
            foreach (var item in data.Summary.DoanhThuTheoNgay)
            {
                ws.Cell(currentRow, 2).Value = stt++;
                ws.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Ngày
                if (DateTime.TryParse(item.Ngay, out var dt))
                {
                    ws.Cell(currentRow, 3).Value = dt.ToString("dd/MM/yyyy");
                }
                else
                {
                    ws.Cell(currentRow, 3).Value = item.Ngay;
                }
                ws.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Số đơn
                ws.Cell(currentRow, 4).Value = item.SoDonHang;
                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0";
                ws.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Doanh thu
                ws.Cell(currentRow, 5).Value = item.DoanhThu;
                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0 \"₫\"";
                ws.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // TB / đơn
                decimal avg = item.SoDonHang > 0 ? Math.Round(item.DoanhThu / item.SoDonHang, 0) : 0;
                ws.Cell(currentRow, 6).Value = avg;
                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0 \"₫\"";
                ws.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // Zebra striping
                if (stt % 2 == 0)
                {
                    ws.Range(currentRow, 2, currentRow, 6).Style.Fill.BackgroundColor = ZebraLight;
                }

                currentRow++;
            }

            // Hàng Tổng Cộng
            if (currentRow > headerRow + 1)
            {
                ws.Cell(currentRow, 2).Value = "TỔNG CỘNG";
                ws.Range(currentRow, 2, currentRow, 3).Merge();
                ws.Range(currentRow, 2, currentRow, 3).Style.Font.Bold = true;
                ws.Range(currentRow, 2, currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 4).FormulaA1 = $"SUM(D{headerRow + 1}:D{currentRow - 1})";
                ws.Cell(currentRow, 4).Style.Font.Bold = true;
                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0";
                ws.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 5).FormulaA1 = $"SUM(E{headerRow + 1}:E{currentRow - 1})";
                ws.Cell(currentRow, 5).Style.Font.Bold = true;
                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0 \"₫\"";
                ws.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 6).FormulaA1 = $"IF(D{currentRow}>0, E{currentRow}/D{currentRow}, 0)";
                ws.Cell(currentRow, 6).Style.Font.Bold = true;
                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0 \"₫\"";
                ws.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Range(currentRow, 2, currentRow, 6).Style.Fill.BackgroundColor = CardBackground;
                ws.Range(currentRow, 2, currentRow, 6).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(currentRow, 2, currentRow, 6).Style.Border.BottomBorder = XLBorderStyleValues.Double;
            }

            // Format Borders cho toàn bộ bảng
            var tableRange = ws.Range(headerRow, 2, Math.Max(currentRow, headerRow + 1), 6);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            tableRange.Style.Border.OutsideBorderColor = BrandPrimary;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorderColor = BorderGray;

            ws.Columns(2, 6).AdjustToContents();
            EnsureMinColumnWidth(ws, 2, 6, 16);
        }

        private static void CreateOrdersDetailSheet(XLWorkbook workbook, SalesReportDataDto data, DateTime start, DateTime end)
        {
            var ws = workbook.Worksheets.Add("Chi Tiết Đơn Hàng");
            ws.ShowGridLines = true;

            // Header
            ws.Cell("B2").Value = "DANH SÁCH CHI TIẾT HÓA ĐƠN BÁN HÀNG";
            ws.Cell("B2").Style.Font.Bold = true;
            ws.Cell("B2").Style.Font.FontSize = 15;
            ws.Cell("B2").Style.Font.FontColor = BrandPrimary;
            ws.Range("B2:H2").Merge();

            ws.Cell("B3").Value = $"Kỳ báo cáo: Từ {start:dd/MM/yyyy} đến {end:dd/MM/yyyy}  |  Tổng số: {data.Orders.Count} hóa đơn";
            ws.Cell("B3").Style.Font.Italic = true;
            ws.Cell("B3").Style.Font.FontSize = 10;
            ws.Cell("B3").Style.Font.FontColor = XLColor.DimGray;
            ws.Range("B3:H3").Merge();

            int headerRow = 5;
            string[] headers = { "STT", "Mã Hóa Đơn", "Ngày Lập", "Khách Hàng", "Số Điện Thoại", "Nhân Viên Lập", "Tổng Tiền (VNĐ)" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = BrandPrimary;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            ws.Row(headerRow).Height = 24;

            int currentRow = headerRow + 1;
            int stt = 1;
            foreach (var order in data.Orders)
            {
                ws.Cell(currentRow, 2).Value = stt++;
                ws.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 3).Value = order.SoHDB;
                ws.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(currentRow, 3).Style.Font.Bold = true;

                ws.Cell(currentRow, 4).Value = order.NgayBan.ToString("dd/MM/yyyy");
                ws.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 5).Value = order.TenKhachHang;
                ws.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell(currentRow, 6).Value = order.DienThoaiKhach;
                ws.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 7).Value = order.TenNhanVien;
                ws.Cell(currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell(currentRow, 8).Value = order.TongTien;
                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0 \"₫\"";
                ws.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                if (stt % 2 == 0)
                {
                    ws.Range(currentRow, 2, currentRow, 8).Style.Fill.BackgroundColor = ZebraLight;
                }

                currentRow++;
            }

            // Hàng Tổng Cộng
            if (currentRow > headerRow + 1)
            {
                ws.Cell(currentRow, 2).Value = "TỔNG CỘNG DOANH THU";
                ws.Range(currentRow, 2, currentRow, 7).Merge();
                ws.Range(currentRow, 2, currentRow, 7).Style.Font.Bold = true;
                ws.Range(currentRow, 2, currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 8).FormulaA1 = $"SUM(H{headerRow + 1}:H{currentRow - 1})";
                ws.Cell(currentRow, 8).Style.Font.Bold = true;
                ws.Cell(currentRow, 8).Style.Font.FontSize = 11;
                ws.Cell(currentRow, 8).Style.Font.FontColor = BrandPrimary;
                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0 \"₫\"";
                ws.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Range(currentRow, 2, currentRow, 8).Style.Fill.BackgroundColor = CardBackground;
                ws.Range(currentRow, 2, currentRow, 8).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(currentRow, 2, currentRow, 8).Style.Border.BottomBorder = XLBorderStyleValues.Double;
            }

            var tableRange = ws.Range(headerRow, 2, Math.Max(currentRow, headerRow + 1), 8);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            tableRange.Style.Border.OutsideBorderColor = BrandPrimary;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorderColor = BorderGray;

            ws.Columns(2, 8).AdjustToContents();
            EnsureMinColumnWidth(ws, 2, 8, 14);
        }

        private static void CreateTopProductsSheet(XLWorkbook workbook, SalesReportDataDto data, DateTime start, DateTime end)
        {
            var ws = workbook.Worksheets.Add("Top Sản Phẩm Bán Chạy");
            ws.ShowGridLines = true;

            // Header
            ws.Cell("B2").Value = "BẢNG XẾP HẠNG SẢN PHẨM GỐM SỨ BÁN CHẠY";
            ws.Cell("B2").Style.Font.Bold = true;
            ws.Cell("B2").Style.Font.FontSize = 15;
            ws.Cell("B2").Style.Font.FontColor = BrandPrimary;
            ws.Range("B2:F2").Merge();

            ws.Cell("B3").Value = $"Kỳ báo cáo: Từ {start:dd/MM/yyyy} đến {end:dd/MM/yyyy}  |  Tổng số: {data.TopProducts.Count} sản phẩm phát sinh doanh số";
            ws.Cell("B3").Style.Font.Italic = true;
            ws.Cell("B3").Style.Font.FontSize = 10;
            ws.Cell("B3").Style.Font.FontColor = XLColor.DimGray;
            ws.Range("B3:F3").Merge();

            int headerRow = 5;
            string[] headers = { "STT", "Mã Hàng", "Tên Sản Phẩm Gốm Sứ", "Số Lượng Đã Bán", "Tổng Doanh Thu (VNĐ)" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = BrandPrimary;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            ws.Row(headerRow).Height = 24;

            int currentRow = headerRow + 1;
            int stt = 1;
            foreach (var prod in data.TopProducts)
            {
                ws.Cell(currentRow, 2).Value = stt++;
                ws.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 3).Value = prod.MaHangHoa;
                ws.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(currentRow, 3).Style.Font.Bold = true;

                ws.Cell(currentRow, 4).Value = prod.TenHangHoa;
                ws.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell(currentRow, 5).Value = prod.SoLuongDaBan;
                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0";
                ws.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 6).Value = prod.TongTien;
                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0 \"₫\"";
                ws.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                if (stt % 2 == 0)
                {
                    ws.Range(currentRow, 2, currentRow, 6).Style.Fill.BackgroundColor = ZebraLight;
                }

                currentRow++;
            }

            // Hàng Tổng Cộng
            if (currentRow > headerRow + 1)
            {
                ws.Cell(currentRow, 2).Value = "TỔNG CỘNG";
                ws.Range(currentRow, 2, currentRow, 4).Merge();
                ws.Range(currentRow, 2, currentRow, 4).Style.Font.Bold = true;
                ws.Range(currentRow, 2, currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 5).FormulaA1 = $"SUM(E{headerRow + 1}:E{currentRow - 1})";
                ws.Cell(currentRow, 5).Style.Font.Bold = true;
                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0";
                ws.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(currentRow, 6).FormulaA1 = $"SUM(F{headerRow + 1}:F{currentRow - 1})";
                ws.Cell(currentRow, 6).Style.Font.Bold = true;
                ws.Cell(currentRow, 6).Style.Font.FontSize = 11;
                ws.Cell(currentRow, 6).Style.Font.FontColor = BrandPrimary;
                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0 \"₫\"";
                ws.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Range(currentRow, 2, currentRow, 6).Style.Fill.BackgroundColor = CardBackground;
                ws.Range(currentRow, 2, currentRow, 6).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(currentRow, 2, currentRow, 6).Style.Border.BottomBorder = XLBorderStyleValues.Double;
            }

            var tableRange = ws.Range(headerRow, 2, Math.Max(currentRow, headerRow + 1), 6);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            tableRange.Style.Border.OutsideBorderColor = BrandPrimary;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorderColor = BorderGray;

            ws.Columns(2, 6).AdjustToContents();
            EnsureMinColumnWidth(ws, 2, 6, 16);
        }

        private static void SetupKpiCard(IXLWorksheet ws, string col, int rowTitle, int rowVal, string title, decimal value, bool isCurrency)
        {
            var cellTitle = ws.Cell($"{col}{rowTitle}");
            cellTitle.Value = title;
            cellTitle.Style.Font.Bold = true;
            cellTitle.Style.Font.FontSize = 9;
            cellTitle.Style.Font.FontColor = BrandSecondary;
            cellTitle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var cellVal = ws.Cell($"{col}{rowVal}");
            cellVal.Value = value;
            cellVal.Style.Font.Bold = true;
            cellVal.Style.Font.FontSize = 13;
            cellVal.Style.Font.FontColor = BrandPrimary;
            cellVal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cellVal.Style.NumberFormat.Format = isCurrency ? "#,##0 \"₫\"" : "#,##0";

            var cardRange = ws.Range($"{col}{rowTitle}:{col}{rowVal}");
            cardRange.Style.Fill.BackgroundColor = CardBackground;
            cardRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cardRange.Style.Border.OutsideBorderColor = BorderGray;
        }

        private static void EnsureMinColumnWidth(IXLWorksheet ws, int startCol, int endCol, double minWidth)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                if (ws.Column(col).Width < minWidth)
                {
                    ws.Column(col).Width = minWidth;
                }
            }
        }
    }
}
