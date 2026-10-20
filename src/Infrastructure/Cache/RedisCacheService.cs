using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QuanLyBanDoGomSu.Application.Port.Out;

namespace QuanLyBanDoGomSu.Infrastructure.Cache
{
    /// <summary>
    /// Implemented Redis Cache service seam.
    /// Kích hoạt khi Redis:Enabled = true trong appsettings.json.
    /// Địa chỉ Redis Server: localhost:6379 (http://localhost:6379).
    /// BẮT BUỘC toàn bộ key trong Redis phải bắt đầu bằng tiền tố 'GomSuNhom5:' 
    /// để tránh đè trùng lặp dữ liệu với các dự án khác trên cùng Redis server.
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private const string KeyPrefix = "GomSuNhom5:";
        private readonly string _connectionString;

        public RedisCacheService(IConfiguration configuration)
        {
            _connectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379";
        }

        private string FormatKey(string key) => key.StartsWith(KeyPrefix) ? key : $"{KeyPrefix}{key}";

        public Task<T?> GetAsync<T>(string key)
        {
            var fullKey = FormatKey(key);
            // Sẵn sàng kết nối StackExchange.Redis ConnectionMultiplexer khi Enabled: true
            // Hiện tại để ngỏ Seam chuẩn bị cho Redis
            return Task.FromResult(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var fullKey = FormatKey(key);
            // Sẵn sàng lưu vào Redis với tiền tố GomSuNhom5:
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            var fullKey = FormatKey(key);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefixKey)
        {
            var fullPrefix = FormatKey(prefixKey);
            return Task.CompletedTask;
        }
    }
}
