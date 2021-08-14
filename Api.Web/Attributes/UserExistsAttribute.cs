using System;
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Services.Services;
using Api.Web.Extensions;
using Api.Repository.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using ServiceStack.Redis;
using Api.Domain.Models;

namespace Api.Web.Attributes
{
    public class UserExistsAttribute : TypeFilterAttribute
    {
        public UserExistsAttribute() : base(typeof(UserExistsFilter)) {}

        private class UserExistsFilter : IAsyncActionFilter
        {
            private readonly IUserRepository _userRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;
            private readonly IRedisClientsManagerAsync _redisManager;

            public UserExistsFilter
            (
                IUserRepository userRepository,
                IStringLocalizer<SharedResources> localizer,
                IRedisClientsManagerAsync redisManager
            )
            {
                _userRepository = userRepository;
                _localizer = localizer;
                _redisManager = redisManager;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var user = await _userRepository.GetByIdAsync(id);

                    if (user is null) {
                        context.Result = new NotFoundObjectResult(new
                        {
                            Status = false,
                            Message = _localizer["UserNotFound"].Value,
                            Code = "UserNotFound" 
                        });
                        return;
                    }

                    var token = context.HttpContext.Request.Headers.ExtractJsonWebToken();
                    string role = token.SelectClaim("role");
                    string station = token.SelectClaim("station");

                    if (role == Roles.StationAdmin && station != user.StationId)
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            Status = false,
                            Code = "InvalidOperation",
                            Message = _localizer["InvalidOperation"].Value
                        });
                        return;
                    }

                    await using var redisClient = await _redisManager.GetCacheClientAsync();
                    await redisClient.SetAsync<User>
                        (
                            user.Id,
                            user,
                            expiresAt: DateTime.UtcNow.AddMinutes(10)
                        );

                    await next();
                }
                catch (FormatException)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        Status = false,
                        Message = _localizer["InvalidObjectId"].Value,
                        Code = "InvalidObjectId"
                    });
                    return;
                }
            }
        }
    }
}