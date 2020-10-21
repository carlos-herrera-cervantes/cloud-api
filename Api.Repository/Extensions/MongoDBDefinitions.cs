using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Api.Domain.Constants;
using Api.Domain.Models;
using MongoDB.Driver;

namespace Api.Repository.Extensions
{
    public static class MongoDBDefinitions
    {
        public static FilterDefinition<T> BuildFilter<T>(Request request) where T : BaseEntity
        {
            var (_, filters) = request;
            if (filters.IsEmpty()) return Builders<T>.Filter.Empty;
            var lambda = GenerateFilter<T>(filters);
            return Builders<T>.Filter.Where(lambda);
        }

        private static Expression<Func<T, bool>> GenerateFilter<T>(string[] keys) where T : BaseEntity
        {
            var firstElement = keys.First();
            var keyValues = firstElement.Split(',');
            var operators = keyValues.Select(key => key.ClassifyOperation());
            var parameterExpression = Expression.Parameter(typeof(T), "entity");
            var lambda = BuildExpression<T>(parameterExpression, operators);
            return lambda;
        }

        public static Expression<Func<T, bool>> BuildExpression<T>(ParameterExpression expression, IEnumerable<TypeOperator> operators) where T : BaseEntity
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

        public static Expression GenerateTypeExpression(Expression property, Expression constant, TypeOperator item) =>
            item.Operation == Operators.Same ? Expression.Equal(property, constant) :
            item.Operation == Operators.NotSame ? Expression.NotEqual(property, constant) :
            item.Operation == Operators.Greather ? Expression.GreaterThan(property, constant) :
            item.Operation == Operators.GreaterThan ? Expression.GreaterThanOrEqual(property, constant) :
            item.Operation == Operators.Lower ? Expression.LessThan(property, constant) :
            item.Operation == Operators.LowerThan ? Expression.LessThanOrEqual(property, constant) :
            throw new InvalidOperationException();
    }
}