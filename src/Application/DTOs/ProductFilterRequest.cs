using System.Collections.Generic;

namespace QuanLyBanDoGomSu.Application.DTOs
{
    public class ProductFilterRequest
    {
        public string? Keyword { get; set; }
        public string? MaLoai { get; set; }
        public string? MaKichThuoc { get; set; }
        public string? MaLoaiMen { get; set; }
        public string? MaMauSac { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);
    }
}
