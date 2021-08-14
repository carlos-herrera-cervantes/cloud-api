using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.JsonPatch;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Repository.Managers;

namespace Api.Tests.Api.Services.Services
{
    public class UserManagerTests
    {
        private List<User> _users = new List<User>
        {
            new User
            {
                Id = "60f0cae6b06995113987163c",
                FirstName = "Isela",
                LastName = "Ortíz",
                Email = "isela.ortiz@mytransformation.com",
                Role = "Employee",
                Password = "secret123"
            }
        };

        [Fact]
        public async Task CreateAsync_ShouldAdd_NewUser()
        {
            var mockManager = new Mock<IManager<User>>();
            var user = new User
            {
                FirstName = "Carlos",
                LastName = "Herrera",
                Email = "carlos.herrera@mytransformation.com",
                Role = "SuperAdmin",
                Password = "super.admin"
            };
            const string OBJECT_ID = "60f0cbb3117067f1b7416088";

            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    user.Id = OBJECT_ID;
                    _users.Add(user);
                });

            var userManager = new UserManager(mockManager.Object);

            await userManager.CreateAsync(user);

            mockManager.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
            Assert.Equal(OBJECT_ID, user.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_ShouldUpdate_ExistingUser()
        {
            var mockManager = new Mock<IManager<User>>();
            var user = new User
            {
                Id = "60f0cae6b06995113987163c",
                FirstName = "Isela",
                LastName = "Ortíz",
                Email = "isela.ortiz@mytransformation.com",
                Role = "Employee",
                Password = "secret123"
            };

            const string OBJECT_ID = "60f0cae6b06995113987163c";
            const string NEW_LAST_NAME = "De Herrera";

            var updatedProperties = new JsonPatchDocument<User>();
            updatedProperties.Replace(u => u.LastName, NEW_LAST_NAME);

            mockManager.Setup(manager => manager
                .UpdateByIdAsync(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<JsonPatchDocument<User>>()))
                .Callback((string id, User user, JsonPatchDocument<User> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(user);
                    _users[_users.FindIndex(u => u.Id == id)] = user;
                });

            var userManager = new UserManager(mockManager.Object);

            await userManager.UpdateByIdAsync(OBJECT_ID, user, updatedProperties);

            mockManager.Verify(x =>
                x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<JsonPatchDocument<User>>()), Times.Once);
            Assert.Equal(user.LastName, NEW_LAST_NAME);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_CorrectUser()
        {
            var mockManager = new Mock<IManager<User>>();

            mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _users.RemoveAt(_users.FindIndex(u => u.Id == id)));

            const string OBJECT_ID = "60f0cae6b06995113987163c";

            var userManager = new UserManager(mockManager.Object);

            await userManager.DeleteByIdAsync(OBJECT_ID);

            mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.Empty(_users);
        }
    }
}