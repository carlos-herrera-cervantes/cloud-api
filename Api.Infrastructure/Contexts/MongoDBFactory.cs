using MongoDB.Driver;

namespace Api.Infrastructure.Contexts
{
    public static class MongoDBFactory
    {
        public static MongoClient CreateClient(string connectionString) => new MongoClient(connectionString);
    }
}
