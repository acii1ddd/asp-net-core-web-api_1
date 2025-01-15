namespace BookAPI.Cache
{
    public interface ICacheService
    {
        // методы могут быть реализованы с разными типами, в отличии от public interface ICacheService<T>
        public Task<T?> GetDataAsync<T>(string key);

        /// <summary>
        /// Вставка данных в кэш
        /// </summary>
        /// <param name="key">Ключ для вставки в кэш</param>
        /// <param name="value">Значние, доступное по ключу key</param>
        /// <param name="expirationTime">Время истечения хранения ключа (когда он должен удалиться из кэша)</param>
        /// <returns></returns>
        public Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime);

        public Task<bool> RemoveDataAsync(string key);

        public Task RemoveListAsync(IEnumerable<string> keys);
    }
}
