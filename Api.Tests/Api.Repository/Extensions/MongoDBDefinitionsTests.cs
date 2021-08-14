using Api.Domain.Models;
using Api.Repository.Extensions;
using MongoDB.Driver;
using Xunit;

namespace Api.Tests.Api.Repository.Extensions
{
    public class MongoDBDefinitionsTests
    {
        [Fact]
        public void BuildFilter_ShouldReturnEmpty_FilterDefinition()
        {
            string filters = string.Empty;

            var result = MongoDBDefinitions.BuildFilter<User>(filters);

            Assert.Equal(Builders<User>.Filter.Empty, result);
        }

        [Fact]
        public void BuildFilter_ShouldReturnLambda_FilterDefinition()
        {
            var filters = "FirstName=Isela";

            var result = MongoDBDefinitions.BuildFilter<User>(filters);

            Assert
                .Equal(Builders<User>.Filter.Where(entity => (entity.FirstName == "Isela"))
                .ToString(), result.ToString());
        }

        [Fact]
        public void BuildSortFilter_ShouldReturnDefault_SortDefinition()
        {
            string sort = string.Empty;

            var result = MongoDBDefinitions.BuildSortFilter<User>(sort);

            Assert.Equal("DirectionalSortDefinition`1", result.GetType().Name);
        }
    }
}