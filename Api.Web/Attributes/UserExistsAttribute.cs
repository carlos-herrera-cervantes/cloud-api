using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Services.Services;
using Api.Web.Extensions;
using Api.Repository.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Attributes
{
    public class UserExistsAttribute : TypeFilterAttribute
    {
        public UserExistsAttribute() : base(typeof(UserExistsFilter)) {}

        private class UserExistsFilter : IAsyncActionFilter
        {
            private readonly IUserRepository _userRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;

            public UserExistsFilter(
                IUserRepository userRepository,
                IStringLocalizer<SharedResources> localizer
            )
                => (_userRepository, _localizer) = (userRepository, localizer);

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