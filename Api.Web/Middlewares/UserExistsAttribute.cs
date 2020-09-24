using System;
using System.Threading.Tasks;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Middlewares
{
    public class UserExistsAttribute : TypeFilterAttribute
    {
        public UserExistsAttribute() : base(typeof(UserExistsFilter)) {}

        private class UserExistsFilter : IAsyncActionFilter
        {
            private readonly IUserRepository _userRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;

            public UserExistsFilter(IUserRepository userRepository, IStringLocalizer<SharedResources> localizer)
                => (_userRepository, _localizer) = (userRepository, localizer);

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var user = await _userRepository.GetByIdAsync(id);

                    if (user is null) {
                        context.Result = new NotFoundObjectResult(new { Status = false, Message = _localizer["UserNotFound"].Value, Code = "UserNotFound" });
                        return;
                    }

                    await next();
                }
                catch (FormatException)
                {
                    context.Result = new BadRequestObjectResult(new { Status = false, Message = _localizer["InvalidObjectId"].Value, Code = "InvalidObjectId" });
                    return;
                }
            }
        }
    }
}