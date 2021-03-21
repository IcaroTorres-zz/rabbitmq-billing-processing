using Library.Caching;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
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
