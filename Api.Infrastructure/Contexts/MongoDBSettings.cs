namespace Api.Infrastructure.Contexts
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string ConnectionString { get; set ; }

        public string Database { get; set; }
    }
}
