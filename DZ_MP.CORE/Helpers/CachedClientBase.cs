using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace DZ_MP.CORE.Helpers;

/// <summary>
/// Base class สำหรับ HTTP client ที่ต้องการ Cache-Aside pattern ผ่าน IDistributedCache (Redis / In-Memory)
/// Subclass ส่ง Func{bool} isReady แทนการ inject IConnectionMultiplexer โดยตรง
/// เพื่อให้ CORE ไม่ต้องผูกกับ StackExchange.Redis
/// </summary>
public abstract class CachedClientBase
{
    protected readonly IDistributedCache _cache;
    private readonly Func<bool> _isReady;

    /// <summary>JsonSerializerOptions ที่ใช้ทั้งอ่านจาก cache และอ่านจาก HTTP response; accessible โดย subclass</summary>
    protected readonly JsonSerializerOptions _jsonOptions;

    protected static readonly TimeSpan DefaultTtl = TimeSpan.FromHours(8);

    protected CachedClientBase(IDistributedCache cache, Func<bool> isReady, JsonSerializerOptions? jsonOptions = null)
    {
        _cache = cache;
        _isReady = isReady;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    /// <summary>true เมื่อ cache backend พร้อมรับ-ส่งข้อมูล</summary>
    protected bool IsReady => _isReady();

    /// <summary>
    /// Cache-Aside helper: เช็ค cache → miss → ยิง fetchFunc → เก็บ cache
    /// ถ้า IsReady = false จะ bypass cache และยิง fetchFunc ตรงๆ
    /// </summary>
    protected async Task<T?> GetOrFetchAsync<T>(string cacheKey, Func<Task<T?>> fetchFunc, TimeSpan? ttl = null) where T : class
    {
        if (IsReady)
        {
            try
            {
                var cached = await _cache.GetStringAsync(cacheKey);
                if (cached is not null)
                    return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
            }
            catch { /* cache read fail → fetch ตรงต่อ */ }
        }

        var data = await fetchFunc();
        if (data is null) return null;

        if (IsReady)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl ?? DefaultTtl
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data, _jsonOptions), options);
            }
            catch { /* cache write fail → ข้าม */ }
        }

        return data;
    }
}
