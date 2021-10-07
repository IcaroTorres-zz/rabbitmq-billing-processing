using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Library.Caching
{

  public class RedisCacheService : ICacheService
  {
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
      _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<string> GetAsync(string key)
    {
      var db = _connectionMultiplexer.GetDatabase();
      return await db.StringGetAsync(key);
    }

    public async Task<bool> SetAsync(string key, string value, int timeInSeconds)
    {
      var db = _connectionMultiplexer.GetDatabase();
      var expiry = TimeSpan.FromSeconds(timeInSeconds);
      return await db.StringSetAsync(key, value, expiry);
    }
  }
}
