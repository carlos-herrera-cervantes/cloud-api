using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Infrastructure.Contexts;
using Api.Repository.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Api.Repository.Managers
{
    public class Manager<T> : IManager<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _context;

        private readonly IConfiguration _configuration;

        public Manager(IConfiguration configuration)
        {
            _configuration = configuration;

            var client = MongoDBFactory
                .CreateClient(_configuration.GetSection("MongoDBSettings").GetSection("ConnectionString").Value);

            _context = client
                .GetDatabase(_configuration.GetSection("MongoDBSettings").GetSection("Database").Value)
                .GetCollection<T>($"{typeof(T).Name}s");
        }

        public async Task CreateAsync(T instance) => await _context.InsertOneAsync(instance);

        public async Task UpdateByIdAsync(string id, T newInstance, JsonPatchDocument<T> currentInstance)
        {
            currentInstance.ApplyTo(newInstance);
            await _context.ReplaceOneAsync(entity => entity.Id == id, newInstance);
        }

        public async Task DeleteByIdAsync(string id) => await _context.DeleteOneAsync(entity => entity.Id == id);

        public async Task<DeleteResult> DeleteManyAsync(ListResourceRequest request)
        {
            var (_, filters) = request;
            var filter = MongoDBDefinitions.BuildFilter<T>(filters);

            return await _context.DeleteManyAsync(filter);
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter) => await _context.DeleteManyAsync(filter);
    }
}
