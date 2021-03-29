using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Api.Domain.Constants;
using Api.Domain.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Api.Repository.Extensions
{
    public static class MongoDBDefinitions
    {
        /// <summary>Build a filter for MongoDB driver</summary>
        /// <param name="request">The type Request object</params>
        /// <returns>A filter definition</returns>
        public static FilterDefinition<T> BuildFilter<T>(string[] filters) where T : BaseEntity
        {
            if (filters.IsEmpty()) return Builders<T>.Filter.Empty;
            var lambda = GenerateFilter<T>(filters);
            return Builders<T>.Filter.Where(lambda);
        }

        /// <summary>Build a sort filter for MongoDB driver</summary>
        /// <param name="request">Request object model</param>
        /// <returns>A sort definition</returns>
        public static SortDefinition<T> BuildSortFilter<T>(string sort) where T : class
        {
            if (String.IsNullOrEmpty(sort)) return Builders<T>.Sort.Descending("createdAt");

            var isAscending = sort.Contains('-');
            var property = isAscending ? sort.Split('-').Last() : sort;

            return isAscending ? Builders<T>.Sort.Ascending(property) : Builders<T>.Sort.Descending(property);
        }

        /// <summary>Populate reference field</summary>
        /// <param name="collection">Collection ued to create the fluent interface</param>
        /// <param name="filter">Filter definition for match pipe</param>
        /// <returns>Query to get documents and its references</returns>
        public static IAggregateFluent<T> Populate<T>
        (
            IMongoCollection<T> collection,
            FilterDefinition<T> filter,
            SortDefinition<BsonDocument> sortFilter,
            List<Relation> relations,
            Request request
        ) where T : BaseEntity
        {
            var (_, pageSize, page, entities, _) = request;
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            var entity = textInfo.ToTitleCase(entities.First());
            var (localKey, foreignKey, justOne) = relations.Find(r => r.Entity == entity);

            var pipe = collection
                .Aggregate()
                .Match(filter)
                .Lookup(entity, localKey, foreignKey, $"{entity}Embedded")
                .Skip(page * pageSize)
                .Limit(pageSize)
                .Sort(sortFilter);

            return justOne ? pipe.Unwind($"{entity}Embedded").As<T>() : pipe.As<T>();
        }

        /// <summary>Generate the filter using lambda expression</summary>
        /// <param name="keys">The query string sended in the http request</param>
        /// <returns>Lambda expression builded</returns>
        private static Expression<Func<T, bool>> GenerateFilter<T>(string[] keys) where T : BaseEntity
        {
            var firstElement = keys.First();
            var keyValues = firstElement.Split(',');
            var operators = keyValues.Select(key => key.ClassifyOperation());
            var parameterExpression = Expression.Parameter(typeof(T), "entity");
            var lambda = BuildExpression<T>(parameterExpression, operators);
            return lambda;
        }

        /// <summary>Prepared the function expression lambda</summary>
        /// <param name="expression">Expression for lambda</param>
        /// <param name="operators">List of TypeOperator objects</param>
        /// <returns>Lambda without compile</returns>
        private static Expression<Func<T, bool>> BuildExpression<T>(ParameterExpression expression, IEnumerable<TypeOperator> operators) where T : BaseEntity
        {
            var operations = new Dictionary<int, Expression>();
            var counter = 1;
            
            foreach (var item in operators)
            {
                var constant = Expression.Constant(item.Value);
                var property = Expression.Property(expression, item.Key);
                operations.Add(counter, GenerateTypeExpression(property, constant, item));
                counter++;
            }

            var lambda = Expression.Lambda<Func<T, bool>>(operations.First().Value, expression);
            return lambda;
        }

        /// <summary>Create a type expression for lambda</summary>
        /// <param name="property">The name of property to compare</param>
        /// <param name="constant">The value to compare</param>
        /// <param name="item">TypeOperator object</param>
        /// <returns>Type of expression: Equal, NotEqual, GreatherThan, and so on</returns>
        private static Expression GenerateTypeExpression(Expression property, Expression constant, TypeOperator item) =>
            item.Operation == Operators.Same ? Expression.Equal(property, constant) :
            item.Operation == Operators.NotSame ? Expression.NotEqual(property, constant) :
            item.Operation == Operators.Greather ? Expression.GreaterThan(property, constant) :
            item.Operation == Operators.GreaterThan ? Expression.GreaterThanOrEqual(property, constant) :
            item.Operation == Operators.Lower ? Expression.LessThan(property, constant) :
            item.Operation == Operators.LowerThan ? Expression.LessThanOrEqual(property, constant) :
            throw new InvalidOperationException();
    }
}