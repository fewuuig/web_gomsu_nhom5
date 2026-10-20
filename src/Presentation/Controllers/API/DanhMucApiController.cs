using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Exception;
using QuanLyBanDoGomSu.Infrastructure.Persistence;

namespace QuanLyBanDoGomSu.Presentation.Controllers.API
{
    [ApiController]
    [Route("api/v1/danhmuc")]
    public class DanhMucApiController : ControllerBase
    {
        private readonly QuanLyBanDoGomSuDbContext _context;

        public DanhMucApiController(QuanLyBanDoGomSuDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách loại sản phẩm đồ gốm sứ
        /// GET /api/v1/danhmuc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllLoai()
        {
            var loais = await _context.Loais.AsNoTracking().ToListAsync();
            var response = ApiResponse<object>.SuccessResponse(loais, "Lấy danh mục loại sản phẩm thành công");
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách loại men
        /// GET /api/v1/danhmuc/loai-men
        /// </summary>
        [HttpGet("loai-men")]
        public async Task<IActionResult> GetAllLoaiMen()
        {
            var loaiMens = await _context.LoaiMens.AsNoTracking().ToListAsync();
            var response = ApiResponse<object>.SuccessResponse(loaiMens, "Lấy danh mục loại men thành công");
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách màu sắc
        /// GET /api/v1/danhmuc/mau-sac
        /// </summary>
        [HttpGet("mau-sac")]
        public async Task<IActionResult> GetAllMauSac()
        {
            var mauSacs = await _context.MauSacs.AsNoTracking().ToListAsync();
            var response = ApiResponse<object>.SuccessResponse(mauSacs, "Lấy danh mục màu sắc thành công");
            return Ok(response);
        }
    }
}
