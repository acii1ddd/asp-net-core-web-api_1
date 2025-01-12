using StackExchange.Redis;
using System.Text.Json;

namespace BookAPI.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IConnectionMultiplexer redisConnection, ILogger<CacheService> logger)
        {
            _cacheDb = redisConnection.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var value = await _cacheDb.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                // null для class (ссылочных типов)
                return default;
            }

            _logger.LogInformation($"Data for key {key} returns successfully");
            return JsonSerializer.Deserialize<T>(value);
        }

        public Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var lifeTime = expirationTime.Subtract(DateTimeOffset.Now);
            _logger.LogInformation($"Data for key {key} cached successfully");
            return _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), lifeTime);
        }

        public async Task<bool> RemoveDataAsync<T>(string key)
        {
            var isExists = await _cacheDb.KeyExistsAsync(key);

            // delete and return true if key exists, false otherwise
            _logger.LogInformation($"Data for key {key} removed successfully");
            return isExists ? await _cacheDb.KeyDeleteAsync(key) : false;
        }
    }
}
