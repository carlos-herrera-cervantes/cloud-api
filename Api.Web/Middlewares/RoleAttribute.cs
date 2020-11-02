using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Middlewares
{
    public class RoleAttribute : TypeFilterAttribute
    {
        public RoleAttribute(string[] roles) : base(typeof(RoleFilter)) => Arguments = new object[] { roles };

        private class RoleFilter : IAsyncActionFilter
        {
            private readonly ITokenRepository _tokenRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;
            private readonly string[] _roles;

            public RoleFilter(ITokenRepository tokenRepository, IStringLocalizer<SharedResources> localizer, string[] roles)
                => (_tokenRepository, _localizer, _roles) = (tokenRepository, localizer, roles);

            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var bearerToken = context.HttpContext.Request.Headers["Authorization"].ToString();
                var token = bearerToken.Split(" ").Last();
                var request = new Request { Filters = new [] { $"Token={token}" } };
                var tokenSession = await _tokenRepository.GetOneAsync(request);
                var role = _roles.Where(role => role == tokenSession.Role).FirstOrDefault();

                if (role is null)
                {
                    context.Result = new BadRequestObjectResult(new 
                    { 
                        Status = false, 
                        Message = _localizer["InvalidPermissions"].Value, 
                        Code = "InvalidPermissions" 
                    });

                    return;
                }
                
                await next();
            }

            #endregion
        }
    }
}