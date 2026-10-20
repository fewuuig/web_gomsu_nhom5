using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.Port.Out;

namespace QuanLyBanDoGomSu.Infrastructure.Cache
{
    /// <summary>
    /// Cache trong bộ nhớ tạm thời khi Redis chưa kích hoạt (Enabled = false).
    /// Tất cả key tự động được gắn tiền tố 'GomSuNhom5:' để đồng bộ chuẩn đặt tên key.
    /// </summary>
    public class InMemoryCacheService : ICacheService
    {
        private const string KeyPrefix = "GomSuNhom5:";
        private readonly ConcurrentDictionary<string, (object Value, DateTime? Expiration)> _cache = new();

        private string FormatKey(string key) => key.StartsWith(KeyPrefix) ? key : $"{KeyPrefix}{key}";

        public Task<T?> GetAsync<T>(string key)
        {
            var fullKey = FormatKey(key);
            if (_cache.TryGetValue(fullKey, out var item))
            {
                if (item.Expiration == null || item.Expiration > DateTime.UtcNow)
                {
                    return Task.FromResult((T?)item.Value);
                }
                _cache.TryRemove(fullKey, out _);
            }
            return Task.FromResult(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var fullKey = FormatKey(key);
            DateTime? expTime = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null;
            if (value != null)
            {
                _cache[fullKey] = (value, expTime);
            }
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            var fullKey = FormatKey(key);
            _cache.TryRemove(fullKey, out _);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefixKey)
        {
            var fullPrefix = FormatKey(prefixKey);
            var keysToRemove = _cache.Keys.Where(k => k.StartsWith(fullPrefix)).ToList();
            foreach (var k in keysToRemove)
            {
                _cache.TryRemove(k, out _);
            }
            return Task.CompletedTask;
        }
    }
}
