using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public class RedisSettings
    {
        public string ConnectionString { get; set; }
    }
}
