using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Exception;

namespace QuanLyBanDoGomSu.Presentation.Controllers.API
{
    [ApiController]
    [Route("api/v1/thong-ke")]
    public class ThongKeApiController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public ThongKeApiController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// Lấy số liệu thống kê doanh số cho Dashboard quản trị
        /// GET /api/v1/thong-ke/doanh-so?startDate=2026-09-01&endDate=2026-10-04
        /// </summary>
        [HttpGet("doanh-so")]
        public async Task<IActionResult> GetSalesStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var data = await _statisticsService.GetDashboardStatisticsAsync(startDate, endDate);
            var response = ApiResponse<DashboardStatisticsDto>.SuccessResponse(
                data, "Lấy dữ liệu thống kê doanh số Dashboard thành công");

            return Ok(response);
        }

        /// <summary>
        /// Xuất báo cáo doanh số ra file Excel (.xlsx) theo khoảng ngày hoặc theo tháng
        /// GET /api/v1/thong-ke/xuat-excel?startDate=2026-09-01&endDate=2026-10-04
        /// </summary>
        [HttpGet("xuat-excel")]
        public async Task<IActionResult> ExportSalesReportExcel([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var (fileBytes, fileName) = await _statisticsService.ExportSalesReportExcelAsync(startDate, endDate);
            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}
