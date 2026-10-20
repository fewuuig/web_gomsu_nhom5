using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Exception;

namespace QuanLyBanDoGomSu.Presentation.Controllers.API
{
    [ApiController]
    [Route("api/v1/hang-hoa")]
    public class HangHoaApiController : ControllerBase
    {
        private readonly IDanhMucHangHoaService _service;

        public HangHoaApiController(IDanhMucHangHoaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách hàng hóa đồ gốm sứ có lọc, tìm kiếm và phân trang (Dành cho AJAX)
        /// GET /api/v1/hang-hoa?keyword=binh&maLoai=L01&pageIndex=1&pageSize=10
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFilterPaged([FromQuery] ProductFilterRequest request)
        {
            var pagedResult = await _service.FilterAsync(request);
            var response = ApiResponse<PagedResultDto<DanhMucHangHoaDto>>.SuccessResponse(
                pagedResult, "Lấy danh sách hàng hóa gốm sứ thành công");

            return Ok(response);
        }

        /// <summary>
        /// Lấy chi tiết hàng hóa theo Mã Hàng Hoa
        /// GET /api/v1/hang-hoa/HH001
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var product = await _service.GetByIdAsync(id);
            var response = ApiResponse<DanhMucHangHoaDto>.SuccessResponse(
                product, $"Lấy chi tiết hàng hóa '{id}' thành công");

            return Ok(response);
        }

        /// <summary>
        /// Thêm mới hàng hóa đồ gốm sứ vào danh mục
        /// POST /api/v1/hang-hoa
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DanhMucHangHoaDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationCustomException("Dữ liệu hàng hóa nhập vào không hợp lệ");
            }

            var created = await _service.CreateAsync(dto);
            var response = ApiResponse<DanhMucHangHoaDto>.SuccessResponse(
                created, "Thêm mới hàng hóa đồ gốm sứ thành công");

            return CreatedAtAction(nameof(GetById), new { id = created.MaHangHoa }, response);
        }

        /// <summary>
        /// Cập nhật thông tin hàng hóa đồ gốm sứ
        /// PUT /api/v1/hang-hoa/HH001
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] DanhMucHangHoaDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationCustomException("Dữ liệu cập nhật không hợp lệ");
            }

            var updated = await _service.UpdateAsync(id, dto);
            var response = ApiResponse<DanhMucHangHoaDto>.SuccessResponse(
                updated, $"Cập nhật hàng hóa '{id}' thành công");

            return Ok(response);
        }

        /// <summary>
        /// Xóa hàng hóa đồ gốm sứ khỏi danh mục
        /// DELETE /api/v1/hang-hoa/HH001
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            await _service.DeleteAsync(id);
            var response = ApiResponse<string>.SuccessResponse(
                id, $"Xóa thành công hàng hóa '{id}' khỏi hệ thống");

            return Ok(response);
        }
    }
}
