using Api.Domain.Models;
using MongoDB.Driver;
using Api.Infrastructure.Contexts;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Repository.Extensions;

namespace Api.Repository.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _context;

        public Repository(IMongoDBSettings context)
        {
            var client = MongoDBFactory.CreateClient(context.ConnectionString);
            var database = client.GetDatabase(context.Database);
            _context = database.GetCollection<T>($"{typeof(T).Name}s");
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Find(entity => true).ToListAsync();

        public async Task<T> GetByIdAsync(string id) => await _context.Find(entity => entity.Id == id).FirstOrDefaultAsync();

        public async Task<T> GetOneAsync(Request request)
        {
            var filter = MongoDBDefinitions.BuildFilter<T>(request);
            return await _context.Find(filter).FirstOrDefaultAsync();
        }
    }
}
