using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Api.Infrastructure.Contexts;
using Microsoft.Extensions.Options;

namespace Api.Infrastructure.Container
{
    public static class Container
    {
        public static IServiceCollection AddServicesFromInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDBSettings>(configuration.GetSection(nameof(MongoDBSettings)));
            services.AddSingleton<IMongoDBSettings>(sp => sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);
            return services;
        }
    }
}