using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace UrlService.Services
{
    public class RedisCacheService : ICacheService
    {
        public readonly IConnectionMultiplexer _connectionMultiplexer;
        public IDatabase database { get; set; }
        // public DistributedCacheEntryOptions options { get; set; }

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            database = _connectionMultiplexer.GetDatabase();
        }

        public async Task<string> GetAsynce(string key)
        {
            return await database.StringGetAsync(key);
        }

        public async Task WriteAsynce(string key, string value)
        {
            // var options = new DistributedCacheEntryOptions();
            // options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
            // options.SlidingExpiration = TimeSpan.FromSeconds(60);

            await database.StringSetAsync(key, value, TimeSpan.FromSeconds(60));
        }

        //docker run --name redis -p 6379:6379 -d redis
    }
}