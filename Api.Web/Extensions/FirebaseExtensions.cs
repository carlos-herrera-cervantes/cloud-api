using System.Threading.Tasks;
using Firebase.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Web.Extensions
{
    public static class FirebaseExtensions
    {
        public static IServiceCollection AddFirebaseClient(this IServiceCollection services, IConfiguration configuration) 
        {
            var firebaseAuth = new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(configuration["Firebase:AppSecret"]) };
            services.AddSingleton<FirebaseClient>(_ => new FirebaseClient(configuration["Firebase:Database"], firebaseAuth));
            return services;
        }
    }
}