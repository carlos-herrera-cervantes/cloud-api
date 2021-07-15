using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Attributes
{
    public class SetPaginateAttribute : TypeFilterAttribute
    {
        public SetPaginateAttribute() : base(typeof(PaginateValidatorFilter)) { }

        private class PaginateValidatorFilter : IAsyncActionFilter
        {
            private readonly IStringLocalizer<SharedResources> _localizer;

            public PaginateValidatorFilter(IStringLocalizer<SharedResources> localizer)
                => _localizer = localizer;

            #region snippet_BeforeExecuted

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var clonedRequest = context.ActionArguments["request"] as ListResourceRequest;

                clonedRequest.Page = clonedRequest.Page > 0 ? clonedRequest.Page - 1 : clonedRequest.Page;
                clonedRequest.PageSize = clonedRequest.PageSize < 1 ? 10 : clonedRequest.PageSize;

                context.ActionArguments["request"] = clonedRequest;

                await next();
            }

            #endregion
        }
    }
}
