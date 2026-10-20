using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.DTOs;

namespace QuanLyBanDoGomSu.Application.Port.In
{
    public interface IAddressService
    {
        Task<List<AddressDto>> GetAddressesByCustomerAsync(string maKhach);
        Task<AddressDto> CreateAddressAsync(string maKhach, CreateAddressRequest request);
        Task<AddressDto> UpdateAddressAsync(int id, string maKhach, UpdateAddressRequest request);
        Task<bool> DeleteAddressAsync(int id, string maKhach);
        Task<AddressDto> SetDefaultAddressAsync(int id, string maKhach);
    }
}
