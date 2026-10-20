using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Exception;

namespace QuanLyBanDoGomSu.Presentation.Controllers.API
{
    [ApiController]
    [Route("api/v1/addresses")]
    public class AddressApiController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressApiController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// Lấy danh sách địa chỉ nhận hàng của khách hàng
        /// GET /api/v1/addresses?maKhach=KHUSER
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAddresses([FromQuery] string maKhach)
        {
            if (string.IsNullOrWhiteSpace(maKhach))
            {
                return BadRequest(ApiResponse<List<AddressDto>>.ErrorResponse(400, "Vui lòng cung cấp mã khách hàng."));
            }

            var addresses = await _addressService.GetAddressesByCustomerAsync(maKhach);
            var response = ApiResponse<List<AddressDto>>.SuccessResponse(
                addresses, "Lấy danh sách địa chỉ nhận hàng thành công.");

            return Ok(response);
        }

        /// <summary>
        /// Thêm mới một địa chỉ nhận hàng vào sổ địa chỉ
        /// POST /api/v1/addresses
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationCustomException("Dữ liệu địa chỉ nhận hàng không hợp lệ.");
            }

            var address = await _addressService.CreateAddressAsync(request.MaKhach ?? string.Empty, request);
            var response = ApiResponse<AddressDto>.SuccessResponse(
                address, "Thêm địa chỉ nhận hàng mới thành công.");

            return CreatedAtAction(nameof(GetAddresses), new { maKhach = address.MaKhach }, response);
        }

        /// <summary>
        /// Cập nhật địa chỉ nhận hàng
        /// PUT /api/v1/addresses/{id}?maKhach=KHUSER
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAddress([FromRoute] int id, [FromQuery] string maKhach, [FromBody] UpdateAddressRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationCustomException("Dữ liệu cập nhật địa chỉ không hợp lệ.");
            }

            var address = await _addressService.UpdateAddressAsync(id, maKhach, request);
            var response = ApiResponse<AddressDto>.SuccessResponse(
                address, "Cập nhật địa chỉ nhận hàng thành công.");

            return Ok(response);
        }

        /// <summary>
        /// Đặt địa chỉ làm mặc định
        /// PUT /api/v1/addresses/{id:int}/default?maKhach=KHUSER
        /// </summary>
        [HttpPut("{id:int}/default")]
        public async Task<IActionResult> SetDefault([FromRoute] int id, [FromQuery] string maKhach)
        {
            if (string.IsNullOrWhiteSpace(maKhach))
            {
                return BadRequest(ApiResponse<AddressDto>.ErrorResponse(400, "Vui lòng cung cấp mã khách hàng."));
            }

            var address = await _addressService.SetDefaultAddressAsync(id, maKhach);
            var response = ApiResponse<AddressDto>.SuccessResponse(
                address, "Đã đặt địa chỉ làm mặc định thành công.");

            return Ok(response);
        }

        /// <summary>
        /// Xóa địa chỉ nhận hàng
        /// DELETE /api/v1/addresses/{id:int}?maKhach=KHUSER
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int id, [FromQuery] string maKhach)
        {
            if (string.IsNullOrWhiteSpace(maKhach))
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(400, "Vui lòng cung cấp mã khách hàng."));
            }

            var success = await _addressService.DeleteAddressAsync(id, maKhach);
            var response = ApiResponse<bool>.SuccessResponse(
                success, "Đã xóa địa chỉ nhận hàng thành công.");

            return Ok(response);
        }
    }
}
