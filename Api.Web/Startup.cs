using Api.Infrastructure.Container;
using Api.Repository.Managers;
using Api.Repository.Repositories;
using Api.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Web
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddServicesFromInfrastructure(Configuration);
            services.AddScoped(typeof(IManager<>), typeof(Manager<>));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IStationManager, StationManager>();
            services.AddTransient<IStationRepository, StationRepository>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IUserRepository, UserRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
