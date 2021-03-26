using Api.Domain.Models;
using MongoDB.Driver;
using MongoDB.Bson;
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

        /// <summary>Get a list of documents</summary>
        /// <param name="request">Object of type request</param>
        /// <param name="relations">List of collections referenced</param>
        /// <returns>Mongo documents list</returns>
        public async Task<IEnumerable<T>> GetAllAsync(Request request, List<Relation> relations)
        {
            var( sort, pageSize, page, entities, filters ) = request;
            var filter = MongoDBDefinitions.BuildFilter<T>(filters);
            var sortTypedFilter = MongoDBDefinitions.BuildSortFilter<T>(sort);

            if (entities.IsEmpty())
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
        public async Task<T> GetByIdAsync(string id) => await _context.Find(entity => entity.Id == id).FirstOrDefaultAsync();

        /// <summary>Get one document asynchronous fashion</summary>
        /// <param name="request">Object of type Request</param>
        /// <returns>Mongo document</returns>
        public async Task<T> GetOneAsync(Request request)
        {
            var( _, filters ) = request;
            var filter = MongoDBDefinitions.BuildFilter<T>(filters);
            return await _context.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Counts the documents of a collection
        /// </summary>
        /// <param name="request">Request object model</param>
        /// <returns>Number of documents</returns>
        public async Task<int> CountAsync(Request request)
        {
            var( _, filters ) = request;
            var filter = MongoDBDefinitions.BuildFilter<T>(filters);
            var totalDocuments = await _context.CountDocumentsAsync(filter);
            return (int)totalDocuments;
        }
    }
}
