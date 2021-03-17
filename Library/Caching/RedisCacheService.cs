using Library.Abstractions;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Library.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            this.connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<string> GetAsync(string key)
        {
            var db = connectionMultiplexer.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task SetAsync(string key, string value, int timeInSeconds)
        {
            var db = connectionMultiplexer.GetDatabase();
            var expiry = TimeSpan.FromSeconds(timeInSeconds);
            await db.StringSetAsync(key, value, expiry);
        }
    }
}
