using PrivatePackage.Abstractions;

namespace PrivatePackage.Caching
{
    public class RedisSettings : IRedisSettings
    {
        public string ConnectionString { get; set; }
    }
}
