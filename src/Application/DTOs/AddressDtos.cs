using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanDoGomSu.Application.DTOs
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string MaKhach { get; set; }
        public string TenNguoiNhan { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChiChiTiet { get; set; }
        public bool LaMacDinh { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class CreateAddressRequest
    {
        public string? MaKhach { get; set; }

        [Required(ErrorMessage = "Tên người nhận không được để trống")]
        [StringLength(100, ErrorMessage = "Tên người nhận tối đa 100 ký tự")]
        public string TenNguoiNhan { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại nhận hàng không đúng định dạng")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Địa chỉ chi tiết không được để trống")]
        [StringLength(255, ErrorMessage = "Địa chỉ chi tiết tối đa 255 ký tự")]
        public string DiaChiChiTiet { get; set; }

        public bool LaMacDinh { get; set; } = false;
    }

    public class UpdateAddressRequest
    {
        [Required(ErrorMessage = "Tên người nhận không được để trống")]
        [StringLength(100, ErrorMessage = "Tên người nhận tối đa 100 ký tự")]
        public string TenNguoiNhan { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại nhận hàng không đúng định dạng")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Địa chỉ chi tiết không được để trống")]
        [StringLength(255, ErrorMessage = "Địa chỉ chi tiết tối đa 255 ký tự")]
        public string DiaChiChiTiet { get; set; }

        public bool LaMacDinh { get; set; } = false;
    }
}
