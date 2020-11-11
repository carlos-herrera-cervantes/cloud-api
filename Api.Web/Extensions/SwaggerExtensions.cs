using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Api.Web.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection UseSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Remastered - Local API", Version = "v1" });
            });
            return services;
        }
    }
}