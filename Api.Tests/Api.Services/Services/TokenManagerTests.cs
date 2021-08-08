using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;
using Api.Services.Services;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Api.Tests.Api.Services.Services
{
    public class TokenManagerTests
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
            }
        };

        [Fact]
        public async Task CreateAsync_ShouldAddNew_AccessToken()
        {
            var mockManager = new Mock<IManager<AccessToken>>();
            var token = new AccessToken
            {
                Token = "dummy-token-2",
                Email = "carlos.herrera@mytransformation.com",
                Role = "SuperAdmin",
                UserId = "60f24318aa63f042314227b8"
            };
            const string OBJECT_ID = "60f0cae6b06995113987163c";

            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<AccessToken>()))
                .Callback((AccessToken token) =>
                {
                    token.Id = OBJECT_ID;
                    _tokens.Add(token);
                });

            var tokenManager = new TokenManager(mockManager.Object);

            await tokenManager.CreateAsync(token);

            mockManager.Verify(x => x.CreateAsync(It.IsAny<AccessToken>()), Times.Once);
            Assert.Equal(OBJECT_ID, token.Id);
        }

        [Fact]
        public async Task DeleteManyAsync_ShouldDeleteAll_AccessToken()
        {
            var mockManager = new Mock<IManager<AccessToken>>();
            const string USER_ID = "60f24318aa63f042314227b8";

            mockManager.Setup(manager => manager.DeleteManyAsync(It.IsAny<FilterDefinition<AccessToken>>()))
                .Callback(() => _tokens.RemoveAll(t => t.UserId == USER_ID));

            var tokenManager = new TokenManager(mockManager.Object);

            await tokenManager.DeleteManyAsync(Builders<AccessToken>.Filter.Empty);

            mockManager.Verify(x => x.DeleteManyAsync(It.IsAny<FilterDefinition<AccessToken>>()), Times.Once);
            Assert.Empty(_tokens);
        }
    }
}
