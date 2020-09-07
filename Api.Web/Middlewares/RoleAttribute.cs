using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Web.Middlewares
{
    public class RoleAttribute : TypeFilterAttribute
    {
        public static string[] Roles;

        public RoleAttribute(string[] roles) : base(typeof(RoleFilter)) => Roles = roles;

        private class RoleFilter : IAsyncActionFilter
        {
            private readonly ITokenRepository _tokenRepository;

            public RoleFilter(ITokenRepository tokenRepository) => _tokenRepository = tokenRepository;

            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var bearerToken = context.HttpContext.Request.Headers["Authorization"].ToString();
                var token = bearerToken.Split(" ").First();
                var request = new Request { Filters = new [] { $"Token={token}" } };
                var tokenSession = await _tokenRepository.GetOneAsync(request);
                var role = RoleAttribute.Roles.Where(role => role == tokenSession.Role).FirstOrDefault();

                if (role is null)
                {
                    context.Result = new BadRequestObjectResult(new { Status = false, Message = "InvalidPermissions" });
                    return;
                }
                
                await next();
            }

            #endregion
        }
    }
}