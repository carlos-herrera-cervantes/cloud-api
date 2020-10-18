using System.Collections.Generic;
using System.Globalization;
using Api.Infrastructure.Container;
using Api.Repository.Managers;
using Api.Repository.Repositories;
using Api.Services.Services;
using Api.Web.Backgrounds;
using Api.Web.Extensions;
using Api.Web.Handlers;
using Api.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace Api.Web
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllers().AddNewtonsoftJson().AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResources));
            });
            services.AddTokenAuthentication(Configuration);
            services.AddServicesFromInfrastructure(Configuration);
            services.AddFirebaseClient(Configuration);
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
            services.AddTransient<ICustomerPurchaseManager, CustomerPurchaseManager>();
            services.AddTransient<ICustomerPurchaseRepository, CustomerPurchaseRepository>();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.AddSingleton<IStringLocalizer, JsonStringLocalizer>();
            services.AddSingleton<IOperationHandler, OperationHandler>();
            services.AddHostedService<FirebaseConsumer>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            var cultures = new List<CultureInfo>
            {
                new CultureInfo("es"),
                new CultureInfo("en")
            };

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("es");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

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
