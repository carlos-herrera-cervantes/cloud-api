using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Api.Domain.Models;
using Api.Domain.Constants;
using Api.Repository.Managers;
using Api.Repository.Repositories;
using Api.Services.Services;

namespace Api.Web.Hooks
{
    public static class Seeder
    {
        /// <summary>
        /// Creates the basic documents for specific collections
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <returns>If the initialization is successful it returns true.</returns>
        public async static Task<bool> Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<IManager<User>>();
            scope.ServiceProvider.GetRequiredService<IRepository<User>>();

            var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SharedResources>>();

            int totalDocs = await userRepository.CountAsync();

            if (totalDocs == 0)
            {
                var user = new User
                {
                    Email = "super.admin@mytransformation.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    Password = "super.admin",
                    Role = Roles.SuperAdmin
                };

                await userManager.CreateAsync(user);

                logger.LogInformation("Basic users created");

                return true;
            }
            else
            {
                logger.LogInformation("Basic users have already been created");

                return false;
            }
        }
    }
}
