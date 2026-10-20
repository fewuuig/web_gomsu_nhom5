using System;
using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.DTOs;

namespace QuanLyBanDoGomSu.Application.Port.In
{
    public interface IStatisticsService
    {
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync(DateTime? startDate, DateTime? endDate);
        Task<(byte[] FileBytes, string FileName)> ExportSalesReportExcelAsync(DateTime? startDate, DateTime? endDate);
    }
}
