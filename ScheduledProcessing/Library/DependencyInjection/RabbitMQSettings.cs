using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public sealed class RabbitMQSettings
    {
        public string AmqpUrl { get; set; }
    }
}
