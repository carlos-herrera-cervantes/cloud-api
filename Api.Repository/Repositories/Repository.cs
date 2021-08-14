using Api.Domain.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Api.Infrastructure.Contexts;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Repository.Extensions;
using Microsoft.Extensions.Configuration;

namespace Api.Repository.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _context;

        private readonly IConfiguration _configuration;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;

            var client = MongoDBFactory
                .CreateClient(_configuration
                .GetSection("MongoDBSettings")
                .GetSection("ConnectionString").Value);


            _context = client
                .GetDatabase(_configuration
                .GetSection("MongoDBSettings")
                .GetSection("Database").Value)
                .GetCollection<T>($"{typeof(T).Name}s");
        }

        /// <summary>Get a list of documents</summary>
        /// <param name="request">Object of type request</param>
        /// <param name="relations">List of collections referenced</param>
        /// <returns>Mongo documents list</returns>
        public async Task<IEnumerable<T>> GetAllAsync(ListResourceRequest request, List<Relation> relations)
        {
            var( sort, pageSize, page, entities, filters ) = request;
            var filter = MongoDBDefinitions.BuildFilter<T>(filters);
            var sortTypedFilter = MongoDBDefinitions.BuildSortFilter<T>(sort);

            if (string.IsNullOrEmpty(entities))
            {
                return await _context
                    .Find(filter)
                    .Skip(page * pageSize)
                    .Limit(pageSize)
                    .Sort(sortTypedFilter)
                    .ToListAsync();
            }

            var sortBsonFilter = MongoDBDefinitions.BuildSortFilter<BsonDocument>(sort);

            return await MongoDBDefinitions
                .Populate<T>
                (
                    collection: _context,
                    filter: filter,
                    sortFilter: sortBsonFilter,
                    relations: relations,
                    request: request
                )
                .ToListAsync();
        }

        /// <summary>Get one document asynchronous fashion using its ID</summary>
        /// <param name="id">The string for object ID</param>
        /// <returns>Mongo document</returns>
        public async Task<T> GetByIdAsync(string id)
            => await _context.Find(entity => entity.Id == id).FirstOrDefaultAsync();

        /// <summary>Get one document asynchronous fashion</summary>
        /// <param name="request">Object of type Request</param>
        /// <returns>Mongo document</returns>
        public async Task<T> GetOneAsync(ListResourceRequest request)
        {
            var( _, filters ) = request;
            var filter = MongoDBDefinitions.BuildFilter<T>(filters);
            return await _context.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>Get one document asynchronous fashion</summary>
        /// <param name="filter">Filter definition</param>
        /// <returns>Mongo document</returns>
        public async Task<T> GetOneAsync(FilterDefinition<T> filter)
            => await _context.Find(filter).FirstOrDefaultAsync();

        /// <summary>
        /// Counts the documents of a collection
        /// </summary>
        /// <param name="request">Request object model</param>
        /// <returns>Number of documents</returns>
        public async Task<int> CountAsync(ListResourceRequest request)
        {
            var( _, filters ) = request;
            var filter = MongoDBDefinitions.BuildFilter<T>(filters);
            var totalDocuments = await _context.CountDocumentsAsync(filter);
            return (int)totalDocuments;
        }

        /// <summary>
        /// Counts the documents of a collection
        /// </summary>
        /// <param name="filter">Filter definition</param>
        /// <returns>Number of documents</returns>
        public async Task<int> CountAsync(FilterDefinition<T> filter)
            => (int)await _context.CountDocumentsAsync(filter);

        /// <summary>
        /// Counts the documents of a collection
        /// </summary>
        /// <returns>Number of documents</returns>
        public async Task<int> CountAsync()
            => (int)await _context.CountDocumentsAsync(Builders<T>.Filter.Empty);
    }
}
