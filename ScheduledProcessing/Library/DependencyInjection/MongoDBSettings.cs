using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public class MongoDBSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public CollectionsDictionary Collections { get; set; }
    }
}
