using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
namespace PRO.Api.Extensions;

public static class DistributedCacheExtention
{
    private static readonly string prefix = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    private static string GetCacheKey(string key) => String.Format(Infrastructure.Contract.CacheKeyFormat, prefix, key);

    public static async Task RemoveCacheAsync(this IDistributedCache cache, string key, CancellationToken token = default(CancellationToken))
    {
        await cache.RemoveAsync(GetCacheKey(key), token);
    }

    public static async Task<T> GetOrSetCacheAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> func, DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions))
    {
        string _key = GetCacheKey(key);
        await semaphore.WaitAsync();
        T output = default(T);
        try
        {
            var tmp = await cache.GetAsync(_key);
            if (tmp != null)
            {
                var data = Encoding.UTF8.GetString(tmp);
                output = JsonSerializer.Deserialize<T>(data);
            }
            else if (func != null)
            {
                output =await func();
                var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(output, Infrastructure.Contract.jsonOptions));
                await cache.SetAsync(_key, data, options);
            }
        }
        finally
        {
            semaphore.Release();
        }
        return output;
    }
}