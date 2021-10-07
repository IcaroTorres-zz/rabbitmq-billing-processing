namespace Microsoft.Extensions.DependencyInjection
{

    public class MongoDBSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public CollectionsDictionary Collections { get; set; }
    }
}
