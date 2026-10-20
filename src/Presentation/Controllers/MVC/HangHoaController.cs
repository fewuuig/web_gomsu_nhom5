using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;

namespace QuanLyBanDoGomSu.Presentation.Controllers.MVC
{
    public class HangHoaController : Controller
    {
        private readonly IDanhMucHangHoaService _service;
        private const string CART_SESSION_KEY = "PotteryStore_CartSession";
        private const string USER_COOKIE_KEY = "PotteryStore_UserCookie";

        public HangHoaController(IDanhMucHangHoaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Giao diện danh sách sản phẩm Gốm Sứ (Phân trang, lọc, tìm kiếm MVC)
        /// </summary>
        public async Task<IActionResult> Index(ProductFilterRequest request)
        {
            var model = await _service.FilterAsync(request);
            ViewBag.Filter = request;

            // Lưu thông tin lượt xem vào Cookie
            Response.Cookies.Append("LastVisitedPage", "DanhMucHangHoa", new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7),
                HttpOnly = true
            });

            return View(model);
        }

        /// <summary>
        /// Giao diện chi tiết sản phẩm đồ gốm sứ
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            var product = await _service.GetByIdAsync(id);
            return View(product);
        }

        /// <summary>
        /// Thêm hàng hóa vào giỏ hàng (Làm việc với Session)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToCart(string maHangHoa, int soLuong = 1)
        {
            var product = await _service.GetByIdAsync(maHangHoa);
            var cart = GetCartFromSession();

            var item = cart.Find(i => i.MaHangHoa == maHangHoa);
            if (item != null)
            {
                item.SoLuong += soLuong;
            }
            else
            {
                cart.Add(new CartItemDto
                {
                    MaHangHoa = product.MaHangHoa,
                    TenHangHoa = product.TenHangHoa,
                    DonGiaBan = product.DonGiaBan,
                    SoLuong = soLuong
                });
            }

            SaveCartToSession(cart);
            TempData["SuccessMessage"] = $"Đã thêm '{product.TenHangHoa}' vào giỏ hàng!";

            return RedirectToAction(nameof(Index));
        }

        private List<CartItemDto> GetCartFromSession()
        {
            var json = HttpContext.Session.GetString(CART_SESSION_KEY);
            return string.IsNullOrEmpty(json) 
                ? new List<CartItemDto>() 
                : JsonSerializer.Deserialize<List<CartItemDto>>(json);
        }

        private void SaveCartToSession(List<CartItemDto> cart)
        {
            var json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CART_SESSION_KEY, json);
        }
    }

    public class CartItemDto
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public decimal DonGiaBan { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGiaBan * SoLuong;
    }
}
