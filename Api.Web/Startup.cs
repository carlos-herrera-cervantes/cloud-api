using Api.Infrastructure.Container;
using Api.Repository.Managers;
using Api.Repository.Repositories;
using Api.Services.Services;
using Api.Web.Extensions;
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
            services.AddTokenAuthentication(Configuration);
            services.AddServicesFromInfrastructure(Configuration);
            services.UseSwagger();
            services.AddScoped(typeof(IManager<>), typeof(Manager<>));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IStationManager, StationManager>();
            services.AddTransient<IStationRepository, StationRepository>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPaymentMethodManager, PaymentMethodManager>();
            services.AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddTransient<IProductManager, ProductManager>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<ITokenRepository, TokenRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gulf Remastered - Local API V1");
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
