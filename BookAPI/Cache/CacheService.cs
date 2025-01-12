using StackExchange.Redis;
using System.Text.Json;

namespace BookAPI.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;

        public CacheService(IConnectionMultiplexer redisConnection)
        {
            _cacheDb = redisConnection.GetDatabase();
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var value = await _cacheDb.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                // null для class (ссылочных типов)
                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }

        public Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var lifeTime = expirationTime.Subtract(DateTimeOffset.Now);
            return _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), lifeTime);
        }

        public async Task<bool> RemoveDataAsync<T>(string key)
        {
            var isExists = await _cacheDb.KeyExistsAsync(key);
            
            // delete and return true if key exists, false otherwise
            return isExists ? await _cacheDb.KeyDeleteAsync(key) : false;
        }
    }
}
