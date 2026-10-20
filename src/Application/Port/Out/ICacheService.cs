using System;
using System.Threading.Tasks;

namespace QuanLyBanDoGomSu.Application.Port.Out
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefixKey);
    }
}
