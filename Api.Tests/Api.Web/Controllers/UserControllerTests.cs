using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Controllers;
using Api.Web.Handlers;
using Api.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceStack.Redis;
using Xunit;

namespace Api.Tests.Api.Web.Controllers
{
    public class UserControllerTests
    {
        private readonly List<User> _users = new List<User>
        {
            new User
            {
                Id = "5fd5ddc549c67b3c6527462f",
                FirstName = "Carlos",
                LastName = "Herrera",
                Email = "carlos.herrera@mytransformation.com",
                Role = "SuperAdmin",
                Password = "super.admin",
                StationId = "5f947448c83ad110d92cde25"
            },
            new User
            {
                Id = "60f0cae6b06995113987163c",
                FirstName = "Isela",
                LastName = "Ortíz",
                Email = "isela.ortiz@mytransformation.com",
                Role = "Employee",
                Password = "secret123",
                StationId = "60f61c45d9f5e4150c2e8f6f"
            }
        };
        private readonly string _accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImNhcmxvc0ByZW1hc3RlcmVkLmNvbSIsInJvbGUiOiJTdGF0aW9uQWRtaW4iLCJuYW1laWQiOiI1ZmQ1ZGRjNTQ5YzY3YjNjNjUyNzQ2MmYiLCJzdGF0aW9uIjoiNWY5NDc0NDhjODNhZDExMGQ5MmNkZTI1IiwibmJmIjoxNjI0MDMzNjgzLCJleHAiOjE2MjQ0NjU2ODMsImlhdCI6MTYyNDAzMzY4M30.FVa35rRAJHN0oYUbk0eab-B3CJRmdy7AkZiCnB0e-Q4";
        private readonly Mock<IUserRepository> _mockRepository = new Mock<IUserRepository>();
        private readonly Mock<IUserManager> _mockManager = new Mock<IUserManager>();
        private readonly Mock<IOperationHandler> _mockOperationHandler = new Mock<IOperationHandler>();
        private readonly Mock<IRedisClientsManagerAsync> _mockRedisManager = new Mock<IRedisClientsManagerAsync>();
        private readonly Mock<IRedisClientAsync> _mockRedisClient = new Mock<IRedisClientAsync>();
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userController = new UserController
            (
                _mockManager.Object,
                _mockRepository.Object,
                _mockOperationHandler.Object,
                _mockRedisManager.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_UsersList()
        {
            var request = new ListResourceRequest
            {
                Page = 1,
                PageSize = 10
            };

            _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(_users);
            _mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(2);
            
            var result = await _userController.GetAllAsync(request);
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListUserResponse;

            _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            _mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ListUserResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(1, responseBody.Paginator.Page);
            Assert.Equal(10, responseBody.Paginator.PageSize);
            Assert.Equal(0, responseBody.Paginator.RemainingDocuments);
            Assert.Equal(2, responseBody.Data.Count());
        }

        [Fact]
        public async Task GetByStation_ShouldReturn_Users_BySpecificStation()
        {
            var request = new ListResourceRequest
            {
                Page = 1,
                PageSize = 10
            };

            const string STATION_ID = "5f947448c83ad110d92cde25";

            _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>()))
                .ReturnsAsync((ListResourceRequest request) => _users.Where(u => u.StationId == STATION_ID));
            _mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(1);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            _userController.ControllerContext.HttpContext = httpContext;

            var result = await _userController.GetByStation(request);
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListUserResponse;

            _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            _mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ListUserResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(1, responseBody.Paginator.Page);
            Assert.Equal(10, responseBody.Paginator.PageSize);
            Assert.Equal(0, responseBody.Paginator.RemainingDocuments);
            Assert.Single(responseBody.Data);
        }

        [Fact]
        public async Task GetMeAsync_ShouldReturn_SingleUser()
        {
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _users.SingleOrDefault(u => u.Id == id));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            _userController.ControllerContext.HttpContext = httpContext;

            var result = await _userController.GetMeAsync();
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleUserResponse;

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleUserResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("5fd5ddc549c67b3c6527462f", responseBody.Data.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_SingleUser()
        {
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _users.SingleOrDefault(u => u.Id == id));

            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<User>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            var result = await _userController.GetByIdAsync("60f0cae6b06995113987163c");
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleUserResponse;

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.GetAsync<User>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleUserResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("60f0cae6b06995113987163c", responseBody.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturn_CreatedUser()
        {
            var user = new User
            {
                FirstName = "Anel",
                LastName = "Ortíz",
                Email = "anel.ortiz@mytransformation.com",
                Role = "Employee",
                Password = "secret123",
                StationId = "60f61c45d9f5e4150c2e8f6f"
            };

            _mockManager.Setup(manager => manager.CreateAsync(It.IsAny<User>()))
                .Callback((User user) => 
                {
                    user.Id = "60f70c1f7098da34083f12e2";
                    _users.Add(user);
                });

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _userController.CreateAsync(user);
            var response = result as CreatedResult;
            var responseBody = response.Value as SingleUserResponse;

            _mockManager.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<CreatedResult>(result);
            Assert.IsType<SingleUserResponse>(response.Value);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal("60f70c1f7098da34083f12e2", responseBody.Data.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_ShouldReturn_UpdatedUser()
        {
            var user = new User
            {
                Id = "60f0cae6b06995113987163c",
                FirstName = "Isela",
                LastName = "Ortíz",
                Email = "isela.ortiz@mytransformation.com",
                Role = "Employee",
                Password = "secret123",
                StationId = "60f61c45d9f5e4150c2e8f6f"
            };

            const string NEW_LAST_NAME = "De Herrera";

            var updatedProperties = new JsonPatchDocument<User>();
            updatedProperties.Replace(u => u.LastName, NEW_LAST_NAME);

            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<User>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            _mockRedisClient.Setup(client => client
                .RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _users.SingleOrDefault(u => u.Id == id));

            _mockManager.Setup(manager => manager
                .UpdateByIdAsync(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<JsonPatchDocument<User>>()))
                .Callback((string id, User user, JsonPatchDocument<User> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(user);
                    _users[_users.FindIndex(u => u.Id == id)] = user;
                });

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _userController.UpdateByIdAsync("60f0cae6b06995113987163c", updatedProperties);
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleUserResponse;

            _mockManager.Verify(x =>
                x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<JsonPatchDocument<User>>()), Times.Once);
            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.GetAsync<User>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleUserResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(responseBody.Data.LastName, NEW_LAST_NAME);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_SpecificUser()
        {
            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<User>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            _mockRedisClient.Setup(client => client
                .RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _users.SingleOrDefault(u => u.Id == id));

            _mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _users.RemoveAt(_users.FindIndex(u => u.Id == id)));

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _userController.DeleteByIdAsync("5fd5ddc549c67b3c6527462f");
            var response = result as NoContentResult;
            
            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.GetAsync<User>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }
    }
}