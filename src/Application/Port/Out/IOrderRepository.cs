using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyBanDoGomSu.Domain.Entities;

namespace QuanLyBanDoGomSu.Application.Port.Out
{
    public interface IOrderRepository
    {
        Task<HoaDonBan> GetByIdAsync(string soHDB);
        Task<(List<HoaDonBan> Items, int TotalCount)> GetOrderHistoryAsync(string maKhach, int pageIndex, int pageSize);
        Task AddAsync(HoaDonBan order, List<ChiTietHoaDonBan> items);
        Task<string> GenerateSequentialOrderIdAsync();
    }
}
