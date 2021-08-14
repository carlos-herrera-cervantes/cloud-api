using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Repository.Repositories;

namespace Api.Tests.Api.Services.Services
{
    public class UserRepositoryTests
    {
        private readonly List<User> _users = new List<User>
        {
            new User
            {
                Id = "60f0cbb3117067f1b7416088",
                FirstName = "Carlos",
                LastName = "Herrera",
                Email = "carlos.herrera@mytransformation.com",
                Role = "SuperAdmin",
                Password = "super.admin"
            },
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
        public async Task GetAllAsync_ShouldReturn_UsersList()
        {
            var mockRepository = new Mock<IRepository<User>>();

            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>(), null))
                .ReturnsAsync(_users);

            const int TOTAL_DOCS = 2;
            const string USER_ID_1 = "60f0cbb3117067f1b7416088";
            const string USER_ID_2 = "60f0cae6b06995113987163c";

            var userRepository = new UserRepository(mockRepository.Object);

            var result = await userRepository.GetAllAsync(new ListResourceRequest());

            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>(), null), Times.Once);
            Assert.Equal(result.Count(), TOTAL_DOCS);
            Assert.Equal(result.FirstOrDefault().Id, USER_ID_1);
            Assert.Equal(result.LastOrDefault().Id, USER_ID_2);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_CorrectUser()
        {
            var mockRepository = new Mock<IRepository<User>>();

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _users.SingleOrDefault((u => u.Id == id)));

            const string USER_ID = "60f0cae6b06995113987163c";
            var userRepository = new UserRepository(mockRepository.Object);

            var result = await userRepository.GetByIdAsync(USER_ID);

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.Equal(result.Id, USER_ID);
        }

        [Fact]
        public async Task CountAsync_ShouldReturn_NumberUsers()
        {
            var mockRepository = new Mock<IRepository<User>>();

            mockRepository.Setup(repo => repo.CountAsync()).ReturnsAsync(2);

            var userRepository = new UserRepository(mockRepository.Object);

            var result = await userRepository.CountAsync();

            mockRepository.Verify(x => x.CountAsync(), Times.Once);
            Assert.Equal(2, result);
        }
    }
}