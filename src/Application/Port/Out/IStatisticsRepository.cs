using System;
using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.DTOs;

namespace QuanLyBanDoGomSu.Application.Port.Out
{
    public interface IStatisticsRepository
    {
        Task<DashboardStatisticsDto> GetSalesStatisticsAsync(DateTime startDate, DateTime endDate);
        Task<SalesReportDataDto> GetSalesReportDataAsync(DateTime startDate, DateTime endDate);
    }
}
