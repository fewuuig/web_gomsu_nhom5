using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Exception;

namespace QuanLyBanDoGomSu.Presentation.Controllers.API
{
    [ApiController]
    [Route("api/v1/don-hang")]
    public class DonHangApiController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public DonHangApiController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Tạo đơn hàng bán mua sản phẩm gốm sứ
        /// POST /api/v1/don-hang
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationCustomException("Thông tin đơn hàng không hợp lệ");
            }

            var order = await _orderService.CreateOrderAsync(request);
            var response = ApiResponse<OrderDto>.SuccessResponse(
                order, $"Đặt hàng đồ gốm sứ thành công (Mã HĐ: {order.SoHDB})");

            return CreatedAtAction(nameof(GetById), new { id = order.SoHDB }, response);
        }

        /// <summary>
        /// Tra cứu chi tiết đơn hàng theo Mã Hóa Đơn Bán (SoHDB)
        /// GET /api/v1/don-hang/HDB202610040001
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            var response = ApiResponse<OrderDto>.SuccessResponse(
                order, $"Lấy chi tiết đơn hàng '{id}' thành công");

            return Ok(response);
        }

        /// <summary>
        /// Xem lịch sử các đơn hàng đã đặt (hỗ trợ lọc theo mã khách hàng, số điện thoại hoặc xem tất cả nếu để trống/ALL)
        /// GET /api/v1/don-hang/lich-su?maKhach=KH001&pageIndex=1&pageSize=10
        /// </summary>
        [HttpGet("lich-su")]
        public async Task<IActionResult> GetHistory([FromQuery] string? maKhach, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            string targetKhach = string.IsNullOrWhiteSpace(maKhach) ? "ALL" : maKhach.Trim();
            var history = await _orderService.GetOrderHistoryAsync(targetKhach, pageIndex, pageSize);
            var response = ApiResponse<PagedResultDto<OrderDto>>.SuccessResponse(
                history, $"Lấy lịch sử đơn hàng '{targetKhach}' thành công");

            return Ok(response);
        }
    }
}
