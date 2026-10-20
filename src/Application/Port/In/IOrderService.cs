using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.DTOs;

namespace QuanLyBanDoGomSu.Application.Port.In
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderDto> GetOrderByIdAsync(string soHDB);
        Task<PagedResultDto<OrderDto>> GetOrderHistoryAsync(string maKhach, int pageIndex = 1, int pageSize = 10);
    }
}
