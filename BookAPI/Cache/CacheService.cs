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
                _logger.LogWarning($"Data for key {key} is null!");
                return default;
            }

            _logger.LogInformation($"Data for key {key} returns successfully");
            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var lifeTime = expirationTime > DateTimeOffset.Now
                ? expirationTime.Subtract(DateTimeOffset.Now)
                : TimeSpan.Zero;

            if (lifeTime == TimeSpan.Zero)
            {
                _logger.LogWarning($"Expiration time for key {key} is in the past");
                return false;
            }

            var isSet = await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), lifeTime);
            if (isSet)
            {
                _logger.LogInformation($"Data for key {key} cached successfully");
                return isSet;
            }

            _logger.LogWarning($"Data for key {key} is not cached");
            return isSet;
        }

        public async Task<bool> RemoveDataAsync(string key)
        {
            // если по ключу уже не было значений - false будет
            var isDelete = await _cacheDb.KeyDeleteAsync(key);
            if (isDelete)
            {
                _logger.LogInformation($"Data for key {key} removed successfully");
            }
            else
            {
                _logger.LogError($"Value for key \"{key}\" is not removed from cache!");
            }
            return isDelete;
        }

        public async Task RemoveListAsync(IEnumerable<string> keys)
        {
            var tasks = keys.Select(async key =>
            {
                await RemoveDataAsync(key);
            });

            await Task.WhenAll(tasks);
        }
    }
}
