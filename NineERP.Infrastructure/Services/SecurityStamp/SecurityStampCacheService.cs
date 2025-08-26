using Microsoft.Extensions.Caching.Memory;

namespace NineERP.Infrastructure.Services.SecurityStamp
{
    public interface ISecurityStampCacheService
    {
        /// <summary>
        /// Get the Security Stamp from the cache (if available); otherwise, call getStampFromDb to query the database and update the cache.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="getStampFromDb">Asynchronous function to query Security Stamp from DB</param>
        /// <returns>User Security Stamp</returns>
        Task<string> GetSecurityStampAsync(string userId, Func<Task<string>> getStampFromDb);
        void RemoveSecurityStampFromCache(string userId);
    }

    public class SecurityStampCacheService(IMemoryCache cache) : ISecurityStampCacheService
    {
        private readonly MemoryCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        public async Task<string> GetSecurityStampAsync(string userId, Func<Task<string>> getStampFromDb)
        {
            // If the value is already in the cache, return it immediately.
            if (cache.TryGetValue(userId, out string? cachedStamp))
            {
                return cachedStamp ?? "";
            }

            // If not, query the database via the passed function.
            string stamp = await getStampFromDb();
            // Save results to cache
            cache.Set(userId, stamp, _cacheOptions);
            return stamp;
        }

        public void RemoveSecurityStampFromCache(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                cache.Remove(userId);
            }
        }
    }
}
