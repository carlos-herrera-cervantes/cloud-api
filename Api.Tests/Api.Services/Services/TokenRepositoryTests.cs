using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;
using Api.Services.Services;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Api.Tests.Api.Services.Services
{
    public class TokenRepositoryTests
    {
        private readonly List<AccessToken> _tokens = new List<AccessToken>
        {
            new AccessToken
            {
                Id = "60f0cbb3117067f1b7416088",
                Token = "dummy-token",
                Email = "carlos.herrera@mytransformation.com",
                Role = "SuperAdmin",
                UserId = "60f24318aa63f042314227b8"
            },
            new AccessToken
            {
                Id = "60f0cae6b06995113987163c",
                Token = "dummy-token-2",
                Email = "carlos.herrera@mytransformation.com",
                Role = "SuperAdmin",
                UserId = "60f24318aa63f042314227b8"
            }
        };

        [Fact]
        public async Task GetOneAsync_Should_Return_Correct_AccessToken()
        {
            var mockRepository = new Mock<IRepository<AccessToken>>();
            const string token = "dummy-token";

            mockRepository.Setup(repo => repo.GetOneAsync(It.IsAny<FilterDefinition<AccessToken>>()))
                .ReturnsAsync(() => _tokens.SingleOrDefault(t => t.Token == token));

            var tokenRepository = new TokenRepository(mockRepository.Object);
            const string OBJECT_ID = "60f0cbb3117067f1b7416088";

            var result = await tokenRepository.GetOneAsync(Builders<AccessToken>.Filter.Empty);

            mockRepository.Verify(x => x.GetOneAsync(It.IsAny<FilterDefinition<AccessToken>>()), Times.Once);
            Assert.Equal(result.Id, OBJECT_ID);
        }
    }
}
