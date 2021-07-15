using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using MongoDB.Driver;

namespace Api.Web.Attributes
{
    public class RoleAttribute : TypeFilterAttribute
    {
        public RoleAttribute(string[] roles) : base(typeof(RoleFilter)) => Arguments = new object[] { roles };

        private class RoleFilter : IAsyncActionFilter
        {
            private readonly ITokenRepository _tokenRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;
            private readonly string[] _roles;

            public RoleFilter
            (
                ITokenRepository tokenRepository,
                IStringLocalizer<SharedResources> localizer,
                string[] roles
            )
                => (_tokenRepository, _localizer, _roles) = (tokenRepository, localizer, roles);

            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var byPass = _roles.Where(role => role == Roles.All).FirstOrDefault();

                if (byPass != null)
                {
                    await next();
                    return;
                }

                var token = context.HttpContext.Request.Headers.ExtractJsonWebToken();
                var tokenSession = await _tokenRepository
                    .GetOneAsync(Builders<AccessToken>.Filter.Where(t => t.Token == token));

                if (tokenSession is null)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        Status = false,
                        Message = _localizer["ExpiredToken"].Value,
                        Code = "ExpiredToken"
                    });

                    return;
                }

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