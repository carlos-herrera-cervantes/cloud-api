using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web;
using Api.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Api.Tests.Api.Web.Controllers
{
    public class LoginControllerTests
    {
        private readonly User _user = new User
        {
            Id = "60fee1e876cb9e9774ae21fc",
            Email = "user@example.com",
            Role = "SuperAdmin",
            Password = "secret123"
        };

        private readonly string _accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImNhcmxvc0ByZW1hc3RlcmVkLmNvbSIsInJvbGUiOiJTdGF0aW9uQWRtaW4iLCJuYW1laWQiOiI1ZmQ1ZGRjNTQ5YzY3YjNjNjUyNzQ2MmYiLCJzdGF0aW9uIjoiNWY5NDc0NDhjODNhZDExMGQ5MmNkZTI1IiwibmJmIjoxNjI0MDMzNjgzLCJleHAiOjE2MjQ0NjU2ODMsImlhdCI6MTYyNDAzMzY4M30.FVa35rRAJHN0oYUbk0eab-B3CJRmdy7AkZiCnB0e-Q4";

        [Fact]
        public async Task Login_ShouldReturn_JsonWebToken()
        {
            var mockRepository = new Mock<IUserRepository>();
            var mockManager = new Mock<ITokenManager>();
            var mockLocalizer = new Mock<IStringLocalizer<SharedResources>>();

            var mockConfigurationSection = new Mock<IConfigurationSection>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfigurationSection.Setup(x => x.Value).Returns("asdwda1d8a4sd8w4das8d*w8d*asd@#");
            mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(mockConfigurationSection.Object);

            mockRepository.Setup(repo => repo.GetOneAsync(It.IsAny<FilterDefinition<User>>())).ReturnsAsync(_user);

            mockManager.Setup(manager => manager.DeleteManyAsync(It.IsAny<FilterDefinition<AccessToken>>()));
            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<AccessToken>()));

            var loginController = new LoginController
            (
                mockRepository.Object,
                mockConfiguration.Object,
                mockManager.Object,
                mockLocalizer.Object
            );

            var result = await loginController.Login(new Credentials { Email = "user@example.com", Password = "secret123" });
            var response = result as OkObjectResult;
            var responseBody = response.Value as StringResponse;

            mockRepository.Verify(x => x.GetOneAsync(It.IsAny<FilterDefinition<User>>()), Times.Once);
            mockManager.Verify(x => x.DeleteManyAsync(It.IsAny<FilterDefinition<AccessToken>>()), Times.Once);
            mockManager.Verify(x => x.CreateAsync(It.IsAny<AccessToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<StringResponse>(responseBody);
        }

        [Fact]
        public async Task Login_ShouldReturn_BadRequest()
        {
            var mockRepository = new Mock<IUserRepository>();
            var mockManager = new Mock<ITokenManager>();
            var mockLocalizer = new Mock<IStringLocalizer<SharedResources>>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockRepository.Setup(repo => repo.GetOneAsync(It.IsAny<FilterDefinition<User>>())).ReturnsAsync(_user);

            var loginController = new LoginController
            (
                mockRepository.Object,
                mockConfiguration.Object,
                mockManager.Object,
                mockLocalizer.Object
            );

            var result = await loginController.Login(new Credentials { Email = "user@example.com", Password = "secret" });
            
            mockRepository.Verify(x => x.GetOneAsync(It.IsAny<FilterDefinition<User>>()), Times.Once);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Logout_ShouldReturn_NoContent()
        {
            var mockRepository = new Mock<IUserRepository>();
            var mockManager = new Mock<ITokenManager>();
            var mockLocalizer = new Mock<IStringLocalizer<SharedResources>>();
            var mockConfiguration = new Mock<IConfiguration>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            mockManager.Setup(manager => manager.DeleteManyAsync(It.IsAny<FilterDefinition<AccessToken>>()));

            var loginController = new LoginController
            (
                mockRepository.Object,
                mockConfiguration.Object,
                mockManager.Object,
                mockLocalizer.Object
            );
            loginController.ControllerContext.HttpContext = httpContext;

            var result = await loginController.Logout();

            mockManager.Verify(x => x.DeleteManyAsync(It.IsAny<FilterDefinition<AccessToken>>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}