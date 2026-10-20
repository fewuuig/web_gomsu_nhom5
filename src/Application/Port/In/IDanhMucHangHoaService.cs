using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.DTOs;

namespace QuanLyBanDoGomSu.Application.Port.In
{
    public interface IDanhMucHangHoaService
    {
        Task<DanhMucHangHoaDto> GetByIdAsync(string maHangHoa);
        Task<PagedResultDto<DanhMucHangHoaDto>> FilterAsync(ProductFilterRequest request);
        Task<DanhMucHangHoaDto> CreateAsync(DanhMucHangHoaDto dto);
        Task<DanhMucHangHoaDto> UpdateAsync(string maHangHoa, DanhMucHangHoaDto dto);
        Task DeleteAsync(string maHangHoa);
    }
}
