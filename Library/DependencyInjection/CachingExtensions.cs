using Library.Abstractions;
using Library.Caching;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Library.DependencyInjection
{
    public static class CachingExtensions
    {
        public static IServiceCollection BootstrapCache(this IServiceCollection services, RedisSettings redis)
        {
            services
                .AddSingleton(_ => redis)
                .AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redis.ConnectionString))
                .AddSingleton<ICacheService, RedisCacheService>();

            return services;
        }
    }
}
