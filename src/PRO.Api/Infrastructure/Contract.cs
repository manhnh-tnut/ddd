using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace PRO.Api.Infrastructure;

public static class Contract{
    public static string CacheKeyFormat="{0}-{1}:";
    
    public static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = null,
    };

    public static DistributedCacheEntryOptions Cache5M = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
    public static DistributedCacheEntryOptions Cache10M = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
    public static DistributedCacheEntryOptions Cache30M = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
    public static DistributedCacheEntryOptions Cache1H = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
}