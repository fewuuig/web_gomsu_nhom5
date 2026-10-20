using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyBanDoGomSu.Domain.Entities;

namespace QuanLyBanDoGomSu.Application.Port.Out
{
    public interface IDanhMucHangHoaRepository
    {
        Task<DanhMucHangHoa> GetByIdAsync(string maHangHoa);
        Task<(List<DanhMucHangHoa> Items, int TotalCount)> GetPagedAsync(
            string keyword, string maLoai, string maKichThuoc, string maLoaiMen, string maMauSac, 
            decimal? minPrice, decimal? maxPrice, int pageIndex, int pageSize);
        Task AddAsync(DanhMucHangHoa entity);
        Task UpdateAsync(DanhMucHangHoa entity);
        Task DeleteAsync(string maHangHoa);
        Task<bool> ExistsAsync(string maHangHoa);
    }
}
