using Microsoft.Extensions.DependencyInjection;
using PrivatePackage.Abstractions;
using PrivatePackage.Caching;
using StackExchange.Redis;

namespace PrivatePackage.DependencyInjection
{
    public static class CachingExtensions
    {
        public static IServiceCollection BootstrapCache(this IServiceCollection services, RedisSettings redis)
        {
            services
                .AddSingleton<IRedisSettings>(_ => redis)
                .AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redis.ConnectionString))
                .AddSingleton<ICacheService, RedisCacheService>();

            return services;
        }
    }
}
